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
    [Table("TBL_Tasks")]
    public class TBL_Tasks
    {
        [PrimaryKey, AutoIncrement, Column("ID")]
        public int ID { get; set; }
        [MaxLength(8)]
        public string Title { get; set; }
        public string Description { get; set; }
        public string DateDue { get; set; }
        //public string DateLastUpdate { get; set; }
    }

}