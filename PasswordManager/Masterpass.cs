using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace PasswordManager
{
    public partial class Masterpass : Form
    {
        public Masterpass()
        {
            InitializeComponent();
        }

        private void CreateButton_Click(object sender, EventArgs e)
        {
            if(PassBox.Text != "" && UserBox.Text != "")
            {
                string hashed = Hashing.HashPassword(PassBox.Text);
                string user = UserBox.Text;
                CreateFile(finalpath, user, hashed);
                string filepath = finalpath + "save.dat";
                Encryption.EncryptFile(filepath, Encryption.GenerateFileKey());
            }
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            if (UserBox.Text != "" && PassBox.Text != "")
            {
                string[] stuff = ReadFile(finalpath);
                string storedUser = stuff[1];
                string storedHash = stuff[2];

                if(UserBox.Text == storedUser && Hashing.ValidatePassword(PassBox.Text, storedHash) == true)
                {
                    Form successful = new Form1();
                }

                else if(UserBox.Text != storedUser || Hashing.ValidatePassword(PassBox.Text, storedHash) == false)
                {
                    MessageBox.Show("Invalid Login, try again with different credentials.");
                    //loginAttempts--;
                    Console.WriteLine("Stored Username is: {0}", storedUser);
                }
            }
        }

        private void CreateFile(string filepath, string user, string hashpass)
        {
            string filename = finalpath + "save.dat";
            Console.WriteLine("Full path: {0}", Path.GetFullPath(filename).ToString());
            if(!File.Exists(filename))
            {
                using (FileStream fs = new FileStream(filename, FileMode.CreateNew))
                {
                    fs.Flush();
                    fs.Close();
                }
                using (StreamWriter sw = File.AppendText(filename))
                {
                    string writeMe = user + "," + hashpass;
                    sw.WriteLine(writeMe);
                    sw.Flush();
                    sw.Close();
                }
            }

            else
            {
                MessageBox.Show("Account has already been created.");
                MessageBox.Show(Path.GetFullPath(filename).ToString());
            }
        }

        private string[] ReadFile(string filepath)
        {
            string[] strings = new string[2];

            using (StreamReader sr = new StreamReader(filepath))
            {
                strings = sr.ReadLine().Split(',');
            }

            return strings;
        }

        //Data Members
        private static string path = Directory.GetCurrentDirectory() + "..\\..\\Storage\\Manager\\Names\\";
        private static DirectoryInfo dir = Directory.CreateDirectory(path);
        private static string finalpath = Path.GetFullPath(path).ToString();
        //private int loginAttempts = 5;
    }
}
