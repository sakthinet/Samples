// -------- Updated Form1.Designer.cs with Clear All (Reset) Button --------
namespace WinAppXMLLLM
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Button btnSelectFolder;
        private System.Windows.Forms.Button btnExtractData;
        private System.Windows.Forms.Button btnExportCsv;
        private System.Windows.Forms.Button btnExportExcel;
        private System.Windows.Forms.Button btnRestoreLayout;
        private System.Windows.Forms.Button btnToggleTheme;
        private System.Windows.Forms.Button btnClearAll;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.RadioButton radioAlfresco;
        private System.Windows.Forms.RadioButton radioDocumentum;
        private System.Windows.Forms.TextBox txtFilter;
        private System.Windows.Forms.ListBox lstElements;
        private System.Windows.Forms.GroupBox groupBox1;

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
            btnSelectFolder = new Button();
            btnExtractData = new Button();
            btnExportCsv = new Button();
            btnExportExcel = new Button();
            btnRestoreLayout = new Button();
            btnToggleTheme = new Button();
            btnClearAll = new Button();
            dataGridView1 = new DataGridView();
            radioAlfresco = new RadioButton();
            radioDocumentum = new RadioButton();
            txtFilter = new TextBox();
            lstElements = new ListBox();
            groupBox1 = new GroupBox();
            cmbThemeSelector = new ComboBox();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // btnSelectFolder
            // 
            btnSelectFolder.Location = new Point(269, 16);
            btnSelectFolder.Name = "btnSelectFolder";
            btnSelectFolder.Size = new Size(118, 35);
            btnSelectFolder.TabIndex = 2;
            btnSelectFolder.Text = "Select XML Folder";
            btnSelectFolder.UseVisualStyleBackColor = true;
            btnSelectFolder.Click += btnSelectFolder_Click;
            // 
            // btnExtractData
            // 
            btnExtractData.Location = new Point(430, 12);
            btnExtractData.Name = "btnExtractData";
            btnExtractData.Size = new Size(134, 35);
            btnExtractData.TabIndex = 8;
            btnExtractData.Text = "Extract Data";
            btnExtractData.UseVisualStyleBackColor = true;
            btnExtractData.Click += btnExtractData_Click;
            // 
            // btnExportCsv
            // 
            btnExportCsv.Location = new Point(12, 442);
            btnExportCsv.Name = "btnExportCsv";
            btnExportCsv.Size = new Size(175, 35);
            btnExportCsv.TabIndex = 7;
            btnExportCsv.Text = "Export to CSV";
            btnExportCsv.UseVisualStyleBackColor = true;
            btnExportCsv.Click += btnExportCsv_Click;
            // 
            // btnExportExcel
            // 
            btnExportExcel.Location = new Point(202, 442);
            btnExportExcel.Name = "btnExportExcel";
            btnExportExcel.Size = new Size(175, 35);
            btnExportExcel.TabIndex = 6;
            btnExportExcel.Text = "Export to Excel";
            btnExportExcel.UseVisualStyleBackColor = true;
            btnExportExcel.Click += btnExportExcel_Click;
            // 
            // btnRestoreLayout
            // 
            btnRestoreLayout.Location = new Point(943, 444);
            btnRestoreLayout.Name = "btnRestoreLayout";
            btnRestoreLayout.Size = new Size(175, 35);
            btnRestoreLayout.TabIndex = 3;
            btnRestoreLayout.Text = "Restore Column Layout";
            btnRestoreLayout.UseVisualStyleBackColor = true;
            btnRestoreLayout.Click += btnRestoreLayout_Click;
            // 
            // btnToggleTheme
            // 
            btnToggleTheme.Location = new Point(1016, 14);
            btnToggleTheme.Name = "btnToggleTheme";
            btnToggleTheme.Size = new Size(102, 35);
            btnToggleTheme.TabIndex = 2;
            btnToggleTheme.Text = "Toggle Theme";
            btnToggleTheme.UseVisualStyleBackColor = true;
            btnToggleTheme.Click += btnToggleTheme_Click;
            // 
            // btnClearAll
            // 
            btnClearAll.Location = new Point(800, 444);
            btnClearAll.Name = "btnClearAll";
            btnClearAll.Size = new Size(130, 35);
            btnClearAll.TabIndex = 0;
            btnClearAll.Text = "Clear All";
            btnClearAll.UseVisualStyleBackColor = true;
            btnClearAll.Click += btnClearAll_Click;
            // 
            // dataGridView1
            // 
            dataGridView1.Location = new Point(430, 69);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.Size = new Size(688, 369);
            dataGridView1.TabIndex = 5;
            // 
            // radioAlfresco
            // 
            radioAlfresco.Location = new Point(7, 23);
            radioAlfresco.Name = "radioAlfresco";
            radioAlfresco.Size = new Size(110, 23);
            radioAlfresco.TabIndex = 0;
            radioAlfresco.Text = "Alfresco XML";
            radioAlfresco.UseVisualStyleBackColor = true;
            // 
            // radioDocumentum
            // 
            radioDocumentum.Location = new Point(125, 22);
            radioDocumentum.Name = "radioDocumentum";
            radioDocumentum.Size = new Size(136, 23);
            radioDocumentum.TabIndex = 1;
            radioDocumentum.Text = "Documentum XML";
            radioDocumentum.UseVisualStyleBackColor = true;
            // 
            // txtFilter
            // 
            txtFilter.Location = new Point(14, 69);
            txtFilter.Name = "txtFilter";
            txtFilter.PlaceholderText = "Filter Elements...";
            txtFilter.Size = new Size(408, 23);
            txtFilter.TabIndex = 5;
            txtFilter.TextChanged += txtFilter_TextChanged;
            // 
            // lstElements
            // 
            lstElements.Location = new Point(12, 98);
            lstElements.Name = "lstElements";
            lstElements.SelectionMode = SelectionMode.MultiExtended;
            lstElements.Size = new Size(410, 334);
            lstElements.TabIndex = 4;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(radioAlfresco);
            groupBox1.Controls.Add(radioDocumentum);
            groupBox1.Controls.Add(btnSelectFolder);
            groupBox1.Location = new Point(12, 3);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(410, 60);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Text = "Select XML Type";
            // 
            // cmbThemeSelector
            // 
            cmbThemeSelector.FormattingEnabled = true;
            cmbThemeSelector.Location = new Point(889, 21);
            cmbThemeSelector.Name = "cmbThemeSelector";
            cmbThemeSelector.Size = new Size(121, 23);
            cmbThemeSelector.TabIndex = 9;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1127, 491);
            Controls.Add(cmbThemeSelector);
            Controls.Add(btnClearAll);
            Controls.Add(groupBox1);
            Controls.Add(btnToggleTheme);
            Controls.Add(btnRestoreLayout);
            Controls.Add(lstElements);
            Controls.Add(txtFilter);
            Controls.Add(btnExportExcel);
            Controls.Add(btnExportCsv);
            Controls.Add(dataGridView1);
            Controls.Add(btnExtractData);
            Name = "Form1";
            Text = "XML Extractor (Alfresco / Documentum)";
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            groupBox1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }
        private ComboBox cmbThemeSelector;
    }
}
