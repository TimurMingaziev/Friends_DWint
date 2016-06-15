using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using System.IO;

namespace Friends_DWint
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                saveTextBox.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void loadButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                listFilesListBox.Items.Clear();
                foreach (String file in openFileDialog1.FileNames)
                {

                    listFilesListBox.Items.Add(file);

                }
            }

        }

        public string getNameUser(string id)
        {
            List<User> listUser = new List<User>();
            string name = "";
            try
            {

                string jfield = new ClassQueries().loadPage(String.Format("https://api.vk.com/method/users.get?user_ids={0}", id));
                JToken jtoken = JToken.Parse(jfield);
                listUser = jtoken["response"].Children().Select(c => c.ToObject<User>()).ToList();
                foreach (User user in listUser)
                {
                    name = user.first_name + " " + user.last_name;
                }
            }
            catch (Exception)
            {
                textBox1.AppendText("Error: " + id + Environment.NewLine);
            }
            return name;
        }

        public void getFriends(string id)
        {
            string jfield = new ClassQueries().loadPage(String.Format("https://api.vk.com/method/friends.get?user_id={0}&order=random", id));
            JToken jtoken = JToken.Parse(jfield);
            listFriend = jtoken["response"].Children().Select(c => c.ToObject<int>()).ToList();
        }


        List<int> listFriend = new List<int>();

        private void button1_Click(object sender, EventArgs e)
        {
            if (listFilesListBox.Items.Count != 0 && saveTextBox.Text != "")
            {
                foreach (String file in openFileDialog1.FileNames)
                {

                    FileInfo fileInfo = new FileInfo(file);
                    StreamWriter strwr = null;
                    StreamReader st = null;
                    if (fileInfo.Exists)
                        if (fileInfo.Length > 0)
                            try
                            {
                                st = new StreamReader(file, System.Text.Encoding.Default);
                                string line;
                                while ((line = st.ReadLine()) != null)
                                {
                                    try
                                    {
                                        if (getNameUser(line) == "")
                                            continue;
                                        string fnameForWrite = saveTextBox.Text + "\\" + "friends " + getNameUser(line) + ".txt";
                                        strwr = new StreamWriter(fnameForWrite);
                                        getFriends(line);
                                        if (listFriend.Count != 0)
                                            foreach (int ids in listFriend)
                                                strwr.WriteLine(ids);

                                        else
                                            textBox1.AppendText("Warning: " + line + " нет друзей (возможна ошибка)" + Environment.NewLine);
                                        strwr.Close();
                                    }
                                    catch (Exception ex) { MessageBox.Show(ex.Message); }
                                    finally { strwr.Close(); }

                                }
                                st.Close();
                            }
                            catch (Exception ex) { MessageBox.Show(ex.Message); }
                            finally { st.Close(); }

                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }


    }
}
