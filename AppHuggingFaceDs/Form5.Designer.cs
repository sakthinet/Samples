namespace AppHuggingFaceDs
{
    partial class Form5
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.Button btnClassify;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnGroup;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.RichTextBox richTextBoxResults;
        private System.Windows.Forms.ListBox metricsListBox;


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
            dataGridView = new DataGridView();
            btnClassify = new Button();
            btnSearch = new Button();
            btnGroup = new Button();
            txtSearch = new TextBox();
            richTextBoxResults = new RichTextBox();
            metricsListBox = new ListBox();
            metricsLabel = new Label();
            metricsPanel = new Panel();
            groupBox1 = new GroupBox();
            groupBox2 = new GroupBox();
            ((System.ComponentModel.ISupportInitialize)dataGridView).BeginInit();
            metricsPanel.SuspendLayout();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            SuspendLayout();
            // 
            // dataGridView
            // 
            dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView.Location = new Point(6, 22);
            dataGridView.Name = "dataGridView";
            dataGridView.Size = new Size(736, 253);
            dataGridView.TabIndex = 0;
            // 
            // btnClassify
            // 
            btnClassify.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            btnClassify.Location = new Point(304, 306);
            btnClassify.Name = "btnClassify";
            btnClassify.Size = new Size(66, 25);
            btnClassify.TabIndex = 3;
            btnClassify.Text = "Classify";
            btnClassify.UseVisualStyleBackColor = true;
            btnClassify.Click += BtnClassify_Click;
            // 
            // btnSearch
            // 
            btnSearch.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            btnSearch.Location = new Point(232, 306);
            btnSearch.Name = "btnSearch";
            btnSearch.Size = new Size(66, 25);
            btnSearch.TabIndex = 2;
            btnSearch.Text = "Search";
            btnSearch.UseVisualStyleBackColor = true;
            btnSearch.Click += BtnSearch_Click;
            // 
            // btnGroup
            // 
            btnGroup.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold);
            btnGroup.Location = new Point(376, 306);
            btnGroup.Name = "btnGroup";
            btnGroup.Size = new Size(66, 25);
            btnGroup.TabIndex = 4;
            btnGroup.Text = "Group";
            btnGroup.UseVisualStyleBackColor = true;
            btnGroup.Click += BtnGroup_Click;
            // 
            // txtSearch
            // 
            txtSearch.Location = new Point(18, 300);
            txtSearch.Multiline = true;
            txtSearch.Name = "txtSearch";
            txtSearch.Size = new Size(208, 31);
            txtSearch.TabIndex = 1;
            // 
            // richTextBoxResults
            // 
            richTextBoxResults.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            richTextBoxResults.Location = new Point(12, 22);
            richTextBoxResults.Name = "richTextBoxResults";
            richTextBoxResults.ScrollBars = RichTextBoxScrollBars.ForcedBoth;
            richTextBoxResults.Size = new Size(751, 389);
            richTextBoxResults.TabIndex = 5;
            richTextBoxResults.Text = "";
            // 
            // metricsListBox
            // 
            metricsListBox.Font = new Font("Century Gothic", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            metricsListBox.HorizontalScrollbar = true;
            metricsListBox.IntegralHeight = false;
            metricsListBox.ItemHeight = 19;
            metricsListBox.Location = new Point(5, 38);
            metricsListBox.Name = "metricsListBox";
            metricsListBox.Size = new Size(409, 674);
            metricsListBox.TabIndex = 0;
            metricsListBox.DrawItem += metricsListBox_DrawItem;
            metricsListBox.SelectedIndexChanged += metricsListBox_SelectedIndexChanged_1;
            // 
            // metricsLabel
            // 
            metricsLabel.Dock = DockStyle.Top;
            metricsLabel.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            metricsLabel.Location = new Point(0, 0);
            metricsLabel.Name = "metricsLabel";
            metricsLabel.Size = new Size(413, 35);
            metricsLabel.TabIndex = 1;
            metricsLabel.Text = "Classification Metrics";
            metricsLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // metricsPanel
            // 
            metricsPanel.BorderStyle = BorderStyle.FixedSingle;
            metricsPanel.Controls.Add(metricsListBox);
            metricsPanel.Controls.Add(metricsLabel);
            metricsPanel.Location = new Point(779, 12);
            metricsPanel.Name = "metricsPanel";
            metricsPanel.Size = new Size(415, 728);
            metricsPanel.TabIndex = 0;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(richTextBoxResults);
            groupBox1.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            groupBox1.Location = new Point(10, 329);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(768, 420);
            groupBox1.TabIndex = 6;
            groupBox1.TabStop = false;
            groupBox1.Text = "Detailed CSV Cell Data";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(dataGridView);
            groupBox2.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            groupBox2.Location = new Point(18, 13);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(744, 281);
            groupBox2.TabIndex = 6;
            groupBox2.TabStop = false;
            groupBox2.Text = "CSV File Data";
            // 
            // Form5
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.LightSkyBlue;
            ClientSize = new Size(1196, 780);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(metricsPanel);
            Controls.Add(btnGroup);
            Controls.Add(btnClassify);
            Controls.Add(btnSearch);
            Controls.Add(txtSearch);
            Name = "Form5";
            Text = "OCR Data Classification";
            ((System.ComponentModel.ISupportInitialize)dataGridView).EndInit();
            metricsPanel.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }
        private Label metricsLabel;
        private Panel metricsPanel;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
    }
}