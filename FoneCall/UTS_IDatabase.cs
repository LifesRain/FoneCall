// FoneCall, a scrcpy UI. Copyright (c) 2026 Lifesain
// Licensed under the PolyForm Noncommercial License 1.0.0.
// You may not use this file except in compliance with the License.
// You may obtain a copy of the License at:
//     <https://polyformproject.org/licenses/noncommercial/1.0.0>
// SPDX-License-Identifier: PolyForm-Noncommercial-1.0.0

namespace FoneCall
{
    internal interface UTS_IDatabase
    {
        string GetVer();
        ExeSpec GetExeSpec();
        bool CheckArg( string name );
        void SaveExeSpec( ExeSpec spec );
    }
}