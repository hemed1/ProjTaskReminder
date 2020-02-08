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

        public bool DeleteRecord(string tableName, int id)
        {
            bool result = true;

            string sqlScript = "DELETE FROM " + tableName + " " +
                               "WHERE TaskID=" + id.ToString();

            try
            {
                //this.DB = this.getWritableDatabase();
                DB.Execute(sqlScript);
                DB.Close();
            }
            catch (Exception ex)
            {
                result = false;
                //Log.d("DataBase", "Error in 'deleteRecord()': " + ex.getMessage());
                Utils.Utils.WriteToLog("DataBase - Error in 'deleteRecord()': \n" + ex.Message, true);
                //throw ex;
            }

            return result;
        }

        public long AddRecord(string tableName, List<KeyValuePair<string, string>> values)
        {
            string sqlScript = "";
            long recordsEffected=0;


            try
            {
                //ContentValues contentValues = addContentValues(values);
                sqlScript = "INSERT INTO " + tableName + " (";
                for (int i = 0; i < values.Count; i++)
                {
                    sqlScript += values[i].Key+",";
                }
                sqlScript = sqlScript.Substring(0, sqlScript.Length - 1);
                sqlScript += ")" + "VALUES ( ";
                for (int i = 0; i < values.Count; i++)
                {
                    sqlScript += values[i].Value + ",";
                }
                sqlScript = sqlScript.Substring(0, sqlScript.Length - 1);
                this.DB.Execute(sqlScript, null);
                //recordsEffected = DB.insert(tableName, null, contentValues);

                DB.Close();
            }
            catch (Exception ex)
            {
                string msg = "DataSet - Error in 'addRecord()'" + "\n" + ex.Message;
                Utils.Utils.WriteToLog(msg, true);
                recordsEffected = -1;
            }

            return recordsEffected;
        }

        public long UpdateRecord(string tableName, List<KeyValuePair<string, string>> values, object[] args)
        {
            long recordsEffected = 0;
            string sqlScript = "";


            try
            {
                sqlScript = "UPDATE " + tableName + " SET ";
                for (int i = 0; i < values.Count; i++)
                {
                    sqlScript += values[i].Key + "='" + values[i].Value + "'" + ",";
                }
                sqlScript = sqlScript.Substring(0, sqlScript.Length - 1);
                //ContentValues contentValues = addContentValues(values);

                if (args==null)
                {
                    recordsEffected = DB.Execute(sqlScript);
                    //recordsEffected = DB.update(tableName, contentValues, null, null);
                }
                else
                {
                    sqlScript += " WHERE  ID=?";
                    recordsEffected = DB.Execute(sqlScript, args[0]);
                    //recordsEffected = DB.update(tableName, contentValues, values.get(0).first + "=?", new string[] { values.get(0).second });
                }
                //Log.d("DataSet - 'updateRecord()'", "Record was updated OK - recordsEffected: "+string.valueOf(recordsEffected));
            }
            catch (Exception ex)
            {
                string msg = "DataSet - Error in 'updateRecord()'";
                //Log.d(msg, ex.getMessage());
                Utils.Utils.WriteToLog(msg + "\n" + ex.Message, true);
                //throw ex;
            }

            ////DB.Close();

            return recordsEffected;
        }

        private ContentValues addContentValues(List<KeyValuePair<String, String>> values)
        {
            ContentValues contentValues = new ContentValues();

            for (int i = 0; i < values.Count; i++)
            {
                contentValues.Put(values[i].Key, values[i].Value);
            }

            return contentValues;
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