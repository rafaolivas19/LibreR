using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Documents;
using System.Windows.Forms;

namespace LibreR.Controllers {
    public static class Keyboard {
        public static event Action<string> BarcodeInserted;

        private static readonly StringBuilder Barcode = new StringBuilder();
        private static EventHandler<KeyEventArgs> _handler;
        private static event EventHandler<KeyEventArgs> KeyPressed;
        private const int WhKeyboardLl = 13;
        private const int WmKeydown = 0x0100;
        private static readonly User32.LowLevelKeyboardProc Proc = HookCallback;
        private static IntPtr _hookId = IntPtr.Zero;
        private static object _sender;

        /// <summary>
        /// Starts listening to keys. This won't work unless App object is initialized (in WPF you can try "((App)Current).InitializeComponent();" before calling this method)
        /// </summary>
        /// <param name="action"></param>
        /// <param name="sender"></param>
        public static void ListenKeys(EventHandler<KeyEventArgs> action = null, object sender = null) {
            if (action == null) action = ListeningKeys;

            _sender = sender;
            if (_handler != null) KeyPressed -= _handler;
            _handler = action;
            KeyPressed += _handler;

            using (var curProcess = Process.GetCurrentProcess())
            using (var curModule = curProcess.MainModule)
                _hookId = User32.SetWindowsHookEx(WhKeyboardLl,
                    Proc, User32.GetModuleHandle(curModule.ModuleName), 0);
        }

        public static void UnlistenKeys() {
            KeyPressed -= _handler;
            User32.UnhookWindowsHookEx(_hookId);
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam) {
            if (nCode < 0 || wParam != (IntPtr)WmKeydown)
                return User32.CallNextHookEx(_hookId, nCode, wParam, lParam);

            var vkCode = Marshal.ReadInt32(lParam);
            KeyPressed(_sender, new KeyEventArgs((Keys)vkCode));

            return User32.CallNextHookEx(_hookId, nCode, wParam, lParam);
        }

        private static void ListeningKeys(object sender, KeyEventArgs args) {
            if (args.KeyData == Keys.Enter) {
                BarcodeInserted?.Invoke(Barcode.ToString());
                Barcode.Clear();
            }
            else if ((args.KeyValue >= 48 && args.KeyValue <= 57) || (args.KeyValue >= 65 && args.KeyValue <= 90))
                Barcode.Append(((char)args.KeyValue).ToString(CultureInfo.InvariantCulture));
        }
    }
}
