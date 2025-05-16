using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json.Linq;

namespace AppHuggingFaceDs
{
    public partial class Form6 : Form
    {
        public Form6()
        {
            InitializeComponent();
        }

        private async void btnFetch_Click(object sender, EventArgs e)
        {
            lblStatus.Text = "Fetching data from Hugging Face...";
            btnFetch.Enabled = false;

            try
            {
                string baseUrl = "https://datasets-server.huggingface.co/rows?dataset=lmms-lab%2FOCRBench-v2&config=default&split=test&offset=0&length=100";
                var allDocs = await FetchAllPages(baseUrl);

                SaveToCsv(allDocs, @"c:\\Users\\NethraSamy\\Downloads\\huggingface_data.csv");
                lblStatus.Text = "✅ Completed: Data saved to huggingface_data.csv";
            }
            catch (Exception ex)
            {
                lblStatus.Text = $"❌ Error: {ex.Message}";
            }

            btnFetch.Enabled = true;
        }

        private async Task<List<Dictionary<string, object>>> FetchAllPages(string baseUrl)
        {
            List<Dictionary<string, object>> allRows = new List<Dictionary<string, object>>();
            int limit = 100;
            int offset = 0;

            using HttpClient client = new HttpClient();

            while (true)
            {
                string url = $"{baseUrl}&limit={limit}&offset={offset}";
                HttpResponseMessage response = await client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    // If it's 500 and we've already fetched some data, break instead of failing
                    if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError && allRows.Count > 0)
                    {
                        break;
                    }

                    throw new Exception($"Failed to fetch data. Status: {response.StatusCode} - {response.ReasonPhrase}");
                }

                string content = await response.Content.ReadAsStringAsync();
                JObject jsonData = JObject.Parse(content);

                if (jsonData["rows"] is not JArray rows || rows.Count == 0)
                    break;

                foreach (var rowItem in rows)
                {
                    var rowData = rowItem["row"];
                    if (rowData != null && rowData.Type == JTokenType.Object)
                    {
                        Dictionary<string, object> dict = new Dictionary<string, object>();
                        foreach (var prop in rowData.Children<JProperty>())
                        {
                            dict[prop.Name] = prop.Value.ToString();
                        }
                        allRows.Add(dict);
                    }
                }

                offset += limit;
                await Task.Delay(300); // avoid rate-limiting
            }

            return allRows;
        }



        private void SaveToCsv(List<Dictionary<string, object>> data, string filePath)
        {
            if (data == null || data.Count == 0)
                return;

            var headers = new HashSet<string>();
            foreach (var row in data)
                foreach (var key in row.Keys)
                    headers.Add(key);

            using StreamWriter writer = new StreamWriter(filePath);
            writer.WriteLine(string.Join(",", headers));

            foreach (var row in data)
            {
                List<string> rowValues = new List<string>();
                foreach (var header in headers)
                {
                    row.TryGetValue(header, out object value);
                    string safeValue = value?.ToString().Replace("\"", "\"\"") ?? "";
                    rowValues.Add($"\"{safeValue}\"");
                }
                writer.WriteLine(string.Join(",", rowValues));
            }
        }
    }
}
