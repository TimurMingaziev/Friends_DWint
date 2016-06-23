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
        List<string> listFriend = new List<string>();
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

            listFriend = jtoken["response"].Children().Select(c => c.ToObject<string>()).ToList();

        }

        public void checkFiltr(List<string> ids)
        {
            string jfield2 = "";
            JToken jtoken2 = null;
            if (ids.Count != 0 && ids.Count > 1)
                jfield2 = new ClassQueries().loadPage(String.Format("https://api.vk.com/method/users.get?user_ids= " + responseString(ids) + " &fields=country,city,sex&lang=0"));
            else
                if (ids.Count == 1)
                    jfield2 = new ClassQueries().loadPage(String.Format("https://api.vk.com/method/users.get?user_ids={0}&fields=country,city,sex&lang=0", ids.First()));
                else return;
            if (jfield2 != "")
                jtoken2 = JToken.Parse(jfield2);
            else
            {
                textBox1.AppendText("Error: " + "ошибка загрузки страницы" + Environment.NewLine);
                return;
            }

            listInfoFriend.Clear();

            if ((countryTextBox.Text == "") && (cityTextBox.Text == "") && (sexComboBox.Text == ""))
                listInfoFriend = jtoken2["response"].Children().
                Select(c => c.ToObject<User>()).ToList();
            else
            {
                if (countryTextBox.Text != "")
                {
                    listInfoFriend = jtoken2["response"].Children().
                    Select(c => c.ToObject<User>()).
                    Where(c => c.Country == countryTextBox.Text && countryTextBox.Text != "").ToList();
                }

                if (listInfoFriend.Count != 0 && cityTextBox.Text != "")
                   listInfoFriend = listInfoFriend.Where(c => c.City == cityTextBox.Text && cityTextBox.Text != "").ToList();
                else
                    if (cityTextBox.Text != "")
                        listInfoFriend = jtoken2["response"].Children().
                    Select(c => c.ToObject<User>()).
                    Where(c => c.City == cityTextBox.Text && cityTextBox.Text != "").ToList();


                if (listInfoFriend.Count != 0 && sexComboBox.Text != "")
                   listInfoFriend = listInfoFriend.Where(c => c.Sex == sexComboBox.Text && sexComboBox.Text != "").ToList();
                else
                    if (sexComboBox.Text != "")
                        listInfoFriend = jtoken2["response"].Children().
                    Select(c => c.ToObject<User>()).
                    Where(c => c.Sex == sexComboBox.Text && sexComboBox.Text != "").ToList();

            }
        }

        public void filterSex(JToken jtoken2)
        {
            listInfoFriend = jtoken2["response"].Children().
                    Select(c => c.ToObject<User>()).
                    Where(c => c.Sex == sexComboBox.Text).ToList();
        }
        public void filterCity(JToken jtoken2) {
            listInfoFriend = jtoken2["response"].Children().
                       Select(c => c.ToObject<User>()).
                       Where(c => c.City == cityTextBox.Text).ToList();
        }
        public void filterCountry(JToken jtoken2) { listInfoFriend = jtoken2["response"].Children().
                Select(c => c.ToObject<User>()).
                Where(c => c.Country == countryTextBox.Text).ToList();}

        public string responseString(List<string> ids)
        {
            
            string uids = ids[0];
            for (int i = 1; i < ids.Count; i++)
            {
                uids += "," + ids.ElementAt(i);
            }
            return uids;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (listFilesListBox.Items.Count != 0 && saveTextBox.Text != "")
            {

                foreach (String file in openFileDialog1.FileNames)
                {
                    List<string> idsForFilter = new List<string>();//test
                    FileInfo fileInfo = new FileInfo(file);
                    StreamWriter strwr = null;
                    StreamReader st = null;
                    if (fileInfo.Exists)
                        if (fileInfo.Length > 0)
                            try
                            {
                                textBox1.AppendText("Идет загрузка... " + Environment.NewLine);
                                st = new StreamReader(file, System.Text.Encoding.Default);
                                string line;
                                int countLine = 0;
                                try
                                {
                                    while ((line = st.ReadLine()) != null)
                                    { countLine++; }
                                    progressBar1.Maximum = countLine+1;
                                }
                                catch (Exception ex) { MessageBox.Show(ex.Message); }
                                finally { st.Close(); }
                                st = new StreamReader(file, System.Text.Encoding.Default);
                                progressBar1.Value = 1;
                                while ((line = st.ReadLine()) != null)
                                {
                                    try
                                    {
                                        if (getNameUser(line) == "")
                                            continue;
                                        
                                        string fnameForWrite = saveTextBox.Text + "\\" + "friends " + getNameUser(line) + "_" +countryTextBox.Text+"_"+cityTextBox.Text+"_"+sexComboBox.Text+".txt";
                                        strwr = new StreamWriter(fnameForWrite);
                                        getFriends(line);
                                        if (listFriend.Count != 0){

                                            checkFiltr(listFriend);//test
                                            if (listInfoFriend.Count!=0)
                                            foreach (User user in listInfoFriend)
                                                strwr.WriteLine(user.uid);
                                            }

                                        else
                                            textBox1.AppendText("Warning: " + line + " нет друзей (возможна ошибка)" + Environment.NewLine);
                                        strwr.Close();
                                    }
                                    catch (Exception ex) { MessageBox.Show(ex.Message); }
                                    finally { strwr.Close(); progressBar1.Value += 1; }

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

        private void countryTextBox_TextChanged(object sender, EventArgs e)
        {

        }


    }
}
