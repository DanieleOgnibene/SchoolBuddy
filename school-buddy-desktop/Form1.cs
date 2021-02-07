using System;
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
using System.Security.Cryptography;


namespace school_buddy_desktop
{
    public partial class SchoolBuddyMain : Form
    {
        #region CONSTANTS

        private readonly string END_COMMAND = "END_COMMAND";
        private readonly char COMMAND_PARAMTERS_SEPARATOR = ';';
        private readonly string GET_HISTORY_END_FILE_SEPARATOR = "END_FILE";
        private readonly string COMMAND_OK_RESPONSE = "OK";
        private readonly string GET_HISTORY_COMMAND = "GET_HISTORY_";
        private readonly string GET_DEVICE_INFO_COMMAND = "GET_DEVICE_INFO_";
        private readonly string RESET_DEVICE_COMMAND = "RESET_DEVICE_";
        private readonly string INIT_DEVICE_COMMAND = "INIT_DEVICE_";
        private readonly string HISTORY_RECORD_END_LINE = "END_LINE";
        private readonly string HISTORY_RECORD_PARAMETER_SEPARATOR = "SEPARATOR";

        private readonly string WINDOW_DEFAULT_TITLE = "School Buddy";
        private readonly string PORT_TOOL_STRIP_DEFAULT_TEXT = "Port";

        #endregion

        private string _serialData = "";
        private bool _waitingSerialResponse = false;
        private string _lastDeviceMAC = "";

        public SchoolBuddyMain()
        {
            InitializeComponent();
        }

        #region FUNCTIONS

        private void InitFormState()
        {
            SetDefaultPortToolStripText();

            connectToolStripMenuItem.Enabled = false;
            disconnectToolStripMenuItem.Enabled = false;

            btnHistoryGet.Enabled = false;
            btnHistorySave.Enabled = false;

            btnConfigurationConfigure.Enabled = false;
            btnConfigurationReset.Enabled = false;

            InitToolStripComPorts();
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
            if (serialPortsNames.Length == 1)
            {
                toolStripCbPorts.Text = serialPortsNames[0];
                UpdateSelectedPortName();
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
            serialPort.Write(command + END_COMMAND);
            SetFormStateConnectedWaitingSerialResponse();
            _waitingSerialResponse = true;
        }

        private void SendInitDeviceSerialCommand()
        {
            var timeStamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            var command = INIT_DEVICE_COMMAND + timeStamp + COMMAND_PARAMTERS_SEPARATOR + tBoxConfigurationName.Text;
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
            if (_serialData.EndsWith(END_COMMAND))
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
            if (!_waitingSerialResponse)
            {
                _serialData = "";
                SetFormStateConnectedReady();
            }
        }

        private void SerialGetHistoryComplete(string response)
        {
            rTxtHistoryResponse.Text = "";
            var files = response.Split(new string[] { GET_HISTORY_END_FILE_SEPARATOR }, StringSplitOptions.None);
            for (var fileIndex = 0; fileIndex < files.Length; fileIndex++)
            {
                var currentFile = files[fileIndex];
                var addressesWithTimeStamp = currentFile.Split(new string[] { HISTORY_RECORD_END_LINE }, StringSplitOptions.None);
                foreach (var add in addressesWithTimeStamp)
                {
                    Debug.WriteLine(add);
                }
                rTxtHistoryResponse.Text += GetNormalizedHistoryFile(addressesWithTimeStamp);
            }
            if (response.Length > 0)
            {
                btnHistorySave.Enabled = true;
                SaveReceivedHistory();
            }
            else
            {
                MessageBox.Show("This device's history is empty.", "History", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            _waitingSerialResponse = false;
        }

        private string GetNormalizedHistoryFile(string[] addressesWithTimeStamp)
        {
            var normalizedHistoryFile = "";
            for (var addressWithTimeStampIndex = addressesWithTimeStamp.Length - 1; addressWithTimeStampIndex >= 0; addressWithTimeStampIndex--)
            {
                var addressWithTimeStamp = addressesWithTimeStamp[addressWithTimeStampIndex];
                if (addressWithTimeStamp != "")
                {
                    var parameters = addressWithTimeStamp.Split(new string[] { HISTORY_RECORD_PARAMETER_SEPARATOR }, StringSplitOptions.None);
                    var address = parameters[0];
                    // Debug.WriteLine(DecryptStringFromBytes_Aes(Encoding.ASCII.GetBytes(address), Encoding.ASCII.GetBytes("qwertyuiopasdfgs")));
                    var timeStamp = Int32.Parse(parameters[1]);
                    var newerThan15Days = ((DateTimeOffset)DateTime.Today.AddDays(-15)).ToUnixTimeSeconds() <= timeStamp;
                    if (newerThan15Days)
                    {
                       var localDateTime = UnixTimeStampToLocalDateTime(timeStamp);
                       var newLine = string.Format("{0};{1}", address, localDateTime) + '\n';
                       normalizedHistoryFile += newLine;
                    }
                }
            }
            return normalizedHistoryFile;
        }

        static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = new byte[16];

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }

        private void SerialGetDeviceInfoComplete(string response)
        {
            var deviceInfos = response.Split(COMMAND_PARAMTERS_SEPARATOR);
            var deviceName = deviceInfos[0];
            var deviceMAC = deviceInfos[1];
            var titleDeviceInfo = (deviceName != "" ? (" | " + deviceName) : "") + " | " + deviceMAC;
            _lastDeviceMAC = deviceMAC;
            ActiveForm.Text = WINDOW_DEFAULT_TITLE + titleDeviceInfo;
            tBoxAboutMac.Text = deviceMAC;
            tBoxAboutName.Text = deviceName;
            tBoxConfigurationName.Text = deviceName;
            tBoxAboutVersion.Text = "1.0.0";
            _waitingSerialResponse = false;
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

        private void SaveReceivedHistory()
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
            var normalizedDeviceMAC = _lastDeviceMAC.Replace(':', '-');
            saveFileDialog.FileName = normalizedDeviceMAC + "  @" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm");
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
                        streamWriter.WriteLine(line);
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
            return _serialData.Replace(END_COMMAND, "").Replace(command, "");
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

        private void UpdateSelectedPortName()
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
            UpdateSelectedPortName();
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
            SaveReceivedHistory();
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
