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

namespace ProjTaskReminder.Utils
{
    public class MH_Scroll
    {
        public HorizontalScrollView ScrollControl { get; set;}


        private int keepX;
        public int PICS_TIMER_INTERVAL = 18;
        public int PICS_TIMER_SCROLL_DELTA = 5;

        public MH_Scroll()
        {

        }
    }
}