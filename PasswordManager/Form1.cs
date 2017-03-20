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
using System.Data.SqlClient;

namespace PasswordManager
{

    public partial class Form1 : Form
    {
        private string query;
        private string strProvider = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\\Users\\Matthew.Zelenka\\Source\\Repos\\PassMan\\PasswordManager\\manager.accdb";
        OleDbConnection sqlConn;
        OleDbCommand cmd;
        OleDbCommandBuilder cmdBuilder;
        OleDbDataAdapter sda;
        DataTable dt;

        public Form1()
        {
            InitializeComponent();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            query = "SELECT * FROM accountManager";
            sqlConn = new OleDbConnection(strProvider);
            cmd = new OleDbCommand(query, sqlConn);
            cmdBuilder = new OleDbCommandBuilder(sda);
            sqlConn.Open();
            cmd.CommandType = CommandType.Text;
            sda = new OleDbDataAdapter(cmd);
            dt = new DataTable();
            sda.Fill(dt);
            dataGridView1.DataSource = dt;
            sqlConn.Close();
        }

        private void update()
        {
            query = "SELECT * FROM accountManager";
            cmd = new OleDbCommand(query, sqlConn);
            cmdBuilder = new OleDbCommandBuilder(sda);
            sqlConn.Open();
            cmd.CommandType = CommandType.Text;
            sda = new OleDbDataAdapter(cmd);
            dt = new DataTable();
            sda.Fill(dt);
            dataGridView1.DataSource = dt;
            sqlConn.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if(AccntBox.Text != "" && PassBox.Text != "")
            {
                sqlConn.Open();
                if(sqlConn.State == ConnectionState.Open)
                {
                    try
                    {
                        query = "INSERT INTO accountManager (acct, pass) VALUES(@acct, @pass)";

                        cmd.CommandText = query;

                        cmd.Parameters.Add("@acct", OleDbType.VarChar).Value = AccntBox.Text;
                        cmd.Parameters.Add("@pass", OleDbType.VarChar).Value = PassBox.Text;

                        sda.InsertCommand = cmd;

                        dt = new DataTable();

                        sda.Fill(dt);

                        sda.Update(dt);
                        sqlConn.Close();
                        update();

                        MessageBox.Show("Data added successfully");
                        
                    }

                    catch (OleDbException ex)
                    {
                        MessageBox.Show("Error at: " + ex.ToString());
                        sqlConn.Close();
                    }

                }

                else
                {
                    MessageBox.Show("Connection failed.");
                }
            }
        }
    }
}