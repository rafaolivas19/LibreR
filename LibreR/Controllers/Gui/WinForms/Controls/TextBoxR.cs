using System;
using System.Drawing;
using System.Windows.Forms;
using LibreR.Models.Enums;

namespace LibreR.Controllers.Gui.WinForms.Controls {
    public partial class TextBoxR : TextBox {
        public string PlaceholderText { get; set; }
        public override string Text {
            get { return IsPlaceholderActive ? string.Empty : base.Text; }
            set { base.Text = value; }
        }
        public bool IsPlaceholderActive {
            get { return _isPlaceholderActive; }
            set {
                _isPlaceholderActive = value;

                if (_isPlaceholderActive) {
                    Text = _placeholderText;
                    ForeColor = _placeholderForeColor;
                    PasswordChar = '\0';
                    SetCarentAt(CaretPosition.Start);
                }
                else {
                    ForeColor = _foreColor;
                    SetCarentAt(CaretPosition.End);
                }
            }
        }
        public bool IsShowingError {
            get { return _isShowingError; }
            set {
                _isShowingError = value;
                Width += value ? -15 : 15;
            }
        }

        public string VisibleText => base.Text;

        private bool _isPlaceholderActive;
        private bool _isShowingError;
        private Color _foreColor;
        private bool _skip = true;
        private char _passwordChar;
        private Color _placeholderForeColor;
        private string _placeholderText;
        private TextOrientation _placeholderOrientation;
        private ErrorProvider _errorProvider;
        private Color _backColor;
        private Color _errorBackColor = Color.Empty;
        private Func<ErrorNotification> _errorFunc;

        public TextBoxR() {
            InitializeComponent();

            TextChanged += OnTextChanged;
            Enter += OnEnter;
            MouseClick += OnMouseClick;
            KeyUp += OnKeyPressed;
        }

        public void SetPlaceHolder(string text) {
            SetPlaceholder(text, Color.Gray);
        }

        public void SetPlaceholder(string text, Color foreColor) {
            _placeholderForeColor = foreColor;
            _placeholderOrientation =
                TextAlign == HorizontalAlignment.Right ? TextOrientation.RightToLeft : TextOrientation.LeftToRight; ;
            _placeholderText = text;
            _foreColor = ForeColor;
            _passwordChar = PasswordChar;

            IsPlaceholderActive = true;
        }

        /// <summary>
        /// Notifies error when condition is fulfilled.
        /// 
        /// Function must return:
        ///     bool Fulfilled
        ///     string Message
        ///     Color BackColor = Gray
        /// </summary>
        /// <param name="function"></param>
        public void NotifyErrorWhen(Func<ErrorNotification> function) {
            _errorFunc = function;
        }

        public void NotifyError(string message) {
            NotifyError(message, Color.Pink);
        }

        public void NotifyError(string message, Color errorBackColor) {
            _errorProvider = _errorProvider ?? new ErrorProvider();
            _backColor = BackColor;
            _errorBackColor = _errorBackColor == Color.Empty ? errorBackColor : _errorBackColor;

            _errorProvider.SetError(this, message);
            BackColor = errorBackColor;

            IsShowingError = true;
        }

        private void SetCarentAt(CaretPosition position) {
            SelectionStart = _placeholderOrientation == TextOrientation.LeftToRight
                ? position == CaretPosition.Start
                    ? 0
                    : TextLength
                : position == CaretPosition.Start
                    ? TextLength
                    : 0;
        }

        private void OnKeyPressed(object sender, KeyEventArgs e) {
            if (IsPlaceholderActive) SetCarentAt(CaretPosition.Start);
        }

        private void OnMouseClick(object sender, MouseEventArgs mouseEventArgs) {
            if (IsPlaceholderActive) SetCarentAt(CaretPosition.Start);
        }

        private void OnEnter(object sender, EventArgs eventArgs) {
            if (IsPlaceholderActive) SetCarentAt(CaretPosition.Start);
        }

        private void OnTextChanged(object sender, EventArgs eventArgs) {
            if (_skip) {
                _skip = false;
            }
            else {
                if (IsPlaceholderActive) {
                    if (!VisibleText.Contains(_placeholderText)) {
                        IsPlaceholderActive = false;
                        return;
                    }

                    Text = VisibleText.Length > 0
                        ? VisibleText.Substring(0, VisibleText.Length - _placeholderText.Length)
                        : string.Empty;

                    PasswordChar = _passwordChar;
                    IsPlaceholderActive = false;
                }
                else if (Text == string.Empty) {
                    _skip = true;
                    IsPlaceholderActive = true;
                }
            }


            if (IsShowingError) {
                _errorProvider.Clear();
                BackColor = _backColor;
                IsShowingError = false;
            }

            if (_errorFunc != null) {
                var aux = _errorFunc();

                if (!aux.Fulfilled) return;

                if (aux.BackColor == Color.Empty) NotifyError(aux.Message);
                else NotifyError(aux.Message, aux.BackColor);
            }

        }
    }
}
