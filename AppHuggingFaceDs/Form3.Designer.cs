namespace AppHuggingFaceDs
{
    partial class Form3
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.ListBox lstResults;
        private System.Windows.Forms.DataGridView dgvResults;
        private System.Windows.Forms.Label lblSearch;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            txtSearch = new TextBox();
            btnSearch = new Button();
            lstResults = new ListBox();
            dgvResults = new DataGridView();
            lblSearch = new Label();
            ((System.ComponentModel.ISupportInitialize)dgvResults).BeginInit();
            SuspendLayout();
            // 
            // txtSearch
            // 
            txtSearch.Location = new Point(110, 17);
            txtSearch.Name = "txtSearch";
            txtSearch.Size = new Size(250, 23);
            txtSearch.TabIndex = 1;
            // 
            // btnSearch
            // 
            btnSearch.Location = new Point(370, 15);
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new Size(100, 25);
            btnSearch.TabIndex = 2;
            btnSearch.Text = "Search";
            btnSearch.UseVisualStyleBackColor = true;
            btnSearch.Click += btnSearch_Click;
            // 
            // lstResults
            // 
            lstResults.FormattingEnabled = true;
            lstResults.ItemHeight = 15;
            lstResults.Location = new Point(20, 60);
            lstResults.Name = "lstResults";
            lstResults.Size = new Size(908, 94);
            lstResults.TabIndex = 3;
            // 
            // dgvResults
            // 
            dgvResults.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvResults.Location = new Point(20, 180);
            dgvResults.Name = "dgvResults";
            dgvResults.Size = new Size(908, 376);
            dgvResults.TabIndex = 4;
            // 
            // lblSearch
            // 
            lblSearch.AutoSize = true;
            lblSearch.Location = new Point(20, 20);
            lblSearch.Name = "lblSearch";
            lblSearch.Size = new Size(75, 15);
            lblSearch.TabIndex = 0;
            lblSearch.Text = "Enter Search:";
            // 
            // Form3
            // 
            ClientSize = new Size(940, 568);
            Controls.Add(lblSearch);
            Controls.Add(txtSearch);
            Controls.Add(btnSearch);
            Controls.Add(lstResults);
            Controls.Add(dgvResults);
            Name = "Form3";
            Text = "MongoDB Atlas Search";
            ((System.ComponentModel.ISupportInitialize)dgvResults).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
