//package ;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using static Android.Support.V7.Widget.RecyclerView;
using static Android.Support.V7.Widget.CardView;
using ProjTaskRemindet;
using ProjTaskReminder.Model;
using Android.Support.V7.Widget;

//using Android.App.Activity;
//using Android.Content;
//using Android.

namespace ProjTaskRemindet.Utils
{
    public class ListViewAdapter : BaseAdapter      //RecyclerView.Adapter    
    {
        private readonly Context context;
        //private MainActivity mainActivity;
        //private List<Task> TaskList;
        public List<Task> TaskList;

        //private Resources resources;
        //private int selectMode = 0;
        //private int selectedItemsCount;

        private LayoutInflater layoutInflater;

        //public override int Count => 21;

        public event EventHandler SetOnClickListener;
        public event EventHandler SetOnItemClick;
        public static event Action<View.IOnClickListener> OnActivityResult;

        public ListViewAdapter(Context applicationContext, List<Task> itemsList)
        {
            this.context = applicationContext;
            TaskList = itemsList;

            layoutInflater = LayoutInflater.From(this.context);
         
        }

        public override int Count
        {
            get
            {
                return this.TaskList.Count;
            }
        }

        //public override int ItemCount   // => throw new NotImplementedException();
        //{
        //    get
        //    {
        //        return TaskList.Count;
        //    }
        //}

        //public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        //{
        //    ListViewHolder listViewHolder = (ListViewHolder)holder;

        //    if (position < TaskList.Count)
        //    {
        //        Task task = TaskList[position];
        //        listViewHolder.title.SetText(task.getTitle(), TextView.BufferType.Normal);
        //        listViewHolder.description.SetText(task.getDescription(), TextView.BufferType.Normal);
        //        listViewHolder.date_date.SetText(task.getDate_due(), TextView.BufferType.Normal);
        //    }
        //}

        //public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        //{
        //    View convertView = layoutInflater.Inflate(ProjTaskReminder.Resource.Layout.list_item, parent, false);


        //    ListViewHolder viewHolder = new ListViewAdapter.ListViewHolder(convertView);        //, context);

        //    return viewHolder;
        //}

        public override Java.Lang.Object GetItem(int position)
        {
            //View convertView = layoutInflater.Inflate(ProjTaskReminder.Resource.Layout.list_item, parent, false);

            //ListViewHolder viewHolder = new ListViewAdapter.ListViewHolder(convertView);        //, context);

            //    return viewHolder;

            View view = (View)this.GetItem(position);
            
            //View view = layoutInflater.Inflate(ProjTaskReminder.Resource.Layout.list_item, null);

            //view.FindViewById(R.id.gv_players);

            return view;    // view;    // TaskList[position];  // null;
        }

        public override long GetItemId(int position)
        {
            long res = 0;

            if (position > -1 && position < TaskList.Count)
            {
                res = TaskList[position].getTaskID();
            }

            return 0;   // res;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            ListViewHolder listViewHolder = null;


            try
            {
                if (convertView == null)
                {
                    convertView = layoutInflater.Inflate(ProjTaskReminder.Resource.Layout.list_item, parent, false);
                }

                if (position >= 0 && position < TaskList.Count)
                {
                    listViewHolder = new ListViewAdapter.ListViewHolder(convertView, position, TaskList);        //, context);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            if (listViewHolder!=null && position >= 0 && position < TaskList.Count)
            {
                Task task = TaskList[position];
                listViewHolder.title.SetText(task.getTitle(), TextView.BufferType.Normal);
                listViewHolder.description.SetText(task.getDescription(), TextView.BufferType.Normal);
                listViewHolder.date_date.SetText(task.getTime_due() + "  " +task.getDate_due(), TextView.BufferType.Normal);
            }

            //convertView.SetOnClickListener(InitOnItemClick(position));

            return convertView;
        }

        public class ListViewHolder // : RecyclerView.ViewHolder
        {
            public TextView title;
            public TextView description;
            public TextView date_date;
            public CardView cardView;

            public event EventHandler SetOnItemClick;
            private int ItemPosition { get; set; }


            public ListViewHolder(View convertView, int position, List<Task> tasks)     //: base(convertView) //, Context containerContext) 
            {
                //int position = 2;   // this.ItemPosition;   // this.Position
                ItemPosition = position;
                
                if (convertView != null && position >= 0 && position < tasks.Count)
                {
                    try
                    {
                        title = (TextView)convertView.FindViewById(ProjTaskReminder.Resource.Id.txtTitle);
                        description = (TextView)convertView.FindViewById(ProjTaskReminder.Resource.Id.txtDescription);
                        date_date = (TextView)convertView.FindViewById(ProjTaskReminder.Resource.Id.txtDateDue);

                        cardView = (CardView)convertView.FindViewById(ProjTaskReminder.Resource.Id.cardTask);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }


                // Remove the 'Selected' methods of the original ListView
                //if (position < tasks.Count)
                //{
                //    convertView.SetOnClickListener(InitOnItemClick(position));     // OnActivityResult
                //}

                //cardView.SetOnClickListener(InitOnItemClick(position));     // OnActivityResult

                //if (position < tasks.Count)
                //{
                //    convertView.SetOnClickListener(InitOnItemClick(position));     // OnActivityResult
                //    //cardView.SetOnClickListener(InitOnItemClick(position));     // OnActivityResult

                //    title.SetText(tasks[position].getTitle(), TextView.BufferType.Normal);
                //    description.SetText(tasks[position].getDescription(), TextView.BufferType.Normal);
                //    date_date.SetText(tasks[position].getDate_due(), TextView.BufferType.Normal);
                //}
            }


            private View.IOnClickListener InitOnItemClick(int position)
            {
                this.ItemPosition = position;

                SetOnItemClick += new EventHandler(OnItemClick);

                return null;
            }

            private void OnItemClick(object sender, EventArgs e)    //, int y)
            {
                SetOnItemClick(this.ItemPosition, e);
            }

        }

        //private void OnItemClick2(int position)
        //{
        //    if (SetOnClickListener!=null)
        //    {
        //        SetOnClickListener(position, EventArgs.Empty);
        //    }
        //}

        public View.IOnClickListener InitOnItemClick(int position)
        {
            SetOnItemClick += new EventHandler(OnItemClick);

            return null;
        }

        public void OnItemClick(object sender, EventArgs e)
        {
            SetOnItemClick(sender, e);
        }

        //public void DeleteTask(object sender, EventArgs e)
        //{

        //    //return null;    // (View.IOnClickListener);
        //}

        //public  int Count2()  // override
        //{
        //    return TaskList.Count;
        //}

       
    }

        
}