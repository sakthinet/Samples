// -------- Full Form1.cs with Context Menu, Delete Logic, Dynamic Layout --------
using System.Data;
using System.Text;
using System.Xml.Linq;
using ClosedXML.Excel;

namespace WinAppXMLLLM
{
    public partial class Form1 : Form
    {
        private List<string> xmlFilePaths = new List<string>();
        private HashSet<string> allElementPaths = new HashSet<string>();
        private List<string> sessionColumnLayout = new List<string>();
        private ContextMenuStrip contextMenu;
        private bool isDarkMode = false;
        //private System.Windows.Forms.Button btnToggleTheme;
        private System.Windows.Forms.ToolTip tooltip;
        private Theme currentTheme = Theme.Light;


        public Form1()
        {
            InitializeComponent();
            InitializeGridContextMenu();
            InitializeThemeToggle();
            ApplyTheme();
            ApplyTooltips();
            this.Resize += Form1_Resize;
            btnExportCsv.Enabled = false;
            btnExportExcel.Enabled = false;
            // Initialize theme dropdown
            cmbThemeSelector.Items.AddRange(Enum.GetNames(typeof(Theme)));
            cmbThemeSelector.SelectedItem = currentTheme.ToString();
            cmbThemeSelector.SelectedIndexChanged += cmbThemeSelector_SelectedIndexChanged;
            txtFilter.TextChanged += txtFilter_TextChanged;
            radioAlfresco.CheckedChanged += RadioAlfresco_CheckedChanged;
            radioDocumentum.CheckedChanged += RadioDocumentum_CheckedChanged;
        }

