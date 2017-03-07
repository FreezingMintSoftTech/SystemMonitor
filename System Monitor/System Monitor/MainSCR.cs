using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Resources;
using System.Drawing;
using System.Xml;
using System.Xml.Linq;
using System.Diagnostics;
using System.Management;
using System.Net.NetworkInformation;
using Microsoft.Win32;
using NativeWifi;

namespace System_Monitor
{
    
    class MainSCR : Form
    {
        #region VariablesDeclaration
        //
        //----Program VARiables declaration----
        //
        //Release Variable
        public string release = " 0.0.22";   //Release number
        public string YearOfRelease = "2017";   //Release year

        //Program Variables
        public int TimeOfSessionInt;    //variable for Time of Session, inc by timer TimeOfSessionTimer

        //Patch Variables
        static string PatchToApp = AppDomain.CurrentDomain.BaseDirectory;  //static var with current path to app
        static string PatchToUserData = PatchToApp + "UserData";   //var with current path to UserData subfolder
               
        //Languages Variables        
        ResourceManager res_man = new ResourceManager("System_Monitor.Data.lang", typeof(MainSCR).Assembly);    // declare Resource manager to access to specific cultureinfo
        CultureInfo language = CultureInfo.CreateSpecificCulture("en");       // declare culture info, this var is for choosing specyfic language
        
        //Starting Parameters Variables
        static XmlDocument UserDataParameters = new XmlDocument();   //Variable used for opening UserData/Parameters xml file
        int MainSCRLocationX;   //Variables used to store MainSCR Location in UserData/Parameters file (X,Y)
        int MainSCRLocationY;

        //Performance measurment Variables
        int[] CPUusageArray = new int[20];  //Array used for storing measured CPU usage values
        int NumberOfProcessorCores = new ManagementObjectSearcher("Select * from Win32_Processor").Get().Cast<ManagementBaseObject>().Sum(item => int.Parse(item["NumberOfCores"].ToString()));
        string ProcessorName;   //used for checking the processor name and showing it in ProcessorNameLabel
        int ActualRAMusage = 0;   //used for keeping actual RAM usage value and showing it on RAM usage graph

        
        //Performance graphs Variables
        Graphics CPUusageGraphGraphics;    //graphics for CPU usage graph
        Bitmap CPUusageGraphBitMap;   //bitmap for CPU usage graph
        Graphics RAMusageGraphGraphics;    //graphics for RAM usage graph
        Bitmap RAMusageGraphBitMap;   //bitmap for RAM usage graph

        //Network Interfaces Variables
        IPGlobalProperties computerProperties = IPGlobalProperties.GetIPGlobalProperties();
        NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();

        #endregion

        #region FormAppearance&MainProgram

        //----Below defining further objects on this form
        private Container components;
        private System.Windows.Forms.Button CloseButton;
        private System.Windows.Forms.Button SettingsButton;
        private System.Windows.Forms.Button HistoryButton;
        private System.Windows.Forms.Label TimeOfSession;
        private System.Windows.Forms.Label TitleLabel;
        private System.Windows.Forms.Label OverallCPUusageLabel;
        private System.Windows.Forms.Label OverallCPUusageValueLabel;
        private System.Windows.Forms.Label LanguageLabel;
        private System.Windows.Forms.Label VersionLabel;
        private System.Windows.Forms.Label AuthorLabel;
        private System.Windows.Forms.Label ProcessorNameLabel;
        private System.Windows.Forms.Label ProcessorCoresLabel;
        private System.Windows.Forms.Label ProcessorTemperatureLabel;
        private System.Windows.Forms.Label OverallRAMMemoryLabel;
        private System.Windows.Forms.Label PercentageRAMMemoryLabel;
        private System.Windows.Forms.Label TotalRAMMemoryLabel;
        private System.Windows.Forms.Label TotalRAMMemoryValueLabel;
        private System.Windows.Forms.Label TotalRAMMemoryUsageLabel;
        private System.Windows.Forms.Label TotalRAMMemoryUsageValueLabel;
        private System.Windows.Forms.Label NetworkIntefacesTitle;
        private System.Windows.Forms.Label WirelessLabel;
        private System.Windows.Forms.Label WirelessIPAddress;
        private System.Windows.Forms.Label WirelessIPObtainMethod;
        private System.Windows.Forms.Label WirelessIPSSID;
        private System.Windows.Forms.Label WireNetworkLabel;
        private System.Windows.Forms.PictureBox CPUusageGraph;
        private System.Windows.Forms.PictureBox RAMusageGraph;
        private System.Windows.Forms.Timer TimeOfSessionTimer;
        private System.Windows.Forms.Timer Time1SecTimer;
        private System.Windows.Forms.ContextMenuStrip LanguagesMenu;
        private System.Windows.Forms.ToolStripButton LanguagesMenuLangEN;
        private System.Windows.Forms.ToolStripButton LanguagesMenuLangPL;
        private System.Windows.Forms.NotifyIcon TrayIcon;
        //----End of defining objects on this form

        public MainSCR()
        {            
            //Below events of MainSCR Form
            this.Resize += MainSCR_Resize;   //Event for resize MainSCR used for showing tray icon after minimize
            this.Load += MainSCR_Load;   //Event for loading MainSCR
            this.FormClosed += MainSCR_FormClosed;   //Event for saving MainSCR parameters after closing

            //this.LanguagesMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainSCR
            //           
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "MainSCR";
            this.Text = "MainSCR";
            this.Height = 500;
            this.Width = 222;
            this.ShowInTaskbar = false;
            this.BackColor = Color.FromArgb(255, 255, 255);

            //Below there are two events used for dragging the form by mouse
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MainSCR_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MainSCR_MouseMove);

            // 
            // CloseButton
            //  
            this.CloseButton = new System.Windows.Forms.Button();
            this.CloseButton.Visible = true;
            this.CloseButton.Location = new System.Drawing.Point(149, 460);            
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.Size = new System.Drawing.Size(70, 20);
            this.CloseButton.Text = res_man.GetString("CloseButton", language);
            this.CloseButton.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.CloseButton.UseVisualStyleBackColor = true;
            this.CloseButton.Font = new Font(new FontFamily(System.Drawing.Text.GenericFontFamilies.Serif), 7);
            this.CloseButton.Click += CloseButton_Click;

            // 
            // SettingsButton
            //  
            this.SettingsButton = new System.Windows.Forms.Button();
            this.SettingsButton.Visible = true;
            this.SettingsButton.Location = new System.Drawing.Point(76, 460);
            this.SettingsButton.Name = "SettingsButton";
            this.SettingsButton.Size = new System.Drawing.Size(70, 20);
            this.SettingsButton.Text = res_man.GetString("SettingsButton", language);
            this.SettingsButton.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.SettingsButton.UseVisualStyleBackColor = true;
            this.SettingsButton.Font = new Font(new FontFamily(System.Drawing.Text.GenericFontFamilies.Serif), 7);
            this.SettingsButton.Click += SettingsButton_Click;

