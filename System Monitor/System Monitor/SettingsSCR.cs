using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
using System.Resources;
using System.Drawing;
using System.IO;
using IWshRuntimeLibrary;

namespace System_Monitor
{
    class SettingsSCR : Form
    {
        #region VariablesDeclaration
        //
        //----Program VARiables declaration----
        //

        //Location Variables (deflaut value is 1, 1, but later on in program we take values of location from MainSCR
        int SettingsSCRXPos = 1;
        int SettingsSCRYPos = 1;
        int MainSCRoffset = 0; //used for moving SettingsSCR next to MainSCR
        int MainSCRHeight1 = 1;   

        //Languages Variables
        ResourceManager res_man = new ResourceManager("System_Monitor.Data.lang", typeof(MainSCR).Assembly);    // declare Resource manager to access to specific cultureinfo
        CultureInfo language = CultureInfo.CreateSpecificCulture("en");       // declare culture info, this var is for choosing specyfic language

        //Patch Variables
        static string PatchToApp = AppDomain.CurrentDomain.BaseDirectory;  //static var with current path to app
                
        #endregion

        #region FormAppearance&MainProgram

        //----Below defining further objects on this form
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.Label SettingsSCRTitle;
        private System.Windows.Forms.Label LanguageLabel;
        private System.Windows.Forms.ComboBox LanguagesComboBox;
        private System.Windows.Forms.CheckBox RunAtSystemStartupCheckBox;
        //----End of defining objects on this form

