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
using SQLite;


namespace ProjTaskReminder.Model
{
    [Table("TBL_Settings")]
    public class TBL_Settings
    {
        //[PrimaryKey, AutoIncrement, Column("ID")]
        //public int ID { get; set; }
        //[MaxLength(8)]
        public string MusicPath { get; set; }
        public string NewsUrl { get; set; }
        public string WeatherUrl { get; set; }
        
    }
}