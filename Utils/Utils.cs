using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Xml;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Java.IO;
using static Android.App.ActivityManager;
//using Java.Net;

namespace MH_Utils
{

    public static class Utils
    {
        public const string LINE_SEPERATOR = "\n";
        public static string LOG_FILE_PATH;
        public static string LOG_FILE_NAME = "LogTaskReminder.txt";

        public static string NEWS_RSS_ADDRESS1 = " http://www.ynet.co.il/Integration/StoryRss1854.xml";
        public static string NEWS_RSS_ADDRESS2 = " https://www.kan.org.il/rss/allnews.ashx";
        public static string NEWS_RSS_ADDRESS3 = " http://rss.walla.co.il/feed/22";

        private const int PERMISSIONS_REQUEST_READ_STORAGE = 111;
        private const int PERMISSIONS_REQUEST_WRITE_STORAGE = 222;


        public static List<KeyValuePair<string, List<string>>> FilesExtra;

        public static Context ClientContext;
        public static Activity ClientActivity;


        public static void WriteToLog(Exception exception)
        {
            WriteToLog("Error:" + exception.Message + LINE_SEPERATOR + exception.StackTrace, true);
        }

        public static void WriteToLog(string data, bool appendLines = true)
        {
            string fileName = string.Empty;
            byte[] bytesData;
            StreamWriter fileWriter = null;
            string folderBackup = "";




            data = LINE_SEPERATOR + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + LINE_SEPERATOR + data + LINE_SEPERATOR;
            Context context = Android.App.Application.Context;

            //UTF8Encoding encoding = new UTF8Encoding();
            Encoding encoding = new UTF8Encoding(false);
            //Encoding encoding = Encoding.GetEncoding("Windows-1255");
            //byte[] pass = Encoding.ASCII.GetBytes("šarže");

            try
            {
                KeyValuePair<string, int>[] perm = new KeyValuePair<string, int>[] { new KeyValuePair<string, int>(Manifest.Permission.WriteExternalStorage, PERMISSIONS_REQUEST_WRITE_STORAGE) };
                PermissionAsk(perm);

                // Dne in ;Client; source that use these utils services
                //LOG_FILE_NAME = "LogTaskReminder.txt";
                //LOG_FILE_PATH = context.GetExternalFilesDir("").AbsolutePath;
                //LOG_FILE_PATH = Android.OS.Environment.GetExternalStoragePublicDirectory(folderBackup).AbsolutePath;
                //LOG_FILE_PATH = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;

                fileName = Path.Combine(LOG_FILE_PATH, LOG_FILE_NAME);

                try
                {
                    fileWriter = new StreamWriter(fileName, appendLines, encoding);

                    //file = new FileStream(fileName, ((appendLines)?FileMode.Append: FileMode.Create), FileAccess.Write);
                }
                catch
                {
                    folderBackup = Android.OS.Environment.DirectoryMusic;        // "ProjTaskReminder"
                    LOG_FILE_PATH = Android.OS.Environment.GetExternalStoragePublicDirectory(folderBackup).AbsolutePath;
                    if (!Directory.Exists(LOG_FILE_PATH))
                    {
                        Directory.CreateDirectory(LOG_FILE_PATH);
                    }
                    fileName = Path.Combine(LOG_FILE_PATH, LOG_FILE_NAME);

                    fileWriter = new StreamWriter(fileName, appendLines, encoding);

                    fileWriter.Write(data);

                    //FileStream file = new FileStream(fileName, ((appendLines) ? FileMode.Append : FileMode.Create), FileAccess.Write);
                }

                fileWriter.Write(data);

                fileWriter.Close();

                //bytesData = Encoding.UTF8.GetBytes(data);        // Encoding.UTF8..ASCII.GetBytes(data);  // encoding="ISO-8859-1
                //file.Write(bytesData);
                //file.Close();
            }
            catch (Exception ex)
            {
                //throw ex;
            }

            // /storage/emulated/0/Android/data/com.meirhemed.projtaskreminder/files
            //System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);  //"/Assets/" + DB_NAME;             
            //Environment.SpecialFolder.LocalApplicationData
            // Environment.getExternalStorageDirectory().getAbsolutePath() + "/MHTaskReminder/"
            //Java.IO.File pathFile = context.GetExternalFilesDir("MUSIC");
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


        }


