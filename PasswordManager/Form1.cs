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
using System.Collections;

namespace PasswordManager
{

    public partial class Form1 : Form
    {
        private string query;

        private string strProvider = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=|DataDirectory|\manager.accdb";
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
            List<string> keyColums = new List<string>();
            keyColums.Add("acct");
            keyColums.Add("pass");
            sda.Fill(dt);
            RemoveDuplicatesFromDataTable(dt, keyColums);
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
            List<string> keyColums = new List<string>();
            keyColums.Add("acct");
            keyColums.Add("pass");
            sda.Fill(dt);
            RemoveDuplicatesFromDataTable(dt, keyColums);
            dataGridView1.DataSource = dt;
            sqlConn.Close();
        }

/*
 * Author: Terry Guo
 * Found on: https://forums.asp.net/t/1957806.aspx?How%20to%20remove%20duplicate%20rows%20with%20same%20multiple%20ID%20s%20from%20a%20datatable
 * Function removes all duplicate entries from a data table.
 */
        public static void RemoveDuplicatesFromDataTable(DataTable table, List<string> keyColumns)
        {

            Dictionary<string, string> uniquenessDict = new Dictionary<string, string>(table.Rows.Count);

            StringBuilder stringBuilder = null;

            int rowIndex = 0;

            DataRow row;

            DataRowCollection rows = table.Rows;

            while (rowIndex < rows.Count)
            {

                row = rows[rowIndex];

                stringBuilder = new StringBuilder();

                foreach (string colname in keyColumns)
                {

                    stringBuilder.Append(((string)row[colname]));

                }

                if (uniquenessDict.ContainsKey(stringBuilder.ToString()))
                {

                    rows.Remove(row);

                }

                else
                {

                    uniquenessDict.Add(stringBuilder.ToString(), string.Empty);

                    rowIndex++;

                }

            }

        }

        private void button1_Click(object sender, EventArgs e)
        {

            //If text in both boxes isn't blank.
            if(AccntBox.Text != "" && PassBox.Text != "")
            {
                //Then open sql connection
                sqlConn.Open();

                //If sql connection is successful.
                if(sqlConn.State == ConnectionState.Open)
                {
                    try
                    {
                        //Then add query to insert into table with username and password.
                        query = "INSERT INTO accountManager (acct, pass) VALUES(@acct, @pass)";

                        //Set the command to the query string.
                        cmd.CommandText = query;

                        //Paramaterize the query string to avoid confusion with sql.
                        cmd.Parameters.Add("@acct", OleDbType.VarChar).Value = AccntBox.Text;
                        cmd.Parameters.Add("@pass", OleDbType.VarChar).Value = PassBox.Text;

                        sda.InsertCommand = cmd;

                        dt = new DataTable(); //Create a new datatable to fill.
                        List<string> keyColums = new List<string>();
                        keyColums.Add("acct");
                        keyColums.Add("pass");
                        RemoveDuplicatesFromDataTable(dt, keyColums); //Remove duplicate accounts.

                        sda.Fill(dt); //Fill the datatable with new entry.

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

        private void SearchBox_TextChanged(object sender, EventArgs e)
        {
            if(SearchBox.Text == "Search username")
            {
                SearchBox.Text = "";
            }
        }

        private void KeyWasPressed(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            if(e.KeyChar == (char)13)
            {
                EventArgs ex = new EventArgs();
                Search_Click(sender, ex);
            }
        }

        private void Delete_Click(object sender, EventArgs e)
        {
            
        }

        private void Search_Click(object sender, EventArgs e)
        {
            if (Search.Text != "")
            {
                query = String.Format("SELECT * FROM accountManager WHERE acct='@accounts'", Search.Text);
                cmd.Parameters.Add("@accounts", OleDbType.VarChar).Value = Search.Text;

                cmd.CommandText = query;

                sda.SelectCommand = cmd;

                dt = new DataTable(); //Create a new datatable to fill.
                List<string> keyColums = new List<string>();
                keyColums.Add("acct");
                keyColums.Add("pass");
                RemoveDuplicatesFromDataTable(dt, keyColums); //Remove duplicate accounts.

                sda.Fill(dt); //Fill the datatable with new entry.

                sda.Update(dt); //Update the datatable
                sqlConn.Close(); //Close the connection
                dataGridView1.DataSource = dt;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void encryptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int key; //The amount to shift the letter by.
            if(CaeserBox.Text != "" && int.TryParse(CaeserKeyBox.Text, out key))
            {
                string s = Caeser_Cipher(CaeserBox.Text, key); //Run the caeser cipher and store result in s.
                MessageBox.Show("Your ciphered message is: " + s); //Show the encrypted message
            }

            else
            {
                MessageBox.Show("One of the caeser boxes is empty, please input some data and try again."); //Error catch
            }
        }

        private void decryptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //This decrypts the ciphered text by taking the negative of the key, mod it by 26, and then adds 26 to it.
            //For example -3 % 26 + 26 = 49. This means that it will have a shift to the left of 49, equivalently, a shift right of 23.

            int key; //Amount to shift letter by.
            if (CaeserBox.Text != "" && int.TryParse(CaeserKeyBox.Text, out key))
            {
                key = -key; //Negate the key.
                string s = Caeser_Cipher(CaeserBox.Text, key); //Run the caeser cipher and store it in s
                MessageBox.Show("Your deciphered message is: " + s); //Show the decrypted message.
            }

            else
            {
                MessageBox.Show("One of the caeser boxes is empty, please input some data and try again."); //Error catch
            }
        }

        private string Caeser_Cipher(string ciphertext, int key)
        {
            if(key < 0)
            {
                key = key % 26 + 26; //Set the key between 1 and 26.
            }

            char c; //Place holder character.
            string word = " "; //Initialize word to empty.

            foreach(char symbol in ciphertext)
            {
                if(symbol >= 'A' && symbol <= 'Z')
                {
                    /*
                     * This converts one character at a time with the caeser cipher method.
                     * Mathematically the Caeser Cipher is: F(x) = (x + n) mod 26
                     * Where x is the letter being encrypted and n is the shift of the cipher.
                     */
                    c = Convert.ToChar((Convert.ToInt32(symbol) - Convert.ToInt32('A') + key) % 26 + Convert.ToInt32('A'));
                }

                else if(symbol >= 'a' && symbol <= 'z')
                {
                    c = Convert.ToChar((Convert.ToInt32(symbol) - Convert.ToInt32('a') + key) % 26 + Convert.ToInt32('a'));
                }

                word += symbol;
            }
            return word;
        }
    }
}