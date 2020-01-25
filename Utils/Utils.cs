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

        /// Get all files - Recorsive
        public static string[] GetFolderFiles(string path, string fileExtentionToSearch, bool searchInFolders)
        {
            string file;
            string[] files = new string[100];
            string[] directory;
            //static ArrayList<File>  foundfiles
            String[] multiSearchValuse = new String[0];


            fileExtentionToSearch = fileExtentionToSearch.ToLower();

            directory = Directory.GetDirectories(path);    //   File(path);

            files = Directory.GetFiles(path, fileExtentionToSearch);

            if (files == null)
            {
                return files;
            }

            //        if (fileExtentionToSearch.contains(";"))
            //        {
            //            multiSearchValuse = fileExtentionToSearch.split(";");
            //            fileExtentionToSearch = multiSearchValuse[0];
            //        }

            for (int i = 0; i < files.Length; i++)
            {
                file = files[i];

                if (searchInFolders && Directory.Exists(file))
                {
                    GetFolderFiles(file, fileExtentionToSearch, searchInFolders);
                }
                else if (file.ToLower().EndsWith(fileExtentionToSearch))
                {
                    //songsFiles.add(file);
                }
            }



            return files;
        }

    }
}