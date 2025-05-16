using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DnsClient.Internal;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using Newtonsoft.Json.Linq;

namespace AppHuggingFaceDs
{
    public partial class Form2 : Form
    {
        // Configuration constants
        private const int API_BATCH_SIZE = 100;  // Match the Huggingface page size
        private const int MONGO_BATCH_SIZE = 20; // Smaller batches for MongoDB
        private const int API_DELAY_MS = 300;    // Delay between API calls
        private const int MAX_RETRIES = 3;       // Maximum number of retries for MongoDB operations
        private const int RETRY_DELAY_MS = 5000; // Delay between retries
        private const int SERVER_TIMEOUT_SEC = 100; // Server timeout
        private const int TOTAL_PAGES = 100;     // Total number of pages in the dataset

        // Status tracking
        private int totalFetched = 0;
        private int totalInserted = 0;
        private int totalErrors = 0;
        private Label statusLabel;

        public Form2()
        {
            InitializeComponent();

            // Optional: Add a status label to your form
            statusLabel = new Label
            {
                AutoSize = true,
                Location = new System.Drawing.Point(12, 50),
                Name = "statusLabel",
                Size = new System.Drawing.Size(0, 13),
                TabIndex = 1
            };
            this.Controls.Add(statusLabel);
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            UpdateStatus("Starting data fetch and insert process...");

            try
            {
                await FetchAndInsertAllPages();
                MessageBox.Show($"✅ Completed: {totalInserted} records inserted out of {totalFetched} fetched. {totalErrors} errors occurred.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ Error: {ex.Message}");
            }
            finally
            {
                button1.Enabled = true;
            }
        }

        private void UpdateStatus(string message)
        {
            if (statusLabel != null && !IsDisposed)
            {
                if (InvokeRequired)
                {
                    Invoke(new Action(() => statusLabel.Text = message));
                }
                else
                {
                    statusLabel.Text = message;
                }
            }
            Console.WriteLine(message); // For debugging
        }

