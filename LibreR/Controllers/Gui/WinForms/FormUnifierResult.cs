using System;
using System.Windows.Forms;
using LibreR.Models;

namespace LibreR.Controllers.Gui.WinForms {
    public class FormUnifierResult {
        public Type ViewType { get; private set; }
        public object[] Params { get; private set; }

        public FormUnifierResult(Type type, params object[] parameters) {
            if (!(type.IsSubclassOf(typeof(Form)))) throw new LibrerException("Received type is not a form");

            ViewType = type;
            Params = parameters;
        }

        public static FormUnifierResult Create<T>(params object[] parameters) {
            if (!typeof(T).IsSubclassOf(typeof(Form))) throw new LibrerException("Received type is not a form");

            return new FormUnifierResult(typeof(T), parameters);
        }
    }
}
