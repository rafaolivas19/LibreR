using System.Windows.Forms;

namespace FormUnifierDemo.Views {
    public partial class Wait : Form {
        public Wait() : this("") { }

        public Wait(string message) {
            InitializeComponent();
            TextLbl.Text = message;
        }
    }
}
