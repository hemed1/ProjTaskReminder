using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.IO;


namespace ProjTaskReminder.Model
{



    public class Weather //: Serializable
    {
        private string city;
        private string country;
        private string last_update;
        private string temperature;
        private string description;
        private string poster;
        private string wind_kph;
        private string local_time;
        private string cloud;
        private string region;
        private string is_day;
        private string tz_id;
        private ImageView imageView;



        public string getTz_id()
        {
            return tz_id;
        }

        public void setTz_id(string tz_id)
        {
            this.tz_id = tz_id;
        }

        public string getLocal_time()
        {
            return local_time;
        }

        public void setLocal_time(string local_time)
        {
            this.local_time = local_time;
        }

        public string getCloud()
        {
            return cloud;
        }

        public void setCloud(string cloud)
        {
            this.cloud = cloud;
        }

        public string getRegion()
        {
            return region;
        }

        public void setRegion(string region)
        {
            this.region = region;
        }

        public string getIs_day()
        {
            string result = is_day.Trim();

            if (is_day.Equals("0"))
            {
                result = "לא";
            }
            if (is_day.Equals("1"))
            {
                result = "כן";
            }
            return result;
        }

        public void setIs_day(string is_day)
        {
            this.is_day = is_day;
        }

        public ImageView getImageView()
        {
            return imageView;
        }

        public void setImageView(ImageView imageView)
        {
            this.imageView = imageView;
        }

        public string getCity()
        {
            return city;
        }

        public void setCity(string city)
        {
            this.city = city;
        }

        public string getCountry()
        {
            return country;
        }

        public void setCountry(string country)
        {
            this.country = country;
        }

        public string getLast_update()
        {
            return last_update;
        }

        public void setLast_update(string last_update)
        {
            this.last_update = last_update;
        }

        public string getTemperature()
        {
            return temperature;
        }

        public void setTemperature(string temperature)
        {
            this.temperature = temperature;
        }

        public string getDescription()
        {
            return description;
        }

        public void setDescription(string description)
        {
            this.description = description;
        }

        public string getPoster()
        {
            return poster;
        }

        public void setPoster(string poster)
        {
            this.poster = poster;
        }

        public string getWind_kph()
        {
            return wind_kph;
        }

        public void setWind_kph(string wind_kph)
        {
            this.wind_kph = wind_kph;
        }
    }

}