        private void cmbThemeSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Enum.TryParse<Theme>(cmbThemeSelector.SelectedItem.ToString(), out var selectedTheme))
            {
                SetTheme(selectedTheme);
            }
        }
        private void SetTheme(Theme theme)
        {
            currentTheme = theme;
            ApplyTheme();
        }
        private void RadioAlfresco_CheckedChanged(object sender, EventArgs e)
        {
            if (radioAlfresco.Checked)
            {
                radioDocumentum.Enabled = false;
                ConfirmClearExistingState();
            }
            else
            {
                radioDocumentum.Enabled = true;
            }
        }

        private void RadioDocumentum_CheckedChanged(object sender, EventArgs e)
        {
            if (radioDocumentum.Checked)
            {
                radioAlfresco.Enabled = false;
                ConfirmClearExistingState();
            }
            else
            {
                radioAlfresco.Enabled = true;
            }
        }
        private void ConfirmClearExistingState()
        {
            if (lstElements.Items.Count > 0 || dataGridView1.Rows.Count > 0)
            {
                DialogResult result = MessageBox.Show("Switching XML type will clear current data. Proceed?", "Confirm Switch", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    ClearForm();
                }
                else
                {
                    radioAlfresco.CheckedChanged -= RadioAlfresco_CheckedChanged;
                    radioDocumentum.CheckedChanged -= RadioDocumentum_CheckedChanged;
                    radioAlfresco.Checked = !radioAlfresco.Checked;
                    radioDocumentum.Checked = !radioDocumentum.Checked;
                    radioAlfresco.CheckedChanged += RadioAlfresco_CheckedChanged;
                    radioDocumentum.CheckedChanged += RadioDocumentum_CheckedChanged;
                }
            }
        }
        private void ClearForm()
        {
            xmlFilePaths.Clear();
            allElementPaths.Clear();
            sessionColumnLayout.Clear();
            lstElements.Items.Clear();
            txtFilter.Text = string.Empty;
            dataGridView1.DataSource = null;
            dataGridView1.Columns.Clear();

            btnExportCsv.Enabled = false;
            btnExportExcel.Enabled = false;
        }

        private void btnClearAll_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to clear all data and reset the form?", "Reset Confirmation", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                ClearForm();
                radioAlfresco.Enabled = true;
                radioDocumentum.Enabled = true;
                radioAlfresco.Checked = false;
                radioDocumentum.Checked = false;
            }
        }
        private void InitializeThemeToggle()
        {
            //btnToggleTheme = new System.Windows.Forms.Button();
            //btnToggleTheme.Text = "Toggle Theme";
            //btnToggleTheme.Name = "btnToggleTheme";
            //btnToggleTheme.Size = new Size(130, 40);
            //btnToggleTheme.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            //btnToggleTheme.Click += (s, e) => ToggleTheme();
            //this.Controls.Add(btnToggleTheme);
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            int spacing = 5;
            dataGridView1.Width = this.ClientSize.Width - dataGridView1.Left - spacing;
            dataGridView1.Height = this.ClientSize.Height - dataGridView1.Top - 100;

            int bottom = dataGridView1.Bottom + spacing;
            btnExportCsv.Top = bottom;
            btnExportExcel.Top = bottom;
            btnRestoreLayout.Top = bottom;
            btnToggleTheme.Top = bottom;
        }


        private void InitializeGridContextMenu()
        {
            //contextMenu = new ContextMenuStrip();
            //var deleteColumn = new ToolStripMenuItem("Delete Column");
            //deleteColumn.Click += (s, e) => DeleteSelectedColumns();
            //var deleteRow = new ToolStripMenuItem("Delete Row");
            //deleteRow.Click += (s, e) => DeleteSelectedRows();
            ////btnRestoreLayout.Click += (s, e) => restor();
            //contextMenu.Items.AddRange(new ToolStripItem[] { deleteColumn, deleteRow });
            //// ✅ This line is necessary!
            //dataGridView1.ContextMenuStrip = contextMenu;

            contextMenu = new ContextMenuStrip();
            var deleteColumnMenuItem = new ToolStripMenuItem("Delete Column");
            var deleteRowMenuItem = new ToolStripMenuItem("Delete Row");

            deleteColumnMenuItem.Click += (s, e) => DeleteSelectedColumns();
            deleteRowMenuItem.Click += (s, e) => DeleteSelectedRows();

            contextMenu.Items.AddRange(new[] { deleteColumnMenuItem, deleteRowMenuItem });
            dataGridView1.ContextMenuStrip = contextMenu;
        }

        private void DataGridView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                dataGridView1.ClearSelection();
                dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Selected = true;
                dataGridView1.Rows[e.RowIndex].Selected = true;
                contextMenu.Show(Cursor.Position);
            }
            if (e.Button == MouseButtons.Right)
            {
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    dataGridView1.ClearSelection();
                    dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Selected = true;
                    contextMenu.Show(Cursor.Position);
                }
                else if (e.RowIndex == -1 && e.ColumnIndex >= 0)
                {
                    dataGridView1.ClearSelection();
                    dataGridView1.Columns[e.ColumnIndex].Selected = true;
                    contextMenu.Show(Cursor.Position);
                }
                else if (e.RowIndex >= 0 && e.ColumnIndex == -1)
                {
                    dataGridView1.ClearSelection();
                    dataGridView1.Rows[e.RowIndex].Selected = true;
                    contextMenu.Show(Cursor.Position);
                }
            }
        }

        private void DeleteSelectedColumns()
        {
            if (dataGridView1?.SelectedCells.Count > 0)
            {
                int columnIndex = dataGridView1.SelectedCells[0].ColumnIndex;
                string columnName = dataGridView1.Columns[columnIndex].Name;

                DialogResult result = MessageBox.Show($"Delete column '{columnName}'?", "Confirm", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    dataGridView1.Columns.RemoveAt(columnIndex);
                    SaveColumnLayout();
                }
            }
        }

        private void DeleteSelectedRows()
        {
            if (dataGridView1?.SelectedRows.Count > 0)
            {
                DialogResult result = MessageBox.Show("Delete selected row(s)?", "Confirm", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    var rowsToRemove = new List<DataGridViewRow>();
                    foreach (DataGridViewRow row in dataGridView1.SelectedRows)
                    {
                        if (!row.IsNewRow)
                            rowsToRemove.Add(row);
                    }

                    foreach (var row in rowsToRemove)
                    {
                        dataGridView1.Rows.Remove(row);
                    }

                    SaveColumnLayout();
                }
            }
        }

        private void SaveColumnLayout()
        {
            try
            {
                sessionColumnLayout = dataGridView1.Columns
                    .Cast<DataGridViewColumn>()
                    .Select(c => c.Name)
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to save layout: " + ex.Message);
            }
        }

        private void LoadColumnLayout()
        {
            try
            {
                foreach (var name in sessionColumnLayout)
                {
                    if (!dataGridView1.Columns.Contains(name))
                        dataGridView1.Columns.Add(name, name);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load layout: " + ex.Message);
            }
        }

        private void btnSelectFolder_Click(object sender, EventArgs e)
        {
            if (!radioAlfresco.Checked && !radioDocumentum.Checked)
            {
                MessageBox.Show("Please select an XML type before choosing a folder.", "Validation Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // Check if grid or listbox has data
            bool hasGridData = dataGridView1.Rows.Count > 0;
            bool hasListItems = lstElements.Items.Count > 0;

            if (hasGridData || hasListItems)
            {
                var result = MessageBox.Show(
                    "You’ve already loaded data. Do you want to clear and select a new folder?",
                    "Confirm Reset",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (result == DialogResult.No)
                {
                    return; // Don’t continue
                }

                // If Yes, clear previous data
                dataGridView1.DataSource = null;
                lstElements.Items.Clear();
                txtFilter.Text = "";
                btnExportCsv.Enabled = false;
                btnExportExcel.Enabled = false;
            }
            using (var fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    xmlFilePaths = Directory.GetFiles(fbd.SelectedPath, "*.xml").ToList();
                    if (xmlFilePaths.Count == 0)
                    {
                        MessageBox.Show("No XML files found in the selected folder.");
                        return;
                    }

                    XDocument doc = XDocument.Load(xmlFilePaths[0]);
                    bool isAlfresco = doc.Descendants("entry").Any(e => e.Attribute("key") != null);
                    bool isDocumentum = doc.Descendants("attribute").Any();

                    if (radioAlfresco.Checked && !isAlfresco)
                    {
                        MessageBox.Show("Selected files do not appear to be Alfresco XML format.", "Type Mismatch");
                        return;
                    }
                    if (radioDocumentum.Checked && !isDocumentum)
                    {
                        MessageBox.Show("Selected files do not appear to be Documentum XML format.", "Type Mismatch");
                        return;
                    }

                    MessageBox.Show($"{xmlFilePaths.Count} XML files loaded.");
                    LoadElementPaths();
                }
            }
        }

        private void LoadElementPaths()
        {
            allElementPaths.Clear();

            foreach (var file in xmlFilePaths)
            {
                try
                {
                    XDocument doc = XDocument.Load(file);

                    if (radioAlfresco.Checked)
                    {
                        LoadAlfrescoElementPaths(doc);
                    }
                    else if (radioDocumentum.Checked)
                    {
                        LoadDocumentumElementPaths(doc);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading paths from file {file}: {ex.Message}");
                }
            }

            UpdateFilteredElementList();
        }

        private void LoadAlfrescoElementPaths(XDocument doc)
        {
            var entries = doc.Descendants().Where(x => x.Name.LocalName == "entry");
            foreach (var entry in entries)
            {
                var key = entry.Attribute("key")?.Value?.Trim();
                var value = entry.Value?.Trim();

                if (!string.IsNullOrEmpty(key))
                {
                    string label = $"{key}-properties/{key}";
                    if (!allElementPaths.Contains(label))
                    {
                        allElementPaths.Add(label);
                    }
                }
            }
        }

        private void LoadDocumentumElementPaths(XDocument doc)
        {
            var folders = doc.Descendants().Where(e => e.Name.LocalName.StartsWith("folder_"));

            foreach (var folder in folders)
            {
                string folderPath = GetElementPath(folder);

                // Include all child elements like name, object_type, etc.
                foreach (var child in folder.Elements())
                {
                    if (child.Name.LocalName == "attribute")
                    {
                        var attrName = child.Element("AttrName")?.Value?.Trim();
                        var attrValue = child.Element("AttrValue")?.Value?.Trim();

                        if (!string.IsNullOrEmpty(attrName))
                        {
                            string attrPath = $"{folderPath}/attribute/{attrName}";
                            allElementPaths.Add(attrPath);
                        }
                    }
                    else
                    {
                        string childPath = $"{folderPath}/{child.Name.LocalName}";
                        allElementPaths.Add(childPath);
                    }
                }
            }

            // Also include other global non-folder elements
            foreach (var element in doc.Descendants())
            {
                if (!element.Ancestors().Any(a => a.Name.LocalName.StartsWith("folder_")))
                {
                    string path = GetElementPath(element);
                    allElementPaths.Add(path);
                }
            }
        }


        private void UpdateFilteredElementList()
        {
            lstElements.Items.Clear();

            string filter = txtFilter.Text.Trim().ToLower();

            var filteredPaths = string.IsNullOrEmpty(filter)
                ? allElementPaths.OrderBy(x => x)
                : allElementPaths.Where(p => p.ToLower().Contains(filter)).OrderBy(x => x);

            lstElements.Items.AddRange(filteredPaths.ToArray());
        }

        private string GetElementPath(XElement element)
        {
            var ancestors = element.Ancestors().Reverse().Select(e => e.Name.LocalName);
            var fullPath = string.Join("/", ancestors.Concat(new[] { element.Name.LocalName }));
            return fullPath;
        }

        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
            UpdateFilteredElementList();
        }

        private void btnExtractData_Click(object sender, EventArgs e)
        {
            if (radioAlfresco.Checked)
                ExtractAlfrescoXml();
            else if (radioDocumentum.Checked)
                ExtractDynamicXml();
            else
                MessageBox.Show("Please select an XML type.");

        }

        private void ExtractAlfrescoXml()
        {
            var selectedPaths = lstElements.SelectedItems.Cast<string>().ToHashSet();
            DataTable dt = new DataTable();

            foreach (var file in xmlFilePaths)
            {
                try
                {
                    XDocument doc = XDocument.Load(file);
                    var propertyGroups = doc.Descendants("properties");

                    foreach (var group in propertyGroups)
                    {
                        DataRow row = dt.NewRow();
                        var entries = group.Elements("entry");

                        foreach (var entry in entries)
                        {
                            var key = entry.Attribute("key")?.Value?.Trim();
                            var value = entry.Value?.Trim();

                            if (!string.IsNullOrEmpty(key))
                            {
                                string fullKey = $"{key}-properties/{key}";

                                // Only include selected elements or all if none selected
                                if (selectedPaths.Count == 0 || selectedPaths.Contains(fullKey))
                                {
                                    if (!dt.Columns.Contains(key))
                                        dt.Columns.Add(key);

                                    row[key] = value;
                                }
                            }
                        }

                        dt.Rows.Add(row);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error processing file {file}: {ex.Message}");
                }
            }

            btnExportCsv.Enabled = dt.Rows.Count > 0;
            btnExportExcel.Enabled = dt.Rows.Count > 0;

            DataTable distinctTable = dt.DefaultView.ToTable(true);
            dataGridView1.DataSource = distinctTable;

        }



        private void ExtractDynamicXml()
        {
            var selectedPaths = lstElements.SelectedItems.Cast<string>().ToHashSet();
            DataTable dt = new DataTable();

            foreach (var file in xmlFilePaths)
            {
                try
                {
                    XDocument doc = XDocument.Load(file);
                    var rowValues = new Dictionary<string, string>();

                    foreach (var element in doc.Descendants())
                    {
                        string elementPath = GetElementPath(element);

                        if (element.Name.LocalName == "attribute")
                        {
                            var attrName = element.Element("AttrName")?.Value?.Trim();
                            var attrValue = element.Element("AttrValue")?.Value?.Trim() ?? string.Empty;

                            if (!string.IsNullOrWhiteSpace(attrName))
                            {
                                string dynamicCol = $"{elementPath}/{attrName}";

                                if (selectedPaths.Count == 0 || selectedPaths.Contains(dynamicCol))
                                {
                                    if (rowValues.ContainsKey(dynamicCol))
                                        rowValues[dynamicCol] += $"; {attrValue}";
                                    else
                                        rowValues[dynamicCol] = attrValue;
                                }
                            }
                        }
                        else
                        {
                            if ((selectedPaths.Count == 0 || selectedPaths.Contains(elementPath)) && !rowValues.ContainsKey(elementPath))
                                rowValues[elementPath] = element.Value?.Trim() ?? string.Empty;

                            foreach (var attr in element.Attributes())
                            {
                                string attrPath = $"{elementPath}[@{attr.Name.LocalName}]";
                                if ((selectedPaths.Count == 0 || selectedPaths.Contains(attrPath)) && !rowValues.ContainsKey(attrPath))
                                    rowValues[attrPath] = attr.Value?.Trim() ?? string.Empty;
                            }
                        }
                    }

                    foreach (var key in rowValues.Keys)
                        if (!dt.Columns.Contains(key))
                            dt.Columns.Add(key);

                    DataRow row = dt.NewRow();
                    foreach (DataColumn col in dt.Columns)
                        row[col.ColumnName] = rowValues.ContainsKey(col.ColumnName) ? rowValues[col.ColumnName] : string.Empty;

                    dt.Rows.Add(row);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error processing file {file}: {ex.Message}");
                }
            }

            DataTable distinctTable = dt.DefaultView.ToTable(true);
            dataGridView1.DataSource = distinctTable;
            LoadColumnLayout();
            if (dt.Rows.Count > 0)
            {
                btnExportCsv.Enabled = dt.Rows.Count > 0;
                btnExportExcel.Enabled = dt.Rows.Count > 0;
            }

        }

        private void btnExportCsv_Click(object sender, EventArgs e)
        {
            if (dataGridView1.DataSource is DataTable dt)
            {
                SaveFileDialog sfd = new SaveFileDialog { Filter = "CSV files|*.csv" };
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    StringBuilder sb = new StringBuilder();
                    IEnumerable<string> columnNames = dt.Columns.Cast<DataColumn>().Select(column => column.ColumnName);
                    sb.AppendLine(string.Join(",", columnNames));
                    foreach (DataRow row in dt.Rows)
                        sb.AppendLine(string.Join(",", row.ItemArray));
                    File.WriteAllText(sfd.FileName, sb.ToString());
                    MessageBox.Show("CSV exported successfully.");
                }
            }
        }

        private void btnExportExcel_Click(object sender, EventArgs e)
        {
            if (dataGridView1.DataSource is DataTable dt)
            {
                SaveFileDialog sfd = new SaveFileDialog { Filter = "Excel files|*.xlsx" };
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    using (var workbook = new XLWorkbook())
                    {
                        var worksheet = workbook.Worksheets.Add("Data");
                        worksheet.Cell(1, 1).InsertTable(dt);
                        workbook.SaveAs(sfd.FileName);
                    }
                    MessageBox.Show("Excel exported successfully.");
                }
            }
        }

        private void DataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                DeleteSelectedRows();
            }
        }

        private void btnRestoreLayout_Click(object sender, EventArgs e)
        {
            LoadColumnLayout();
        }
        private void ApplyTooltips()
        {
            tooltip = new System.Windows.Forms.ToolTip();
            tooltip.SetToolTip(btnSelectFolder, "Select XML folder");
            tooltip.SetToolTip(btnExportCsv, "Export table to CSV");
            tooltip.SetToolTip(btnExportExcel, "Export table to Excel");
            tooltip.SetToolTip(btnRestoreLayout, "Restore saved column layout");
            tooltip.SetToolTip(btnToggleTheme, "Switch between light and dark themes");
        }

        private void ApplyTheme()
        {
            bool isDark = currentTheme == Theme.Dark;

            Color bgColor = isDark ? Color.Black : currentTheme == Theme.Blue ? Color.FromArgb(0, 120, 215) : SystemColors.Control;
            Color fgColor = isDark ? Color.White : Color.Black;
            Color btnColor = isDark ? Color.FromArgb(40, 40, 40) : Color.White;
            Color borderColor = isDark ? Color.Gray : Color.LightGray;

            this.BackColor = bgColor;
            this.ForeColor = fgColor;

            // Theme radio group container (e.g. GroupBox or Panel)
            groupBox1.BackColor = bgColor;
            groupBox1.ForeColor = fgColor;

            radioAlfresco.ForeColor = fgColor;
            radioDocumentum.ForeColor = fgColor;

            // Buttons
            foreach (Control ctrl in Controls)
            {
                if (ctrl is Button btn)
                {
                    btn.BackColor = btnColor;
                    btn.ForeColor = fgColor;
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderColor = borderColor;
                }
            }

            // Listbox, textbox
            lstElements.BackColor = isDark ? Color.FromArgb(30, 30, 30) : Color.White;
            lstElements.ForeColor = fgColor;

            txtFilter.BackColor = isDark ? Color.FromArgb(30, 30, 30) : Color.White;
            txtFilter.ForeColor = fgColor;

            // DataGridView styling
            dataGridView1.BackgroundColor = isDark ? Color.DimGray : Color.White;
            dataGridView1.DefaultCellStyle.BackColor = isDark ? Color.Black : Color.White;
            dataGridView1.DefaultCellStyle.ForeColor = fgColor;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = isDark ? Color.Gray : Color.LightGray;
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = fgColor;
        }

        // Optional: Enum for themes
        public enum Theme
        {
            Light,
            Dark,
            Blue,
            Solarized
        }


        private void ToggleTheme()
        {
            isDarkMode = !isDarkMode;
            ApplyTheme();
        }

        private void btnToggleTheme_Click(object sender, EventArgs e)
        {
            ToggleTheme();
        }
    }
}