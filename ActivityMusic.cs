using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Icu.Text;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Java.Util;
using ProjTaskReminder.Model;
using ProjTaskReminder.Utils;
using System.Timers;
using System.IO;

namespace ProjTaskReminder
{
    [Activity(Label = "ActivityMusic", Theme = "@style/AppTheme.NoActionBar")]
    public class ActivityMusic : AppCompatActivity
    {
        private Button btnPrev;
        private Button btnNext;
        private Button btnPlay;
        private MediaPlayer mediaPlayer;
        private SeekBar barSeek;
        private TextView lblSongName;
        private TextView lblSongArtist;
        private TextView lblAlbum;
        private TextView lblPosNow;
        private TextView lblPosLeft;
        private int CurrentSongPosition;
        private ImageView imgSongArtist1;
        private ImageView imgSongArtist2;
        private ImageView imgSongArtist3;


        private TimerTask TimerTaskSongProgress;
        private System.Timers.Timer TimerSongProgress;
        private TimerTask picsTimerTask;
        private System.Timers.Timer picsTimer;
        private bool IsTimerWork;
        private bool IsHaveToScroll;
        private int keepX;
        private int PICS_TIMER_INTERVAL = 18;
        private int PICS_TIMER_SCROLL_DELTA = 5;
        private HorizontalScrollView scrHorizon;
        private event Action ActionOnPicsScrolling;


        public static Context context;
        private List<KeyValuePair<string, List<string>>> ListItemsPath;
        private List<KeyValuePair<string, ListItemSong>> ListItemsRecycler;
        private int ListPositionIndex;
        private bool isPlayingNow;
        private Thread ThreadTask;
        private event Action ActionOnPlayingMusic;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_music);

            SetControlsIO();

