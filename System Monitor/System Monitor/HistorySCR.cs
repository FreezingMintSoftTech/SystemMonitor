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

        //Database SMuserDB variables
        DatabaseConnection SMuserDB_Connection;
        string conString;

        //Other Variables
        Boolean OrderASC = true; //var used for changing sorting order

        #endregion

        #region FormAppearance&MainProgram

        //----Below defining further objects on this form
        private System.Windows.Forms.Button CloseButton;
        private System.Windows.Forms.Button SessionsHistoryButton;
        private System.Windows.Forms.Label HistorySCRTitle;
        private System.Windows.Forms.Label DateTitle;
        private System.Windows.Forms.Label TimeOfAllSessionTitle;
        private System.Windows.Forms.Label QuantityOfSessionsTitle;
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
            this.Height = 500;
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
            this.CloseButton.Location = new System.Drawing.Point(427, 470);
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
            this.SessionsHistoryButton.Location = new System.Drawing.Point(427, 50);
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
            // Label DateTitle
            //
            this.DateTitle = new System.Windows.Forms.Label();
            this.DateTitle.Location = new System.Drawing.Point(30, 40);
            this.DateTitle.Name = "DateTitle";
            this.DateTitle.TabIndex = 1;
            this.DateTitle.Size = new System.Drawing.Size(50, 20);
            this.DateTitle.Text = res_man.GetString("DateTitle", language);
            this.DateTitle.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.DateTitle.Visible = false;
            this.DateTitle.Click += DateTitle_Click;

            // 
            // Label TimeOfAllSessionTitle
            //
            this.TimeOfAllSessionTitle = new System.Windows.Forms.Label();
            this.TimeOfAllSessionTitle.Location = new System.Drawing.Point(90, 40);
            this.TimeOfAllSessionTitle.Name = "TimeOfAllSessionTitle";
            this.TimeOfAllSessionTitle.TabIndex = 1;
            this.TimeOfAllSessionTitle.Size = new System.Drawing.Size(150, 20);
            this.TimeOfAllSessionTitle.Text = res_man.GetString("TimeOfAllSessionTitle", language);
            this.TimeOfAllSessionTitle.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.TimeOfAllSessionTitle.Visible = false;
            this.TimeOfAllSessionTitle.Click += TimeOfAllSession_Click;

            // 
            // Label QuantityOfSessionsTitle
            //
            this.QuantityOfSessionsTitle = new System.Windows.Forms.Label();
            this.QuantityOfSessionsTitle.Location = new System.Drawing.Point(250, 40);
            this.QuantityOfSessionsTitle.Name = "QuantityOfSessionsTitle";
            this.QuantityOfSessionsTitle.TabIndex = 1;
            this.QuantityOfSessionsTitle.Size = new System.Drawing.Size(160, 20);
            this.QuantityOfSessionsTitle.Text = res_man.GetString("QuantityOfSessionsTitle", language);
            this.QuantityOfSessionsTitle.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.QuantityOfSessionsTitle.Visible = false;
            this.QuantityOfSessionsTitle.Click += QuantityOfSessionsTitle_Click;

            // 
            // Adding objects to Controls
            //  
            this.Controls.AddRange(new Control[] { CloseButton, HistorySCRTitle, SessionsHistoryButton, DateTitle, TimeOfAllSessionTitle, QuantityOfSessionsTitle });
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
            //
            //----Database SMuserDB connection startup
            //
            try
            {
                // Try for startup connection with SMuserDB

                SMuserDB_Connection = new DatabaseConnection();
                conString = Properties.Settings.Default.SMuserDBConnectionString;

                SMuserDB_Connection.connection_string = conString;
            }
            catch (Exception err)
            {
                MessageBox.Show("Error during SMuserDB startup at HistorySCR: " + err.Message);
            }

            //making title labels visible
            this.DateTitle.Visible = true;
            this.TimeOfAllSessionTitle.Visible = true;            
            this.QuantityOfSessionsTitle.Visible = true;

            //
            //clearing of all old labels with TAG dispose to draw new ones on click
            //
            try
            {
                List<Control> itemsToRemove = new List<Control>();
                foreach (Control ctrl in Controls)
                {
                    if (ctrl.Tag != null && ctrl.Tag.ToString() == "dispose")
                    {
                        itemsToRemove.Add(ctrl);
                    }
                }

                foreach (Control ctrl in itemsToRemove)
                {
                    Controls.Remove(ctrl);
                    ctrl.Dispose();
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("Error during clearing old labels on HistorySCR: " + err.Message);
            }

            //
            //----Creating dynamically new labels from records from SMuserDB
            //
            try
            {
                System.Data.DataSet QueryResult;  

                //at first we create label arrays for each record in SMuserDB 
                Label[] DateLabel = new Label[30];
                Label[] TimeOfAllSessionsLabel = new Label[30];
                Label[] QuantityOfSessionsLabel = new Label[30];

                //Take all records from SessionsTable order descending by Date
                SMuserDB_Connection.Sql_Query = "SELECT * FROM SessionsTable ORDER BY Date DESC";

                QueryResult = SMuserDB_Connection.GetConnection;   //Assign all this records to QueryResult
                int TimeOfSessionsInt = 0;   //Var used for showing Time of sessions as Hours and Minutes

                //In for loop we are assigning each record from QueryResult to coresponding labels
                for (int i = 0; i < QueryResult.Tables[0].Rows.Count; i++)
                {                    
                    System.Data.DataRow row = QueryResult.Tables[0].Rows[i];

                    TimeOfSessionsInt = Int32.Parse(row["TimeOfAllSessions"].ToString());
                    int hours = 0;
                    int minutes = 0;

                    DateLabel[i] = new Label();
                    DateLabel[i].Location = new System.Drawing.Point(10, 60 + i*20);
                    DateLabel[i].Name = "DateLabel" + i.ToString();
                    DateLabel[i].Tag = "dispose";
                    DateLabel[i].Text = row["Date"].ToString();
                    
                    TimeOfAllSessionsLabel[i] = new Label();
                    TimeOfAllSessionsLabel[i].Location = new System.Drawing.Point(120, 60 + i * 20);
                    TimeOfAllSessionsLabel[i].Name = "TimeOfAllSessionsLabel" + i.ToString();
                    TimeOfAllSessionsLabel[i].Tag = "dispose";
                    if (TimeOfSessionsInt < 60)
                    {
                        TimeOfAllSessionsLabel[i].Text = TimeOfSessionsInt.ToString() + " " + res_man.GetString("Minutes", language);
                    }
                    else
                    {
                        hours = TimeOfSessionsInt / 60;
                        minutes = TimeOfSessionsInt - (hours * 60);
                        TimeOfAllSessionsLabel[i].Text = hours.ToString() + " " + res_man.GetString("Hours", language) + " " + minutes.ToString() + " " + res_man.GetString("Minutes", language);
                    }

                    QuantityOfSessionsLabel[i] = new Label();
                    QuantityOfSessionsLabel[i].Location = new System.Drawing.Point(250, 60 + i * 20);
                    QuantityOfSessionsLabel[i].Name = "QuantityOfSessionsLabel" + i.ToString();
                    QuantityOfSessionsLabel[i].Tag = "dispose";
                    QuantityOfSessionsLabel[i].Text = row["QuantityOfSessions"].ToString();


                    //adding all new labels to controls
                    this.Controls.Add(DateLabel[i]);
                    this.Controls.Add(TimeOfAllSessionsLabel[i]);
                    this.Controls.Add(QuantityOfSessionsLabel[i]);

                }


            }
            catch (Exception err)
            {
                MessageBox.Show("Error during generating history list from SMuserDB: " + err.Message);
            }
        }

        #endregion

        #region TitleLabels_Events

        //
        //In this region there are events for click on title labels, after click we are changing sorting and order
        //

        //
        // TimeOfAllSession Click Event
        //
        void TimeOfAllSession_Click(object sender, EventArgs e)   //TimeOfAllSession Click Event
        {
            //
            //clearing of all old labels with TAG dispose to draw new ones on click
            //
            try
            {
                List<Control> itemsToRemove = new List<Control>();
                foreach (Control ctrl in Controls)
                {
                    if (ctrl.Tag != null && ctrl.Tag.ToString() == "dispose")
                    {
                        itemsToRemove.Add(ctrl);
                    }
                }

                foreach (Control ctrl in itemsToRemove)
                {
                    Controls.Remove(ctrl);
                    ctrl.Dispose();
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("Error during clearing old labels on HistorySCR: " + err.Message);
            }

            //
            //----Creating dynamically new labels from records from SMuserDB
            //
            try
            {
                System.Data.DataSet QueryResult;

                //at first we create label arrays for each record in SMuserDB 
                Label[] DateLabel = new Label[30];
                Label[] TimeOfAllSessionsLabel = new Label[30];
                Label[] QuantityOfSessionsLabel = new Label[30];

                //Take all records from SessionsTable order descending by Date
                if (OrderASC == true)
                {
                    SMuserDB_Connection.Sql_Query = "SELECT * FROM SessionsTable ORDER BY TimeOfAllSessions ASC";
                    OrderASC = false;
                }
                else
                {
                    SMuserDB_Connection.Sql_Query = "SELECT * FROM SessionsTable ORDER BY TimeOfAllSessions DESC";
                    OrderASC = true;
                }
                

                QueryResult = SMuserDB_Connection.GetConnection;   //Assign all this records to QueryResult
                int TimeOfSessionsInt = 0;   //Var used for showing Time of sessions as Hours and Minutes

                //In for loop we are assigning each record from QueryResult to coresponding labels
                for (int i = 0; i < QueryResult.Tables[0].Rows.Count; i++)
                {
                    System.Data.DataRow row = QueryResult.Tables[0].Rows[i];

                    TimeOfSessionsInt = Int32.Parse(row["TimeOfAllSessions"].ToString());
                    int hours = 0;
                    int minutes = 0;

                    DateLabel[i] = new Label();
                    DateLabel[i].Location = new System.Drawing.Point(10, 60 + i * 20);
                    DateLabel[i].Name = "DateLabel" + i.ToString();
                    DateLabel[i].Tag = "dispose";
                    DateLabel[i].Text = row["Date"].ToString();

                    TimeOfAllSessionsLabel[i] = new Label();
                    TimeOfAllSessionsLabel[i].Location = new System.Drawing.Point(120, 60 + i * 20);
                    TimeOfAllSessionsLabel[i].Name = "TimeOfAllSessionsLabel" + i.ToString();
                    TimeOfAllSessionsLabel[i].Tag = "dispose";
                    if (TimeOfSessionsInt < 60)
                    {
                        TimeOfAllSessionsLabel[i].Text = TimeOfSessionsInt.ToString() + " " + res_man.GetString("Minutes", language);
                    }
                    else
                    {
                        hours = TimeOfSessionsInt / 60;
                        minutes = TimeOfSessionsInt - (hours * 60);
                        TimeOfAllSessionsLabel[i].Text = hours.ToString() + " " + res_man.GetString("Hours", language) + " " + minutes.ToString() + " " + res_man.GetString("Minutes", language);
                    }

                    QuantityOfSessionsLabel[i] = new Label();
                    QuantityOfSessionsLabel[i].Location = new System.Drawing.Point(250, 60 + i * 20);
                    QuantityOfSessionsLabel[i].Name = "QuantityOfSessionsLabel" + i.ToString();
                    QuantityOfSessionsLabel[i].Tag = "dispose";
                    QuantityOfSessionsLabel[i].Text = row["QuantityOfSessions"].ToString();


                    //adding all new labels to controls
                    this.Controls.Add(DateLabel[i]);
                    this.Controls.Add(TimeOfAllSessionsLabel[i]);
                    this.Controls.Add(QuantityOfSessionsLabel[i]);

                }


            }
            catch (Exception err)
            {
                MessageBox.Show("Error during generating history list from SMuserDB: " + err.Message);
            }
        }

        //
        // QuantityOfSessionsTitle Click Event
        //
        void QuantityOfSessionsTitle_Click(object sender, EventArgs e)   //QuantityOfSessionsTitle Click Event
        {
            //
            //clearing of all old labels with TAG dispose to draw new ones on click
            //
            try
            {
                List<Control> itemsToRemove = new List<Control>();
                foreach (Control ctrl in Controls)
                {
                    if (ctrl.Tag != null && ctrl.Tag.ToString() == "dispose")
                    {
                        itemsToRemove.Add(ctrl);
                    }
                }

                foreach (Control ctrl in itemsToRemove)
                {
                    Controls.Remove(ctrl);
                    ctrl.Dispose();
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("Error during clearing old labels on HistorySCR: " + err.Message);
            }

            //
            //----Creating dynamically new labels from records from SMuserDB
            //
            try
            {
                System.Data.DataSet QueryResult;

                //at first we create label arrays for each record in SMuserDB 
                Label[] DateLabel = new Label[30];
                Label[] TimeOfAllSessionsLabel = new Label[30];
                Label[] QuantityOfSessionsLabel = new Label[30];

                //Take all records from SessionsTable order descending by Date
                if (OrderASC == true)
                {
                    SMuserDB_Connection.Sql_Query = "SELECT * FROM SessionsTable ORDER BY QuantityOfSessions ASC";
                    OrderASC = false;
                }
                else
                {
                    SMuserDB_Connection.Sql_Query = "SELECT * FROM SessionsTable ORDER BY QuantityOfSessions DESC";
                    OrderASC = true;
                }


                QueryResult = SMuserDB_Connection.GetConnection;   //Assign all this records to QueryResult
                int TimeOfSessionsInt = 0;   //Var used for showing Time of sessions as Hours and Minutes

                //In for loop we are assigning each record from QueryResult to coresponding labels
                for (int i = 0; i < QueryResult.Tables[0].Rows.Count; i++)
                {
                    System.Data.DataRow row = QueryResult.Tables[0].Rows[i];

                    TimeOfSessionsInt = Int32.Parse(row["TimeOfAllSessions"].ToString());
                    int hours = 0;
                    int minutes = 0;

                    DateLabel[i] = new Label();
                    DateLabel[i].Location = new System.Drawing.Point(10, 60 + i * 20);
                    DateLabel[i].Name = "DateLabel" + i.ToString();
                    DateLabel[i].Tag = "dispose";
                    DateLabel[i].Text = row["Date"].ToString();

                    TimeOfAllSessionsLabel[i] = new Label();
                    TimeOfAllSessionsLabel[i].Location = new System.Drawing.Point(120, 60 + i * 20);
                    TimeOfAllSessionsLabel[i].Name = "TimeOfAllSessionsLabel" + i.ToString();
                    TimeOfAllSessionsLabel[i].Tag = "dispose";
                    if (TimeOfSessionsInt < 60)
                    {
                        TimeOfAllSessionsLabel[i].Text = TimeOfSessionsInt.ToString() + " " + res_man.GetString("Minutes", language);
                    }
                    else
                    {
                        hours = TimeOfSessionsInt / 60;
                        minutes = TimeOfSessionsInt - (hours * 60);
                        TimeOfAllSessionsLabel[i].Text = hours.ToString() + " " + res_man.GetString("Hours", language) + " " + minutes.ToString() + " " + res_man.GetString("Minutes", language);
                    }

                    QuantityOfSessionsLabel[i] = new Label();
                    QuantityOfSessionsLabel[i].Location = new System.Drawing.Point(250, 60 + i * 20);
                    QuantityOfSessionsLabel[i].Name = "QuantityOfSessionsLabel" + i.ToString();
                    QuantityOfSessionsLabel[i].Tag = "dispose";
                    QuantityOfSessionsLabel[i].Text = row["QuantityOfSessions"].ToString();


                    //adding all new labels to controls
                    this.Controls.Add(DateLabel[i]);
                    this.Controls.Add(TimeOfAllSessionsLabel[i]);
                    this.Controls.Add(QuantityOfSessionsLabel[i]);

                }


            }
            catch (Exception err)
            {
                MessageBox.Show("Error during generating history list from SMuserDB: " + err.Message);
            }
        }

        //
        // DateTitle Click Event
        //
        void DateTitle_Click(object sender, EventArgs e)   //DateTitle Click Event
        {
            //
            //clearing of all old labels with TAG dispose to draw new ones on click
            //
            try
            {
                List<Control> itemsToRemove = new List<Control>();
                foreach (Control ctrl in Controls)
                {
                    if (ctrl.Tag != null && ctrl.Tag.ToString() == "dispose")
                    {
                        itemsToRemove.Add(ctrl);
                    }
                }

                foreach (Control ctrl in itemsToRemove)
                {
                    Controls.Remove(ctrl);
                    ctrl.Dispose();
                }
            }
            catch (Exception err)
            {
                MessageBox.Show("Error during clearing old labels on HistorySCR: " + err.Message);
            }

            //
            //----Creating dynamically new labels from records from SMuserDB
            //
            try
            {
                System.Data.DataSet QueryResult;

                //at first we create label arrays for each record in SMuserDB 
                Label[] DateLabel = new Label[30];
                Label[] TimeOfAllSessionsLabel = new Label[30];
                Label[] QuantityOfSessionsLabel = new Label[30];

                //Take all records from SessionsTable order descending by Date
                if (OrderASC == true)
                {
                    SMuserDB_Connection.Sql_Query = "SELECT * FROM SessionsTable ORDER BY Date ASC";
                    OrderASC = false;
                }
                else
                {
                    SMuserDB_Connection.Sql_Query = "SELECT * FROM SessionsTable ORDER BY Date DESC";
                    OrderASC = true;
                }


                QueryResult = SMuserDB_Connection.GetConnection;   //Assign all this records to QueryResult
                int TimeOfSessionsInt = 0;   //Var used for showing Time of sessions as Hours and Minutes

                //In for loop we are assigning each record from QueryResult to coresponding labels
                for (int i = 0; i < QueryResult.Tables[0].Rows.Count; i++)
                {
                    System.Data.DataRow row = QueryResult.Tables[0].Rows[i];

                    TimeOfSessionsInt = Int32.Parse(row["TimeOfAllSessions"].ToString());
                    int hours = 0;
                    int minutes = 0;

                    DateLabel[i] = new Label();
                    DateLabel[i].Location = new System.Drawing.Point(10, 60 + i * 20);
                    DateLabel[i].Name = "DateLabel" + i.ToString();
                    DateLabel[i].Tag = "dispose";
                    DateLabel[i].Text = row["Date"].ToString();

                    TimeOfAllSessionsLabel[i] = new Label();
                    TimeOfAllSessionsLabel[i].Location = new System.Drawing.Point(120, 60 + i * 20);
                    TimeOfAllSessionsLabel[i].Name = "TimeOfAllSessionsLabel" + i.ToString();
                    TimeOfAllSessionsLabel[i].Tag = "dispose";
                    if (TimeOfSessionsInt < 60)
                    {
                        TimeOfAllSessionsLabel[i].Text = TimeOfSessionsInt.ToString() + " " + res_man.GetString("Minutes", language);
                    }
                    else
                    {
                        hours = TimeOfSessionsInt / 60;
                        minutes = TimeOfSessionsInt - (hours * 60);
                        TimeOfAllSessionsLabel[i].Text = hours.ToString() + " " + res_man.GetString("Hours", language) + " " + minutes.ToString() + " " + res_man.GetString("Minutes", language);
                    }

                    QuantityOfSessionsLabel[i] = new Label();
                    QuantityOfSessionsLabel[i].Location = new System.Drawing.Point(250, 60 + i * 20);
                    QuantityOfSessionsLabel[i].Name = "QuantityOfSessionsLabel" + i.ToString();
                    QuantityOfSessionsLabel[i].Tag = "dispose";
                    QuantityOfSessionsLabel[i].Text = row["QuantityOfSessions"].ToString();


                    //adding all new labels to controls
                    this.Controls.Add(DateLabel[i]);
                    this.Controls.Add(TimeOfAllSessionsLabel[i]);
                    this.Controls.Add(QuantityOfSessionsLabel[i]);

                }


            }
            catch (Exception err)
            {
                MessageBox.Show("Error during generating history list from SMuserDB: " + err.Message);
            }
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
                rect.Size = new Size(498, 498);                  // specify rectangle size here (size of form -2, -2)

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
