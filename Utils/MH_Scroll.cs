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
        public bool IsScrollRightToLeft { get; set; }



        public MH_Scroll()
        {
            IsTimerWork = false;
            IsScrollng = false;
            IsScrollRightToLeft = false;
        }

        public MH_Scroll(HorizontalScrollView scrollViewControl)
        {
            this.ScrollControl = scrollViewControl;
            IsTimerWork = false;
            IsScrollng = false;
            IsScrollRightToLeft = false;
            ScrollControl.Click += Scroll_OnClick;
        }

        private void Scroll_OnClick(object sender, EventArgs e)
        {
            if (this.IsScrollng)
            {
                Stop();
            }
            else
            {
                Start();
            }

        }

        public bool Start()
        {
            bool result = false;

            TimerPicsScrollStop();

            TimerPicsScrollRun();

            return result;
        }

        public void Stop()
        {
            TimerPicsScrollStop();
        }

        public void StartPosstion()
        {
            ScrollControl.SmoothScrollingEnabled = true;

            if (!IsScrollRightToLeft)
            {
                keepX = 1;
            }
            else
            {
                keepX = this.SCROLL_END_POINT;  // ScrollControl.ScrollX
                ScrollControl.FullScroll(FocusSearchDirection.Right);
            }
            
            ScrollControl.SmoothScrollTo(keepX, 0);
            
            //if (this.SCROLL_DELTA < 0)
            //{
            //    ScrollControl.SmoothScrollTo(this.SCROLL_END_POINT, 0);
            //}

        }

        private Action TimerPicsScrollRun()
        {

            // Run the Timer
            timerScroll = new System.Timers.Timer();
            timerScroll.Interval = SCROLL_INTERVAL;
            timerScroll.AutoReset = true;
            timerScroll.Elapsed += Timer_onTick;
            timerScroll.Enabled = true;
            //timerScroll.Start();
            //timerScroll.schedule(picsTimerTask, 500, PICS_TIMER_INTERVAL);

            //StartPosstion();


            IsTimerWork = true;
            IsScrollng = true;

            return null;
        }

        private void Timer_onTick(object sender, ElapsedEventArgs e)
        {
            bool isGetToEdge = false;


            keepX += SCROLL_DELTA;

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
                if ((!IsScrollRightToLeft && keepX > SCROLL_END_POINT * -1) || (IsScrollRightToLeft && keepX < 0))
                {
                    //MainActivity.FadeInPicture(getApplicationContext(), imgSongArtist1, 1);
                    isGetToEdge = true;
                }
            }

            if (isGetToEdge)
            {
                if (!IsScrollRightToLeft)
                {
                    keepX = 1;
                }
                else
                {
                    keepX = this.SCROLL_END_POINT;
                }
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