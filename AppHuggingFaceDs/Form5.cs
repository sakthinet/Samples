namespace AppHuggingFaceDs
{
    using Microsoft.ML;
    using Microsoft.ML.Data;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;

    //string filePath = @"C:\\Users\\NethraSamy\\Downloads\\HFDataset\\Set_copy1.csv"; // Ensure correct path

    /// <summary>
    /// Defines the <see cref="Form5" />
    /// </summary>
    public partial class Form5 : Form
    {
        /// <summary>
        /// Defines the dataTable
        /// </summary>
        private DataTable dataTable;

        /// <summary>
        /// Defines the groupedTable
        /// </summary>
        private DataTable groupedTable;

        /// <summary>
        /// Defines the mlContext
        /// </summary>
        private MLContext mlContext;

        /// <summary>
        /// Defines the predictionEngine
        /// </summary>
        private PredictionEngine<OCRData, OCRPrediction> predictionEngine;

        /// <summary>
        /// Defines the model
        /// </summary>
        private ITransformer model;

        /// <summary>
        /// Defines the classPredictions
        /// </summary>
        private Dictionary<string, List<PredictionScore>> classPredictions = new Dictionary<string, List<PredictionScore>>();

        /// <summary>
        /// Defines the metrics
        /// </summary>
        private MulticlassClassificationMetrics metrics;

        /// <summary>
        /// Initializes a new instance of the <see cref="Form5"/> class.
        /// </summary>
        public Form5()
        {
            InitializeComponent();
            LoadCsvData();
            mlContext = new MLContext();
            TrainModel();
            dataGridView.CellClick += DataGridView_CellClick;
            dataGridView.SelectionChanged += DataGridView_SelectionChanged;
        }

        /// <summary>
        /// The LoadCsvData
        /// </summary>
        private void LoadCsvData()
        {
            string filePath = @"D:\Programming\VectorDbTest\dsFiles\Set_copy1.csv"; // Ensure correct path
            if (!File.Exists(filePath))
            {
                MessageBox.Show("CSV file not found.");
                return;
            }

            dataTable = new DataTable();

            using (var reader = new StreamReader(filePath))
            {
                var headers = reader.ReadLine()?.Split(',');
                if (headers == null || headers.Length == 0)
                {
                    MessageBox.Show("CSV file is empty or header is missing.");
                    return;
                }

                foreach (var header in headers)
                {
                    dataTable.Columns.Add(header.Trim());
                }

                // Add a Classification column
                if (!dataTable.Columns.Contains("Classification"))
                {
                    dataTable.Columns.Add("Classification");
                }

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = ParseCsvLine(line);

                    if (values != null && values.Length == dataTable.Columns.Count - 1)
                    {
                        var newRow = dataTable.NewRow();
                        for (int i = 0; i < values.Length; i++)
                        {
                            newRow[i] = values[i];
                        }
                        newRow["Classification"] = "Unclassified"; // Default value
                        dataTable.Rows.Add(newRow);
                    }
                }
            }

            dataGridView.DataSource = dataTable;
        }

        /// <summary>
        /// The ParseCsvLine
        /// </summary>
        /// <param name="line">The line<see cref="string"/></param>
        /// <returns>The <see cref="string[]"/></returns>
        private string[] ParseCsvLine(string line)
        {
            var values = new List<string>();
            bool inQuotes = false;
            string current = "";

            foreach (char c in line)
            {
                if (c == '"') inQuotes = !inQuotes;
                else if (c == ',' && !inQuotes)
                {
                    values.Add(current.Trim());
                    current = "";
                }
                else
                {
                    current += c;
                }
            }
            values.Add(current.Trim());
            return values.ToArray();
        }

        /// <summary>
        /// The BtnSearch_Click
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="EventArgs"/></param>
        private void BtnSearch_Click(object sender, EventArgs e)
        {
            string searchTerm = txtSearch.Text.ToLower();
            if (string.IsNullOrEmpty(searchTerm)) return;

            var filteredRows = dataTable.AsEnumerable()
                .Where(row => row.ItemArray.Any(field => field.ToString().ToLower().Contains(searchTerm)))
                .CopyToDataTable();

            dataGridView.DataSource = filteredRows;
        }

        /// <summary>
        /// The BtnClassify_Click
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="EventArgs"/></param>
        private void BtnClassify_Click(object sender, EventArgs e)
        {
            foreach (DataRow row in dataTable.Rows)
            {
                string rawMetadata = row["metadata"].ToString();

                // Extract 'format' from metadata JSON-like string
                string format = ExtractFormat(rawMetadata);

                var input = new OCRData
                {
                    Metadata = format,  // Use extracted format only
                    JsonOutput = row["true_json_output"].ToString()
                };

                var prediction = predictionEngine.Predict(input);
                row["Classification"] = prediction.Category;
            }

            dataGridView.DataSource = dataTable;
            // Calculate and display metrics
            DisplayClassificationMetrics();
        }

        // Function to extract 'format' from metadata

        /// <summary>
        /// The ExtractFormat
        /// </summary>
        /// <param name="metadata">The metadata<see cref="string"/></param>
        /// <returns>The <see cref="string"/></returns>
        private string ExtractFormat(string metadata)
        {
            if (metadata.Contains("format:"))
            {
                int startIndex = metadata.IndexOf("format:") + 7;
                int endIndex = metadata.IndexOf(',', startIndex);
                if (endIndex == -1) endIndex = metadata.IndexOf('}', startIndex);
                if (endIndex > startIndex)
                    return metadata.Substring(startIndex, endIndex - startIndex).Trim();
            }
            return metadata;  // Return original string if extraction fails
        }

        /// <summary>
        /// The BtnGroup_Click
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="EventArgs"/></param>
        private void BtnGroup_Click(object sender, EventArgs e)
        {
            groupedTable = new DataTable();
            groupedTable.Columns.Add("Metadata", typeof(string));
            groupedTable.Columns.Add("Count", typeof(int));

            var groupedData = dataTable.AsEnumerable()
                .GroupBy(row => row["metadata"].ToString())
                .Select(group => new { Key = group.Key, Count = group.Count() })
                .ToList();

            foreach (var item in groupedData)
            {
                groupedTable.Rows.Add(item.Key, item.Count);
            }

            dataGridView.DataSource = groupedTable;
        }

        /// <summary>
        /// The DataGridView_CellClick
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="DataGridViewCellEventArgs"/></param>
        private void DataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                string cellValue = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString();
                richTextBoxResults.Text = cellValue;
            }
        }

        /// <summary>
        /// The DataGridView_SelectionChanged
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="EventArgs"/></param>
        private void DataGridView_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count > 0)
            {
                var selectedRow = dataGridView.SelectedRows[0];
                string metadata = selectedRow.Cells["metadata"].Value.ToString();
                // UpdateMetricsListBoxForDocument(metadata);
            }
        }

        /// <summary>
        /// The DisplayClassificationMetrics
        /// </summary>
        private void DisplayClassificationMetrics()
        {
            metricsListBox.Items.Clear();
            // Store metrics values and colors in tag objects
            metricsListBox.DrawMode = DrawMode.OwnerDrawFixed;

            // Remove existing handler if any to avoid duplicates
            metricsListBox.DrawItem -= metricsListBox_DrawItem;
            metricsListBox.DrawItem += metricsListBox_DrawItem;

            // Retrieve classification metrics
            var predictions = model.Transform(mlContext.Data.LoadFromEnumerable(dataTable.AsEnumerable().Select(row => new OCRData
            {
                Metadata = row["metadata"].ToString(),
                JsonOutput = row["true_json_output"].ToString(),
                Category = row["Classification"].ToString()
            })));
            metrics = mlContext.MulticlassClassification.Evaluate(predictions);

            // Add headers
            metricsListBox.Items.Add("== MODEL PERFORMANCE METRICS ==");
            metricsListBox.Items.Add("");

            // Add metrics with their display text and color stored in tags
            string macroText = $"Macro Accuracy: {metrics.MacroAccuracy:P2}";
            Color macroColor = metrics.MacroAccuracy >= 0.75 ? Color.Green :
                              (metrics.MacroAccuracy < 0.65 ? Color.Red : Color.Orange);
            metricsListBox.Items.Add(new ColoredListItem(macroText, macroColor));

            string microText = $"Micro Accuracy: {metrics.MicroAccuracy:P2}";
            Color microColor = metrics.MicroAccuracy >= 0.8 ? Color.Green :
                              (metrics.MicroAccuracy < 0.7 ? Color.Red : Color.Orange);
            metricsListBox.Items.Add(new ColoredListItem(microText, microColor));

            string logLossText = $"Log Loss: {metrics.LogLoss:F4}";
            Color logLossColor = metrics.LogLoss <= 0.5 ? Color.Green :
                                (metrics.LogLoss > 1.0 ? Color.Red : Color.Orange);
            metricsListBox.Items.Add(new ColoredListItem(logLossText, logLossColor));

            if (metrics.PerClassLogLoss != null)
            {
                metricsListBox.Items.Add("");
                metricsListBox.Items.Add("Per-class Log Loss:");
                metricsListBox.Items.Add("------------------");
                for (int i = 0; i < metrics.PerClassLogLoss.Count; i++)
                {
                    string perClassText = $"Class {i}: {metrics.PerClassLogLoss[i]:F4}";
                    Color perClassColor = metrics.PerClassLogLoss[i] <= 0.5 ? Color.Green :
                                         (metrics.PerClassLogLoss[i] > 1.0 ? Color.Red : Color.Orange);
                    metricsListBox.Items.Add(new ColoredListItem(perClassText, perClassColor));
                }
            }

            // Remove any existing selection handler and add a new one
            metricsListBox.SelectedIndexChanged -= metricsListBox_SelectedIndexChanged_1;
            metricsListBox.SelectedIndexChanged += metricsListBox_SelectedIndexChanged_1;
        }

        // Helper class to store text and color information

        /// <summary>
        /// Defines the <see cref="ColoredListItem" />
        /// </summary>
        private class ColoredListItem
        {
            /// <summary>
            /// Gets or sets the Text
            /// </summary>
            public string Text { get; set; }

            /// <summary>
            /// Gets or sets the TextColor
            /// </summary>
            public Color TextColor { get; set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="ColoredListItem"/> class.
            /// </summary>
            /// <param name="text">The text<see cref="string"/></param>
            /// <param name="color">The color<see cref="Color"/></param>
            public ColoredListItem(string text, Color color)
            {
                Text = text;
                TextColor = color;
            }

            /// <summary>
            /// The ToString
            /// </summary>
            /// <returns>The <see cref="string"/></returns>
            public override string ToString()
            {
                return Text;
            }
        }

        /// <summary>
        /// The GetCategoryFromIndex
        /// </summary>
        /// <param name="index">The index<see cref="int"/></param>
        /// <returns>The <see cref="string"/></returns>
        private string GetCategoryFromIndex(int index)
        {
            var categories = new List<string>
            {
                "Financial", "Transport", "Healthcare", "Legal",
                "General", "Ranking", "Education", "Health & Food",
                "Property", "Workforce"
            };

            return index < categories.Count ? categories[index] : $"Category {index}";
        }

        /// <summary>
        /// The TrainModel
        /// </summary>
        private void TrainModel()
        {
            //var trainingData = new[]
            //{
            //    new OCRData { Metadata = "TABLE", JsonOutput = "sample", Category = "Financial" },
            //    new OCRData { Metadata = "INVOICE", JsonOutput = "sample", Category = "Billing" },
            //    new OCRData { Metadata = "TABLE", JsonOutput = "sample", Category = "RANKING" },
            //};
            var trainingData = new[]
                  {
	// Financial category
	new OCRData { Metadata = "BANK_CHECK", JsonOutput = "contains account number, amount, signature", Category = "Financial" },
    new OCRData { Metadata = "ACCOUNT_STATEMENT", JsonOutput = "contains transactions, balance, account details", Category = "Financial" },
    new OCRData { Metadata = "CREDIT_CARD_STATEMENT", JsonOutput = "contains credit card number, transactions, payment due", Category = "Financial" },
    new OCRData { Metadata = "INVOICE_PAYMENT", JsonOutput = "contains payment amount, invoice number, due date", Category = "Financial" },
    new OCRData { Metadata = "BANK_RECEIPT", JsonOutput = "contains transaction details, account info, timestamp", Category = "Financial" },

	//Sales
	new OCRData { Metadata = "PAY_IN_SHEET", JsonOutput = "contains payment details, legal references, verification", Category = "Sales" },
	
	// Transport category
	new OCRData { Metadata = "SHIPPING_INVOICE", JsonOutput = "contains tracking number, delivery address, shipping cost", Category = "Transport" },
    new OCRData { Metadata = "DELIVERY_NOTE", JsonOutput = "contains package details, delivery instructions, recipient", Category = "Transport" },
    new OCRData { Metadata = "BILL_OF_LADING", JsonOutput = "contains cargo details, shipping routes, vessel information", Category = "Transport" },
    new OCRData { Metadata = "FREIGHT_RECEIPT", JsonOutput = "contains freight charges, shipping details, carrier info", Category = "Transport" },
    new OCRData { Metadata = "TRANSPORT_MANIFEST", JsonOutput = "contains vehicle details, route information, cargo list", Category = "Transport" },
	
	// Healthcare category
	
	new OCRData { Metadata = "EQUIPMENT_INSPECTION", JsonOutput = "contains medical equipment details, inspection results, compliance", Category = "Healthcare" },
    new OCRData { Metadata = "PATIENT_INTAKE", JsonOutput = "contains patient details, medical history, symptoms", Category = "Healthcare" },
    new OCRData { Metadata = "MEDICAL_REPORT", JsonOutput = "contains diagnosis, treatment plan, doctor recommendations", Category = "Healthcare" },
    new OCRData { Metadata = "PRESCRIPTION", JsonOutput = "contains medication details, dosage, patient name", Category = "Healthcare" },
	
	// Legal category
	new OCRData { Metadata = "FORM_1040", JsonOutput = "contains taxpayer information, income details, deductions", Category = "Legal" },
    new OCRData { Metadata = "PROXY_VOTING", JsonOutput = "contains shareholder details, voting instructions, meeting date", Category = "Legal" },
    new OCRData { Metadata = "COMMERCIAL_LEASE_AGREEMENT", JsonOutput = "contains property details, lease terms, signatures", Category = "Legal" },
    new OCRData { Metadata = "CONTRACT", JsonOutput = "contains terms and conditions, party details, effective date", Category = "Legal" },
    new OCRData { Metadata = "PETITION_FORM", JsonOutput = "contains terms and conditions, party details, effective date", Category = "Legal" },
    new OCRData { Metadata = "PATENT", JsonOutput = "contains medical device specifications, diagrams, claims", Category = "Legal" },
	
	// General category
	new OCRData { Metadata = "PHOTO_DOC", JsonOutput = "contains image of document, text extraction results", Category = "General" },
    new OCRData { Metadata = "PHOTO_TABLE", JsonOutput = "contains image of table, structured data extraction", Category = "General" },
    new OCRData { Metadata = "PHOTO_RECEIPT", JsonOutput = "contains image of receipt, item list, total amount", Category = "General" },
    new OCRData { Metadata = "GENERAL_LETTER", JsonOutput = "contains correspondence details, date, signature", Category = "General" },
    new OCRData { Metadata = "MEMO", JsonOutput = "contains brief message, sender details, date", Category = "General" },
	
	// Ranking category
	new OCRData { Metadata = "TABLE", JsonOutput = "contains rows, columns, structured data", Category = "Ranking" },
    new OCRData { Metadata = "CHART", JsonOutput = "contains visual representation, data points, labels", Category = "Ranking" },
    new OCRData { Metadata = "RANKING", JsonOutput = "contains ordered list, scores, positions", Category = "Ranking" },
    new OCRData { Metadata = "LEADERBOARD", JsonOutput = "contains rankings, names, performance metrics", Category = "Ranking" },
    new OCRData { Metadata = "PERFORMANCE_METRICS", JsonOutput = "contains KPIs, comparative analysis, benchmarks", Category = "Ranking" },
	
	// Education category
	new OCRData { Metadata = "GLOSSARY", JsonOutput = "contains terms, definitions, references", Category = "Education" },
    new OCRData { Metadata = "SYLLABUS", JsonOutput = "contains course details, schedule, requirements", Category = "Education" },
    new OCRData { Metadata = "ACADEMIC_TRANSCRIPT", JsonOutput = "contains grades, courses, student information", Category = "Education" },
    new OCRData { Metadata = "LESSON_PLAN", JsonOutput = "contains learning objectives, activities, materials", Category = "Education" },
    new OCRData { Metadata = "EDUCATIONAL_CERTIFICATE", JsonOutput = "contains qualification details, institution, date", Category = "Education" },
	
	// Health & Food category
	new OCRData { Metadata = "NUTRITION", JsonOutput = "contains calorie information, ingredients, serving size", Category = "Health & Food" },
    new OCRData { Metadata = "FOOD_LABEL", JsonOutput = "contains nutritional facts, allergens, ingredients", Category = "Health & Food" },
    new OCRData { Metadata = "MENU", JsonOutput = "contains food items, prices, descriptions", Category = "Health & Food" },
    new OCRData { Metadata = "DIET_PLAN", JsonOutput = "contains meal schedule, calorie targets, food recommendations", Category = "Health & Food" },
    new OCRData { Metadata = "RECIPE", JsonOutput = "contains ingredients, instructions, cooking time", Category = "Health & Food" },
	
	// Property category
	new OCRData { Metadata = "REAL_ESTATE", JsonOutput = "contains property details, price, location", Category = "Property" },
    new OCRData { Metadata = "PROPERTY_DEED", JsonOutput = "contains ownership details, property description, signatures", Category = "Property" },
    new OCRData { Metadata = "MORTGAGE_DOCUMENT", JsonOutput = "contains loan terms, property details, payment schedule", Category = "Property" },
    new OCRData { Metadata = "PROPERTY_LISTING", JsonOutput = "contains property features, price, contact information", Category = "Property" },
    new OCRData { Metadata = "PROPERTY_ASSESSMENT", JsonOutput = "contains valuation details, tax information, property attributes", Category = "Property" },
	
	// Workforce category
	new OCRData { Metadata = "SHIFT_SCHEDULE", JsonOutput = "contains work hours, employee assignments, dates", Category = "Workforce" },
    new OCRData { Metadata = "EMPLOYEE_RECORD", JsonOutput = "contains personnel details, employment history, skills", Category = "Workforce" },
    new OCRData { Metadata = "PAYROLL", JsonOutput = "contains salary details, deductions, employee information", Category = "Workforce" },
    new OCRData { Metadata = "JOB_DESCRIPTION", JsonOutput = "contains role details, responsibilities, requirements", Category = "Workforce" },
    new OCRData { Metadata = "PERFORMANCE_REVIEW", JsonOutput = "contains evaluation metrics, feedback, goals", Category = "Workforce" }
};

            IDataView dataView = mlContext.Data.LoadFromEnumerable(trainingData);
            var pipeline = mlContext.Transforms.Conversion.MapValueToKey("Label", "Category")
                .Append(mlContext.Transforms.Text.FeaturizeText("Features", "Metadata"))
                .Append(mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy())
                .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

            model = pipeline.Fit(dataView);
            predictionEngine = mlContext.Model.CreatePredictionEngine<OCRData, OCRPrediction>(model);
            // Evaluate model
            var predictions = model.Transform(dataView);
            dynamic metrics = mlContext.MulticlassClassification.Evaluate(predictions);

            // Capture prediction scores
            foreach (var data in trainingData)
            {
                var prediction = predictionEngine.Predict(data);
                var scores = new List<PredictionScore>();
                for (int i = 0; i < prediction.Score.Length; i++)
                {
                    string categoryName = GetCategoryFromIndex(i);
                    scores.Add(new PredictionScore
                    {
                        Category = categoryName,
                        Score = prediction.Score[i],
                        Confidence = prediction.Score[i] * 100 / prediction.Score.Sum()
                    });
                }
                classPredictions[data.Metadata] = scores.OrderByDescending(s => s.Score).ToList();
            }
        }

        /// <summary>
        /// The metricsListBox_SelectedIndexChanged_1
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="EventArgs"/></param>
        private void metricsListBox_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (metricsListBox.SelectedIndex < 0) return;

            object selectedItem = metricsListBox.SelectedItem;
            string selectedText = selectedItem.ToString();
            string message = "";
            string title = "";

            if (selectedText.Contains("Macro Accuracy"))
            {
                title = "Macro Accuracy Thresholds";
                message = "Macro Accuracy gives equal weight to each class.\n\n" +
                          "Good values: Generally above 0.75 (75%)\n" +
                          "Concerns if: Below 0.65 (65%)\n\n" +
                          "This metric is better for understanding performance across all classes, especially when classes are imbalanced.";
            }
            else if (selectedText.Contains("Micro Accuracy"))
            {
                title = "Micro Accuracy Thresholds";
                message = "Micro Accuracy gives equal weight to each sample.\n\n" +
                          "Good values: Generally above 0.8 (80%)\n" +
                          "Concerns if: Below 0.7 (70%)\n\n" +
                          "This metric is better when classes are imbalanced and you care about overall performance.";
            }
            else if (selectedText.Contains("Log Loss") && !selectedText.Contains("Per-class"))
            {
                title = "Log Loss Thresholds";
                message = "Log Loss evaluates the quality of predicted probabilities.\n\n" +
                          "Good values: Lower numbers are better, typically below 0.5\n" +
                          "Concerns if: Above 1.0\n\n" +
                          "Unlike accuracy metrics, log-loss penalizes confident incorrect predictions more severely.";
            }
            else if (selectedText.Contains("Class") && selectedText.Contains(":"))
            {
                title = "Per-class Log Loss";
                message = "Per-class Log Loss shows the log loss for each individual class.\n\n" +
                          "Good values: Lower numbers are better, typically below 0.5\n" +
                          "Concerns if: Above 1.0\n\n" +
                          "High values for specific classes indicate poor probability estimates for those classes.";
            }

            if (!string.IsNullOrEmpty(message))
            {
                // MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
                ShowCustomMessageBox(message, title);
            }
        }

        /// <summary>
        /// The metricsListBox_DrawItem
        /// </summary>
        /// <param name="sender">The sender<see cref="object"/></param>
        /// <param name="e">The e<see cref="DrawItemEventArgs"/></param>
        private void metricsListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            // Check if the item index is valid
            if (e.Index < 0) return;

            // Draw the background
            e.DrawBackground();

            // Get the item from the collection
            object item = metricsListBox.Items[e.Index];

            // Set appropriate text color
            Color textColor = Color.Black; // Default color
            string text;

            if (item is ColoredListItem coloredItem)
            {
                textColor = coloredItem.TextColor;
                text = coloredItem.Text;
            }
            else
            {
                text = item.ToString();
            }

            // Create a brush with the specified color
            using (Brush brush = new SolidBrush(textColor))
            {
                // Draw the text
                e.Graphics.DrawString(text, e.Font, brush, e.Bounds);
            }

            // If the item is selected, draw the focus rectangle
            e.DrawFocusRectangle();
        }

        private void ShowCustomMessageBox(string message, string title)
        {
            using (Form customMsgBox = new Form())
            {
                // Set up the form
                customMsgBox.Text = title;
                customMsgBox.Size = new Size(450, 300);
                customMsgBox.StartPosition = FormStartPosition.CenterParent;
                customMsgBox.MinimizeBox = false;
                customMsgBox.MaximizeBox = false;
                customMsgBox.FormBorderStyle = FormBorderStyle.FixedDialog;
                customMsgBox.Icon = SystemIcons.Information;

                // Create the message label with larger font
                Label lblMessage = new Label();
                lblMessage.Text = message;
                lblMessage.Font = new Font("Segoe UI", 12F, FontStyle.Regular); // Larger font size
                lblMessage.AutoSize = false;
                lblMessage.Size = new Size(customMsgBox.Width - 50, customMsgBox.Height - 100);
                lblMessage.Location = new Point(20, 20);
                lblMessage.TextAlign = ContentAlignment.TopLeft;

                // Create the OK button
                Button btnOk = new Button();
                btnOk.Text = "OK";
                btnOk.DialogResult = DialogResult.OK;
                btnOk.Size = new Size(80, 30);
                btnOk.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
                btnOk.Location = new Point((customMsgBox.Width - btnOk.Width) / 2, customMsgBox.Height - 80);

                // Add controls and show dialog
                customMsgBox.Controls.Add(lblMessage);
                customMsgBox.Controls.Add(btnOk);
                customMsgBox.AcceptButton = btnOk;
                customMsgBox.ShowDialog();
            }
        }

        // Option 2: Use TaskDialog (Windows Vista and later)
        private void ShowTaskDialog(string message, string title)
        {
            // Make sure to add reference: System.Windows.Forms.dll (version 4.0.0.0)
            // Requires Windows Vista or later
            TaskDialogPage page = new TaskDialogPage()
            {
                Heading = title,
                Text = message,
                Icon = TaskDialogIcon.Information,
                Buttons = { TaskDialogButton.OK } // Fix: Use collection initializer
            };

            TaskDialog.ShowDialog(this, page);
        }
    }

    /// <summary>
    /// Defines the <see cref="OCRData" />
    /// </summary>
    public class OCRData
    {
        /// <summary>
        /// Gets or sets the Metadata
        /// </summary>
        public string Metadata { get; set; }

        /// <summary>
        /// Gets or sets the JsonOutput
        /// </summary>
        public string JsonOutput { get; set; }

        /// <summary>
        /// Gets or sets the Category
        /// </summary>
        public string Category { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="OCRPrediction" />
    /// </summary>
    public class OCRPrediction
    {
        /// <summary>
        /// Gets or sets the Category
        /// </summary>
        [ColumnName("PredictedLabel")]
        public string Category { get; set; }

        /// <summary>
        /// Gets or sets the Score
        /// </summary>
        public float[] Score { get; set; }
    }

    /// <summary>
    /// Defines the <see cref="PredictionScore" />
    /// </summary>
    public class PredictionScore
    {
        /// <summary>
        /// Gets or sets the Category
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Gets or sets the Score
        /// </summary>
        public float Score { get; set; }

        /// <summary>
        /// Gets or sets the Confidence
        /// </summary>
        public float Confidence { get; set; }
    }

}
