using System;
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
using Android.Text;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;

namespace ProjTaskReminder.Utils
{

    public static class Utils
    {
        public const string                LINE_SEPERATOR = "\n";
        public static string                LOG_FILE_PATH;
        public static string                LOG_FILE_NAME;

        public static List<KeyValuePair<string, List<string>>> FilesExtra;

        public static Context context;
        public static Activity activity;


        public static void WriteToLog(string data, bool appendLines = true)
        {
            string fileName = string.Empty;
            byte[] bytesData;
            FileStream file;




            data = LINE_SEPERATOR + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + LINE_SEPERATOR + data + LINE_SEPERATOR;
            Context context = Android.App.Application.Context;


            // /storage/emulated/0/Android/data/com.meirhemed.projtaskreminder/files
            //System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);  //"/Assets/" + DB_NAME;             
            //Environment.SpecialFolder.LocalApplicationData
            // Environment.getExternalStorageDirectory().getAbsolutePath() + "/MHTaskReminder/"
            //Java.IO.File pathFile = context.GetExternalFilesDir("MUSIC");


            try
            {
                string folderBackup = Android.OS.Environment.DirectoryMusic;        // "ProjTaskReminder"

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


                LOG_FILE_PATH = Android.OS.Environment.GetExternalStoragePublicDirectory(folderBackup).AbsolutePath;
                //LOG_FILE_PATH = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
                //LOG_FILE_PATH = context.GetExternalFilesDir("").AbsolutePath;

                if (!Directory.Exists(LOG_FILE_PATH))
                {
                    Directory.CreateDirectory(LOG_FILE_PATH);
                }

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

                bytesData = Encoding.UTF8.GetBytes(data);        // Encoding.UTF8..ASCII.GetBytes(data);

                file.Write(bytesData);

                file.Close();
            }
            catch (Exception ex)
            {
                //throw ex;
            }
        }


        /// Get all files - Recorsive
        public static List<string> GetFolderFiles(string path, string fileExtentionToSearch, bool searchInFolders, string fileExtentionToSearch2="")
        {
            string file;
            List<string> files = new List<string>();
            List<KeyValuePair<string, List<string>>> filesResult = new List<KeyValuePair<string, List<string>>>();
            List<string> directories = new List<string>(); ; 
            //static ArrayList<File>  foundfiles
            String[] multiSearchValuse = new String[0];




            fileExtentionToSearch = fileExtentionToSearch.ToLower();

            try
            {
                directories = Directory.GetDirectories(path).ToList();    //   File(path);
            }
            catch (Exception ex)
            {

            }

            if (directories == null || directories.Count()==0)
            {
                //directories = new List<string>() { path }.ToArray();
                return files;
            }

            //        if (fileExtentionToSearch.contains(";"))
            //        {
            //            multiSearchValuse = fileExtentionToSearch.split(";");
            //            fileExtentionToSearch = multiSearchValuse[0];
            //        }

            for (int j = 0; j < directories.Count(); j++)
            {
                path = directories[j];
                files = Directory.GetFiles(path, fileExtentionToSearch).ToList();

                for (int i = 0; i < files.Count; i++)
                {
                    file = files[i];

                    List<string> fileExtra = new List<string>();

                    if (FilesExtra == null)
                    {
                        FilesExtra = new List<KeyValuePair<string, List<string>>>();
                    }

                    path = Directory.GetParent(file).FullName;
                    List<string> dirs = Directory.GetFiles(path, fileExtentionToSearch2).ToList();
                    //if (Directory.Exists(file))
                    //{
                    //    dirs = GetFolderFiles(Directory.GetParent(file).FullName, fileExtentionToSearch2, searchInFolders, "");
                    //}

                    KeyValuePair<string, List<string>> fileProp = new KeyValuePair<string, List<string>>(file, dirs);
                    FilesExtra.Add(fileProp);


                    if (searchInFolders && Directory.Exists(file))
                    {
                        files.AddRange(GetFolderFiles(file, fileExtentionToSearch, searchInFolders));
                    }
                    else    //if (file.ToLower().EndsWith(fileExtentionToSearch))
                    {
                        //files.Add(file);
                        //songsFiles.add(file);
                    }
                }
            }


            return files;
        }

        //public static List<string> AddFilesToArray(List<string> files, string file, string fileExtentionToSearch, bool searchInFolders, string fileExtentionToSearch2 = "")
        //{
        //    List<string> fileExtra = new List<string>();

        //    if (FilesExtra == null)
        //    {
        //        FilesExtra = new List<KeyValuePair<string, List<string>>>();
        //    }

