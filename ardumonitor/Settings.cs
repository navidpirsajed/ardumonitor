using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ardumonitor
{
    public partial class Settings : Form
    {
        public string selected_gputemp;
        public string[] Moduleslist = {"GPU Temperature", "GPU Utilization" };
        public Settings()
        {
            InitializeComponent();
        }
        private void Settings_Load(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        public void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {
            Form1 asss = new Form1();
            
            MessageBox.Show(selected_gputemp, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
