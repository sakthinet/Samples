namespace AppHuggingFaceDs
{
    partial class Form6
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Button btnFetch;
        private System.Windows.Forms.Label lblStatus;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.btnFetch = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnFetch
            // 
            this.btnFetch.Location = new System.Drawing.Point(30, 30);
            this.btnFetch.Name = "btnFetch";
            this.btnFetch.Size = new System.Drawing.Size(220, 40);
            this.btnFetch.TabIndex = 0;
            this.btnFetch.Text = "Fetch Hugging Face Models";
            this.btnFetch.UseVisualStyleBackColor = true;
            this.btnFetch.Click += new System.EventHandler(this.btnFetch_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.Location = new System.Drawing.Point(30, 90);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(400, 23);
            this.lblStatus.TabIndex = 1;
            this.lblStatus.Text = "Status: Waiting to start...";
            // 
            // Form6
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 150);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnFetch);
            this.Name = "Form6";
            this.Text = "Hugging Face Data Downloader";
            this.ResumeLayout(false);
        }
    }
}
