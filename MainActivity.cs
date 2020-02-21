﻿using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using ProjTaskReminder.Model;
using ProjTaskRemindet.Utils;
using ProjTaskReminder.Data;
using static ProjTaskReminder.Data.DBTaskReminder;
using SQLite;
using Android.Content;
using Android.Support.V7.Widget;
using System.Timers;
using System.Threading;
using System.IO;
using Java.Util;
using ProjTaskReminder.Utils;
using Android.Text;
using Android.Text.Style;
//using System.Threading.Timer;


namespace ProjTaskReminder
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity   //, ListView.IOnItemClickListener
    {

        public static string DB_TASK_DB_NAME = "TaskReminder.db";
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

        private ListView simpleList;    //RecyclerView
        private ListViewAdapter listViewAdapter;
        public static DBTaskReminder DBTaskReminder;
        private List<Task> TasksList = new List<Task>();

        private static Context context;
        //private readonly string[]  countryList = new string[6] { "India", "China", "australia", "Portugle", "America", "NewZealand" };

        public static Task CurrentTask;
        public static List<Object[]> TimersArray;
        public static string MainMessageText;
        public static int DefaultCardBackgroundColor;
        public static bool isShowTimerReminder;
        private event Action ActionOnTaskDateDue;
        private int TimerInterval;
        private MH_Notification mh_Notification;

        private Android.Support.V4.App.NotificationCompat.Builder notificationBuilder;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            //Utils.Utils.WriteToLog("Enter 1", true);

            context = this.ApplicationContext;

            SetControlsIO();

            Initialize();

            ConnectToDB();

            BackupDataBaseFile();

            //Utils.Utils.WriteToLog("Enter 3", true);

            FillListFromDB();
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


            ImageButton bntMainMusic = (ImageButton)FindViewById(Resource.Id.bntMainMusic);
            ImageButton btnMainWeather = (ImageButton)FindViewById(Resource.Id.btnMainWeather);
            ImageButton btnMainEmail = (ImageButton)FindViewById(Resource.Id.btnMainEmail);

            bntMainMusic.Click += btnMainMusic_Click;


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

        private void Initialize()
        {

            TimersArray = new List<object[]>();

            TimerInterval = 600000;  // 1000 * 60 * 30

            //mainActivityServices = new MainActivityServices(MainActivity.this, this, this);
            //IsManualHtml = false;
            //setControlsColors();

            Utils.Utils.activity = this;
            Utils.Utils.context = context;

            // Open Notification Channel. Must be in the start init
            // Done again in 'Util.createNotificationBuilder()'
            mh_Notification = new MH_Notification(this, context);
            mh_Notification.createNotificationChannel();

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
            TasksList = TasksList.OrderBy(a => a.getDate()).ToList();

            RefreshListAdapter();
        }

        [Obsolete]
        private void RefreshListAdapter()
        {
            listViewAdapter = new ListViewAdapter(context, TasksList);
            simpleList.SetAdapter(listViewAdapter);

            listViewAdapter.NotifyDataSetChanged();
        }

        private bool ConnectToDB()
        {
            bool result = true;


            try
            {
                DBTaskReminder = new DBTaskReminder(DB_TASK_DB_NAME);
            }
            catch (Exception ex)
            {
                Utils.Utils.WriteToLog(ex.Message, true);
            }

            return result;
        }

        private List<Task> GetTasksFromDB()
        {
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

                TasksList.Add(task);
            }

            return TasksList;
        }

        public static List<KeyValuePair<String, String>> getTaskValues(Task currentTask)
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
            //Log.d("Just before save", currentTask.getDescriptionWithHtml());
            //values.Add(new KeyValuePair<string, string>(DB_FIELDNAME_DESC, currentTask.getDescriptionWithHtml()));     // Html.toHtml(currentTask.getDescription())
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
                //Log.d("Open New task screen - TaskID: ", String.valueOf(taskID));
                task = newTaskDetails();
            }

            Intent intent = new Intent(this, typeof(ActivityTaskDetails));
            //intent.setFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP);

            intent.PutExtra("TaskID", task.getTaskID());
            //intent.putExtra("task", task);  //TODO: Seriize

            MainActivity.CurrentTask = task;

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
            if (id == Resource.Id.action_settings)
            {
                return true;
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


            DateTime today = Utils.Utils.getDateFixed(DateTime.Now);       //Util.getDateNow.getTime();

            if (timerDate.Value.CompareTo(today) <= 0)
            {
                // Show mssage if there is message in que
                showGlobalMessageOnToast();
                return;
            }


            //Toast.MakeText(this, "Task date: " + Utils.Utils.getDateFormattedString(timerDate.Value) + "     Now: " + Utils.Utils.getDateFormattedString(Utils.Utils.getDateFixed(DateTime.Now)), ToastLength.Long).Show();

            CurrentTask = currentTask;
            ActionOnTaskDateDue += TimerNextTime;

            TimerInterval = 30000;  // 30 seconds


            TimerExecute2(currentTask);
            //TimerExecute(currentTask);
            //TimerExecute3(currentTask, timerDate);


            dateStr = Utils.Utils.getDateFormattedString(timerDate.Value);

            if (isShowTimerReminder && !MainMessageText.Trim().Equals(""))
            {
                string text = "יצר תזכורת ביום " + Utils.Utils.getDateDayName(timerDate.Value) + "  " + dateStr;
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
            System.Threading.Thread timer = new System.Threading.Thread(new ThreadStart(TimerThreadExecute));

            //Thread second = new Thread(new ThreadStart(secondThread));

            currentTask.setTimer(timer);
            addTimerToKillArray(currentTask.getTaskID(), timer, null);     // timer timerTask);

            timer.Start();
        }

        private void TimerThreadExecute()
        {
            TimerRunning(CurrentTask, null);
        }

        private void TimerStop(Task currentTask)
        {
            int index = searchIDInTimersList(currentTask.getTaskID());

            if (index == -1)
            {
                return;
            }

            TimerKill(index, currentTask);

        }

        private ElapsedEventHandler TimerRunning(object sender, ElapsedEventArgs args)
        {

            if (sender == null)
            {
                return null;
            }

            long counter = 0;
            // Limited for 3 Days
            long limitSeconds = 1000 * 60 * 60 * 24 * 3;


            Task currentTask = (Task)sender;



            if (currentTask.getDate() == null)
            {
                return null;
            }


            DateTime dateNow = Utils.Utils.getDateFixed(DateTime.Now);
            string strDateNow = Utils.Utils.getDateFormattedString(dateNow);
            string strDateTask = Utils.Utils.getDateFormattedString(currentTask.getDate().Value);


            while (counter <= limitSeconds && strDateNow.CompareTo(strDateTask) < 0)
            {
                Thread.Sleep(TimerInterval);

                RunOnUiThread(() =>
                {
                    dateNow = Utils.Utils.getDateFixed(DateTime.Now);
                    strDateNow = Utils.Utils.getDateFormattedString(dateNow);
                });

                counter += TimerInterval;

                //RunOnUiThread(ActionOnTaskDateDue); // (strDateNow));    // =>
                //{
                //int newPosition = mediaPlayer.CurrentPosition;
                //barSeek.Progress = newPosition;
                //};
            }


            TimerStop(currentTask);

            // Do the Job - Notification
            picsTimer_onTick(currentTask);


            return null;    // new ElapsedEventHandler(null, new ElapsedEventArgs());
        }

        #endregion


        #region Timer as System.Timers.Timer

        private void TimerExecute2(Task currentTask)
        {
            System.Timers.Timer timer = new System.Timers.Timer();

            currentTask.setTimer(timer);
            addTimerToKillArray(currentTask.getTaskID(), timer, null);     // timer timerTask);

            //timer.BeginInit();   
            timer.AutoReset = true;
            timer.Elapsed += TimerRunning2;
            timer.Interval = TimerInterval; // 30 seconds
            timer.Enabled = true;
            //timer.Start();

            //timer.ScheduleAtFixedRate(customTimerTask, new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute), 5000);
        }

        private void TimerRunning2(object sender, ElapsedEventArgs e)
        {
            //Toast.MakeText(this, "Timer Ticking...", ToastLength.Short).Show();

            Task currentTask = CurrentTask;

            DateTime dateNow = Utils.Utils.getDateFixed(DateTime.Now);
            string strDateNow = Utils.Utils.getDateFormattedString(dateNow);
            string strDateTask = Utils.Utils.getDateFormattedString(CurrentTask.getDate().Value);

            if (strDateNow.CompareTo(strDateTask) >= 0)
            {
                System.Timers.Timer timer = ((System.Timers.Timer)sender);

                TimerStop(currentTask);

                // Do the Job - Notification
                picsTimer_onTick(currentTask);
            }

        }

        #endregion

        private void TimerKill(int timerArrayIndex, Task currentTask)
        {
            try
            {
                Object[] timersArray = TimersArray[timerArrayIndex];

                //Thread timer = (Thread)timersArray[1];
                System.Timers.Timer timer = (System.Timers.Timer)timersArray[1];
                //Java.Util.Timer timer = (Java.Util.Timer)timersArray[1];
                //TimerTask timerTask = (TimerTask)timersArray[2];

                if (timer != null && timer.Enabled)
                //if (timer != null && timer.IsAlive)
                {
                    timer.Stop();
                    timer.Close();
                    timer.Dispose();
                    timer = null;

                    //timer.Abort();
                    //timer = null;

                    //timer.purge();
                    //timer.c.Cancel();
                    //timerTask.cancel();
                    //timer = null;
                    //timerTask = null;
                }

                ////TimersArray.RemoveAt(timerArrayIndex);

                currentTask.setTimer(timer);
                currentTask.setTimer_task(null);
            }
            catch (Exception ex)
            {
                Utils.Utils.WriteToLog("TimerKill: " + Utils.Utils.LINE_SEPERATOR + ex.Message);
            }
        }

        #region Tier as Java.Util.Timer

        private void TimerExecute3(Task currentTask, Date timerDate)
        {
            Java.Util.TimerTask timerTask = null;  // = new Java.Util.TimerTask(picsTimer_onTick);
            long ticks = timerTask.ScheduledExecutionTime();
            timerTask.Run();

            Java.Util.Timer timer = new Java.Util.Timer();
            timer.Schedule(timerTask, timerDate);

            //currentTask.setTimer(timer);
            //addTimerToKillArray(currentTask.getTaskID(), timer, 0);     // timer timerTask);


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
            DateTime dateNow = Utils.Utils.getDateFixed(DateTime.Now);
            string strDateNow = Utils.Utils.getDateFormattedString(dateNow);

            //return strDateNow;
        }

        private void addTimerToKillArray(int taskID, System.Timers.Timer timer, object timerTask)  //  | Java.Util.Timer
        {
            Object[] timersArray = new Object[3];

            timersArray[0] = taskID;
            timersArray[1] = timer;
            timersArray[2] = timerTask;

            TimersArray.Add(timersArray);
        }

        private void addTimerToKillArray(int taskID, Thread timer, object timerTask)  // System.Timers.Timer | Java.Util.Timer
        {
            Object[] timersArray = new Object[3];
            timersArray[0] = taskID;
            timersArray[1] = timer;
            timersArray[2] = timerTask;

            TimersArray.Add(timersArray);
        }

        private static int searchIDInTimersList(int id)
        {
            int result = -1;

            for (int i = 0; i < TimersArray.Count; i++)
            {
                Object[] timersArray = TimersArray[i];

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
                TimersArray = new List<Object[]>();
                return;
            }


            for (int i = TimersArray.Count - 1; i >= 0; i--)
            {
                Object[] timersArray = TimersArray[i];
                TimerKill(i, searchTaskByID((int)timersArray[0]));
            }

            TimersArray = null;
            TimersArray = new List<Object[]>();

        }

        /// <summary>
        /// Show mssage if there is message in que
        /// </summary>
        public void showGlobalMessageOnToast()
        {
            if (isShowTimerReminder && !MainMessageText.Trim().Equals(""))
            {
                Toast.MakeText(this, MainMessageText, ToastLength.Long).Show();
                MainMessageText = "";
            }
        }

        //protected void showNotifications()  //Task currentTask)
        //{
        //    showNotifications(CurrentTask);
        //}

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
            DateTime date;

            date = Utils.Utils.getDateFixed(DateTime.Now);     //Util.getDateNow.getTime();

            CurrentTask = new Task();

            CurrentTask.setDate_due("");
            CurrentTask.setBackgroundColor(DefaultCardBackgroundColor.ToString());
            CurrentTask.setDate_last_update(Utils.Utils.getDateFormattedString(date));
            CurrentTask.setRepeat("פעם אחת");

            //Log.d("new task", CurrentTask.getDate_due());

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

                DateTime dateNow = Utils.Utils.getDateFixed(DateTime.Now);       // args.SignalTime;
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

                    dateNow = Utils.Utils.getDateFixed(DateTime.Now);
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

                DateTime dateNow = Utils.Utils.getDateFixed(DateTime.Now);       // args.SignalTime;
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

            base.OnDestroy();
        }

        private void BackupDataBaseFile()
        {
            string folderBackup = Android.OS.Environment.DirectoryMusic;    // "ProjTaskReminder"
            string sourcePath = context.GetExternalFilesDir("").AbsolutePath;
            string targetPath = Android.OS.Environment.GetExternalStoragePublicDirectory(folderBackup).AbsolutePath;
            //targetPath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;

            string fullPathSource = Path.Combine(DBTaskReminder.DB_PATH, DBTaskReminder.DB_NAME);
            string fullPathTarget = Path.Combine(targetPath, DBTaskReminder.DB_NAME);

            bool result = Utils.Utils.CopyFile(fullPathSource, fullPathTarget);

            if (result)
            {
                string message = "Database was copied OK";
                //Toast.MakeText(this, message, ToastLength.Long).Show();
                //Utils.Utils.WriteToLog(message, true);
            }
        }

        private Task searchTaskByID(int taskID)
        {
            return TasksList.FirstOrDefault(a => a.getTaskID() == taskID);
        }
    }
}

