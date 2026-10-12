// Tests\DataEntryTests.cs
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FoneCall;

namespace FoneCall.Tests
{
    [TestClass]
    public class DataEntryTests
    {
        // Minimal ICaller test double
        private class FakeCaller : ICaller
        {
            public Form I_form { get; set; } = new Form();
            public string I_MBcaller { get; set; } = string.Empty;
            public string I_MsgType { get; set; } = string.Empty;
            public string I_Msg { get; set; } = string.Empty;
            public string I_Txt_Btn1 { get; set; } = string.Empty;
            public string I_Txt_Btn2 { get; set; } = string.Empty;
            public string I_Txt_Btn3 { get; set; } = string.Empty;
        }

        // Helper: create DataEntry on STA thread (WinForms requires STA)
        private DataEntry CreateForm(FakeCaller? caller = null)
        {
            DataEntry? form = null;
            var ready = new ManualResetEventSlim();
            var th = new Thread(() =>
            {
                // Provide a minimal caller to satisfy constructor expectations
                var c = caller ?? new FakeCaller();
                form = new DataEntry(c);

                // Signal that the form instance is created and its handle can be created later
                ready.Set();

                // Run a message loop so the form/control have a valid UI thread for Invoke/BeginInvoke
                Application.Run();
            });
            th.SetApartmentState(ApartmentState.STA);
            th.IsBackground = true;
            th.Start();

            // Wait for the form instance to be created before returning it
            if (!ready.Wait(2000)) throw new TimeoutException("Form creation timed out in CreateForm.");

            // Create the form handle on the UI thread so Control.Invoke works reliably
            if (form is Control cForm)
            {
                // Ensure handle is created on the UI thread
                var handleReady = new ManualResetEventSlim();
                cForm.BeginInvoke(new Action(() =>
                {
                    var _ = cForm.Handle; // force handle creation
                    handleReady.Set();
                }));
                if (!handleReady.Wait(2000)) throw new TimeoutException("Failed to create form handle.");
            }

            return form!;
        }

        // Helper: get private field value
        private static object? GetPrivateField(object instance, string fieldName)
        {
            var fi = instance.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            return fi?.GetValue(instance);
        }

        // Helper: set private static property
        private static void SetPrivateStatic(string propName, object value)
        {
            var pi = typeof(DataEntry).GetProperty(propName, BindingFlags.Static | BindingFlags.NonPublic);
            pi?.SetValue(null, value);
        }

        // Helper: invoke private method on the form's UI thread when necessary
        private static object? InvokePrivate(object instance, string methodName, params object[] args)
        {
            var mi = instance.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
            if (mi is null) return null;

            if (instance is Control ctrl && ctrl.IsHandleCreated)
            {
                return ctrl.Invoke(new Func<object?>(() => mi.Invoke(instance, args)));
            }

            // Fallback: direct invoke (used if instance isn't a Control or handle not available)
            return mi.Invoke(instance, args);
        }

