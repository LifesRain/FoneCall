// FoneCall, a scrcpy UI. Copyright (c) 2026 Lifesain
// Licensed under the PolyForm Noncommercial License 1.0.0.
// You may not use this file except in compliance with the License.
// You may obtain a copy of the License at:
//     <https://polyformproject.org/licenses/noncommercial/1.0.0>
// SPDX-License-Identifier: PolyForm-Noncommercial-1.0.0

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Dapper;

namespace FoneCall
{
    internal class MiigrateDB
    {
        private const int LatestSchemaVersion = 1;

        public static bool MigrateChk()
        {
            bool migrated = false;
            using (var connection = DBC.SQLyt())
            {
                connection.Open();

                int currentVersion = connection.ExecuteScalar<int>( "PRAGMA user_version;" );

                if (currentVersion >= LatestSchemaVersion)
                    return migrated;

                // Create a timestamped backup before modifying the user's database
                if (File.Exists( Paths.AppDB ))
                {
                    string backupPath = $"{Paths.AppDB}.v{currentVersion}" +
                                        $"_{DateTime.Now:yyyyMMddHHmmss}.bak";
                    File.Copy( Paths.AppDB, backupPath, overwrite: true );
                }

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        if (currentVersion < 1)
                        {
                            ApplyVersion1( connection, transaction );
                            connection.Execute( "PRAGMA user_version = 1;", transaction: transaction );
                            LogR.Norm( $"Database migrated to version 1." );
                            migrated = true;
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            return migrated;
        }

        private static void ApplyVersion1( SQLiteConnection conn, SQLiteTransaction tx )
        {
            // 1. Rebuild ExeSpec without the deprecated RecID column (if ExeSpec exists)
            int exeSpecExists = conn.ExecuteScalar<int>(
                "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='ExeSpec';",
                transaction: tx );

            if (exeSpecExists > 0)
            {
                conn.Execute( @"
                    CREATE TABLE IF NOT EXISTS ExeSpec_New (
                        ScrcpyFQN TEXT,
                        Arguments TEXT
                    );

                    INSERT INTO ExeSpec_New (ScrcpyFQN, Arguments)
                    SELECT ScrcpyFQN, Arguments FROM ExeSpec;

                    DROP TABLE ExeSpec;
                    ALTER TABLE ExeSpec_New RENAME TO ExeSpec;
                ", transaction: tx );
            }

            // 2. Create Options and migrate existing data from Args (if Args exists)
            conn.Execute( @"
                CREATE TABLE IF NOT EXISTS Options (
                    Role TEXT DEFAULT 'ARG',
                    Used TEXT DEFAULT 'false',
                    Option TEXT,
                    Desc TEXT
                );
            ", transaction: tx );

            int argsTableExists = conn.ExecuteScalar<int>(
                "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='Args';",
                transaction: tx );

            if (argsTableExists > 0)
            {
                conn.Execute( @"
                    INSERT INTO Options (Role, Option, Desc)
                    SELECT Role, Arg, Explain FROM Args;

                    DROP TABLE Args;
                ", transaction: tx );
            }
        }
    }
}
