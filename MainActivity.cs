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

namespace ProjTaskReminder
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {

        private ListView            simpleList;
        public static DBTaskReminder      DBTaskReminder;
        private List<Task> TasksList = new List<Task>();

        private readonly string[]  countryList = new string[6] { "India", "China", "australia", "Portugle", "America", "NewZealand" };

        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            SetControlsIO();

            ConnectToDB();

            FillList();
        }

        private void SetControlsIO()
        {
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;

            simpleList = (ListView)FindViewById(Resource.Id.simpleListView);

            //simpleList.setHasFixedSize(true);
            //simpleList.SetLayerType(new LinearLayout(this), new Android.Graphics.Paint());
        }

        [Obsolete]
        private void FillList()
        {

            TasksList = GetTasksFromDB();

            ListViewAdapter listViewAdapter = new ListViewAdapter(this.ApplicationContext, TasksList);
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

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            View view = (View) sender;

            Toast.MakeText(view.Context, "Click", ToastLength.Long);

            Task task = new Task();

            OpenTaskDetailsPage(task, true, Application.Context);

            FillList();

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

            context.StartActivity(intent);
            
            //Bundle bundle;  // = new bundle();
            //Intent backIntent;
            //context.StartIntentSender(intent, backIntent , null, null, 0, bundle);

        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        public override void OnActivityReenter(int resultCode, Intent data)
        {
            base.OnActivityReenter(resultCode, data);

            FileList();
        }
    }
}

