using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;
using System.Threading;
using System.Diagnostics;
using OpenHardwareMonitor.Hardware;
using System.Text.RegularExpressions;


namespace ardumonitor
{
    public partial class Form1 : Form
    {
        String[] availableports;
        String selectedPort;
        String storedPort;
        string selectedbaudRate;
        string storedbaudRate;
        string stored_gputemp;
        bool running = false;
        public Form1()
        {
            InitializeComponent();
            getAvailablePorts();
            checkconfig();
        }

        void getAvailablePorts()
        {
            availableports = SerialPort.GetPortNames();
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(availableports);
        }

        void checkconfig()
        {
            if (!File.Exists(@"config.txt"))
            {
                using (StreamWriter sw = File.CreateText(@"config.txt"))
                {
                    sw.Write("Port: ");
                    sw.WriteLine("COM");
                    sw.Write("Baud_Rate: ");
                    sw.WriteLine("9600");
                    sw.Write("GPU_Temperature_Enabled: ");
                    sw.WriteLine("false");
                }
            }
            else
            {
                string lines = System.IO.File.ReadAllText(@"config.txt");
                string[] line = lines.Split(new char[] { ':', ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                try
                {
                    int testaaa = 0;
                    foreach (var a in line)
                    {
                        Console.WriteLine("config.txt(" + testaaa + "): " +  line[testaaa]);
                        testaaa++;
                    }
                    storedPort = line[1];
                    storedbaudRate = line[3];
                    stored_gputemp = line[5];



                }
                catch
                {
                    MessageBox.Show("Could Not Retrieve Settings from the config File!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                comboBox1.Text = storedPort;
                comboBox2.Text = storedbaudRate;
                //aa

            }
            {
            }
        }

        void hshake()
        {
            label5.ForeColor = System.Drawing.Color.Green;
            label5.Text = "Establishing";
            SerialPort serial = new SerialPort(selectedPort, int.Parse(selectedbaudRate));
            try
            {
                serial.Open();
                serial.ReadTimeout = 1000;
                serial.Write("ready?#");
                string inData = "";
                inData = serial.ReadTo("#");
                if (string.Equals(inData, "ready") == true)
                {
                    Console.WriteLine("Handshake established");
                    label5.ForeColor = System.Drawing.Color.Green;
                    label5.Text = "Established";
                    backgroundWorker1.RunWorkerAsync();
                }
            }
            catch
            {
                serial.Close();
                MessageBox.Show("Could not open the serial port!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                button1.PerformClick();
                label5.ForeColor = System.Drawing.Color.Red;
                label5.Text = "Failed";
                return;
            }
        }

        public void updateConfig()
        {
            using (StreamWriter sw = File.CreateText(@"config.txt"))
            {
                sw.Write("Port: ");
                sw.WriteLine(selectedPort);
                sw.Write("Baud_Rate: ");
                sw.WriteLine(selectedbaudRate);
                sw.Write("GPU_Temperature_Enabled: ");
                sw.WriteLine(stored_gputemp);
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            while (running == true)
            {
                if(stored_gputemp == "true")
                {

                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedPort = comboBox1.Text;
            Console.WriteLine("Port: " + selectedPort);
            if(selectedPort != storedPort)
            {
                updateConfig();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            availableports = SerialPort.GetPortNames();
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(availableports);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedbaudRate = comboBox2.Text;
            Console.WriteLine("Baud Rate: " + selectedbaudRate);
            if (selectedbaudRate != storedbaudRate)
            {
                updateConfig();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (running == true)
            {
                label4.ForeColor = System.Drawing.Color.Red;
                label4.Text = "Stopped";
                label5.ForeColor = System.Drawing.Color.Red;
                label5.Text = "Not established";
                running = false;
                backgroundWorker1.CancelAsync();

            }
            else if(running == false){
                label4.ForeColor = System.Drawing.Color.Green;
                label4.Text = "Running";
                running = true;
                hshake();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {
            
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            
            
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
