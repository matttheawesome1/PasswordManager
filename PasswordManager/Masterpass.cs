using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

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
            string path = "Storage\\manager\\debug\\names\\";
            string dir = Directory.GetDirectoryRoot(path);

            if(PassBox.Text != "" && UserBox.Text != "")
            {
                string hashed = Hashing.HashPassword(PassBox.Text);
                string user = UserBox.Text;
                CreateFile(dir, user, hashed);
                string filename = dir + "save.dat";
                Encryption.EncryptFile(filename, Encryption.GenerateFileKey());
            }
        }

        private void CreateFile(string directory, string user, string hashpass)
        {
            string filename = directory + "save.dat";
            if(!File.Exists(filename))
            {
                using (FileStream fs = File.Create(filename))
                {
                    using (StreamWriter sw = File.AppendText(filename))
                    {
                        string writeMe = user + "," + hashpass;
                        sw.WriteLine(user, hashpass);
                        sw.Flush();
                        sw.Close();
                    }
                }
            }

            else
            {
                using (StreamWriter sw = File.AppendText(filename))
                {
                    string writeMe = user + "," + hashpass;
                    sw.WriteLine(writeMe);
                    sw.Flush();
                    sw.Close();
                }
            }

        }
    }
}
