using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using SQLite;
using Android.Content;
using Android.Support.V7.Widget;
using System.Timers;
//using System.Threading.Timer;
//using System.Threading;
using System.IO;
//using Java.Util;
using Android.Text;

using ProjTaskReminder.Model;
using ProjTaskRemindet.Utils;
using ProjTaskReminder.Data;
using ProjTaskReminder.Utils;
using Android.Graphics.Drawables;
using System.Text;
using Android;
using MH_Utils;
using static MH_Utils.Utils;

namespace ProjTaskReminder
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true, Name = "com.meirhemed.projtaskreminder.mainactivity")]
    public class MainActivity : AppCompatActivity   //, ListView.IOnItemClickListener
    {

        public static string DB_TASK_DB_NAME = "TaskReminder.db";
        public static string DB_TASK_DB_PATH = "TaskReminder.db";
        public static string DB_TABLE_NAME = "TBL_Tasks";
        public static string DB_TABLE_SETTING = "TBL_Settings";
        private static string DB_FIELDNAME_ID = "TaskID";
        private static string DB_FIELDNAME_TITLE = "Title";
        private static string DB_FIELDNAME_DESC = "Description";
        private static string DB_FIELDNAME_DATE = "DateDue";
        private static string DB_FIELDNAME_REPEAT = "Repeat";
        private static string DB_FIELDNAME_COLOR = "CardBackColor";
        private static string DB_FIELDNAME_LAST_UPDATE = "LastUpdateDate";
        private static string DB_FIELDNAME_IS_ARCHIVE = "IsArchive";

        // The code for get result from TasDetails screen
        public const string WRITE_LIST_FILE_NAME = "MH_Tasks.txt";
        public const int SCREEN_TASK_DETAILS_SAVED = 1234;
        public const int SCREEN_TASK_DETAILS_DELETE = 9999;
        public const int SHOW_SCREEN_SETTING = 9998;
        private const string CARD_SEPERATOR = "**----------**";
        private const string WRITE_DB_PREFIX_ID = "id: ";
        private const string WRITE_DB_PREFIX_TITLE = "title: ";
        private const string WRITE_DB_PREFIX_DESC = "desc: ";
        private const string WRITE_DB_PREFIX_DATE = "date: ";
        private const string WRITE_DB_PREFIX_TIME = "time: ";
        private const string WRITE_DB_PREFIX_REPEAT = "repeat: ";
        private const string WRITE_DB_PREFIX_LAST_UPDATE = "last_update: ";
        private const string WRITE_DB_PREFIX_BACKGROUND_COLOR = "backcolor: ";

        static readonly string TAG = typeof(MainActivity).FullName;
        static readonly string SERVICE_STARTED_KEY = "has_service_been_started";

        private const int PERMISSIONS_REQUEST_READ_STORAGE = 111;
        private const int PERMISSIONS_REQUEST_WRITE_STORAGE = 222;


        private ListView lstTasks;    //RecyclerView
        private ListViewAdapter listViewAdapter;
        public static DBTaskReminder DBTaskReminder;
        private static List<Task> TasksList = new List<Task>();
        private static List<Task> TasksListBackup = new List<Task>();
        private List<Weather> WeatherList = new List<Weather>();
        private int WeatherCounter = 0;
        private MH_Weather mH_Weather;
        private MH_Scroll WeatherScroll;
        private MH_Scroll NewsScroll;
        private TextView txtRssNews;
        private HorizontalScrollView scrollWeather;
        private HorizontalScrollView scrollNews;
        private System.Threading.Thread ThreadNews;
        private ImageView imageTimerPointWeater;


        private static Context MainContext;
        //private readonly string[]  countryList = new string[6] { "India", "China", "australia", "Portugle", "America", "NewZealand" };

        public static Task CurrentTask;
        public static List<object[]> TimersArray;
        public static string MainMessageText;
        public static int DefaultCardBackgroundColor;
        public static bool isShowTimerReminder;
        private int TimerInterval;
        private MH_Notification mh_Notification;
        private Intent IntentMusic = null;


        private Android.Support.V4.App.NotificationCompat.Builder notificationBuilder;
        private TimerService TaskTimerService;     // , ElapsedEventHandler
        private delegate AdapterView.IOnItemClickListener lstTasks_OnItemClick3();

        private bool isServiceStarted;
        private  Intent ServiceKeppAliveIntent;
        private Bundle InputSavedInstanceState { get; set; }
        private MHServiceKeepAlive.ServiceStayAlive serviceStayAlive { get; set; }

        private MH_SearchDialog             mh_SearchDialog { get; set; }

        //public delegate ElapsedEventHandler delegateMethod(object timer, ElapsedEventArgs args, Task TaskObject);



        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);


            try
            {
                InputSavedInstanceState = savedInstanceState;

                MainContext = this.ApplicationContext;

                Initialize();

                ConnectToDB();

                //BackupDataBaseFile(false);

                FillListFromDB();

                focusListOnToday(MH_Utils.Utils.GetDateNow());


                ServiceKeepAliveHandle(InputSavedInstanceState);


                //StartRssNewsScroll();

                // First city
                //Weather weather;
                //weather = mH_Weather.GetWather("Ashdod");
                //WeatherList.Add(weather);
                //OnWeatherChangingPlace(weather, 0);

                //WeatherScroll.StartPosstion();
            }
            catch (Exception ex)
            {
                MH_Utils.Utils.WriteToLog(ex);
            }
        }

        private void ServiceKeepAliveHandle(Bundle savedInstanceState)
        {

            // When the app will die, the service will ReStart this activity again, Get the indication thT SERVICE START IT AGAIN
            if (savedInstanceState != null)
            {
                isServiceStarted = savedInstanceState.GetBoolean(SERVICE_STARTED_KEY, false);
            }


            ServiceKeppAliveIntent = new Intent(this, typeof(MHServiceKeepAlive.ServiceStayAlive));
            //ServiceKeppAliveIntent = new Intent(MainContext, typeof(MHServiceKeepAlive.ServiceStayAlive));
            ServiceKeppAliveIntent.SetFlags(ActivityFlags.NewTask);

            // Just tto use same constants values
            serviceStayAlive = new MHServiceKeepAlive.ServiceStayAlive();

            Bundle bundle = new Bundle();
            bundle.PutString(serviceStayAlive.INTENT_KEY_CALLER_PACKAGE_NAME, this.PackageName);
            bundle.PutString(serviceStayAlive.INTENT_KEY_CALLER_COMPONENT_ACTIVITY_NAME, this.ComponentName.ClassName);     // "com.meirhemed.executeanotheractivity.mainactivity"
            bundle.PutBoolean(serviceStayAlive.INTENT_KEY_CALLER_IS_RESTART_ON_START, true);
            bundle.PutBoolean(serviceStayAlive.INTENT_KEY_CALLER_IS_RESTART_ON_DESTROY, false);
            bundle.PutString(serviceStayAlive.INTENT_KEY_CALLER_LOG_FILE_PATH, MainContext.GetExternalFilesDir("").AbsolutePath);

            ServiceKeppAliveIntent.PutExtra(serviceStayAlive.INTENT_KEY_CALLER_BUNDLE, bundle);

            //serviceStayAlive.OnCreate();
            //serviceStayAlive.OnStartCommand(null, StartCommandFlags.Retry, 0);
            serviceStayAlive = null;

            StartService(ServiceKeppAliveIntent);

            //intent = new Intent(Intent.ActionMain);     // Intent.ActionMain - "android.intent.action.MAIN"
            //intent.SetComponent(new ComponentName("com.meirhemed.intentedactivity", "com.meirhemed.intentedactivity.mainactivity"));
            //StartActivity(intent);

            // Put in the client taht call that servic - Provide the package name and the name of the service with a ComponentName object.
            //ComponentName cn = new ComponentName("com.meirhemed.intentedactivity", "com.meirhemed.intentedactivity.mainactivity");
            //Intent serviceToStart = new Intent();
            //serviceToStart.SetComponent(cn);

            //var intent = new Intent(ApplicationContext, typeof(PostService));
            //var source = PendingIntent.GetBroadcast(ApplicationContext, 0, intent, 0);

            //Android.Net.Uri uri = Android.Net.Uri.Parse("com.meirhemed.mhServiceKeepAlive.mhServiceStayAlive");     // callerName
            //ServiceKeppAliveIntent = new Intent(Intent.ActionCall, uri);


        }

        public void StartRssNewsScroll(object sender, EventArgs e)
        {
            txtRssNews.Text = "טוען מידע ...";
            imageTimerPointWeater.Visibility = ViewStates.Visible;
            imageTimerPointWeater.BringToFront();

            ThreadNews = new System.Threading.Thread(new System.Threading.ThreadStart(StartGetNews));
            //ThreadNews.IsBackground = true;

            ThreadNews.Start();

        }

        private void StartGetNews()
        {

            this.RunOnUiThread(GetRssNewsScroll);

        }

        [Obsolete]
        private void GetRssNewsScroll()
        {
            List<MH_Utils.XmlItem> items;
            DateTime nowDate;



            items = MH_Utils.Utils.OpenXmlData(MH_Utils.Utils.NEWS_RSS_ADDRESS2);

            nowDate = MH_Utils.Utils.GetDateNow().Date;

            if (items.Count > 0)
            {
                items = items.Where(a => a.PublishDate.Date.CompareTo(nowDate) == 0).ToList();
            }

            items = items.OrderByDescending(a => a.PublishDate).ToList();

            string news = string.Empty;
            string pubDate = string.Empty;

            news = "<html><p dir=\"rtl" + "\">";
            string newsFlate = "";

            for (int i = items.Count - 1; i >= 0; i--)
            {
                MH_Utils.XmlItem newsItem = items[i];

                if (newsItem.PublishDate.Date.CompareTo(nowDate) == 0)
                {
                    pubDate = newsItem.PublishDate.ToString("HH:mm");
                }
                else
                {
                    pubDate = newsItem.PublishDate.ToString("dd/MM/yyyy HH:mm");
                }
                //pubDate = newsItem.PublishDateString;

                string str = "<b>" + pubDate + "</b>" + " - " + "<span style=\"color:blue" + "\">" + newsItem.Title + "</span>" + "  ***  ";    // + "</p>";
                news += str;

                newsFlate += pubDate + " - " +
                             newsItem.Title + "  ***  ";
                //+ ", " + newsItem.Description + ".  ";
            }

            news += "</p></html>";
            txtRssNews.SetText(Html.FromHtml(news), TextView.BufferType.Spannable);
            //txtRssNews.Text = news;
            //imageTimerPointWeater.Visibility = ViewStates.Invisible;

            ThreadNews.Abort();
            ThreadNews = null;

            NewsScroll.SCROLL_END_POINT = newsFlate.Length * 20;      // 12000;     // txtRssNews.Width-500;
            NewsScroll.Start();
        }

        /// <summary>
        /// Fill controls in the task card - From the adapter
        /// </summary>
        /// <param name="listViewHolder"></param>
        /// <param name="position"></param>
        private void SetTaskItemControlsInView(ListViewAdapter.ListViewHolder listViewHolder, int position)
        {
            if (position >= TasksList.Count || position < 0)
            {
                return;
            }



            try
            {
                Task task = TasksList[position];

                listViewHolder.FirstLine.SetText(task.getTitle(), TextView.BufferType.Normal);
                listViewHolder.SecondLine.SetText(task.getDescriptionWithHtml(), TextView.BufferType.Normal);

                if (!task.getDate_due().Equals(""))
                {
                    listViewHolder.ThirdLine.SetText(task.getDate_due() + "  " + task.getTime_due() + " יום " + MH_Utils.Utils.getDateDayName(task.getDate().Value), TextView.BufferType.Normal);
                }
                else
                {
                    listViewHolder.ThirdLine.SetText(task.getDate_last_update() + " יום " + MH_Utils.Utils.getDateDayName(MH_Utils.Utils.getDateFromString(task.getDate_last_update())), TextView.BufferType.Normal);
                }

                // Card backcolor
                Android.Graphics.Color colorDefault = new Android.Graphics.Color(MainContext.GetColor(Resource.Color.CardBackgroundColor));

                if (!string.IsNullOrEmpty(task.getBackgroundColor()))
                {
                    int colorInt = int.Parse(task.getBackgroundColor().Trim());
                    Android.Graphics.Color color = new Android.Graphics.Color(colorInt);
                    if (colorDefault != color)
                    {
                        listViewHolder.cardView.SetBackgroundColor(color);
                    }
                    else
                    {
                        listViewHolder.cardView.SetBackgroundColor(colorDefault);
                    }
                }
                else
                {
                    listViewHolder.cardView.SetBackgroundColor(colorDefault);
                }

            }
            catch (Exception ex)
            {
                MH_Utils.Utils.WriteToLog("Error - SetTaskItemControlsInView():" + MH_Utils.Utils.LINE_SEPERATOR + ex.InnerException.Message);
            }

        }

        private void SetControlsIO()
        {
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            Android.Support.Design.Widget.FloatingActionButton btnMainNew = (Android.Support.Design.Widget.FloatingActionButton)FindViewById(Resource.Id.btnMainNew);
            btnMainNew.Click += btnMainNew_OnClick;
            //btnMainNew.SetBackgroundResource(Android.Resource.Drawable.IcMediaNext);

            //FloatingActionButton btnMainDelete = FindViewById<FloatingActionButton>(Resource.Id.btnMainDelete);
            //btnMainDelete.Click += btnMainDelete_OnClick;
            //btnMainDelete.SetBackgroundResource(Android.Resource.Drawable.IcDelete);


            ImageButton btnMainMusic = (ImageButton)FindViewById(Resource.Id.bntMainMusic);
            //btnMainWeather = (ImageView)FindViewById(Resource.Id.btnMainWeather);
            ImageButton btnMainEmail = (ImageButton)FindViewById(Resource.Id.btnMainEmail);
            CardView cardWeather = (CardView)FindViewById(Resource.Id.cardWeather);
            scrollWeather = (HorizontalScrollView)FindViewById(Resource.Id.scrollWeather);
            scrollNews = (HorizontalScrollView)FindViewById(Resource.Id.scrollNews);
            txtRssNews = (TextView)FindViewById(Resource.Id.txtRssNews);
            imageTimerPointWeater = (ImageView)FindViewById(Resource.Id.imageTimerPointWeater);


            lstTasks = (ListView)FindViewById(Resource.Id.lstTasks);


            ActivityTaskDetails.OnExitResult += OnExitResult;
            btnMainMusic.Click += btnMainMusic_Click;
            //btnMainWeather.Click += btnMainWeather_Click;
            btnMainEmail.Click += StartRssNewsScroll;
            txtRssNews.Click += txtRssNews_Click;


            ImageView btnMainWeather1 = (ImageView)FindViewById(Resource.Id.btnMainWeather1);
            ImageView btnMainWeather2 = (ImageView)FindViewById(Resource.Id.btnMainWeather2);
            ImageView btnMainWeather3 = (ImageView)FindViewById(Resource.Id.btnMainWeather3);
            ImageView btnMainWeather4 = (ImageView)FindViewById(Resource.Id.btnMainWeather4);
            btnMainWeather1.BringToFront();
            btnMainWeather2.BringToFront();
            btnMainWeather3.BringToFront();
            btnMainWeather4.BringToFront();
            btnMainWeather1.Click += btnMainWeather_Click;
            btnMainWeather2.Click += btnMainWeather_Click;
            btnMainWeather3.Click += btnMainWeather_Click;
            btnMainWeather4.Click += btnMainWeather_Click;

            Button btnSearch = (Button)FindViewById(Resource.Id.btnSearch);
            btnSearch.Click += btnSearch_Click;

            Button btnRefreshList = (Button)FindViewById(Resource.Id.btnRefreshList);
            btnRefreshList.Click += btnRefreshList_Click;
        }

        private void btnRefreshList_Click(object sender, EventArgs e)
        {
            FillListFromDB();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            SearchDialogInit();

            mh_SearchDialog.SearchDialogShow();
        }

        private void SearchDialogInit()
        { 
            try
            {
                mh_SearchDialog = new MH_SearchDialog(MH_SearchDialog.SearchDialogModeEN.UtilsGlobalCard, MainContext, this, null, null);       // ClientView, cardView);      


                mh_SearchDialog.OnSearchTextChanged += SearchDialogs_OnTextChanged;
                mh_SearchDialog.OnSearchFind += SearchDialogs_OnFindButton;
                mh_SearchDialog.OnSearchCancel += SearchDialogs_OnCancelButton;

                mh_SearchDialog.SetSearchDialogInit();



                // No need
                //LayoutInflater layoutInflater = LayoutInflater.From(MainContext);
                //View ClientView = layoutInflater.Inflate(Resource.Layout.search_dialog, null);
                //View ClientView = this.LayoutInflater.Inflate(Resource.Layout.search_dialog, null, true);
                //CardView cardView = (CardView)ClientView.FindViewById(Resource.Id.cardSearchDialog);
                //mh_SearchDialog.ViewResourcesID = Resource.Layout.search_dialog;
                //mh_SearchDialog.SearchDialogMode = SearchDialogModeEN.ActivityPersonalCard;
                //mh_SearchDialog.ClientCard = cardView;
                //Activity activity = MainContext as Activity;
                //View ClientView = activity.LayoutInflater.Inflate(Resource.Layout.search_dialog, null, true);
                //textureView = view.FindViewById<TextureView>(Resource.Id.textureView);
                //textureView.SurfaceTextureListener = this;
                //EditText editText = (EditText)ClientView.FindViewById(Resource.Id.txtSearchTextToFind);
                //editText.TextChanged += SearchDialog_TestTextChanged;       // new EventHandler<TextChangedEventArgs>(SearchDialog_TestTextChanged);
                //editText.AfterTextChanged += SearchDialog_AfterTextChanged;
                //editText.AfterTextChanged += (sender, args) =>
                //{
                //    //do your stuff in here
                //    //args.Editable is the equivalent of the `s` argument from Java
                //    //afterTextChanged(Editable s)
                //};           
                //editText.AddTextChangedListener(new MyTextWatcher());
                //editText.Focusable = true;
                //editText.RequestFocus();
                // Needed Just when use SearchDialogMode = ActivityPersonalCard
                //mh_SearchDialog.txtSearchTextToFind = (EditText)ClientView.FindViewById(Resource.Id.txtSearchTextToFind);
                //mh_SearchDialog.imgSearchFindIcon = (ImageView)ClientView.FindViewById(Resource.Id.imgSearchFindIcon);
                //mh_SearchDialog.imgSearchCancelIcon = (ImageView)ClientView.FindViewById(Resource.Id.imgSearchCancelIcon);
                //mh_SearchDialog.layoutMainSearchDialog = (RelativeLayout)ClientView.FindViewById(Resource.Id.layoutMainSearchDialog);

                //mh_SearchDialog.txtSearchTextToFind.SurfaceTextureListener = this;

            }
            catch (Exception ex)
            {

            }

        }

        //public class MyTextWatcher : Java.Lang.Object, ITextWatcher
        //{
        //    public void AfterTextChanged(IEditable s) 
        //    {
                
        //    }

        //    public void BeforeTextChanged(Java.Lang.ICharSequence arg0, int start, int count, int after) 
        //    {

        //    }

        //    public void OnTextChanged(Java.Lang.ICharSequence arg0, int start, int before, int count) 
        //    {

        //    }
        //}

        //private void SearchDialog_AfterTextChanged(object sender, AfterTextChangedEventArgs e)
        //{
            
        //}

        //private void SearchDialog_TestTextChanged(object sender, TextChangedEventArgs e)
        //{
            
        //}

        private void SearchDialogs_OnCancelButton(object sender, EventArgs e)
        {
            FillListFromDB();
        }

        private void SearchDialogs_OnFindButton(string finalTextToFind)
        {
            SearchDialogs_OnTextChanged(finalTextToFind);
        }

        private void SearchDialogs_OnTextChanged(string changedText)
        {
            if (!string.IsNullOrEmpty(changedText))
            {
                TasksList = RestoreTaskList();  // GetTasksFromDB();
                searchInListByText(changedText);
                FillList(false);
            }
            else
            {
                FillListFromDB();
            }
        }


        private void txtRssNews_Click(object sender, EventArgs e)
        {
            if (NewsScroll.IsScrollng)
            {
                NewsScroll.Stop();
            }
            else
            {
                NewsScroll.Start();
            }
        }

        [Obsolete]
        private void OnWeatherChangingPlace(object weatherObject, int indexInWeatherList)
        {


            if (weatherObject != null)
            {
                Weather weather = (Weather)weatherObject;
                int resourceIDPlace = 0;
                int resourceIDTemperture = 0;
                int resourceIDDescription = 0;
                int resourceIDImage = 0;
                int resourceIDLayouy = 0;


                switch (indexInWeatherList)
                {
                    case 0:
                        resourceIDPlace = Resource.Id.txtWeatherPlace1;
                        resourceIDTemperture = Resource.Id.txtWeatherTemperture1;
                        resourceIDDescription = Resource.Id.txtWeatherDescription1;
                        resourceIDImage = Resource.Id.btnMainWeather1;
                        resourceIDLayouy = Resource.Id.layoutWeather1;
                        break;
                    case 1:
                        resourceIDPlace = Resource.Id.txtWeatherPlace2;
                        resourceIDTemperture = Resource.Id.txtWeatherTemperture2;
                        resourceIDDescription = Resource.Id.txtWeatherDescription2;
                        resourceIDImage = Resource.Id.btnMainWeather2;
                        resourceIDLayouy = Resource.Id.layoutWeather2;
                        break;
                    case 2:
                        resourceIDPlace = Resource.Id.txtWeatherPlace3;
                        resourceIDTemperture = Resource.Id.txtWeatherTemperture3;
                        resourceIDDescription = Resource.Id.txtWeatherDescription3;
                        resourceIDImage = Resource.Id.btnMainWeather3;
                        resourceIDLayouy = Resource.Id.layoutWeather3;
                        break;
                    case 3:
                        resourceIDPlace = Resource.Id.txtWeatherPlace4;
                        resourceIDTemperture = Resource.Id.txtWeatherTemperture4;
                        resourceIDDescription = Resource.Id.txtWeatherDescription4;
                        resourceIDImage = Resource.Id.btnMainWeather4;
                        resourceIDLayouy = Resource.Id.layoutWeather4;
                        break;
                }

                LinearLayout layoutWeather = (LinearLayout)FindViewById(resourceIDLayouy);
                TextView txtWeatherPlace = (TextView)FindViewById(resourceIDPlace);
                TextView txtWeatherTemperture = (TextView)FindViewById(resourceIDTemperture);
                TextView txtWeatherDescription = (TextView)FindViewById(resourceIDDescription);
                ImageView btnMainWeather = (ImageView)FindViewById(resourceIDImage);

                txtWeatherPlace.Text = weather.getCity();
                //txtWeatherPlace.BringToFront();
                txtWeatherTemperture.Text = weather.getTemperature();
                //txtWeatherTemperture.BringToFront();
                txtWeatherDescription.Text = weather.getDescription();
                //txtWeatherDescription.BringToFront();

                try
                {
                    //weather.setImageView(Utils.GetImageViewFromhUrl(weather.getPoster()));
                    //weather.setImageView(new ImageView(Application.Context));
                    Android.Net.Uri uri = Android.Net.Uri.Parse(weather.getPoster());
                    btnMainWeather.SetImageURI(uri);
                    //Android.Graphics.Bitmap drawable = weather.getImageView().GetDrawingCache(true);   //.Drawable;
                    //btnMainWeather.SetImageDrawable(drawable);
                }
                catch
                {

                }

                //layoutWeather.Click += btnMainWeather_Click;
                //btnMainWeather.Click += btnMainWeather_Click;
                //btnMainWeather.BringToFront();

                weather.setImageView(btnMainWeather);

                try
                {
                    Android.Net.Uri uri = Android.Net.Uri.Parse(weather.getPoster());
                    btnMainWeather.SetImageURI(uri);
                }
                catch (Exception ex)
                {
                    MH_Utils.Utils.WriteToLog("Error when try to set Weater icon: " + weather.getPoster() + MH_Utils.Utils.LINE_SEPERATOR + ex.Message);
                }
            }

        }

        private void OnWeatherCompleteLoadData(List<Weather> weatherList)
        {
            if (weatherList != null && weatherList.Count > 0 && !mH_Weather.IsChangePlaces)
            {
                WeatherScroll.Start();
            }
            else
            {
                WeatherScroll.Stop();
            }
        }

        [Obsolete]
        private void btnMainWeather_Click(object sender, EventArgs e)
        {

            if (mH_Weather.IsChangePlaces)
            {
                WeatherScroll.Stop();
                mH_Weather.StotChangePlace();
                WeatherScroll.StartPosstion();
            }
            else
            {
                WeatherScroll.Stop();
                mH_Weather.StartChangePlace();

                // Asyncronic - Not relevant
                //if (mH_Weather.WeatherList.Count > 0)
                //{
                //WeatherScroll.Start();
                //}
                //else
                //{
                //    WeatherScroll.StartPosstion();
                //}
            }

        }

        private void Initialize()
        {

            SetControlsIO();

            TasksList = new List<Task>();

            TimersArray = new List<object[]>();

            TimerInterval = 600000;  // 1000 * 60 * 30

            //mainActivityServices = new MainActivityServices(MainActivity.this, this, this);
            //IsManualHtml = false;
            //setControlsColors();

            MH_Utils.Utils.ClientActivity = this;
            MH_Utils.Utils.ClientContext = MainContext;
            MH_Utils.Utils.LOG_FILE_NAME = "LogTaskReminder.txt";
            MH_Utils.Utils.LOG_FILE_PATH = MainContext.GetExternalFilesDir("").AbsolutePath;
            //string folderBackup = Android.OS.Environment.DirectoryMusic;        // "ProjTaskReminder"
            //LOG_FILE_PATH = Android.OS.Environment.GetExternalStoragePublicDirectory(folderBackup).AbsolutePath;
            //LOG_FILE_PATH = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;


            // Open Notification Channel. Must be in the start init
            // Done again in 'Util.createNotificationBuilder()'
            mh_Notification = new MH_Notification(this, MainContext);
            mh_Notification.createNotificationChannel();

            WeatherList = new List<Weather>();
            mH_Weather = new MH_Weather();
            mH_Weather.activity = this;
            mH_Weather.WEATER_CHANE_PLACE_TIMER_INTERVAL = 360000;
            mH_Weather.OnChanePlace += OnWeatherChangingPlace;
            mH_Weather.OnCompleteLoadData += OnWeatherCompleteLoadData;

            WeatherScroll = new MH_Scroll();
            WeatherScroll.SCROLL_DELTA = 6;
            WeatherScroll.SCROLL_END_POINT = 980;
            WeatherScroll.SCROLL_INTERVAL = 300;
            WeatherScroll.ScrollControl = scrollWeather;
            //WeatherScroll.OnScrolling += OnWeatherScroll;

            NewsScroll = new MH_Scroll();
            NewsScroll.SCROLL_DELTA = -40;
            NewsScroll.SCROLL_END_POINT = 980;
            NewsScroll.SCROLL_INTERVAL = 200;
            NewsScroll.ScrollControl = scrollNews;
            NewsScroll.IsScrollRightToLeft = true;
            //WeatherScroll.OnScrolling += OnWeatherScroll;

            KeyValuePair<string, int>[] perm = new KeyValuePair<string, int>[] {
                                                                                new KeyValuePair<string, int>(Manifest.Permission.WriteExternalStorage, PERMISSIONS_REQUEST_WRITE_STORAGE),
                                                                                new KeyValuePair<string, int>(Manifest.Permission.ReadExternalStorage, PERMISSIONS_REQUEST_READ_STORAGE)
                                                                               };
            MH_Utils.Utils.PermissionAsk(perm);


            //SearchDialogInit();
        }

        private void OnWeatherScroll(int obj)
        {

        }

        private void btnMainMusic_Click(object sender, EventArgs e)
        {
            if (IntentMusic == null)
            {
                IntentMusic = new Intent(this, typeof(ActivityMusic));
                IntentMusic.SetFlags(ActivityFlags.NewTask);
            }
            else
            {
                //intent.SetFlags(ActivityFlags.TaskOnHome);
            }

            ActivityMusic.context = MainContext;      //Application.Context;

            //StartActivityForResult(intent, 2345);
            MainContext.StartActivity(IntentMusic);
        }


        private void FillListFromDB()
        {
            TasksList = GetTasksFromDB();

            FillList();
        }

        private void FillList(bool showMessage = true)
        {

            //Toast.MakeText(this, "Fill list with items...", ToastLength.Short).Show();



            killOldTimers();


            for (int i = 0; i < TasksList.Count; i++)
            {
                try
                {
                    Task task = TasksList[i];

                    TimerRun(task);
                }
                catch (Exception ex)
                {
                    //Log.d("fillList Error: ", ex.getMessage());
                    //ex.printStackTrace();
                }
            }


            SetListAdapter();

            SortList(showMessage);



            //for (int i = 0; i < countryList.Length; ++i)
            //{
            //    Task task = new Task();
            //    task.setTitle(countryList[i]);
            //    task.setDescription(countryList[i]);
            //    task.setDate_due(DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            //    list.Add(task);
            //}
            // Directly from array
            //ArrayAdapter<string> arrayAdapter = new ArrayAdapter<string>(this, Resource.Layout.list_item, Resource.Id.txtTitle, countryList);
            //lstTasks.SetAdapter(arrayAdapter);
        }

        private void SortList(bool showMessage = true)
        {
            int id = -1;



            if (showMessage)
            {
                Toast.MakeText(this, "Refresh items...", ToastLength.Short).Show();
            }

            TasksList = TasksList.OrderByDescending(a => a.getDate()).ToList();

            if (CurrentTask != null && CurrentTask.getTaskID() > 0)
            {
                id = CurrentTask.getTaskID();
            }

            RefreshListAdapter();

            if (id > -1)
            {
                // Scroll to handled task
                focusListByID(id);
            }

        } 

        [Obsolete]
        private void SetListAdapter()
        {
            listViewAdapter = new ListViewAdapter(MainContext, TasksList);

            // Fill controls in the task card - From the adapter
            listViewAdapter.OnListItemSetControlsInView += SetTaskItemControlsInView;
            listViewAdapter.OnItemClick += listViewAdapter_OnItemClick;

            lstTasks.SetAdapter(listViewAdapter);
            //lstTasks.ItemClick += lstTasks_OnItemClick;

            listViewAdapter.ParentListView = lstTasks;


            //lstTasks.OnItemClickListener = lstTasks_OnItemClick3;
            //lstTasks.ContextClickable = true;
            //lstTasks.Clickable = true;
            //AdapterView.IOnItemClickListener lstTasks_OnItemClick3;
            ///lstTasks.OnItemClickListener = lstTasks_OnItemClick3;
            //View.IOnClickListener lstTasks_OnItemClick3;
            //lstTasks.SetOnClickListener(lstTasks_OnItemClick3);
            //lstTasks.ContextClick += lstTasks_OnItemClick2;
            //lstTasks.PerformContextClick();
            //lstTasks.SetOnClickListener(simpleListItem_DoubleClick);
            //lstTasks.OnItemClickListener += OnItemClick
            //lstTasks.SetLayoutManager(mLayoutManager);
            //lstTasks.setHasFixedSize(true);
            //lstTasks.SetLayerType(new LinearLayout(this), new Android.Graphics.Paint());

        }


        private void RefreshListAdapter()
        {
            //listViewAdapter = new ListViewAdapter(MainContext, TasksList);
            //listViewAdapter.OnListItemSetControlsInView += SetTaskItemControlsInView;
            //lstTasks.SetAdapter(listViewAdapter);

            listViewAdapter.NotifyDataSetChanged();
        } 
         
        private void focusListByID(int id)
        {
            if (TasksList.Count == 0 || id == -1)
            {
                return;
            }

            int index = searchIndexListByID(id);

            if (index > -1)
            {
                lstTasks.SmoothScrollToPosition(index);
            }

        }
        private void focusListOnToday(DateTime date)
        {
            if (TasksList.Count == 0)
            {
                return;
            }

            int index = searchDateInList(TasksList, date);

            if (index > -1)
            {
                lstTasks.SmoothScrollToPosition(index);
            }
        }

        private bool ConnectToDB()
        {
            bool result = true;


            try
            {
                // Its Internal
                //KeyValuePair<string, int>[] perm = new KeyValuePair<string, int>[] {
                //                                                                      new KeyValuePair<string, int>(Manifest.Permission.WriteExternalStorage, PERMISSIONS_REQUEST_WRITE_STORAGE),
                //                                                                      new KeyValuePair<string, int>(Manifest.Permission.ReadExternalStorage, PERMISSIONS_REQUEST_READ_STORAGE)
                //                                                                   };
                //MH_Utils.Utils.PermissionAsk(perm);


                DB_TASK_DB_PATH = MainContext.GetExternalFilesDir("").AbsolutePath;

                DBTaskReminder = new DBTaskReminder(DB_TASK_DB_NAME, DB_TASK_DB_PATH, DB_TABLE_NAME);


                if (!DBTaskReminder.IsTableExists(DB_TABLE_NAME))
                {
                    try
                    {
                        CreateTableResult tableWasCreated = DBTaskReminder.DB.CreateTable<TBL_Tasks>();
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
                        Console.WriteLine(ex.StackTrace + "  -  " + ex.Message);
                    }
                }
                else
                {
                    if (!DBTaskReminder.IsFieldExists(DB_TABLE_NAME, DB_FIELDNAME_LAST_UPDATE))
                    {
                        DBTaskReminder.addFieldToTable(DB_TABLE_NAME, DB_FIELDNAME_LAST_UPDATE, "VARCHAR(16)");
                        //DBTaskReminder = new DBTaskReminder(DB_TASK_DB_NAME, DB_TASK_DB_PATH, DB_TABLE_NAME);
                    }
                    if (!DBTaskReminder.IsFieldExists(DB_TABLE_NAME, DB_FIELDNAME_COLOR))
                    {
                        DBTaskReminder.addFieldToTable(DB_TABLE_NAME, DB_FIELDNAME_COLOR, "VARCHAR(10)");
                        //DBTaskReminder = new DBTaskReminder(DB_TASK_DB_NAME, DB_TASK_DB_PATH, DB_TABLE_NAME);
                    }
                }

                if (!DBTaskReminder.IsTableExists(DB_TABLE_SETTING))
                {
                    try
                    {
                        var t = DBTaskReminder.DB.CreateTable<TBL_Settings>();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace + "  -  " + ex.Message);
                    }

                }
                else
                {
                    TBL_Settings tBL_Settings = null;
                    TableQuery<TBL_Settings> table = MainActivity.DBTaskReminder.DB.Table<TBL_Settings>();
                    tBL_Settings = table.FirstOrDefault();
                    if (tBL_Settings != null)
                    {
                        ActivityMusic.MUSIC_PATH = tBL_Settings.MusicPath;
                        MH_Utils.Utils.NEWS_RSS_ADDRESS2 = tBL_Settings.NewsUrl;
                        MH_Weather.URL_LEFT_WEATHER = tBL_Settings.WeatherUrl;
                    }
                }
            }
            catch (Exception ex)
            {
                MH_Utils.Utils.WriteToLog(ex.Message, true);
            }

            // /storage/emulated/0/Music/
            // /storage/emulated/0/Android/data/com.meirhemed.projtaskreminder/files
            //DB_PATH = MainContext.GetExternalFilesDir("").AbsolutePath;         
            //DB_PATH = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);  //"/Assets/" + DB_NAME;             //Environment.SpecialFolder.LocalApplicationData

            return result;
        }

        private List<Task> GetTasksFromDB()
        {
            //Toast.MakeText(this, "Load items from Database", ToastLength.Short).Show();


            TasksList.Clear();
            TasksListBackup = new List<Task>();


            TableQuery<TBL_Tasks> table = DBTaskReminder.DB.Table<TBL_Tasks>();

            foreach (TBL_Tasks record in table)
            {
                Task task = new Task();

                task.setTaskID(record.ID);
                task.setTitle(record.Title);
                task.setDescription(record.Description);
                if (!string.IsNullOrEmpty(record.DateDue) && record.DateDue != null && !record.DateDue.Trim().Equals(""))
                {
                    task.setDate_due(record.DateDue.Trim().Substring(0, 10));
                    task.setTime_due(record.DateDue.Trim().Substring(11, 5));
                }
                task.setBackgroundColor(record.CardBackColor);
                task.setDate_last_update(record.LastUpdateDate);

                task.TableRecord = record;

                TasksList.Add(task);
                TasksListBackup.Add(task);
            }

            return TasksList;
        }

        private List<Task> RestoreTaskList()
        {

            TasksList.Clear();

            for (int i=0; i<TasksListBackup.Count; i++)
            {
                Task task = TasksListBackup[i];
                TasksList.Add(task);
            }

            return TasksList;
        }

        private void OpenTaskDetailsPage(Task task, bool isNewMode, Context context)
        {

            if (isNewMode)
            {
                task = newTaskDetails();
            }

            Intent intent = new Intent(this, typeof(ActivityTaskDetails));
            intent.SetFlags(ActivityFlags.NewTask);

            intent.PutExtra("TaskID", task.getTaskID());

            CurrentTask = task;

            ActivityTaskDetails.isNewMode = isNewMode;
            ActivityTaskDetails.CurrentTask = task;
            ActivityTaskDetails.context = MainContext;      //Application.Context;

            //this.StartActivityForResult(intent, SCREEN_TASK_DETAILS_SAVED);
            MainContext.StartActivity(intent);

        }

        private void ShowSetting()
        {
            Intent intent = new Intent(this, typeof(ActivitySettings));
            //intent.setFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP);

            intent.PutExtra("MusicPath", ActivityMusic.MUSIC_PATH); // "/storage/emulated/0/Music");
            intent.PutExtra("NewsUrl", MH_Utils.Utils.NEWS_RSS_ADDRESS2);
            intent.PutExtra("WeatherUrl", MH_Weather.URL_LEFT_WEATHER);    // "http://api.weatherstack.com/current?access_key=f2896ef52242c1e367e2170ce40352ba&query=");


            ActivitySettings.context = MainContext;

            StartActivityForResult(intent, SHOW_SCREEN_SETTING);
            //StartActivity.StartActivity(intent);

        }

        /// <summary>
        /// On ListViewAdapter item click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listViewAdapter_OnItemClick(ListViewAdapter.ListViewHolder listViewHolder, int position)
        {
            CurrentTask = TasksList[position];

            OpenTaskDetailsPage(CurrentTask, false, MainContext);
        }

        /// <summary>
        /// On ListView control Item click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lstTasks_OnItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            CurrentTask = TasksList[e.Position];

            OpenTaskDetailsPage(CurrentTask, false, MainContext);

        }

        //private void OnTaskItemClick3(object sender, EventArgs e)
        //{
        //    int position = (int)sender;
        //    CurrentTask = TasksList[position];
        //    OpenTaskDetailsPage(CurrentTask, false, MainContext);
        //}

        private void lstTasks_OnItemClick2(object sender, View.ContextClickEventArgs e)
        {
            if (lstTasks.SelectedItemPosition < 0 || lstTasks.SelectedItemPosition >= TasksList.Count)
            {
                return;
            }

            CurrentTask = TasksList[lstTasks.SelectedItemPosition];

            OpenTaskDetailsPage(CurrentTask, false, MainContext);
        }

        public View.IOnClickListener lstTasks_DoubleClick(View.IOnClickListener vvv)
        {
            OpenTaskDetailsPage(CurrentTask, false, MainContext);

            return null;    // View.IOnClickListener;
        }

        private void btnMainNew_OnClick(object sender, EventArgs eventArgs)
        {
            View view = (View)sender;

            OpenTaskDetailsPage(CurrentTask, true, Application.Context);

            //Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
            //              .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
        }

        protected void OnExitResult(int requestCode, Result resultCode, Intent inputIntent)
        {
            //base.OnActivityReenter(requestCode,resultCode, data);

            if (resultCode == Result.Ok)
            {
                switch (requestCode)
                {
                    case SCREEN_TASK_DETAILS_SAVED:
                        {
                            if (ActivityTaskDetails.isNewMode)
                            {
                                TimerRun(CurrentTask);
                                TasksList.Add(CurrentTask);
                                FillList();
                                //SetListAdapter();
                                //SortList();
                            }
                            else
                            {
                                TimerStop(CurrentTask);
                                TimerRun(CurrentTask);
                                SortList();
                                //FillList();
                            }
                            break;
                        }

                    case SCREEN_TASK_DETAILS_DELETE:        // Delete
                        {
                            TimerStop(CurrentTask);
                            TasksList.Remove(CurrentTask);
                            FillList();
                            //SortList();
                            break;
                        }
                }

            }


        }

        private void SaveSettingsValues(Intent inputIntent)
        {
            if (!string.IsNullOrEmpty(inputIntent.GetStringExtra("MusicPath")))
            {
                ActivityMusic.MUSIC_PATH = inputIntent.GetStringExtra("MusicPath");
            }
            if (!string.IsNullOrEmpty(inputIntent.GetStringExtra("NewsUrl")))
            {
                MH_Utils.Utils.NEWS_RSS_ADDRESS2 = inputIntent.GetStringExtra("NewsUrl");
            }
            if (!string.IsNullOrEmpty(inputIntent.GetStringExtra("WeatherUrl")))
            {
                MH_Weather.URL_LEFT_WEATHER = inputIntent.GetStringExtra("WeatherUrl");
            }


            bool isNewMode = false;
            long recorsWasEffected = 0;
            TBL_Settings tBL_Settings = new TBL_Settings();


            TableQuery<TBL_Settings> table = MainActivity.DBTaskReminder.DB.Table<TBL_Settings>();
            tBL_Settings = table.FirstOrDefault();
            if (tBL_Settings == null)
            {
                isNewMode = true;
                tBL_Settings = new TBL_Settings();
            }

            tBL_Settings.MusicPath = ActivityMusic.MUSIC_PATH;
            tBL_Settings.NewsUrl = MH_Utils.Utils.NEWS_RSS_ADDRESS2;
            tBL_Settings.WeatherUrl = MH_Weather.URL_LEFT_WEATHER;

            if (isNewMode)
            {
                recorsWasEffected = MainActivity.DBTaskReminder.RecordInser(tBL_Settings, MainActivity.DB_TABLE_SETTING);
            }
            else
            {
                List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
                list.Add(new KeyValuePair<string, string>("MusicPath", ActivityMusic.MUSIC_PATH));
                list.Add(new KeyValuePair<string, string>("NewsUrl", MH_Utils.Utils.NEWS_RSS_ADDRESS2));
                list.Add(new KeyValuePair<string, string>("WeatherUrl", MH_Weather.URL_LEFT_WEATHER));

                recorsWasEffected = MainActivity.DBTaskReminder.RecordUpdate(DB_TABLE_SETTING, list, null);
                //recorsWasEffected = MainActivity.DBTaskReminder.RecordUpdate(tBL_Settings);
            }

        }

        private void TimerRun(Task currentTask)
        {
            DateTime? timerDate;
            String dateStr;




            timerDate = currentTask.getDate();

            if (timerDate == null)
            {
                // Show mssage if there is message in que
                showGlobalMessageOnToast();
                return;
            }


            DateTime today = MH_Utils.Utils.GetDateNow();

            if (timerDate.Value.CompareTo(today) <= 0)
            {
                // Show mssage if there is message in que
                showGlobalMessageOnToast();
                return;
            }


            //Toast.MakeText(this, "Task date: " + MH_Utils.Utils.getDateFormattedString(timerDate.Value) + "     Now: " + MH_Utils.Utils.getDateFormattedString(MH_Utils.Utils.GetDateNow()), ToastLength.Long).Show();

            //CurrentTask = currentTask;


            TimerExecute(currentTask);
            //TimerExecute2(currentTask);
            //TimerExecute3(currentTask, timerDate);


            dateStr = MH_Utils.Utils.getDateFormattedString(timerDate.Value);

            if (isShowTimerReminder && !MainMessageText.Trim().Equals(""))
            {
                string text = "יצר תזכורת ביום " + MH_Utils.Utils.getDateDayName(timerDate.Value) + "  " + dateStr;
                MainMessageText = MainMessageText + " - " + text;
                showGlobalMessageOnToast();
            }

            isShowTimerReminder = false;

        }


        #region Timer as System.Timers.Timer

        private void TimerExecute(Task currentTask)
        {
            //Timer timer = new Timer();

            TimerInterval = 30000;  // 30 seconds

            TaskTimerService = new MH_Utils.TimerService(currentTask, MH_Utils.TimerService.enumTimerType.Timer);
            //TaskTimerService = new TimerService(currentTask, timer);
            TaskTimerService.TimerInterval = TimerInterval;
            TaskTimerService.OnTimerTick += OnTaskTimer_OK;
            TaskTimerService.DateDue = currentTask.getDate();

            currentTask.setTimer(TaskTimerService.TimerObject);

            addTimerToKillArray(currentTask.getTaskID(), currentTask, TaskTimerService);   // , TaskTimerService.TimerObject

            TaskTimerService.Start();
        }

        private void OnTaskTimer_OK(Object currentTask, Timer timerObject)   // Task
        {
            // just kill other objects in timers array
            TimerStop((Task)currentTask);

            // Do the Job - Notification
            TimerTask_onTick((Task)currentTask);

        }

        #endregion

        #region Timer as System.Threading.Thread

        private void TimerExecute2(Task currentTask)
        {
            // Just to init object. will ReInit next lines
            System.Threading.Thread timer = System.Threading.Thread.CurrentThread;

            MH_Utils.TimerService TaskTimerService = new MH_Utils.TimerService(currentTask, MH_Utils.TimerService.enumTimerType.Thread);
            //TimerService TaskTimerService = new TimerService(currentTask, timer);
            TaskTimerService.TimerInterval = TimerInterval;
            TaskTimerService.activity = this;
            TaskTimerService.OnThreadTick += OnTaskThread_OK;
            TaskTimerService.DateDue = currentTask.getDate();

            //timer = new System.Threading.Thread(new System.Threading.ThreadStart(TaskTimerService.Thread_Elapsed));

            // Before, just for declare
            //TaskTimerService.ThreadObject = timer;

            //currentTask.setTimer(timer);
            addTimerToKillArray(currentTask.getTaskID(), currentTask, TaskTimerService);

            TaskTimerService.Start();
        }

        private void OnTaskThread_OK(Object currentTask, System.Threading.Thread timerObject)    // Task
        {
            TimerStop((Task)currentTask);

            // Do the Job - Notification
            TimerTask_onTick((Task)currentTask);

        }

        //private void TimerThreadExecute()
        //{
        //    TimerRunning(CurrentTask, null);
        //}
        //
        //private ElapsedEventHandler TimerRunning(object sender, ElapsedEventArgs args)
        //{

        //    //if (sender == null)
        //    //{
        //    //    return null;
        //    //}

        //    long counter = 0;
        //    // Limited for 3 Days
        //    long limitSeconds = 1000 * 60 * 60 * 24 * 3;


        //    Task currentTask = (Task)sender;



        //    if (currentTask.getDate() == null)
        //    {
        //        return null;
        //    }


        //    DateTime dateNow = MH_Utils.Utils.GetDateNow();
        //    string strDateNow = MH_Utils.Utils.getDateFormattedString(dateNow);
        //    string strDateTask = MH_Utils.Utils.getDateFormattedString(currentTask.getDate().Value);


        //    while (counter <= limitSeconds && strDateNow.CompareTo(strDateTask) < 0)
        //    {
        //        Thread.Sleep(TimerInterval);

        //        RunOnUiThread(() =>
        //        {
        //            dateNow = MH_Utils.Utils.GetDateNow();
        //            strDateNow = MH_Utils.Utils.getDateFormattedString(dateNow);
        //        });

        //        counter += TimerInterval;

        //        //RunOnUiThread(ActionOnTaskDateDue); // (strDateNow));    // =>
        //        //{
        //        //int newPosition = mediaPlayer.CurrentPosition;
        //        //barSeek.Progress = newPosition;
        //        //};
        //    }


        //    TimerStop(currentTask);

        //    // Do the Job - Notification
        //    TimerTask_onTick(currentTask);


        //    return null;    // new ElapsedEventHandler(null, new ElapsedEventArgs());
        //}

        #endregion

        private void TimerStop(Task currentTask)
        {
            int index = searchIDInTimersList(currentTask.getTaskID());

            if (index == -1)
            {
                return;
            }

            TimerKill(index);
        }

        private void TimerKill(int timerArrayIndex)
        {
            try
            {
                if (timerArrayIndex >= TasksList.Count)
                {
                    return;
                }

                //timersArray[0] = taskID;
                //timersArray[1] = currentTask;
                // Cancel - timersArray[2] = timer;
                //timersArray[2] = taskTimerElapsed;

                object[] taskTimerArray = TimersArray[timerArrayIndex];

                Task currentTask = (Task)taskTimerArray[1];
                Timer timer = currentTask.getTimer();
                TimerService taskTimerElapsed = (TimerService)taskTimerArray[2];

                if (timer != null)
                {
                    TimerDispose(timer);
                }

                //timer = (Timer)taskTimerArray[2];
                //if (timer != null)
                //{
                //    TimerDispose(timer);
                //}

                taskTimerElapsed.Destroy();
                taskTimerElapsed = null;

                timer = null;

                TimersArray.RemoveAt(timerArrayIndex);

                currentTask.setTimer(null);
                currentTask.setTimer_task(null);
            }
            catch (Exception ex)
            {
                MH_Utils.Utils.WriteToLog("TimerKill: " + MH_Utils.Utils.LINE_SEPERATOR + ex.Message);
            }
        }

        private void TimerDispose(Timer timer)
        {
            timer.Stop();
            timer.Close();
            timer.Dispose();

            //timer.Stop();
            //timer.Close();
            //timer.Dispose();
            //timer = null;
            //timer.Abort();
            //timer = null;
            //timer.purge();
            //timer.c.Cancel();
            //timerTask.cancel();
            //timer = null;
            //timerTask = null;

            timer = null;

        }

        #region Timer as Java.Util.Timer

        private void TimerExecute3(Task currentTask, Java.Util.Date timerDate)
        {
            Java.Util.TimerTask timerTask = null;  // = new Java.Util.TimerTask(TimerTask_onTick);
            long ticks = timerTask.ScheduledExecutionTime();
            timerTask.Run();

            Java.Util.Timer timer = new Java.Util.Timer();
            timer.Schedule(timerTask, timerDate);

            //currentTask.setTimer(timer);
            //addTimerToKillArray(currentTask.getTaskID(), 0);     // timer timerTask);


            // Run the Timer
            //timerTask.ActionOnPlayingMusic += timerTask.picsTimer_onTick;
            //{
            //    @Override
            //    public void run()
            //{
            //    TimerTask_onTick(currentTask);
            //}
            //
            //            timer.Schedule(timerTask, 4, 3000);  //, timerDate);
            //timer.Notify();

        }

        #endregion


        private void TimerTask_onTick(Task task)
        {
            // Play Notification sound
            //int resourceID = Resource.Raw.deduction;
            //Android.Media.MediaPlayer mediaPlayer = Android.Media.MediaPlayer.Create(this, resourceID);   // uri);
            //mediaPlayer.Start();

            //Toast.MakeText(this, "Timer is tiking: "+task.getDescription(), ToastLength.Long).Show();

            this.SetTurnScreenOn(true);
            //Util.turnOnScreen(this);

            // Show android notification
            showNotificationToScreen(task);

            //showAlarmScreen(task);

            //showReminderDialog(task);

        }

        private void TimerNextTime()    // string ss)
        {
            DateTime dateNow = MH_Utils.Utils.GetDateNow();
            string strDateNow = MH_Utils.Utils.getDateFormattedString(dateNow);

            //return strDateNow;
        }

        private void addTimerToKillArray(int taskID, Task currentTask, MH_Utils.TimerService taskTimerElapsed)   // , Timer timer
        {
            object[] timersArray = new object[3];

            timersArray[0] = taskID;
            timersArray[1] = currentTask;
            //timersArray[2] = timer;
            timersArray[2] = taskTimerElapsed;

            TimersArray.Add(timersArray);
        }

        private static int searchIDInTimersList(int taskID)
        {
            int result = -1;

            for (int i = 0; i < TimersArray.Count; i++)
            {
                object[] timersArray = TimersArray[i];

                if ((int)timersArray[0] == taskID)
                {
                    result = i;
                    break;
                }
            }

            return result;
        }

        private void killOldTimers()
        {

            if (TimersArray == null)
            {
                TimersArray = new List<object[]>();
                return;
            }

            for (int i = TimersArray.Count - 1; i >= 0; i--)
            {
                object[] timersArray = TimersArray[i];

                TimerKill(i);
            }

            TimersArray.Clear();
            TimersArray = null;
            TimersArray = new List<object[]>();

        }

        /// <summary>
        /// Show mssage if there is message in que
        /// </summary>
        public static void showGlobalMessageOnToast()
        {
            if (isShowTimerReminder && !MainMessageText.Trim().Equals(""))
            {
                Toast.MakeText(MainContext, MainMessageText, ToastLength.Long).Show();
                MainMessageText = "";
            }
        }

        protected void showNotifications(Task currentTask)
        {
        }

        private void showNotificationToScreen(Task currentTask)
        {

            if (currentTask == null)
            {
                return;
            }

            int notificationID = currentTask.getTaskID();


            string title = currentTask.getTitle().Trim();
            string titleText = title;
            string titleSpannableString = titleText;
            //SpannableString titleSpannableString = new SpannableString(titleText);

            if (!title.Equals(""))
            {
                titleText = title;  //.SubSequence(0, title.length());   // + " - ";    // + "\n";
                //titleSpannableString = new SpannableString(titleText);
                //StyleSpan styleSpan = new StyleSpan(Android.Graphics.TypefaceStyle.Bold);   // Typeface Typeface.BOLD);
                //titleSpannableString.SetSpan(styleSpan, 0, titleSpannableString.Length(), SpanTypes.ExclusiveExclusive); // .SPAN_EXCLUSIVE_EXCLUSIVE);
            }

            string description = currentTask.getDescriptionWithHtml();
            //SpannedString description = currentTask.getDescription();

            SpannableStringBuilder builder = new SpannableStringBuilder();
            builder.Append(titleSpannableString);
            builder.Append(description);
            //Log.d("Noti", builder.subSequence(0, builder.length()-0).toString());

            notificationBuilder = mh_Notification.createNotificationBuilder(titleSpannableString, description, Resource.Mipmap.note1, notificationID);  // getString(R.string.app_name), builder

        }

        public static Task newTaskDetails()
        {
            CurrentTask = new Task();

            CurrentTask.setDate_due("");
            //CurrentTask.setBackgroundColor(DefaultCardBackgroundColor.ToString());
            CurrentTask.setDate_last_update(MH_Utils.Utils.getDateFormattedString(MH_Utils.Utils.GetDateNow()));
            CurrentTask.setRepeat("פעם אחת");

            return CurrentTask;
        }

        private void BackupDataBaseFile(bool showMessage = true)
        {
            //KeyValuePair<string, int>[] perm = new KeyValuePair<string, int>[] {
            //                                                                          new KeyValuePair<string, int>(Manifest.Permission.WriteExternalStorage, PERMISSIONS_REQUEST_WRITE_STORAGE),
            //                                                                          new KeyValuePair<string, int>(Manifest.Permission.ReadExternalStorage, PERMISSIONS_REQUEST_READ_STORAGE)
            //                                                                       };
            //MH_Utils.Utils.PermissionAsk(perm);


            string backupFolderName = Android.OS.Environment.DirectoryMusic;    // "ProjTaskReminder"
            string targetPath = Android.OS.Environment.GetExternalStoragePublicDirectory(backupFolderName).AbsolutePath;
            //targetPath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            //targetPath = MainContext.GetExternalFilesDir("").AbsolutePath;;


            string fullPathSource = Path.Combine(DBTaskReminder.DB_PATH, DBTaskReminder.DB_NAME);
            string fullPathTarget = Path.Combine(targetPath, DBTaskReminder.DB_NAME);

            bool result = MH_Utils.Utils.CopyFile(fullPathSource, fullPathTarget);

            fullPathSource = Path.Combine(MH_Utils.Utils.LOG_FILE_PATH, MH_Utils.Utils.LOG_FILE_NAME);
            fullPathTarget = Path.Combine(targetPath, MH_Utils.Utils.LOG_FILE_NAME);

            result = MH_Utils.Utils.CopyFile(fullPathSource, fullPathTarget);

            fullPathSource = Path.Combine(MH_Utils.Utils.LOG_FILE_PATH, WRITE_LIST_FILE_NAME);
            fullPathTarget = Path.Combine(targetPath, WRITE_LIST_FILE_NAME);

            bool resultTextFile = MH_Utils.Utils.CopyFile(fullPathSource, fullPathTarget);

            if (result)
            {
                string message = "Database was copied OK";
                if (showMessage)
                {
                    Toast.MakeText(this, message, ToastLength.Long).Show();
                }
                //MH_Utils.Utils.WriteToLog(message, true);
            }
        }

        private void RestoreDataBaseFile()
        {

            //KeyValuePair<string, int>[] perm = new KeyValuePair<string, int>[] {
            //                                                                          new KeyValuePair<string, int>(Manifest.Permission.WriteExternalStorage, PERMISSIONS_REQUEST_WRITE_STORAGE),
            //                                                                          new KeyValuePair<string, int>(Manifest.Permission.ReadExternalStorage, PERMISSIONS_REQUEST_READ_STORAGE)
            //                                                                       };
            //MH_Utils.Utils.PermissionAsk(perm);


            // Must first close DB
            DBTaskReminder.DB.Close();


            string backupFolderName = Android.OS.Environment.DirectoryMusic;    // "ProjTaskReminder"
            string sourcePath = Android.OS.Environment.GetExternalStoragePublicDirectory(backupFolderName).AbsolutePath;
            string targetPath = DBTaskReminder.DB_PATH;


            // Database file
            string fullPathTarget = Path.Combine(targetPath, DBTaskReminder.DB_NAME);
            string fullPathSource = Path.Combine(sourcePath, DBTaskReminder.DB_NAME);

            bool result = MH_Utils.Utils.CopyFile(fullPathSource, fullPathTarget);

            // Log file
            fullPathSource = Path.Combine(MH_Utils.Utils.LOG_FILE_PATH, MH_Utils.Utils.LOG_FILE_NAME);
            fullPathTarget = Path.Combine(targetPath, MH_Utils.Utils.LOG_FILE_NAME);

            bool result2 = MH_Utils.Utils.CopyFile(fullPathSource, fullPathTarget);


            ConnectToDB();

            if (result)
            {
                string message = "Database was restored OK";
                Toast.MakeText(this, message, ToastLength.Short).Show();
            }

            FillListFromDB();
        }

        private Task searchTaskByID(int taskID)
        {
            return TasksList.FirstOrDefault(a => a.getTaskID() == taskID);
        }

        private List<Task> searchInListByText(string textToSearch)
        {
            List<Task> result = new List<Task>();


            TasksList = TasksList.Where(a => a.getTitle().IndexOf(textToSearch) > -1 || a.getDescriptionWithHtml().IndexOf(textToSearch) > -1).ToList();

            result = TasksList;

            return result;
        }

        private int searchIndexListByID(int taskID)
        {
            int result = -1;

            for (int i = 0; i < TasksList.Count; i++)
            {
                if (TasksList[i].getTaskID() == taskID)
                {
                    result = i;
                    break;
                }
            }

            return result;
        }

        private int searchDateInList(List<Task> tasksList, DateTime date)
        {
            int result = -1;
            //SimpleDateFormat simpleDateFormat = new SimpleDateFormat("dd/MM/yyyy");
            //string dateStrToFind;
            //string dateStrTask;



            //dateStrToFind = MH_Utils.Utils.getDateFormattedString(date);    //simpleDateFormat.format(date);

            for (int i = 0; i < tasksList.Count; i++)
            {
                DateTime? taskDate = tasksList[i].getDate();

                if (taskDate.HasValue)
                {
                    if (taskDate.Value.CompareTo(date) == 0)
                    {
                        result = i;
                        break;
                    }
                    else if (taskDate.Value.CompareTo(date) > 0)
                    {
                        result = i;
                        break;
                    }

                }
            }

            return result;
        }

        private bool readDBFromTextFile()
        {
            bool result = true;
            List<string> data;
            int posLine;
            string line;
            string tmpStr = "";
            Task task = null;





            string fileName = Path.Combine(MH_Utils.Utils.LOG_FILE_PATH, WRITE_LIST_FILE_NAME);

            data = MH_Utils.Utils.TextFileRead(fileName, false);

            if (data == null || data.Count == 0)
            {
                Toast.MakeText(MainContext, "לא נמצא קובץ לייבוא מקובץ", ToastLength.Long).Show();
                return false;
            }


            for (int i = 0; i < TasksList.Count; i++)
            {
                task = TasksList[i];
                DBTaskReminder.DeleteRecord(MainActivity.DB_TABLE_NAME, "ID", task.getTaskID());
            }


            TasksList.Clear();

            for (int i = 0; i < data.Count; i++)
            {
                line = data[i];

                if (string.IsNullOrEmpty(line.Trim()))
                {
                    continue;
                }

                task = new Task();

                posLine = line.IndexOf(CARD_SEPERATOR);     // + LINE_SEPARATOR);

                if (posLine > -1)
                {
                    try
                    {
                        i++;
                        line = data[i];
                        posLine = line.IndexOf(WRITE_DB_PREFIX_ID);
                        if (posLine > -1)
                        {
                            posLine += WRITE_DB_PREFIX_ID.Length;
                            tmpStr = line.Substring(posLine);
                            task.setTitle(tmpStr);
                        }

                        i++;
                        line = data[i];
                        posLine = line.IndexOf(WRITE_DB_PREFIX_TITLE);
                        if (posLine > -1)
                        {
                            posLine += WRITE_DB_PREFIX_TITLE.Length;
                            tmpStr = line.Substring(posLine);
                            task.setTitle(tmpStr);
                        }

                        i++;
                        line = data[i];
                        posLine = line.IndexOf(WRITE_DB_PREFIX_DESC);
                        if (posLine > -1)
                        {
                            posLine += WRITE_DB_PREFIX_DESC.Length;
                            tmpStr = line.Substring(posLine);
                            task.setDescription(tmpStr);
                        }


                        i++;
                        line = data[i];
                        posLine = line.IndexOf(WRITE_DB_PREFIX_DATE);
                        if (posLine > -1)
                        {
                            posLine += WRITE_DB_PREFIX_DATE.Length;
                            tmpStr = line.Substring(posLine);
                            task.setDate_due(tmpStr);
                        }

                        i++;
                        line = data[i];
                        posLine = line.IndexOf(WRITE_DB_PREFIX_TIME);
                        if (posLine>-1)
                        {
                            posLine += WRITE_DB_PREFIX_TIME.Length;
                            tmpStr = line.Substring(posLine);
                            task.setTime_due(tmpStr);
                        }

                        i++;
                        line = data[i];
                        posLine = line.IndexOf(WRITE_DB_PREFIX_REPEAT);
                        if (posLine > -1)
                        {
                            posLine += WRITE_DB_PREFIX_REPEAT.Length;
                            tmpStr = line.Substring(posLine);
                            task.setRepeat(tmpStr);
                        }

                        i++;
                        line = data[i];
                        posLine = line.IndexOf(WRITE_DB_PREFIX_LAST_UPDATE);
                        if (posLine > -1)
                        {
                            posLine += WRITE_DB_PREFIX_LAST_UPDATE.Length;
                            tmpStr = line.Substring(posLine);
                            task.setDate_last_update(tmpStr);
                        }

                        i++;
                        line = data[i];
                        posLine = line.IndexOf(WRITE_DB_PREFIX_BACKGROUND_COLOR);
                        if (posLine > -1)
                        {
                            posLine += WRITE_DB_PREFIX_BACKGROUND_COLOR.Length;
                            tmpStr = line.Substring(posLine);
                            task.setBackgroundColor(tmpStr);
                        }


                        TasksList.Add(task);

                        TBL_Tasks record = new TBL_Tasks();

                        record.Title = task.getTitle();
                        record.Description = task.getDescriptionWithHtml();
                        record.DateDue = task.getDate_due() + " " + task.getTime_due();
                        record.CardBackColor = task.getBackgroundColor();
                        record.LastUpdateDate = task.getDate_last_update();

                        long recorsWasEffected = MainActivity.DBTaskReminder.RecordInser(record, MainActivity.DB_TABLE_NAME);

                        //long recorsWasEffected = DBTaskReminder.RecordInser(task, MainActivity.DB_TABLE_NAME);

                    }
                    catch (Exception ex)
                    {
                        result = false;
                        MH_Utils.Utils.WriteToLog("Error in readDBFromTextFile: " + ex.Message + "\n" + ex.StackTrace);
                        Toast.MakeText(this, "פעולת ייבוא מקובץ נכשלה" + " - בפתק מספר " + task.getDescription() + "  " + ex.Message, ToastLength.Long).Show();
                        result = false;
                        //return result;
                    }
                }
            }

            CurrentTask = null;

            if (result)
            {
                FillList();
                Toast.MakeText(this, "פעולת ייבוא מקובץ עברה בהצלחה", ToastLength.Long).Show();
            }
            else
            {
                Toast.MakeText(this, "פעולת ייבוא מקובץ נכשלה", ToastLength.Long).Show();
            }

            return result;
        }

        private bool writeDBToTextFile()
        {
            string data = "";
            bool result = true;
            StringBuilder stringBuilder = new StringBuilder();
            string line;



            //KeyValuePair<string, int>[] perm = new KeyValuePair<string, int>[] { new KeyValuePair<string, int>(Manifest.Permission.WriteExternalStorage, PERMISSIONS_REQUEST_WRITE_STORAGE) };
            //MH_Utils.Utils.PermissionAsk(perm);

            if (TasksList == null || TasksList.Count == 0)
            {
                Toast.MakeText(this, "פעולת ייצוא לקובץ טקסט נכשלה", ToastLength.Long).Show();
                return false;
            }

            line = "";

            for (int i = 0; i < TasksList.Count; i++)
            {
                try
                {
                    Task task = TasksList[i];

                    line = MH_Utils.Utils.LINE_SEPERATOR + CARD_SEPERATOR + MH_Utils.Utils.LINE_SEPERATOR;
                    line += WRITE_DB_PREFIX_ID + task.getTaskID() + MH_Utils.Utils.LINE_SEPERATOR;
                    line += WRITE_DB_PREFIX_TITLE + task.getTitle() + MH_Utils.Utils.LINE_SEPERATOR;
                    //line += WRITE_DB_PREFIX_DESC + task.getDescriptionWithHtml() + MH_Utils.Utils.LINE_SEPERATOR;
                    line += WRITE_DB_PREFIX_DESC + task.getDescriptionWithHtml() + MH_Utils.Utils.LINE_SEPERATOR;
                    //line += WRITE_DB_PREFIX_DESC + task.getDescriptionPure() + MH_Utils.Utils.LINE_SEPERATOR;
                    line += WRITE_DB_PREFIX_DATE + task.getDate_due() + MH_Utils.Utils.LINE_SEPERATOR;
                    line += WRITE_DB_PREFIX_TIME + task.getTime_due() + MH_Utils.Utils.LINE_SEPERATOR;
                    line += WRITE_DB_PREFIX_REPEAT + task.getRepeat() + MH_Utils.Utils.LINE_SEPERATOR;
                    line += WRITE_DB_PREFIX_LAST_UPDATE + task.getDate_last_update() + MH_Utils.Utils.LINE_SEPERATOR;
                    line += WRITE_DB_PREFIX_BACKGROUND_COLOR + task.getBackgroundColor() + MH_Utils.Utils.LINE_SEPERATOR;

                    stringBuilder.Append(line);
                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, "Error in - Export list to text file: " + ex.Message + MH_Utils.Utils.LINE_SEPERATOR + ex.StackTrace, ToastLength.Long).Show();
                    result = false;
                    continue;
                    //break;
                }
            }


            if (result)
            {
                //data = line;
                data = stringBuilder.ToString();

                string fileName = Path.Combine(MH_Utils.Utils.LOG_FILE_PATH, WRITE_LIST_FILE_NAME);

                result = MH_Utils.Utils.TextFileSave(fileName, data, false, false);

                if (result)
                {
                    string backupFolderName = Android.OS.Environment.DirectoryMusic; 
                    string targetPath = Android.OS.Environment.GetExternalStoragePublicDirectory(backupFolderName).AbsolutePath;
                    //targetPath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
                    //targetPath = MainContext.GetExternalFilesDir("").AbsolutePath;;

                    string fullPathSource = Path.Combine(MH_Utils.Utils.LOG_FILE_PATH, WRITE_LIST_FILE_NAME);
                    string fullPathTarget = Path.Combine(targetPath, WRITE_LIST_FILE_NAME);

                    result = MH_Utils.Utils.CopyFile(fullPathSource, fullPathTarget);

                    Toast.MakeText(this, "פעולת ייצוא לקובץ עברה בהצלחה", ToastLength.Long).Show();
                }
            }
            else
            {
                Toast.MakeText(this, "פעולת ייצוא לקובץ נכשלה", ToastLength.Long).Show();
            }


            return result;
        }



        #region Self override events

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;

            switch (id)
            {
                case Resource.Id.menu_settings:
                    ShowSetting();
                    break;

                case Resource.Id.menu_db_backup:
                    BackupDataBaseFile();
                    break;

                case Resource.Id.menu_db_restore:
                    RestoreDataBaseFile();
                    break;

                case Resource.Id.menu_textfile_write:
                    writeDBToTextFile();
                    break;

                case Resource.Id.menu_textfile_read:
                    readDBFromTextFile();
                    break;

                case Resource.Id.menu_show_log_file:
                    Task task = newTaskDetails();
                    task.setTitle("קובץ Log");
                    string fileName = Path.Combine(MH_Utils.Utils.LOG_FILE_PATH, MH_Utils.Utils.LOG_FILE_NAME);

                    List<string> descs = MH_Utils.Utils.TextFileRead(fileName, false);

                    if (descs != null && descs.Count > 0)
                    {
                        string desc = "";
                        foreach (string line in descs)
                        {
                            desc += line + MH_Utils.Utils.LINE_SEPERATOR;
                        }
                        task.setDescription(desc);
                        OpenTaskDetailsPage(task, false, MainContext);
                        ActivityTaskDetails.isNewMode = true;
                        //EditText txtDetailsDescription = FindViewById<EditText>(Resource.Id.txtDetailsDescription);
                        //txtDetailsDescription.TextDirection = TextDirection.Ltr;
                    }
                    break;
            }

            return base.OnOptionsItemSelected(item);
        }


        protected override void OnActivityResult(int requestCode, Result resultCode, Intent inputIntent)
        {

            if (inputIntent != null)
            {
                int taskID = inputIntent.GetIntExtra("TaskID", 0);
            }

            if (resultCode == Result.Ok)
            {
                switch (requestCode)
                {
                    case SHOW_SCREEN_SETTING:
                        {
                            SaveSettingsValues(inputIntent);
                        }
                        break;
                }

                OnExitResult(requestCode, resultCode, inputIntent);

            }

        }

        protected override void OnDestroy()
        {
            string message="";
                
                
            DBTaskReminder.DB.Close();

            BackupDataBaseFile(false);

            bool isMedyaPlayerClosed = ActivityMusic.MusicStopFinal();

            if (isMedyaPlayerClosed)
            {
                //Toast.MakeText(this, "Destroy Medya Player", ToastLength.Long).Show();
            }

            Toast.MakeText(this, "Close Application", ToastLength.Long).Show();

            MH_Utils.Utils.WriteToLog("Task Reminder app 'OnDestroy' - System close Task Reminder application");


            // Just tto use same constants values
            if (this.serviceStayAlive != null && ServiceKeppAliveIntent==null)
            {
                serviceStayAlive.OnDestroy();
                serviceStayAlive = null;
            }
            else
            {
                serviceStayAlive = new MHServiceKeepAlive.ServiceStayAlive();
            }

            // The latter may seem rather peculiar: why do we want to stop exactly the service that we want to keep alive? 
            // Because if we do not stop it, the service will die with our app.Instead, by stopping the service, 
            // we will force the service to call its own onDestroy which will force it to recreate itself after the app is dead.

            //	Why can't I simply send a message to the broadcast receiver directly from the Activity's onDestroy
            // You can but in that case the service will not
            //be restarted if your app is in the background and Android decides to kill your service because it needs resources.I.e.your service would not be guaranteed to work indefinitely

            // Its mean, this start the Service in Beginning
            // What is I do not want the counter to restart when the process is killed ?
            // Yes this is the usual case. Well you cannot directly. The only way is to save the status of the service and to reload it when the service is started.You will do this by using the following code:
            if (ServiceKeppAliveIntent != null)
            {
                // Just to prevent execute ReStart again in 'Service.OnDestroy()'
                Bundle bundle = ServiceKeppAliveIntent.GetBundleExtra(serviceStayAlive.INTENT_KEY_CALLER_BUNDLE);
                // Prevent ReStart again this activity on 'Service:OnDestroy()'
                bool isServiceRestartOnServiceDestroy = bundle.GetBoolean(serviceStayAlive.INTENT_KEY_CALLER_IS_RESTART_ON_DESTROY, true);

                // If not null, mea Service was start in begining - Must stop him
                //if (isServiceRestartOnServiceDestroy)
                //{
                    message = "Task Reminder:OnDestroy - Going to Stop Service";
                    MH_Utils.Utils.WriteToLog(message);
                    Toast.MakeText(this, message, ToastLength.Long).Show();

                    StopService(ServiceKeppAliveIntent);

                    ServiceKeppAliveIntent = null;
                //}
                // Not Relevant - Servie was start in Begining
                //else
                //{
                //    bool isReStartAtBeginingOfService = bundle.GetBoolean(serviceStayAlive.INTENT_KEY_CALLER_IS_RESTART_ON_DESTROY);
                //    //bool isReStartAtBeginingOfService = bundle.GetBoolean(serviceStayAlive.INTENT_KEY_CALLER_IS_RESTART_ON_START);
                //    if (isReStartAtBeginingOfService)
                //    {
                //        message = "Task Reminder:OnDestroy - Going to Start Service";
                //        MH_Utils.Utils.WriteToLog(message);
                //        Toast.MakeText(this, message, ToastLength.Long).Show();

                //        ServiceKeepAliveHandle(InputSavedInstanceState);
                //    }
                //}
            }
            else
            {
                message = "Task Reminder:OnDestroy - Going to Start Service  (first time on OnDestroy event)";
                MH_Utils.Utils.WriteToLog(message);
                Toast.MakeText(this, message, ToastLength.Long).Show();

                ServiceKeepAliveHandle(InputSavedInstanceState);
            }


            base.OnDestroy();
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            outState.PutBoolean(SERVICE_STARTED_KEY, isServiceStarted);
            base.OnSaveInstanceState(outState);
        }

        protected override void OnRestoreInstanceState(Bundle savedInstanceState)
        {
            base.OnRestoreInstanceState(savedInstanceState);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            string message = string.Empty;


            if (grantResults.Length == 0)
            {
                return;
            }

                // Received permission result for permission.
            switch (requestCode)
            {
                case PERMISSIONS_REQUEST_READ_STORAGE:
                    {
                        // Check if the only required permission has been granted
                        if (grantResults[0] == Android.Content.PM.Permission.Granted)
                        {
                            // Location permission has been granted, okay to retrieve the location of the device.
                            message = "Read permission - Granted.";
                        }
                        else
                        {
                            message = "Read permission - NOT Granted.";
                        }
                    }
                    break;

                case PERMISSIONS_REQUEST_WRITE_STORAGE:
                    {
                        // Check if the only required permission has been granted
                        if (grantResults[0] == Android.Content.PM.Permission.Granted)
                        {
                            // Location permission has been granted, okay to retrieve the location of the device.
                            message = "Write permission - Granted.";
                        }
                        else
                        {
                            message = "Write permission - NOT Granted.";
                        }
                    }
                    break;

                default:
                    base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
                    break;
            }


            if (message != string.Empty)
            {
                //Toast.MakeText(MainContext, message, ToastLength.Long).Show();

                //Toast.MakeText(this, (Android.Support.V4.Content.ContextCompat.CheckSelfPermission(this, Android.Manifest.Permission.ReadExternalStorage) == (int)Android.Content.PM.Permission.Granted).ToString(), ToastLength.Long);
                //View view = LayoutInflater.From(this).Inflate(Android.Resource.Layout.ActivityListItem, null, false);
                //Snackbar.Make(view, "granted", Snackbar.LengthLong).Show();            }
            }
        }

    }
    
    #endregion Self override events

}