        private async Task FetchAndInsertAllPages()
        {
            string baseUrl = "https://datasets-server.huggingface.co/rows?dataset=echo840%2FOCRBench&config=default&split=test&offset=0&length=100";
            int currentPage = 0;
            int offset = 0;
            totalFetched = 0;
            totalInserted = 0;
            totalErrors = 0;

            // MongoDB connection info
            var username = "mytestuser";
            var password = Uri.EscapeDataString("May123456789");

            // Connection string with explicit auth mechanism (SCRAM-SHA-256 for MongoDB Atlas)
            var connectionString = $"mongodb+srv://{username}:{password}@cluster0.j8cmk.mongodb.net/?retryWrites=true&w=majority&authSource=admin&authMechanism=SCRAM-SHA-1&appName=Cluster0";
            string databaseName = "TestECMDatabase";
            string collectionName = "HgOCRDataSet2";

            using (HttpClient client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromMinutes(5);
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
                client.DefaultRequestHeaders.Add("Accept", "application/json");

                // Create MongoDB client with proper error handling
                MongoClient mongoClient;
                try
                {
                    mongoClient = CreateMongoClient(connectionString);

                    // Test connection
                    UpdateStatus("Testing MongoDB connection...");
                    var adminDb = mongoClient.GetDatabase("admin");
                    await adminDb.RunCommandAsync((Command<BsonDocument>)"{ping:1}");
                    UpdateStatus("MongoDB connection successful!");
                }
                catch (Exception ex)
                {
                    UpdateStatus($"MongoDB connection error: {ex.Message}");
                    throw new Exception($"Failed to connect to MongoDB: {ex.Message}", ex);
                }

                var database = mongoClient.GetDatabase(databaseName);
                var collection = database.GetCollection<BsonDocument>(collectionName);

                // Create index on id field if it doesn't exist
                try
                {
                    var indexKeysDefinition = Builders<BsonDocument>.IndexKeys.Ascending("id");
                    await collection.Indexes.CreateOneAsync(new CreateIndexModel<BsonDocument>(indexKeysDefinition));
                }
                catch (Exception)
                {
                    // Index might already exist, which is fine
                }

                // Process all pages
                while (currentPage < TOTAL_PAGES)
                {
                    try
                    {
                        string url = $"{baseUrl}&offset={offset}&length={API_BATCH_SIZE}";
                        UpdateStatus($"Fetching page {currentPage + 1}/{TOTAL_PAGES} (offset: {offset})...");

                        HttpResponseMessage response = await client.GetAsync(url);

                        if (!response.IsSuccessStatusCode)
                        {
                            if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                            {
                                UpdateStatus($"Server error on page {currentPage + 1}. Retrying after delay...");
                                await Task.Delay(RETRY_DELAY_MS);
                                continue;
                            }
                            throw new Exception($"Failed to fetch data. Status: {response.StatusCode} - {response.ReasonPhrase}");
                        }

                        string content = await response.Content.ReadAsStringAsync();
                        JObject jsonData = JObject.Parse(content);

                        if (jsonData["rows"] is not JArray rows || rows.Count == 0)
                        {
                            UpdateStatus("No more rows to fetch. Completed.");
                            break;
                        }

                        // Process fetched data
                        List<BsonDocument> allDocs = new List<BsonDocument>();
                        foreach (var rowItem in rows)
                        {
                            try
                            {
                                var rowData = rowItem["row"];
                                if (rowData != null && rowData.Type == JTokenType.Object)
                                {
                                    BsonDocument doc = BsonDocument.Parse(rowData.ToString());

                                    // Ensure document has an _id field
                                    if (!doc.Contains("_id"))
                                    {
                                        if (doc.Contains("id"))
                                        {
                                            doc["_id"] = doc["id"];
                                        }
                                        else
                                        {
                                            doc["_id"] = ObjectId.GenerateNewId();
                                        }
                                    }

                                    allDocs.Add(doc);
                                    totalFetched++;
                                }
                            }
                            catch (Exception ex)
                            {
                                UpdateStatus($"Error parsing document: {ex.Message}");
                                totalErrors++;
                            }
                        }

                        // Insert documents in batches
                        if (allDocs.Count > 0)
                        {
                            UpdateStatus($"Inserting {allDocs.Count} documents from page {currentPage + 1}...");

                            for (int i = 0; i < allDocs.Count; i += MONGO_BATCH_SIZE)
                            {
                                var batch = allDocs.Skip(i).Take(MONGO_BATCH_SIZE).ToList();
                                bool success = await InsertBatchWithRetry(collection, batch);

                                // Success count is handled inside the InsertBatchWithRetry method
                            }
                        }

                        currentPage++;
                        offset += API_BATCH_SIZE;

                        UpdateStatus($"Progress: Page {currentPage}/{TOTAL_PAGES} - {totalInserted} inserted, {totalFetched} fetched, {totalErrors} errors");

                        await Task.Delay(API_DELAY_MS); // Respect API rate limits
                    }
                    catch (Exception ex)
                    {
                        UpdateStatus($"Error processing page {currentPage + 1}: {ex.Message}");
                        totalErrors++;
                        await Task.Delay(RETRY_DELAY_MS);
                        // Continue to next page despite errors
                        currentPage++;
                        offset += API_BATCH_SIZE;
                    }
                }
            }

            UpdateStatus($"Process completed. Total: {totalFetched} fetched, {totalInserted} inserted, {totalErrors} errors");
        }

        private MongoClient CreateMongoClient(string connectionString)
        {
            try
            {
                var settings = MongoClientSettings.FromConnectionString(connectionString);

                // Increase timeouts
                settings.ServerSelectionTimeout = TimeSpan.FromSeconds(SERVER_TIMEOUT_SEC);
                settings.ConnectTimeout = TimeSpan.FromSeconds(SERVER_TIMEOUT_SEC);
                settings.SocketTimeout = TimeSpan.FromSeconds(SERVER_TIMEOUT_SEC);

                // Connection pooling settings
                settings.MaxConnectionPoolSize = 100;
                settings.MinConnectionPoolSize = 10;

                // Write concern
                settings.WriteConcern = WriteConcern.WMajority;

                // Enable detailed logging for debugging
                // Removed the incorrect LoggingSettings assignment
                // settings.LoggingSettings = new LoggingSettings(new ConsoleLogger(LogLevel.Debug));

                return new MongoClient(settings);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to create MongoDB client: {ex.Message}");
                throw;
            }
        }

