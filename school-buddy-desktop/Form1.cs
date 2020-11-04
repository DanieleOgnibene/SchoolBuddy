using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
        private readonly string GET_HISTORY_COMMAND = "GET_HISTORY";

        #endregion

        private string _serialData = "";

        public SchoolBuddyMain()
        {
            InitializeComponent();
        }

        #region FUNCTIONS

        private void InitFormState()
        {
            InitToolStripComPorts();
            connectToolStripMenuItem.Enabled = false;
            disconnectToolStripMenuItem.Enabled = false;
            portToolStripMenuItem.Text = "Port";

            btnHistoryGet.Enabled = false;
            btnHistorySave.Enabled = false;
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
                    disconnectToolStripMenuItem.Enabled = false;
                    connectToolStripMenuItem.Enabled = true;
                    progressBarConnection.Value = 0;
                    btnHistoryGet.Enabled = false;
                    portToolStripMenuItem.Enabled = true;
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

                connectToolStripMenuItem.Enabled = false;
                disconnectToolStripMenuItem.Enabled = true;
                progressBarConnection.Value = 100;
                btnHistoryGet.Enabled = true;
                portToolStripMenuItem.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SerialDataReceived()
        {
            var newData = serialPort.ReadExisting();
            _serialData += newData.Replace("#", String.Empty);
            if (newData.Contains(END_COMMAND_CHAR))
            {
                Invoke(new EventHandler(SerialDataReceivedCompleteEventHandler));
            }
        }

        private void SerialDataReceivedCompleteEventHandler(object sender, EventArgs e)
        {
            rTxtHistoryResponse.Text = _serialData;
            btnHistorySave.Enabled = true;
            btnHistoryGet.Enabled = true;
            disconnectToolStripMenuItem.Enabled = true;
            SetDefaultCursor();
            SaveSerialDataReceived();
        }

        private void SendSerialCommand(string command)
        {
            serialPort.Write(command + END_COMMAND_CHAR);
        }

        private void SaveSerialDataReceived()
        {
            var saveFilePath = GetSaveFilePath();
            if (saveFilePath != "")
            {
                WriteSerialDataToFile(saveFilePath);
            }
        }

        private string GetSaveFilePath()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = "1a-e8-de-a3-93-0c  @" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm");
            saveFileDialog.Filter = "Text File|*.txt";
            saveFileDialog.ValidateNames = true;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                return saveFileDialog.FileName;
            }
            return "";
        }

        private void WriteSerialDataToFile(string filePath)
        {
            try
            {
                StreamWriter streamWriter = new StreamWriter(filePath);
                streamWriter.Write(_serialData);
                streamWriter.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetWaitCursor()
        {
            Cursor = Cursors.WaitCursor;
        }

        private void SetDefaultCursor()
        {
            Cursor = Cursors.Default;
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
                portToolStripMenuItem.Text = "Port: \"" + selectedPortName + "\"";
            }
            else
            {
                connectToolStripMenuItem.Enabled = false;
                portToolStripMenuItem.Text = "Port";
            }
        }

        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenSerialConnection();
        }

        private void btnHistoryGet_Click(object sender, EventArgs e)
        {
            SendSerialCommand(GET_HISTORY_COMMAND);
            _serialData = "";
            btnHistorySave.Enabled = false;
            btnHistoryGet.Enabled = false;
            disconnectToolStripMenuItem.Enabled = false;
            SetWaitCursor();
        }

        private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialDataReceived();
        }

        private void btnHistorySave_Click(object sender, EventArgs e)
        {
            SaveSerialDataReceived();
        }

        private void toolStripCbPorts_Click(object sender, EventArgs e)
        {
            InitToolStripComPorts();
        }

        private void disconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseSerialConnection();
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        #endregion

    }
}
