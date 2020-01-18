using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using SQLite;



namespace ProjTaskReminder.Data
{
    public class DBTaskReminder
    {
        
        public SQLiteConnection DB { get; set; }


        private string DB_PATH;
        private string DB_NAME;


        public DBTaskReminder(string dbName)
        {
            DB_NAME = dbName;       // "TaskReminderDB"

            Connect();
        }

        private void Connect()
        {
            SQLiteConnection db = null;


            DB_PATH = "/storage/emulated/0/Android/data/com.meirhemed.projtaskreminder/files";         //System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);  //Environment.SpecialFolder.LocalApplicationData

            string fullPath = Path.Combine(DB_PATH, DB_NAME);      //"/Assets/" + DB_NAME;            

            try
            {
                //System.IO.DirectoryInfo info =  System.IO.Directory.CreateDirectory(DB_PATH);

                Context context = Android.App.Application.Context;
                var filePath = context.GetExternalFilesDir("");

                // /storage/emulated/0/Android/data/com.meirhemed.projtaskreminder/files

                Uri originalDbFileUri;  // = new Uri("ms-appx:///Assets/"+DB_NAME);
                Uri.TryCreate("ms-appx:///Assets/" + DB_NAME, UriKind.Absolute, out originalDbFileUri);

                db = new SQLiteConnection(fullPath);

                this.DB = db;

                db.CreateTable<TBL_Tasks>();

             }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            
        }

        private void RecordInser(SQLiteConnection db)
        {
            if (db.Table<TBL_Tasks>().Count() == 0)
            {
                // only insert the data if it doesn't already exist
                var item = new TBL_Tasks();
                item.Title = "Apple";
                item.Description = "Apple Indsroy";
                db.Insert(item);

                item = new TBL_Tasks();
                item.Title = "Google";
                item.Description = "Google Indsroy";
                db.Insert(item);
            }

            Console.WriteLine("Reading data");
            
            TableQuery< TBL_Tasks> table = db.Table<TBL_Tasks>();

            var stock = db.Get<TBL_Tasks>(2); // primary key id of 5
            var stockList = db.Table<TBL_Tasks>();

            foreach (TBL_Tasks s in table)
            {
                Console.WriteLine(s.ID + " " + s.Title);
            }
        }



        [Table("TBL_Tasks")]
        public class TBL_Tasks
        {
            [PrimaryKey, AutoIncrement, Column("_ID")]
            public int ID { get; set; }
            [MaxLength(8)]
            public string Title { get; set; }
            public string Description { get; set; }
            public string DateDue { get; set; }
        }
    }

    
}