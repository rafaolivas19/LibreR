using System.Threading;
using FormUnifierDemo.Models.Enums;
using LibreR.Controllers.Gui.WinForms;

namespace FormUnifierDemo.Views {
    public partial class B : Parent {
        public B() {
            InitializeComponent();
        }

        private void OpenBtn_Click(object sender, System.EventArgs e) {
            switch (Index.TestMode) {
                case TestType.StartWithWaitGeneric:
                    FormUnifier.StartWithWait<A>(() => Thread.Sleep(2000));
                    break;

                case TestType.StartWithWait:
                    FormUnifier.StartWithWait(() => {
                        Thread.Sleep(2000);
                        return FormUnifierResult.Create<A>();
                    }, "Waiting for A...");
                    break;
            }
        }
    }
}
