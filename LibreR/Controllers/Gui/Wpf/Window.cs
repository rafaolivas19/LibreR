using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Windows.Interop;
using LibreR.Models.Enums;

namespace LibreR.Controllers.Gui.Wpf {
    public static class Window {
        public static IntPtr GetHandle(this System.Windows.Window window) {
            return new WindowInteropHelper(window).Handle;
        }
    }
}