            LoadFilesFromPhone();
        }

        private void LoadFilesFromPhone()
        {
            string folderBackup = Android.OS.Environment.DirectoryMusic;        // "ProjTaskReminder"

            //Java.IO.File[] jjj = context.GetExternalFilesDirs("MUSIC");
            //Java.IO.File[] mmm = context.GetExternalMediaDirs();
            //string zzz = System.Environment.SystemDirectory;
            //string aaa = System.Environment.CurrentDirectory;
            //string bbb = System.Environment.GetFolderPath(System.Environment.SpecialFolder.CommonMusic);
            //string fff = System.IO.Directory.GetCurrentDirectory();
            //string hjhj = LOG_FILE_PATH = System.IO.Directory.GetCurrentDirectory();
            //Java.IO.File hdhd = Android.OS.Environment.DataDirectory;
            //string folderNameDocuments = Android.OS.Environment.DirectoryDocuments;
            //Java.IO.File externalPath = Android.OS.Environment.ExternalStorageDirectory;

            string file = "";
            string path = Android.OS.Environment.GetExternalStoragePublicDirectory(folderBackup).AbsolutePath;

            Utils.Utils.GetFolderFiles(path, "*.mp3", true, "*.jpg");

            ListItemsPath = Utils.Utils.FilesExtra;

            ListItemsRecycler.Clear();

            for (int i=0; i<ListItemsPath.Count; i++)
            {
                file = Directory.GetParent(ListItemsPath[i].Key).FullName;
                file = ListItemsPath[i].Key.Substring(file.Length + 1);
                file = Utils.Utils.FixSongName(file);
                ListItemSong listItemSong = new ListItemSong(file, "Artist " + i.ToString(), "Album " + i.ToString());
                ListItemsRecycler.Add(new KeyValuePair<string, ListItemSong>(listItemSong.getSongName(), listItemSong));
            }
        }

        private void SetControlsIO()
        {
            btnPrev = (Button)FindViewById(Resource.Id.btnPrev);
            btnNext = (Button)FindViewById(Resource.Id.btnNext);
            btnPlay = (Button)FindViewById(Resource.Id.btnPlay);
            //mediaPlayer = (MediaPlayer)FindViewById(Resource.Id.mediaControllerMain);
            barSeek = (SeekBar)FindViewById(Resource.Id.barSeek);

            barSeek.BringToFront();
            barSeek.ProgressChanged += OnSeekbarChanged;

            lblSongName = (TextView)FindViewById(Resource.Id.lblSongName);
            lblSongArtist = (TextView)FindViewById(Resource.Id.lblSongArtist);
            lblAlbum = (TextView)FindViewById(Resource.Id.lblAlbum);
            lblPosNow = (TextView)FindViewById(Resource.Id.lblPosNow);
            lblPosLeft = (TextView)FindViewById(Resource.Id.lblPosLeft);

            imgSongArtist1 = (ImageView)FindViewById(Resource.Id.imgSongArtist1);
            imgSongArtist2 = (ImageView)FindViewById(Resource.Id.imgSongArtist2);
            imgSongArtist3 = (ImageView)FindViewById(Resource.Id.imgSongArtist3);
            scrHorizon = (HorizontalScrollView)FindViewById(Resource.Id.scrHorizon);

            ListPositionIndex = 0;
            ListItemsRecycler = new List<KeyValuePair<string, ListItemSong>>();
            

            string folderNameMusic = Android.OS.Environment.DirectoryMusic;
            string folderMusic = Android.OS.Environment.GetExternalStoragePublicDirectory(folderNameMusic).AbsolutePath;
            string songPath = folderMusic + "/Dizzy - Bleachers.mp3";

            

            //songPath = externalPath + "/ProjTaskReminder";    //, FileCreationMode.Append).AbsolutePath;
            //Java.IO.File externalPath = Android.OS.Environment.ExternalStorageDirectory;
            //string externalPathFile = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;

            KeyValuePair<string, List<string>> songKeys = new KeyValuePair<string, List<string>>(folderMusic + "/Dizzy - Bleachers.mp3", new List<string>());
            ListItemsPath = new List<KeyValuePair<string, List<string>>>();
            ListItemsPath.Add(songKeys);
            //KeyValuePair<string, string> songKeys = new KeyValuePair<string, string>("love_the_one.mp3", songPath);
            //ListItemsPath.Add(songKeys);

            ListItemSong song = new ListItemSong("Dizzy - Bleachers", "Dizzy", "Album");
            KeyValuePair<string, ListItemSong> songItem = new KeyValuePair<string, ListItemSong>("love_the_one", song);
            ListItemsRecycler.Add(songItem);


            mediaPlayer = new MediaPlayer();
            mediaPlayer.Completion += OnSongFinish;

            btnPrev.Click += PlaySongPrev;
            btnNext.Click += PlaySongNext;
            btnPlay.Click += PlaySongPlay;

            isPlayingNow = false;
            CurrentSongPosition = 0;
            IsTimerWork = false;
            IsHaveToScroll = true;
        }

        private void OnSeekbarChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            //UpdateProgressControls();

            CurrentSongPosition = e.Progress;

            if (e.FromUser)
            {
                mediaPlayer.SeekTo(CurrentSongPosition);
            }

        }

        private void OnSongFinish(object sender, EventArgs eventArgs)
        {
            PlaySongNext(sender, eventArgs);
        }

        private void PlaySongPlay(object sender, EventArgs eventArgs)
        {
            if (!isPlayingNow)
            {
                if (CurrentSongPosition == 0)
                {
                    if (ListPositionIndex <= ListItemsRecycler.Count - 1)
                    {
                        LoadSongIntoPlayer(ListPositionIndex);  //, true);
                    }
                }
                else
                {
                    isPlayingNow = true;
                    MusicPlay();
                    mediaPlayer.SeekTo(CurrentSongPosition);
                    //mediaPlayer.Notify();
                    //LoadSongIntoPlayer(ListPositionIndex, false);
                }
                IsHaveToScroll = !IsHaveToScroll;
                //setPicsScroll();
            }
            else
            {
                isPlayingNow = false;
                MusicPause();
            }

        }

        private void PlaySongNext(object sender, EventArgs eventArgs)
        //private void PlaySongNext(object sender, AdapterView.ItemClickEventArgs e)
        {
            if (ListPositionIndex < ListItemsRecycler.Count)    // ListItemsRecycler.Count - 1)
            {
                ListPositionIndex++;
                LoadSongIntoPlayer(ListPositionIndex);
            }

            //seekMusic(5000);
        }

        private void PlaySongPrev(object sender, EventArgs eventArgs)
        //private void PlaySongPrev(object sender, AdapterView.ItemClickEventArgs e)
        {
            if (ListPositionIndex < ListItemsRecycler.Count && ListPositionIndex > 0)       // //  ListItemsRecycler.Count
            {
                ListPositionIndex--;
                LoadSongIntoPlayer(ListPositionIndex);
            }
            //seekMusic(-5000);
        }

        private void LoadSongIntoPlayer(int listPositionIndex)  //, bool playFromStart = true)
        {

            try
            {
                //if (playFromStart)
                //{
                MusicPause();

                CurrentSongPosition = 0;
                
                int resourceID = Resource.Raw.love_the_one;
                //int resourceID = ListItemsRecycler.get(listPositionIndex).getResourceID();

                //string folderNameMusic = Android.OS.Environment.DirectoryMusic;
                //string folderMusic = Android.OS.Environment.GetExternalStoragePublicDirectory(folderNameMusic).AbsolutePath;
                //string songPath = folderMusic + "/Brad.mp3";
                string songPath= ListItemsPath[listPositionIndex].Key;
                Android.Net.Uri uri = Android.Net.Uri.Parse(songPath);

                mediaPlayer = MediaPlayer.Create(this, uri);
                //mediaPlayer = MediaPlayer.Create(this, resourceID); 

                mediaPlayer.SetScreenOnWhilePlaying(true);

                // Set the Song props - Name, Artist, Album, Duration
                SetSongControls(listPositionIndex);

                MusicPlay();

                //}
                //else
                //{
                //    MusicPlay();
                //    mediaPlayer.SeekTo(CurrentSongPosition);
                //    //mediaPlayer.Notify();
                //}

                //mediaPlayer.SetOnCompletionListener(MediaPlayer.IOnCompletionListener());
                //{
                //    @Override
                //    public void onCompletion(MediaPlayer mediaPlayer)
                //    {
                //        PlaySongNext();
                //    }
                //});
            }
            catch (Exception ex)
            {
                //Println("Error while load a song into Media player.. \n" + e.getMessage());
            }



        }

        private void MusicPause()
        {
            if (mediaPlayer != null)
            {
                //if (thread != null)
                //{
                //    thread.interrupt();
                //    thread = null;
                //}

                mediaPlayer.Pause();

                //mediaPlayer.Stop();
                btnPlay.SetBackgroundResource(Android.Resource.Drawable.IcMediaPlay);
            }

        }

        public void MusicPlay()
        {
            if (mediaPlayer != null)
            {
                mediaPlayer.Start();

                isPlayingNow = true;

                btnPlay.SetBackgroundResource(Android.Resource.Drawable.IcMediaPause);

                //setPicsScroll();

                UpdateSongPositionThread();
            }
        }

        public void MusicStop()
        {
            //TimerStop();

            //TimerStopSongProgress();

            if (mediaPlayer != null)
            {
                mediaPlayer.Stop();
                mediaPlayer.Release();
                //mediaPlayer.reset();
            }

            mediaPlayer = null;
            //ThreadTask.Abort();
            ThreadTask = null;

        }

        private void seekMusic(int interval)
        {
            barSeek.Progress = barSeek.Progress + interval;
            mediaPlayer.SeekTo(barSeek.Progress);
        }

        // Set the Song props - Name, Artist, Album, Duration
        private void SetSongControls(int listPositionIndex)
        {
            Drawable drawable;
            Bitmap bitmap;


            ListItemSong item = ListItemsRecycler[listPositionIndex].Value;

            lblSongName.Text = item.getSongName();
            lblSongArtist.Text = item.getArtist();
            lblAlbum.Text = item.getAlbum();

            if (mediaPlayer != null)
            {
                CurrentSongPosition = 0;
                //MusicStop();
                mediaPlayer.SeekTo(0);
                barSeek.Max = mediaPlayer.Duration;
                barSeek.SetProgress(0, false);
            }

            UpdateProgressControls();

            //item.setDuration(mediaPlayer.getDuration());

            //if (item.getPicsToSongPathsArray().size() > 0)
            //{
            //    bitmap = ConvertPictureFileToDrawable(item.getPicsToSongPathsArray().get(0));
            //    imgSongArtist1.setImageBitmap(bitmap);
            //}
            //else
            //{
            //    imgSongArtist1.setImageDrawable(getDrawable(R.drawable.default1));
            //}

            //MainActivity.FadeInPicture(getApplicationContext(), imgSongArtist1, 1);

            //if (item.getPicsToSongPathsArray().size() > 1)
            //{
            //    bitmap = ConvertPictureFileToDrawable(item.getPicsToSongPathsArray().get(1));
            //    imgSongArtist2.setImageBitmap(bitmap);
            //}
            //else
            //{
            //    imgSongArtist2.setImageDrawable(getDrawable(R.drawable.default2));
            //}


            //if (item.getPicsToSongPathsArray().size() > 2)
            //{
            //    bitmap = ConvertPictureFileToDrawable(item.getPicsToSongPathsArray().get(2));
            //    imgSongArtist2.setImageBitmap(bitmap);
            //}
            //else
            //{
            //    imgSongArtist3.setImageDrawable(getDrawable(R.drawable.default3));
            //}

            //keepX = 0;

        }

        private void UpdateProgressControls()
        {
            int currentPos = mediaPlayer.CurrentPosition;
            int duration = mediaPlayer.Duration;
            CurrentSongPosition = currentPos;

            try
            {
                SimpleDateFormat dateFormat = new SimpleDateFormat("mm:ss");
                lblPosNow.Text = dateFormat.Format(new Date(currentPos));
                lblPosLeft.Text = dateFormat.Format(new Date(duration));
            }
            catch (Exception ex)
            {

            }

            //lblPosNow.Text = new DateTime(currentPos).ToString("mm:ss");
            //lblPosLeft.Text = new DateTime(duration).ToString("mm:ss");
        }

        public void UpdateSongPositionThread()
        {
            //TimerRunSongProgress();

            ActionOnPlayingMusic += OnPlayingMusic;

            ThreadTask = new Thread(new ThreadStart(firstThread));
            //var second = new Thread(new ThreadStart(secondThread));
            //btnStart.TouchUpInside += delegate {

            ThreadTask.Start();


            // causes a 10ms delay between starting the next thread
            //second.Start();


            //        thread = new Thread()
            //        {
            //            @Override
            //            public void run()
            //            {
            //                try
            //                {
            //                    while (mediaPlayer != null && mediaPlayer.isPlaying())
            //                    {
            //                        Thread.sleep(500);
            //                        runOnUiThread(new Runnable()
            //                        {
            //                            @Override
            //                            public void run()
            //                            {
            //                                int newPosition = mediaPlayer.getCurrentPosition();
            //                                barSeek.setProgress(newPosition);
            //                            }
            //                        });
            //
            //                    }
            //                } catch (InterruptedException e)
            //                {
            //                    e.printStackTrace();
            //                }
            //            }
            //        };
            //
            //        thread.start();
        }

        private void firstThread()
        {
            while (mediaPlayer != null && mediaPlayer.IsPlaying)
            {
                Thread.Sleep(1000);

                RunOnUiThread(ActionOnPlayingMusic);    // =>
                //{
                    //int newPosition = mediaPlayer.CurrentPosition;
                    //barSeek.Progress = newPosition;
                //};

                UpdateProgressControls();
            }

            //ThreadTask.Abort();
            ThreadTask = null;


        }

        protected void OnPlayingMusic()     //int songsListIndex, int currentMusicPlayerPosition)
        {
            int newPosition = mediaPlayer.CurrentPosition;
            barSeek.Progress = newPosition;
            CurrentSongPosition = mediaPlayer.CurrentPosition;
        }

        private void secondThread()
        {
            //int i=0;
            //string text = string.Empty;
            //while (i < 10)
            //{
            //    text = string.Format("1st thread going i from {0} to {1}", i, ++i);
            //    InvokeOnMainThread(delegate () {
            //        thread1.Text = text;
            //    });

            //}
        }

        protected override void OnDestroy()
        {
            //if (thread != null)
            //{
            //    thread.Interrupt();
            //    thread = null;
            //}

            MusicStop();

            Toast.MakeText(this, "Destroing Media Player control", ToastLength.Long).Show();

            base.OnDestroy();
        }

        private void setPicsScroll()
        {

            if (IsHaveToScroll)
            {
                if (IsTimerWork)
                {
                    TimerStop();
                }
                return;
            }

            ActionOnPicsScrolling += TimerRun();

            scrHorizon.PostDelayed(ActionOnPicsScrolling, 1000);    
            //{
                //new Runnable()
                //public void run()
                //{
                //    if (!IsTimerWork)
                //    {
                //        TimerRun();
                //    }
                //}
            //}

        }

        private Action TimerRun()
        {
            IsTimerWork = true;

            //picsTimerTask = new TimerTask()
            //{
            //    @Override
            //    public void run()
            //    {
            //        picsTimer_onTick();
            //    }
            //};

            // Run the Timer
            picsTimer = new System.Timers.Timer();
            picsTimer.Interval = 500;
            picsTimer.AutoReset = true;
            picsTimer.Elapsed += picsTimer_onTick;
            picsTimer.Enabled = true;
            //picsTimer.schedule(picsTimerTask, 500, PICS_TIMER_INTERVAL);

            return null;
        }

        private void TimerStop()
        {
            if (IsTimerWork)
            {
                picsTimer.Stop();
                picsTimer.Close();
                picsTimer.Dispose();
                picsTimer = null;
                //picsTimerTask.cancel();
                //picsTimerTask = null;
                //picsTimerTask.run();
            }
            IsTimerWork = false;
        }

        private void picsTimer_onTick(object sender, ElapsedEventArgs e)
        {
            bool isGetToEdge = false;


            scrHorizon.SmoothScrollTo(keepX, 0);

            //scrHorizon.scrollTo(keepX, 0);
            //scrHorizon.fullScroll(HorizontalScrollView.FOCUS_RIGHT);

            keepX += PICS_TIMER_SCROLL_DELTA;

            if (PICS_TIMER_SCROLL_DELTA > 0)
            {
                if (keepX > (imgSongArtist1.Width * 3) - 500)  //scrHorizon.getRight())
                {
                    //MainActivity.FadeInPicture(getApplicationContext(), imgSongArtist3, 2);
                    isGetToEdge = true;
                }
            }
            else
            {
                if (keepX < PICS_TIMER_SCROLL_DELTA * -1 - 450)
                {
                    //MainActivity.FadeInPicture(getApplicationContext(), imgSongArtist1, 1);
                    isGetToEdge = true;
                }
            }

            if (isGetToEdge)
            {
                PICS_TIMER_SCROLL_DELTA = PICS_TIMER_SCROLL_DELTA * -1;
                keepX += PICS_TIMER_SCROLL_DELTA;
            }

        }


        //    private void picsTimer_onTick()
        //    {
        //        boolean isGetToEdge = false;

        //        scrHorizon.smoothScrollTo(keepX, 0);
        //        //scrHorizon.scrollTo(keepX, 0);
        //        //scrHorizon.fullScroll(HorizontalScrollView.FOCUS_RIGHT);

        //        keepX += PICS_TIMER_SCROLL_DELTA;

        //        if (PICS_TIMER_SCROLL_DELTA > 0)
        //        {
        //            if (keepX > (imgSongArtist1.getWidth() * 3) - 1000)  //scrHorizon.getRight())
        //            {
        //                //MainActivity.FadeInPicture(getApplicationContext(), imgSongArtist3, 2);
        //                isGetToEdge = true;
        //            }
        //        }
        //        else
        //        {
        //            if (keepX < PICS_TIMER_SCROLL_DELTA * -1 - 450)
        //            {
        //                //MainActivity.FadeInPicture(getApplicationContext(), imgSongArtist1, 1);
        //                isGetToEdge = true;
        //            }
        //        }

        //        if (isGetToEdge)
        //        {
        //            PICS_TIMER_SCROLL_DELTA = PICS_TIMER_SCROLL_DELTA * -1;
        //            keepX += PICS_TIMER_SCROLL_DELTA;
        //        }

        //    }

        //    private void TimerRunSongProgress()
        //    {
        //        TimerTaskSongProgress = new TimerTask()
        //    {
        //        @Override
        //        public void run()
        //        {
        //            TimerSongProgress_onTick();
        //        }
        //    };

        //    // Run the Timer
        //    TimerSongProgress = new Timer();
        //    TimerSongProgress.schedule(TimerTaskSongProgress, 900, 900);
        //}

        //    private void TimerStopSongProgress()
        //    {
        //        if (TimerSongProgress != null)
        //        {
        //            TimerSongProgress.cancel();
        //            TimerSongProgress.purge();
        //            TimerTaskSongProgress.cancel();
        //            TimerSongProgress = null;
        //            TimerTaskSongProgress = null;
        //        }
        //    }

        //    private void TimerSongProgress_onTick()
        //    {
        //        int newPosition = mediaPlayer.getCurrentPosition();
        //        barSeek.setProgress(newPosition);
        //    }


        //    private void TimerRun()
        //    {
        //        IsTimerWork = true;
        //        picsTimerTask = new TimerTask()
        //        {
        //            @Override
        //            public void run()
        //        {
        //            picsTimer_onTick();
        //        }
        //        };

        //    // Run the Timer
        //    picsTimer = new Timer();
        //    picsTimer.schedule(picsTimerTask, 500, PICS_TIMER_INTERVAL);
        //    }

        //    private void TimerStop()
        //    {
        //        if (IsTimerWork)
        //        {
        //            picsTimer.cancel();
        //            picsTimer.purge();
        //            picsTimerTask.cancel();
        //            picsTimer = null;
        //            picsTimerTask = null;
        //            //picsTimerTask.run();
        //        }
        //        IsTimerWork = false;
        //    }

    }

}
