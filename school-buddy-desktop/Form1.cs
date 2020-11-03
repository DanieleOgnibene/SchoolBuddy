using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace school_buddy_desktop
{
    public partial class Form1 : Form
    {
        bool isConnected = false;
        SerialPort _serialPort;

        private const string getHistoryCommand = "#GET_HISTORY#\n";

        public Form1()
        {
            InitializeComponent();
            DisableControls();
            InitComPorts();
        }

        void InitComPorts()
        {
            var ports = SerialPort.GetPortNames();
            portsComboBox.Items.AddRange(ports);
        }


        private void ConnectionButtonClick(object sender, EventArgs e)
        {
            if (!isConnected)
            {
                Connect();
            }
            else
            {
                Disconnect();
            }
        }

        private void Connect()
        {
            isConnected = true;
            string selectedPort = portsComboBox.GetItemText(portsComboBox.SelectedItem);
            _serialPort = new SerialPort(selectedPort, 9600, Parity.None, 8, StopBits.One);
            _serialPort.Open();
            _serialPort.DataReceived += new SerialDataReceivedEventHandler(SerialDataReceived);
            connectionButton.Text = "Disconnect";
            EnableControls();
        }

        private void Disconnect()
        {
            isConnected = false;
            _serialPort.Close();
            connectionButton.Text = "Connect";
            DisableControls();
            ResetDefaults();
        }

        private void SerialDataReceived(object sender, SerialDataReceivedEventArgs eventArgs)
        {
            int dataLength = _serialPort.BytesToRead;
            byte[] data = new byte[dataLength];
            int nbrDataRead = _serialPort.Read(data, 0, dataLength);
            if (nbrDataRead == 0)
                return;

            DataReceived(data, dataLength);
        }

        private void DataReceived(byte[] data, int length)
        {
            textBox2.Invoke(new Action(() => textBox2.Text = Encoding.ASCII.GetString(data, 0, length)));
        }

        private void EnableControls()
        {
            sendMessage.Enabled = true;
            textBox2.Enabled = true;
            groupBox1.Enabled = true;

        }

        private void DisableControls()
        {
            sendMessage.Enabled = false;
            textBox2.Enabled = false;
            groupBox1.Enabled = false;
        }

        private void ResetDefaults()
        {
            textBox2.Text = "";
        }

        private void GetHistory_Click(object sender, EventArgs e)
        {
            _serialPort.Write(getHistoryCommand);
        }
    }
}