            // 
            // HistoryButton
            //  
            this.HistoryButton = new System.Windows.Forms.Button();
            this.HistoryButton.Visible = true;
            this.HistoryButton.Location = new System.Drawing.Point(3, 460);
            this.HistoryButton.Name = "HistoryButton";
            this.HistoryButton.Size = new System.Drawing.Size(70, 20);
            this.HistoryButton.Text = res_man.GetString("HistoryButton", language);
            this.HistoryButton.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.HistoryButton.UseVisualStyleBackColor = true;
            this.HistoryButton.Font = new Font(new FontFamily(System.Drawing.Text.GenericFontFamilies.Serif), 7);
            this.HistoryButton.Click += HistoryButton_Click;

            // 
            // Label TimeOfSession
            //  
            this.TimeOfSession = new System.Windows.Forms.Label();
            this.TimeOfSession.Location = new System.Drawing.Point(2, 439);
            this.TimeOfSession.Name = "TimeOfSession";
            this.TimeOfSession.Size = new System.Drawing.Size(218, 15);
            this.TimeOfSession.TabIndex = 1;
            this.TimeOfSession.Text = res_man.GetString("CurrentSessionLasts", language) + " 0 " + res_man.GetString("Minutes", language);
            this.TimeOfSession.TextAlign = System.Drawing.ContentAlignment.TopCenter;

            // 
            // Label TitleLabel
            //  
            this.TitleLabel = new System.Windows.Forms.Label();
            this.TitleLabel.Location = new System.Drawing.Point(2, 5);
            this.TitleLabel.Name = "TitleLabel";
            this.TitleLabel.Size = new System.Drawing.Size(216, 20);
            this.TitleLabel.TabIndex = 1;
            this.TitleLabel.Text = res_man.GetString("TitleLabel", language);
            this.TitleLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.TitleLabel.Font = new Font(new FontFamily(System.Drawing.Text.GenericFontFamilies.Serif), 12);


            // 
            // Label OverallCPUusageLabel
            //  
            this.OverallCPUusageLabel = new System.Windows.Forms.Label();
            this.OverallCPUusageLabel.Location = new System.Drawing.Point(2, 32);
            this.OverallCPUusageLabel.Name = "OverallCPUusageLabel";
            this.OverallCPUusageLabel.Size = new System.Drawing.Size(180, 18);
            this.OverallCPUusageLabel.TabIndex = 1;
            this.OverallCPUusageLabel.Text = res_man.GetString("OverallCPUusageLabel", language);
            this.OverallCPUusageLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.OverallCPUusageLabel.Font = new Font(new FontFamily(System.Drawing.Text.GenericFontFamilies.Serif), 8);
            this.OverallCPUusageLabel.BackColor = System.Drawing.Color.Transparent;

            // 
            // Label OverallCPUusageValueLabel
            //  
            this.OverallCPUusageValueLabel = new System.Windows.Forms.Label();
            this.OverallCPUusageValueLabel.Location = new System.Drawing.Point(164, 32);
            this.OverallCPUusageValueLabel.Name = "OverallCPUusageValueLabel";
            this.OverallCPUusageValueLabel.Size = new System.Drawing.Size(60, 17);
            this.OverallCPUusageValueLabel.TabIndex = 1;
            this.OverallCPUusageValueLabel.Text = "0 %";
            this.OverallCPUusageValueLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.OverallCPUusageValueLabel.Font = new Font(new FontFamily(System.Drawing.Text.GenericFontFamilies.Serif), 10);
            this.OverallCPUusageValueLabel.Parent = OverallCPUusageLabel;
            this.OverallCPUusageValueLabel.BackColor = System.Drawing.Color.Transparent;

            // 
            // Label LanguageLabel
            //  
            this.LanguageLabel = new System.Windows.Forms.Label();
            this.LanguageLabel.Location = new System.Drawing.Point(120, 420);
            this.LanguageLabel.Name = "LanguageLabel";
            this.LanguageLabel.TabIndex = 1;
            this.LanguageLabel.Text = res_man.GetString("LanguageXX", language);
            this.LanguageLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.LanguageLabel.Click += LanguageLabel_Click;

            // 
            // Label VersionLabel
            //
            this.VersionLabel = new System.Windows.Forms.Label();
            this.VersionLabel.Location = new System.Drawing.Point(140, 486);
            this.VersionLabel.Size = new System.Drawing.Size(80, 12);
            this.VersionLabel.Name = "VersionLabel";
            this.VersionLabel.TabIndex = 1;
            this.VersionLabel.Text = res_man.GetString("VersionLabel", language) + release;
            this.VersionLabel.TextAlign = System.Drawing.ContentAlignment.BottomRight;
            this.VersionLabel.Font = new Font(new FontFamily(System.Drawing.Text.GenericFontFamilies.Serif), 7);

            // 
            // Label AuthorLabel
            //
            this.AuthorLabel = new System.Windows.Forms.Label();
            this.AuthorLabel.Location = new System.Drawing.Point(3, 486);
            this.AuthorLabel.Size = new System.Drawing.Size(60, 12);
            this.AuthorLabel.Name = "AuthorLabel";
            this.AuthorLabel.TabIndex = 1;
            this.AuthorLabel.Text = YearOfRelease + res_man.GetString("AuthorLabel", language);
            this.AuthorLabel.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.AuthorLabel.Font = new Font(new FontFamily(System.Drawing.Text.GenericFontFamilies.Serif), 7);

            // 
            // Label ProcessorNameLabel
            //
            this.ProcessorNameLabel = new System.Windows.Forms.Label();
            this.ProcessorNameLabel.Location = new System.Drawing.Point(3, 137);
            this.ProcessorNameLabel.Size = new System.Drawing.Size(215, 15);
            this.ProcessorNameLabel.Name = "ProcessorNameLabel";
            this.ProcessorNameLabel.TabIndex = 1;
            this.ProcessorNameLabel.Text = res_man.GetString("ProcessorNameLabel", language) + ProcessorName;
            this.ProcessorNameLabel.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.ProcessorNameLabel.Font = new Font(new FontFamily(System.Drawing.Text.GenericFontFamilies.Serif), 7);

            // 
            // Label ProcessorCoresLabel
            //
            this.ProcessorCoresLabel = new System.Windows.Forms.Label();
            this.ProcessorCoresLabel.Location = new System.Drawing.Point(3, 155);
            this.ProcessorCoresLabel.Size = new System.Drawing.Size(215, 15);
            this.ProcessorCoresLabel.Name = "ProcessorCoresLabel";
            this.ProcessorCoresLabel.TabIndex = 1;
            this.ProcessorCoresLabel.Text = res_man.GetString("ProcessorCoresLabel", language) + "   " + NumberOfProcessorCores.ToString();
            this.ProcessorCoresLabel.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.ProcessorCoresLabel.Font = new Font(new FontFamily(System.Drawing.Text.GenericFontFamilies.Serif), 7);