        private async Task<bool> InsertBatchWithRetry(IMongoCollection<BsonDocument> collection, List<BsonDocument> documents)
        {
            int attempts = 0;
            while (attempts < MAX_RETRIES)
            {
                try
                {
                    var options = new InsertManyOptions { IsOrdered = false }; // Continue on error
                    await collection.InsertManyAsync(documents, options);
                    totalInserted += documents.Count;
                    return true;
                }
                catch (MongoBulkWriteException ex)
                {
                    // Calculate successful documents
                    int successCount = documents.Count - ex.WriteErrors.Count;
                    if (successCount > 0)
                    {
                        totalInserted += successCount;
                        totalErrors += ex.WriteErrors.Count;
                        UpdateStatus($"Partial success: {successCount} inserted, {ex.WriteErrors.Count} failed");
                        return true;
                    }

                    // If all documents failed, fall through to retry or one-by-one insertion
                    attempts++;
                    if (attempts >= MAX_RETRIES)
                    {
                        UpdateStatus("All batch inserts failed. Trying one-by-one insertion...");
                        return await InsertDocumentsOneByOne(collection, documents);
                    }

                    UpdateStatus($"Batch insert failed (attempt {attempts}/{MAX_RETRIES}). Retrying...");
                    await Task.Delay(RETRY_DELAY_MS * attempts);
                }
                catch (MongoException ex)
                {
                    // Handle other MongoDB exceptions
                    attempts++;
                    UpdateStatus($"MongoDB error (attempt {attempts}/{MAX_RETRIES}): {ex.Message}");

                    if (attempts >= MAX_RETRIES)
                    {
                        UpdateStatus("All attempts failed. Trying one-by-one insertion...");
                        return await InsertDocumentsOneByOne(collection, documents);
                    }

                    await Task.Delay(RETRY_DELAY_MS * attempts);
                }
            }

            return false;
        }

        private void SaveToCSV(List<Dictionary<string, string>> data)
        {
            if (data.Count == 0) return;

            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), @"huggingface_data.csv");
            StringBuilder csvContent = new StringBuilder();

            var headers = data.First().Keys;
            csvContent.AppendLine(string.Join(",", headers));

            foreach (var row in data)
            {
                csvContent.AppendLine(string.Join(",", row.Values));
            }

            File.WriteAllText(filePath, csvContent.ToString(), Encoding.UTF8);
            UpdateStatus($"CSV file saved: {filePath}");
        }

        private async Task<bool> InsertDocumentsOneByOne(IMongoCollection<BsonDocument> collection, List<BsonDocument> documents)
        {
            UpdateStatus("Attempting individual document inserts...");
            int successCount = 0;

            foreach (var doc in documents)
            {
                try
                {
                    await collection.InsertOneAsync(doc);
                    successCount++;
                }
                catch (Exception ex)
                {
                    UpdateStatus($"Individual insert failed: {ex.Message}");
                    // Continue with next document
                }
            }

            UpdateStatus($"Individual inserts completed: {successCount}/{documents.Count} successful");
            totalInserted += successCount;
            totalErrors += (documents.Count - successCount);

            return successCount > 0;
        }

        // Custom logger for MongoDB driver
        private class ConsoleLogger : ILogger
        {
            private readonly LogLevel _logLevel;

            public ConsoleLogger(LogLevel logLevel)
            {
                _logLevel = logLevel;
            }

            public void Log(LogLevel logLevel, int eventId, Exception exception, string message, params object[] args)
            {
                if (logLevel >= _logLevel)
                {
                    Console.WriteLine($"[{logLevel}] {string.Format(message, args)}");
                    if (exception != null)
                    {
                        Console.WriteLine(exception);
                    }
                }
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return logLevel >= _logLevel;
            }
        }
    }
}