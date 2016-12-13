using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using LibreR.Models.Enums;

namespace LibreR.Controllers.Gui.WinForms {
    public static class TextBoxPlaceHolderExtensions {
        private static readonly Dictionary<TextBox, TextBoxPlaceHolder> _binderTextBoxPlaceHolder =
            new Dictionary<TextBox, TextBoxPlaceHolder>();

        public static void SetPlaceHolder(this TextBox box, string text) {
            _binderTextBoxPlaceHolder[box] = new TextBoxPlaceHolder(box, text);
        }

        public static void SetPlaceHolder(this TextBox box, string text, Color foreColor) {
            _binderTextBoxPlaceHolder[box] = new TextBoxPlaceHolder(box, text, foreColor);
        }

        public static void SetPlaceHolder(this TextBox box, string text, TextOrientation orientation) {
            _binderTextBoxPlaceHolder[box] = new TextBoxPlaceHolder(box, text, orientation);
        }

        public static void SetPlaceHolder(this TextBox box, string text, Color foreColor, TextOrientation orientation) {
            _binderTextBoxPlaceHolder[box] = new TextBoxPlaceHolder(box, text, foreColor, orientation);
        }

        public static TextBoxPlaceHolder GetPlaceholder(this TextBox box) {
            return !_binderTextBoxPlaceHolder.ContainsKey(box) ? null : _binderTextBoxPlaceHolder[box];
        }

        public static bool IsPlaceholderActive(this TextBox box) {
            return _binderTextBoxPlaceHolder.ContainsKey(box) && _binderTextBoxPlaceHolder[box].Active;
        }

        public static bool IsEmptyOrPlaceholderActive(this TextBox box) {
            return string.IsNullOrEmpty(box.Text) || box.IsPlaceholderActive();
        }

        public static string GetText(this TextBox box) {
            return box.IsPlaceholderActive() ? string.Empty : box.Text;
        }
    }
}
