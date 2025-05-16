using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

namespace WinFormECMTest
{
    public partial class Form2 : Form
    {
        private WebView2 webView;
        private ComboBox comboBoxFileType;

        public Form2()
        {
            InitializeComponent();
            InitializeControls();
            InitializeWebView();
        }

        private void InitializeControls()
        {
            // Button to open files
            Button buttonOpenFile = new Button
            {
                Text = "Open File",
                Location = new System.Drawing.Point(12, 12),
                Size = new System.Drawing.Size(100, 30)
            };
            buttonOpenFile.Click += buttonOpenFile_Click;
            this.Controls.Add(buttonOpenFile);

            // ComboBox for file type filter
            comboBoxFileType = new ComboBox
            {
                Location = new System.Drawing.Point(120, 12),
                Size = new System.Drawing.Size(200, 30),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            comboBoxFileType.Items.AddRange(new string[] { "All Files", "PDF Files", "Image Files", "Word Files", "Excel Files", "PowerPoint Files" });
            comboBoxFileType.SelectedIndex = 0;
            this.Controls.Add(comboBoxFileType);
        }

        private void InitializeWebView()
        {
            webView = new WebView2
            {
                Dock = DockStyle.Fill,
                Location = new System.Drawing.Point(0, 50),
                Size = new System.Drawing.Size(800, 400)
            };
            this.Controls.Add(webView);

            // Initialize WebView2
            webView.EnsureCoreWebView2Async(null);
        }

        private async void buttonOpenFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                // Set file filter based on ComboBox selection
                switch (comboBoxFileType.SelectedItem.ToString())
                {
                    case "PDF Files":
                        openFileDialog.Filter = "PDF Files (*.pdf)|*.pdf";
                        break;
                    case "Image Files":
                        openFileDialog.Filter = "Image Files (*.png;*.jpg;*.jpeg;*.bmp;*.gif)|*.png;*.jpg;*.jpeg;*.bmp;*.gif";
                        break;
                    case "Word Files":
                        openFileDialog.Filter = "Word Files (*.doc;*.docx)|*.doc;*.docx";
                        break;
                    case "Excel Files":
                        openFileDialog.Filter = "Excel Files (*.xls;*.xlsx)|*.xls;*.xlsx";
                        break;
                    case "PowerPoint Files":
                        openFileDialog.Filter = "PowerPoint Files (*.ppt;*.pptx)|*.ppt;*.pptx";
                        break;
                    default:
                        openFileDialog.Filter = "All Files (*.*)|*.*";
                        break;
                }

                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    string fileExtension = Path.GetExtension(filePath).ToLower();

                    // Ensure WebView2 is initialized
                    await webView.EnsureCoreWebView2Async(null);

                    // Handle file types
                    switch (fileExtension)
                    {
                        case ".pdf":
                            // Directly navigate to the PDF file
                            webView.CoreWebView2.Navigate(filePath);
                            break;

                        case ".png":
                        case ".jpg":
                        case ".jpeg":
                        case ".bmp":
                        case ".gif":
                            // Display image in an HTML page
                            string imageHtml = $"<html><body><img src='{filePath}' style='max-width: 100%; max-height: 100%;'/></body></html>";
                            webView.CoreWebView2.NavigateToString(imageHtml);
                            break;

                        case ".doc":
                        case ".docx":
                        case ".xls":
                        case ".xlsx":
                        case ".ppt":
                        case ".pptx":
                            // For Office files, show a message (you can extend this to convert files to PDF/HTML)
                            MessageBox.Show("Office files are not directly supported. Convert them to PDF or HTML first.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            break;

                        default:
                            // For unsupported files, show a message
                            MessageBox.Show("Unsupported file type.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                    }
                }
            }
        }
    }
}