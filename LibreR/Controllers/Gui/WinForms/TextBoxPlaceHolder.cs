using System;
using System.Drawing;
using System.Windows.Forms;
using LibreR.Models.Enums;

namespace LibreR.Controllers.Gui.WinForms {
    public class TextBoxPlaceHolder {
        public string Text { get; set; }
        public TextBox Box { get; set; }
        public Color ForeColor { get; set; }
        public bool Active {
            get { return _active; }
            set {
                _active = value;

                if (_active) {
                    Box.Text = Text;
                    Box.ForeColor = ForeColor;
                    Box.PasswordChar = '\0';
                    SetCarentAt(CaretPosition.Start);
                }
                else {
                    Box.ForeColor = _oriForeColor;
                    SetCarentAt(CaretPosition.End);
                }
            }
        }
        public TextOrientation Orientation { get; set; }

        private readonly Color _oriForeColor;
        private bool _skip;
        private bool _active;
        private char _passwordChar;

        public TextBoxPlaceHolder(TextBox box, string text) : this(box, text, Color.Gray) { }

        public TextBoxPlaceHolder(TextBox box, string text, Color foreColor) : this(box, text, foreColor, TextOrientation.LeftToRight) { }

        public TextBoxPlaceHolder(TextBox box, string text, TextOrientation orientation) : this(box, text, Color.Gray, orientation) { }

        public TextBoxPlaceHolder(TextBox box, string text, Color foreColor, TextOrientation orientation) {
            Box = box;
            Text = text;
            ForeColor = foreColor;
            Orientation = orientation;

            _oriForeColor = box.ForeColor;
            _passwordChar = box.PasswordChar;

            box.Text = text;
            box.ForeColor = ForeColor;
            SetCarentAt(CaretPosition.Start);
            box.PasswordChar = '\0';

            Active = true;

            box.TextChanged += OnTextChanged;
            box.Enter += OnEnter;
            box.MouseClick += OnMouseClick;
            box.KeyUp += OnKeyPressed;
        }

        private void SetCarentAt(CaretPosition position) {
            Box.SelectionStart =
                Orientation == TextOrientation.LeftToRight
                    ? position == CaretPosition.Start
                        ? 0
                        : Box.TextLength
                    : position == CaretPosition.Start
                        ? Box.TextLength
                        : 0;
        }

        private void OnKeyPressed(object sender, KeyEventArgs e) {
            if (Active) SetCarentAt(CaretPosition.Start);
        }

        private void OnMouseClick(object sender, MouseEventArgs mouseEventArgs) {
            if (Active) SetCarentAt(CaretPosition.Start);
        }

        private void OnEnter(object sender, EventArgs eventArgs) {
            if (Active) SetCarentAt(CaretPosition.Start);
        }

        private void OnTextChanged(object sender, EventArgs eventArgs) {
            if (_skip) {
                _skip = false;
                return;
            }

            if (Active) {
                if (!Box.Text.Contains(Text)) {
                    Active = false;
                    return;
                }

                Box.Text = Box.TextLength > 0 ?
                    Box.Text.Substring(0, Box.TextLength - Text.Length) :
                    string.Empty;

                Box.PasswordChar = _passwordChar;
                Active = false;
            }
            else if (Box.Text == string.Empty) {
                _skip = true;
                Active = true;
            }
        }
    }
}
