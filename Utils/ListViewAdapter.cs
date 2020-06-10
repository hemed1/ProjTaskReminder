

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
using ProjTaskReminder.Utils;
using ProjTaskReminder.Model;
using Android.Support.V7.Widget;
using ProjTaskReminder;
using System.Timers;

namespace ProjTaskRemindet.Utils
{
    public class ListViewAdapter : BaseAdapter      //RecyclerView.Adapter    
    {
        private readonly Context context;
        private readonly Object ItemsList;
        private LayoutInflater layoutInflater;
        public int ViewResourcesID = ProjTaskReminder.Resource.Layout.list_item;


        private int SelectedPosition;
        private int SelectedItemPosition { get; set; }
        public System.Timers.Timer TimerObject { get; set; }

        public event Action<ListViewHolder, int> OnListItemSetControlsInView;
        public event Action<ListViewHolder, int> OnItemClick;
        public event EventHandler SetOnItemClick;
        //public View.IOnClickListener OnItemClick2;                  // Not in use

        //private Resources resources;
        //private int selectMode = 0;
        //private int selectedItemsCount;
        //private MainActivity mainActivity;

        public ListView ParentListView;


        public ListViewAdapter(Context applicationContext, Object itemsList)
        {
            this.context = applicationContext;
            ItemsList = itemsList;

            layoutInflater = LayoutInflater.From(this.context);

            ViewResourcesID = ProjTaskReminder.Resource.Layout.list_item;
        }

        public override int Count
        {
            get
            {
                return GetListObject.Count();
            }
        }

        //public override int ItemCount   // => throw new NotImplementedException();
        //{
        //    get
        //    {
        //        return GetListObject.Count();
        //    }
        //}

        //public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        //{
        //    ListViewHolder listViewHolder = (ListViewHolder)holder;
              //if (listViewHolder!=null && position >= 0 && position<GetListObject.Count())
              //{
              //      // Set controls in CradView with Object props Outsiderly (In Client source code)
              //      SetControlsInView(listViewHolder, position);
              //}

    //    if (position < GetListObject.Count)
    //    {
    //        Task task = GetListObject[position];
    //        listViewHolder.FirstLine.SetText(task.getTitle(), TextView.BufferType.Normal);
    //        listViewHolder.SecondLine.SetText(task.getDescription(), TextView.BufferType.Normal);
    //        listViewHolder.ThirdLine.SetText(task.getDate_due(), TextView.BufferType.Normal);
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
            View view = (View)this.GetItem(position);

            //View view = layoutInflater.Inflate(ProjTaskReminder.Resource.Layout.list_item, null);
            //View convertView = layoutInflater.Inflate(ProjTaskReminder.Resource.Layout.list_item, parent, false);

