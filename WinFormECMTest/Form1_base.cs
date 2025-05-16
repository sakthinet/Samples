using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormECMTest
{
    public partial class Form1_base: Form
    {
        public Form1_base()
        {
            InitializeComponent();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void Form1_base_Load(object sender, EventArgs e)
        {
            int formHeight = this.Height;
            int formWidth = this.Width;
            string screenWidth = Screen.PrimaryScreen.Bounds.Width.ToString();
            
            string screenHeight = Screen.PrimaryScreen.Bounds.Height.ToString();
            label4.Text = ("Resolution: " + formHeight + "x" + formWidth);
            
        }

        private void btnOpenURL_Click(object sender, EventArgs e)
        {

        }
    }
}
