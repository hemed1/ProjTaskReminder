﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;

namespace ProjTaskReminder.Utils
{

    public static class Utils
    {
        private const string                LINE_SEPERATOR = "\n";
        public static string                LOG_FILE_PATH;
        public static string                LOG_FILE_NAME;

        public static Context context;
        public static Activity activity;

        public static void WriteToLog(string data, bool appendLines = true)
        {
            string fileName = string.Empty;
            byte[] bytesData;
            FileStream file;




            data = DateTime.Now.ToString("dd/MM/yyyy HH:mm") + LINE_SEPERATOR + data + LINE_SEPERATOR;
            Context context = Android.App.Application.Context;


            // /storage/emulated/0/Android/data/com.meirhemed.projtaskreminder/files
            //System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);  //"/Assets/" + DB_NAME;             
            //Environment.SpecialFolder.LocalApplicationData
            // Environment.getExternalStorageDirectory().getAbsolutePath() + "/MHTaskReminder/"
            //Java.IO.File pathFile = context.GetExternalFilesDir("MUSIC");


            try
            {
                string folderNameMusic = Android.OS.Environment.DirectoryMusic;

                //Java.IO.File[] jjj = context.GetExternalFilesDirs("MUSIC");
                //Java.IO.File[] mmm = context.GetExternalMediaDirs();
                //string zzz = System.Environment.SystemDirectory;
                //string aaa = System.Environment.CurrentDirectory;
                //string bbb = System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonMusic);
                //string fff = System.IO.Directory.GetCurrentDirectory();
                //string hjhj = LOG_FILE_PATH = System.IO.Directory.GetCurrentDirectory();
                //Java.IO.File hdhd = Android.OS.Environment.DataDirectory;
                //string folderNameDocuments = Android.OS.Environment.DirectoryDocuments;
                //Java.IO.File externalPath = Android.OS.Environment.ExternalStorageDirectory;


                LOG_FILE_PATH = Android.OS.Environment.GetExternalStoragePublicDirectory(folderNameMusic).AbsolutePath;
                //LOG_FILE_PATH = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
                //LOG_FILE_PATH = context.GetExternalFilesDir("").AbsolutePath;

                //if (!Directory.Exists(LOG_FILE_PATH))
                //{
                //    //Directory.CreateDirectory(LOG_FILE_PATH);
                //}

                LOG_FILE_NAME = "LogTaskReminder.txt";

                string songPath = LOG_FILE_PATH + "/" + LOG_FILE_NAME;
                fileName = Path.Combine(LOG_FILE_PATH, LOG_FILE_NAME);

                try
                {
                    file = new FileStream(fileName, ((appendLines)?FileMode.Append: FileMode.Create), FileAccess.Write);       // FileMode.Append
                }
                catch 
                {
                    LOG_FILE_PATH = context.GetExternalFilesDir("").AbsolutePath;
                    songPath = LOG_FILE_PATH + "/" + LOG_FILE_NAME;
                    fileName = Path.Combine(LOG_FILE_PATH, LOG_FILE_NAME);
                    file = new FileStream(fileName, ((appendLines) ? FileMode.Append : FileMode.Create), FileAccess.Write);       // FileMode.Append
                }

                bytesData = Encoding.ASCII.GetBytes(data);

                file.Write(bytesData);

                file.Close();
            }
            catch (Exception ex)
            {
                //throw ex;
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

        public static DateTime getDateFromString(string dateString)
        {
            DateTime result = DateTime.MinValue;

            try
            {
                if (dateString.Trim().Equals(""))
                {
                    return result;
                }

                int day = int.Parse(dateString.Substring(0, 2));
                int month = int.Parse(dateString.Substring(3, 2));
                int year = int.Parse(dateString.Substring(6, 4));
                int hour = 0;
                int minute = 0;

                if (dateString.Trim().Length > 10)
                {
                    hour = int.Parse(dateString.Substring(11, 2));
                    minute = int.Parse(dateString.Substring(14, 2));
                }

                
                dateString = month.ToString().PadLeft(2, '0') + "/" + day.ToString().PadLeft(2, '0') + "/" + year.ToString().PadLeft(4, '0') + " " +
                            hour.ToString().PadLeft(2, '0') + ":" + minute.ToString().PadLeft(2, '0');

                result = DateTime.Parse(dateString);    //, DateTimeStyles.AssumeLocal, );  // (year, month, day, hour, minute);

                // TODO:: Just to show the real Input Month value in 'Log.D()'
                //month = month -1;
                // TODO:: Why
                
                //// TODO: Meir //result = getDateFixed(year, month - 1, day, hour, minute);

                //Log.d("Util - getDateFromString()", dateString + "  **"+String.valueOf(day)+"**"+String.valueOf(month)+"**"+String.valueOf(year)+"***  Final:" + String.valueOf(result));

            }
            catch (Exception ex)
            {
                WriteToLog("Error in 'getDateByString()': \n" + ex.Message);
                //throw ex;
            }

            return result;
        }

        public static Calendar getDateInstance()
        {
            //long timezoneAlteredTime = getDateTimeDiff();


          // Make the Hebrew Calendar the current calendar and
          // Hebrew (Israel) the current thread culture.
            //HebrewCalendar hc = new HebrewCalendar();
            //CultureInfo culture = CultureInfo.CreateSpecificCulture("he-IL");
            //culture.DateTimeFormat.Calendar = hc;
            //Thread.CurrentThread.CurrentCulture = culture;

            //DateTime date = DateTime.Date;
            Calendar cal = new GregorianCalendar(GregorianCalendarTypes.Localized);  //.GetInstance(TimeZone.getTimeZone("Asia/Jerusalem"));
            //cal.setTimeInMillis(timezoneAlteredTime);

            return cal;
        }

        public static DateTime getDateFixed(DateTime date)
        {
            Calendar cal;
            int month;
            int day;
            int year;
            DateTime result;



            cal = getDateInstance();

            //cal = date; //.setTime(date);

            result = cal.ToDateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, date.Millisecond).AddHours(2);

            //Log.d("Util - getDateFixed() - Final", String.valueOf(result));


            return result;
        }

        public static DateTime getDateFixed(int year, int month, int day, int hour, int minutes)
        {
            DateTime date;
            Calendar cal;


            cal = getDateInstance();     //new GregorianCalendar(year, month, day, hour, minutes);

            date = cal.ToDateTime(year, month, day, hour, minutes, 0, 0);

            //Log.d("Util - getDateFixed() 1","Day: "+String.valueOf(day)+"  Month: "+String.valueOf(month)+"  Year: "+ String.valueOf(year) + "  Fixed: " +String.valueOf(date));

            ////date = getDateFixed(date);

            //Log.d("Util - getDateFixed() 2", String.valueOf(date));

            return date;
        }

        public static String getDateFormattedString(DateTime date)
        {
            ////string monthStr;
            ////string dayStr;
            string dateStr;



            // TODO:  Maybe delete
            //date = getDateFixed(date);

            //Locale locale = new Locale.Builder().setLanguage("iw").setRegion("IL").build();

            ////DateFormat timeFormat = new SimpleDateFormat("dd/MM/yyyy HH:mm");   //, locale);
            ////timeFormat.setTimeZone(TimeZone.getTimeZone("Asia/Jerusalem"));

            ////dateStr = timeFormat.format(date);

            dateStr = date.ToString("dd/MM/yyyy HH:mm");

            //Log.d("Util - getDateFormattedString()", dateStr);


            return dateStr;
        }

        public static void openKeyboard()
        {
            //InputMethodManager inputMethodManager = (InputMethodManager)context.getSystemService(Context.InputMethodService);
            //if (inputMethodManager != null)
            //{
            //    inputMethodManager.toggleSoftInput(InputMethodManager.SHOW_IMPLICIT, 0);   //InputMethodManager.SHOW_FORCED
            //}

            //activity.getWindow().setSoftInputMode(WindowManager.LayoutParams.SOFT_INPUT_STATE_VISIBLE);

        }

        public static void closeKeyboard()
        {
            //InputMethodManager inputMethodManager = (InputMethodManager)context.getSystemService(Context.INPUT_METHOD_SERVICE);
            //if (inputMethodManager != null)
            //{
            //    //Log.d("keyboard close", "Close Keyboard");
            //    inputMethodManager.toggleSoftInput(InputMethodManager.RESULT_HIDDEN, 0);   //InputMethodManager., , RESULT_HIDDEN , HIDE_IMPLICIT_ONLY
            //}

            //activity.getWindow().setSoftInputMode(WindowManager.LayoutParams.SOFT_INPUT_STATE_HIDDEN);      //.SOFT_INPUT_STATE_ALWAYS_HIDDEN);        //);  // WindowManager.LayoutParams.SOFT_INPUT_STATE_VISIBLE,

        }

        //public static void RequestPermissions()
        //{


        //    if (ContextCompat.checkSelfPermission(this, Manifest.permission.READ_EXTERNAL_STORAGE) != PackageManager.PERMISSION_GRANTED)
        //    {
        //        Toast.makeText(this, "Permission READ_EXTERNAL_STORAGE is Not OK", Toast.LENGTH_SHORT).show();
        //        //return;
        //    }
        //    else
        //    {
        //        // Have to ask permmissions
        //    }

        //    // Here, thisActivity is the current activity
        //    if (ContextCompat.checkSelfPermission(this, Manifest.permission.READ_EXTERNAL_STORAGE) != PackageManager.PERMISSION_GRANTED)
        //    {
        //        //Toast.makeText(this, "Permission WRITE_CALENDAR not granted", Toast.LENGTH_SHORT).show();
        //        // Permission is not granted
        //        // Should we show an explanation?
        //        if (ActivityCompat.shouldShowRequestPermissionRationale(this, Manifest.permission.READ_EXTERNAL_STORAGE))
        //        {
        //            // Show an explanation to the user *asynchronously* -- don't block
        //            // this thread waiting for the user's response! After the user
        //            // sees the explanation, try again to request the permission.
        //        }
        //        else
        //        {
        //            // No explanation needed; request the permission
        //            ActivityCompat.requestPermissions(this, new String[] { Manifest.permission.READ_EXTERNAL_STORAGE }, PERMISSIONS_REQUEST_READ_STORAGE);

        //            // PERMISSIONS_REQUEST_READ_STORAGE is an app-defined int constant. The callback method gets the
        //            // result of the request.
        //        }
        //    }
        //    else
        //    {
        //        // Permission has already been granted
        //    }

        //}

        //public static void onRequestPermissionsResult(int requestCode, String permissions[], int[] grantResults)
        //{
        //    switch (requestCode)
        //    {
        //        case PERMISSIONS_REQUEST_READ_STORAGE:
        //            {
        //                // If request is cancelled, the result arrays are empty.
        //                if (grantResults.length > 0 && grantResults[0] == PackageManager.PERMISSION_GRANTED)
        //                {
        //                    // permission was granted, yay! Do the
        //                    // contacts-related task you need to do.
        //                }
        //                else
        //                {
        //                    // permission denied, boo! Disable the
        //                    // functionality that depends on this permission.
        //                }
        //                return;
        //            }

        //            // other 'case' lines to check for other
        //            // permissions this app might request.
        //    }
        //}

    }
}