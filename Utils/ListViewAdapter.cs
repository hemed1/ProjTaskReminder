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

//using Android.App.Activity;
//using Android.Content;
//using Android.

namespace ProjTaskRemindet.Utils
{
    public class ListViewAdapter : BaseAdapter
    {
        private readonly Context context;
        //private MainActivity mainActivity;
        //private List<Task> TaskList;
        private readonly List<Task> TaskList;

        //private Resources resources;
        //private int selectMode = 0;
        //private int selectedItemsCount;

        private LayoutInflater layoutInflater;

        //public override int Count => 21;

        

        public ListViewAdapter(Context applicationContext, List<Task> itemsList)
        {
            this.context = applicationContext;
            this.TaskList = itemsList;

            layoutInflater = LayoutInflater.From(this.context);
        }

        public override int Count
        {
            get
            {
                return this.TaskList.Count;
            }
        }

        public override Java.Lang.Object GetItem(int position)
        {
            //View view = layoutInflater.Inflate(ProjTaskReminder.Resource.Layout.list_item, null);

            //view.FindViewById(R.id.gv_players);

            return null;    // view;    // TaskList[position];  // null;
        }

        public override long GetItemId(int position)
        {
            return position;    //TaskList[position];
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            //if (position >= TaskList.Count)
            //{
            //    return convertView;
            //}

            //layoutInflater = LayoutInflater.From(this.context);

            if (convertView == null)
            {
                convertView = layoutInflater.Inflate(ProjTaskReminder.Resource.Layout.list_item, parent, false);
            }

            if (position < TaskList.Count)
            {
                TextView txtTitle = (TextView)convertView.FindViewById(ProjTaskReminder.Resource.Id.txtTitle);
                TextView txtDescription = (TextView)convertView.FindViewById(ProjTaskReminder.Resource.Id.txtDescription);
                TextView txtDateDue = (TextView)convertView.FindViewById(ProjTaskReminder.Resource.Id.txtDateDue);


                txtTitle.SetText(TaskList[position].getTitle(), TextView.BufferType.Normal);
                txtDescription.SetText(TaskList[position].getDescription(), TextView.BufferType.Normal);
                txtDateDue.SetText(TaskList[position].getDate_due(), TextView.BufferType.Normal);
            }


            //ImageView icon = (ImageView)convertView.FindViewById(Resource.Id.icon);
            //icon.setImageResource(TaskList[i]);

            return convertView;
        }

        public  int Count2()  // override
        {
            return TaskList.Count;
        }
    }

        
    }