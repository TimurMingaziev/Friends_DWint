using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Windows.Forms;

namespace Friends_DWint
{
   public class User
    {
        public int uid { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        private string country;
        private string city;
        private string sex;

        public string Sex
        {
            get
            {
                return sex;
            }
            set
            {
                if (value == "1")
                    sex = "женский";
                else if (value == "2")
                    sex = "мужской";
            }
        }

        public string Country
        {
            get
            {
                return country;
            }
            set
            {

                string jfield2 = new ClassQueries().loadPage(String.Format("https://api.vk.com/method/database.getCountriesById?country_ids={0}&v=5.52&lang=0", value));
                JToken jtoken = JToken.Parse(jfield2);
                List<Country> lcountry = jtoken["response"].Children().Select(c => c.ToObject<Country>()).ToList();
                try
                {
                    if (lcountry.Count!=0)
                        country = lcountry.First().title;
                }
                catch{ }
            }
        }

        public string City
        {
            get
            {
                return city;
            }
            set
            {

                string jfield2 = new ClassQueries().loadPage(String.Format("https://api.vk.com/method/database.getCitiesById?city_ids={0}&v=5.52&lang=0", value));
                JToken jtoken = JToken.Parse(jfield2);
                List<City> lcity = jtoken["response"].Children().Select(c => c.ToObject<City>()).ToList();         
                try
                {
                    if (lcity.Count != 0)
                        city = lcity.First().title;
                }
                catch{ }
            }
        }

        public int hidden { get; set; }
        //public string photo_200_orig { get; set; }
        //public string online { get; set; }
    }
}
