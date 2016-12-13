namespace FormUnifierDemo.Views {
    partial class Wait {
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
            this.label1 = new System.Windows.Forms.Label();
            this.TextLbl = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Image = global::FormUnifierDemo.Properties.Resources.Wait;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(560, 423);
            this.label1.TabIndex = 0;
            // 
            // TextLbl
            // 
            this.TextLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F);
            this.TextLbl.Location = new System.Drawing.Point(15, 45);
            this.TextLbl.Name = "TextLbl";
            this.TextLbl.Size = new System.Drawing.Size(557, 34);
            this.TextLbl.TabIndex = 1;
            this.TextLbl.Text = "TEXT";
            this.TextLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Wait
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 441);
            this.Controls.Add(this.TextLbl);
            this.Controls.Add(this.label1);
            this.Name = "Wait";
            this.Text = "Wait";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label TextLbl;
    }
}