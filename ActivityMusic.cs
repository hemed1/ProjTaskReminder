using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
using ProjTaskReminder.Model;

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


        public static Context context;
        private List<KeyValuePair<string, string>>          ListItemsPath;
        private List<KeyValuePair<string, ListItemSong>>    ListItemsRecycler;
        private int ListPositionIndex;
        private bool isPlayingNow;




        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_music);

            SetControlsIO();
        }

        private void SetControlsIO()
        {
            btnPrev = (Button)FindViewById(Resource.Id.btnPrev);
            btnNext = (Button)FindViewById(Resource.Id.btnNext);
            btnPlay = (Button)FindViewById(Resource.Id.btnPlay);
            //mediaPlayer = (MediaPlayer)FindViewById(Resource.Id.mediaControllerMain);
            barSeek = (SeekBar)FindViewById(Resource.Id.barSeek);

            barSeek.BringToFront();

            lblSongName = (TextView)FindViewById(Resource.Id.lblSongName);
            lblSongArtist = (TextView)FindViewById(Resource.Id.lblSongArtist);
            lblAlbum = (TextView)FindViewById(Resource.Id.lblAlbum);
            lblPosNow = (TextView)FindViewById(Resource.Id.lblPosNow);
            lblPosLeft = (TextView)FindViewById(Resource.Id.lblPosLeft);


            ListPositionIndex = -1;
            ListItemsRecycler = new List<KeyValuePair<string, ListItemSong>>();

            string folderNameMusic = Android.OS.Environment.DirectoryMusic;
            //Java.IO.File externalPath = Android.OS.Environment.ExternalStorageDirectory;
            //string externalPathFile = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            string folderMusic = Android.OS.Environment.GetExternalStoragePublicDirectory(folderNameMusic).AbsolutePath;
            //LOG_FILE_PATH = externalPath + "/ProjTaskReminder";    //, FileCreationMode.Append).AbsolutePath;
            string songPath = folderMusic + "/love_the_one.mp3";
            KeyValuePair<string, string> songKeys = new KeyValuePair<string, string>("love_the_one.mp3", songPath);
            ListItemsPath.Add(songKeys);

            ListItemSong song = new ListItemSong("love_the_one", "Neil Young", "Album");
            //song.setSongName("love_the_one");
            //song.setArtist("Neil Young");
            //song.setAlbum("Album");
            KeyValuePair<string, ListItemSong> songItem = new KeyValuePair<string, ListItemSong>("love_the_one", song);
            ListItemsRecycler.Add(songItem);


            mediaPlayer = new MediaPlayer();

            btnPrev.Click += PlaySongPrev;
            btnNext.Click += PlaySongNext;
            btnPlay.Click += PlaySongPlay;

            isPlayingNow = false;
        }

        private void PlaySongPlay(object sender, EventArgs eventArgs)
        {
            if (!isPlayingNow && ListPositionIndex < ListItemsRecycler.Count - 1)
            {
                isPlayingNow = true;
                LoadSongIntoPlayer(ListPositionIndex);
            }
            else
            {
                isPlayingNow = false;
                MusicPause();
            }

            //seekMusic(5000);
        }

        private void PlaySongNext(object sender, EventArgs eventArgs)
        //private void PlaySongNext(object sender, AdapterView.ItemClickEventArgs e)
        {
            if (ListPositionIndex < ListItemsRecycler.Count - 1)
            {
                ListPositionIndex++;
                LoadSongIntoPlayer(ListPositionIndex);
            }

            //seekMusic(5000);
        }

        private void PlaySongPrev(object sender, EventArgs eventArgs)
        //private void PlaySongPrev(object sender, AdapterView.ItemClickEventArgs e)
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

                //songPath = this.Resources.GetLayout(Resource.Raw.love_the_one).Value;
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

                MusicPlay();
            }
            catch (Exception ex)
            {
                //Println("Error while load a song into Media player.. \n" + e.getMessage());
            }

            // Set the Song props - Name, Artist, Album, Duration
            //setSongControls(listPositionIndex);

            

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
                btnPlay.SetBackgroundResource(GetDrawable(ic_media_play));
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

        private void seekMusic(int interval)
        {
            barSeek.Progress = barSeek.Progress + interval;
            mediaPlayer.SeekTo(barSeek.Progress);
        }

        // Set the Song props - Name, Artist, Album, Duration
        private void setSongControls(int listPositionIndex)
        {
            Drawable drawable;
            Bitmap bitmap;


            ListItemSong item = ListItemsRecycler[listPositionIndex].Value;

            lblSongName.Text=item.getSongName();
            lblSongArtist.Text=item.getArtist();
            lblAlbum.Text=item.getAlbum();

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

        private void updateProgressControls()
        {
            int currentPos = mediaPlayer.CurrentPosition;
            int duration = mediaPlayer.Duration;

            SimpleDateFormat dateFormat = new SimpleDateFormat("mm:ss");
            lblPosNow.Text=dateFormat.Format(new Java.Util.Date(currentPos));
            lblPosLeft.Text=dateFormat.Format(new Java.Util.Date(duration));
            //lblPosLeft.setText(dateFormat.format(new Date(duration - currentPos)));
        }

    }

}
