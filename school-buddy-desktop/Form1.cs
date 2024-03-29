﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace school_buddy_desktop
{
    public partial class SchoolBuddyMain : Form
    {
        #region CONSTANTS

        private readonly char END_COMMAND_CHAR = '#';
        private readonly char COMMAND_PARAMTERS_SEPARATOR = ';';
        private readonly string COMMAND_OK_RESPONSE = "OK";
        private readonly string GET_HISTORY_COMMAND = "GET_HISTORY_";
        private readonly string GET_DEVICE_INFO_COMMAND = "GET_DEVICE_INFO_";
        private readonly string RESET_DEVICE_COMMAND = "RESET_DEVICE_";
        private readonly string INIT_DEVICE_COMMAND = "INIT_DEVICE_";

        private readonly string WINDOW_DEFAULT_TITLE = "School Buddy";
        private readonly string PORT_TOOL_STRIP_DEFAULT_TEXT = "Port";

        #endregion

        private string _serialData = "";
        private bool waitingSerialResponse = false;

        public SchoolBuddyMain()
        {
            InitializeComponent();
        }

        #region FUNCTIONS

        private void InitFormState()
        {
            InitToolStripComPorts();

            SetDefaultPortToolStripText();

            connectToolStripMenuItem.Enabled = false;
            disconnectToolStripMenuItem.Enabled = false;

            btnHistoryGet.Enabled = false;
            btnHistorySave.Enabled = false;

            btnConfigurationConfigure.Enabled = false;
            btnConfigurationReset.Enabled = false;
        }

        private void InitToolStripComPorts()
        {
            string[] serialPortsNames = SerialPort.GetPortNames();
            toolStripCbPorts.Items.Clear();
            toolStripCbPorts.Items.AddRange(serialPortsNames);
            if (!serialPortsNames.Contains(toolStripCbPorts.Text))
            {
                toolStripCbPorts.Text = "";
            }
        }

        private void CloseSerialConnection()
        {
            if (serialPort.IsOpen)
            {
                try
                {
                    serialPort.Close();
                    SetFormStateDisconnectedReady();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Disconnection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void OpenSerialConnection()
        {
            try
            {
                serialPort.PortName = toolStripCbPorts.Text;
                serialPort.BaudRate = 9600;
                serialPort.Open();
                SetFormStateConnectedReady();
                SendGetDeviceInfoSerialCommand();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SendSerialCommand(string command)
        {
            _serialData = "";
            serialPort.Write(command + END_COMMAND_CHAR);
            SetFormStateConnectedWaitingSerialResponse();
            waitingSerialResponse = true;
        }

        private void SendInitDeviceSerialCommand()
        {
            var timeStamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            Debug.WriteLine(timeStamp);
            var command = INIT_DEVICE_COMMAND + timeStamp + COMMAND_PARAMTERS_SEPARATOR + tBoxConfigurationName.Text;
            Debug.WriteLine(command);
            SendSerialCommand(command);
        }

        private void SendGetDeviceInfoSerialCommand()
        {
            SendSerialCommand(GET_DEVICE_INFO_COMMAND);
        }

        private void SendGetHistorySerialCommand()
        {
            SendSerialCommand(GET_HISTORY_COMMAND);
        }

        private void SendResetDeviceSerialCommand()
        {
            SendSerialCommand(RESET_DEVICE_COMMAND);
        }

        private void SerialDataReceived()
        {
            var newData = serialPort.ReadExisting();
            _serialData += newData;
            if (newData.Contains(END_COMMAND_CHAR))
            {
                Invoke(new EventHandler(SerialDataReceivedCompleteEventHandler));
            }
        }

        private void SerialDataReceivedCompleteEventHandler(object sender, EventArgs e)
        {
            if (_serialData.StartsWith(GET_HISTORY_COMMAND))
            {
                var history = GetSerialResponseFromSerialData(GET_HISTORY_COMMAND);
                SerialGetHistoryComplete(history);
            }
            if (_serialData.StartsWith(GET_DEVICE_INFO_COMMAND))
            {
                var response = GetSerialResponseFromSerialData(GET_DEVICE_INFO_COMMAND);
                SerialGetDeviceInfoComplete(response);
            }
            if (_serialData.StartsWith(RESET_DEVICE_COMMAND))
            {
                var response = GetSerialResponseFromSerialData(RESET_DEVICE_COMMAND);
                SerialResetDeviceComplete(response);
            }
            if (_serialData.StartsWith(INIT_DEVICE_COMMAND))
            {
                var response = GetSerialResponseFromSerialData(INIT_DEVICE_COMMAND);
                SerialInitDeviceComplete(response);
            }
            if (!waitingSerialResponse)
            {
                _serialData = "";
                SetFormStateConnectedReady();
            }
        }

        private void SerialGetHistoryComplete(string response)
        {
            rTxtHistoryResponse.Text = response;
            if (response.Length > 0)
            {
                btnHistorySave.Enabled = true;
                SaveHistoryReceived();
            }
            else
            {
                MessageBox.Show("This device's history is empty.", "History", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            waitingSerialResponse = false;
        }

        private void SerialGetDeviceInfoComplete(string response)
        {
            var deviceInfos = response.Split(COMMAND_PARAMTERS_SEPARATOR);
            var deviceName = deviceInfos[0];
            var deviceMAC = deviceInfos[1];
            var titleDeviceInfo = (deviceName != "" ? (" | " + deviceName) : "") + " | " + deviceMAC;
            ActiveForm.Text = WINDOW_DEFAULT_TITLE + titleDeviceInfo;
            tBoxAboutMac.Text = deviceMAC;
            tBoxAboutName.Text = deviceName;
            tBoxConfigurationName.Text = deviceName;
            tBoxAboutVersion.Text = "1.0.0";
            waitingSerialResponse = false;
        }

        private void SerialResetDeviceComplete(string response)
        {
            if (response.Equals(COMMAND_OK_RESPONSE))
            {
                MessageBox.Show("Device Reset", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SendGetDeviceInfoSerialCommand();
            }
        }

        private void SerialInitDeviceComplete(string response)
        {
            if (response.Equals(COMMAND_OK_RESPONSE))
            {
                MessageBox.Show("Device Init", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SendGetDeviceInfoSerialCommand();
            }
        }

        private void SaveHistoryReceived()
        {
            var saveFilePath = GetSaveHistoryFilePath();
            if (saveFilePath != "")
            {
                WriteHistoryToFile(saveFilePath);
            }
        }

        private string GetSaveHistoryFilePath()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = "1a-e8-de-a3-93-0c  @" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm");
            saveFileDialog.Filter = "Text File|*.csv";
            saveFileDialog.ValidateNames = true;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                return saveFileDialog.FileName;
            }
            return "";
        }

        private void WriteHistoryToFile(string filePath)
        {
            try
            {
                var streamWriter = new StreamWriter(filePath);
                var content = rTxtHistoryResponse.Text;
                var lines = content.Split('\n');
                var header = string.Format("{0};{1}", "MAC Address", "Time");
                streamWriter.WriteLine(header);
                streamWriter.Flush();
                foreach (string line in lines)
                {
                    if (line != "")
                    {
                        var parameters = line.Split(';');
                        var address = parameters[0];
                        Debug.WriteLine(parameters[1]);
                        Debug.WriteLine(Int32.Parse(parameters[1]));
                        var localDateTime = UnixTimeStampToLocalDateTime(Int32.Parse(parameters[1]));
                        var newLine = string.Format("{0};{1}", address, localDateTime);
                        streamWriter.WriteLine(newLine);
                        streamWriter.Flush();
                    }
                }
                streamWriter.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetSerialResponseFromSerialData(string command)
        {
            return _serialData.Replace("" + END_COMMAND_CHAR, "").Replace(command, "");
        }

        private void SetFormStateConnectedWaitingSerialResponse()
        {
            disconnectToolStripMenuItem.Enabled = false;

            btnHistoryGet.Enabled = false;

            btnConfigurationConfigure.Enabled = false;
            btnConfigurationReset.Enabled = false;

            SetWaitCursor();
        }

        private void SetFormStateConnectedReady()
        {
            disconnectToolStripMenuItem.Enabled = true;
            connectToolStripMenuItem.Enabled = false;
            portToolStripMenuItem.Enabled = false;
            progressBarConnection.Value = 100;

            btnHistoryGet.Enabled = true;

            btnConfigurationConfigure.Enabled = true;
            btnConfigurationReset.Enabled = true;

            SetDefaultCursor();
        }

        private void SetFormStateDisconnectedReady()
        {
            SetDefaultWindowTitle();

            disconnectToolStripMenuItem.Enabled = false;
            connectToolStripMenuItem.Enabled = true;
            portToolStripMenuItem.Enabled = true;
            progressBarConnection.Value = 0;

            btnHistoryGet.Enabled = false;

            tBoxAboutMac.Text = "";
            tBoxAboutName.Text = "";
            tBoxAboutVersion.Text = "";

            tBoxConfigurationName.Text = "";
            btnConfigurationConfigure.Enabled = false;
            btnConfigurationReset.Enabled = false;

            SetDefaultCursor();
        }

        private void SetWaitCursor()
        {
            Cursor = Cursors.WaitCursor;
        }

        private void SetDefaultCursor()
        {
            Cursor = Cursors.Default;
        }

        private void SetDefaultWindowTitle()
        {
            ActiveForm.Text = WINDOW_DEFAULT_TITLE;
        }

        private void SetDefaultPortToolStripText()
        {
            portToolStripMenuItem.Text = PORT_TOOL_STRIP_DEFAULT_TEXT;
        }

        private void SetPortToolStripTextWithPortName(string portName)
        {
            portToolStripMenuItem.Text = PORT_TOOL_STRIP_DEFAULT_TEXT + ": \"" + portName + "\"";
        }

        private DateTime UnixTimeStampToLocalDateTime(int unixTimeStamp)
        {
            var dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(unixTimeStamp);
            return dateTimeOffset.LocalDateTime;
        }

        #endregion

        #region GUI

        private void Form1_Load(object sender, EventArgs e)
        {
            InitFormState();
        }

        private void SchoolBuddyMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseSerialConnection();
        }

        private void toolStripCbPorts_DropDownClosed(object sender, EventArgs e)
        {
            var selectedPortName = toolStripCbPorts.Text;
            if (selectedPortName != "")
            {
                connectToolStripMenuItem.Enabled = true;
                SetPortToolStripTextWithPortName(selectedPortName);
            }
            else
            {
                connectToolStripMenuItem.Enabled = false;
                SetDefaultPortToolStripText();
            }
        }

        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenSerialConnection();
        }

        private void disconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseSerialConnection();
        }

        private void btnHistoryGet_Click(object sender, EventArgs e)
        {
            SendGetHistorySerialCommand();
        }

        private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialDataReceived();
        }

        private void btnHistorySave_Click(object sender, EventArgs e)
        {
            SaveHistoryReceived();
        }

        private void toolStripCbPorts_Click(object sender, EventArgs e)
        {
            InitToolStripComPorts();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void btnConfigurationReset_Click(object sender, EventArgs e)
        {
            SendResetDeviceSerialCommand();
        }

        private void btnConfigurationConfigure_Click(object sender, EventArgs e)
        {
            SendInitDeviceSerialCommand();
        }

        #endregion

    }
}
