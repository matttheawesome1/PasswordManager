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
            if(Search.Text != "")
            {
                query = String.Format("SELECT * FROM accountManager WHERE ( acct LIKE @{0} )", Search.Text);
                cmd.Parameters.Add("@accounts", OleDbType.VarChar).Value = Search.Text;
            }
        }

        private void Search_Click(object sender, EventArgs e)
        {

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