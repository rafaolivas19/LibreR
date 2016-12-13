using System.Drawing;

namespace LibreR.Controllers.Gui.WinForms.Controls {
    public class ErrorNotification {
        public string Message { get; set; }
        public bool Fulfilled { get; set; }
        public Color BackColor { get; set; }

        public ErrorNotification() : this(false, null, Color.Empty) { }

        public ErrorNotification(bool fullfilled) : this(fullfilled, null, Color.Empty) { }

        public ErrorNotification(bool fullfilled, string message) : this(fullfilled, message, Color.Empty) { }

        public ErrorNotification(bool fullfilled, string message, Color backColor) {
            Message = message;
            Fulfilled = fullfilled;
            BackColor = backColor;
        }
    }
}