            // 
            // Label ProcessorTemperatureLabel
            //
            this.ProcessorTemperatureLabel = new System.Windows.Forms.Label();
            this.ProcessorTemperatureLabel.Location = new System.Drawing.Point(3, 173);
            this.ProcessorTemperatureLabel.Size = new System.Drawing.Size(215, 15);
            this.ProcessorTemperatureLabel.Name = "ProcessorTemperatureLabel";
            this.ProcessorTemperatureLabel.TabIndex = 1;
            this.ProcessorTemperatureLabel.Text = res_man.GetString("ProcessorTemperatureLabel", language) + "  -----";
            this.ProcessorTemperatureLabel.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.ProcessorTemperatureLabel.Font = new Font(new FontFamily(System.Drawing.Text.GenericFontFamilies.Serif), 7);

            // 
            // PictureBox CPUusageGraph
            //
            this.CPUusageGraph = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.CPUusageGraph)).BeginInit();
            //this.CPUusageGraph.Dock = System.Windows.Forms.DockStyle.Fill;  //commented because this crashes application appearance
            this.CPUusageGraph.Location = new System.Drawing.Point(30, 50);
            this.CPUusageGraph.Name = "CPUusageGraph";
            this.CPUusageGraph.Size = new System.Drawing.Size(152, 80);
            this.CPUusageGraph.TabIndex = 0;
            this.CPUusageGraph.TabStop = false;
            this.CPUusageGraph.BackColor = System.Drawing.Color.Transparent;
            this.CPUusageGraph.Paint += new PaintEventHandler(CPUusageGraph_Paint);
            ((System.ComponentModel.ISupportInitialize)(this.CPUusageGraph)).EndInit();

            // 
            // PictureBox RAMusageGraph
            //
            this.RAMusageGraph = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.RAMusageGraph)).BeginInit();
            this.RAMusageGraph.Location = new System.Drawing.Point(140, 220);
            this.RAMusageGraph.Name = "RAMusageGraph";
            this.RAMusageGraph.Size = new System.Drawing.Size(70, 75);
            this.RAMusageGraph.TabIndex = 0;
            this.RAMusageGraph.TabStop = false;
            this.RAMusageGraph.BackColor = System.Drawing.Color.Transparent;
            this.RAMusageGraph.Paint += new PaintEventHandler(RAMusageGraph_Paint);
            ((System.ComponentModel.ISupportInitialize)(this.RAMusageGraph)).EndInit();

            // 
            // Label OverallRAMMemoryLabel
            //  
            this.OverallRAMMemoryLabel = new System.Windows.Forms.Label();
            this.OverallRAMMemoryLabel.Location = new System.Drawing.Point(2, 200);
            this.OverallRAMMemoryLabel.Name = "OverallRAMMemoryLabel";
            this.OverallRAMMemoryLabel.Size = new System.Drawing.Size(220, 18);
            this.OverallRAMMemoryLabel.TabIndex = 1;
            this.OverallRAMMemoryLabel.Text = res_man.GetString("OverallRAMMemoryLabel", language);
            this.OverallRAMMemoryLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.OverallRAMMemoryLabel.Font = new Font(new FontFamily(System.Drawing.Text.GenericFontFamilies.Serif), 8);
            this.OverallRAMMemoryLabel.BackColor = System.Drawing.Color.Transparent;

            // 
            // Label PercentageRAMMemoryLabel
            //  
            this.PercentageRAMMemoryLabel = new System.Windows.Forms.Label();
            this.PercentageRAMMemoryLabel.Location = new System.Drawing.Point(145, 300);
            this.PercentageRAMMemoryLabel.Name = "PercentageRAMMemoryLabel";
            this.PercentageRAMMemoryLabel.Size = new System.Drawing.Size(70, 18);
            this.PercentageRAMMemoryLabel.TabIndex = 1;
            this.PercentageRAMMemoryLabel.Text = "0 %";
            this.PercentageRAMMemoryLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.PercentageRAMMemoryLabel.Font = new Font(new FontFamily(System.Drawing.Text.GenericFontFamilies.Serif), 10, FontStyle.Bold);
            this.PercentageRAMMemoryLabel.ForeColor = System.Drawing.Color.Lime;
            this.PercentageRAMMemoryLabel.BackColor = System.Drawing.Color.Transparent;

            // 
            // Label TotalRAMMemoryLabel
            //
            this.TotalRAMMemoryLabel = new System.Windows.Forms.Label();
            this.TotalRAMMemoryLabel.Location = new System.Drawing.Point(3, 225);
            this.TotalRAMMemoryLabel.Size = new System.Drawing.Size(135, 15);
            this.TotalRAMMemoryLabel.Name = "TotalRAMMemoryLabel";
            this.TotalRAMMemoryLabel.TabIndex = 1;
            this.TotalRAMMemoryLabel.Text = res_man.GetString("TotalRAMMemoryLabel", language);
            this.TotalRAMMemoryLabel.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.TotalRAMMemoryLabel.Font = new Font(new FontFamily(System.Drawing.Text.GenericFontFamilies.Serif), 7);

            // 
            // Label TotalRAMMemoryValueLabel
            //
            this.TotalRAMMemoryValueLabel = new System.Windows.Forms.Label();
            this.TotalRAMMemoryValueLabel.Location = new System.Drawing.Point(3, 250);
            this.TotalRAMMemoryValueLabel.Size = new System.Drawing.Size(135, 15);
            this.TotalRAMMemoryValueLabel.Name = "TotalRAMMemoryValueLabel";
            this.TotalRAMMemoryValueLabel.TabIndex = 1;
            this.TotalRAMMemoryValueLabel.Text = "0 GB";
            this.TotalRAMMemoryValueLabel.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.TotalRAMMemoryValueLabel.Font = new Font(new FontFamily(System.Drawing.Text.GenericFontFamilies.Serif), 7);

            // 
            // Label TotalRAMMemoryUsageLabel
            //
            this.TotalRAMMemoryUsageLabel = new System.Windows.Forms.Label();
            this.TotalRAMMemoryUsageLabel.Location = new System.Drawing.Point(3, 275);
            this.TotalRAMMemoryUsageLabel.Size = new System.Drawing.Size(135, 15);
            this.TotalRAMMemoryUsageLabel.Name = "TotalRAMMemoryUsageLabel";
            this.TotalRAMMemoryUsageLabel.TabIndex = 1;
            this.TotalRAMMemoryUsageLabel.Text = res_man.GetString("TotalRAMMemoryUsageLabel", language);
            this.TotalRAMMemoryUsageLabel.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.TotalRAMMemoryUsageLabel.Font = new Font(new FontFamily(System.Drawing.Text.GenericFontFamilies.Serif), 7);

            // 
            // Label TotalRAMMemoryUsageValueLabel
            //
            this.TotalRAMMemoryUsageValueLabel = new System.Windows.Forms.Label();
            this.TotalRAMMemoryUsageValueLabel.Location = new System.Drawing.Point(3, 300);
            this.TotalRAMMemoryUsageValueLabel.Size = new System.Drawing.Size(135, 15);
            this.TotalRAMMemoryUsageValueLabel.Name = "TotalRAMMemoryUsageValueLabel";
            this.TotalRAMMemoryUsageValueLabel.TabIndex = 1;
            this.TotalRAMMemoryUsageValueLabel.Text = "0 GB";
            this.TotalRAMMemoryUsageValueLabel.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.TotalRAMMemoryUsageValueLabel.Font = new Font(new FontFamily(System.Drawing.Text.GenericFontFamilies.Serif), 7);

            // 
            // Label NetworkIntefacesTitle
            //  
            this.NetworkIntefacesTitle = new System.Windows.Forms.Label();
            this.NetworkIntefacesTitle.Location = new System.Drawing.Point(2, 330);
            this.NetworkIntefacesTitle.Name = "NetworkIntefacesTitle";
            this.NetworkIntefacesTitle.Size = new System.Drawing.Size(218, 18);
            this.NetworkIntefacesTitle.TabIndex = 1;
            this.NetworkIntefacesTitle.Text = res_man.GetString("NetworkIntefacesTitle", language);
            this.NetworkIntefacesTitle.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.NetworkIntefacesTitle.Font = new Font(new FontFamily(System.Drawing.Text.GenericFontFamilies.Serif), 8);
            this.NetworkIntefacesTitle.BackColor = System.Drawing.Color.Transparent;

            // 
            // Label WirelessLabel
            //
            this.WirelessLabel = new System.Windows.Forms.Label();
            this.WirelessLabel.Location = new System.Drawing.Point(114, 350);
            this.WirelessLabel.Size = new System.Drawing.Size(105, 15);
            this.WirelessLabel.Name = "WirelessLabel";
            this.WirelessLabel.TabIndex = 1;
            this.WirelessLabel.Text = res_man.GetString("WirelessLabel", language); 
            this.WirelessLabel.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.WirelessLabel.Font = new Font(new FontFamily(System.Drawing.Text.GenericFontFamilies.Serif), 7);

            // 
            // Label WirelessIPAddress
            //
            this.WirelessIPAddress = new System.Windows.Forms.Label();
            this.WirelessIPAddress.Location = new System.Drawing.Point(114, 380);
            this.WirelessIPAddress.Size = new System.Drawing.Size(105, 15);
            this.WirelessIPAddress.Name = "WirelessIPAddress";
            this.WirelessIPAddress.TabIndex = 1;
            this.WirelessIPAddress.Text = "IP: " + res_man.GetString("NotDetected", language); ;
            this.WirelessIPAddress.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.WirelessIPAddress.Font = new Font(new FontFamily(System.Drawing.Text.GenericFontFamilies.Serif), 7);

            // 
            // Label WirelessIPObtainMethod
            //
            this.WirelessIPObtainMethod = new System.Windows.Forms.Label();
            this.WirelessIPObtainMethod.Location = new System.Drawing.Point(114, 395);
            this.WirelessIPObtainMethod.Size = new System.Drawing.Size(105, 15);
            this.WirelessIPObtainMethod.Name = "WirelessIPObtainMethod";
            this.WirelessIPObtainMethod.TabIndex = 1;
            this.WirelessIPObtainMethod.Text = "IP: " + res_man.GetString("NotDetected", language); ;
            this.WirelessIPObtainMethod.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.WirelessIPObtainMethod.Font = new Font(new FontFamily(System.Drawing.Text.GenericFontFamilies.Serif), 7);

            // 
            // Label WirelessIPSSID
            //
            this.WirelessIPSSID = new System.Windows.Forms.Label();
            this.WirelessIPSSID.Location = new System.Drawing.Point(114, 365);
            this.WirelessIPSSID.Size = new System.Drawing.Size(105, 15);
            this.WirelessIPSSID.Name = "WirelessIPSSID";
            this.WirelessIPSSID.TabIndex = 1;
            this.WirelessIPSSID.Text = "SSID:";
            this.WirelessIPSSID.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.WirelessIPSSID.Font = new Font(new FontFamily(System.Drawing.Text.GenericFontFamilies.Serif), 7);

            // 
            // Label WireNetworkLabel
            //
            this.WireNetworkLabel = new System.Windows.Forms.Label();
            this.WireNetworkLabel.Location = new System.Drawing.Point(3, 350);
            this.WireNetworkLabel.Size = new System.Drawing.Size(105, 15);
            this.WireNetworkLabel.Name = "WireNetworkLabel";
            this.WireNetworkLabel.TabIndex = 1;
            this.WireNetworkLabel.Text = res_man.GetString("WireNetworkLabel", language);
            this.WireNetworkLabel.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.WireNetworkLabel.Font = new Font(new FontFamily(System.Drawing.Text.GenericFontFamilies.Serif), 7);

            // 
            // TimeOfSessionTimer
            // 
            this.components = new System.ComponentModel.Container();
            this.TimeOfSessionTimer = new System.Windows.Forms.Timer(this.components);
            this.TimeOfSessionTimer.Interval = 60000;
            this.TimeOfSessionTimer.Tick += new System.EventHandler(this.TimeOfSessionTimer_Tick);
            TimeOfSessionTimer.Enabled = true;

            // 
            // Time1SecTimer
            // 
            //this.components = new System.ComponentModel.Container();
            this.Time1SecTimer = new System.Windows.Forms.Timer(this.components);
            this.Time1SecTimer.Interval = 1000;
            this.Time1SecTimer.Tick += new System.EventHandler(this.Time1SecTimer_Tick);
            Time1SecTimer.Enabled = true;

            // 
            // LanguagesMenu
            // 
            this.LanguagesMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.LanguagesMenu.BackColor = System.Drawing.SystemColors.Menu;
            this.LanguagesMenu.Name = "contextMenuStrip1";
            this.LanguagesMenu.Size = new System.Drawing.Size(161, 76);
            this.LanguagesMenu.Text = "Langu";

            // 
            // LanguagesMenuLangEN
            // 
            this.LanguagesMenuLangEN = new System.Windows.Forms.ToolStripButton();
            this.LanguagesMenuLangEN.BackColor = System.Drawing.SystemColors.Menu;
            this.LanguagesMenuLangEN.Name = "LanguagesMenuLangEN";
            this.LanguagesMenuLangEN.Size = new System.Drawing.Size(100, 23);
            this.LanguagesMenuLangEN.Text = "EN";
            this.LanguagesMenuLangEN.Click += LanguagesMenuLangEN_Click;   

            // 
            // LanguagesMenuLangPL
            // 
            this.LanguagesMenuLangPL = new System.Windows.Forms.ToolStripButton();
            this.LanguagesMenuLangPL.BackColor = System.Drawing.SystemColors.Menu;
            this.LanguagesMenuLangPL.Name = "LanguagesMenuLangPL";
            this.LanguagesMenuLangPL.Size = new System.Drawing.Size(100, 23);
            this.LanguagesMenuLangPL.Text = "PL";
            this.LanguagesMenuLangPL.Click += LanguagesMenuLangPL_Click;

            // 
            // TrayIcon
            // 
            this.TrayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.TrayIcon.Text = "System Monitor";
            this.TrayIcon.Visible = false;
            this.TrayIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.TrayIcon.BalloonTipText = res_man.GetString("TrayIconBalloonTipText", language);
            this.TrayIcon.BalloonTipTitle = "System Monitor";
            this.TrayIcon.Icon = new System.Drawing.Icon("GFX/Icon1.ico");
            this.TrayIcon.Click += TrayIcon_Click;
                       
            // 
            // Adding objects to Controls
            //  
            this.Controls.AddRange(new Control[] { CloseButton, SettingsButton, HistoryButton, TimeOfSession, TitleLabel, LanguageLabel, OverallCPUusageLabel, OverallCPUusageValueLabel, VersionLabel, AuthorLabel, ProcessorNameLabel, ProcessorCoresLabel, ProcessorTemperatureLabel, CPUusageGraph, RAMusageGraph, OverallRAMMemoryLabel, PercentageRAMMemoryLabel, TotalRAMMemoryLabel, TotalRAMMemoryValueLabel, TotalRAMMemoryUsageLabel, TotalRAMMemoryUsageValueLabel, NetworkIntefacesTitle, WirelessLabel, WirelessIPAddress, WirelessIPSSID, WirelessIPObtainMethod, WireNetworkLabel });

            //
            // Adding Languages to Languages Menu
            //        
            this.LanguagesMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.LanguagesMenuLangEN,
            this.LanguagesMenuLangPL});
            this.LanguagesMenu.ResumeLayout(false);
            this.LanguagesMenu.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        #region FunctionsForLoadAndSaveUserDataParametersXMLFile
        //
        //----In This region there are defined functions for loading and saving UserData Parametrs xml file for further use
        //

        //
        //Function for loading UserData Parametrs xml file
        //
        public void LoadUserDataParametersXML()
        {
            UserDataParameters.Load("UserData/Parameters.xml");

            //Location X and Y
            XmlNodeList xnlWezly = UserDataParameters.GetElementsByTagName("X");
            if (xnlWezly != null)
                foreach (XmlNode xnWezel in xnlWezly)
                   MainSCRLocationX = Int32.Parse(xnWezel.InnerText);

            xnlWezly = UserDataParameters.GetElementsByTagName("Y");
            if (xnlWezly != null)
                foreach (XmlNode xnWezel in xnlWezly)
                    MainSCRLocationY = Int32.Parse(xnWezel.InnerText);

            string pom = "en"; //setting deflaut language if language not recognized by program

            //Language
            xnlWezly = UserDataParameters.GetElementsByTagName("Language");
            if (xnlWezly != null)
            {
                foreach (XmlNode xnWezel in xnlWezly)
                    pom = (string)(xnWezel.InnerText);
                if (pom.Equals("pl"))
                {
                    language = CultureInfo.CreateSpecificCulture("pl");
                    ChangeLanguage();
                }
                if (pom.Equals("en"))
                {
                    language = CultureInfo.CreateSpecificCulture("en");
                    ChangeLanguage();
                }
            }
        }

        //
        //Function for saving UserData Parameters xml file
        //
        public void SaveUserDataParametersXML()
        {
            UserDataParameters.Load("UserData/Parameters.xml");

            //Location X and Y
            MainSCRLocationX = this.Location.X;
            MainSCRLocationY = this.Location.Y;

            XmlNodeList xnlWezly = UserDataParameters.GetElementsByTagName("X");
                foreach (XmlNode xnWezel in xnlWezly)
                    xnWezel.InnerText = MainSCRLocationX.ToString();

            xnlWezly = UserDataParameters.GetElementsByTagName("Y");
                foreach (XmlNode xnWezel in xnlWezly)
                    xnWezel.InnerText = MainSCRLocationY.ToString();

            //Language 
            string pom = "en";
            if (language.Equals(CultureInfo.CreateSpecificCulture("pl")))
                pom = "pl";
            if (language.Equals(CultureInfo.CreateSpecificCulture("en")))
                pom = "en";
            xnlWezly = UserDataParameters.GetElementsByTagName("Language");
            foreach (XmlNode xnWezel in xnlWezly)
                xnWezel.InnerText = pom;

            UserDataParameters.Save("UserData/Parameters.xml");
        }

        #endregion
        
        #region ChangeLanguage

        //
        //----Function below is for changing the language on all Controls elements
        //----After adding a new controls element must add here changing of text
        private void ChangeLanguage()
        {
            //LoadUserDataParametersXML(); --this coused stack overflow error because of loading starting language, should be deleted if no new errors/bugs detected
            this.LanguageLabel.Text = res_man.GetString("LanguageXX", language);
            this.VersionLabel.Text = res_man.GetString("VersionLabel", language) + release;
            //this.TimeOfSession.Text = res_man.GetString("CurrentSessionLasts", language) + " " + TimeOfSessionInt.ToString() + " " + res_man.GetString("Minutes", language);
            ShowActualSessionTime();   //Showing actual session time
            this.CloseButton.Text = res_man.GetString("CloseButton", language);
            this.SettingsButton.Text = res_man.GetString("SettingsButton", language);
            this.TrayIcon.BalloonTipText = res_man.GetString("TrayIconBalloonTipText", language);
            this.HistoryButton.Text = res_man.GetString("HistoryButton", language);
            this.AuthorLabel.Text = YearOfRelease + res_man.GetString("AuthorLabel", language);
            this.TitleLabel.Text = res_man.GetString("TitleLabel", language);
            this.ProcessorNameLabel.Text = res_man.GetString("ProcessorNameLabel", language) + ProcessorName;
            this.ProcessorCoresLabel.Text = res_man.GetString("ProcessorCoresLabel", language) + "   " + NumberOfProcessorCores.ToString();
            this.ProcessorTemperatureLabel.Text = res_man.GetString("ProcessorTemperatureLabel", language) + "  -----";
            this.OverallCPUusageLabel.Text = res_man.GetString("OverallCPUusageLabel", language);
            this.OverallCPUusageValueLabel.BringToFront();
            this.OverallRAMMemoryLabel.Text = res_man.GetString("OverallRAMMemoryLabel", language);
            this.TotalRAMMemoryLabel.Text = res_man.GetString("TotalRAMMemoryLabel", language);
            this.TotalRAMMemoryUsageLabel.Text = res_man.GetString("TotalRAMMemoryUsageLabel", language);
            this.NetworkIntefacesTitle.Text = res_man.GetString("NetworkIntefacesTitle", language);
            this.WirelessLabel.Text = res_man.GetString("WirelessLabel", language);
            this.WireNetworkLabel.Text = res_man.GetString("WireNetworkLabel", language);
        }

        #endregion

        #region NotifyIconInSystemTray
        //
        //----Below is the section for Notify Icon in system tray----
        //        

        //MainSCR Event - when form is Resize
        private void MainSCR_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized) //If MainSCR is minimized then activate TrayIcon
            {
                TrayIcon.Visible = true;
                TrayIcon.ShowBalloonTip(500);                    
            }           
        }

        //Click on TrayIcon event
        private void TrayIcon_Click(object sender, EventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;           
            TrayIcon.Visible = false;
        }

        #endregion

        #region CPU_Usage_Value

        //
        //----In this region there is a function getCPUCounter which is checking actual usage of CPU
        //
        public object getCPUCounter()
        {            
            ManagementObject processor = new ManagementObject("Win32_PerfFormattedData_PerfOS_Processor.Name='_Total'");
            processor.Get();

            return double.Parse(processor.Properties["PercentProcessorTime"].Value.ToString());                     

        }
        #endregion

        #region CPU_Name

        //
        //----In this region there is a function getCPUName which is checking type of current CPU and later in program is showing this label
        //
        public object getCPUName()
        {
            string cpuname = "unknown";
            ManagementObjectSearcher mos = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Processor");
            foreach (ManagementObject mo in mos.Get())
            {
                cpuname = (mo["Name"]).ToString();
            }
            return cpuname;
        }

        #endregion

        #region RAM_usage_Value

        //
        //----In this region there is a function getRAMusage which is checking actual usage of total RAM
        //
        public object getRAMusage()
        {
            double memAvailable, memPhysical;

            PerformanceCounter pCntr = new PerformanceCounter("Memory", "Available KBytes");
            memAvailable = (double)pCntr.NextValue();  // Returns Available RAM memory in KBytes
            memAvailable = memAvailable/1000/1024;

            memPhysical = getPhysicalMemory();

            return Math.Truncate((memPhysical - memAvailable) * 100 / memPhysical);

        }

        #endregion

        #region Total_RAM_availible

        //
        //----In this region there is a function getPhysicalMemory which is checking total RAM availible for the system
        //
        private double getPhysicalMemory()
        {
            ObjectQuery winQuery = new ObjectQuery("SELECT * FROM Win32_ComputerSystem");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(winQuery);

            double memory = 0;
            foreach (ManagementObject item in searcher.Get())
            {
                memory = double.Parse(item["TotalPhysicalMemory"].ToString()); // Returns total RAM memory in bytes
            }
            return Math.Truncate(memory/1000/1024/1024);
        }


        #endregion

        #region RAM_in_use_Value

        //
        //----In this region there is a function getRAMinUse which is checking actual usage of RAM
        //
        public object getRAMinUse()
        {
            double memAvailable, memPhysical;

            PerformanceCounter pCntr = new PerformanceCounter("Memory", "Available KBytes");
            memAvailable = (double)pCntr.NextValue();   // Returns Available RAM memory in KBytes
            memAvailable = memAvailable/1000/1024;

            memPhysical = getPhysicalMemory();

            //TEST
            //But next release can be done yet, because this can be adjusted in future
            return Math.Truncate(100*(memPhysical - memAvailable + 0.23))/100;   //const 0.23 is a try to retrieve a proper value after truncate of Total Physical Memory, must be tested

        }

        #endregion

        #region Show_Actual_TimeOFSession_Value

        public void ShowActualSessionTime()
        {

            int Hours = 0;   //Variables for counting hours and minutes
            int Minutes = 0;

            if (TimeOfSessionInt < 60)   //if question for displaying hours and minutes or only minutes
            {
                TimeOfSession.Text = res_man.GetString("CurrentSessionLasts", language) + " " + TimeOfSessionInt.ToString() + " " + res_man.GetString("Minutes", language);

            }
            else
            {
                Hours = TimeOfSessionInt / 60;
                Minutes = TimeOfSessionInt - (Hours * 60);
                TimeOfSession.Text = res_man.GetString("CurrentSessionLasts", language) + " " + Hours.ToString() + " " + res_man.GetString("Hours", language) + " " + Minutes.ToString() + " " + res_man.GetString("Minutes", language);
            }
        }

        #endregion

        #region GetWirelessNetworkIP

        //
        //----Below is method for getting Wireless IP Address
        //
        public string getWirelessNetworkIP()
        {
            string IP = res_man.GetString("NotDetected", language);
            foreach (NetworkInterface adapter in nics)
            {
                if (adapter.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
                {
                    foreach (UnicastIPAddressInformation ip in adapter.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                           IP = ip.Address.ToString();
                        }
                    }
                }
                
            }

            return IP;
        }

        #endregion

        #region GetIPObtainMethod

        //
        //----Below is method for getting IP obtain method: Auto or Manual
        //
        public string getIPObtainForWirelessMethod()
        {
            string ObtainMethod = res_man.GetString("NotDetected", language);
            foreach (NetworkInterface adapter in nics)
            {                
                if (adapter.NetworkInterfaceType == NetworkInterfaceType.Wireless80211)
                {
                    foreach (UnicastIPAddressInformation ip in adapter.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                        {
                            IPInterfaceProperties properties = adapter.GetIPProperties();
                            if (properties.GetIPv4Properties().IsDhcpEnabled)
                            {
                                ObtainMethod = res_man.GetString("IPAutoObtain", language);
                            }
                            else
                            {
                                ObtainMethod = res_man.GetString("IPManualObtain", language);
                            }
                        }
                    }
                }                
            }

            return ObtainMethod;
        }

        #endregion
        
        #region GetWirelessSSID
        //
        //----In  this part there is a method to get wireless SSID using NativeWifi dll
        //
        public string getWirelessSSID()
        {
            WlanClient wlan = new WlanClient();   //WlanClient is class from NativeWifi
            string SSIDret = res_man.GetString("NotDetected", language);

            //
            //----added try catch method because during changing of wireless network checking of ssid from NativeWiFi can throw an exception
            //
            try
            {
                foreach (WlanClient.WlanInterface wlanInterface in wlan.Interfaces)
                {
                    Wlan.Dot11Ssid ssid = wlanInterface.CurrentConnection.wlanAssociationAttributes.dot11Ssid;
                    SSIDret = new string(Encoding.ASCII.GetChars(ssid.SSID, 0, (int)ssid.SSIDLength));
                }
            }
            catch
            {
                SSIDret = res_man.GetString("NotDetected", language); ;
            }

            return SSIDret;
        }


        #endregion

        #region ProgramEvents
        //
        //----Below is the section with Events----
        //

        //
        // CloseButton Click Event
        //
        void CloseButton_Click(object sender, EventArgs e)   //CloseButton Click Event
        {
            this.WindowState = FormWindowState.Minimized;
            Hide();
        }

        //
        // SettingsButton Click Event
        //
        void SettingsButton_Click(object sender, EventArgs e)   //SettingsButton Click Event
        {
            Close();
        }

        //
        // HistoryButton Click Event
        //
        void HistoryButton_Click(object sender, EventArgs e)   //HistoryButton Click Event
        {
            HistorySCR frm = new HistorySCR(language, MainSCRLocationX, MainSCRLocationY, this.Width);
            frm.Show();
        }

        //
        // TimeOfSessionTimer Tick Event (every 1 min)
        //
        private void TimeOfSessionTimer_Tick(object sender, EventArgs e) //TimeOfSessionTimer Tick Event (every 1 min)
        {
            //Increment value of TimeOfSession and update text in TimeOfSession Label to the current value
            TimeOfSessionInt++; //evere one minute var TimeOfSession is incrementing
            ShowActualSessionTime();   //Showing actual session time
            
        }

        //
        // Time1SecTimer Tick Event (every 1 sec)
        //
        private void Time1SecTimer_Tick(object sender, EventArgs e) //Time1SecTimer Tick Event (every 1 sec)
        {
            //
            //----Below is a part for showing actual CPU usage
            //
            int ActualCPUusageRead = Int32.Parse(getCPUCounter().ToString());
            //below are commands for showing current CPU usage on OverallCPUusageValueLabel
            string cpuPercent = ActualCPUusageRead.ToString();            
            this.OverallCPUusageValueLabel.Text = cpuPercent + " %";

            //Command used for moving all records in array one pos to left and new 19 value is null
            Array.Copy(CPUusageArray, 1, CPUusageArray, 0, CPUusageArray.Length - 1);
            //below are commands for adding actual cpu usage read into CPUusageArray on last position
            CPUusageArray[19] = ActualCPUusageRead;

            //
            //----Below is part for drawing a CPU usage graph
            //            
            this.CPUusageGraph.Invalidate();    //used for drawing new graph of CPU usage every timer tick
            
            //
            //----Below is part for showing RAM parameters
            //            
            this.PercentageRAMMemoryLabel.Text = getRAMusage().ToString() + " %";
            this.TotalRAMMemoryValueLabel.Text = getPhysicalMemory().ToString() + " GB";
            this.TotalRAMMemoryUsageValueLabel.Text = getRAMinUse().ToString() + " GB";
            ActualRAMusage = Int32.Parse(getRAMusage().ToString());

            //
            //----Below is part for drawing a RAM usage graph
            //            
            this.RAMusageGraph.Invalidate();    //used for drawing new graph of RAM usage every timer tick

            //
            //----Below is part for showing Network Interfaces in labels
            //
            computerProperties = IPGlobalProperties.GetIPGlobalProperties();
            nics = NetworkInterface.GetAllNetworkInterfaces();    //needed to get new info when user switch between networks

            this.WirelessIPAddress.Text = "IP: " + getWirelessNetworkIP();
            this.WirelessIPObtainMethod.Text = "IP: " + getIPObtainForWirelessMethod();
            this.WirelessIPSSID.Text = "SSID: " + getWirelessSSID();

        }

        //
        //Paint event for CPUusageGraph picture box
        //
        private void CPUusageGraph_Paint(object sender, PaintEventArgs e)
        {            
            if (CPUusageGraphBitMap == null)
            {
                CPUusageGraphBitMap = new Bitmap(152, 80);   //size of bitmap must be the same as size of picturebox CPUusageGraph
                CPUusageGraphGraphics = Graphics.FromImage(CPUusageGraphBitMap);
                CPUusageGraphGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                CPUusageGraphGraphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.GammaCorrected;
                CPUusageGraphGraphics.Clear(Color.Transparent);
            }
            CPUusageGraphGraphics.Clear(Color.Transparent);

            int cellSize = 8;  //cell size must be the same as the grid cell size
            Pen p = new Pen(Color.Lime, 2); 

            //below is an for loop for drawing CPU usage graph on data stored in CPUusageArray
            for (int i = 0; i < 19; i++)
            {
                Point oldpoint = new Point(i * cellSize, (int)(80 - CPUusageArray[i] * 0.8));   //multiplication by 0.8 is for adjusting graph to grid size
                Point newpoint = new Point((i + 1) * cellSize, (int)(80 - CPUusageArray[i + 1] * 0.8));   //multiplication by 0.8 is for adjusting graph to grid size
                CPUusageGraphGraphics.DrawLine(p, oldpoint, newpoint);
            }
            e.Graphics.DrawImage(CPUusageGraphBitMap, 0, 0);
        }

        //
        //Paint event for RAMusageGraph picture box
        //
        private void RAMusageGraph_Paint(object sender, PaintEventArgs e)
        {
            if (RAMusageGraphBitMap == null)
            {
                RAMusageGraphBitMap = new Bitmap(70, 75);   //size of bitmap must be the same as size of picturebox RAMusageGraph
                RAMusageGraphGraphics = Graphics.FromImage(RAMusageGraphBitMap);
                RAMusageGraphGraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                RAMusageGraphGraphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.GammaCorrected;
                RAMusageGraphGraphics.Clear(Color.Transparent);
            }
            RAMusageGraphGraphics.Clear(Color.Transparent);

            Pen p1 = new Pen(Color.Lime, 4);   //pen used for active RAM memory on RAM usage graph
            Pen p2 = new Pen(Color.Green, 4);   //pen used for non-active RAM memory on RAM usage graph

            int stepsize = 6;   //const used for drawing next line up to last one
            int StartPointY = 65;     //starting point for start drawing

            //below is an for loop for drawing RAM usage graph 
            for (int i = 0; i < 10; i++)
            {
                int CompInt = i * 10;   //var used for comparing actual drawing line with actual RAM usage

                //point for first line
                Point oldpoint = new Point(10, StartPointY - i * stepsize);
                Point newpoint = new Point(34, StartPointY - i * stepsize);                  

                //points for second line
                Point oldpoint2 = new Point(36, StartPointY - i * stepsize);
                Point newpoint2 = new Point(60, StartPointY - i * stepsize);

                if (CompInt > ActualRAMusage)    //if we are drawing line for not used RAM memory then use pen p2 else use pen p1 (for active RAM)
                {
                    RAMusageGraphGraphics.DrawLine(p2, oldpoint, newpoint);
                    RAMusageGraphGraphics.DrawLine(p2, oldpoint2, newpoint2);
                }
                else
                {
                    RAMusageGraphGraphics.DrawLine(p1, oldpoint, newpoint);
                    RAMusageGraphGraphics.DrawLine(p1, oldpoint2, newpoint2);
                }
                
            }

            e.Graphics.DrawImage(RAMusageGraphBitMap, 0, 0);
        }

        //
        // Click event for LanguageLabel
        //
        private void LanguageLabel_Click(object sender, EventArgs e)
        {
            LanguagesMenu.Show(this.Location.X + LanguageLabel.Location.X, this.Location.Y + LanguageLabel.Location.Y);
        }

        //
        // Click event for choosing English Language
        //
        private void LanguagesMenuLangEN_Click(object sender, EventArgs e)
        {
            language = CultureInfo.CreateSpecificCulture("en");
            ChangeLanguage();
        }
        //
        // Click event for choosing Polish Language
        //
        private void LanguagesMenuLangPL_Click(object sender, EventArgs e)
        {
            language = CultureInfo.CreateSpecificCulture("pl");
            ChangeLanguage();
        }

        #endregion

        #region ProgramCommandsForDragingTheFormByMouse

        //
        //----In this region there are commands of funcionality for dragging the MainSCR by mouse
        //

        private Point mouse_offset;  //Var for keeping the current position of mouse

        //
        //Event for mouse down on MainSCR
        //
        private void MainSCR_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            mouse_offset = new Point(-e.X, -e.Y);
        }

        //
        //Event for mouse move with clicked button to drag the MainSCR
        //
        private void MainSCR_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Point mousePos = Control.MousePosition;
                mousePos.Offset(mouse_offset.X, mouse_offset.Y);
                this.Location = mousePos;
                SaveUserDataParametersXML();
            }
        }

        #endregion

        #region EventsForLoadingAndClosingMainSCR
        //
        //----In this region there are events for loading and closing MainSCR
        //

        //
        //Event for loading MainSCR
        //
        private void MainSCR_Load(object sender, EventArgs e)
        {
            //Loading user data from XMLfile
            LoadUserDataParametersXML();
            if (MainSCRLocationX != 0 || MainSCRLocationY != 0)
            {
                this.Location = new Point (MainSCRLocationX, MainSCRLocationY);
            }

            //Getting Name of CPU
            ProcessorName = getCPUName().ToString();
            //And applying it to ProcessorNameLabel
            this.ProcessorNameLabel.Text = res_man.GetString("ProcessorNameLabel", language) + ProcessorName;  //must be done during loading to see it after startup
        }

        //
        //Event for saving MainSCR parameters after closing
        //
        private void MainSCR_FormClosed(object sender, EventArgs e)
        {
            SaveUserDataParametersXML();
        }

        #endregion

        #region CreatingFormFrame&GridOnPaint
        //
        //----In this region program is creating form frame & grids for graphs during OnPaint
        //

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            using (Graphics g = e.Graphics)
            {
                //below is drawing of a form frame
                Rectangle rect = ClientRectangle;
                rect.Location = new Point(1, 1);                  // specify rectangle relative position here (always 1, 1)
                rect.Size = new Size(220, 498);                  // specify rectangle size here (size of form -2, -2)

                using (Pen pen = new Pen(Color.Black, 2))    // specify color here and pen type here
                {
                    g.DrawRectangle(pen, rect);             //drawing of frame
                }

                //below is drawing of line over authors & version labels
                using (Pen pen = new Pen(Color.Black, 1))    // specify color here and pen type here
                {
                    g.DrawLine(pen, 0, this.Height - 16, this.Width, this.Height - 16);
                }

                //below is drawing of line over history, settings & close buttons
                using (Pen pen = new Pen(Color.Black, 1))    // specify color here and pen type here
                {
                    g.DrawLine(pen, 0, this.Height - 45, this.Width, this.Height - 45);
                }

                //below is drawing of line separating CPU info screen and RAM Memory info screen
                using (Pen pen = new Pen(Color.Black, 1))    // specify color here and pen type here
                {
                    g.DrawLine(pen, 0, this.Height - 305, this.Width, this.Height - 305);
                }

                //below is drawing of line separating RAM Memory info screen and IP parameters section
                using (Pen pen = new Pen(Color.Black, 1))    // specify color here and pen type here
                {
                    g.DrawLine(pen, 0, this.Height - 175, this.Width, this.Height - 175);
                }

                //below is drawing of line under System Monitor title label
                using (Pen pen = new Pen(Color.Black, 2))    // specify color here and pen type here
                {
                    g.DrawLine(pen, 40, 30, this.Width - 40, 30);
                }

                //below is declaration of variables used for drawing a grid for CPU performance graph
                int numOfCellsY = 11;
                int numOfCellsX = 20;
                int cellSize = 8;
                int StartPointX = 30;
                int StartPointY = 50;

                //below is drawing of background of grid for CPU performance graph
                Rectangle CPUPerfB = ClientRectangle;
                CPUPerfB.Location = new Point(StartPointX, StartPointY);  // specify rectangle relative position here
                CPUPerfB.Size = new Size(152, 80);   // specify rectangle size here (must be the same as values of lenths in loops creating lines of a grid)
               
                using (SolidBrush brush = new SolidBrush(Color.Black))    // specify color here and pen type here
                {
                    g.FillRectangle(brush, CPUPerfB);             //drawing of frame
                }

                //below is drawing of grid for CPU perfomance graph                                
                Pen p = new Pen(Color.Green, 1);    //defining the pen for drawing a grid

                for (int y = 0; y < numOfCellsY; ++y)  //loop for creating Y lines, value of X in second point is lenth
                {
                    g.DrawLine(p, StartPointX, StartPointY + y * cellSize, StartPointX + 152, StartPointY + y * cellSize);
                }

                for (int x = 0; x < numOfCellsX; ++x)  //loop for creating X line, value of Y in second point is lenth
                {
                    g.DrawLine(p, StartPointX + x * cellSize, StartPointY, StartPointX + x * cellSize, StartPointY + 80);
                }

                //below is drawing of background for RAM usage graph
                Rectangle RAMPerfB = ClientRectangle;
                RAMPerfB.Location = new Point(140, 220);  // specify rectangle relative position here
                RAMPerfB.Size = new Size(70, 100);   // specify rectangle size here

                using (SolidBrush brush = new SolidBrush(Color.Black))    // specify color here and pen type here
                {
                    g.FillRectangle(brush, RAMPerfB);             //drawing of frame
                }

                
            }
        }
        #endregion

    }
}
