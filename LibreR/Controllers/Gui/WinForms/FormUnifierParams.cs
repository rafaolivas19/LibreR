namespace LibreR.Controllers.Gui.WinForms {
    public class FormUnifierParams {
        public static FormUnifierParams Empty => _empty;

        public object[] Params { get; private set; }
        private static readonly FormUnifierParams _empty = new FormUnifierParams { Params = new object[] { } };

        public FormUnifierParams(params object[] parameters) {
            Params = parameters;
        }
    }
}
