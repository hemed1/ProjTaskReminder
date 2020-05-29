

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
        //private List<Task> ItemsList;
        private LayoutInflater layoutInflater;

        

        public event EventHandler SetOnClickListener;
        public event EventHandler SetOnItemClick;
        public static event Action<View.IOnClickListener> OnActivityResult;
        public event Action<ListViewHolder, int> OnListItemControlsView;

        public int ViewResourcesID = ProjTaskReminder.Resource.Layout.list_item;
        /// <summary>
        /// Not Used - To Execute some external method
        /// </summary>
        public int MethodNumberToSetViewControls { get; set; }


        //private Resources resources;
        //private int selectMode = 0;
        //private int selectedItemsCount;
        //private MainActivity mainActivity;
        
        //public override int Count => 21;
        //public abstract bool SetControlsInView(int position, View convertView, ViewGroup parent);



        public ListViewAdapter(Context applicationContext, Object itemsList)
        {
            this.context = applicationContext;
            ItemsList = itemsList;

            layoutInflater = LayoutInflater.From(this.context);

            ViewResourcesID = ProjTaskReminder.Resource.Layout.list_item;
            MethodNumberToSetViewControls = 1;
        }

        public ListViewAdapter(Context applicationContext, Object itemsList, int methodNumberToSetViewControls)
        {
            this.context = applicationContext;
            ItemsList = itemsList;

            layoutInflater = LayoutInflater.From(this.context);

            ViewResourcesID = ProjTaskReminder.Resource.Layout.list_item;
            MethodNumberToSetViewControls = methodNumberToSetViewControls;
        }

        public override int Count
        {
            get
            {
                return GetListObject().Count();
            }
        }

        //public override int ItemCount   // => throw new NotImplementedException();
        //{
        //    get
        //    {
        //        return GetListObject().Count();
        //    }
        //}

        //public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        //{
        //    ListViewHolder listViewHolder = (ListViewHolder)holder;
              //if (listViewHolder!=null && position >= 0 && position<GetListObject().Count())
              //{
              //      // Set controls in CradView with Object props Outsiderly (In Client source code)
              //      SetControlsInView(listViewHolder, position);
              //}

    //    if (position < GetListObject().Count)
    //    {
    //        Task task = GetListObject()[position];
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

            return view;       // GetListObject()[position];  // null;
        }

        public override long GetItemId(int position)
        {
            long res = 0;

            if (position > -1 && position < GetListObject().Count())
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

                if (position >= 0 && position < GetListObject().Count())
                {
                    listViewHolder = new ListViewAdapter.ListViewHolder(convertView, position, ItemsList);        //, context);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            
            if (listViewHolder!=null && position >= 0 && position < GetListObject().Count())
            {
                // Set controls in CradView with Object props Outsiderly (In Client source code)
                SetControlsInView(listViewHolder, position);
            }

            //convertView.SetOnClickListener(InitOnItemClick(position));

            return convertView;
        }

        private IEnumerable<Object> GetListObject()
        {
            return ((IEnumerable<Object>)ItemsList);

        }

        /// <summary>
        /// Set controls in CradView with Object props Outsiderly (In Client source code)
        /// </summary>
        /// <param name="listViewHolder"></param>
        /// <param name="position"></param>
        public void SetControlsInView(ListViewHolder listViewHolder, int position)  
        {

            if (OnListItemControlsView != null)
            {
                // Rase evnt in client source code
                OnListItemControlsView(listViewHolder, position);
            }

        }

        public class ListViewHolder // : RecyclerView.ViewHolder
        {
            public TextView title;
            public TextView description;
            public TextView date_due;
            public CardView cardView;

            public event EventHandler SetOnItemClick;
            private int ItemPosition { get; set; }


            public ListViewHolder(View convertView, int position, Object itemsList)     // List<Task> : base(convertView) //, Context containerContext) 
            {
                //int position = 2;   // this.ItemPosition;   // this.Position
                ItemPosition = position;
                
                if (convertView != null && position >= 0 && position < ((IEnumerable<Object>)itemsList).Count())
                {
                    try
                    {
                        title = (TextView)convertView.FindViewById(ProjTaskReminder.Resource.Id.txtTitle);
                        description = (TextView)convertView.FindViewById(ProjTaskReminder.Resource.Id.txtDescription);
                        date_due = (TextView)convertView.FindViewById(ProjTaskReminder.Resource.Id.txtDateDue);
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
                //    date_due.SetText(tasks[position].getDate_due(), TextView.BufferType.Normal);
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
        //    return ItemsList.Count;
        //}

       
    }

        
}