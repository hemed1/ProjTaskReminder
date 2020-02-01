using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;

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


        public static Context context;
        private List<KeyValuePair<string, string>>          ListItemsRecycler;
        private int ListPositionIndex;





        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_music);

            SetControlsIO();
        }

        private void SetControlsIO()
        {
            //btnPrev = (Button)FindViewById(Resource.Id.btnPrev);
            //btnNext = (Button)FindViewById(Resource.Id.btnNext);
            //btnPlay = (Button)FindViewById(Resource.Id.btnPlay);
            ////mediaPlayer = (MediaPlayer)FindViewById(Resource.Id.mediaControllerMain);
            //barSeek = (SeekBar)FindViewById(Resource.Id.barSeek);

            //barSeek.BringToFront();

            ListPositionIndex = -1;
            ListItemsRecycler = new List<KeyValuePair<string, string>>();

            string folderNameMusic = Android.OS.Environment.DirectoryMusic;
            //Java.IO.File externalPath = Android.OS.Environment.ExternalStorageDirectory;
            //string externalPathFile = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            string folderMusic = Android.OS.Environment.GetExternalStoragePublicDirectory(folderNameMusic).AbsolutePath;
            //LOG_FILE_PATH = externalPath + "/ProjTaskReminder";    //, FileCreationMode.Append).AbsolutePath;
            string songPath = folderMusic + "/love_the_one.mp3";
            KeyValuePair<string, string> songItem = new KeyValuePair<string, string>("love_the_one.mp3", songPath);
            ListItemsRecycler.Add(songItem);

            mediaPlayer = new MediaPlayer();

            //btnPrev.Click += PlaySongPrev;
            //btnNext.Click += PlaySongNext;
        }

        private void PlaySongNext(object sender, AdapterView.ItemClickEventArgs e)
        {
            if (ListPositionIndex < ListItemsRecycler.Count - 1)
            {
                ListPositionIndex++;
                LoadSongIntoPlayer(ListPositionIndex);
            }

            //seekMusic(5000);
        }

        private void PlaySongPrev(object sender, AdapterView.ItemClickEventArgs e)
        {
            if (ListPositionIndex < ListItemsRecycler.Count && ListPositionIndex > 0)
            {
                ListPositionIndex--;
                LoadSongIntoPlayer(ListPositionIndex);
            }
            //seekMusic(-5000);
        }

        private void LoadSongIntoPlayer(int listPositionIndex)
        {

            try
            {
                MusicPause();

                string songPath;

                songPath = this.Resources.GetLayout(Resource.Raw.love_the_one).Value;
                songPath= ListItemsRecycler[listPositionIndex].Value;
                int resourceID = Resource.Raw.love_the_one;

                Android.Net.Uri uri = Android.Net.Uri.Parse(songPath);

                mediaPlayer = MediaPlayer.Create(this, resourceID);   // uri);
                
                mediaPlayer.SetScreenOnWhilePlaying(true);

                if (mediaPlayer != null)
                {
                    mediaPlayer.SeekTo(0);
                    barSeek.Max = mediaPlayer.Duration;
                    barSeek.SetProgress(0, false);
                }
                //mediaPlayer.SetOnCompletionListener(MediaPlayer.IOnCompletionListener());
                //{
                //    @Override
                //    public void onCompletion(MediaPlayer mediaPlayer)
                //    {
                //        PlaySongNext();
                //    }
                //});
                //int resourceID = ListItemsRecycler.get(listPositionIndex).getResourceID();
                //mediaPlayer = MediaPlayer.create(getApplicationContext(), resourceID);  //listItems.get(listPositionIndex).getResourceID()

                //int resourceID = Resource.raw.love_the_one;
                //mediaPlayer = MediaPlayer.Create(this, resourceID);  //listItems.get(listPositionIndex).getResourceID()
            }
            catch (Exception ex)
            {
                //Println("Error while load a song into Media player.. \n" + e.getMessage());
            }

            // Set the Song props - Name, Artist, Album, Duration
            //setSongControls(listPositionIndex);

            MusicPlay();

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

                //btnPlay.SetBackgroundResource(GetDrawable(ic_media_play));
            }

        }

        public void MusicPlay()
        {
            if (mediaPlayer != null)
            {
                mediaPlayer.Start();

                //btnPlay.setBackgroundResource(android.R.drawable.ic_media_pause);

                //setPicsScroll();

                //updateSongPositionThread();
            }
        }



    }

}
