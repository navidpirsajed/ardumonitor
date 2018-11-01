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
                    sw.WriteLine("COM1");
                    sw.Write("Baud_Rate: ");
                    sw.WriteLine("9600");
                }
            }
            else
            {
                string lines = System.IO.File.ReadAllText(@"config.txt");
                string[] line = lines.Split(new char[] { ':', ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                try
                {
                    int index = 0;
                    foreach (string config in line)
                    {
                        switch(config){
                            case "Port": int i = index + 1; storedPort = line[i]; break;
                            case "Baud_Rate": i = index + 1; storedbaudRate = line[i]; break;
                        }
                        Console.WriteLine(index + config);
                        index++;
                    }
                }
                catch(Exception exp)
                {
                    MessageBox.Show("Error: " + exp, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                comboBox1.Text = storedPort;
                comboBox2.Text = storedbaudRate;
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
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            var GPU_Computer = new Computer();
            GPU_Computer.GPUEnabled = true;
            GPU_Computer.Open();
            foreach (var GPU in GPU_Computer.Hardware)
            {
                foreach (var sensor in GPU.Sensors)
                {
                    if (sensor.SensorType == SensorType.Temperature)
                    {
                        while (true)
                        {
                            if (sensor.Name == "GPU Core")
                            {
                                GPU.Update();
                                Console.WriteLine(sensor.Value.ToString());
                                Thread.Sleep(1000);
                            }
                        }
                    }
                }
            }
                Console.WriteLine();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();
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