        /// Get all files - Recorsive
        public static List<string> GetFolderFiles(string path, string fileExtentionToSearch, bool searchInFolders, string fileExtentionToSearch2 = "")
        {
            string file;
            List<string> files = new List<string>();
            //List<KeyValuePair<string, List<string>>> filesResult = new List<KeyValuePair<string, List<string>>>();
            List<string> directories = new List<string>();



            if (FilesExtra == null)
            {
                KeyValuePair<string, int>[] perm = new KeyValuePair<string, int>[] {
                                                                                      //new KeyValuePair<string, int>(Manifest.Permission.WriteExternalStorage, PERMISSIONS_REQUEST_WRITE_STORAGE),
                                                                                      new KeyValuePair<string, int>(Manifest.Permission.ReadExternalStorage, PERMISSIONS_REQUEST_READ_STORAGE)
                                                                                   };
                PermissionAsk(perm);
            }

            fileExtentionToSearch = fileExtentionToSearch.ToLower();

            try
            {
                directories = Directory.GetDirectories(path).ToList();
            }
            catch (Exception ex)
            {
                WriteToLog(ex);
            }

            if (directories == null || directories.Count() == 0)
            {
                directories.Add(path);
                //return files;
            }


            for (int j = 0; j < directories.Count(); j++)
            {
                path = directories[j];
                files = Directory.GetFiles(path, fileExtentionToSearch).ToList();

                List<string> subDirectories = Directory.GetDirectories(path).ToList();
                if (subDirectories.Count > 0)
                {
                    // Read Sub Directories
                    directories.AddRange(subDirectories);
                }

                for (int i = 0; i < files.Count; i++)
                {
                    file = files[i];

                    List<string> fileExtra = new List<string>();

                    if (FilesExtra == null)
                    {
                        FilesExtra = new List<KeyValuePair<string, List<string>>>();
                    }

                    List<string> filesPicts = new List<string>();

                    if (!fileExtentionToSearch2.Equals(""))
                    {
                        path = Directory.GetParent(file).FullName;
                        filesPicts = Directory.GetFiles(path, fileExtentionToSearch2).ToList();
                    }

                    KeyValuePair<string, List<string>> fileProp = new KeyValuePair<string, List<string>>(file, filesPicts);
                    FilesExtra.Add(fileProp);


                    if (searchInFolders && Directory.Exists(file))
                    {
                        files.AddRange(GetFolderFiles(file, fileExtentionToSearch, searchInFolders));
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
                if (dateString == null || dateString.Trim().Equals(""))
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

            //ClientActivity.getWindow().setSoftInputMode(WindowManager.LayoutParams.SOFT_INPUT_STATE_VISIBLE);

        }

        public static void closeKeyboard()
        {
            InputMethodManager inputMethodManager = (InputMethodManager)ClientContext.GetSystemService(Context.InputMethodService);       //.INPUT_METHOD_SERVICE);

            if (inputMethodManager != null)
            {
                //Log.d("keyboard close", "Close Keyboard");
                inputMethodManager.ToggleSoftInput(ShowFlags.Implicit, HideSoftInputFlags.ImplicitOnly);   //InputMethodManager., , RESULT_HIDDEN , HIDE_IMPLICIT_ONLY
            }

            ClientActivity.Window.SetSoftInputMode(SoftInput.StateHidden);    // WindowManager.LayoutParams.SOFT_INPUT_STATE_HIDDEN);      //.SOFT_INPUT_STATE_ALWAYS_HIDDEN);        //);  // WindowManager.LayoutParams.SOFT_INPUT_STATE_VISIBLE,

        }

        /// <summary>
        /// Check permission
        /// Example Input: Manifest.Permission.ReadExternalStorage
        /// Example Output: (int)Android.Content.PM.Permission.Granted
        /// </summary>
        public static string PermissionAsk(KeyValuePair<string, int>[] permissionNames)   //Manifest.Permission.ReadExternalStorage
        {
            string result = string.Empty;
            string[] requiredPermissions = permissionNames.Select(a => a.Key).ToArray();         //new string[]  { Manifest.Permission.ReadExternalStorage, Manifest.Permission.WriteExternalStorage };


            if (ClientActivity==null)
            {
                return result;
            }

            try
            {
                for (int i = 0; i < permissionNames.Length; i++)
                {
                    if (Android.Support.V4.Content.ContextCompat.CheckSelfPermission(ClientActivity, permissionNames[i].Key) == (int)Android.Content.PM.Permission.Granted)
                    {
                        // We have permission, go ahead and use the camera.
                        result += permissionNames[i].Key + " - " + "Is Granted" + LINE_SEPERATOR;
                    }
                    else
                    {
                        // Camera permission is not granted. If necessary display rationale & request.
                        result += permissionNames[i].Key + " - " + "Is Not Granted" + LINE_SEPERATOR;

                        // Manifest.Permission.Camera
                        // Manifest.Permission.ReadExternalStorage

                        if (Android.Support.V4.App.ActivityCompat.ShouldShowRequestPermissionRationale(ClientActivity, permissionNames[i].Key))
                        {
                            // Provide an additional rationale to the user if the permission was not granted
                            // and the user would benefit from additional context for the use of the permission.
                            // For example if the user has previously denied the permission.
                            Log.Info("Check Per", "Have to Displaying '" + permissionNames[i].Key + "' permission dialog screen");

                        }
                        else
                        {
                            Log.Info("Check Per", "Not Have to Displaying '" + permissionNames[i].Key + "' permission dialog screen");
                        }

                        Android.Support.V4.App.ActivityCompat.RequestPermissions(ClientActivity, new string[] { permissionNames[i].Key }, permissionNames[i].Value);     // PERMISSIONS_REQUEST_WRITE_STORAGE);
                    }

                }
            }
            catch (Exception ex)
            {

            }

            return result;
        }

        // Will be in Caller Activity
        //public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        //{
        //    Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

        //    if (requestCode == REQUEST_LOCATION)
        //    {
        //        // Received permission result for camera permission.
        //        lblResult.Text = "Received response for Read permission request.";


        //        View view = LayoutInflater.From(this).Inflate(Android.Resource.Layout.ActivityListItem, null, false);

        //        // Check if the only required permission has been granted
        //        if (grantResults.Length > 0 && grantResults[0] == Android.Content.PM.Permission.Granted)
        //        {
        //            // Location permission has been granted, okay to retrieve the location of the device.
        //            lblResult.Text = "Read permission has now been granted.";
        //            Toast.MakeText(this, (Android.Support.V4.Content.ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReadExternalStorage) == (int)Android.Content.PM.Permission.Granted).ToString(), ToastLength.Long);
        //            //Snackbar.Make(view, "granted", Snackbar.LengthLong).Show();
        //            //.SetAction("Action", (Android.Views.View.IOnClickListener)null)
        //        }
        //        else
        //        {
        //            lblResult.Text = "Read permission was NOT granted.";
        //            //Snackbar.Make(view, "not granted", Snackbar.LengthLong).Show();
        //            Toast.MakeText(this, (Android.Support.V4.Content.ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReadExternalStorage) == (int)Android.Content.PM.Permission.Granted).ToString(), ToastLength.Long);
        //        }
        //    }
        //    else
        //    {
        //        base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        //    }
        //}


        //private void RequestPermissions()
        //{


        //    if (context.CheckSelfPermission(this, Android.Manifest.Permission.ReadExternalStorage) != PackageManager.PERMISSION_GRANTED)
        //    {
        //        Toast.MakeText(this, "Permission READ_EXTERNAL_STORAGE is Not OK", ToastLength.Long).show();
        //        //return;
        //    }
        //    else
        //    {
        //        // Have to ask permmissions
        //    }

        //    // Here, thisActivity is the current activity
        //    if (context.CheckSelfPermission(this, Android.Manifest.Permission.ReadExternalStorage) != PackageManager..PERMISSION_GRANTED)
        //    {
        //        //Toast.makeText(this, "Permission WRITE_CALENDAR not granted", Toast.LENGTH_SHORT).show();
        //        // Permission is not granted
        //        // Should we show an explanation?
        //        if (ClientActivity.ShouldShowRequestPermissionRationale(Android.Manifest.Permission.ReadExternalStorage))
        //        {
        //            // Show an explanation to the user *asynchronously* -- don't block
        //            // this thread waiting for the user's response! After the user
        //            // sees the explanation, try again to request the permission.
        //        }
        //        else
        //        {
        //            // No explanation needed; request the permission
        //            ClientActivity.RequestPermissions(this, new String[] { Android.Manifest.Permission.ReadExternalStorage }, PERMISSIONS_REQUEST_READ_STORAGE);

        //            // PERMISSIONS_REQUEST_READ_STORAGE is an app-defined int constant. The callback method gets the
        //            // result of the request.
        //        }
        //    }
        //    else
        //    {
        //        // Permission has already been granted
        //    }

        //}

        //public static void OnRequestPermissionsResult2(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        //{
        //    Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

        //    ClientActivity.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        //}

        //public void onRequestPermissionsResult(int requestCode, String permissions[], int[] grantResults)
        //{
        //    switch (requestCode)
        //    {
        //        case PERMISSIONS_REQUEST_READ_STORAGE:
        //            {
        //                // If request is cancelled, the result arrays are empty.
        //                if (grantResults.Length > 0 && grantResults[0] == PackageManager.PERMISSION_GRANTED)
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

        /* Checks if external storage is available for read and write */
        //public boolean isExternalStorageAvailable()
        //{
        //    String state = Android.OS.Environment.GetExternalStorageState();
        //    if (Environment.MEDIA_MOUNTED.equals(state))
        //    {
        //        return true;
        //    }
        //    return false;
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
                if (System.IO.File.Exists(sourcePath))
                {
                    System.IO.File.Copy(sourcePath, targetPath, true);
                    result = true;
                }
            }
            catch (Exception ex)
            {
                WriteToLog(ex.Message, true);
            }

            return result;
        }

        public static string FixSongName(string songName)
        {
            string result = "";
            ///string chars ="abcdefghijklmnopqrstuvwxyz";


            songName = songName.ToLower();
            songName = songName.Substring(0, 1).ToUpper() + songName.Substring(1);

            for (int i = 0; i < songName.Length; i++)
            {
                string letter = songName.Substring(i, 1);

                // After Space char - Uppercas letter
                if (i < songName.Length - 1 && letter == " " && songName.Substring(i + 1, 1) != " " && songName.Substring(i + 1, 1) != "-")
                {
                    result += letter + songName.Substring(i + 1, 1).ToUpper();
                    i++;
                }
                else if (i > 0 && letter != "-" && (songName.Substring(i - 1, 1) == " " || songName.Substring(i - 1, 1) == "-"))
                {
                    result += letter.ToUpper();
                }
                // When '-', make space before and after him
                else if (letter == "-")
                {
                    if (i > 0 && songName.Substring(i - 1, 1) != " ")
                    {
                        result += " " + letter;
                    }
                    else if (i < songName.Length - 1 && songName.Substring(i + 1, 1) != " ")
                    {
                        result += letter + " ";
                    }
                    else
                    {
                        result += letter;
                    }
                }
                else
                {
                    result += letter;
                }
            }

            return result;
        }

        public static Drawable GetImageDrawableFromUrl(string urlAddress)
        {
            Drawable drawable = null;

            ImageView imageView = GetImageViewFromhUrl(urlAddress);

            if (imageView != null)
            {
                drawable = imageView.Drawable;
            }

            //imageView = new ImageView(Application.Context);
            //string posterLink = "http:" + urlAddress;   //weather.getPoster();
            //new Utills.DownloadImageTask(imageView).execute(posterLink);

            return drawable;
        }

        public static ImageView GetImageViewFromhUrl(string urlAddress)
        {
            ImageView imageView = null;

            try
            {
                imageView = new ImageView(ClientContext);
                Android.Net.Uri uri = Android.Net.Uri.Parse(urlAddress);
                imageView.SetImageURI(uri);
            }
            catch (Exception ex)
            {

            }

            //imageView = new ImageView(Application.Context);
            //string posterLink = "http:" + urlAddress;   //weather.getPoster();
            //new Utills.DownloadImageTask(imageView).execute(posterLink);
            //Android.Graphics.Drawables.Drawable drawable = Utils.LoadImageFromWebOperations(urlAddress);
            //imageView.SetBackgroundDrawable(drawable);

            return imageView;
        }

        public static Drawable GetDrawableFrom(string urlAddress)
        {
            Drawable drawable = null;


            try
            {
                drawable = Drawable.CreateFromPath(urlAddress);
                //Stream stream = (Stream)new Uri(urlAddress).getContent();
                //drawable = Drawable.CreateFromStream(stream, "src name");
                //Android.Graphics.Drawables.Drawable drawable = Utils.LoadImageFromWebOperations(urlAddress);
                //imageView.SetBackgroundDrawable(drawable);
            }
            catch (Exception ex)
            {
                Utils.WriteToLog(ex.Message);
            }

            return drawable;
        }

        public static Android.Graphics.Bitmap GetBitmapFromUrl(string urlAddress)
        {
            Android.Graphics.Bitmap bmp = null;

            try
            {
                Uri aURL = new Uri(urlAddress);
                Java.Net.URLConnection conn = null;     // aURL.openConnection();
                conn.Connect();
                Stream stream = conn.InputStream;
                BufferedInputStream bis = new BufferedInputStream(stream);
                bmp = Android.Graphics.BitmapFactory.DecodeStream(stream);
                bis.Close();
                stream.Close();
            }
            catch (Exception e)
            {
                //Log.e("app", "Error getting bitmap", e);
            }

            return bmp;
        }

        public static string GetHttpRequest(string urlAdddress)
        {
            string responseObj = "";
            Uri url = null;
            StreamReader readStream = null;


            try
            {
                url = new Uri(urlAdddress);

                // Specify the URL to receive the request.
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                // Set some reasonable limits on resources used by this request
                request.MaximumAutomaticRedirections = 4;
                request.MaximumResponseHeadersLength = 4;
                // Set credentials to use for this request.
                request.Credentials = CredentialCache.DefaultCredentials;

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                //Console.WriteLine("Content length is {0}", response.ContentLength);
                //Console.WriteLine("Content type is {0}", response.ContentType);

                // Get the stream associated with the response.
                Stream receiveStream = response.GetResponseStream();

                // Pipes the stream to a higher level stream reader with the required encoding format. 
                readStream = new StreamReader(receiveStream, Encoding.UTF8);

                responseObj = readStream.ReadToEnd();

                response.Close();
                readStream.Close();

                //{ "request":{ "type":"City","query":"Tel Aviv-Yafo, Israel","language":"en","unit":"m"},
                //"location":{ "name":"Tel Aviv-Yafo","country":"Israel","region":"Tel Aviv","lat":"32.068","lon":"34.765",
                //"timezone_id":"Asia\/Jerusalem","localtime":"2020-03-02 19:30","localtime_epoch":1583177400,"utc_offset":"2.0"},
                //"current":{ "observation_time":"05:30 PM","temperature":17,"weather_code":116,"weather_icons":["https:\/\/assets.weatherstack.com\/images\/wsymbols01_png_64\/wsymbol_0004_black_low_cloud.png"],
                //"weather_descriptions":["Partly cloudy"],"wind_speed":10,"wind_degree":311,"wind_dir":"NW","pressure":1021,"precip":0,"humidity":60,"cloudcover":7,"feelslike":17,"uv_index":1,"visibility":10,"is_day":"no"}}

            }
            catch (Exception ex)
            {
                WriteToLog("Can't reach site: " + url.AbsoluteUri + "\n" + "Error: " + ex.Message);
            }

            return responseObj;
        }


        public static List<XmlItem> OpenXmlData(string urlAddress)
        {
            List<XmlItem> result = new List<XmlItem>();




            try
            {
                //StreamReader streamReader = GetHttpRequest(urlAddress);
                //string responseObj = streamReader.ReadToEnd();
                string responseData = GetHttpRequest(urlAddress);
                //xmlReader = XmlReader.Create(streamReader);
                string name = "NewXmlRss.Xml";
                string fileFullName = Path.Combine(LOG_FILE_PATH, name);

                FileStream file = new FileStream(fileFullName, FileMode.Create, FileAccess.Write);
                byte[] bytesData = Encoding.UTF8.GetBytes(responseData);        // Encoding.UTF8..ASCII.GetBytes(data);  // encoding="ISO-8859-1
                file.Write(bytesData);
                file.Close();

                result = ReadXmlDocument(fileFullName);
            }
            catch (Exception ex)
            {
                WriteToLog("Error in 'OpenXmlData - " + ex.Message);
                return result;
            }


            //XmlReader xmlReader = null;
            //XmlReaderResourceParser xmlReaderResourceParser = new XmlReaderResourceParser(xmlReader);
            //xmlReader.MoveToFirstAttribute();

            //while (!xmlReader.Read())
            //{
            //    //xmlReader.MoveToElement();
            //    XmlItem xmlItem = new XmlItem();

            //    //xmlReader.MoveToNextAttribute();
            //    itemData = xmlReader.GetAttribute("title");
            //    xmlItem.Title = itemData;

            //    //xmlReader.MoveToNextAttribute();
            //    itemData = xmlReader.GetAttribute("description");
            //    xmlItem.Description = itemData;

            //    //xmlReader.MoveToNextAttribute();
            //    itemData = xmlReader.GetAttribute("pubDate");
            //    xmlItem.PublishDateString = itemData;

            //    //xmlReader.MoveToNextAttribute();
            //    itemData = xmlReader.GetAttribute("link");      // link_url
            //    xmlItem.Link = itemData;

            //    //xmlReader.MoveToNextAttribute();
            //    itemData = xmlReader.GetAttribute("image_url"); // image
            //    xmlItem.Link = itemData;


            //    result.Add(xmlItem);
            //}


            return result;
        }

        public static List<XmlItem> ReadXmlDocument(string fileFullPath)
        {
            List<XmlItem> result = new List<XmlItem>();

            try
            {
                XmlDocument doc = new XmlDocument();

                doc.Load(fileFullPath);


                // get all <item> nodes to a list
                string itemKeyName = "item";
                XmlNodeList nodelist = doc.SelectNodes(itemKeyName);
                nodelist = doc.GetElementsByTagName(itemKeyName);
                //nodelist = doc.DocumentElement.ChildNodes[2]

                XmlElement xmlElement2 = doc.DocumentElement;
                nodelist = xmlElement2.ChildNodes;


                foreach (XmlNode node in nodelist) // for each <testcase> node
                {
                    XmlItem xmlItem = new XmlItem();

                    try
                    {
                        if (node.ChildNodes.Count > 1)
                        {
                            if (node.SelectSingleNode("title") != null)
                            {
                                xmlItem.Title = node.SelectSingleNode("title").InnerText.Trim();
                            }
                            if (node.SelectSingleNode("description") != null)
                            {
                                xmlItem.Description = node.SelectSingleNode("description").InnerText.Trim();
                            }
                            if (node.SelectSingleNode("pubDate") != null)
                            {
                                string dateString = node.SelectSingleNode("pubDate").InnerText.Trim();
                                if (!string.IsNullOrEmpty(dateString))
                                {
                                    dateString = dateString.Replace("T", " ");
                                    dateString = dateString.Replace("Z", " ");
                                    xmlItem.PublishDateString = dateString;
                                    string[] dateParts = dateString.Split("-");
                                    if (dateParts != null && dateParts.Length > 1)
                                    {
                                        try
                                        {
                                            DateTime date;
                                            int year, month, day, hours, minutes;
                                            Int32.TryParse(dateParts[0], out year);
                                            Int32.TryParse(dateParts[1], out month);
                                            string dayStr = dateParts[2];
                                            dayStr = dayStr.Substring(0, dayStr.IndexOf(" "));
                                            Int32.TryParse(dayStr, out day);
                                            hours = Int32.Parse(dateString.Substring(11, 2));
                                            minutes = Int32.Parse(dateString.Substring(14, 2));
                                            date = new DateTime(year, month, day, hours, minutes, 0);
                                            xmlItem.PublishDate = date;
                                        }
                                        catch
                                        {

                                        }
                                    }
                                }
                            }
                            if (node.SelectSingleNode("link_url") != null)
                            {
                                xmlItem.Link = node.SelectSingleNode("link_url").InnerText.Trim();     //link
                            }
                            if (node.SelectSingleNode("image_url") != null)
                            {
                                xmlItem.Image = node.SelectSingleNode("image_url").InnerText.Trim();   // image
                            }

                            result.Add(xmlItem);
                        }
                    }
                    catch (Exception ex)
                    {
                        //WriteToLog("Error in reading XML - " + ex.Message);
                    }

                }
            }
            catch (Exception ex)
            {

            }


            //xmlItem.Title = node.Attributes.GetNamedItem("title").Value;
            //xmlItem.Description= node.Attributes.GetNamedItem("description").Value;
            //xmlItem.PublishDateString = node.Attributes.GetNamedItem("pubDate").Value;
            //xmlItem.Link = node.Attributes.GetNamedItem("link_url").Value;       //link_url
            //xmlItem.Image = node.Attributes.GetNamedItem("image_url").Value;  // image
            //date = (string)tc.Element("date"),
            //name = (string)tc.Element("name"),
            //sub = (string)tc.Element("subject")
            //XmlElement xmlElement = node.SelectSingleNode("title").InnerText;
            //doc.DocumentElement.GetAttributeNode("description");
            //string value = doc.DocumentElement.GetAttribute(itemKeyName);


            return result;
        }

        public static List<string> TextFileRead(string fileName, bool showMessage = true)
        {
            string line = null;
            string file = "";
            List<string> resultList = new List<string>();



            try
            {
                KeyValuePair<string, int>[] perm = new KeyValuePair<string, int>[] { new KeyValuePair<string, int>(Manifest.Permission.ReadExternalStorage, PERMISSIONS_REQUEST_READ_STORAGE) };
                PermissionAsk(perm);

                List<string> files = Directory.GetFiles(Directory.GetParent(fileName).FullName).ToList();       //, fileExtentionToSearch).ToList();

                //file = new File(fileName);
                if (files.Count > 0)
                {
                    file = files[0];
                }

                if (files.IndexOf(fileName) == -1)
                {
                    string message = "קובץ טקסט לא נמצא בדיסק" + " - " + fileName;
                    Toast.MakeText(ClientContext, message, ToastLength.Long).Show();
                    return null;
                }

                //UTF8Encoding encoding = new UTF8Encoding();
                Encoding encoding = new UTF8Encoding(false);
                //Encoding encoding = Encoding.GetEncoding("Windows-1255");

                StringBuilder stringBuilder = new StringBuilder();

                StreamReader fileInputStream = new StreamReader(fileName, encoding);
                //Java.IO.FileInputStream fileInputStream = new Java.IO.FileInputStream(fileName);
                //Java.IO.InputStreamReader inputStreamReader = new Java.IO.InputStreamReader(fileInputStream);
                //Java.IO.BufferedReader bufferedReader = new Java.IO.BufferedReader(inputStreamReader);


                //while ((line = bufferedReader.ReadLine()) != null)
                //{
                //    stringBuilder.Append(line + LINE_SEPARATOR);
                //    resultList.Add(line);
                //}

                //fileInputStream.close();
                //bufferedReader.close();



                resultList = new List<string>();

                while (!fileInputStream.EndOfStream)
                {
                    line = fileInputStream.ReadLine();

                    //byte[] bytes = Encoding.ASCII.GetBytes(line + LINE_SEPERATOR);       // "šarže");
                    //char[] chars = Encoding.ASCII.GetChars(bytes);
                    //line = Encoding.ASCII.GetString(bytes);

                    //stringBuilder.Append(chars);
                    stringBuilder.Append(line + LINE_SEPERATOR);
                    resultList.Add(line);
                }

                fileInputStream.Close();
                //bufferedReader.Close();
                //TODO: inputStreamReader.close();

                //lines = (String[])resultList.toArray();
                //line = stringBuilder.toString();

                if (showMessage)
                {
                    Toast.MakeText(ClientContext, "קריאה מקובץ טקסט עבר בהצלחה", ToastLength.Long).Show();
                }
            }
            catch (Exception ex)     //  FileNotFoundException ex)
            {
                Toast.MakeText(ClientContext, "קריאה מקובץ טקסט נכשל" + " - " + ex.Message, ToastLength.Long).Show();
                WriteToLog("קריאה מקובץ טקסט נכשל" + " - " + ex.Message);
                resultList = null;
            }

            return resultList;
        }

        public static bool TextFileSave(string fileName, string data, bool appendLines, bool showMessage)
        {
            bool result = false;




            try
            {
                KeyValuePair<string, int>[] perm = new KeyValuePair<string, int>[] { 
                                                                                      new KeyValuePair<string, int>(Manifest.Permission.WriteExternalStorage, PERMISSIONS_REQUEST_WRITE_STORAGE),
                                                                                      new KeyValuePair<string, int>(Manifest.Permission.ReadExternalStorage, PERMISSIONS_REQUEST_READ_STORAGE)
                                                                                   };
                PermissionAsk(perm);

                if (!Directory.GetParent(fileName).Exists)
                {
                    Toast.MakeText(ClientContext, "saveToFile - Directory not exist. Create Directory", ToastLength.Short).Show();
                    Directory.CreateDirectory(Directory.GetParent(fileName).Name);
                }

                data = data + LINE_SEPERATOR;

                //UTF8Encoding encoding = new UTF8Encoding();
                Encoding encoding = new UTF8Encoding(false);
                //Encoding encoding = Encoding.GetEncoding("Windows-1255");
                byte[] bytes = Encoding.ASCII.GetBytes(data);       // "šarže");
                char[] chars = Encoding.ASCII.GetChars(bytes);

                //Java.IO.FileOutputStream fileOutputStream = new Java.IO.FileOutputStream(fileName, appendLines);
                //fileOutputStream.Write(bytes);

                StreamWriter fileOutputStream = new StreamWriter(fileName, appendLines, encoding);
                fileOutputStream.Write(data);
                //fileOutputStream.Write(chars);

                if (showMessage)
                {
                    Toast.MakeText(ClientContext, "שמירה לקובץ" + " '" + fileName + "' " + "עברה בהצלחה", ToastLength.Long).Show();
                }

                fileOutputStream.Close();

                result = true;
            }
            catch (Exception ex)
            {
                Toast.MakeText(ClientContext, "Error in saveToFile" + ex.Message, ToastLength.Long).Show();
                WriteToLog("Error in saveToFile(): " + fileName + "\n" + ex.Message);
            }

            return result;
        }

        public static bool IsActivityRunning(Intent intent)
        {
            ComponentName componentName = intent.Component;
            Context context = ClientContext;  // ApplicationContext;
            // (Context)InputIntent.ResolveActivity(ApplicationContext.PackageManager);
            string clsName = componentName.ClassName;
            //Java.Lang.Class @class = InputIntent.Class;


            return IsActivityRunning(context);
        }

        [Obsolete]
        public static bool IsActivityRunning(Context context)
        {
            bool result = false;
            ActivityManager activityManager = (ActivityManager)context.GetSystemService(Context.ActivityService);
            IList<ActivityManager.RunningTaskInfo> tasks = activityManager.GetRunningTasks(int.MaxValue);


            foreach (ActivityManager.RunningTaskInfo task in tasks)
            {
                // ApplicationContext.ApplicationInfo.ProcessName  "com.meirhemed.projtaskreminder"
                // ApplicationContext.ApplicationInfo.TaskAffinity	"com.meirhemed.projtaskreminder"	
                // ApplicationContext.ApplicationInfo.DataDir	"/data/user/0/com.meirhemed.projtaskreminder"	
                // ApplicationContext.ApplicationInfo.PackageName  "com.meirhemed.projtaskreminder"
                // ApplicationContext.ApplicationInfo	{ApplicationInfo{7db657c com.meirhemed.projtaskreminder}}	Android.Content.PM.ApplicationInfo

                if (context.PackageName.Equals(task.BaseActivity.PackageName))
                // if (componentName.PackageName.Equals(task.BaseActivity.PackageName))
                {
                    result = true;
                    break;
                }
            }


            Activity activity = new Activity();
            // Activity is at least partially visible
            //getLifecycle().getCurrentState().isAtLeast(STARTED)
            //Activity is in the foreground
            //getLifecycle().getCurrentState().isAtLeast(RESUMED)

            if (activity.Window.DecorView.RootView.IsShown)
            {

            }

            // if(!activity.isFinishing() && !activity.isDestroyed())
            // if (MainActivity.mainActivityIsOpen() == true)


            //foreach (RunningTaskInfo task in tasks)
            //{
            //    if (ctx.PackageManager.Equals(task.BaseActivity.PackageName))
            //    //if (ctx.PackageManager.EqualsIgnoreCase(task.BaseActivity.PackageName()))
            //    {
            //        return true;
            //    }
            //}


            //for (int i = 0; i < tasks.Count; i++)
            //{

            //    if (Constants.PACKAGE.equals(tasks[i].PackageName))
            //    {

            //        if (Constants.BACKGROUND_SERVICE_CLASS.equals(services.get(i).service.getClassName()))
            //        {
            //            isServiceFound = true;
            //        }
            //    }
            //}

            return result;
        }

        [Obsolete]
        public static bool isActivityRunning2(Java.Lang.Class activityClass)
        {
            ActivityManager activityManager = (ActivityManager)ClientContext.GetSystemService(Context.ActivityService);
            List<ActivityManager.RunningTaskInfo> tasks = (List<RunningTaskInfo>)activityManager.GetRunningTasks(int.MaxValue);

            foreach (RunningTaskInfo task in tasks)
            {
                if (activityClass.CanonicalName.Equals(task.BaseActivity.ClassName))
                {
                    return true;
                }
            }

            return false;
        }


        [Obsolete]
        public static bool IsServiceRunning(Context ctx)
        {
            ActivityManager activityManager = (ActivityManager)ctx.GetSystemService(Context.BluetoothService);
            List<RunningServiceInfo> tasks = (List<RunningServiceInfo>)activityManager.GetRunningServices(int.MaxValue);

            foreach (RunningServiceInfo task in tasks)
            {
                if (ctx.PackageManager.Equals(task.ClientPackage))
                //if (ctx.PackageManager.Equals(task.PackageName))
                //if (ctx.PackageManager.Equals(task.Process))
                //if (ctx.PackageManager.Equals(task.Service))
                //if (ctx.PackageManager.EqualsIgnoreCase(task.BaseActivity.PackageName()))
                {
                    return true;
                }
            }

            return false;
        }


    }





    public class XmlItem
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string PublishDateString { get; set; }
        public DateTime PublishDate { get; set; }
        public string Link { get; set; }
        public string Image { get; set; }


        public XmlItem()
        {

        }
    }

}