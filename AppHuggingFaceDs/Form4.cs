using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Xml;
using Newtonsoft.Json.Linq;
using System.Data;

namespace AppHuggingFaceDs
{
    public partial class Form4 : Form
    {
        private DataTable jsonDataTable;

        public Form4()
        {
            InitializeComponent();
        }

        private void btnLoadXml_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "XML Files (*.xml)|*.xml",
                Title = "Select an XML File"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string xmlContent = File.ReadAllText(openFileDialog.FileName);

                    // Convert XML to JSON
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(xmlContent);
                    string jsonText = JsonConvert.SerializeXmlNode(doc, Newtonsoft.Json.Formatting.Indented);

                    // Display JSON in RichTextBox
                    rtbJson.Text = jsonText;

                    // Display JSON in TreeView
                    PopulateTreeView(jsonText);

                    // Convert JSON to DataTable dynamically
                    jsonDataTable = ConvertJsonToDataTable(jsonText);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void PopulateTreeView(string jsonText)
        {
            try
            {
                treeViewJson.Nodes.Clear();
                JObject jsonObject = JObject.Parse(jsonText);
                TreeNode rootNode = new TreeNode("JSON Root");
                treeViewJson.Nodes.Add(rootNode);
                AddNodes(jsonObject, rootNode);
                treeViewJson.ExpandAll();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error Parsing JSON: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddNodes(JToken jsonToken, TreeNode parentNode)
        {
            if (jsonToken is JObject obj)
            {
                foreach (var property in obj.Properties())
                {
                    TreeNode newNode = new TreeNode(property.Name);
                    parentNode.Nodes.Add(newNode);
                    AddNodes(property.Value, newNode);
                }
            }
            else if (jsonToken is JArray array)
            {
                for (int i = 0; i < array.Count; i++)
                {
                    TreeNode arrayNode = new TreeNode($"[{i}]");
                    parentNode.Nodes.Add(arrayNode);
                    AddNodes(array[i], arrayNode);
                }
            }
            else
            {
                parentNode.Nodes.Add(new TreeNode(jsonToken.ToString()));
            }
        }

        private void btnSaveJson_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "JSON Files (*.json)|*.json",
                Title = "Save JSON File"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    File.WriteAllText(saveFileDialog.FileName, rtbJson.Text);
                    MessageBox.Show("JSON file saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error saving file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnLoadDataset_Click(object sender, EventArgs e)
        {
            if (jsonDataTable != null && jsonDataTable.Rows.Count > 0)
            {
                dataGridViewJson.DataSource = jsonDataTable;
            }
            else
            {
                MessageBox.Show("No data available!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private DataTable ConvertJsonToDataTable(string jsonText)
        {
            try
            {
                JObject jsonObject = JObject.Parse(jsonText);

                // Find the first array in the JSON structure
                JArray jsonArray = FindFirstArray(jsonObject);
                if (jsonArray == null)
                {
                    MessageBox.Show("No valid JSON array found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return new DataTable();
                }

                // Convert JArray to DataTable
                DataTable table = new DataTable();

                // Extract column names from the first object in JSON array
                if (jsonArray.Count > 0)
                {
                    foreach (var key in jsonArray[0].ToObject<JObject>().Properties())
                    {
                        table.Columns.Add(key.Name, typeof(string)); // Modify type based on expected values
                    }
                }

                // Add rows to DataTable
                foreach (var item in jsonArray)
                {
                    var row = table.NewRow();
                    foreach (var key in item.ToObject<JObject>().Properties())
                    {
                        row[key.Name] = key.Value?.ToString() ?? string.Empty;
                    }
                    table.Rows.Add(row);
                }

                return table;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error converting JSON to DataTable: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new DataTable();
            }
        }

        private JArray FindFirstArray(JObject jsonObject)
        {
            foreach (var property in jsonObject.Properties())
            {
                if (property.Value is JArray array)
                {
                    return array;
                }
                else if (property.Value is JObject nestedObject)
                {
                    // Recursively search for arrays
                    JArray nestedArray = FindFirstArray(nestedObject);
                    if (nestedArray != null)
                        return nestedArray;
                }
            }
            return null;
        }
    }
}
