using System.Data;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace OcrDataClassificationWebApp.Services
{
    public class MlClassifier
    {
        private readonly MLContext mlContext;
        private ITransformer model;
        private PredictionEngine<OCRData, OCRPrediction> predictionEngine;
        private DataTable lastClassifiedData;

        public MlClassifier()
        {
            mlContext = new MLContext();
            TrainModel();
        }

        public void TrainModel()
        {
            var trainingData = SampleTrainingData.Get();

            // Debug: Log training data to verify mapping
            Console.WriteLine("Training Data Preview:");
            foreach (var item in trainingData.Take(10))
            {
                Console.WriteLine($"Metadata: '{item.Metadata}' -> Category: '{item.Category}'");
            }
            Console.WriteLine();

            var dataView = mlContext.Data.LoadFromEnumerable(trainingData);

            var pipeline = mlContext.Transforms.Conversion.MapValueToKey("Label", "Category")
                .Append(mlContext.Transforms.Text.FeaturizeText("MetadataFeatures", "Metadata"))
                .Append(mlContext.Transforms.Text.FeaturizeText("JsonFeatures", "JsonOutput"))
                .Append(mlContext.Transforms.Concatenate("Features", "MetadataFeatures", "JsonFeatures"))
                .Append(mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy())
                .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

            model = pipeline.Fit(dataView);
            predictionEngine = mlContext.Model.CreatePredictionEngine<OCRData, OCRPrediction>(model);

            // Test with some sample data
            Console.WriteLine("Testing trained model:");
            var testCases = new[] { "TABLE", "CHART", "BANK_CHECK", "PHOTO_DOC" };
            foreach (var testCase in testCases)
            {
                var testInput = new OCRData { Metadata = testCase, JsonOutput = "test" };
                var prediction = predictionEngine.Predict(testInput);
                Console.WriteLine($"'{testCase}' -> '{prediction.Category}'");
            }
            Console.WriteLine();
        }

        public void Classify(DataTable table)
        {
            int rowIndex = 0;
            foreach (DataRow row in table.Rows)
            {
                string rawMetadata = row["metadata"]?.ToString() ?? "";

                // Extract 'format' from metadata JSON-like string (same logic as WinForms)
                string format = ExtractFormat(rawMetadata);

                var input = new OCRData
                {
                    Metadata = format,  // Use extracted format only
                    JsonOutput = row["true_json_output"]?.ToString() ?? ""
                };

                var prediction = predictionEngine.Predict(input);
                row["Classification"] = prediction.Category;

                // Debug logging for first few rows
                if (rowIndex < 5)
                {
                    Console.WriteLine($"Row {rowIndex}: Raw metadata: {rawMetadata?.Substring(0, Math.Min(50, rawMetadata.Length))}...");
                    Console.WriteLine($"Row {rowIndex}: Extracted format: '{format}'");
                    Console.WriteLine($"Row {rowIndex}: Predicted category: {prediction.Category}");

                    // Show prediction scores
                    if (prediction.Score != null && prediction.Score.Length > 0)
                    {
                        var categoryNames = GetCategoryNames();
                        Console.WriteLine($"Row {rowIndex}: Top predictions:");
                        for (int i = 0; i < Math.Min(3, prediction.Score.Length); i++)
                        {
                            var categoryName = i < categoryNames.Count ? categoryNames[i] : $"Category{i}";
                            Console.WriteLine($"  {categoryName}: {prediction.Score[i]:F4}");
                        }
                    }
                    Console.WriteLine();
                }
                rowIndex++;
            }

            // Store the classified data for metrics calculation
            lastClassifiedData = table;
        }

        // Function to extract 'format' from metadata (same as WinForms)
        private string ExtractFormat(string metadata)
        {
            if (string.IsNullOrEmpty(metadata))
                return "";

            if (metadata.Contains("format:"))
            {
                int startIndex = metadata.IndexOf("format:") + 7;
                int endIndex = metadata.IndexOf(',', startIndex);
                if (endIndex == -1) endIndex = metadata.IndexOf('}', startIndex);
                if (endIndex > startIndex)
                {
                    string extracted = metadata.Substring(startIndex, endIndex - startIndex).Trim();
                    // Remove quotes if present
                    extracted = extracted.Trim('"', '\'', ' ');
                    return extracted;
                }
            }
            return metadata;  // Return original string if extraction fails
        }

        public OCRPrediction PredictSingle(OCRData input)
        {
            return predictionEngine.Predict(input);
        }

        public (List<string>, List<float>) GetPerClassLosses()
        {
            if (lastClassifiedData == null || lastClassifiedData.Rows.Count == 0)
            {
                return (new List<string>(), new List<float>());
            }

            try
            {
                // Create evaluation data from classified results
                var evaluationData = new List<OCRData>();

                foreach (DataRow row in lastClassifiedData.Rows)
                {
                    string rawMetadata = row["metadata"]?.ToString() ?? "";
                    string format = ExtractFormat(rawMetadata);
                    string classification = row["Classification"]?.ToString() ?? "";

                    if (!string.IsNullOrEmpty(classification) && classification != "Unclassified")
                    {
                        evaluationData.Add(new OCRData
                        {
                            Metadata = format,
                            JsonOutput = row["true_json_output"]?.ToString() ?? "",
                            Category = classification
                        });
                    }
                }

                if (evaluationData.Count == 0)
                {
                    return (new List<string>(), new List<float>());
                }

                var dataView = mlContext.Data.LoadFromEnumerable(evaluationData);
                var predictions = model.Transform(dataView);
                var metrics = mlContext.MulticlassClassification.Evaluate(predictions);

                var labels = new List<string>();
                var losses = new List<float>();

                // Get category names instead of just "Class X"
                var categoryNames = GetCategoryNames();

                for (int i = 0; i < metrics.PerClassLogLoss.Count && i < categoryNames.Count; i++)
                {
                    labels.Add(categoryNames[i]);
                    losses.Add((float)metrics.PerClassLogLoss[i]);
                }

                return (labels, losses);
            }
            catch (Exception)
            {
                return (new List<string>(), new List<float>());
            }
        }

        public (float macroAccuracy, float microAccuracy, float logLoss) Evaluate()
        {
            if (lastClassifiedData == null || lastClassifiedData.Rows.Count == 0)
            {
                return (0f, 0f, 0f);
            }

            try
            {
                // Create evaluation data from classified results
                var evaluationData = new List<OCRData>();

                foreach (DataRow row in lastClassifiedData.Rows)
                {
                    string rawMetadata = row["metadata"]?.ToString() ?? "";
                    string format = ExtractFormat(rawMetadata);
                    string classification = row["Classification"]?.ToString() ?? "";

                    if (!string.IsNullOrEmpty(classification) && classification != "Unclassified")
                    {
                        evaluationData.Add(new OCRData
                        {
                            Metadata = format,
                            JsonOutput = row["true_json_output"]?.ToString() ?? "",
                            Category = classification
                        });
                    }
                }

                if (evaluationData.Count == 0)
                {
                    return (0f, 0f, 0f);
                }

                var dataView = mlContext.Data.LoadFromEnumerable(evaluationData);
                var predictions = model.Transform(dataView);
                var metrics = mlContext.MulticlassClassification.Evaluate(predictions);

                return ((float)metrics.MacroAccuracy, (float)metrics.MicroAccuracy, (float)metrics.LogLoss);
            }
            catch (Exception)
            {
                return (0f, 0f, 0f);
            }
        }

        private List<string> GetCategoryNames()
        {
            return new List<string>
            {
                "Financial", "Transport", "Healthcare", "Legal",
                "General", "Ranking", "Education", "Health & Food",
                "Property", "Workforce", "Sales"
            };
        }

        public class OCRData
        {
            public string Metadata { get; set; }
            public string JsonOutput { get; set; }
            public string Category { get; set; }
        }

        public class OCRPrediction
        {
            [ColumnName("PredictedLabel")]
            public string Category { get; set; }
            public float[] Score { get; set; }
        }
    }
}