namespace TextBoxPlaceHolderDemo {
    partial class MainView {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.TestTbx = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // TestTbx
            // 
            this.TestTbx.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F);
            this.TestTbx.Location = new System.Drawing.Point(12, 34);
            this.TestTbx.Name = "TestTbx";
            this.TestTbx.Size = new System.Drawing.Size(260, 38);
            this.TestTbx.TabIndex = 0;
            // 
            // MainView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.ClientSize = new System.Drawing.Size(284, 112);
            this.Controls.Add(this.TestTbx);
            this.Name = "MainView";
            this.Text = "Main";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TestTbx;
    }
}