        public SettingsSCR(CultureInfo langH, int MainSCRX, int MainSCRY, int MainSCRWidth, int MainSCRHeight)
        {
            //Parameter langH is used for starting HistorySCR with anguage choosen by user in MainSCR
            language = langH;

            //Parameters MainSCRX and MainSCRY are used to set up a starting location of HistorySCR
            SettingsSCRXPos = MainSCRX;
            SettingsSCRYPos = MainSCRY;
            MainSCRoffset = MainSCRWidth;
            MainSCRHeight1 = MainSCRHeight;


            //Events for SettingsSCR
            this.Load += SettingsSCR_Load;

            // 
            // SettingsSCR
            //   
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SettingsSCR";
            this.Text = "SettingsSCR";
            this.Height = 230;
            this.Width = MainSCRoffset;
            this.ShowInTaskbar = false;
            this.BackColor = Color.FromArgb(255, 255, 255);

            // 
            // CancelButton
            //  
            this.CancelButton = new System.Windows.Forms.Button();
            this.CancelButton.Visible = true;
            this.CancelButton.Location = new System.Drawing.Point(145, 200);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(70, 20);
            this.CancelButton.Text = res_man.GetString("CancelButton", language);
            this.CancelButton.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.CancelButton.UseVisualStyleBackColor = true;
            this.CancelButton.Font = new Font(new FontFamily(System.Drawing.Text.GenericFontFamilies.Serif), 7);
            this.CancelButton.Click += CancelButton_Click;

            // 
            // SaveButton
            //  
            this.SaveButton = new System.Windows.Forms.Button();
            this.SaveButton.Visible = true;
            this.SaveButton.Location = new System.Drawing.Point(70, 200);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(70, 20);
            this.SaveButton.Text = res_man.GetString("SaveButton", language);
            this.SaveButton.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Font = new Font(new FontFamily(System.Drawing.Text.GenericFontFamilies.Serif), 7);
            this.SaveButton.Click += SaveButton_Click;

            // 
            // Label SettingsSCRTitle
            //
            this.SettingsSCRTitle = new System.Windows.Forms.Label();
            this.SettingsSCRTitle.Location = new System.Drawing.Point(2, 5);
            this.SettingsSCRTitle.Name = "SettingsSCRTitle";
            this.SettingsSCRTitle.TabIndex = 1;
            this.SettingsSCRTitle.Size = new System.Drawing.Size(218, 22);
            this.SettingsSCRTitle.Text = res_man.GetString("SettingsSCRTitle", language);
            this.SettingsSCRTitle.Font = new Font(new FontFamily(System.Drawing.Text.GenericFontFamilies.Serif), 12);
            this.SettingsSCRTitle.TextAlign = System.Drawing.ContentAlignment.TopCenter;

            // 
            // Label LanguageLabel
            // 
            this.LanguageLabel = new System.Windows.Forms.Label();
            this.LanguageLabel.Location = new System.Drawing.Point(40, 40);
            this.LanguageLabel.Name = "LanguageLabel";
            this.LanguageLabel.TabIndex = 1;
            this.LanguageLabel.Text = res_man.GetString("LanguageXX", language);
            this.LanguageLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;

            //
            // List langs for displaying in combobox LanguagesComboBox
            //
            List<string> langs = new List<string>();

            langs.Add(res_man.GetString("LangShortXX", language));   //at first position we are showing actual language

            //then we are checking what is actual language and we are showing other languages on next positions

            //Below for PL language
            if (res_man.GetString("LangShortXX", language).Equals("PL"))
            langs.Add(res_man.GetString("LangShortXX", CultureInfo.CreateSpecificCulture("en")));
            //Below for EN language
            if (res_man.GetString("LangShortXX", language).Equals("EN"))
            langs.Add(res_man.GetString("LangShortXX", CultureInfo.CreateSpecificCulture("pl")));          
            
            
            // 
            // LanguagesComboBox
            // 
            this.LanguagesComboBox = new System.Windows.Forms.ComboBox();
            this.LanguagesComboBox.FormattingEnabled = true;
            this.LanguagesComboBox.Location = new System.Drawing.Point(150, 38);
            this.LanguagesComboBox.Name = "LanguagesComboBox";
            this.LanguagesComboBox.Size = new System.Drawing.Size(60, 24);
            this.LanguagesComboBox.TabIndex = 3;            
            this.LanguagesComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            this.LanguagesComboBox.DataSource = langs;

            // 
            // RunAtSystemStartupCheckBox
            // 
            this.RunAtSystemStartupCheckBox = new System.Windows.Forms.CheckBox();
            this.RunAtSystemStartupCheckBox.AutoSize = true;
            this.RunAtSystemStartupCheckBox.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.RunAtSystemStartupCheckBox.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.RunAtSystemStartupCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.RunAtSystemStartupCheckBox.Location = new System.Drawing.Point(2, 80);
            this.RunAtSystemStartupCheckBox.Name = "RunAtSystemStartupCheckBox";
            this.RunAtSystemStartupCheckBox.Size = new System.Drawing.Size(218, 21);
            this.RunAtSystemStartupCheckBox.TabIndex = 4;
            this.RunAtSystemStartupCheckBox.Text = res_man.GetString("RunAtSystemStartup", language);
            this.RunAtSystemStartupCheckBox.UseVisualStyleBackColor = true;

            // 
            // Adding objects to Controls
            //  
            this.Controls.AddRange(new Control[] { CancelButton, SaveButton, SettingsSCRTitle, LanguageLabel, LanguagesComboBox, RunAtSystemStartupCheckBox });
        }

        #endregion       

        #region ProgramEvents

        //
        //----Below is the section with Events----
        //

        //
        // CancelButton Click Event
        //
        void CancelButton_Click(object sender, EventArgs e)   //CancelButton Click Event
        {
            Close();
        }

