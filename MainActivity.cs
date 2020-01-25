using System;
using System.Collections.Generic;
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

namespace ProjTaskReminder
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {

        private ListView simpleList;    //RecyclerView
        private ListViewAdapter listViewAdapter;
        public static DBTaskReminder      DBTaskReminder;
        private List<Task> TasksList = new List<Task>();

        private static Context context;
        //private readonly string[]  countryList = new string[6] { "India", "China", "australia", "Portugle", "America", "NewZealand" };

        public static Task CurrentTask;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            context = this.ApplicationContext;

            SetControlsIO();

            ConnectToDB();

            FillList();
        }

        private void SetControlsIO()
        {
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            FloatingActionButton btnMainNew = FindViewById<FloatingActionButton>(Resource.Id.btnMainNew);
            btnMainNew.Click += btnMainNew_OnClick;

            FloatingActionButton btnMainDelete = FindViewById<FloatingActionButton>(Resource.Id.btnMainDelete);
            btnMainDelete.Click += btnMainDelete_OnClick;

            ActivityTaskDetails.OnSaveButton += new EventHandler(TaskDetailsSave_Click);

            ActivityTaskDetails.OnActivityResult += OnActivityResult;

            TasksList = new List<Task>();

            listViewAdapter = new ListViewAdapter(context, TasksList);
            
            //listViewAdapter.SetOnClickListener += new EventHandler(OnItemClick);
            listViewAdapter.SetOnItemClick += new EventHandler(OnItemClickFromAdapter);

            simpleList = (ListView)FindViewById(Resource.Id.simpleListView);        // RecyclerView

            // RecyclerView
            //LinearLayoutManager mLayoutManager = new LinearLayoutManager(this);
            //simpleList.SetLayoutManager(mLayoutManager);

            //simpleList.setHasFixedSize(true);
            //simpleList.SetLayerType(new LinearLayout(this), new Android.Graphics.Paint());


            //simpleList.OnItemClickListener += OnItemClick

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

        [Obsolete]
        private void FillList()
        {

            TasksList = GetTasksFromDB();

            listViewAdapter = new ListViewAdapter(context, TasksList);
            simpleList.SetAdapter(listViewAdapter);

            listViewAdapter.NotifyDataSetChanged();



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

        private bool ConnectToDB()
        {
            bool result = true;
            

            DBTaskReminder = new DBTaskReminder("TaskReminder.db");

            return result;
        }

        private List<Task> GetTasksFromDB()
        {
            TasksList.Clear();

            TableQuery<TBL_Tasks> table = DBTaskReminder.DB.Table<TBL_Tasks>();

            //var stock = DBTaskReminder.DB.Get<TBL_Tasks>(2); // primary key id of 5
            //var stockList = DBTaskReminder.DB.Table<TBL_Tasks>();

            foreach (TBL_Tasks s in table)
            {
                Task task = new Task();

                task.setTaskID(s.ID);
                task.setTitle(s.Title);
                task.setDescription(s.Description);
                task.setDate_due(s.DateDue);

                TasksList.Add(task);
            }

            return TasksList;
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

        private void OnItemClick(object sender, EventArgs e)
        {
            OnItemClickFromAdapter(sender, e);
        }

        private void OnItemClickFromAdapter(object sender, EventArgs e)
        {
            CurrentTask = TasksList[simpleList.SelectedItemPosition];

            ActivityTaskDetails.CurrentTask = CurrentTask;

            Toast.MakeText(context, "Click", ToastLength.Long).Show();
        }

        private void btnMainNew_OnClick(object sender, EventArgs eventArgs)
        {
            View view = (View) sender;

            Toast.MakeText(view.Context, "Click", ToastLength.Long).Show();

            Task task = new Task();

            OpenTaskDetailsPage(task, true, Application.Context);

            //Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
            //              .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
        }

        private void btnMainDelete_OnClick(object sender, EventArgs eventArgs)
        {
            View view = (View)sender;

            Toast.MakeText(view.Context, "Delete", ToastLength.Long).Show();

            //simpleList.Selected;
            //simpleList.SelectedItem;
            //simpleList.SelectedView;

            if (simpleList.SelectedItemPosition > 0 && simpleList.SelectedItemPosition < TasksList.Count)
            {
                Task task = TasksList[simpleList.SelectedItemPosition];

                DBTaskReminder.DB.Delete<TBL_Tasks>(task.getTaskID());  // "ID==" +currentTask.getTaskID().ToString(), null);

                FillList();
            }
            //Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
            //              .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
        }

        private void OpenTaskDetailsPage(Task task, bool isNewMode, Context context)
        {
            Intent intent = new Intent(this, typeof(ActivityTaskDetails));
            //intent.setFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP);

            intent.PutExtra("TaskID", task.getTaskID());
            //intent.putExtra("task", task);  //TODO: Seriize

            ////MainActivity.CurrentTask = task;

            ActivityTaskDetails.isNewMode = isNewMode;
            ActivityTaskDetails.CurrentTask = task;
            ////ActivityTaskDetails.dbHandler = MainActivity.dbHandler;
            ActivityTaskDetails.context = context;      //Application.Context;
            ////ActivityTaskDetails.mainActivity = mainActivity;

            StartActivityForResult(intent, 1234);
            //context.StartActivity(intent);

            //var req = new Intent();
            //req.SetComponent(new ComponentName("com.example.Tristan.Android", "com.example.Tristan.Android.IsoldeQueryActivity"));
            //StartActivityForResult(req, 1234);
            //var rtn = await TristanStateCompletion.Task;
            //if (!rtn) bomb("can't get state");
            //TristanStateCompletion = null;


            //Bundle bundle;  // = new bundle();
            //Intent backIntent;
            //context.StartIntentSender(intent, backIntent , null, null, 0, bundle);

        }

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
        //if (requestCode == 1234)
        //{
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            //base.OnActivityReenter(requestCode,resultCode, data);

            FillList();
        }
    }
}

