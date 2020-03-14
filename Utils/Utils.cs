using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Xml;
using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Java.IO;
//using Java.Net;

namespace ProjTaskReminder.Utils
{

    public static class Utils
    {
        public const string             LINE_SEPERATOR = "\n";
        public static string            LOG_FILE_PATH;
        public static string            LOG_FILE_NAME;

        public static string            NEWS_RSS_ADDRESS1 = " http://www.ynet.co.il/Integration/StoryRss1854.xml";
        public static string            NEWS_RSS_ADDRESS2 = " https://www.kan.org.il/rss/allnews.ashx";
        public static string            NEWS_RSS_ADDRESS3 = " http://rss.walla.co.il/feed/22";


        public static List<KeyValuePair<string, List<string>>> FilesExtra;

        public static Context context;
        public static Activity activity;


        public static void WriteToLog(string data, bool appendLines = true)
        {
            string fileName = string.Empty;
            byte[] bytesData;
            FileStream file;
            string folderBackup = "";




            data = LINE_SEPERATOR + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + LINE_SEPERATOR + data + LINE_SEPERATOR;
            Context context = Android.App.Application.Context;


            try
            {
                // Dne in ;Client; source that use these utils services
                //LOG_FILE_NAME = "LogTaskReminder.txt";
                //LOG_FILE_PATH = context.GetExternalFilesDir("").AbsolutePath;
                //LOG_FILE_PATH = Android.OS.Environment.GetExternalStoragePublicDirectory(folderBackup).AbsolutePath;
                //LOG_FILE_PATH = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;

                fileName = Path.Combine(LOG_FILE_PATH, LOG_FILE_NAME);

                try
                {
                    file = new FileStream(fileName, ((appendLines)?FileMode.Append: FileMode.Create), FileAccess.Write);
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
                    file = new FileStream(fileName, ((appendLines) ? FileMode.Append : FileMode.Create), FileAccess.Write);
                }

                bytesData = Encoding.UTF8.GetBytes(data);        // Encoding.UTF8..ASCII.GetBytes(data);  // encoding="ISO-8859-1

                file.Write(bytesData);

                file.Close();
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
        public static List<string> GetFolderFiles(string path, string fileExtentionToSearch, bool searchInFolders, string fileExtentionToSearch2="")
        {
            string file;
            List<string> files = new List<string>();
            List<KeyValuePair<string, List<string>>> filesResult = new List<KeyValuePair<string, List<string>>>();
            List<string> directories = new List<string>(); ; 




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
                return files;
            }

            //        if (fileExtentionToSearch.contains(";"))
            //        {
            //            String[] multiSearchValuse = fileExtentionToSearch.split(";");
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

                    List<string> filesPicts = new List<string>();

                    if (!fileExtentionToSearch2.Equals(""))
                    {
                        path = Directory.GetParent(file).FullName;
                        filesPicts = Directory.GetFiles(path, fileExtentionToSearch2).ToList();
                    }

                    //if (Directory.Exists(file))
                    //{
                    //    dirs = GetFolderFiles(Directory.GetParent(file).FullName, fileExtentionToSearch2, searchInFolders, "");
                    //}

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
            string chars ="abcdefghijklmnopqrstuvwxyz";

            if (chars.Contains(songName.Substring(0, 1)))
            {
                songName = songName.Substring(0, 1).ToUpper() + songName.Substring(1);
            }
            if (songName.Contains("- "))
            {
                int pos = songName.IndexOf("- ");
                if (pos + 3 < songName.Length - 1)
                {
                    int len = songName.Length;
                    songName = songName.Substring(0, pos + 2) + songName.Substring(pos + 2, len-pos-2).ToUpper();
                }
            }

            return songName;
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
                imageView = new ImageView(context);
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
                string responseData= GetHttpRequest(urlAddress);
                //xmlReader = XmlReader.Create(streamReader);
                string name = "NewXmlRss.Xml";
                string fileFullName= Path.Combine(LOG_FILE_PATH, name);

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
            //    xmlItem.PublishDate = itemData;

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
                string itemKeyName = "item";        // "/item"
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
                        //date = (string)tc.Element("date"),
                        //name = (string)tc.Element("name"),
                        //sub = (string)tc.Element("subject")

                        //XmlElement xmlElement = node.SelectSingleNode("title").InnerText;
                        //doc.DocumentElement.GetAttributeNode("description");
                        //string value = doc.DocumentElement.GetAttribute(itemKeyName);
                        
                        if (node.Name== itemKeyName && node.SelectSingleNode("title")!=null) //node.Attributes.Count>0 &&
                        {
                            //xmlItem.Title = node.Attributes.GetNamedItem("title").Value;
                            //xmlItem.Description= node.Attributes.GetNamedItem("description").Value;
                            //xmlItem.PublishDate = node.Attributes.GetNamedItem("pubDate").Value;
                            //xmlItem.Link = node.Attributes.GetNamedItem("link_url").Value;       //link_url
                            //xmlItem.Image = node.Attributes.GetNamedItem("image_url").Value;  // image
                            xmlItem.Title = node.SelectSingleNode("title").InnerText;
                            xmlItem.Description = node.SelectSingleNode("description").InnerText;
                            xmlItem.PublishDate = node.SelectSingleNode("pubDate").InnerText;
                            xmlItem.Link = node.SelectSingleNode("link_url").InnerText;     //link
                            xmlItem.Image = node.SelectSingleNode("image_url").InnerText;

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


            return result;
        }
        public class XmlItem
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public string PublishDate { get; set; }
            public string Link { get; set; }
            public string Image { get; set; }


            public XmlItem()
            {

            }
        }
    }
}