            return view;       // GetListObject[position];  // null;
        }

        
        public override long GetItemId(int position)
        {
            long res = 0;

            this.SelectedItemPosition = position;

            if (position > -1 && position < GetListObject.Count())
            {
                //OnItemClick(null, position);
                //res = ItemsList[position].getTaskID();
            }

            return res;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            ListViewHolder listViewHolder = null;


            try
            {
                if (convertView == null)
                {
                    convertView = layoutInflater.Inflate(ViewResourcesID, parent, false);
                }

                if (position >= 0 && position < GetListObject.Count())
                {
                    listViewHolder = new ListViewAdapter.ListViewHolder(convertView, position, ItemsList, this);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            
            if (listViewHolder!=null && position >= 0 && position < GetListObject.Count())
            {
                // Set controls in CradView with Object props Outsiderly (In Client source code)
                SetControlsInView(listViewHolder, position);

                //convertView.SetOnClickListener(InitOnItemClick(position));
            }


            return convertView;
        }

        private IEnumerable<Object> GetListObject
        {
            get
            {
                return ((IEnumerable<Object>)ItemsList);
            }
        }

        /// <summary>
        /// Set controls in CradView with Object props Outsiderly (In Client source code)
        /// </summary>
        /// <param name="listViewHolder"></param>
        /// <param name="position"></param>
        public void SetControlsInView(ListViewHolder listViewHolder, int position)  
        {

            if (OnListItemSetControlsInView != null)
            {
                // Rase evnt in client source code
                OnListItemSetControlsInView(listViewHolder, position);
            }

        }

        public void TimerStart()
        {
            TimerObject = new System.Timers.Timer();

            //TimerObject.BeginInit();   
            TimerObject.AutoReset = true;           // Continue repeatly fire events
            TimerObject.Elapsed += Timer_Tick;
            TimerObject.Interval = 500;
            TimerObject.Enabled = true;
            //TimerObject.Start();
        }

        private void Timer_Tick(object sender, ElapsedEventArgs e)
        {
            TimerStop();

            if (this.OnItemClick != null)
            {
                this.OnItemClick(null, this.SelectedItemPosition);
            }
        }

        private void TimerStop()
        {
            if (TimerObject != null)
            {
                TimerObject.Stop();
                TimerObject.Close();
                TimerObject.Dispose();
            }

            TimerObject = null;
        }

        public View.IOnClickListener InitOnItemClick(int position)
        {
            this.SelectedItemPosition = position;
            SetOnItemClick += new EventHandler(OnItemClick2);

            return null;
        }

        public void OnItemClick2(object sender, EventArgs e)
        {
            //if (OnItemClick != null)
            //{
            //    OnItemClick(null, SelectedPosition);
            //}

            SetOnItemClick(SelectedPosition, e);
        }



        // *** -------------------------------------------------------------***


        public class ListViewHolder // : RecyclerView.ViewHolder
        {
            public TextView FirstLine;
            public TextView SecondLine;
            public TextView ThirdLine;
            private CardView cardView;
            

            public View ViewObject { get; set; }
            private int ItemPosition { get; set; }
            private ListViewAdapter ParentListViewAdapter { get; set; }

            public event EventHandler SetOnItemClick;


            public ListViewHolder(View convertView, int position, Object itemsList, ListViewAdapter parentListViewAdapter)     // List<Task> 
            {
                this.ItemPosition = position;
                this.ParentListViewAdapter = parentListViewAdapter;
                this.ViewObject = convertView;

                if (convertView != null && position >= 0 && position < ((IEnumerable<Object>)itemsList).Count())
                {
                    try
                    {
                        FirstLine = (TextView)convertView.FindViewById(ProjTaskReminder.Resource.Id.txtTitle);
                        SecondLine = (TextView)convertView.FindViewById(ProjTaskReminder.Resource.Id.txtDescription);
                        ThirdLine = (TextView)convertView.FindViewById(ProjTaskReminder.Resource.Id.txtDateDue);
                        cardView = (CardView)convertView.FindViewById(ProjTaskReminder.Resource.Id.cardTask);

                        //cardView.Click += OnViewClick;
                        convertView.Click += OnViewClick;
                        //cardView.SetOnClickListener(InitOnItemClick(position));
                        //convertView.SetOnClickListener(InitOnItemClick(position));
                        //cardView.OnTouchEvent(Touch_Click);
                        //convertView.SetOnClickListener(this.ParentListViewAdapter.InitOnItemClick(position));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }


                // Remove the 'Selected' methods of the original ListView
                //if (position >= 0 && position < ((IEnumerable<Object>)itemsList).Count())
                //{
                    //convertView.SetOnClickListener(InitOnItemClick(position));   
                    //cardView.SetOnClickListener(InitOnItemClick(position)); 
                //}
                // Old - Fill controls in card by EVENT 'SetControlsInView(listViewHolder, position)'
                //if (position < tasks.Count)
                //{
                //    
                //    FirstLine.SetText(tasks[position].getTitle(), TextView.BufferType.Normal);
                //    SecondLine.SetText(tasks[position].getDescription(), TextView.BufferType.Normal);
                //    ThirdLine.SetText(tasks[position].getDate_due(), TextView.BufferType.Normal);
                //}
            }

            private void OnViewClick(object sender, EventArgs e)
            {
                if (ParentListViewAdapter.OnItemClick != null)
                {
                    ParentListViewAdapter.TimerStop();

                    ParentListViewAdapter.SelectedItemPosition = this.ItemPosition;

                    ParentListViewAdapter.TimerStart();

                    //ParentListViewAdapter.OnItemClick(this, this.ItemPosition);
                }
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

    }

        
}