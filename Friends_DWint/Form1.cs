﻿using System;
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
        List<Country> listCoutries;
        List<City> listCities;
        int idCountry = 0;
        int idCity = 0;

        public Form1()
        {
            InitializeComponent();
        }

        // Имя для создания файла
        public string getNameUser(string id)
        {
            string name = "";
            try
            {

                string jfield = new ClassQueries().loadPage(String.Format("https://api.vk.com/method/users.get?user_ids={0}", id));
                JToken jtoken = JToken.Parse(jfield);
                List<User> listUser = jtoken["response"].Children().Select(c => c.ToObject<User>()).ToList();
                foreach (User user in listUser)
                {
                    name = user.first_name + " " + user.last_name;
                }
            }
            catch (Exception ex)
            {
                textBox1.AppendText("Error by id: " + id + " ^" + ex.Message + "^ " + Environment.NewLine);
            }
            return name;
        }

        // Весь список друзей, который сохраняется в listFriend
        public int getFriends(string id)
        {
            try
            {
                string jfield = new ClassQueries().loadPage(String.Format("https://api.vk.com/method/friends.get?user_id={0}&order=random", id));
                JToken jtoken = JToken.Parse(jfield);

                listFriend = jtoken["response"].Children().Select(c => c.ToObject<string>()).ToList();
                return 1;
            }
            catch (Exception ex)
            {
                textBox1.AppendText("Error by id: " + id + " ^" + ex.Message + "^ " + Environment.NewLine);
                return 0;
            }
            
        }

        // Загрузка информации о друзьях и фильтрация
        public void checkFiltr(List<string> ids)
        {
            string jfield2 = "";
            JToken jtoken2 = null;
            try
            {
                if (ids.Count != 0 && ids.Count > 1)
                    jfield2 = new ClassQueries().loadPagePOST("https://api.vk.com/method/users.get", "user_ids="+ responseString(ids) +"&fields=country,city,sex&lang=0", ids.Count);
                else
                    if (ids.Count == 1)
                        jfield2 = new ClassQueries().loadPage(String.Format("https://api.vk.com/method/users.get?user_ids={0}&fields=country,city,sex&lang=0", ids.First()));
                    else return;

                if (jfield2 != "")
                    try
                    {
                        jtoken2 = JToken.Parse(jfield2);
                    }
                    catch (Exception ex) { textBox1.AppendText("Error: " + "ошибка загрузки страницы " + " ^" + ex.Message + "^ " + Environment.NewLine); }

                else
                {
                    textBox1.AppendText("Error: " + "ошибка загрузки страницы" + Environment.NewLine);
                    return;
                }
            }
            catch (Exception ex)
            {
                textBox1.AppendText("Error :"+ " ^" + ex.Message + "^ " + Environment.NewLine);
            }

            listInfoFriend.Clear();

            if ((countryComboBox.Text == "") && (cityComboBox.Text == "") && (sexComboBox.Text == ""))
                listInfoFriend = jtoken2["response"].Children().
                Select(c => c.ToObject<User>()).ToList();
            else
            {
                if (countryComboBox.Text != "")
                {
                    listInfoFriend = jtoken2["response"].Children().
                    Select(c => c.ToObject<User>()).
                    Where(c => c.country == idCountry && countryComboBox.Text != "").ToList();
                }

                if (listInfoFriend.Count != 0 && cityComboBox.Text != "")
                    listInfoFriend = listInfoFriend.Where(c => c.city == idCity && cityComboBox.Text != "").ToList();
                else
                    if (cityComboBox.Text != "")
                        listInfoFriend = jtoken2["response"].Children().
                    Select(c => c.ToObject<User>()).
                    Where(c => c.city == idCity && cityComboBox.Text != "").ToList();


                if (listInfoFriend.Count != 0 && sexComboBox.Text != "")
                   listInfoFriend = listInfoFriend.Where(c => c.Sex == sexComboBox.Text && sexComboBox.Text != "").ToList();
                else
                    if (sexComboBox.Text != "")
                        listInfoFriend = jtoken2["response"].Children().
                    Select(c => c.ToObject<User>()).
                    Where(c => c.Sex == sexComboBox.Text && sexComboBox.Text != "").ToList();

            }
        }

        // Конвертация списка ids в строку
        public string responseString(List<string> ids)
        {
            
            string uids = ids[0];
            for (int i = 1; i < ids.Count; i++)
            {
                uids += "," + ids.ElementAt(i);
            }
            return uids;
        }

        // Проверка на не заполненные поля перед стартом
        public bool checkStart() {
            if (DateTime.Today > Convert.ToDateTime("30.06.2016 0:00:00"))
            {
                MessageBox.Show("Пробная версия закончилась");
                return false;
            }
            if (idCountry == 0 && countryComboBox.Text != "")
            {
                MessageBox.Show("Страна введена не верно");
                return false;
            }

            if (idCity == 0 && cityComboBox.Text != "")
            {
                MessageBox.Show("Город введен не верно");
                return false;
            }
            if (listFilesListBox.Items.Count == 0 && saveTextBox.Text == "")
            {
                MessageBox.Show("Выберите место сохранения/загрузки файла");
                return false;
            }
            return true;
        }

        // Установка progressBar-у максимальное значение
        public void progressBarStartPosition(String file)
        {
            textBox1.AppendText("Идет загрузка... " + Environment.NewLine);
            StreamReader st = new StreamReader(file, System.Text.Encoding.Default);
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
        }

        // На сколько нужно делить запрос
        public int countResponse(int count)
        {
            int k = 0;
            while (count >= 700)
            {
                if (count % 700 == 0)
                    k++;
                count--;
            }
            return k + 1;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (checkStart())
                foreach (String file in openFileDialog1.FileNames)
                {
                    List<string> idsForFilter = new List<string>(); //test
                    FileInfo fileInfo = new FileInfo(file);
                    StreamWriter strwr = null;
                    StreamReader st = null;
                    if (fileInfo.Exists)
                        if (fileInfo.Length > 0)
                            try
                            {
                                progressBarStartPosition(file);
                                string line;
                                st = new StreamReader(file, System.Text.Encoding.Default);
                                progressBar1.Value += 1;
                                int countFile = 0;
                                while ((line = st.ReadLine()) != null)
                                {
                                    countFile++;
                                    try
                                    {
                                        if (getNameUser(line) == "")
                                            continue;

                                        string fnameForWrite = saveTextBox.Text + "\\" + countFile + "_friends " + getNameUser(line) + "_" + countryComboBox.Text + "_" + cityComboBox.Text + "_" + sexComboBox.Text + ".txt";
                                      
                                        int isFriends = getFriends(line);
                                        strwr = new StreamWriter(fnameForWrite);
                                        if (isFriends != 0)
                                        {
                                            
                                            if (listFriend.Count != 0)
                                            {
                                                textBox1.AppendText("INFO :" + "Друзей у " + line + " " + listFriend.Count + Environment.NewLine);
                                                int countSplit = 0;
                                                if (listFriend.Count > 700)
                                                    countSplit = countResponse(listFriend.Count);

                                                if (countSplit > 1)
                                                {
                                                    IEnumerable<IEnumerable<string>> lists = LinqExtensions.Split<string>(listFriend, countSplit);
                                                    foreach (IEnumerable<string> list in lists)
                                                    {
                                                        checkFiltr(list.ToList());
                                                        if (listInfoFriend.Count != 0)
                                                            foreach (User user in listInfoFriend)
                                                                strwr.WriteLine(+user.uid);
                                                    }
                                                }
                                                else
                                                {
                                                    checkFiltr(listFriend);//test
                                                    if (listInfoFriend.Count != 0)
                                                        foreach (User user in listInfoFriend)
                                                            strwr.WriteLine(user.uid);
                                                }
                                            }
                                        }
                                        else
                                            textBox1.AppendText("Warning: " + line + " нет друзей (возможна ошибка)" + Environment.NewLine);
                                        strwr.Close();
                                    }
                                    catch (Exception ex) { textBox1.AppendText("Error by id: " + line + " ^" + ex.Message + "^ " + Environment.NewLine); }
                                    finally { strwr.Close(); progressBar1.Value += 1; }

                                }
                                st.Close();
                            }
                            catch (Exception ex) { textBox1.AppendText("Error :" + " ^" + ex.Message + "^ " + Environment.NewLine); }
                            finally { st.Close(); progressBar1.Value = 0; textBox1.AppendText("Загрузка окончена" + Environment.NewLine); }
                }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            if (DateTime.Today > Convert.ToDateTime("30.06.2016 0:00:00"))
                Application.Exit();
            else MessageBox.Show("Пробная версия до 29.06.2016 (включительно)");
            try
            {
                countryComboBox.Items.Clear();
                string jfield = new ClassQueries().loadPage(String.Format("https://api.vk.com/method/database.getCountries?need_all=1&count=1000&lang=0&v=5.52"));
                JToken jtoken = JToken.Parse(jfield);

                listCoutries = jtoken["response"]["items"].Children().Select(c => c.ToObject<Country>()).ToList();
                foreach (Country country in listCoutries)
                {
                    countryComboBox.Items.Add(country.title);
                }
            }
            catch (Exception ex) {MessageBox.Show(ex.Message);}
            
        }

        private void countryComboBox_Leave(object sender, EventArgs e)
        {
            idCountry = 0;
            cityComboBox.Items.Clear();
            string selectedCountry = countryComboBox.Text;
            if (listCoutries.Count != 0)
            {
                foreach (Country country in listCoutries)
                {
                    if (selectedCountry == country.title)
                    {
                        idCountry = country.id;
                        break;
                    }
                }
                if (idCountry != 0)
                    try
                    {
                        string jfield = new ClassQueries().loadPage(String.Format("https://api.vk.com/method/database.getCities?country_id={0}&need_all=0&count=1000&lang=0&v=5.52", idCountry));
                        JToken jtoken = JToken.Parse(jfield);

                        listCities = jtoken["response"]["items"].Children().Select(c => c.ToObject<City>()).ToList();
                        foreach (City city in listCities)
                        {
                            cityComboBox.Items.Add(city.title);
                        }
                    }
                    catch (Exception ex) {MessageBox.Show(ex.Message);}

            }
            else return;
        }

        private void cityComboBox_Leave(object sender, EventArgs e)
        {
           try{ idCity = 0;
            string selectedCity = cityComboBox.Text;
            if (listCities.Count !=0)
            foreach (City city in listCities)
                if (selectedCity == city.title)
                    idCity = city.id;
           }
           catch { }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                saveTextBox.Text = folderBrowserDialog1.SelectedPath;
        }

        private void loadButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                listFilesListBox.Items.Clear();
                foreach (String file in openFileDialog1.FileNames)
                    listFilesListBox.Items.Add(file);
            }

        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            countryComboBox.SelectedIndex = -1;
            cityComboBox.Items.Clear();
            sexComboBox.SelectedIndex = -1;
        }


    }
}
