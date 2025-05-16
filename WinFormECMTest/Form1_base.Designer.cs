namespace WinFormECMTest
{
    partial class Form1_base
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1_base));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbAccelerator = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.lblEcmproduct = new System.Windows.Forms.Label();
            this.cmdProductService = new System.Windows.Forms.ComboBox();
            this.cmbProductList = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.btnOpenURL = new CustomControls.CustomButton.CustomButton();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.btnOpenApplication = new CustomControls.CustomButton.CustomButton();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(8, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(106, 1);
            this.pictureBox1.TabIndex = 19;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pictureBox2.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox2.Image")));
            this.pictureBox2.Location = new System.Drawing.Point(1079, 3);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(272, 1);
            this.pictureBox2.TabIndex = 20;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Click += new System.EventHandler(this.pictureBox2_Click);
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.BackColor = System.Drawing.Color.DodgerBlue;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(164, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(858, 1);
            this.label1.TabIndex = 19;
            this.label1.Text = "ECM ACCELERATOR LANDING PAGE";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.974359F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 68.9577F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 22.12991F));
            this.tableLayoutPanel1.Controls.Add(this.pictureBox1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.pictureBox2, 2, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1367, 0);
            this.tableLayoutPanel1.TabIndex = 21;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.label2.Location = new System.Drawing.Point(603, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(199, 1);
            this.label2.TabIndex = 11;
            this.label2.Text = "ECM Product Accelerator";
            // 
            // cmbAccelerator
            // 
            this.cmbAccelerator.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cmbAccelerator.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.cmbAccelerator.FormattingEnabled = true;
            this.cmbAccelerator.Location = new System.Drawing.Point(576, -45);
            this.cmbAccelerator.Name = "cmbAccelerator";
            this.cmbAccelerator.Size = new System.Drawing.Size(252, 33);
            this.cmbAccelerator.TabIndex = 10;
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.label3.Location = new System.Drawing.Point(319, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(168, 1);
            this.label3.TabIndex = 9;
            this.label3.Text = "ECM Product Service";
            // 
            // lblEcmproduct
            // 
            this.lblEcmproduct.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblEcmproduct.AutoSize = true;
            this.lblEcmproduct.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblEcmproduct.Location = new System.Drawing.Point(61, 0);
            this.lblEcmproduct.Name = "lblEcmproduct";
            this.lblEcmproduct.Size = new System.Drawing.Size(138, 1);
            this.lblEcmproduct.TabIndex = 8;
            this.lblEcmproduct.Text = "ECM Product List";
            // 
            // cmdProductService
            // 
            this.cmdProductService.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cmdProductService.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.cmdProductService.FormattingEnabled = true;
            this.cmdProductService.Location = new System.Drawing.Point(282, -45);
            this.cmdProductService.Name = "cmdProductService";
            this.cmdProductService.Size = new System.Drawing.Size(243, 33);
            this.cmdProductService.TabIndex = 7;
            // 
            // cmbProductList
            // 
            this.cmbProductList.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cmbProductList.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.cmbProductList.FormattingEnabled = true;
            this.cmbProductList.Location = new System.Drawing.Point(25, -45);
            this.cmbProductList.Name = "cmbProductList";
            this.cmbProductList.Size = new System.Drawing.Size(209, 33);
            this.cmbProductList.TabIndex = 6;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30.36369F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.50368F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 36.13263F));
            this.tableLayoutPanel2.Controls.Add(this.lblEcmproduct, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.cmbAccelerator, 2, 1);
            this.tableLayoutPanel2.Controls.Add(this.label2, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.cmbProductList, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.label3, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.cmdProductService, 1, 1);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(12, 139);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 59F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(858, 0);
            this.tableLayoutPanel2.TabIndex = 22;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel3.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Location = new System.Drawing.Point(599, 558);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(113, 0);
            this.tableLayoutPanel3.TabIndex = 23;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 1;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.Controls.Add(this.btnOpenURL, 0, 0);
            this.tableLayoutPanel4.Location = new System.Drawing.Point(839, 139);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 1;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(157, 59);
            this.tableLayoutPanel4.TabIndex = 24;
            // 
            // btnOpenURL
            // 
            this.btnOpenURL.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnOpenURL.BackColor = System.Drawing.Color.DodgerBlue;
            this.btnOpenURL.BackgroundColor = System.Drawing.Color.DodgerBlue;
            this.btnOpenURL.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.btnOpenURL.BorderRadius = 25;
            this.btnOpenURL.BorderSize = 0;
            this.btnOpenURL.FlatAppearance.BorderSize = 0;
            this.btnOpenURL.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpenURL.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnOpenURL.ForeColor = System.Drawing.Color.White;
            this.btnOpenURL.Location = new System.Drawing.Point(3, 4);
            this.btnOpenURL.Name = "btnOpenURL";
            this.btnOpenURL.Size = new System.Drawing.Size(151, 51);
            this.btnOpenURL.TabIndex = 13;
            this.btnOpenURL.Text = "Open URL";
            this.btnOpenURL.TextColor = System.Drawing.Color.White;
            this.btnOpenURL.UseVisualStyleBackColor = false;
            this.btnOpenURL.Click += new System.EventHandler(this.btnOpenURL_Click);
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.ColumnCount = 1;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.Controls.Add(this.btnOpenApplication, 0, 0);
            this.tableLayoutPanel5.Location = new System.Drawing.Point(999, 139);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 1;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(170, 55);
            this.tableLayoutPanel5.TabIndex = 25;
            // 
            // btnOpenApplication
            // 
            this.btnOpenApplication.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnOpenApplication.BackColor = System.Drawing.Color.DodgerBlue;
            this.btnOpenApplication.BackgroundColor = System.Drawing.Color.DodgerBlue;
            this.btnOpenApplication.BorderColor = System.Drawing.Color.PaleVioletRed;
            this.btnOpenApplication.BorderRadius = 25;
            this.btnOpenApplication.BorderSize = 0;
            this.btnOpenApplication.FlatAppearance.BorderSize = 0;
            this.btnOpenApplication.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpenApplication.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnOpenApplication.ForeColor = System.Drawing.Color.White;
            this.btnOpenApplication.Location = new System.Drawing.Point(3, 3);
            this.btnOpenApplication.Name = "btnOpenApplication";
            this.btnOpenApplication.Size = new System.Drawing.Size(164, 48);
            this.btnOpenApplication.TabIndex = 12;
            this.btnOpenApplication.Text = "Open Accelerator";
            this.btnOpenApplication.TextColor = System.Drawing.Color.White;
            this.btnOpenApplication.UseVisualStyleBackColor = false;
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(102, 327);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(308, 52);
            this.label4.TabIndex = 26;
            this.label4.Text = "label4";
            // 
            // Form1_base
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1378, 848);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tableLayoutPanel5);
            this.Controls.Add(this.tableLayoutPanel4);
            this.Controls.Add(this.tableLayoutPanel3);
            this.Controls.Add(this.tableLayoutPanel2);
            this.Controls.Add(this.tableLayoutPanel1);
            this.MinimumSize = new System.Drawing.Size(100, 100);
            this.Name = "Form1_base";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_base_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel5.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private CustomControls.CustomButton.CustomButton btnOpenURL;
        private CustomControls.CustomButton.CustomButton btnOpenApplication;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbAccelerator;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblEcmproduct;
        private System.Windows.Forms.ComboBox cmdProductService;
        private System.Windows.Forms.ComboBox cmbProductList;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.Label label4;
    }
}