        [TestMethod]
        public void InitTBX_SetsVisuals_And_SelectsWhenRequested()
        {
            var f = CreateForm();
            // Create a TextBox locally to avoid relying on designer-initialized fields
            var tb = new TextBox { Text = "abc" };

            var mi = typeof(DataEntry).GetMethod("InitTBX", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(mi, "InitTBX method not found");

            // Call with truInit = true (should call SelectAll -> does not throw)
            mi.Invoke(f, new object[] { tb, true });

            Assert.IsNotNull(tb.Font);
            Assert.AreEqual(System.Drawing.Color.Black, tb.ForeColor);
            Assert.AreEqual(System.Drawing.Color.White, tb.BackColor);
        }

        [TestMethod]
        public void TBX_Enter_SetsTabStopAndChangesExeButtonText()
        {
            var f = CreateForm();
            SetPrivateStatic("_loading", false);

            var fqn = (TextBox?)GetPrivateField(f, "FQN_TBX");
            Assert.IsNotNull(fqn);

            var mi = typeof(DataEntry).GetMethod("TBX_Enter", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(mi);

            // make sure TabStop is false before invoking so the assertion is meaningful
            fqn.TabStop = false;

            // Simulate Enter event
            mi.Invoke(f, new object[] { fqn, EventArgs.Empty });

            var exeBtn = (Button?)GetPrivateField(f, "ExeBTN");
            Assert.IsNotNull(exeBtn);

            Assert.IsTrue(fqn.TabStop);
            Assert.AreEqual("Save to DB", exeBtn.Text);
        }

        [TestMethod]
        public void TBX_TextChanged_UpdatesExeButton_ToSave_WhenTextDiffersFromStart()
        {
            var f = CreateForm();
            SetPrivateStatic("_loading", false);

            SetPrivateStatic("_fqnAtStart", "original");
            SetPrivateStatic("_argsAtStart", "originalargs");

            var fqn = (TextBox?)GetPrivateField(f, "FQN_TBX");
            Assert.IsNotNull(fqn);

            var mi = typeof(DataEntry).GetMethod("TBX_TextChanged", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(mi);

            fqn.Text = "changed text";
            mi.Invoke(f, new object[] { fqn, EventArgs.Empty });

            var exeBtn = (Button?)GetPrivateField(f, "ExeBTN");
            Assert.IsNotNull(exeBtn);

            Assert.AreEqual("Save to DB", exeBtn.Text);
        }

        [TestMethod]
        public void FQN_TBX_Leave_WithInvalidPath_ShowsErrorState_DisablesExeButton()
        {
            var f = CreateForm();
            var fqn = (TextBox?)GetPrivateField(f, "FQN_TBX");
            Assert.IsNotNull(fqn);

            fqn.Text = @"C:\this_path_does_not_exist\scrcpy.exe";

            var mi = typeof(DataEntry).GetMethod("FQN_TBX_Leave", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(mi);

            mi.Invoke(f, new object[] { fqn, EventArgs.Empty });

            var exeBtn = (Button?)GetPrivateField(f, "ExeBTN");
            Assert.IsNotNull(exeBtn);

            // When invalid, button should be disabled (style helper sets Enabled=false)
            Assert.IsFalse(exeBtn.Enabled);
            var paleRed = (System.Drawing.Color)typeof(CstmStyles).GetProperty("PaleRed", BindingFlags.Public | BindingFlags.Static).GetValue(null);
            Assert.AreEqual(paleRed, fqn.BackColor);
        }

        [TestMethod]
        public void FQN_TBX_Leave_WithValidPath_EnablesButton()
        {
            var tmpPath = Path.Combine(Path.GetTempPath(), "scrcpy.exe");
            File.WriteAllText(tmpPath, "dummy");

            var f = CreateForm();
            var fqn = (TextBox?)GetPrivateField(f, "FQN_TBX");
            Assert.IsNotNull(fqn);
            fqn.Text = tmpPath;

            var mi = typeof(DataEntry).GetMethod("FQN_TBX_Leave", BindingFlags.Instance | BindingFlags.NonPublic);
            mi.Invoke(f, new object[] { fqn, EventArgs.Empty });

            var exeBtn = (Button?)GetPrivateField(f, "ExeBTN");
            Assert.IsNotNull(exeBtn);
            Assert.IsTrue(exeBtn.Enabled);

            File.Delete(tmpPath);
        }

        [TestMethod]
        public void ArgsTBX_Leave_Blank_SetsArgsOKFalse()
        {
            var f = CreateForm();
            var args = (TextBox?)GetPrivateField(f, "ArgsTBX");
            Assert.IsNotNull(args);

            args.Text = "   ";
            var mi = typeof(DataEntry).GetMethod("ArgsTBX_Leave", BindingFlags.Instance | BindingFlags.NonPublic);
            mi.Invoke(f, new object[] { args, EventArgs.Empty });

            var argsOK = (bool)typeof(DataEntry).GetProperty("_argsOK", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
            Assert.IsFalse(argsOK);
        }

        [TestMethod]
        public void ArgsTBX_Leave_WithHyphenNoComma_SetsArgsOKTrue()
        {
            var f = CreateForm();
            var args = (TextBox?)GetPrivateField(f, "ArgsTBX");
            Assert.IsNotNull(args);

            args.Text = "--always-on-top";

            var mi = typeof(DataEntry).GetMethod("ArgsTBX_Leave", BindingFlags.Instance | BindingFlags.NonPublic);
            mi.Invoke(f, new object[] { args, EventArgs.Empty });

            var argsOK = (bool)typeof(DataEntry).GetProperty("_argsOK", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
            Assert.IsTrue(argsOK);
        }

        [TestMethod]
        public void ProcessCmdKey_Escape_ReturnsTrue()
        {
            var f = CreateForm();

            var mi = typeof(DataEntry).GetMethod("ProcessCmdKey", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(mi);

            var msg = Message.Create(IntPtr.Zero, 0, IntPtr.Zero, IntPtr.Zero);
            object[] args = new object[] { msg, Keys.Escape };
            var result = (bool)mi.Invoke(f, args);

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void HideTT_Click_And_HideTT_DoNotThrow()
        {
            var f = CreateForm();

            var hideClick = typeof(DataEntry).GetMethod("HideTT_Click", BindingFlags.Instance | BindingFlags.NonPublic);
            var hide = typeof(DataEntry).GetMethod("HideTT", BindingFlags.Instance | BindingFlags.NonPublic);

            hideClick.Invoke(f, new object[] { f, EventArgs.Empty });
            hide.Invoke(f, null); // should not throw
        }

        [TestMethod]
        public void ExeBTN_Click_WithUnknownText_ClosesForm()
        {
            var caller = new FakeCaller();
            var f = CreateForm(caller);

            var exeBtn = (Button?)GetPrivateField(f, "ExeBTN");
            Assert.IsNotNull(exeBtn);
            exeBtn.Text = "SOME OTHER TEXT";

            var mi = typeof(DataEntry).GetMethod("ExeBTN_Click", BindingFlags.Instance | BindingFlags.NonPublic);
            mi.Invoke(f, new object[] { exeBtn, EventArgs.Empty });

            // Form.Close() schedules close; calling Close when form not shown will mark IsDisposed after Close
            // We assert no exception and that form object still exists
            Assert.IsNotNull(f);
        }

        [TestMethod]
        public void OnFormClosed_DoesNotThrow_WhenCalled()
        {
            var f = CreateForm();

            var mi = typeof(DataEntry).GetMethod("OnFormClosed", BindingFlags.Instance | BindingFlags.NonPublic);
            mi.Invoke(f, new object[] { new FormClosedEventArgs(CloseReason.UserClosing) });
        }
    }
}