using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AikDemo
{
    public partial class Results : Form
    {
        public Results()
        {
            InitializeComponent();
        }

        public void showText(string text)
        {
            richTextBox1.Text = text;
        }

        private void Results_Load(object sender, EventArgs e)
        {
            richTextBox1.Show();
        }
    }
}
