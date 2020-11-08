namespace school_buddy_desktop
{
    partial class SchoolBuddyMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.serialPort = new System.IO.Ports.SerialPort(this.components);
            this.tabControlPages = new System.Windows.Forms.TabControl();
            this.tabHistory = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.gBoxResponse = new System.Windows.Forms.GroupBox();
            this.rTxtHistoryResponse = new System.Windows.Forms.RichTextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnHistoryGet = new System.Windows.Forms.Button();
            this.btnHistorySave = new System.Windows.Forms.Button();
            this.tabConfiguration = new System.Windows.Forms.TabPage();
            this.gBoxConfigurationConfigureDevice = new System.Windows.Forms.GroupBox();
            this.btnConfigurationConfigure = new System.Windows.Forms.Button();
            this.lblConfigurationName = new System.Windows.Forms.Label();
            this.txtConfigurationName = new System.Windows.Forms.TextBox();
            this.gBoxConfigurationRemoveAllData = new System.Windows.Forms.GroupBox();
            this.btnConfigurationReset = new System.Windows.Forms.Button();
            this.progressBarConnection = new System.Windows.Forms.ProgressBar();
            this.toolStripSchoolBuddyMain = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.disconnectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.portToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripCbPorts = new System.Windows.Forms.ToolStripComboBox();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.panel3 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.tabControlPages.SuspendLayout();
            this.tabHistory.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.gBoxResponse.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tabConfiguration.SuspendLayout();
            this.gBoxConfigurationConfigureDevice.SuspendLayout();
            this.gBoxConfigurationRemoveAllData.SuspendLayout();
            this.toolStripSchoolBuddyMain.SuspendLayout();
            this.panel3.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // serialPort
            // 
            this.serialPort.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort_DataReceived);
            // 
            // tabControlPages
            // 
            this.tabControlPages.Controls.Add(this.tabHistory);
            this.tabControlPages.Controls.Add(this.tabConfiguration);
            this.tabControlPages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlPages.Location = new System.Drawing.Point(4, 33);
            this.tabControlPages.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabControlPages.Name = "tabControlPages";
            this.tabControlPages.SelectedIndex = 0;
            this.tabControlPages.Size = new System.Drawing.Size(378, 275);
            this.tabControlPages.TabIndex = 0;
            // 
            // tabHistory
            // 
            this.tabHistory.Controls.Add(this.tableLayoutPanel3);
            this.tabHistory.Location = new System.Drawing.Point(4, 23);
            this.tabHistory.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabHistory.Name = "tabHistory";
            this.tabHistory.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.tabHistory.Size = new System.Drawing.Size(402, 248);
            this.tabHistory.TabIndex = 1;
            this.tabHistory.Text = "History";
            this.tabHistory.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.gBoxResponse, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.panel2, 0, 1);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(4, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.Size = new System.Drawing.Size(394, 242);
            this.tableLayoutPanel3.TabIndex = 4;
            // 
            // gBoxResponse
            // 
            this.gBoxResponse.Controls.Add(this.rTxtHistoryResponse);
            this.gBoxResponse.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gBoxResponse.Location = new System.Drawing.Point(3, 3);
            this.gBoxResponse.Name = "gBoxResponse";
            this.gBoxResponse.Size = new System.Drawing.Size(388, 203);
            this.gBoxResponse.TabIndex = 2;
            this.gBoxResponse.TabStop = false;
            this.gBoxResponse.Text = "Response Preview";
            // 
            // rTxtHistoryResponse
            // 
            this.rTxtHistoryResponse.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rTxtHistoryResponse.Location = new System.Drawing.Point(3, 18);
            this.rTxtHistoryResponse.Name = "rTxtHistoryResponse";
            this.rTxtHistoryResponse.ReadOnly = true;
            this.rTxtHistoryResponse.Size = new System.Drawing.Size(382, 182);
            this.rTxtHistoryResponse.TabIndex = 1;
            this.rTxtHistoryResponse.Text = "";
            // 
            // panel2
            // 
            this.panel2.AutoSize = true;
            this.panel2.Controls.Add(this.btnHistoryGet);
            this.panel2.Controls.Add(this.btnHistorySave);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel2.Location = new System.Drawing.Point(158, 209);
            this.panel2.Margin = new System.Windows.Forms.Padding(0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(236, 33);
            this.panel2.TabIndex = 3;
            // 
            // btnHistoryGet
            // 
            this.btnHistoryGet.Location = new System.Drawing.Point(23, 3);
            this.btnHistoryGet.Name = "btnHistoryGet";
            this.btnHistoryGet.Size = new System.Drawing.Size(102, 27);
            this.btnHistoryGet.TabIndex = 7;
            this.btnHistoryGet.Text = "&Get";
            this.btnHistoryGet.UseVisualStyleBackColor = true;
            this.btnHistoryGet.Click += new System.EventHandler(this.btnHistoryGet_Click);
            // 
            // btnHistorySave
            // 
            this.btnHistorySave.Location = new System.Drawing.Point(131, 3);
            this.btnHistorySave.Name = "btnHistorySave";
            this.btnHistorySave.Size = new System.Drawing.Size(102, 27);
            this.btnHistorySave.TabIndex = 8;
            this.btnHistorySave.Text = "&Save";
            this.btnHistorySave.UseVisualStyleBackColor = true;
            this.btnHistorySave.Click += new System.EventHandler(this.btnHistorySave_Click);
            // 
            // tabConfiguration
            // 
            this.tabConfiguration.Controls.Add(this.panel1);
            this.tabConfiguration.Controls.Add(this.gBoxConfigurationConfigureDevice);
            this.tabConfiguration.Location = new System.Drawing.Point(4, 23);
            this.tabConfiguration.Name = "tabConfiguration";
            this.tabConfiguration.Padding = new System.Windows.Forms.Padding(3);
            this.tabConfiguration.Size = new System.Drawing.Size(370, 248);
            this.tabConfiguration.TabIndex = 2;
            this.tabConfiguration.Text = "Configuration";
            this.tabConfiguration.UseVisualStyleBackColor = true;
            // 
            // gBoxConfigurationConfigureDevice
            // 
            this.gBoxConfigurationConfigureDevice.Controls.Add(this.btnConfigurationConfigure);
            this.gBoxConfigurationConfigureDevice.Controls.Add(this.lblConfigurationName);
            this.gBoxConfigurationConfigureDevice.Controls.Add(this.txtConfigurationName);
            this.gBoxConfigurationConfigureDevice.Location = new System.Drawing.Point(6, 6);
            this.gBoxConfigurationConfigureDevice.Name = "gBoxConfigurationConfigureDevice";
            this.gBoxConfigurationConfigureDevice.Size = new System.Drawing.Size(189, 98);
            this.gBoxConfigurationConfigureDevice.TabIndex = 2;
            this.gBoxConfigurationConfigureDevice.TabStop = false;
            this.gBoxConfigurationConfigureDevice.Text = "Configure Device";
            // 
            // btnConfigurationConfigure
            // 
            this.btnConfigurationConfigure.Location = new System.Drawing.Point(6, 69);
            this.btnConfigurationConfigure.Name = "btnConfigurationConfigure";
            this.btnConfigurationConfigure.Size = new System.Drawing.Size(75, 23);
            this.btnConfigurationConfigure.TabIndex = 2;
            this.btnConfigurationConfigure.Text = "Save";
            this.btnConfigurationConfigure.UseVisualStyleBackColor = true;
            this.btnConfigurationConfigure.Click += new System.EventHandler(this.btnConfigurationConfigure_Click);
            // 
            // lblConfigurationName
            // 
            this.lblConfigurationName.AutoSize = true;
            this.lblConfigurationName.Location = new System.Drawing.Point(6, 35);
            this.lblConfigurationName.Name = "lblConfigurationName";
            this.lblConfigurationName.Size = new System.Drawing.Size(42, 14);
            this.lblConfigurationName.TabIndex = 1;
            this.lblConfigurationName.Text = "Name:";
            // 
            // txtConfigurationName
            // 
            this.txtConfigurationName.Location = new System.Drawing.Point(54, 32);
            this.txtConfigurationName.Name = "txtConfigurationName";
            this.txtConfigurationName.Size = new System.Drawing.Size(121, 22);
            this.txtConfigurationName.TabIndex = 0;
            // 
            // gBoxConfigurationRemoveAllData
            // 
            this.gBoxConfigurationRemoveAllData.Controls.Add(this.btnConfigurationReset);
            this.gBoxConfigurationRemoveAllData.Location = new System.Drawing.Point(70, 6);
            this.gBoxConfigurationRemoveAllData.Name = "gBoxConfigurationRemoveAllData";
            this.gBoxConfigurationRemoveAllData.Size = new System.Drawing.Size(127, 54);
            this.gBoxConfigurationRemoveAllData.TabIndex = 1;
            this.gBoxConfigurationRemoveAllData.TabStop = false;
            this.gBoxConfigurationRemoveAllData.Text = "Remove All Data";
            // 
            // btnConfigurationReset
            // 
            this.btnConfigurationReset.Location = new System.Drawing.Point(22, 21);
            this.btnConfigurationReset.Name = "btnConfigurationReset";
            this.btnConfigurationReset.Size = new System.Drawing.Size(87, 23);
            this.btnConfigurationReset.TabIndex = 0;
            this.btnConfigurationReset.Text = "Reset";
            this.btnConfigurationReset.UseVisualStyleBackColor = true;
            this.btnConfigurationReset.Click += new System.EventHandler(this.btnConfigurationReset_Click);
            // 
            // progressBarConnection
            // 
            this.progressBarConnection.Dock = System.Windows.Forms.DockStyle.Right;
            this.progressBarConnection.Location = new System.Drawing.Point(310, 0);
            this.progressBarConnection.Name = "progressBarConnection";
            this.progressBarConnection.Size = new System.Drawing.Size(70, 24);
            this.progressBarConnection.TabIndex = 6;
            // 
            // toolStripSchoolBuddyMain
            // 
            this.toolStripSchoolBuddyMain.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStripSchoolBuddyMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.disconnectToolStripMenuItem,
            this.connectToolStripMenuItem,
            this.portToolStripMenuItem});
            this.toolStripSchoolBuddyMain.Location = new System.Drawing.Point(0, 0);
            this.toolStripSchoolBuddyMain.Name = "toolStripSchoolBuddyMain";
            this.toolStripSchoolBuddyMain.Size = new System.Drawing.Size(310, 24);
            this.toolStripSchoolBuddyMain.TabIndex = 1;
            this.toolStripSchoolBuddyMain.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(93, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // disconnectToolStripMenuItem
            // 
            this.disconnectToolStripMenuItem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.disconnectToolStripMenuItem.Name = "disconnectToolStripMenuItem";
            this.disconnectToolStripMenuItem.Size = new System.Drawing.Size(78, 20);
            this.disconnectToolStripMenuItem.Text = "&Disconnect";
            this.disconnectToolStripMenuItem.Click += new System.EventHandler(this.disconnectToolStripMenuItem_Click);
            // 
            // connectToolStripMenuItem
            // 
            this.connectToolStripMenuItem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.connectToolStripMenuItem.Name = "connectToolStripMenuItem";
            this.connectToolStripMenuItem.Size = new System.Drawing.Size(64, 20);
            this.connectToolStripMenuItem.Text = "&Connect";
            this.connectToolStripMenuItem.Click += new System.EventHandler(this.connectToolStripMenuItem_Click);
            // 
            // portToolStripMenuItem
            // 
            this.portToolStripMenuItem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.portToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.portToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripCbPorts});
            this.portToolStripMenuItem.Name = "portToolStripMenuItem";
            this.portToolStripMenuItem.Size = new System.Drawing.Size(41, 20);
            this.portToolStripMenuItem.Text = "&Port";
            // 
            // toolStripCbPorts
            // 
            this.toolStripCbPorts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolStripCbPorts.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.toolStripCbPorts.Name = "toolStripCbPorts";
            this.toolStripCbPorts.Size = new System.Drawing.Size(121, 23);
            this.toolStripCbPorts.DropDownClosed += new System.EventHandler(this.toolStripCbPorts_DropDownClosed);
            this.toolStripCbPorts.Click += new System.EventHandler(this.toolStripCbPorts_Click);
            // 
            // panel3
            // 
            this.panel3.AutoSize = true;
            this.panel3.Controls.Add(this.toolStripSchoolBuddyMain);
            this.panel3.Controls.Add(this.progressBarConnection);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(3, 3);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(380, 24);
            this.panel3.TabIndex = 9;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.panel3, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tabControlPages, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(386, 311);
            this.tableLayoutPanel1.TabIndex = 10;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel4);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(3, 182);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(364, 63);
            this.panel1.TabIndex = 3;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.gBoxConfigurationRemoveAllData);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel4.Location = new System.Drawing.Point(164, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(200, 63);
            this.panel4.TabIndex = 0;
            // 
            // SchoolBuddyMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(386, 311);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MainMenuStrip = this.toolStripSchoolBuddyMain;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MinimumSize = new System.Drawing.Size(402, 350);
            this.Name = "SchoolBuddyMain";
            this.Text = "School Buddy";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SchoolBuddyMain_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControlPages.ResumeLayout(false);
            this.tabHistory.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.gBoxResponse.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.tabConfiguration.ResumeLayout(false);
            this.gBoxConfigurationConfigureDevice.ResumeLayout(false);
            this.gBoxConfigurationConfigureDevice.PerformLayout();
            this.gBoxConfigurationRemoveAllData.ResumeLayout(false);
            this.toolStripSchoolBuddyMain.ResumeLayout(false);
            this.toolStripSchoolBuddyMain.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.IO.Ports.SerialPort serialPort;
        private System.Windows.Forms.TabControl tabControlPages;
        private System.Windows.Forms.TabPage tabHistory;
        private System.Windows.Forms.MenuStrip toolStripSchoolBuddyMain;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.GroupBox gBoxResponse;
        private System.Windows.Forms.RichTextBox rTxtHistoryResponse;
        private System.Windows.Forms.ProgressBar progressBarConnection;
        private System.Windows.Forms.Button btnHistoryGet;
        private System.Windows.Forms.Button btnHistorySave;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem disconnectToolStripMenuItem;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ToolStripMenuItem portToolStripMenuItem;
        private System.Windows.Forms.ToolStripComboBox toolStripCbPorts;
        private System.Windows.Forms.TabPage tabConfiguration;
        private System.Windows.Forms.Button btnConfigurationReset;
        private System.Windows.Forms.GroupBox gBoxConfigurationConfigureDevice;
        private System.Windows.Forms.Label lblConfigurationName;
        private System.Windows.Forms.TextBox txtConfigurationName;
        private System.Windows.Forms.GroupBox gBoxConfigurationRemoveAllData;
        private System.Windows.Forms.Button btnConfigurationConfigure;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel4;
    }
}

