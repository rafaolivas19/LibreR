using System;
using System.Threading;
using System.Windows.Forms;
using LibreR.Models;

namespace LibreR.Controllers.Gui.WinForms {
    public static class FormUnifier {
        /// <summary>
        /// Contains all the new forms to show
        /// </summary>
        public static Form Container { get; set; }
        /// <summary>
        /// Is shown when there is a process running before
        /// showing a new form
        /// </summary>
        public static Form Wait { get; set; }

        private static Form _active;

        /// <summary>
        /// This method has to be called at first.
        /// 
        /// Defines the form that will contain all the others and the form that
        /// will be shown as the waiting form.
        /// </summary>
        /// <typeparam name="TContainer"></typeparam>
        /// <typeparam name="TWait"></typeparam>
        /// <returns></returns>
        public static void Init<TContainer, TWait>() {
            Container = (Form)Activator.CreateInstance(typeof(TContainer));
            Wait = (Form)Activator.CreateInstance(typeof(TWait));
        }

        /// <summary>
        /// Shows a form of the specified type
        /// on the container. 
        /// 
        /// Previous form will be removed if there is one.
        /// 
        /// Parameters will be used on the form constructor.
        /// </summary>
        /// <typeparam name="TWindow"></typeparam>
        /// <param name="parameters"></param>
        public static void Start<TWindow>(params object[] parameters) {
            Start(typeof(TWindow), parameters);
        }

        /// <summary>
        /// Shows a form of the specified generic
        /// on the container showing the waiting form
        /// as long as the defined action takes to finish.
        /// 
        /// Previous form will be removed if there is one.
        /// 
        /// Parameters will be used on the form constructor.
        /// </summary>
        /// <typeparam name="TWindow"></typeparam>
        /// <param name="action"></param>
        /// <param name="formParameters"></param>
        /// <param name="waitParameters"></param>
        public static void StartWithWait<TWindow>(Action action, FormUnifierParams formParameters = null, FormUnifierParams waitParameters = null) {
            if (formParameters == null) formParameters = FormUnifierParams.Empty;
            if (waitParameters == null) waitParameters = FormUnifierParams.Empty;

            var waitThread = new Thread(() => Start(Wait.GetType(), waitParameters.Params));

            var formThread = new Thread(() => {
                waitThread.Join();
                action();
                Start(typeof(TWindow), formParameters.Params);
            });

            waitThread.Start();
            formThread.Start();
        }

        /// <summary>
        /// Shows the wait form while the specified functions is
        /// executed, at the end shows the form of the type specified
        /// by the result of the function
        /// </summary>
        /// <param name="function"></param>
        /// <param name="waitParams"></param>
        public static void StartWithWait(Func<FormUnifierResult> function, params object[] waitParams) {
            var waitThread = new Thread(() => Start(Wait.GetType(), waitParams));

            var formThread = new Thread(() => {
                waitThread.Join();
                var result = function();

                Start(result.ViewType, result.Params);
            });

            waitThread.Start();
            formThread.Start();
        }

        public static void Start(Type type, params object[] parameters) {
            if (Container.InvokeRequired) {
                Container.BeginInvoke(new Action(() => Start(type, parameters)));
                return;
            }

            if (Container == null) throw new LibrerException("FormUnifier hasn't been initialized yet");
            if (!type.IsSubclassOf(typeof(Form))) throw new LibrerException($"{type} has to be a Form");

            var old = _active;
            _active = (Form)Activator.CreateInstance(type, parameters);
            _active.TopLevel = false;
            _active.FormBorderStyle = FormBorderStyle.None;
            _active.Anchor = AnchorStyles.None;
            _active.Visible = true;

            Container.Controls.Add(_active);

            if (old != null) Remove(old);
        }

        private static void Remove(Form form) {
            if (Container.InvokeRequired) {
                Container.BeginInvoke(new Action(() => Remove(form)));
                return;
            }

            Container.Controls.Remove(form);
            form.Close();
        }
    }
}
