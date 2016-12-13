using System;
using System.Threading;
using System.Windows.Forms;
using FormUnifierDemo.Models.Enums;
using FormUnifierDemo.Views;
using LibreR.Controllers.Gui;
using LibreR.Controllers.Gui.WinForms;

namespace FormUnifierDemo {
    static class Index {
        public const TestType TestMode = TestType.StartWithWait;

        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            FormUnifier.Init<Parent, Wait>();

            switch (TestMode) {
                case TestType.StartWithWaitGeneric:
                    FormUnifier.StartWithWait<A>(() => Thread.Sleep(2000), null, new FormUnifierParams("Starting..."));
                    break;

                case TestType.StartWithWait:
                    FormUnifier.StartWithWait(() => {
                        Thread.Sleep(2000);
                        return FormUnifierResult.Create<A>();
                    }, "Starting...");
                    break;
            }

            Application.Run(FormUnifier.Container);
        }
    }
}
