using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using OcrDataClassificationWebApp.Helpers;
using OcrDataClassificationWebApp.Services;

namespace OcrDataClassificationWebApp.Pages
{
    public class IndexModel : PageModel
    {
        public float MacroAccuracy { get; set; }
        public float MicroAccuracy { get; set; }
        public float LogLoss { get; set; }
        public bool IsDataClassified { get; set; }
        public int TotalRecords { get; set; }
        public int ClassifiedRecords { get; set; }
        public int UnclassifiedRecords { get; set; }
        public string DebugInfo { get; set; } = "";

        private readonly ILogger<IndexModel> _logger;
        private readonly MlClassifier _ml;
        private readonly IWebHostEnvironment _env;

        public IndexModel(ILogger<IndexModel> logger, MlClassifier ml, IWebHostEnvironment env)
        {
            _logger = logger;
            _ml = ml;
            _env = env;
        }

        public static DataTable UploadedData { get; set; }

        public void OnGet()
        {
            try
            {
                var filePath = Path.Combine(_env.WebRootPath, "data", "Set_copy1.csv");

                if (!System.IO.File.Exists(filePath))
                {
                    _logger.LogWarning($"CSV file not found at: {filePath}");
                    UploadedData = CreateEmptyDataTable();
                    return;
                }

                using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                UploadedData = CsvHelperUtility.LoadCsv(stream);

                if (!UploadedData.Columns.Contains("Classification"))
                    UploadedData.Columns.Add("Classification");

                foreach (DataRow row in UploadedData.Rows)
                {
                    if (row["Classification"] == null || string.IsNullOrWhiteSpace(row["Classification"].ToString()))
                        row["Classification"] = "Unclassified";
                }

                UpdateRecordCounts();
                _logger.LogInformation($"Loaded {UploadedData.Rows.Count} records from CSV file");

                // Debug: Log sample metadata
                if (UploadedData.Rows.Count > 0)
                {
                    var sampleMetadata = UploadedData.Rows[0]["metadata"]?.ToString();
                    _logger.LogInformation($"Sample metadata: {sampleMetadata}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading CSV data");
                UploadedData = CreateEmptyDataTable();
                TempData["ErrorMessage"] = "Error loading data file. Please check if the file exists and is accessible.";
            }
        }

        public IActionResult OnPostClassify()
        {
            try
            {
                if (UploadedData == null || UploadedData.Rows.Count == 0)
                {
                    TempData["ErrorMessage"] = "No data available for classification. Please ensure the data file is loaded.";
                    return Page();
                }

                _logger.LogInformation($"Starting classification of {UploadedData.Rows.Count} records");

                // Debug: Log sample data before classification
                if (UploadedData.Rows.Count > 0)
                {
                    var firstRow = UploadedData.Rows[0];
                    var sampleMetadata = firstRow["metadata"]?.ToString();
                    var sampleJsonOutput = firstRow["true_json_output"]?.ToString();
                    _logger.LogInformation($"Sample before classification - Metadata: {sampleMetadata?.Substring(0, Math.Min(100, sampleMetadata.Length))}...");
                    _logger.LogInformation($"Sample before classification - JSON Output: {sampleJsonOutput?.Substring(0, Math.Min(100, sampleJsonOutput.Length))}...");
                }

                // Perform classification
                _ml.Classify(UploadedData);

                // Debug: Log sample data after classification
                if (UploadedData.Rows.Count > 0)
                {
                    var firstRow = UploadedData.Rows[0];
                    var classification = firstRow["Classification"]?.ToString();
                    _logger.LogInformation($"Sample after classification - Classification: {classification}");
                }

                // Count classifications
                int classifiedCount = 0;
                var classificationCounts = new Dictionary<string, int>();

                foreach (DataRow row in UploadedData.Rows)
                {
                    var classification = row["Classification"]?.ToString();
                    if (!string.IsNullOrEmpty(classification) && classification != "Unclassified")
                    {
                        classifiedCount++;
                        if (classificationCounts.ContainsKey(classification))
                            classificationCounts[classification]++;
                        else
                            classificationCounts[classification] = 1;
                    }
                }

                // Log classification distribution
                _logger.LogInformation($"Classification completed. {classifiedCount} out of {UploadedData.Rows.Count} records classified.");
                foreach (var kvp in classificationCounts)
                {
                    _logger.LogInformation($"  {kvp.Key}: {kvp.Value} records");
                }

                // Evaluate metrics
                var metrics = _ml.Evaluate();
                MacroAccuracy = metrics.Item1;
                MicroAccuracy = metrics.Item2;
                LogLoss = metrics.Item3;

                _logger.LogInformation($"Metrics - Macro Accuracy: {MacroAccuracy:P2}, Micro Accuracy: {MicroAccuracy:P2}, Log Loss: {LogLoss:F4}");

                // Get per-class losses
                var (labels, losses) = _ml.GetPerClassLosses();
                ViewData["Labels"] = System.Text.Json.JsonSerializer.Serialize(labels);
                ViewData["Losses"] = System.Text.Json.JsonSerializer.Serialize(losses);

                // Debug info for display
                DebugInfo = $"Classified {classifiedCount}/{UploadedData.Rows.Count} records. " +
                           $"Classifications: {string.Join(", ", classificationCounts.Select(kvp => $"{kvp.Key}({kvp.Value})"))}";

                IsDataClassified = true;
                UpdateRecordCounts();

                TempData["SuccessMessage"] = $"Classification completed successfully! Processed {UploadedData.Rows.Count} records with {classifiedCount} classifications.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during classification");
                TempData["ErrorMessage"] = $"An error occurred during classification: {ex.Message}";
            }

            return Page();
        }

