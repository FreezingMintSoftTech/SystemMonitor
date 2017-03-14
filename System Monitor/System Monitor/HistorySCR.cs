using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using System.Resources;
using System.Globalization;

namespace System_Monitor
{
    class HistorySCR : Form
    {
        #region VariablesDeclaration
        //
        //----Program VARiables declaration----
        //

        //Location Variables (deflaut value is 1, 1, but later on in program we take values of location from MainSCR
        int HistorySCRXPos = 1;
        int HistorySCRYPos = 1;
        int MainSCRoffset = 0; //used for moving HistorySCR next to MainSCR

        //Languages Variables
        ResourceManager res_man = new ResourceManager("System_Monitor.Data.lang", typeof(MainSCR).Assembly);    // declare Resource manager to access to specific cultureinfo
        CultureInfo language = CultureInfo.CreateSpecificCulture("en");       // declare culture info, this var is for choosing specyfic language

        #endregion

        #region FormAppearance&MainProgram

        //----Below defining further objects on this form
        private System.Windows.Forms.Button CloseButton;
        private System.Windows.Forms.Button SessionsHistoryButton;
        private System.Windows.Forms.Label HistorySCRTitle;
        //----End of defining objects on this form


        public HistorySCR(CultureInfo langH, int MainSCRX, int MainSCRY, int MainSCRWidth)
        {
            //Parameter langH is used for starting HistorySCR with anguage choosen by user in MainSCR
            language = langH;

            //Parameters MainSCRX and MainSCRY are used to set up a starting location of HistorySCR
            HistorySCRXPos = MainSCRX;
            HistorySCRYPos = MainSCRY;
            MainSCRoffset = MainSCRWidth;

            //Events for HistorySCR
            this.Load += HistorySCR_Load;

            // 
            // HistorySCR
            //   
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "HistorySCR";
            this.Text = "HistorySCR";
            this.Height = 230;
            this.Width = 500;
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
            this.CloseButton.Location = new System.Drawing.Point(427, 200);
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.Size = new System.Drawing.Size(70, 20);
            this.CloseButton.Text = res_man.GetString("CloseButton", language);
            this.CloseButton.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.CloseButton.UseVisualStyleBackColor = true;
            this.CloseButton.Font = new Font(new FontFamily(System.Drawing.Text.GenericFontFamilies.Serif), 7);
            this.CloseButton.Click += CloseButton_Click;

            // 
            // SessionsHistoryButton
            //  
            this.SessionsHistoryButton = new System.Windows.Forms.Button();
            this.SessionsHistoryButton.Visible = true;
            this.SessionsHistoryButton.Location = new System.Drawing.Point(427, 10);
            this.SessionsHistoryButton.Name = "SessionsHistoryButton";
            this.SessionsHistoryButton.Size = new System.Drawing.Size(70, 40);
            this.SessionsHistoryButton.Text = res_man.GetString("SessionsHistoryButton", language);
            this.SessionsHistoryButton.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.SessionsHistoryButton.UseVisualStyleBackColor = true;
            this.SessionsHistoryButton.Font = new Font(new FontFamily(System.Drawing.Text.GenericFontFamilies.Serif), 7);
            this.SessionsHistoryButton.Click += SessionsHistoryButton_Click;

            // 
            // Label HistorySCRTitle
            //
            this.HistorySCRTitle = new System.Windows.Forms.Label();
            this.HistorySCRTitle.Location = new System.Drawing.Point(150, 5);
            this.HistorySCRTitle.Name = "HistorySCRTitle";
            this.HistorySCRTitle.TabIndex = 1;
            this.HistorySCRTitle.Size = new System.Drawing.Size(200, 20);
            this.HistorySCRTitle.Text = res_man.GetString("HistorySCRTitle", language);
            this.HistorySCRTitle.TextAlign = System.Drawing.ContentAlignment.TopLeft; 

            // 
            // Adding objects to Controls
            //  
            this.Controls.AddRange(new Control[] { CloseButton, HistorySCRTitle, SessionsHistoryButton });
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
            Close();
        }

        //
        // SessionsHistoryButton Click Event
        //
        void SessionsHistoryButton_Click(object sender, EventArgs e)   //SessionsHistoryButton Click Event
        {
            Close();
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
               // SaveUserDataParametersXML();
            }
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
                rect.Size = new Size(498, 228);                  // specify rectangle size here (size of form -2, -2)

                using (Pen pen = new Pen(Color.Black, 2))    // specify color here and pen type here
                {
                    g.DrawRectangle(pen, rect);             //drawing of frame
                }
            }
        }
        #endregion

        #region EventForLoadingHistorySCR
        //
        //----In this region there are events for loading HistorySCR
        //

        //
        //Event for loading HistorySCR
        //
        private void HistorySCR_Load(object sender, EventArgs e)
        {
            //The if statement is for checking that the HistorySCR will not be displayed behind the 0 point of screen
            if (HistorySCRXPos - this.Width < 0)
            this.Location = new Point(HistorySCRXPos + MainSCRoffset, HistorySCRYPos);  
            else
                this.Location = new Point(HistorySCRXPos - this.Width, HistorySCRYPos);
        }

        #endregion
    }
}
