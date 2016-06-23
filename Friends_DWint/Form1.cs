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
using VkNet;

namespace Friends_DWint
{
    public partial class Form1 : Form
    {
        List<int> listFriend = new List<int>();
        List<User> listInfoFriend = new List<User>();

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

        public void checkFiltr(int id)
        {
            string jfield2 = new ClassQueries().loadPage(String.Format("https://api.vk.com/method/users.get?user_id={0}&fields=country,city,sex&lang=0", id));
            JToken jtoken2 = JToken.Parse(jfield2);
            List<User> listUser = jtoken2["response"].Children().
                Select(c => c.ToObject<User>()).ToList();
            try { 
                listInfoFriend.Add(filter(listUser).First());
            }
            catch { }
        }


        public List<User> filter(List<User> listUser)
        {
            List<User> newUserListCountry = new List<User>();
            List<User> newUserListSex = new List<User>();
            List<User> newUserListCity = new List<User>();
            List<User> filteredList = new List<User>();

            if ((countryTextBox.Text == "") && (cityTextBox.Text == "") && (sexComboBox.Text == ""))
                return null;
            if ((countryTextBox.Text != "") && (cityTextBox.Text != "") && (sexComboBox.Text != "")){
                foreach (User user in listUser)
                {
                    if ((user.Country != null) && (user.Country == countryTextBox.Text) &&
                             (user.City != null) && (user.City == cityTextBox.Text) &&
                                 (user.Sex != null) && (user.Sex == sexComboBox.Text))
                        filteredList.Add(user);
                }
                return filteredList;
            }
            if (sexComboBox.Text != "")
            {
                foreach (User user in listUser)
                {
                    if ((user.Sex != null) && (user.Sex == sexComboBox.Text))
                        newUserListSex.Add(user);
                }
                filteredList = newUserListSex;
            }

            if (countryTextBox.Text != "")
            {
                foreach (User user in newUserListSex)
                {
                    if ((user.Country != null) && (user.Country == countryTextBox.Text))
                        newUserListCountry.Add(user);
                }
                filteredList = newUserListCountry;
                if(cityTextBox.Text!="")
                    foreach (User user in newUserListCountry)
                    {
                        if ((user.City != null) && (user.City == cityTextBox.Text))
                            newUserListCity.Add(user);
                    }
                filteredList = newUserListCity;
                
            }
            return filteredList;
        }


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
                                        checkFiltr(48327052);
                                        string fnameForWrite = saveTextBox.Text + "\\" + "friends " + getNameUser(line) + ".txt";
                                        strwr = new StreamWriter(fnameForWrite);
                                        getFriends(line);
                                        if (listFriend.Count != 0)
                                            foreach (int ids in listFriend)
                                                //сюда user.get
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
            //if (DateTime.Today > Convert.ToDateTime("19.06.2016 0:00:00"))
            //    Application.Exit();
            //else MessageBox.Show("Пробная версия до 18.06.2016 (включительно)");
        }


    }
}
