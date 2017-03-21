using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System_Monitor
{
    class DatabaseConnection
    {
        //
        //----This class is used for connecting to database and find data sets from tables
        //

        private string sql_string; //this string will hold SQL query string
        private string strCon;   //this string will hold location of the database
        System.Data.SqlClient.SqlDataAdapter da_1;

        public string Sql_Query    //fist property of class DatabaseConnection 
        {
            set { sql_string = value; }
        }

        public string connection_string   //second property of class DatabaseConnection
        {
            set { strCon = value; }
        }

        public System.Data.DataSet GetConnection   //third property of class DatabaseConnection, this property is read only
        {
            get { return MyDataSet(); }
        }

        private System.Data.DataSet MyDataSet()   //this will hold data set from DB
        {
            System.Data.SqlClient.SqlConnection con = new System.Data.SqlClient.SqlConnection(strCon);   //define a new sql connection using strCon

            con.Open();

            da_1 = new System.Data.SqlClient.SqlDataAdapter(sql_string, con);   //new data adapter 

            System.Data.DataSet dat_set = new System.Data.DataSet();
            da_1.Fill(dat_set, "Table_Data_1");    //fill data adapter to data set 
            con.Close();

            return dat_set;
        }

        public void UpdateDatabase(System.Data.DataSet ds)   //method for updating database
        {
            System.Data.SqlClient.SqlCommandBuilder cb = new System.Data.SqlClient.SqlCommandBuilder(da_1);

            cb.DataAdapter.Update(ds.Tables[0]);
        }
    }
}
