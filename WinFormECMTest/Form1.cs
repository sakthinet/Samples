using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsTestECM
{
    public partial class Form1 : Form
    {
        private Dictionary<string, Dictionary<string, Dictionary<string, string>>> ecmData;
        public Form1()
        {
            InitializeComponent();
            this.Load += new EventHandler(Form1_Load);
            InitializeECMData();
            LoadECMProducts();
        }
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        //private async void Form2_Load(object sender, EventArgs e)
        //{
        //    await InitializeWebViewAsync();
        //}

        private void InitializeECMData()
        {
            ecmData = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>
            {
                { "OpenText", new Dictionary<string, Dictionary<string, string>>
                    {
                        { "Documentum", new Dictionary<string, string>
                            {
                                { "Docxfer", "E:\\Programming\\TestJframe.jar" },
                                { "Vignette", "https://www.opentext.com/about/brands/vignette" }
                            }
                        }
                    }
                },
                { "Hyland", new Dictionary<string, Dictionary<string, string>>
                    {
                        { "Alfresco", new Dictionary<string, string>
                            {
                                { "Alfresco Import/Export", "E:\\Programming\\TestJframe.jar" },
                                { "Alfresco Web", "https://www.hyland.com/en/products/alfresco-platform" }
                            }
                        }
                    }
                }
            };
        }


        private async Task InitializeWebViewAsync()
        {
            try
            {
                if (webView21 == null)
                {
                    MessageBox.Show("WebView2 control is missing from the form.");
                    return;
                }

                await webView21.EnsureCoreWebView2Async();

                if (webView21.CoreWebView2 != null)
                {
                    webView21.CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"WebView2 Initialization Error: {ex.Message}");
            }
        }

        private void CoreWebView2_NavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            if (webView21 != null && webView21.CoreWebView2 != null && txtAddressbar != null)
            {
                txtAddressbar.Text = webView21.CoreWebView2.Source;
            }
        }
        private void LoadECMProducts()
        {
            cmbProductList.Items.Clear();
            cmbProductList.Items.AddRange(ecmData.Keys.ToArray());
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string filepathtoJar = "E:\\Programming\\javajar\\Calculator.jar";
            OpenFileDialog od = new OpenFileDialog();      //
                                                           //if (od.ShowDialog() == DialogResult.OK)        //
                                                           //{                                              //   
                                                           // Process proc = Process.Start(@"give your program address here");
            Process proc = Process.Start(filepathtoJar);  //
            proc.WaitForInputIdle();

            while (proc.MainWindowHandle == IntPtr.Zero)
            {
                Thread.Sleep(100);
                proc.Refresh();
            }

            SetParent(proc.MainWindowHandle, this.panel1.Handle);
            // SetParent(proc.MainWindowHandle, this.Handle);
            // }                                              //
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            await InitializeWebViewAsync();
            //panel1.Visible = false;
            // panel3.Visible = false;

        }

        private void btnOpenApplication_Click(object sender, EventArgs e)
        {
            panel1.Visible = true;
            // panel3.Controls.Clear();
            //panel3.Visible = false;
            string filepathtoJar = "E:\\Programming\\TestJframe.jar";
            OpenFileDialog od = new OpenFileDialog();      //
                                                           //if (od.ShowDialog() == DialogResult.OK)        //
                                                           //{                                              //   
                                                           // Process proc = Process.Start(@"give your program address here");
            Process proc = Process.Start(filepathtoJar);  //
            proc.WaitForInputIdle();

            while (proc.MainWindowHandle == IntPtr.Zero)
            {
                Thread.Sleep(100);
                proc.Refresh();
            }

            SetParent(proc.MainWindowHandle, this.panel1.Handle);
        }

        private void btnOpenURL_Click_1(object sender, EventArgs e)
        {
            //  panel1.Controls.Clear();
            //  panel1.Visible = false;
            //  panel3.Visible = true;

            if (webView21 != null && webView21.CoreWebView2 != null)
            {
                webView21.CoreWebView2.Navigate("https://www.hyland.com/en/products/alfresco-platform");
            }
            else
            {
                MessageBox.Show("WebView2 is not initialized.");
            }

        }

        private void txtAddressbar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && webView21 != null && webView21.CoreWebView2 != null)
            {
                webView21.CoreWebView2.Navigate(txtAddressbar.Text);
                txtAddressbar.Font = new Font("Arial", 10, FontStyle.Regular);
            }
        }

        private void btnRefreshButton_Click(object sender, EventArgs e)
        {
            if (webView21 != null && webView21.CoreWebView2 != null)
            {
                webView21.CoreWebView2.Reload();
            }
        }

        private void btnBackButton_Click(object sender, EventArgs e)
        {
            if (webView21 != null && webView21.CoreWebView2 != null && webView21.CoreWebView2.CanGoBack)
            {
                webView21.CoreWebView2.GoBack();
            }
        }

        private void btnOpenApplication_Click_1(object sender, EventArgs e)
        {
            string filepathtoJar = "D:\\Programming\\javajar\\Calculator.jar";
            OpenFileDialog od = new OpenFileDialog();      //
                                                           //if (od.ShowDialog() == DialogResult.OK)        //
                                                           //{                                              //   
                                                           // Process proc = Process.Start(@"give your program address here");
            Process proc = Process.Start(filepathtoJar);  //
            proc.WaitForInputIdle();

            while (proc.MainWindowHandle == IntPtr.Zero)
            {
                Thread.Sleep(100);
                proc.Refresh();
            }

            SetParent(proc.MainWindowHandle, this.panel1.Handle);
            // SetParent(proc.MainWindowHandle, this.Handle);
            // }       
        }
        private void OpenURL(string url)
        {
            try
            {
                if (webView21.CoreWebView2 == null)
                {
                    MessageBox.Show("WebView2 is not initialized. Try restarting the application.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(url))
                {
                    MessageBox.Show("Invalid URL: The URL is empty.");
                    return;
                }

                if (!Uri.IsWellFormedUriString(url, UriKind.Absolute))
                {
                    MessageBox.Show("Invalid URL format. Ensure it starts with http:// or https://");
                    return;
                }

                webView21.CoreWebView2.Navigate(url);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading URL: {ex.Message}");
            }
        }

        private async void btnOpenURL_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Text Files (*.txt)|*.txt|HTML Files (*.html)|*.html|All Files (*.*)|*.*";
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = openFileDialog.FileName;
                    string fileContent = File.ReadAllText(filePath);

                    // Ensure WebView2 is initialized
                    await webView21.EnsureCoreWebView2Async(null);

                    // Display the file content in WebView2
                    if (Path.GetExtension(filePath).ToLower() == ".html")
                    {
                        // If it's an HTML file, navigate to the file
                        webView21.CoreWebView2.Navigate(filePath);
                    }
                    else
                    {
                        // For non-HTML files, display the content in a simple HTML page
                        string htmlContent = $"<html><body><pre>{fileContent}</pre></body></html>";
                        webView21.CoreWebView2.NavigateToString(htmlContent);
                    }
                }
            }
            // First, try to use the URL from the address bar if it's not empty
            if (!string.IsNullOrWhiteSpace(txtAddressbar.Text))
            {
                OpenURL(txtAddressbar.Text);
                return;
            }

            // If address bar is empty, use the combo box selections
            if (cmbProductList.SelectedItem == null || cmdProductService.SelectedItem == null || cmbAccelerator.SelectedItem == null)
            {
                MessageBox.Show("Please select ECM Product, Service, and Accelerator or enter a URL in the address bar.");
                return;
            }

            var product = cmbProductList.SelectedItem.ToString();
            var service = cmdProductService.SelectedItem.ToString();
            var accelerator = cmbAccelerator.SelectedItem.ToString();
            var path = ecmData[product][service][accelerator];

            if (path.StartsWith("http"))
            {
                OpenURL(path);
            }
            else
            {
                MessageBox.Show("Invalid URL.");
            }
        }

        private void cmbProductList_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmdProductService.Items.Clear();
            if (cmbProductList.SelectedItem != null && ecmData.ContainsKey(cmbProductList.SelectedItem.ToString()))
            {
                cmdProductService.Items.AddRange(ecmData[cmbProductList.SelectedItem.ToString()].Keys.ToArray());
            }
        }

        private void cmdProductService_SelectedIndexChanged(object sender, EventArgs e)
        {
            cmbAccelerator.Items.Clear();
            if (cmbProductList.SelectedItem != null && cmdProductService.SelectedItem != null)
            {
                var product = cmbProductList.SelectedItem.ToString();
                var service = cmdProductService.SelectedItem.ToString();
                if (ecmData[product].ContainsKey(service))
                {
                    cmbAccelerator.Items.AddRange(ecmData[product][service].Keys.ToArray());
                }
            }
        }

        private void txtAddressbar_KeyPress(object sender, KeyPressEventArgs e)
        {
            //if (webView21 != null && webView21.CoreWebView2 != null)
            //{
            //    webView21.CoreWebView2.Navigate(txtAddressbar.Text);
            //}
            //else
            //{
            //    MessageBox.Show("WebView2 is not initialized.");
            //}
        }

        private void txtAddressbar_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                OpenURL(txtAddressbar.Text);
            }
        }

        private void btnBackButton_Click_1(object sender, EventArgs e)
        {

            if (webView21 != null && webView21.CoreWebView2 != null && webView21.CoreWebView2.CanGoBack)
            {
                webView21.CoreWebView2.GoBack();

                // Add a small delay to update the address bar in case the NavigationCompleted event doesn't fire
                Task.Delay(500).ContinueWith(_ =>
                {
                    if (txtAddressbar.InvokeRequired)
                    {
                        txtAddressbar.Invoke(new Action(() =>
                        {
                            txtAddressbar.Text = webView21.CoreWebView2.Source;
                        }));
                    }
                    else
                    {
                        txtAddressbar.Text = webView21.CoreWebView2.Source;
                    }
                });
            }
        }
        //private void CoreWebView2_NavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        //{
        //    if (webView21 != null && webView21.CoreWebView2 != null && txtAddressbar != null)
        //    {
        //        txtAddressbar.Text = webView21.CoreWebView2.Source;
        //    }
        //}

    }
}


//i have put the comments so if someone wants to directly put address remove the lines which have "//" at the end and use the line which has "//" in the beginning