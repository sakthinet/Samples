using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace AppHuggingFaceDs
{
    public partial class Form3 : Form
    {
        private readonly IMongoCollection<BsonDocument> _collection;

        public Form3()
        {
            InitializeComponent();
           
            // MongoDB connection setup
            string connectionString = "mongodb+srv://sakthi56:tv8UK9NNbObxupQR@cluster0.j8cmk.mongodb.net/";
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("TestECMDatabase");
            _collection = database.GetCollection<BsonDocument>("HuggingOCRSet");
        }

        private async void btnSearch_Click(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text.Trim();
            if (string.IsNullOrEmpty(searchText))
            {
                MessageBox.Show("Please enter search text.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var results = await PerformAtlasSearch(searchText);
            PopulateResults(results);
        }

        private async Task<List<BsonDocument>> PerformAtlasSearch(string query)
        {
            var pipeline = new[]
            {
                new BsonDocument("$search", new BsonDocument
                {
                    { "index", "OCR_Index" }, // Replace with your actual search index name
                    { "text", new BsonDocument
                        {
                            { "query", query },
                            { "path", new BsonDocument { { "wildcard", "*" } } } // Search across all fields
                        }
                    }
                })
            };

            var results = await _collection.Aggregate<BsonDocument>(pipeline).ToListAsync();
            return results;
        }


        private void PopulateResults(List<BsonDocument> documents)
        {
            lstResults.Items.Clear();
            dgvResults.Rows.Clear();
            dgvResults.Columns.Clear();

            if (documents.Count == 0)
            {
                MessageBox.Show("No results found.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Populate ListBox with raw JSON data
            foreach (var doc in documents)
            {
                lstResults.Items.Add(doc.ToJson());
            }

            // Get all unique keys from the documents
            var allKeys = documents.SelectMany(doc => doc.Names).Distinct().ToList();

            // Add columns dynamically
            dgvResults.Columns.Add("Id", "ID");
            foreach (var key in allKeys)
            {
                if (key != "_id")  // Avoid duplicate ID column
                {
                    dgvResults.Columns.Add(key, key);
                }
            }

            // Populate DataGridView with document values
            foreach (var doc in documents)
            {
                List<string> rowValues = new List<string>
                {
                    doc.Contains("_id") ? doc["_id"].ToString() : "N/A"
                };

                foreach (var key in allKeys)
                {
                    if (key != "_id")
                    {
                        rowValues.Add(doc.Contains(key) ? doc[key].ToString() : "N/A");
                    }
                }

                dgvResults.Rows.Add(rowValues.ToArray());
            }
        }
    }
}

