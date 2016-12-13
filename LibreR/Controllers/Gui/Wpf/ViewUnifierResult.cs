using System;
using System.Windows.Controls;

namespace LibreR.Controllers.Gui.Wpf {
    public class ViewUnifierResult {
        public Type ViewType { get; private set; }
        public object[] Params { get; private set; }

        public ViewUnifierResult(Type type, params object[] parameters) {
            if (!(type.IsSubclassOf(typeof(Page)))) throw new ApplicationException("Received type is not a Page");

            ViewType = type;
            Params = parameters;
        }

        public static ViewUnifierResult Create<T>(params object[] parameters) {
            if (!typeof(T).IsSubclassOf(typeof(Page))) throw new ApplicationException("Received type is not a Page");

            return new ViewUnifierResult(typeof(T), parameters);
        }
    }
}