        //
        // SaveButton Click Event
        //
        void SaveButton_Click(object sender, EventArgs e)   //SaveButton Click Event
        {
            //
            //----After button Save click we are checking all fields in settings and applying this settings to MainSCR
            //

            //
            //----Checking what language is selected and applying this setting to MainSCR
            //

            //Language EN
            if (this.LanguagesComboBox.SelectedItem.Equals("EN"))
            {
                if (System.Windows.Forms.Application.OpenForms["MainSCR"] != null)
                {
                    (System.Windows.Forms.Application.OpenForms["MainSCR"] as MainSCR).LanguagesMenuLangEN_Change();
                }
            }

            //Language PL
            if (this.LanguagesComboBox.SelectedItem.Equals("PL"))
            {
                if (System.Windows.Forms.Application.OpenForms["MainSCR"] != null)
                {
                    (System.Windows.Forms.Application.OpenForms["MainSCR"] as MainSCR).LanguagesMenuLangPL_Change();
                }
            }

            //
            //----Checking if RunAtSystemStartupCheckbox is checked and apply
            //
            string startUpFolder = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            string shortcutPath = startUpFolder + @"\System_Monitor.lnk";

            if (this.RunAtSystemStartupCheckBox.Checked == true) //if checkbox checked
            {
                if (System.IO.File.Exists(shortcutPath))  //if file already exists we don't have to create it
                {
                    //MessageBox.Show("RunAtSystemStartupCheckBox checked but file already exist");   //only for tests/debug purposes
                }
                else   //if file don't exist we need to create a new one
                {
                    try
                    {
                        WshShell shell = new WshShell();
                        IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);
                        shortcut.Description = "System Monitor Shortcut";
                        shortcut.TargetPath = PatchToApp + @"\System Monitor.exe";
                        shortcut.WorkingDirectory = PatchToApp;
                        shortcut.Save();
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show("Error during 'RunAtSystemStartupCheckBox checked, file don't exist - created new file': " + err);
                    }
                    //MessageBox.Show("RunAtSystemStartupCheckBox checked, file don't exist - created new file");   //only for tests/debug purposes
                }
            }
            else   //if checkbox not checked
            {
                if (System.IO.File.Exists(shortcutPath))   //if file exist we need to delete it
                {
                    try
                    {
                        System.IO.File.Delete(shortcutPath);
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show("Error during 'RunAtSystemStartupCheckBox NOT checked and file exist - deleted': " + err);
                    }
                    //MessageBox.Show("RunAtSystemStartupCheckBox NOT checked and file exist - deleted");   //only for tests/debug purposes
                }
                else   //if file don't exist we don't have to delete it
                {
                    //MessageBox.Show("RunAtSystemStartupCheckBox NOT checked but file already don't exist");   //only for tests/debug purposes
                }
            }
            
            //After all settings are applied form SettingsSCR can be closed
            Close();
        }

        #endregion

        #region CreatingFormFrameOnPaint
        //
        //----In this region program is creating form frame during OnPaint
        //

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            using (Graphics g = e.Graphics)
            {
                Rectangle rect = ClientRectangle;
                rect.Location = new Point(1, 1);                  // specify rectangle relative position here (always 1, 1)
                rect.Size = new Size(this.Width - 2, this.Height - 2);                  // specify rectangle size here (size of form -2, -2)

                using (Pen pen = new Pen(Color.Black, 2))    // specify color here and pen type here
                {
                    g.DrawRectangle(pen, rect);             //drawing of frame
                }

                //below is drawing of line under System Monitor title label
                using (Pen pen = new Pen(Color.Black, 2))    // specify color here and pen type here
                {
                    g.DrawLine(pen, 40, 30, this.Width - 40, 30);
                }
            }
        }
        #endregion

        #region EventForLoadingSettingsSCR
        //
        //----In this region there are events for loading SettingsSCR
        //

        //
        //Event for loading SettingsSCR
        //
        private void SettingsSCR_Load(object sender, EventArgs e)
        {
            this.Location = new Point(SettingsSCRXPos, SettingsSCRYPos + MainSCRHeight1 - this.Height); 
           
            //
            //Checking if shortcut to application is created in Startup directory
            //
            string startUpFolder = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
            string shortcutPath = startUpFolder + @"\System_Monitor.lnk";

            if (System.IO.File.Exists(shortcutPath))
            {
                RunAtSystemStartupCheckBox.Checked = true;
                //MessageBox.Show("Shortcut file exists in startup folder: " + shortcutPath);   //only for tests/debug purposes
            }
            else
            {
                RunAtSystemStartupCheckBox.Checked = false;
                //MessageBox.Show("Shortcut file don't exists in startup folder: " + shortcutPath);   //only for tests/debug purposes
            }
        }

        #endregion
    }
}
