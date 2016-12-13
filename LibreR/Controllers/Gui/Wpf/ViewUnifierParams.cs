namespace LibreR.Controllers.Gui.Wpf {
    public class ViewUnifierParams {
        public static ViewUnifierParams Empty => _empty;

        public object[] Params { get; private set; }
        private static readonly ViewUnifierParams _empty = new ViewUnifierParams { Params = new object[] { } };

        public ViewUnifierParams(params object[] parameters) {
            Params = parameters;
        }
    }
}
