using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;
using System.Resources;
using System.Drawing;

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

        #endregion

        #region FormAppearance&MainProgram

        //----Below defining further objects on this form
        private System.Windows.Forms.Button CancelButton;
        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.Label SettingsSCRTitle;
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
            // Adding objects to Controls
            //  
            this.Controls.AddRange(new Control[] { CancelButton, SaveButton, SettingsSCRTitle });
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
        }

        #endregion
    }
}
