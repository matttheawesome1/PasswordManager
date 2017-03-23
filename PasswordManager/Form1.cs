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

            RemoveDuplicateRows(dt, "@acct");

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
            RemoveDuplicateRows(dt, "acct");
            dataGridView1.DataSource = dt;
            sqlConn.Close();
        }

        /*
         *Author: Username ratty on StackOverflow
         *Found on website: http://stackoverflow.com/questions/4415519/best-way-to-remove-duplicate-entries-from-a-data-table
         *Removes duplicate entries from datatable. 
         */
        public DataTable RemoveDuplicateRows(DataTable dTable, string colName)
        {
            Hashtable hTable = new Hashtable();
            ArrayList duplicateList = new ArrayList();

            //Add list of all the unique item value to hashtable, which stores combination of key, value pair.
            //And add duplicate item value in arraylist.
            foreach (DataRow drow in dTable.Rows)
            {
                if (hTable.Contains(drow[colName]))
                    duplicateList.Add(drow);
                else
                    hTable.Add(drow[colName], string.Empty);
            }

            //Removing a list of duplicate items from datatable.
            foreach (DataRow dRow in duplicateList)
                dTable.Rows.Remove(dRow);

            //Datatable which contains unique records will be return as output.
            return dTable;
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
                        RemoveDuplicateRows(dt, "acct"); //Remove duplicate accounts.

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
            if(Search.Text != "")
            {
                SearchString(SearchBox.Text);
                DeleteFromTable(dt, SearchBox.Text);
            }

            else
            {
                Update();
            }
        }

        private void Search_Click(object sender, EventArgs e)
        {
            if (Search.Text != "")
            {
                SearchString(SearchBox.Text);
            }

            else
            {
                Update();
            }

            MessageBox.Show(SearchBox.Text);
        }

        private void SearchString(string searched)
        {
            sqlConn.Open();
            if (sqlConn.State == ConnectionState.Open)
            {
                try
                {
                    query = "SELECT * FROM accountManager WHERE acct LIKE @accounts";
                    cmd = new OleDbCommand(query, sqlConn);
                    cmdBuilder = new OleDbCommandBuilder(sda);
                    sda = new OleDbDataAdapter(cmd);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add("@accounts", OleDbType.VarChar).Value = searched;

                    cmd.ExecuteNonQuery();

                    sda.SelectCommand = cmd;

                    dt = new DataTable(); //Create a new datatable to fill.
                    RemoveDuplicateRows(dt, "acct"); //Remove duplicate entries.

                    sda.Fill(dt); //Fill the datatable.
                    MessageBox.Show("Search complete.");

                    dataGridView1.DataSource = dt; //Set the datasource to the datatable.

                    sqlConn.Close(); //Close the connection
                }

                catch (OleDbException ex)
                {
                    MessageBox.Show("Error occured at: " + ex.ToString());
                }
            }

            else
            {
                MessageBox.Show("Connection failed.");
            }
        }

        private void DeleteFromTable(DataTable dt, string column)
        {
            sqlConn.Open();
            try
            {
                if(sqlConn.State == ConnectionState.Open)
                {
                    query = "DELETE FROM accountManager WHERE EXISTS(SELECT * FROM accountManager WHERE acct = @account)"; //Delete the entire record.
                    cmd = new OleDbCommand(query, sqlConn);
                    cmdBuilder = new OleDbCommandBuilder(sda);
                    sda = new OleDbDataAdapter(cmd);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add("@account", OleDbType.VarChar).Value = column;

                    cmd.CommandText = query;

                    cmd.ExecuteNonQuery();

                    sda.DeleteCommand = cmd;

                    dt = new DataTable();

                    sda.Fill(dt);
                    MessageBox.Show("Deleted account");

                    dataGridView1.DataSource = dt;

                    sqlConn.Close();
                    Update();
                }

                else
                {
                    MessageBox.Show("Connection failed.");
                }
            }

            catch(OleDbException ex)
            {
                MessageBox.Show("There was an error at: " + ex.ToString());
            }
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

            char tostring = ' '; //Place holder character.
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
                    tostring = Convert.ToChar((Convert.ToInt32(symbol) - Convert.ToInt32('A') + key) % 26 + Convert.ToInt32('A'));
                }

                else if(symbol >= 'a' && symbol <= 'z')
                {
                    tostring = Convert.ToChar((Convert.ToInt32(symbol) - Convert.ToInt32('a') + key) % 26 + Convert.ToInt32('a'));
                }

                word += tostring;
            }
            return word;
        }
    }
}