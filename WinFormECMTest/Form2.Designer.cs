using System.Windows.Forms;

namespace WinFormECMTest
{
    partial class Form2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Button buttonOpenFile;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.buttonOpenFile = new Button();
            this.comboBoxFileType = new ComboBox();
            this.SuspendLayout();

            // buttonOpenFile
            this.buttonOpenFile.Location = new System.Drawing.Point(12, 12);
            this.buttonOpenFile.Name = "buttonOpenFile";
            this.buttonOpenFile.Size = new System.Drawing.Size(100, 30);
            this.buttonOpenFile.Text = "Open File";
            this.buttonOpenFile.Click += new System.EventHandler(this.buttonOpenFile_Click);

            // comboBoxFileType
            this.comboBoxFileType.Location = new System.Drawing.Point(120, 12);
            this.comboBoxFileType.Size = new System.Drawing.Size(200, 30);
            this.comboBoxFileType.DropDownStyle = ComboBoxStyle.DropDownList;

            // Form1
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.buttonOpenFile);
            this.Controls.Add(this.comboBoxFileType);
            this.Name = "Form1";
            this.Text = "WebView2 File Viewer";
            this.ResumeLayout(false);
        }

        #endregion
    }
}