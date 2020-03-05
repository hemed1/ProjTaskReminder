using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
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
        private System.Timers.Timer timerScroll;
        private bool IsTimerWork;
        public int SCROLL_INTERVAL = 200;
        public int SCROLL_DELTA = 7;
        public int SCROLL_END_POINT = 2400;

        public event Action<int> OnScrolling;
        public bool IsScrollng { get; set; }


        public MH_Scroll()
        {
            IsTimerWork = false;
        }

        public MH_Scroll(HorizontalScrollView scrollViewControl)
        {
            this.ScrollControl = scrollViewControl;
            IsTimerWork = false;
            IsScrollng = false;
        }


        public bool Start()
        {
            bool result = false;

            //if (IsHaveToScroll)
            //{
            //    if (IsTimerWork)
            //    {
            TimerPicsScrollStop();
            //    }
            //    return;
            //}

            //ActionOnPicsScrolling += TimerPicsScrollRun();

            TimerPicsScrollRun();

            //scrHorizon.PostDelayed(ActionOnPicsScrolling, 1000);    
            //{
            //new Runnable()
            //public void run()
            //{
            //    if (!IsTimerWork)
            //    {
            //        TimerPicsScrollRun();
            //    }
            //}
            //}

            return result;
        }

        public void Stop()
        {
            TimerPicsScrollStop();
        }

        private Action TimerPicsScrollRun()
        {
            IsTimerWork = true;
            IsScrollng = true;

            //keepX = 1000;
            //Timer_onTick(null, null);

            keepX = 0;

            // Run the Timer
            timerScroll = new System.Timers.Timer();
            timerScroll.Interval = 200;
            timerScroll.AutoReset = true;
            timerScroll.Elapsed += Timer_onTick;
            timerScroll.Enabled = true;
            //timerScroll.Start();
            //timerScroll.schedule(picsTimerTask, 500, PICS_TIMER_INTERVAL);

            return null;
        }

        private void Timer_onTick(object sender, ElapsedEventArgs e)
        {
            bool isGetToEdge = false;


            keepX += SCROLL_DELTA;

            ScrollControl.SmoothScrollingEnabled = true;
            ScrollControl.SmoothScrollTo(keepX, 0);
            
            //scrHorizon.scrollTo(keepX, 0);
            //scrHorizon.fullScroll(HorizontalScrollView.FOCUS_RIGHT);

            if (SCROLL_DELTA > 0)
            {
                if (keepX > SCROLL_END_POINT)  //scrHorizon.getRight())
                {
                    //MainActivity.FadeInPicture(getApplicationContext(), imgSongArtist3, 2);
                    isGetToEdge = true;
                }
            }
            else
            {
                if (keepX > SCROLL_END_POINT * -1)
                {
                    //MainActivity.FadeInPicture(getApplicationContext(), imgSongArtist1, 1);
                    isGetToEdge = true;
                }
            }

            if (isGetToEdge)
            {
                keepX = 1;
                //SCROLL_DELTA = SCROLL_DELTA * -1;
                //keepX += SCROLL_DELTA;
            }

            if (OnScrolling!=null)
            {
                OnScrolling(keepX);
            }
        }


        private void TimerPicsScrollStop()
        {
            if (timerScroll != null && (IsTimerWork || timerScroll.Enabled))
            {
                ScrollControl.SmoothScrollingEnabled = false;
                timerScroll.Stop();
                timerScroll.Close();
                timerScroll.Dispose();
                timerScroll = null;
                //picsTimerTask.cancel();
                //picsTimerTask = null;
                //picsTimerTask.run();
            }

            IsTimerWork = false;
            IsScrollng = false;
        }

    }
}