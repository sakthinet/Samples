// Form1.Designer.cs
namespace AppHuggingFaceDs
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button btnFetchData;
        private System.Windows.Forms.ListBox listBoxData;
        private System.Windows.Forms.RichTextBox richTextBoxData;

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
            dataGridView1 = new DataGridView();
            btnFetchData = new Button();
            listBoxData = new ListBox();
            richTextBoxData = new RichTextBox();
            btnLoadCSV = new Button();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Location = new Point(14, 14);
            dataGridView1.Margin = new Padding(4, 3, 4, 3);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.Size = new Size(713, 686);
            dataGridView1.TabIndex = 0;
            dataGridView1.CellClick += dataGridView1_CellClick;
            // 
            // btnFetchData
            // 
            btnFetchData.Location = new Point(27, 718);
            btnFetchData.Margin = new Padding(4, 3, 4, 3);
            btnFetchData.Name = "btnFetchData";
            btnFetchData.Size = new Size(140, 35);
            btnFetchData.TabIndex = 1;
            btnFetchData.Text = "Fetch Data";
            btnFetchData.UseVisualStyleBackColor = true;
            btnFetchData.Click += btnFetchData_Click;
            // 
            // listBoxData
            // 
            listBoxData.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            listBoxData.ItemHeight = 21;
            listBoxData.Location = new Point(735, 14);
            listBoxData.Margin = new Padding(4, 3, 4, 3);
            listBoxData.Name = "listBoxData";
            listBoxData.Size = new Size(887, 277);
            listBoxData.TabIndex = 2;
            // 
            // richTextBoxData
            // 
            richTextBoxData.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            richTextBoxData.Location = new Point(735, 310);
            richTextBoxData.Margin = new Padding(4, 3, 4, 3);
            richTextBoxData.Name = "richTextBoxData";
            richTextBoxData.Size = new Size(887, 390);
            richTextBoxData.TabIndex = 3;
            richTextBoxData.Text = "";
            // 
            // btnLoadCSV
            // 
            btnLoadCSV.Location = new Point(198, 718);
            btnLoadCSV.Margin = new Padding(4, 3, 4, 3);
            btnLoadCSV.Name = "btnLoadCSV";
            btnLoadCSV.Size = new Size(140, 35);
            btnLoadCSV.TabIndex = 4;
            btnLoadCSV.Text = "Load CSV";
            btnLoadCSV.UseVisualStyleBackColor = true;
            btnLoadCSV.Click += btnLoadCSV_Click_1;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1635, 779);
            Controls.Add(btnLoadCSV);
            Controls.Add(richTextBoxData);
            Controls.Add(listBoxData);
            Controls.Add(btnFetchData);
            Controls.Add(dataGridView1);
            Margin = new Padding(4, 3, 4, 3);
            Name = "Form1";
            Text = "Hugging Face Dataset Viewer";
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
        }
        private Button btnLoadCSV;
    }
}
