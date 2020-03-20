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
        public static MediaPlayer mediaPlayer;
        private SeekBar barSeek;
        private TextView lblSongName;
        private TextView lblSongArtist;
        private TextView lblAlbum;
        private TextView lblPosNow;
        private TextView lblPosEnd;
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
        private bool IsFirstPlay;
        private int keepX;
        private int PICS_TIMER_INTERVAL = 200;
        private int PICS_TIMER_SCROLL_DELTA = 5;
        private int PICS_TIMER_SCROLL_END_POINT;
        private HorizontalScrollView scrHorizon;
        private HorizontalScrollView scrHorizonSongName;
        private event Action ActionOnPicsScrolling;
        private MH_Scroll ScrollPictures;
        private MH_Scroll ScrollSongName;




        public static Context context;
        public static string MUSIC_PATH = "/storage/emulated/0/Music";
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
            string folderBackup = Android.OS.Environment.DirectoryMusic;        // "ProjTaskReminder"
            ////MUSIC_PATH = Android.OS.Environment.GetExternalStoragePublicDirectory(folderBackup).AbsolutePath;

            lblSongName.Text = "טוען שירים ...";

            Utils.Utils.GetFolderFiles(MUSIC_PATH, "*.mp3", true, "*.jpg");

            lblSongName.Text = "";

            ListItemsPath = Utils.Utils.FilesExtra;

            if (ListItemsPath==null)
            {
                return;
            }

            ListItemsRecycler.Clear();

            for (int i=0; i<ListItemsPath.Count; i++)
            {
                file = Directory.GetParent(ListItemsPath[i].Key).FullName;
                file = ListItemsPath[i].Key.Substring(file.Length + 1);
                //if (file.Length>30)
                //{
                //    file = file.Substring(0, 30);
                //}
                file = Utils.Utils.FixSongName(file);
                file = file.Substring(0, file.Length - 4);
                ListItemSong listItemSong = new ListItemSong(file, "Artist " + (i+1).ToString(), "Album " + (i+1).ToString());
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
            scrHorizonSongName = (HorizontalScrollView)FindViewById(Resource.Id.scrHorizonSongName);
            lblSongArtist = (TextView)FindViewById(Resource.Id.lblSongArtist);
            lblAlbum = (TextView)FindViewById(Resource.Id.lblAlbum);
            lblPosNow = (TextView)FindViewById(Resource.Id.lblPosNow);
            lblPosEnd = (TextView)FindViewById(Resource.Id.lblPosEnd);

            imgSongArtist1 = (ImageView)FindViewById(Resource.Id.imgSongArtist1);
            imgSongArtist2 = (ImageView)FindViewById(Resource.Id.imgSongArtist2);
            imgSongArtist3 = (ImageView)FindViewById(Resource.Id.imgSongArtist3);
            scrHorizon = (HorizontalScrollView)FindViewById(Resource.Id.scrHorizon);
            PICS_TIMER_SCROLL_END_POINT = 1000;     // (imgSongArtist1.Width * 3) - 500;


            ListPositionIndex = 0;
            ListItemsRecycler = new List<KeyValuePair<string, ListItemSong>>();
            ListItemsPath = new List<KeyValuePair<string, List<string>>>();


            //string folderNameMusic = Android.OS.Environment.DirectoryMusic;
            //string folderMusic = Android.OS.Environment.GetExternalStoragePublicDirectory(folderNameMusic).AbsolutePath;
            //string songPath = folderMusic + "/Dizzy - Bleachers.mp3";

            

            //songPath = externalPath + "/ProjTaskReminder";    //, FileCreationMode.Append).AbsolutePath;
            //Java.IO.File externalPath = Android.OS.Environment.ExternalStorageDirectory;
            //string externalPathFile = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;

            //KeyValuePair<string, List<string>> songKeys = new KeyValuePair<string, List<string>>(folderMusic + "/Dizzy - Bleachers.mp3", new List<string>());
            //ListItemsPath = new List<KeyValuePair<string, List<string>>>();
            //ListItemsPath.Add(songKeys);
            //KeyValuePair<string, string> songKeys = new KeyValuePair<string, string>("love_the_one.mp3", songPath);
            //ListItemsPath.Add(songKeys);

            //ListItemSong song = new ListItemSong("Dizzy - Bleachers", "Dizzy", "Album");
            //KeyValuePair<string, ListItemSong> songItem = new KeyValuePair<string, ListItemSong>("love_the_one", song);
            //ListItemsRecycler.Add(songItem);


            mediaPlayer = new MediaPlayer();
            mediaPlayer.Completion += OnSongFinish;

            btnPrev.Click += PlaySongPrev;
            btnNext.Click += PlaySongNext;
            btnPlay.Click += PlaySongPlay;

            ScrollPictures = new MH_Scroll(scrHorizon);
            ScrollPictures.SCROLL_INTERVAL = 200;
            ScrollPictures.SCROLL_DELTA = 20;
            ScrollPictures.SCROLL_END_POINT = 2300;      // (imgSongArtist1.Width * 3) - 500;
            //ScrollPictures.OnScrolling += ScrollPictures_OnScrolling;

            ScrollSongName = new MH_Scroll(scrHorizonSongName);
            ScrollSongName.SCROLL_INTERVAL = 200;
            ScrollSongName.SCROLL_DELTA = 8;
            ScrollSongName.SCROLL_END_POINT = 220;      // (imgSongArtist1.Width * 3) - 500;
            //ScrollSongName.OnScrolling += ScrollSongName_OnScrolling;

            isPlayingNow = false;
            CurrentSongPosition = 0;
            IsTimerWork = false;
            IsHaveToScroll = true;
            IsFirstPlay = true;
        }

        private void ScrollPictures_OnScrolling(int scrollPossion)
        {
            
        }

        private void OnSeekbarChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            //UpdateProgressControls();

            CurrentSongPosition = e.Progress;

            // Play next song. Bug in Complte SeekBar control
            if (barSeek.Max - e.Progress < 1001)
            {
                PlaySongNext(sender, null);
            }

            if (e.FromUser)
            {
                mediaPlayer.SeekTo(CurrentSongPosition);
            }

        }

        private void OnSongFinish(object sender, EventArgs eventArgs)
        {
            if (IsFirstPlay)
            {
                IsFirstPlay = false;
                return;
            }

            PlaySongNext(sender, eventArgs);
        }

        private void PlaySongPlay(object sender, EventArgs eventArgs)
        {
            if (!isPlayingNow)
            {
                if (barSeek.Progress == 0)
                {
                    LoadSongIntoPlayer(ListPositionIndex);
                }
                else
                {
                    MusicPlay();
                    mediaPlayer.SeekTo(CurrentSongPosition);
                }
                
                IsHaveToScroll = !IsHaveToScroll;
            }
            else
            {
                isPlayingNow = false;
                MusicPause();
            }

        }

        private void PlaySongNext(object sender, EventArgs eventArgs)
        {
            MusicPause();

            if (ListPositionIndex < ListItemsRecycler.Count)
            {
                ListPositionIndex++;

                if (isPlayingNow)
                {
                    LoadSongIntoPlayer(ListPositionIndex);
                }
                else
                {
                    // Set the Song props - Name, Artist, Album, Duration
                    SetSongControls(ListPositionIndex);
                    CurrentSongPosition = 0;
                }
            }

            //seekMusic(5000);
        }

        private void PlaySongPrev(object sender, EventArgs eventArgs)
        {
            if (ListPositionIndex < ListItemsRecycler.Count && ListPositionIndex > 0)
            {
                ListPositionIndex--;
                if (isPlayingNow)
                {
                    LoadSongIntoPlayer(ListPositionIndex);
                }
                else
                {
                    // Set the Song props - Name, Artist, Album, Duration
                    SetSongControls(ListPositionIndex);
                    CurrentSongPosition = 0;
                }
            }

            //seekMusic(-5000);
        }

        private void LoadSongIntoPlayer(int listPositionIndex)
        {

            try
            {
                MusicPause();

                if (ListPositionIndex >= ListItemsRecycler.Count)
                {
                    return;
                }
                
               
                //int resourceID = Resource.Raw.love_the_one;
                //int resourceID = ListItemsRecycler.get(listPositionIndex).getResourceID();
                //string folderNameMusic = Android.OS.Environment.DirectoryMusic;
                //string folderMusic = Android.OS.Environment.GetExternalStoragePublicDirectory(folderNameMusic).AbsolutePath;
                //string songPath = folderMusic + "/Brad.mp3";
                
                string songPath= ListItemsPath[listPositionIndex].Key;
                
                Android.Net.Uri uri = Android.Net.Uri.Parse(songPath);

                mediaPlayer = MediaPlayer.Create(this, uri);
                //mediaPlayer = MediaPlayer.Create(this, resourceID); 

                mediaPlayer.SetScreenOnWhilePlaying(true);

                CurrentSongPosition = 0;
                mediaPlayer.SeekTo(0);
                barSeek.Max = mediaPlayer.Duration;
                barSeek.SetProgress(0, false);

                ListItemSong listItemSong = ListItemsRecycler[listPositionIndex].Value;
                listItemSong.setDuration(mediaPlayer.Duration);

                // Set the Song props - Name, Artist, Album, Duration
                SetSongControls(listPositionIndex);

                MusicPlay();

            }
            catch (Exception ex)
            {
                Utils.Utils.WriteToLog("Error while load a song into Media player.. \n" + ex.Message);
            }



        }

        public void MusicPlay()
        {
            if (mediaPlayer != null)
            {
                mediaPlayer.Start();

                isPlayingNow = true;

                btnPlay.SetBackgroundResource(Android.Resource.Drawable.IcMediaPause);

                ScrollPictures.Start();
                if (lblSongName.Text.Length > 30)
                {
                    ScrollSongName.Start();
                }

                //setPicsScroll();

                UpdateSongPositionThread();
            }
        }

        private void MusicPause()
        {

            ScrollPictures.Stop();
            ScrollSongName.Stop();

            if (ThreadTask != null)
            {
                ThreadTask.Abort();
            }
            ThreadTask = null;

            if (mediaPlayer != null)
            {
                mediaPlayer.Pause();

                //mediaPlayer.Stop();
                btnPlay.SetBackgroundResource(Android.Resource.Drawable.IcMediaPlay);
            }
        }

        public void MusicStop()
        {

            ScrollPictures.Stop();
            ScrollSongName.Stop();

            if (ThreadTask != null)
            {
                ThreadTask.Abort();
            }
            ThreadTask = null;

            if (mediaPlayer != null)
            {
                mediaPlayer.Stop();
                mediaPlayer.Release();
            }

            mediaPlayer = null;
        }

        private void seekMusic(int interval)
        {
            barSeek.Progress = barSeek.Progress + interval;
            mediaPlayer.SeekTo(barSeek.Progress);
        }

        /// <summary>
        /// Set the Song props - Name, Artist, Album, Duration
        /// </summary>
        /// <param name="listPositionIndex"></param>
        private void SetSongControls(int listPositionIndex)
        {
            Drawable drawable;
            Bitmap bitmap;


            ListItemSong item = ListItemsRecycler[listPositionIndex].Value;

            lblSongName.Text = item.getSongName();
            ScrollSongName.SCROLL_END_POINT = lblSongName.Text.Length * 21;    // 220;  // lblSongName.Width - 200;
            lblSongArtist.Text = item.getArtist();
            lblAlbum.Text = item.getAlbum();

            barSeek.SetProgress(0, false);
            CurrentSongPosition = 0;

            UpdateProgressControls();

            

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
                lblPosEnd.Text = dateFormat.Format(new Date(duration));
            }
            catch (Exception ex)
            {

            }

            //lblPosNow.Text = new DateTime(currentPos).ToString("mm:ss");
            //lblPosEnd.Text = new DateTime(duration).ToString("mm:ss");
        }

        public void UpdateSongPositionThread()
        {

            //ActionOnPlayingMusic += OnPlayingMusic;

            ThreadTask = new Thread(new ThreadStart(OnThreadRunning));

            //var second = new Thread(new ThreadStart(secondThread));
            //second.Start();
            //btnStart.TouchUpInside += delegate {

            ThreadTask.Start();

        }

        private void OnThreadRunning()
        {
            while (mediaPlayer != null && mediaPlayer.IsPlaying)
            {
                Thread.Sleep(1000);

                RunOnUiThread(OnPlayingMusic);      // ActionOnPlayingMusic);    // =>
                //{
                    //int newPosition = mediaPlayer.CurrentPosition;
                    //barSeek.Progress = newPosition;
                //};

                UpdateProgressControls();
            }

            if (ThreadTask != null)
            {
                ThreadTask.Abort();
            }
            ThreadTask = null;


        }

        protected void OnPlayingMusic()
        {
            try
            {
                barSeek.Progress = mediaPlayer.CurrentPosition;
                CurrentSongPosition = mediaPlayer.CurrentPosition;
            }
            catch (Exception ex)
            {
                Utils.Utils.WriteToLog(ex.Message);
            }
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
            MusicStop();

            Toast.MakeText(this, "Destroing Media Player control", ToastLength.Long).Show();

            base.OnDestroy();
        }


    }

}
