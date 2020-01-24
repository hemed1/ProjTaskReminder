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

        private SQLiteConnection Connect()
        {
            SQLiteConnection db = null;
            string fullPath;


            Context context = Android.App.Application.Context;

            // /storage/emulated/0/Android/data/com.meirhemed.projtaskreminder/files
            DB_PATH = context.GetExternalFilesDir("").AbsolutePath;         //System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);  //"/Assets/" + DB_NAME;             //Environment.SpecialFolder.LocalApplicationData

            fullPath = Path.Combine(DB_PATH, DB_NAME);      

            try
            {
                db = new SQLiteConnection(fullPath);

                this.DB = db;

                //db.DropTable<TBL_Tasks>();
                var r = db.GetTableInfo("TBL_Tasks");

                if (r.Count == 0)
                {
                    var t = db.CreateTable<TBL_Tasks>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            //System.IO.DirectoryInfo info =  System.IO.Directory.CreateDirectory(DB_PATH);
            //Uri originalDbFileUri;  // = new Uri("ms-appx:///Assets/"+DB_NAME);
            //Uri.TryCreate("ms-appx:///Assets/" + DB_NAME, UriKind.Absolute, out originalDbFileUri);

            return db;
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
            
            TableQuery<TBL_Tasks> tableQuery = db.Table<TBL_Tasks>();
            TableMapping tableMap = db.GetMapping<TBL_Tasks>();
            //db.Delete<TBL_Tasks>(2);
            var stock = db.Get<TBL_Tasks>(2); // primary key id of 5

            //tableMap.Columns[0].Name = "ID";

            foreach (TBL_Tasks s in tableQuery)
            {
                Console.WriteLine(s.ID + " " + s.Title);
            }
        }



        [Table("TBL_Tasks")]
        public class TBL_Tasks
        {
            [PrimaryKey, AutoIncrement, Column("ID")]
            public int ID { get; set; }
            [MaxLength(8)]
            public string Title { get; set; }
            public string Description { get; set; }
            public string DateDue { get; set; }
        }
    }

    
}