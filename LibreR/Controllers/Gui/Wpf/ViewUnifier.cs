using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace LibreR.Controllers.Gui.Wpf {
    public static class ViewUnifier {
        public static System.Windows.Window Container;
        public static Page Wait;
        public static Page PreviousContent;

        public static event Action<Type, Type, object[]> LoadingView;

        public static void Start<TWindow>(params object[] parameters) {
            Start(typeof(TWindow), parameters);
        }

        public static void Start(Type type, params object[] parameters) {
            Start(type, true, parameters);
        }

        public static void Start(Type type, bool savePrevious, params object[] parameters) {
            Container.Dispatcher.SafelyInvoke((() => {
                var parametersCopy = new object[parameters.Length];
                Array.Copy(parameters, parametersCopy, parameters.Length);

                LoadingView?.Invoke(Container.Content.GetType(), type, parametersCopy);

                if (savePrevious && Container.Content.GetType() != Wait.GetType() && Container.Content is Page)
                    PreviousContent = (Page)Container.Content;

                if (Container == null) throw new ApplicationException("Container hasn't been defined yet");
                if (!type.IsSubclassOf(typeof(Page))) throw new ApplicationException($"{type} has to be a Page");

                Container.Content = Activator.CreateInstance(type, parameters);
            }));
        }

        public static void StartWithWait(Type type, Action action, ViewUnifierParams formParameters = null, ViewUnifierParams waitParameters = null) {
            if (formParameters == null) formParameters = ViewUnifierParams.Empty;
            if (waitParameters == null) waitParameters = ViewUnifierParams.Empty;

            var waitThread = new Thread(() => Start(Wait.GetType(), waitParameters.Params));

            var formThread = new Thread(() => {
                waitThread.Join();
                action();
                Start(type, formParameters.Params);
            });

            waitThread.SetApartmentState(ApartmentState.STA);
            waitThread.Start();
            formThread.Start();
        }

        public static void StartWithWait<TWindow>(Action action, ViewUnifierParams formParameters = null, ViewUnifierParams waitParameters = null) {
            StartWithWait(typeof(TWindow), action, formParameters);
        }

        public static void StartWithWait(Func<ViewUnifierResult> function, params object[] waitParams) {
            var waitThread = new Thread(() => Start(Wait.GetType(), waitParams));

            var formThread = new Thread(() => {
                waitThread.Join();
                var result = function();

                Start(result.ViewType, result.Params);
            });

            waitThread.Start();
            formThread.Start();
        }

        public static void BackToPrevious() {
            if (!Container.Dispatcher.CheckAccess()) {
                Container.Dispatcher.BeginInvoke(new Action(BackToPrevious));
                return;
            }

            LoadingView?.Invoke(Container.Content.GetType(), PreviousContent.GetType(), new object[] { });
            Container.Content = PreviousContent;
        }

        public static void BackToPreviousWithWait(Action action, ViewUnifierParams waitParameters = null) {
            if (waitParameters == null) waitParameters = ViewUnifierParams.Empty;

            var waitThread = new Thread(() =>
            Start(Wait.GetType(), false, waitParameters.Params));

            var formThread = new Thread(() => {
                waitThread.Join();
                action();

                BackToPrevious();
            });

            waitThread.SetApartmentState(ApartmentState.STA);
            waitThread.Start();
            formThread.Start();
        }
    }
}