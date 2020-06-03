

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



namespace ProjTaskRemindet.Utils
{
    public class ListViewAdapter : BaseAdapter      //RecyclerView.Adapter    
    {
        private readonly Context context;
        private readonly Object ItemsList;
        private LayoutInflater layoutInflater;
        public int ViewResourcesID = ProjTaskReminder.Resource.Layout.list_item;
        



        public event Action<ListViewHolder, int> OnListItemSetControlsInView;
        public event Action<ListViewHolder, int> OnItemClick;       // Not in Use
        public View.IOnClickListener OnItemClick2;

        //private Resources resources;
        //private int selectMode = 0;
        //private int selectedItemsCount;
        //private MainActivity mainActivity;




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
    //        listViewHolder.title.SetText(task.getTitle(), TextView.BufferType.Normal);
    //        listViewHolder.description.SetText(task.getDescription(), TextView.BufferType.Normal);
    //        listViewHolder.date_due.SetText(task.getDate_due(), TextView.BufferType.Normal);
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

            if (position > -1 && position < GetListObject.Count())
            {
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
            }

            //convertView.SetOnClickListener(InitOnItemClick(position));

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

        public class ListViewHolder // : RecyclerView.ViewHolder
        {
            public TextView title;
            public TextView description;
            public TextView date_due;
            public CardView cardView;
            

            public View ViewObject { get; set; }
            private int ItemPosition { get; set; }
            private ListViewAdapter ParentListViewAdapter { get; set; }

            //public event EventHandler SetOnItemClick;



            public ListViewHolder(View convertView, int position, Object itemsList, ListViewAdapter parentListViewAdapter)     // List<Task> 
            {
                this.ItemPosition = position;
                this.ParentListViewAdapter = parentListViewAdapter;
                this.ViewObject = convertView;

                if (convertView != null && position >= 0 && position < ((IEnumerable<Object>)itemsList).Count())
                {
                    try
                    {
                        title = (TextView)convertView.FindViewById(ProjTaskReminder.Resource.Id.txtTitle);
                        description = (TextView)convertView.FindViewById(ProjTaskReminder.Resource.Id.txtDescription);
                        date_due = (TextView)convertView.FindViewById(ProjTaskReminder.Resource.Id.txtDateDue);
                        cardView = (CardView)convertView.FindViewById(ProjTaskReminder.Resource.Id.cardTask);

                        cardView.Click += OnViewClick;
                        //convertView.Click += OnViewClick;
                        //convertView.SetOnClickListener(this.ParentListViewAdapter.OnItemClick2);
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
                //    // Old - Fill controls in card by EVENT 'SetControlsInView(listViewHolder, position)'
                //    title.SetText(tasks[position].getTitle(), TextView.BufferType.Normal);
                //    description.SetText(tasks[position].getDescription(), TextView.BufferType.Normal);
                //    date_due.SetText(tasks[position].getDate_due(), TextView.BufferType.Normal);
                //}
            }

            private void OnViewClick(object sender, EventArgs e)
            {
                if (ParentListViewAdapter.OnItemClick != null)
                {
                    ParentListViewAdapter.OnItemClick(this, this.ItemPosition);
                }
            }



        }

    }

        
}