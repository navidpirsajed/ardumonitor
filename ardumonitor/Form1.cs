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
using System.Security.Principal;

namespace ardumonitor
{
    public partial class Form1 : Form
    {
        
        String[] availableports;
        String selectedPort;
        String storedPort;
        String selectedbaudRate;
        string storedbaudRate;
        bool running = false;
        bool isElevated = false;
        public Form1()
        {
            isadmin();
            InitializeComponent();
            getAvailablePorts();
            checkconfig();
        }
        
        void isadmin()
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                isElevated = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            if (!isElevated)
                MessageBox.Show("Error: " + "Some Features require administrative privileges, please restart the program with elevated rights!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            SerialPort serial = new SerialPort(selectedPort, int.Parse(selectedbaudRate));
            try
            {
                serial.Open();
            }catch(Exception exp)
            {
                MessageBox.Show("Error: " + exp, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            double /*Temperature*/GPU_Temp = 0, CPU_Temp = 0,/*Load*/ GPU_load = 0, CPU_load = 0,/*Data*/ FreeRam = 0, UsedRam = 0,/*Clock*/ GPU_CoreClock = 0, CPU_Clock1 = 0;
            Computer c = new Computer();
            c.GPUEnabled = true;
            c.CPUEnabled = true;
            c.RAMEnabled = true;
            c.Open();
            while (true)
            {
                foreach (var h in c.Hardware)
                {
                    h.Update();
                    foreach (var s in h.Sensors)
                    {
                        if (s.SensorType == SensorType.Temperature)//Temperature
                        {
                            if (s.Name == "GPU Core")
                                GPU_Temp = s.Value.GetValueOrDefault();
                            if (s.Name == "CPU Package")
                                CPU_Temp = s.Value.GetValueOrDefault();
                        }
                        if (s.SensorType == SensorType.Load)//Load
                        {
                            if (s.Name == "GPU Core")
                                GPU_load = s.Value.GetValueOrDefault();
                            if (s.Name == "CPU Total")
                                CPU_load = s.Value.GetValueOrDefault();
                        }
                        if (s.SensorType == SensorType.Data)
                        {
                            if (s.Name == "Used Memory")
                                UsedRam = s.Value.GetValueOrDefault();
                            if (s.Name == "Available Memory")
                                FreeRam = s.Value.GetValueOrDefault() * 1000;
                        }
                        if (s.SensorType == SensorType.Clock)
                        {
                            if (s.Name == "GPU Core")
                                GPU_CoreClock = s.Value.GetValueOrDefault();
                            if (s.Name == "CPU Core #1")
                                CPU_Clock1 = s.Value.GetValueOrDefault();
                        }
                    }
                }
                //Console.WriteLine("GPU CORE TEMP: " + GPU_Temp.ToString() + " CPU TEMP: " + CPU_Temp + " GPU Load: " + GPU_load + " CPU Load: " + CPU_load.ToString("0.00") + " Free Ram: " + FreeRam.ToString("0") + " GPU core Clock: " + GPU_CoreClock + " CPU Core 1 Clock: " + CPU_Clock1.ToString("0"));
                try
                {
                    serial.Write("#GPU CORE TEMP#" + GPU_Temp.ToString() + "#CPU TEMP#" + CPU_Temp + "#GPU Load#" + GPU_load + "#CPU Load#" + CPU_load.ToString("0.00") + "#Free Ram#" + FreeRam.ToString("0") + "#GPU core Clock#" + GPU_CoreClock + "#CPU Core 1 Clock#" + CPU_Clock1.ToString("0") + "$");
                }
                catch (Exception exp)
                {
                    MessageBox.Show("Error: " + exp, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                }
                    Thread.Sleep(1000);
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
                running = false;
                button1.Text = "Connect";
                backgroundWorker1.CancelAsync();
            }
            else if(running == false){
                running = true;
                button1.Text = "Disconnect";
                backgroundWorker1.RunWorkerAsync();
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

        private void Form1_Load(object sender, EventArgs e)
        {

        }

    }
}
