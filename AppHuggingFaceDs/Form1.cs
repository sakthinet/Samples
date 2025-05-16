// Form1.cs
using System;
using System.Data;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json.Linq;

namespace AppHuggingFaceDs
{
    public partial class Form1 : Form
    {
        private static readonly HttpClient client = new HttpClient();

        public Form1()
        {
            InitializeComponent();
        }

        private async void btnFetchData_Click(object sender, EventArgs e)
        {
            //string apiUrl = "https://datasets-server.huggingface.co/rows?dataset=getomni-ai%2Focr-benchmark&config=default&split=test&offset=0&length=100";

            //try
            //{
            //    string response = await FetchDataFromAPI(apiUrl);
            //    DataTable dataTable = ParseJsonToDataTable(response);
            //    dataGridView1.DataSource = dataTable;
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show($"Error fetching data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
        }

        private async Task<string> FetchDataFromAPI(string url)
        {
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        private DataTable ParseJsonToDataTable(string jsonResponse)
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Image");
            dataTable.Columns.Add("id");
            dataTable.Columns.Add("metadata");
            dataTable.Columns.Add("json_schema");
            dataTable.Columns.Add("true_json_output");
            dataTable.Columns.Add("true_markdown_output");
            dataTable.Columns.Add("stringlengths");

            JObject jsonObject = JObject.Parse(jsonResponse);
            JArray rows = (JArray)jsonObject["rows"];

            foreach (var row in rows)
            {
                var rowData = row["row"];
                dataTable.Rows.Add(
                    rowData["Image"],
                    rowData["id"],
                    rowData["metadata"],
                    rowData["json_schema"],
                    rowData["true_json_output"],
                    rowData["true_markdown_output"],
                    rowData["stringlengths"]
                );
            }

            return dataTable;
        }

        //private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        //{
        //    if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
        //    {
        //        listBoxData.Items.Clear();
        //        richTextBoxData.Clear();

        //        string cellValue = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString();
        //        string columnName = dataGridView1.Columns[e.ColumnIndex].HeaderText;

        //        listBoxData.Items.Add(columnName + ": " + cellValue);
        //        richTextBoxData.AppendText(columnName + ": " + cellValue);
        //    }
        //}
       

        private DataTable LoadCSVToDataTable(string filePath)
        {
            DataTable dt = new DataTable();
            
            try
            {
                using (TextFieldParser parser = new TextFieldParser(filePath))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(",");
                    parser.HasFieldsEnclosedInQuotes = true;

                    if (!parser.EndOfData)
                    {
                        string[] headers = parser.ReadFields();
                        foreach (string header in headers)
                        {
                            dt.Columns.Add(header.Trim());
                        }
                    }

                    while (!parser.EndOfData)
                    {
                        string[] fields = parser.ReadFields();
                        dt.Rows.Add(fields);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading CSV: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return dt;
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                listBoxData.Items.Clear();
                richTextBoxData.Clear();

                string cellValue = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString();
                string columnName = dataGridView1.Columns[e.ColumnIndex].HeaderText;

                listBoxData.Items.Add(columnName + ": " + cellValue);
                richTextBoxData.AppendText(columnName + ": " + cellValue);
            }
        }

        private void btnLoadCSV_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                DataTable dataTable = LoadCSVToDataTable(filePath);
                dataGridView1.DataSource = dataTable;
            }
        }
    }
}
