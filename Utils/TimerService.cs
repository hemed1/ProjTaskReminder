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
    public class TimerService
    {
        private Object EntityObject { get; set; }       // Task
        public int TimerInterval { get; set; }
        public DateTime? DateDue { get; set; }
        public enumTimerType TimerType { get; set; }
        public System.Timers.Timer TimerObject { get; set; }
        public System.Threading.Thread ThreadObject { get; set; }
        public Activity activity { get; set; }

        public event Action<Object, System.Timers.Timer> OnTimerTick;        //object, ElapsedEventArgs, 
        public event Action<Object, System.Threading.Thread> OnThreadTick;        //object, ElapsedEventArgs, 

        public enum enumTimerType
        {
            Timer = 1,
            Thread = 2
        }

        public TimerService(Object entityObject, enumTimerType timerType)     //System.Timers.Timer timerObject)
        {
            this.EntityObject = entityObject;
            this.TimerType = timerType;
            this.TimerInterval = 30000;
            //this.TimerObject = timerObject;
            //this.ThreadObject = threadObject;

            if (timerType==enumTimerType.Timer)
            {
                TimerObject = new Timer();
            }
            else
            {
                ThreadObject = new System.Threading.Thread(new System.Threading.ThreadStart(Thread_Elapsed));
                ThreadObject.IsBackground = true;
            }
        }

        //public TimerService(Object entityObject, System.Threading.Thread threadObject)
        //{
        //    this.EntityObject = entityObject;
        //    this.ThreadObject = threadObject;
        //    this.TimerInterval = 30000;
        //}

        /// <summary>
        /// Check if target DateDue reach the target mition
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {

            DateTime dateNow = MH_Utils.Utils.GetDateNow();
            DateTime? dateDue = this.DateDue;     // this.EntityObject.getDate();


            if (!dateDue.HasValue)
            {
                return;
            }

            if (dateNow.CompareTo(dateDue) >= 0)
            {
                TimerDispose();

                // Raise event Timer Tick
                if (OnTimerTick != null)
                {
                    OnTimerTick(EntityObject, TimerObject);
                }
            }
        }

        /// <summary>
        /// Check if target DateDue reach the target mition
        /// </summary>
        public void Thread_Elapsed()
        {
            long counter = 0;
            long limitSeconds = 1000 * 60 * 60 * 24 * 3;        // Limited for 3 Days



            DateTime? dateDue = this.DateDue;           // this.EntityObject.getDate();

            if (!dateDue.HasValue)
            {
                return;
            }


            DateTime dateNow = MH_Utils.Utils.GetDateNow();

            while (counter <= limitSeconds && dateNow.CompareTo(dateDue) < 0)
            {
                System.Threading.Thread.Sleep(TimerInterval);

                activity.RunOnUiThread(() =>
                {
                    dateNow = MH_Utils.Utils.GetDateNow();
                });

                counter += TimerInterval;
            }

            ThreadDispose();

            // Raise event Timer Tick
            if (OnThreadTick != null)
            {
                OnThreadTick(EntityObject, ThreadObject);
            }

            return;
        }

        private void TimerDispose()
        {
            if (TimerObject != null)
            {
                TimerObject.Stop();
                TimerObject.Close();
                TimerObject.Dispose();
            }

            TimerObject = null;
        }

        private void ThreadDispose()
        {
            if (ThreadObject != null && ThreadObject.IsAlive)
            {
                ThreadObject.Abort();
            }

            ThreadObject = null;
        }

        public void Start()
        {
            if (TimerObject != null)
            {
                //TimerObject.BeginInit();   
                TimerObject.AutoReset = true;           // Continue repeatly fire events
                TimerObject.Elapsed += Timer_Elapsed;
                TimerObject.Interval = TimerInterval;   // 30 seconds
                TimerObject.Enabled = true;
                //TimerObject.Start();
            }

            if (ThreadObject != null)
            {
                ThreadObject.Start();
            }
        }

        public void Destroy()
        {
            TimerDispose();

            ThreadDispose();
        }
    }
}