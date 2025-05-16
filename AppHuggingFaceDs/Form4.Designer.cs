namespace AppHuggingFaceDs
{
    partial class Form4
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Button btnLoadXml;
        private System.Windows.Forms.Button btnSaveJson;
        private System.Windows.Forms.Button btnLoadDataset;
        private System.Windows.Forms.RichTextBox rtbJson;
        private System.Windows.Forms.TreeView treeViewJson;
        private System.Windows.Forms.DataGridView dataGridViewJson;

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
            btnLoadXml = new Button();
            btnSaveJson = new Button();
            btnLoadDataset = new Button();
            rtbJson = new RichTextBox();
            treeViewJson = new TreeView();
            dataGridViewJson = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)dataGridViewJson).BeginInit();
            SuspendLayout();
            // 
            // btnLoadXml
            // 
            btnLoadXml.Location = new Point(20, 20);
            btnLoadXml.Name = "btnLoadXml";
            btnLoadXml.Size = new Size(100, 30);
            btnLoadXml.TabIndex = 0;
            btnLoadXml.Text = "Load XML";
            btnLoadXml.Click += btnLoadXml_Click;
            // 
            // btnSaveJson
            // 
            btnSaveJson.Location = new Point(130, 20);
            btnSaveJson.Name = "btnSaveJson";
            btnSaveJson.Size = new Size(100, 30);
            btnSaveJson.TabIndex = 1;
            btnSaveJson.Text = "Save JSON";
            btnSaveJson.Click += btnSaveJson_Click;
            // 
            // btnLoadDataset
            // 
            btnLoadDataset.Location = new Point(240, 20);
            btnLoadDataset.Name = "btnLoadDataset";
            btnLoadDataset.Size = new Size(120, 30);
            btnLoadDataset.TabIndex = 2;
            btnLoadDataset.Text = "Load Dataset";
            btnLoadDataset.Click += btnLoadDataset_Click;
            // 
            // rtbJson
            // 
            rtbJson.Location = new Point(20, 60);
            rtbJson.Name = "rtbJson";
            rtbJson.ScrollBars = RichTextBoxScrollBars.Vertical;
            rtbJson.Size = new Size(455, 344);
            rtbJson.TabIndex = 3;
            rtbJson.Text = "";
            // 
            // treeViewJson
            // 
            treeViewJson.Location = new Point(481, 60);
            treeViewJson.Name = "treeViewJson";
            treeViewJson.Size = new Size(518, 344);
            treeViewJson.TabIndex = 4;
            // 
            // dataGridViewJson
            // 
            dataGridViewJson.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewJson.Location = new Point(12, 410);
            dataGridViewJson.Name = "dataGridViewJson";
            dataGridViewJson.Size = new Size(987, 200);
            dataGridViewJson.TabIndex = 5;
            // 
            // Form4
            // 
            ClientSize = new Size(1004, 636);
            Controls.Add(btnLoadXml);
            Controls.Add(btnSaveJson);
            Controls.Add(btnLoadDataset);
            Controls.Add(rtbJson);
            Controls.Add(treeViewJson);
            Controls.Add(dataGridViewJson);
            Name = "Form4";
            Text = "XML to JSON Converter";
            ((System.ComponentModel.ISupportInitialize)dataGridViewJson).EndInit();
            ResumeLayout(false);
        }
    }
}
