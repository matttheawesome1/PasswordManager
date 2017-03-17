using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Data.OleDb;

namespace PasswordManager
{

     public partial class Form1 : Form
    {
        //Data stuff
        private OleDbDataAdapter memDataAdap;
        private DataSet memDS;
        private OleDbCommandBuilder comBuild;
        private OleDbCommand dbCom;
        private OleDbConnection dbConn;

        //Private members.
        private string sConnection;
        private string sql;
        
        public Form1()
        {
            InitializeComponent();
        }

        private void FileUpdate()
        {
            try
            {
                comBuild = new OleDbCommandBuilder(memDataAdap);
                memDataAdap.Update(memDS, "accountManager");
            }

            catch(Exception exc)
            {
                MessageBox.Show(exc.ToString());
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'managerDataSet.accountManager' table. You can move, or remove it, as needed.
            this.accountManagerTableAdapter.Fill(this.managerDataSet.accountManager);

            try
            {
                /*
                 * Construct an object of the type of the OleDbConnection class
                 * to store the connection string representing the type of
                 * data provider (database) and the source (actual db)
                 */
                sConnection = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=manager.accdb";
                dbConn = new OleDbConnection(sConnection);
                dbConn.Open();

                //Construct an object of the OleDbCommand class to hold the sql query.
                sql = "SELECT * FROM accountManager";
                dbCom = new OleDbCommand();

                //Tie the OleDbCommand object to the OleDbConnection object.
                dbCom.CommandText = sql;
                dbCom.Connection = dbConn;
                memDataAdap = new OleDbDataAdapter();
                memDataAdap.SelectCommand = dbCom;
                memDS = new DataSet();
                memDataAdap.Fill(memDS, "accountManager");

                FileUpdate();
            }

            catch(Exception exc)
            {
                MessageBox.Show(exc.ToString());
            }

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}
