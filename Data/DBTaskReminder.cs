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
using ProjTaskReminder.Model;
using SQLite;



namespace ProjTaskReminder.Data
{
    public class DBTaskReminder
    {
        
        public SQLiteConnection DB { get; set; }


        public string DB_PATH;
        public string DB_NAME;          // "TaskReminderDB"
        public string DB_TABLE_NAME;    // "TaskReminderDB"
        private Context context;        // Android.App.Application.Context;



        public DBTaskReminder(string dbName, string dbPath, string tableName)
        {
            context = Android.App.Application.Context;

            DB_NAME = dbName;
            DB_PATH = dbPath;
            DB_TABLE_NAME = tableName;

            Connect(DB_TABLE_NAME);
        }

        private SQLiteConnection Connect(string tableName) 
        {
            SQLiteConnection db = null;
            string fullPath;


            

            try
            {
                fullPath = Path.Combine(DB_PATH, DB_NAME);

                db = new SQLiteConnection(fullPath);

                //mDBcon.ConnectionString = "Data Source=" + DataSourcePath;
                //mDBcon.Open();

                this.DB = db;

                
                //List<SQLiteConnection.ColumnInfo> columns = db.GetTableInfo(tableName);
                //if (DB.GetTableInfo(tableName).Exists(a=> a.notnull==1) || columns.Count == 0)
                //{
                //    CreateTableResult(tableName);
                //}
                //else if (columns.Count<5)
                //{
                    

                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace + "  -  " + ex.Message);
            }

            //System.IO.DirectoryInfo info =  System.IO.Directory.CreateDirectory(DB_PATH);
            //Uri originalDbFileUri;  // = new Uri("ms-appx:///Assets/"+DB_NAME);
            //Uri.TryCreate("ms-appx:///Assets/" + DB_NAME, UriKind.Absolute, out originalDbFileUri);

            return db;
        }

        private void CreateTableResult(string tableName)
        {
            try
            {
                var t = DB.CreateTable<TBL_Tasks>();
                //string commanScript = "ALTER TABLE TBL_Tasks ADD COLUMN DateLastUpdate VARCHAR(11);";
                //commanScript = "CREATE TABLE IF NOT EXISTS tags (ISBN VARCHAR(15), Tag VARCHAR(15));";
                //DB.Execute(commanScript, null);    
                //SQLiteCommand cmd = db.CreateCommand(commanScript, null);        //new SQLiteCommand(this.DB);
                //cmd.CommandText = commanScript;
                //cmd.ExecuteNonQuery();
                //db.DropTable<TBL_Tasks>();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace+"  -  "+ex.Message);
            }
        }

        public long RecordInser(Object record)  //, Type tableTyp = TBL_Tasks)
        {
            long recordsEffected = 0;


            try
            {
                recordsEffected = DB.Insert(record);
                Object recordUpdated = getLastRecord(MainActivity.DB_TABLE_NAME);
                ((TBL_Tasks)record).ID = ((TBL_Tasks)recordUpdated).ID;
            }
            catch (Exception ex)
            {
                string msg = "DataSet - Error in 'RecordInser()'" + "\n" + ex.Message;
                Utils.Utils.WriteToLog(msg, true);
            }

            return recordsEffected;

            //if (db.Table<TBL_Tasks>().Count() == 0)
            //{
            // only insert the data if it doesn't already exist
            //var item = new TBL_Tasks();
            //item.Title = "Apple";
            //item.Description = "Apple Indsroy";
            //db.Insert(item);
            //item = new TBL_Tasks();
            //item.Title = "Google";
            //item.Description = "Google Indsroy";
            //db.Insert(item);
            //}
            //Console.WriteLine("Reading data");
            //TableQuery<TBL_Tasks> tableQuery = DB.Table<TBL_Tasks>();
            //TableMapping tableMap = DB.GetMapping<TBL_Tasks>();
            //db.Delete<TBL_Tasks>(2);
            //var stock = db.Get<TBL_Tasks>(2); // primary key id of 2
            //tableMap.Columns[0].Name = "ID";
            //foreach (TBL_Tasks s in tableQuery)
            //{
            //    Console.WriteLine(s.ID + " " + s.Title);
            //}
        }

        public long RecordInser(string tableName, List<KeyValuePair<string, string>> values)
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

        public long RecordUpdate(Object record)  //, Type tableTyp = TBL_Tasks)
        {
            long recordsEffected = 0;


            try
            {
                recordsEffected = DB.Update(record);
            }
            catch (Exception ex)
            {
                string msg = "DataSet - Error in 'RecordUpdate()'" + "\n" + ex.Message;
                Utils.Utils.WriteToLog(msg, true);
            }

            return recordsEffected;

        }

        public long RecordUpdate(string tableName, List<KeyValuePair<string, string>> values, object[] args)
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
                string msg = "DataSet - Error in 'RecordUpdate()'";
                //Log.d(msg, ex.getMessage());
                Utils.Utils.WriteToLog(msg + "\n" + ex.Message, true);
                //throw ex;
            }

            ////DB.Close();

            return recordsEffected;
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

        public object getRecordByID(int id, string tableName)
        {
            var result = DB.Get<TBL_Tasks>(id);  // primary key id of 5

            //TableQuery<TBL_Tasks> table = DB.Table<TBL_Tasks>();
            //result = table.Where(a => a.ID == id).FirstOrDefault();
            //foreach (TBL_Tasks record in table)
            //{
            //}

            //TableMapping tableMap = DB.GetMapping<TBL_Tasks>();

            //string sqlScript = "SELECT * FROM " + TableName + " WHERE ID=?";
            //List<object> objects = DB.Query(tableMap, sqlScript, new object[1] { id });

            DB.Close();

            return result;
        }

        public Object getLastRecord(string tableName) //: where T = TBL_Tasks
        {
            TBL_Tasks result = null;
            //var result = DB.Get<TBL_Tasks>(id);  // primary key id of 5

            //List < ColumnInfo >= DB.GetTableInfo(tableName);
            TableQuery<TBL_Tasks> table = DB.Table<TBL_Tasks>();

            result = table.Last();
            //foreach (TBL_Tasks record in table)
            //{
            //}

            //TableMapping tableMap = DB.GetMapping<TBL_Tasks>();

            //string sqlScript = "SELECT * FROM " + TableName + " WHERE ID=?";
            //List<object> objects = DB.Query(tableMap, sqlScript, new object[1] { id });

            DB.Close();

            return result;
        }

        public bool addFieldToTable(String tableName, String fieldName, String fieldType)
        {
            String sqlScript;
            bool result = false;


            sqlScript = "ALTER TABLE " + tableName + " ADD COLUMN " + fieldName + " " + fieldType;

            //string commanScript = "ALTER TABLE TBL_Tasks ADD COLUMN DateLastUpdate VARCHAR(11);";
            //commanScript = "CREATE TABLE IF NOT EXISTS tags (ISBN VARCHAR(15), Tag VARCHAR(15));";
            //Utils.Utils.WriteToLog("DataBase - 'addFieldToTable()': " + tableName + "'" + fieldName);

            try
            {
                //this.DB = this.getWritableDatabase();
                this.DB.Execute(sqlScript, null);
                DB.Close();
            }
            catch (Exception ex)
            {
                //Log.d("DataBase", "Error in 'addFieldToTable()': " + ex.getMessage());
                Utils.Utils.WriteToLog("DataBase - Error in 'addFieldToTable()': " + tableName + "' \n" + ex.Message);
                //throw ex;
            }

            return result;
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

    
    
    }

    
}