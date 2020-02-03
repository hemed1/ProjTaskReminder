using System;
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

namespace ProjTaskReminder.Utils
{

    public static class Utils
    {

        public static string                LOG_FILE_PATH;
        public static string                LOG_FILE_NAME;

        public static void LoggerWrite(string data, bool appendLines)
        {
            string fileName = string.Empty;
            byte[] bytesData;
            FileStream file;




            data += DateTime.Now.ToString("dd/MM/yyyy HH:mm") + System.Environment.NewLine + data;


            // /storage/emulated/0/Android/data/com.meirhemed.projtaskreminder/files
            //System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);  //"/Assets/" + DB_NAME;             
            //Environment.SpecialFolder.LocalApplicationData
            // Environment.getExternalStorageDirectory().getAbsolutePath() + "/MHTaskReminder/"
            Context context = Android.App.Application.Context;
            //Java.IO.File pathFile = context.GetExternalFilesDir("MUSIC");


            try
            {
                //Java.IO.File[] jjj = context.GetExternalFilesDirs("MUSIC");
                //Java.IO.File[] mmm = context.GetExternalMediaDirs();
                //string zzz = System.Environment.SystemDirectory;
                //string aaa = System.Environment.CurrentDirectory;
                //string bbb = System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonMusic);
                //string fff = System.IO.Directory.GetCurrentDirectory();
                //string hjhj = LOG_FILE_PATH = System.IO.Directory.GetCurrentDirectory();
                //Java.IO.File hdhd = Android.OS.Environment.DataDirectory;
                //string folderNameDocuments = Android.OS.Environment.DirectoryDocuments;
                //string folderNameMusic = Android.OS.Environment.DirectoryMusic;
                //Java.IO.File externalPath = Android.OS.Environment.ExternalStorageDirectory;
                //string externalPathFile = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
                //string folderMusic = Android.OS.Environment.GetExternalStoragePublicDirectory(folderNameMusic).AbsolutePath;
                //LOG_FILE_PATH = externalPath + "/ProjTaskReminder";    //, FileCreationMode.Append).AbsolutePath;

                //if (!Directory.Exists(LOG_FILE_PATH))
                //{
                //    //Directory.CreateDirectory(LOG_FILE_PATH);
                //}

                LOG_FILE_PATH = context.GetExternalFilesDir("").AbsolutePath;
                LOG_FILE_NAME = "LogTaskReminder.txt";

                fileName = Path.Combine(LOG_FILE_PATH, LOG_FILE_NAME);

                file = new FileStream(fileName, FileMode.Append, FileAccess.Write);       // FileMode.Append

                bytesData = Encoding.ASCII.GetBytes(data);

                file.Write(bytesData);

                file.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// Get all files - Recorsive
        public static List<string> GetFolderFiles(string path, string fileExtentionToSearch, bool searchInFolders)
        {
            string file;
            List<string> files = new List<string>();
            string[] directories;
            //static ArrayList<File>  foundfiles
            String[] multiSearchValuse = new String[0];


            fileExtentionToSearch = fileExtentionToSearch.ToLower();

            directories = Directory.GetDirectories(path);    //   File(path);

            files = Directory.GetFiles(path, fileExtentionToSearch).ToList();

            if (files == null || files.Count()==0)
            {
                return files;
            }

            //        if (fileExtentionToSearch.contains(";"))
            //        {
            //            multiSearchValuse = fileExtentionToSearch.split(";");
            //            fileExtentionToSearch = multiSearchValuse[0];
            //        }

            for (int i = 0; i < files.Count; i++)
            {
                file = files[i];

                if (searchInFolders && Directory.Exists(file))
                {
                    files.AddRange(GetFolderFiles(file, fileExtentionToSearch, searchInFolders));
                }
                else if (file.ToLower().EndsWith(fileExtentionToSearch))
                {
                    files.Add(file);
                    //songsFiles.add(file);
                }
            }


            return files;
        }

    }
}