        public IActionResult OnPostExport()
        {
            try
            {
                if (UploadedData == null || UploadedData.Rows.Count == 0)
                {
                    TempData["ErrorMessage"] = "No data available for export.";
                    return Page();
                }

                var exportTable = UploadedData.Copy();
                var exportStream = new MemoryStream();

                using (var writer = new StreamWriter(exportStream, leaveOpen: true))
                using (var csv = new CsvHelper.CsvWriter(writer, System.Globalization.CultureInfo.InvariantCulture))
                {
                    // Write headers
                    foreach (DataColumn col in exportTable.Columns)
                    {
                        csv.WriteField(col.ColumnName);
                    }
                    csv.NextRecord();

                    // Write data rows
                    foreach (DataRow row in exportTable.Rows)
                    {
                        foreach (var field in row.ItemArray)
                        {
                            csv.WriteField(field?.ToString());
                        }
                        csv.NextRecord();
                    }
                }

                exportStream.Position = 0;

                var fileName = $"ClassifiedOutput_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                _logger.LogInformation($"Exporting {exportTable.Rows.Count} records to {fileName}");

                TempData["SuccessMessage"] = $"Data exported successfully as {fileName}";
                return File(exportStream, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during export");
                TempData["ErrorMessage"] = "An error occurred during export. Please try again.";
                return Page();
            }
        }

        public IActionResult OnPostRefreshData()
        {
            try
            {
                OnGet(); // Reload the data
                TempData["SuccessMessage"] = "Data refreshed successfully.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing data");
                TempData["ErrorMessage"] = "Error refreshing data. Please try again.";
            }

            return Page();
        }

        private void UpdateRecordCounts()
        {
            if (UploadedData != null)
            {
                TotalRecords = UploadedData.Rows.Count;
                ClassifiedRecords = UploadedData.AsEnumerable()
                    .Count(row => !string.IsNullOrWhiteSpace(row["Classification"]?.ToString()) &&
                                  row["Classification"].ToString() != "Unclassified");
                UnclassifiedRecords = TotalRecords - ClassifiedRecords;
            }
        }

        private DataTable CreateEmptyDataTable()
        {
            var dt = new DataTable();
            dt.Columns.Add("Message", typeof(string));
            dt.Columns.Add("Classification", typeof(string));

            var row = dt.NewRow();
            row["Message"] = "No data loaded. Please ensure the CSV file exists in wwwroot/data/Set_copy1.csv";
            row["Classification"] = "N/A";
            dt.Rows.Add(row);

            return dt;
        }

        public string GetClassificationStatusClass(string classification)
        {
            return string.IsNullOrWhiteSpace(classification) || classification == "Unclassified"
                ? "status-unclassified"
                : "status-classified";
        }

        public string GetClassificationIcon(string classification)
        {
            return string.IsNullOrWhiteSpace(classification) || classification == "Unclassified"
                ? "fas fa-clock"
                : "fas fa-check-circle";
        }
    }
}