        //    List<string> tmpFiles = GetFolderFiles(file, fileExtentionToSearch, searchInFolders, fileExtentionToSearch2);
        //    KeyValuePair<string, List<string>> fileProp = new KeyValuePair<string, List<string>>(file, tmpFiles);
        //    FilesExtra.Add(fileProp);


        //    if (searchInFolders && Directory.Exists(file))
        //    {
        //        files.AddRange(GetFolderFiles(file, fileExtentionToSearch, searchInFolders));
        //    }
        //    else if (file.ToLower().EndsWith(fileExtentionToSearch))
        //    {
        //        files.Add(file);
        //        //songsFiles.add(file);
        //    }

        //    retrun files;
        //}

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

        public static DateTime GetDateNow()
        {
            return getDateFixed(DateTime.Now);
        }

        public static Calendar getDateNow()
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



            cal = getDateNow();

            //cal = date; //.setTime(date);

            result = cal.ToDateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, date.Millisecond);

            //result = result.AddHours(2);

            //Log.d("Util - getDateFixed() - Final", String.valueOf(result));


            return result;
        }

        public static DateTime getDateFixed(int year, int month, int day, int hour, int minutes)
        {
            DateTime date;
            Calendar cal;


            cal = getDateNow();     //new GregorianCalendar(year, month, day, hour, minutes);

            date = cal.ToDateTime(year, month, day, hour, minutes, 0, 0);

            //Log.d("Util - getDateFixed() 1","Day: "+String.valueOf(day)+"  Month: "+String.valueOf(month)+"  Year: "+ String.valueOf(year) + "  Fixed: " +String.valueOf(date));

            ////date = getDateFixed(date);

            //Log.d("Util - getDateFixed() 2", String.valueOf(date));

            return date;
        }

        public static String getDateFormattedString(DateTime date)
        {
            string dateStr;


            //Locale locale = new Locale.Builder().setLanguage("iw").setRegion("IL").build();
            ////DateFormat timeFormat = new SimpleDateFormat("dd/MM/yyyy HH:mm");   //, locale);
            ////timeFormat.setTimeZone(TimeZone.getTimeZone("Asia/Jerusalem"));
            ////dateStr = timeFormat.format(date);

            dateStr = date.ToString("dd/MM/yyyy HH:mm");

            //Log.d("Util - getDateFormattedString()", dateStr);


            return dateStr;
        }

        /// <summary>
        /// Return DateTime in USA format 'MM/dd/yyyy'
        /// </summary>
        /// <param name="dateStr"></param>
        /// <returns></returns>
        public static DateTime getDateFormatUSA(string dateStr)
        {
            DateTime date;

            dateStr = dateStr.Substring(3, 2) + "/" + dateStr.Substring(0, 2) + "/" + dateStr.Substring(6, 4);

            if (!DateTime.TryParse(dateStr, out date))
            {
                return DateTime.MinValue;
            }

            return date;
        }

        public static string getDateDayName(DateTime date)
        {
            string dayName = "";


            switch (date.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    dayName = "א'";
                    break;
                case DayOfWeek.Monday:
                    dayName = "ב'";
                    break;
                case DayOfWeek.Tuesday:
                    dayName = "ג'";
                    break;
                case DayOfWeek.Wednesday:
                    dayName = "ד'";
                    break;
                case DayOfWeek.Thursday:
                    dayName = "ה'";
                    break;
                case DayOfWeek.Friday:
                    dayName = "ו'";
                    break;
                case DayOfWeek.Saturday:
                    dayName = "שבת";
                    break;
            }

            return dayName;
        }

        public static SpannedString trimSpannedText(SpannedString spannedInput)
        {
            SpannedString spanned = (SpannedString)spannedInput;
            int end = -1;
            int start = -1;


            if (spanned == null)
            {
                return spanned;
            }
            if (spanned.ToString().Equals(""))
            {
                return spanned;
            }
            if (spanned.SubSequence(spanned.Length() - 1, spanned.Length()).ToString().Equals("\n"))
            {
                start = spanned.Length() - 2;
                end = spanned.Length();
            }


            //end = spanned.getSpanEnd(new BackgroundColorSpan(Color.MAGENTA));
            //start = spanned.getSpanStart(new BackgroundColorSpan(Color.MAGENTA));
            //BackgroundColorSpan[] bbb = spanned.getSpans(start, end, new BackgroundColorSpan(Color.MAGENTA).getClass());

            if (start != -1 && end != -1)
            {
                //spanned.Delete(start, end);
            }

            return spanned;
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

        public static bool CopyFile(string sourcePath, string targetPath)
        {
            bool result = false;


            try
            {
                if (File.Exists(sourcePath))
                {
                    File.Copy(sourcePath, targetPath, true);
                    result = true;
                }
            }
            catch (Exception ex)
            {
                WriteToLog(ex.Message, true);
            }

            return result;
        }
    }
}