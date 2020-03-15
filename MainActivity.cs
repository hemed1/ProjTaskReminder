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
using System.Threading;
using System.IO;
using Java.Util;
using Android.Text;

using ProjTaskReminder.Model;
using ProjTaskRemindet.Utils;
using ProjTaskReminder.Data;
using ProjTaskReminder.Utils;
using Android.Graphics.Drawables;
using static ProjTaskReminder.Utils.Utils;

//using System.Threading.Timer;


namespace ProjTaskReminder
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity   //, ListView.IOnItemClickListener
    {

        public static string DB_TASK_DB_NAME = "TaskReminder.db";
        public static string DB_TASK_DB_PATH = "TaskReminder.db";
        public static string DB_TABLE_NAME = "TBL_Tasks";
        public static string DB_TABLE_SETTING = "TBL_Setting";
        private static string DB_FIELDNAME_ID = "TaskID";
        private static string DB_FIELDNAME_TITLE = "Title";
        private static string DB_FIELDNAME_DESC = "Description";
        private static string DB_FIELDNAME_DATE = "DateDue";
        private static string DB_FIELDNAME_REPEAT = "Repeat";
        private static string DB_FIELDNAME_COLOR = "Color";
        private static string DB_FIELDNAME_LAST_UPDATE = "LastUpdate";
        private static string DB_FIELDNAME_IS_ARCHIVE = "IsArchive";

        // The code for get result from TasDetails screen
        public const int SHOW_SCREEN_TASK_DETAILS = 1234;

        //ImageButton btnMainMusic = (ImageButton)FindViewById(Resource.Id.bntMainMusic);
        //private ImageView btnMainWeather;
        //ImageButton btnMainEmail = (ImageButton)FindViewById(Resource.Id.btnMainEmail);
        //private CardView cardWeather;

        private ListView simpleList;    //RecyclerView
        private ListViewAdapter listViewAdapter;
        public static DBTaskReminder DBTaskReminder;
        private List<Task> TasksList = new List<Task>();
        private List<Weather> WeatherList = new List<Weather>();
        private int WeatherCounter=0;
        private MH_Weather mH_Weather;
        private MH_Scroll WeatherScroll;
        private MH_Scroll NewsScroll;
        private TextView txtRssNews;
        private HorizontalScrollView scrollWeather;
        private HorizontalScrollView scrollNews;
        private Thread ThreadNews;


        private static Context context;
        //private readonly string[]  countryList = new string[6] { "India", "China", "australia", "Portugle", "America", "NewZealand" };

        public static Task CurrentTask;
        public static List<object[]> TimersArray;
        public static string MainMessageText;
        public static int DefaultCardBackgroundColor;
        public static bool isShowTimerReminder;
        private event Action ActionOnTaskDateDue;
        private int TimerInterval;
        private MH_Notification mh_Notification;

        private Android.Support.V4.App.NotificationCompat.Builder notificationBuilder;
        private TaskTimerElapsed ActionOnTaskTimerTick;     // , ElapsedEventHandler

        public delegate ElapsedEventHandler delegateMethod(object timer, ElapsedEventArgs args, Task TaskObject);



        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            //Utils.Utils.WriteToLog("Enter 1", true);

            context = this.ApplicationContext;

            Initialize();

            ConnectToDB();

            //BackupDataBaseFile();

            FillListFromDB();

            focusListOnToday(Utils.Utils.GetDateNow());

            //StartRssNewsScroll();

            // First city
            //Weather weather;
            //weather = mH_Weather.GetWather("Ashdod");
            //WeatherList.Add(weather);
            //OnWeatherChangingPlace(weather, 0);

            //WeatherScroll.StartPosstion();
        }

        public void StartRssNewsScroll(object sender, EventArgs e)
        {

            ThreadNews = new Thread(new ThreadStart(StartGetNews));
            //ThreadNews.IsBackground = true;

            ThreadNews.Start();

        }

        private void StartGetNews()
        {

            activity.RunOnUiThread(GetRssNewsScroll);

        }

        private void GetRssNewsScroll()
        {
            List<XmlItem> items;

            items = Utils.Utils.OpenXmlData(Utils.Utils.NEWS_RSS_ADDRESS2);

            string news = string.Empty;
            string pubDate = string.Empty;

            for (int i=0; i<items.Count; i++)
            {
                XmlItem newsItem = items[i];

                pubDate = newsItem.PublishDate;
                //if (Utils.Utils.GetDateNow().Date.CompareTo(Utils.Utils.getDateFromString(newsItem.PublishDate))==0)
                //{
                //    //pubDate = pubDate.Substring(11, 2) + ":" + pubDate.Substring(14, 2);
                //}
                news += pubDate + " - " +
                        newsItem.Title; // + ", " +
                        //newsItem.Description + ".  ";
            }

            txtRssNews.Text = news;

            ThreadNews.Abort();
            ThreadNews = null;

            NewsScroll.SCROLL_END_POINT = 12000;     // txtRssNews.Width-500;
            NewsScroll.Start();
        }

        private void SetControlsIO()
        {
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            FloatingActionButton btnMainNew = FindViewById<FloatingActionButton>(Resource.Id.btnMainNew);
            btnMainNew.Click += btnMainNew_OnClick;
            //btnMainNew.SetBackgroundResource(Android.Resource.Drawable.IcMediaNext);

            //FloatingActionButton btnMainDelete = FindViewById<FloatingActionButton>(Resource.Id.btnMainDelete);
            //btnMainDelete.Click += btnMainDelete_OnClick;
            //btnMainDelete.SetBackgroundResource(Android.Resource.Drawable.IcDelete);

            ActivityTaskDetails.OnSaveButton += new EventHandler(TaskDetailsSave_Click);
            ActivityTaskDetails.OnActivityResult += OnActivityResult;


            TasksList = new List<Task>();
            listViewAdapter = new ListViewAdapter(context, TasksList);



            //listViewAdapter.SetOnClickListener += new EventHandler(OnItemClick);
            //listViewAdapter.SetOnItemClick += new EventHandler(OnItemClickFromAdapter);

            simpleList = (ListView)FindViewById(Resource.Id.simpleListView);        // RecyclerView

            simpleList.ItemClick += OnItemClick;
            //simpleList.SetOnClickListener(simpleListItem_DoubleClick);
            //simpleList.OnItemClickListener += OnItemClick


            ImageButton btnMainMusic = (ImageButton)FindViewById(Resource.Id.bntMainMusic);
            //btnMainWeather = (ImageView)FindViewById(Resource.Id.btnMainWeather);
            ImageButton btnMainEmail = (ImageButton)FindViewById(Resource.Id.btnMainEmail);
            CardView cardWeather = (CardView)FindViewById(Resource.Id.cardWeather);
            scrollWeather = (HorizontalScrollView)FindViewById(Resource.Id.scrollWeather);
            scrollNews = (HorizontalScrollView)FindViewById(Resource.Id.scrollNews);
            txtRssNews = (TextView)FindViewById(Resource.Id.txtRssNews);

            btnMainMusic.Click += btnMainMusic_Click;
            //btnMainWeather.Click += btnMainWeather_Click;
            btnMainEmail.Click += StartRssNewsScroll;
            //cardWeather.Click += btnMainWeather_Click;
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

            // RecyclerView
            //LinearLayoutManager mLayoutManager = new LinearLayoutManager(this);
            //simpleList.SetLayoutManager(mLayoutManager);

            //simpleList.setHasFixedSize(true);
            //simpleList.SetLayerType(new LinearLayout(this), new Android.Graphics.Paint());


            //public event EventHandler YourEvent;
            //        or
            //public event EventHandler YourEventWithParameters;
            //        Fire the eventhandler like this:
            //        if (YourEvent!= null)
            //        {
            //              YourEvent(this, EventArgs.Empty);
            //        }
            //            then If you have a second class Class2 , and the event handler is present in Class1
            //            So inside Class2
            //            you create an object of class1.
            //            Class1 obj = new Class1();
            //            Class1.YourEvent+=OnEventHandlingInClass2;
            //          The signature of OnEventHandlingInClass2 will be:
            //          void OnEventHandlingInClass2(object sender, EventArgs e)
            //        {
            //        }
            //        or in second scenario.
            //        Class1.YourEventWithParameters+=OnEventHandlingInClass2;
            //        The signature of OnEventHandlingInClass2 will be:
            //void OnEventHandlingInClass2(object sender, parameer T)
            //        {
            //        }

            //ActivityTaskDetails.OnSaveButton(new ActivityTaskDetails.OnSaveButtonInterface()
            //{
            //    public void SetSaveButton(long recordsEffected)
            //    {
            //        //updateTaskInlist();
            //    }
            //});
            //new System.Windows.RoutedEventHandler(btnOkClick);
            //ActivityTaskDetails.btnSave_Click += (object sender, EventArgs eventArgs) =>     //new btnSave_Click<object, EventArgs>()
            //ActivityTaskDetails.btnSave_Click(null, null);     //new btnSave_Click<object, EventArgs>()
            //ActivityTaskDetails.btnSave_Click += ActivityTaskDetails.OnSaveButtonInterface;     // (null, null);           //(new ActivityTaskDetails.OnSaveButton()
            //ActivityTaskDetails.btnSave_Click += (object sender, EventArgs eventArgs) =>
            //{
            //public override void SetSaveButton(long recordsEffected)
            //{

            //}
            //});
            //{
            //    FillList();
            //};



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
                int resourceIDPlace=0;
                int resourceIDTemperture=0;
                int resourceIDDescription=0;
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
                    Utils.Utils.WriteToLog("Error when try to set Weater icon: " + weather.getPoster() + Utils.Utils.LINE_SEPERATOR + ex.Message);
                }
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
                WeatherScroll.Start();
                mH_Weather.StartChangePlace();
            }

        }

        private void Initialize()
        {

            SetControlsIO();

            TimersArray = new List<object[]>();

            TimerInterval = 600000;  // 1000 * 60 * 30

            //mainActivityServices = new MainActivityServices(MainActivity.this, this, this);
            //IsManualHtml = false;
            //setControlsColors();

            Utils.Utils.activity = this;
            Utils.Utils.context = context;
            Utils.Utils.LOG_FILE_NAME = "LogTaskReminder.txt";
            Utils.Utils.LOG_FILE_PATH = context.GetExternalFilesDir("").AbsolutePath;
            //string folderBackup = Android.OS.Environment.DirectoryMusic;        // "ProjTaskReminder"
            //LOG_FILE_PATH = Android.OS.Environment.GetExternalStoragePublicDirectory(folderBackup).AbsolutePath;
            //LOG_FILE_PATH = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;




            // Open Notification Channel. Must be in the start init
            // Done again in 'Util.createNotificationBuilder()'
            mh_Notification = new MH_Notification(this, context);
            mh_Notification.createNotificationChannel();

            WeatherList = new List<Weather>();
            mH_Weather = new MH_Weather();
            mH_Weather.activity = this;
            mH_Weather.WEATER_CHANE_PLACE_TIMER_INTERVAL = 60000;
            mH_Weather.OnChanePlace += OnWeatherChangingPlace;

            WeatherScroll = new MH_Scroll();
            WeatherScroll.SCROLL_DELTA = 6;
            WeatherScroll.SCROLL_END_POINT =980;
            WeatherScroll.SCROLL_INTERVAL = 300;
            WeatherScroll.ScrollControl = scrollWeather;
            //WeatherScroll.OnScrolling += OnWeatherScroll;

            NewsScroll = new MH_Scroll();
            NewsScroll.SCROLL_DELTA = 40;
            NewsScroll.SCROLL_END_POINT = 980;
            NewsScroll.SCROLL_INTERVAL = 200;
            NewsScroll.ScrollControl = scrollNews;
            //WeatherScroll.OnScrolling += OnWeatherScroll;

        }

        private void OnWeatherScroll(int obj)
        {
            
        }

        private void btnMainMusic_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(ActivityMusic));
            //intent.setFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP);

            intent.PutExtra("TaskID", "Meir");
            //intent.putExtra("task", task);  //TODO: Seriize

            //ActivityTaskDetails.isNewMode = isNewMode;
            //ActivityTaskDetails.CurrentTask = task;
            ////ActivityTaskDetails.dbHandler = MainActivity.dbHandler;
            ActivityTaskDetails.context = context;      //Application.Context;
            ////ActivityTaskDetails.mainActivity = mainActivity;

            StartActivityForResult(intent, 2345);
            //context.StartActivity(intent);
        }


        private void FillListFromDB()
        {
            TasksList = GetTasksFromDB();

            FillList();
        }

        private void FillList()
        {

            Toast.MakeText(this, "Fill list with items...", ToastLength.Short).Show();



            killOldTimers();


            for (int i = 0; i < TasksList.Count; i++)
            {
                try
                {
                    Task task = TasksList[i];

                    //TimerStop(task);

                    TimerRun(task);

                }
                catch (Exception ex)
                {
                    //Log.d("fillList Error: ", ex.getMessage());
                    //ex.printStackTrace();
                }
            }


            SortList();

            //RefreshListAdapter();


            //for (int i = 0; i < countryList.Length; ++i)
            //{
            //    Task task = new Task();
            //    task.setTitle(countryList[i]);
            //    task.setDescription(countryList[i]);
            //    task.setDate_due(DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
            //    list.Add(task);
            //}
            // Directly from array
            //ArrayAdapter<String> arrayAdapter = new ArrayAdapter<String>(this, Resource.Layout.list_item, Resource.Id.txtTitle, countryList);
            //simpleList.SetAdapter(arrayAdapter);
        }

        private void SortList()
        {
            int id = -1;



            Toast.MakeText(this, "Refresh items...", ToastLength.Short).Show();

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
        private void RefreshListAdapter()
        {
            listViewAdapter = new ListViewAdapter(context, TasksList);
            simpleList.SetAdapter(listViewAdapter);

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
                ScroolListToItem(index);
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
                ScroolListToItem(index);
            }
        }

        private void ScroolListToItem(int index)
        {

            simpleList.SmoothScrollToPosition(index);
            
            //View itemView = simpleList.GetChildAt(index);
            //if (itemView != null)
            //{
            //    int scrolly = (-1 * itemView.Top) + simpleList.FirstVisiblePosition * itemView.Height;
            //    scrolly = (int)itemView.GetY();
            //    simpleList.ScrollY = scrolly;
            //}

            //simpleList.ScrollChange += (o, e) =>
            //{
            //    if (simpleList.FirstVisiblePosition == 0 && Convert.ToInt32(simpleList.GetChildAt(index).GetY()) == 0)
            //    {
            //        // Reach top
            //    }
            //};
            //int pos = simpleList.FirstVisiblePosition;
            //simpleList.VerticalScrollbarPosition(index);
            //listRecycler.scrollToPosition(index);

        }
        private bool ConnectToDB()
        {
            bool result = true;


            try
            {
                DB_TASK_DB_PATH = context.GetExternalFilesDir("").AbsolutePath;

                DBTaskReminder = new DBTaskReminder(DB_TASK_DB_NAME, DB_TASK_DB_PATH, DB_TABLE_NAME);
            }
            catch (Exception ex)
            {
                Utils.Utils.WriteToLog(ex.Message, true);
            }

            // /storage/emulated/0/Music/
            // /storage/emulated/0/Android/data/com.meirhemed.projtaskreminder/files
            //DB_PATH = context.GetExternalFilesDir("").AbsolutePath;         
            //DB_PATH = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);  //"/Assets/" + DB_NAME;             //Environment.SpecialFolder.LocalApplicationData

            return result;
        }

        private List<Task> GetTasksFromDB()
        {
            Toast.MakeText(this, "Load items from Database", ToastLength.Short).Show();


            TasksList.Clear();

            TableQuery<TBL_Tasks> table = DBTaskReminder.DB.Table<TBL_Tasks>();

            //var stock = DBTaskReminder.DB.Get<TBL_Tasks>(2); // primary key id of 5
            //var stockList = DBTaskReminder.DB.Table<TBL_Tasks>();


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
                task.TableRecord = record;

                TasksList.Add(task);
            }

            return TasksList;
        }

        public static List<KeyValuePair<String, String>> SetTaskValuesForDB(Task currentTask)
        {
            List<KeyValuePair<string, string>> values = new List<KeyValuePair<string, string>>();
            //List<KeyValuePair<string,CharSequence>> values2 = new ArrayList<>();
            string strDateTime = "";


            if ((!currentTask.getDate_due().Equals("")) && (!currentTask.getTime_due().Equals("")))
            {
                strDateTime = currentTask.getDate_due() + " " + currentTask.getTime_due();
            }

            /// TODO: Primary key - no needvalues.Add(new KeyValuePair<string, string>(DB_FIELDNAME_ID, currentTask.getTaskID().ToString()));
            ///
            values.Add(new KeyValuePair<string, string>(DB_FIELDNAME_TITLE, currentTask.getTitle()));
            //values.Add(new KeyValuePair<string, string>(DB_FIELDNAME_DESC, currentTask.getDescription);     // Html.toHtml(currentTask.getDescription())
            values.Add(new KeyValuePair<string, string>(DB_FIELDNAME_DESC, currentTask.getDescriptionWithHtml()));
            values.Add(new KeyValuePair<string, string>(DB_FIELDNAME_DATE, strDateTime));
            //values.Add(new KeyValuePair<string, string>(DB_FIELDNAME_COLOR, currentTask.getBackgroundColor()));
            //values.Add(new KeyValuePair<string, string>(DB_FIELDNAME_REPEAT, currentTask.getRepeat()));
            //values.Add(new KeyValuePair<string, string>(DB_FIELDNAME_LAST_UPDATE, currentTask.getDate_last_update()));
            //values.Add(new KeyValuePair<string, string>(DB_FIELDNAME_IS_ARCHIVE, ((currentTask.getIsArchive()) ? "1" : "0")));
            //Log.d("Set asArchive", "getTaskValues(): "+string.valueOf(CurrentTask.getIsArchive()));

            return values;
        }

        private void OpenTaskDetailsPage(Task task, bool isNewMode, Context context)
        {

            if (!isNewMode)
            {
                //Log.d("Open Update task screen - TaskID: ", String.valueOf(taskID));    //, "Is MainActivity finished: "+ this.isFinishing());
                //task = getTaskInListByID(tasksList, taskID);
                //if (task == null)
                //{
                //    return null;
                //}
            }
            else
            {
                task = newTaskDetails();
            }

            Intent intent = new Intent(this, typeof(ActivityTaskDetails));
            //intent.setFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP);

            intent.PutExtra("TaskID", task.getTaskID());
            //intent.putExtra("task", task);  //TODO: Seriize

            CurrentTask = task;

            ActivityTaskDetails.isNewMode = isNewMode;
            ActivityTaskDetails.CurrentTask = task;
            ////ActivityTaskDetails.dbHandler = MainActivity.dbHandler;
            ActivityTaskDetails.context = context;      //Application.Context;
            ////ActivityTaskDetails.mainActivity = mainActivity;

            StartActivityForResult(intent, SHOW_SCREEN_TASK_DETAILS);
            //context.StartActivity(intent);

            //var req = new Intent();
            //req.SetComponent(new ComponentName("com.example.Tristan.Android", "com.example.Tristan.Android.IsoldeQueryActivity"));
            //StartActivityForResult(req, SHOW_SCREEN_TASK_DETAILS);
            //var rtn = await TristanStateCompletion.Task;
            //if (!rtn) bomb("can't get state");
            //TristanStateCompletion = null;

        }

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
                    break;

                case Resource.Id.menu_db_backup:
                    BackupDataBaseFile();
                    break;

                case Resource.Id.menu_db_restore:
                    RestoreDataBaseFile();
                    break;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void OnItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            CurrentTask = TasksList[e.Position];

            OpenTaskDetailsPage(CurrentTask, false, Application.Context);

            //View.IOnClickListener ii = simpleListItem_DoubleClick(sender, e);

            //OnItemClickFromAdapter(sender, e);
        }

        public View.IOnClickListener simpleListItem_DoubleClick(View.IOnClickListener vvv)
        {
            OpenTaskDetailsPage(CurrentTask, false, Application.Context);

            return null;    // View.IOnClickListener;
        }

        private void OnItemClickFromAdapter(object sender, EventArgs e)
        {
            CurrentTask = TasksList[simpleList.SelectedItemPosition];

            ActivityTaskDetails.CurrentTask = CurrentTask;

            //Toast.MakeText(context, "Click", ToastLength.Long).Show();
        }

        private void btnMainNew_OnClick(object sender, EventArgs eventArgs)
        {
            View view = (View)sender;

            //Toast.MakeText(view.Context, "Click", ToastLength.Long).Show();

            //Task task = new Task();

            OpenTaskDetailsPage(CurrentTask, true, Application.Context);

            //Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
            //              .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
        }

        //private void btnMainDelete_OnClick(object sender, EventArgs eventArgs)
        //{
        //    View view = (View)sender;

        //simpleList.Selected;
        //simpleList.SelectedItem;
        //simpleList.SelectedView;

        //if (simpleList.SelectedItemPosition > 0 && simpleList.SelectedItemPosition < TasksList.Count)
        //{
        //    Task task = TasksList[simpleList.SelectedItemPosition];

        //    DBTaskReminder.DB.Delete<TBL_Tasks>(task.getTaskID());  // "ID==" +currentTask.getTaskID().ToString(), null);

        //    Toast.MakeText(view.Context, "Deletet", ToastLength.Long).Show();

        //    FillList();
        //}

        //Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
        //              .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
        //}

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public void TaskDetailsSave_Click(object sender, EventArgs e)
        {
            FillList();
        }

        //protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        //{
        //this.OnActivityResult(requestCode, resultCode, data);
        //if (requestCode == SHOW_SCREEN_TASK_DETAILS)
        //{
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            //base.OnActivityReenter(requestCode,resultCode, data);

            if (resultCode == Result.Ok)
            {
                switch (requestCode)
                {
                    case SHOW_SCREEN_TASK_DETAILS:
                        {
                            if (ActivityTaskDetails.isNewMode)
                            {
                                TimerRun(CurrentTask);
                                TasksList.Add(CurrentTask);
                                SortList();
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
                    // Delete
                    case 999:
                        {
                            TimerStop(CurrentTask);
                            TasksList.Remove(CurrentTask);
                            SortList();
                            //FillList();
                            break;
                        }
                }

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


            DateTime today = Utils.Utils.GetDateNow();       //Util.GetDateNow.getTime();

            if (timerDate.Value.CompareTo(today) <= 0)
            {
                // Show mssage if there is message in que
                showGlobalMessageOnToast();
                return;
            }


            //Toast.MakeText(this, "Task date: " + Utils.Utils.getDateFormattedString(timerDate.Value) + "     Now: " + Utils.Utils.getDateFormattedString(Utils.Utils.GetDateNow()), ToastLength.Long).Show();

            CurrentTask = currentTask;
            ActionOnTaskDateDue += TimerNextTime;

            TimerInterval = 30000;  // 30 seconds


            TimerExecute2(currentTask);
            //TimerExecute(currentTask);
            //TimerExecute3(currentTask, timerDate);


            dateStr = Utils.Utils.getDateFormattedString(timerDate.Value);

            if (isShowTimerReminder && !MainMessageText.Trim().Equals(""))
            {
                string text = "יצר תזכורת ביום " + Utils.Utils.getDateDayName(timerDate.Value) + "  " + dateStr;        // + "  (Now: " + Utils.Utils.getDateFormattedString(Utils.Utils.GetDateNow()) + ")";
                MainMessageText = MainMessageText + " - " + text;
                showGlobalMessageOnToast();
            }

            isShowTimerReminder = false;


            //MyTask timerTask = new MyTask(timer, currentTask);
            //timerTask.IntervalTime = 36000;
            //timerTask.ActionOnPlayingMusic += picsTimer_onTick;     // (currentTask, null);  //timerTask.picsTimer_onTick;

        }

        #region Timer as System.Threading.Thread

        private void TimerExecute(Task currentTask)
        {
            System.Threading.Thread timer = System.Threading.Thread.CurrentThread;

            TaskTimerElapsed  ActionOnTaskTimerTick = new TaskTimerElapsed(currentTask, timer);
            ActionOnTaskTimerTick.TimerInterval = TimerInterval;
            ActionOnTaskTimerTick.activity = this;
            ActionOnTaskTimerTick.OnThreadTick += Thread_Elapsed;

            timer = new System.Threading.Thread(new ThreadStart(ActionOnTaskTimerTick.Thread_Elapsed));        //TimerThreadExecute));

            // Before, just for declare
            ActionOnTaskTimerTick.ThreadObject = timer;

            //Thread second = new Thread(new ThreadStart(secondThread));


            currentTask.setTimer(timer);

            addTimerToKillArray(currentTask.getTaskID(), currentTask);     // timer timerTask);


            timer.Start();
        }

        private void Thread_Elapsed(Task currentTask, System.Threading.Thread timerObject)     // ElapsedEventHandler object sender, ElapsedEventArgs e, 
        {
            TimerStop(currentTask);

            // Do the Job - Notification
            picsTimer_onTick(currentTask);

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


        //    DateTime dateNow = Utils.Utils.GetDateNow();
        //    string strDateNow = Utils.Utils.getDateFormattedString(dateNow);
        //    string strDateTask = Utils.Utils.getDateFormattedString(currentTask.getDate().Value);


        //    while (counter <= limitSeconds && strDateNow.CompareTo(strDateTask) < 0)
        //    {
        //        Thread.Sleep(TimerInterval);

        //        RunOnUiThread(() =>
        //        {
        //            dateNow = Utils.Utils.GetDateNow();
        //            strDateNow = Utils.Utils.getDateFormattedString(dateNow);
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
        //    picsTimer_onTick(currentTask);


        //    return null;    // new ElapsedEventHandler(null, new ElapsedEventArgs());
        //}

        #endregion


        #region Timer as System.Timers.Timer

        private void TimerExecute2(Task currentTask)
        {
            System.Timers.Timer timer = new System.Timers.Timer();

            currentTask.setTimer(timer);

            addTimerToKillArray(currentTask.getTaskID(), currentTask);

            ActionOnTaskTimerTick = new TaskTimerElapsed(currentTask, timer);
            ActionOnTaskTimerTick.TimerInterval = TimerInterval;
            ActionOnTaskTimerTick.OnTimerTick += Timer_Elapsed;

            //timer.BeginInit();   
            timer.AutoReset = true;         // Continue repeatly fire events
            timer.Elapsed += ActionOnTaskTimerTick.Timer_Elapsed;
            timer.Interval = TimerInterval; // 30 seconds
            timer.Enabled = true;
            //timer.Start();

          }

        //ActionOnTaskTimerTick += MainActivity_ActionOnTaskTimerTick;
        //ActionOnTaskTimerTick += delegateMethod2;  
        //ActionOnTaskTimerTick.Invoke(timer, null);  //, currentTask);
        //timer.Elapsed += ActionOnTaskTimerTick;
        //timer.Elapsed += MainActivity_ActionOnTaskTimerTick;
        //timer.ScheduleAtFixedRate(customTimerTask, new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute), 5000);
        //private void MainActivity_ActionOnTaskTimerTick(object sender, ElapsedEventArgs e)
        //{
        //    ActionOnTaskTimerTick(sender, e, CurrentTask);
        //}
        //private void delegateMethod2(object timer, ElapsedEventArgs args, Task currentTask)
        //{
        //    throw new NotImplementedException();
        //}

        private void Timer_Elapsed(Task currentTask, System.Timers.Timer timerObject)     // ElapsedEventHandler object sender, ElapsedEventArgs e, 
        {
            // Cant in Threading proccess
            //Toast.MakeText(this, "Timer Ticking...", ToastLength.Short).Show();

            //Task currentTask = CurrentTask;

            // TODO: Done  'TaskTimerElapsed.Timer_Elapsed()'
//            DateTime dateNow = Utils.Utils.GetDateNow();
//            string strDateNow = Utils.Utils.getDateFormattedString(dateNow);
//            string strDateTask = Utils.Utils.getDateFormattedString(currentTask.getDate().Value);

//            if (strDateNow.CompareTo(strDateTask) >= 0)
//            {
//                System.Timers.Timer timer = timerObject;
                //System.Timers.Timer timer = ((System.Timers.Timer)sender);
//                timer.Stop();
//                timer.Close();
//                timer.Dispose();
//                timer = null;

                // No need. allready killed. just for shure.
                TimerStop(currentTask);

                // Do the Job - Notification
                picsTimer_onTick(currentTask);
 //           }

            //return null;
        }

        #endregion

        private void TimerStop(Task currentTask)
        {
            int index = searchIDInTimersList(currentTask.getTaskID());

            if (index == -1)
            {
                return;
            }

            TimerKill(index);       //, currentTask);
        }

        private void TimerKill(int timerArrayIndex)     //, Task currentTask)
        {
            try
            {
                if (timerArrayIndex>=TasksList.Count)
                {
                    return;
                }

                object[] taskTimerArray = TimersArray[timerArrayIndex];

                if (taskTimerArray[1] == null)
                {
                    return;
                }

                Task currentTask = (Task)taskTimerArray[1];
                System.Timers.Timer timer = currentTask.getTimer();

                //System.Timers.Timer timer = (System.Timers.Timer)taskTimerArray[1];
                //System.Threading.Thread timer = (Thread)timersArray[1];
                //Java.Util.Timer timer = (Java.Util.Timer)timersArray[1];
                //TimerTask timerTask = (TimerTask)timersArray[2];

                if (timer != null)  // && timer.Enabled)
                //if (timer != null && timer.IsAlive)
                {
                    TimerDispose(timer);
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
                }

                TimersArray.RemoveAt(timerArrayIndex);

                currentTask.setTimer(timer);
                currentTask.setTimer_task(null);
            }
            catch (Exception ex)
            {
                Utils.Utils.WriteToLog("TimerKill: " + Utils.Utils.LINE_SEPERATOR + ex.Message);
            }
        }

        private void TimerDispose(System.Timers.Timer timer)
        {
            timer.Stop();
            timer.Close();
            timer.Dispose();

            timer = null;

        }

        #region Timer as Java.Util.Timer

        private void TimerExecute3(Task currentTask, Date timerDate)
        {
            Java.Util.TimerTask timerTask = null;  // = new Java.Util.TimerTask(picsTimer_onTick);
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
            //    picsTimer_onTick(currentTask);
            //}
            //
            //            timer.Schedule(timerTask, 4, 3000);  //, timerDate);
            //timer.Notify();

        }

        #endregion


        private void picsTimer_onTick(Task task)
        {
            // Play Notification sound
            int resourceID = Resource.Raw.deduction;
            Android.Media.MediaPlayer mediaPlayer = Android.Media.MediaPlayer.Create(this, resourceID);   // uri);
            mediaPlayer.SetScreenOnWhilePlaying(true);
            mediaPlayer.Start();

            //Toast.MakeText(this, "Timer is tiking: "+task.getDescription(), ToastLength.Long).Show();

            showNotifications(task);
        }

        private void TimerNextTime()    // string ss)
        {
            DateTime dateNow = Utils.Utils.GetDateNow();
            string strDateNow = Utils.Utils.getDateFormattedString(dateNow);

            //return strDateNow;
        }

        private void addTimerToKillArray(int taskID, Task currentTask)       
        {
            object[] timersArray = new object[2];

            timersArray[0] = taskID;
            //timersArray[1] = timer;
            timersArray[1] = currentTask;

            TimersArray.Add(timersArray);
        }

        private static int searchIDInTimersList(int id)
        {
            int result = -1;

            for (int i = 0; i < TimersArray.Count; i++)
            {
                object[] timersArray = TimersArray[i];

                if ((int)timersArray[0] == id)
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
                TimerKill(i);   //, (Task)timersArray[1]);     // searchTaskByID((int)timersArray[0]));
            }

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
                Toast.MakeText(context, MainMessageText, ToastLength.Long).Show();
                MainMessageText = "";
            }
        }

        protected void showNotifications(Task currentTask)
        {
            //Util.turnOnScreen(this);

            // Show android notification
            showNotificationToScreen(currentTask);

            //showAlarmScreen(currentTask);

            //showReminderDialog(currentTask);
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
            CurrentTask.setBackgroundColor(DefaultCardBackgroundColor.ToString());
            CurrentTask.setDate_last_update(Utils.Utils.getDateFormattedString(Utils.Utils.GetDateNow()));
            CurrentTask.setRepeat("פעם אחת");

            return CurrentTask;
        }

        public class MyTask : Java.Util.TimerTask      // timerTask = new Java.Util.TimerTask();
        {
            private Java.Util.Timer ParentTimer { get; set; }
            private Task ParentTask { get; set; }
            //public event Java.Lang.Runnable ActionOnPlayingMusic;
            public event Action<Task> ActionOnPlayingMusic;
            //public event Action ActionOnPlayingMusic;
            //public event Action<object, ElapsedEventArgs> ActionOnPlayingMusic;
            //public event ElapsedEventHandler ActionOnPlayingMusic;
            private bool IsRunning;
            public int IntervalTime { get; set; }


            public MyTask(Java.Util.Timer parentTimer, Task parentTask)
            {
                this.ParentTimer = parentTimer;
                this.ParentTask = parentTask;
                IntervalTime = 3000;
            }

            public override void Run()
            {
                //ActionOnPlayingMusic += picsTimer_onTick;
                //ActionOnPlayingMusic);
                //RunOnUiThread(() =>     
                //{
                //    picsTimer_onTick(); // ParentTask);
                //});

                DateTime dateNow = Utils.Utils.GetDateNow();       // args.SignalTime;
                Task currentTask = ParentTask;


                if (currentTask != null && currentTask.getDate() == null)
                {
                    return;
                }

                string strDateNow = Utils.Utils.getDateFormattedString(dateNow);
                string strDateTask = Utils.Utils.getDateFormattedString(currentTask.getDate().Value);

                IsRunning = true;

                while (IsRunning && strDateNow != strDateTask)
                {
                    Thread.Sleep(IntervalTime);

                    picsTimer_onTick(); // ParentTask);

                    dateNow = Utils.Utils.GetDateNow();
                    strDateNow = Utils.Utils.getDateFormattedString(dateNow);

                    //RunOnUiThread(ActionOnPlayingMusic);    // =>
                    //{
                    //    picsTimer_onTick(); // ParentTask);
                    //});

                    //UpdateProgressControls();

                }

                ParentTimer.Cancel();
                ParentTimer.Dispose();

                //picsTimer_onTick(); // ParentTask);
            }

            protected void picsTimer_onTick() //Task sender)
            {
                Task sender = ParentTask;

                if (sender == null)     // || args==null)
                {
                    return;
                }

                DateTime dateNow = Utils.Utils.GetDateNow();       // args.SignalTime;
                Task currentTask = (Task)sender;       // CurrentTask;

                if (currentTask.getDate() == null)
                {
                    return;
                }


                string strNow = Utils.Utils.getDateFormattedString(dateNow);
                string strDateTask = Utils.Utils.getDateFormattedString(currentTask.getDate().Value);

                if (strNow != strDateTask)
                {
                    return;
                }


                //IsRunning = false;
                //// TODO: imerStop(currentTask);

                //Log.d("timer Tik ", currentTask.getTitle() + " - " + currentTask.getDate_due() + currentTask.getTime_due());

                //showNotifications(currentTask);

                return;    // new ElapsedEventHandler(null, new ElapsedEventArgs());
            }

        }

        protected override void OnDestroy()
        {
            BackupDataBaseFile();

            Toast.MakeText(this, "Close Application", ToastLength.Long).Show();

            base.OnDestroy();
        }

        private void BackupDataBaseFile()
        {
            string backupFolderName = Android.OS.Environment.DirectoryMusic;    // "ProjTaskReminder"
            string targetPath = Android.OS.Environment.GetExternalStoragePublicDirectory(backupFolderName).AbsolutePath;
            //targetPath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            //targetPath = context.GetExternalFilesDir("").AbsolutePath;;


            string fullPathSource = Path.Combine(DBTaskReminder.DB_PATH, DBTaskReminder.DB_NAME);
            string fullPathTarget = Path.Combine(targetPath, DBTaskReminder.DB_NAME);

            bool result = Utils.Utils.CopyFile(fullPathSource, fullPathTarget);

            fullPathSource = Path.Combine(Utils.Utils.LOG_FILE_PATH, Utils.Utils.LOG_FILE_NAME);
            fullPathTarget = Path.Combine(targetPath, Utils.Utils.LOG_FILE_NAME);

            result = Utils.Utils.CopyFile(fullPathSource, fullPathTarget);
            
            if (result)
            {
                string message = "Database was copied OK";
                Toast.MakeText(this, message, ToastLength.Long).Show();
                //Utils.Utils.WriteToLog(message, true);
            }
        }

        private void RestoreDataBaseFile()
        {
            string backupFolderName = Android.OS.Environment.DirectoryMusic;    // "ProjTaskReminder"
            string sourcePath = Android.OS.Environment.GetExternalStoragePublicDirectory(backupFolderName).AbsolutePath;  
            string targetPath = DBTaskReminder.DB_PATH;

            string fullPathTarget = Path.Combine(targetPath, DBTaskReminder.DB_NAME);
            string fullPathSource = Path.Combine(sourcePath, DBTaskReminder.DB_NAME);

            DBTaskReminder.DB.Close();

            bool result = Utils.Utils.CopyFile(fullPathSource, fullPathTarget);
            
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
        private int searchIndexListByID(int taskID)
        {
            int result = -1;

            for (int i=0; i<TasksList.Count; i++)
            {
                if (TasksList[i].getTaskID()==taskID)
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



            //dateStrToFind = Utils.Utils.getDateFormattedString(date);    //simpleDateFormat.format(date);

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

    }

    public class TaskTimerElapsed
    {
        private Task TaskObject { get; set; }
        public event Action<Task, System.Timers.Timer> OnTimerTick;        //object, ElapsedEventArgs, 
        public event Action<Task, System.Threading.Thread> OnThreadTick;        //object, ElapsedEventArgs, 
        public int TimerInterval { get; set; }
        public System.Timers.Timer TimerObject { get; set; }
        public System.Threading.Thread ThreadObject { get; set; }
        public Activity activity { get; set; }


        public TaskTimerElapsed(Task task, System.Timers.Timer timerObject)
        {
            this.TaskObject = task;
            this.TimerObject = timerObject;
            this.TimerInterval = 30000;
        }

        public TaskTimerElapsed(Task task, System.Threading.Thread timerObject)
        {
            this.TaskObject = task;
            this.ThreadObject = timerObject;
            this.TimerInterval = 30000;
        }

        public void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {

            DateTime dateNow = Utils.Utils.GetDateNow();
            DateTime? dateTask = TaskObject.getDate();

            if (!dateTask.HasValue)
            {
                TimerDispose(TimerObject);

                // Raise event Timer Tick
                if (OnTimerTick != null)
                {
                    OnTimerTick(TaskObject, TimerObject);
                }
                return;
            }

            //string strDateNow = Utils.Utils.getDateFormattedString(dateNow);
            //string strDateTask = Utils.Utils.getDateFormattedString(dateTask.Value);

            if (dateNow.CompareTo(dateTask) >= 0)
            //if (strDateNow.CompareTo(strDateTask) >= 0)
                {
                System.Timers.Timer timer = this.TimerObject;
                //System.Timers.Timer timer = ((System.Timers.Timer)sender);

                TimerDispose(timer);

                // Raise event Timer Tick
                if (OnTimerTick != null)
                {
                    OnTimerTick(TaskObject, TimerObject);
                }

                //TimerStop(TaskObject);

                // Do the Job - Notification
                //picsTimer_onTick(TaskObject);

            }
        }

        public void Thread_Elapsed()      //object sender, ElapsedEventArgs args) ElapsedEventHandler
        {
            long counter = 0;
            long limitSeconds = 1000 * 60 * 60 * 24 * 3;        // Limited for 3 Days




            Task TaskObject = this.TaskObject;      // (Task)sender;
            DateTime? dateTask = this.TaskObject.getDate();

            if (!dateTask.HasValue)
            {
                return;
            }

            DateTime dateNow = Utils.Utils.GetDateNow();
            //string strDateNow = Utils.Utils.getDateFormattedString(dateNow);
            //string strDateTask = Utils.Utils.getDateFormattedString(dateTask.Value);


            while (counter <= limitSeconds && dateNow.CompareTo(dateTask) < 0)
            {
                Thread.Sleep(TimerInterval);

                activity.RunOnUiThread(() =>
                {
                    dateNow = Utils.Utils.GetDateNow();
                    //strDateNow = Utils.Utils.getDateFormattedString(dateNow);
                });

                counter += TimerInterval;

                //RunOnUiThread(ActionOnTaskDateDue); // (strDateNow));    // =>
                //{
                //int newPosition = mediaPlayer.CurrentPosition;
                //barSeek.Progress = newPosition;
                //};
            }


            // Raise event Timer Tick
            if (OnThreadTick != null)
            {
                OnThreadTick(TaskObject, ThreadObject);
            }

            //TimerStop(TaskObject);

            // Do the Job - Notification
            //picsTimer_onTick(TaskObject);


            return;
        }

        private void TimerDispose(System.Timers.Timer timer)
        {
            timer.Stop();
            timer.Close();
            timer.Dispose();

            timer = null;

        }
    }

}

