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
using ProjTaskRemindet.Utils;



namespace ProjTaskReminder
{
    [Activity(Label = "ActivityMusic", Theme = "@style/AppTheme.NoActionBar")]
    public class ActivityMusic : AppCompatActivity
    {
        private Button btnPrev;
        private Button btnNext;
        private Button btnPlay;
        public  static MediaPlayer mediaPlayer;
        private SeekBar barSeek;
        private SeekBar barVolume;
        private TextView lblSongName;
        private TextView lblSongArtist;
        private TextView lblAlbum;
        private TextView lblPosNow;
        private TextView lblPosEnd;
        private ImageView imgSongArtist1;
        private ImageView imgSongArtist2;
        private ImageView imgSongArtist3;
        private ListView lstFiles;
        private Android.Support.V7.Widget.CardView cardFilesList;
        private RelativeLayout layFolderList;
        private TextView lblSongsListCaption;
        private TimerTask TimerTaskSongProgress;
        private System.Timers.Timer TimerSongProgress;
        private TimerTask picsTimerTask;
        private System.Timers.Timer picsTimer;
        private bool IsTimerWork;
        private bool IsHaveToScroll;
        private bool IsFirstPlay;
        private bool IsFolderButtonPressed;
        private int keepX;
        private int PICS_TIMER_INTERVAL = 200;
        private int PICS_TIMER_SCROLL_DELTA = 5;
        private int PICS_TIMER_SCROLL_END_POINT;
        private int SONG_NAME_CHARS_LENGTH_TO_SCROLL = 32;
        private HorizontalScrollView scrHorizonPics;
        private HorizontalScrollView scrHorizonSongName;
        private event Action ActionOnPicsScrolling;
        private MH_Scroll ScrollPictures;
        private MH_Scroll ScrollSongName;


        public static Context context;
        public static string MUSIC_PATH = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryMusic).AbsolutePath;          // "/storage/emulated/0/Music";
        private static List<KeyValuePair<string, List<string>>> ListItemsPath;
        private static List<KeyValuePair<string, ListItemSong>> ListItemsRecycler;
        private static List<KeyValuePair<string, ListItemSong>> ListItemsRecyclerBackup;
        private  List<ListItemSong> SongFoldersList;
        //private  List<ListItemSong> SongNamesList;
        ListViewAdapter listViewAdapter;
        private static int ListPositionIndex = 0;
        private static int CurrentSongPosition;
        private bool isPlayingNow;
        private Thread ThreadTask;
        private event Action ActionOnPlayingMusic;
        private int CurrentVolumPosition;



        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_music);


            SetControlsIO();

            SONG_NAME_CHARS_LENGTH_TO_SCROLL = GetCharsCountPerFontSize(lblSongName);

            if (mediaPlayer == null)
            {
                LoadSongsFilesFromPhone();

                ListPositionIndex = 0;
                if (ListItemsRecycler != null && ListItemsRecycler.Count > 0)
                {
                    LoadSongIntoPlayer(ListPositionIndex, false);
                }
            }
            else
            {
                if (mediaPlayer != null && ListItemsRecycler!=null  && ListItemsRecycler.Count > 0 && ListPositionIndex<ListItemsRecycler.Count && ListPositionIndex>-1)
                {
                    int tmpPos = CurrentSongPosition;
                    LoadSongIntoPlayer(ListPositionIndex, mediaPlayer.IsPlaying);
                    CurrentSongPosition = tmpPos;
                    seekMusic(CurrentSongPosition);
                }
            }
            

        }

        private int GetCharsCountPerFontSize(TextView txtControl)
        {
            float textSize = lblSongName.TextSize;

            return (int)((63*32)/textSize); 
        }

        private void LoadSongsFilesFromPhone()
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

            ListItemsRecycler = new List<KeyValuePair<string, ListItemSong>>();
            

            for (int i=0; i<ListItemsPath.Count; i++)
            {
                string fileFullName = ListItemsPath[i].Key;
                string path = Directory.GetParent(fileFullName).FullName;
                string fileName = fileFullName.Substring(path.Length + 1);
                string artist = "Artist " + (i + 1).ToString();
                string album = Directory.GetParent(fileFullName).Name;     // GetFolderNameFromPath(path);


                fileName = Utils.Utils.FixSongName(fileName);
                fileName = fileName.Substring(0, fileName.Length - 4);
                
                int pos = fileName.IndexOf("-");
                if (pos>-1)
                {
                    if (pos < 4)
                    {
                        artist = album;
                    }
                    else
                    {
                        artist = fileName.Substring(0, pos-1);
                    }
                }

                ListItemSong listItemSong = new ListItemSong(fileName, artist, album);
                listItemSong.setSongPath(path);
                listItemSong.setSongPathFull(fileFullName);

                ListItemsRecycler.Add(new KeyValuePair<string, ListItemSong>(fileFullName, listItemSong));
            }

            ListItemsPath = ListItemsPath.OrderBy(a => a.Key).ToList();
            ListItemsRecycler = ListItemsRecycler.OrderBy(a => a.Key).ToList();

            BackupSongList();
        }


        private void SetControlsIO()
        {
            btnPrev = (Button)FindViewById(Resource.Id.btnPrev);
            btnNext = (Button)FindViewById(Resource.Id.btnNext);
            btnPlay = (Button)FindViewById(Resource.Id.btnPlay);
            Button btnFoldersList = (Button)FindViewById(Resource.Id.btnFoldersList);
            Button btnSongsList = (Button)FindViewById(Resource.Id.btnSongsList);
            //mediaPlayer = (MediaPlayer)FindViewById(Resource.Id.mediaControllerMain);

            barSeek = (SeekBar)FindViewById(Resource.Id.barSeek);
            barSeek.BringToFront();
            barSeek.ProgressChanged += barSeek_OnProgressChanged;

            barVolume = (SeekBar)FindViewById(Resource.Id.barVolume);
            barVolume.Min = 0;  // 0.0f;
            barVolume.Max = 100;  // 1.0f
            barVolume.BringToFront();
            barVolume.ProgressChanged += barVolume_OnChanged;

            lblSongName = (TextView)FindViewById(Resource.Id.lblSongName);
            scrHorizonSongName = (HorizontalScrollView)FindViewById(Resource.Id.scrHorizonSongName);
            lblSongArtist = (TextView)FindViewById(Resource.Id.lblSongArtist);
            lblAlbum = (TextView)FindViewById(Resource.Id.lblAlbum);
            lblPosNow = (TextView)FindViewById(Resource.Id.lblPosNow);lblPosNow = (TextView)FindViewById(Resource.Id.lblPosNow);
            lblPosEnd = (TextView)FindViewById(Resource.Id.lblPosEnd);
            lblSongsListCaption = (TextView)FindViewById(Resource.Id.lblSongsListCaption);

            imgSongArtist1 = (ImageView)FindViewById(Resource.Id.imgSongArtist1);
            imgSongArtist2 = (ImageView)FindViewById(Resource.Id.imgSongArtist2);
            imgSongArtist3 = (ImageView)FindViewById(Resource.Id.imgSongArtist3);
            scrHorizonPics = (HorizontalScrollView)FindViewById(Resource.Id.scrHorizonPics);
            PICS_TIMER_SCROLL_END_POINT = 1000;     // (imgSongArtist1.Width * 3) - 500;

            //layFolderList = (RelativeLayout)FindViewById(Resource.Id.layFolderList);
            //layFolderList.Visibility = ViewStates.Invisible;

            cardFilesList = (Android.Support.V7.Widget.CardView)FindViewById(Resource.Id.cardFilesList);
            cardFilesList.Visibility = ViewStates.Invisible;


            lstFiles = (ListView)FindViewById(Resource.Id.lstFiles);
            //lstFiles.ItemClick += ListSongOrFolder_ItemClick;
            lstFiles.FocusedByDefault = true;

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


            //mediaPlayer = new MediaPlayer();
            //mediaPlayer.Completion += OnSongFinish;

            imgSongArtist1.Click += ScrollPictures_OnClick;
            imgSongArtist2.Click += ScrollPictures_OnClick;
            imgSongArtist3.Click += ScrollPictures_OnClick;
            scrHorizonPics.Click += ScrollPictures_OnClick;

            btnPrev.Click += PlaySongPrev;
            btnNext.Click += PlaySongNext;
            btnPlay.Click += OnPlayButton;
            btnFoldersList.Click += btnFoldersList_OnListItem;
            btnSongsList.Click += btnSongsList_OnListItem;

            ScrollPictures = new MH_Scroll(scrHorizonPics);
            ScrollPictures.SCROLL_INTERVAL = 200;
            ScrollPictures.SCROLL_DELTA = 20;
            ScrollPictures.SCROLL_END_POINT = 2300;      // (imgSongArtist1.Width * 3) - 500;
            //ScrollPictures.OnScrolling += ScrollPictures_OnScrolling;

            ScrollSongName = new MH_Scroll(scrHorizonSongName);
            ScrollSongName.SCROLL_INTERVAL = 200;
            ScrollSongName.SCROLL_DELTA = 8;
            ScrollSongName.SCROLL_END_POINT = 220;      // (imgSongArtist1.Width * 3) - 500;
            ScrollSongName.IsScrollRightToLeft = false;
            //ScrollSongName.OnScrolling += ScrollSongName_OnScrolling;

            isPlayingNow = false;
            IsTimerWork = false;
            IsHaveToScroll = true;
            IsFirstPlay = true;

            //ListItemsRecyclerBackup = new List<KeyValuePair<string, ListItemSong>>();
        }



        #region Folders/Songs list handle

        private void btnSongsList_OnListItem(object sender, EventArgs e)
        {
            IsFolderButtonPressed = false;
            lblSongsListCaption.Text = "רשימת כל השירים";

            //RestoreSongsList();

            List<ListItemSong> SongNamesList = GetSongsObjects();

            //layFolderList.Visibility = ViewStates.Invisible;
            cardFilesList.Visibility = ViewStates.Visible;

            ShowSongsListView(SongNamesList);
        }

        private void btnFoldersList_OnListItem(object sender, EventArgs e)
        {
            IsFolderButtonPressed = true;
            lblSongsListCaption.Text = "רשימת תיקיות";

            SongFoldersList = FillSongsFolders();

            //layFolderList.Visibility = ViewStates.Invisible;
            cardFilesList.Visibility = ViewStates.Visible;

            ShowSongsListView(SongFoldersList);
        }

        /// <summary>
        /// On ListView control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListSongOrFolder_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            ChooseFileItem(e.Position);
        }

        private void ListSongOrFolder_ItemClick(ListViewAdapter.ListViewHolder listViewHolder, int position)
        {
            ChooseFileItem(position);
        }

        private void ChooseFileItem(int position)
        {

            List<ListItemSong> SongNamesList;


            if (IsFolderButtonPressed)
            {

                ListPositionIndex = position; 

                RunOnUiThread(ShowSongsListOnIO);


                //RestoreSongsList();

                //string path = SongFoldersList[position].getSongPath();
                //ListItemsRecycler = ListItemsRecycler.Where(a => a.Value.getSongPath() == path).ToList();

                //SongNamesList = GetSongsObjects();

                ////cardFilesList.Visibility = ViewStates.Visible;
                ////layFolderList.Visibility = ViewStates.Visible;
                ////layFolderList.Visibility = ViewStates.Invisible;
                ////cardFilesList.Visibility = ViewStates.Invisible;

                //ShowSongsListView(SongNamesList);

                //IsFolderButtonPressed = false;
                //ListPositionIndex = 0;
            }
            else
            {
                SongNamesList = GetSongsObjects();
                if (position < SongNamesList.Count)
                {
                    string fullSongPath = SongNamesList[position].getSongPathFull();
                    KeyValuePair<string, ListItemSong> songItem = ListItemsRecycler.FirstOrDefault(a => a.Key == fullSongPath);
                    ListPositionIndex = position;       //ListItemsRecycler.IndexOf(songItem);      
                    //ListPositionIndex = GetSongIndexInList(fullSongPath);
                    if (ListPositionIndex == -1)
                    {
                        ListPositionIndex = 0;
                    }
                    LoadSongIntoPlayer(ListPositionIndex, true);
                }

                //layFolderList.Visibility = ViewStates.Invisible;
                cardFilesList.Visibility = ViewStates.Invisible;
            }
            
            
        }

        private void ShowSongsListOnIO()
        {
            List<ListItemSong> SongNamesList;


            RestoreSongsList();

            string path = SongFoldersList[ListPositionIndex].getSongPath();
            ListItemsRecycler = ListItemsRecycler.Where(a => a.Value.getSongPath() == path).ToList();

            SongNamesList = GetSongsObjects();


            //cardFilesList.Visibility = ViewStates.Visible;
            //layFolderList.Visibility = ViewStates.Visible;
            //layFolderList.Visibility = ViewStates.Invisible;
            //cardFilesList.Visibility = ViewStates.Invisible;

            ShowSongsListView(SongNamesList);

            IsFolderButtonPressed = false;
            ListPositionIndex = 0;

        }

        /// <summary>
        /// Fill controls values in card
        /// </summary>
        /// <param name="listViewHolder"></param>
        /// <param name="position"></param>
        private void OnListViewSetControls(ListViewAdapter.ListViewHolder listViewHolder, int position)
        {

            ListItemSong listItemSong = null;

            if (IsFolderButtonPressed)
            {
                listItemSong = SongFoldersList[position];
                string path = GetFolderNameFromPath(listItemSong.getSongPath());
                string fileName = listItemSong.getSongName();

                listViewHolder.FirstLine.SetText(path, TextView.BufferType.Normal);
                listViewHolder.SecondLine.SetText(listItemSong.getArtist(), TextView.BufferType.Normal);
                listViewHolder.ThirdLine.SetText(listItemSong.getSongPath(), TextView.BufferType.Normal);
            }
            else
            {
                KeyValuePair<string, ListItemSong> songItem = ListItemsRecycler[position];
                listItemSong = songItem.Value;

                string songLenght = (listItemSong.getDuration()=="") ? listItemSong.getArtist() : listItemSong.getDuration();
                string songName = listItemSong.getSongName();

                listViewHolder.FirstLine.SetText(songName, TextView.BufferType.Normal);
                listViewHolder.SecondLine.SetText(songLenght, TextView.BufferType.Normal);
                listViewHolder.ThirdLine.SetText(listItemSong.getSongPath(), TextView.BufferType.Normal);
            }
        }


        [Obsolete]
        private void ShowSongsListView(List<ListItemSong> list)
        { 
            //if (cardFilesList.Visibility == ViewStates.Visible)
            //{
            //    //layFolderList.Visibility = ViewStates.Invisible;
            //    cardFilesList.Visibility = ViewStates.Invisible;
            //    return;
            //}

            try
            {
                //RunOnUiThread(ShowSongsList);

                //layFolderList.Visibility = ViewStates.Visible;
                cardFilesList.Visibility = ViewStates.Visible;
                //cardFilesList.BringToFront();
                lstFiles.BringToFront();
                

                listViewAdapter = new ListViewAdapter(context, list);

                listViewAdapter.OnListItemSetControlsInView += OnListViewSetControls;
                listViewAdapter.OnItemClick += ListSongOrFolder_ItemClick;

                lstFiles.SetAdapter(listViewAdapter);
                listViewAdapter.NotifyDataSetChanged();
            }
            catch (Exception ex)
            {

            }
        }


        private List<ListItemSong> FillSongsFolders()
        {
            List<ListItemSong> list = new List<ListItemSong>();


            RestoreSongsList();

            List<string> paths = ListItemsRecycler.Select(a => a.Value.getSongPath()).ToList();
            paths = paths.Distinct().ToList();


            for (int i = 0; i < paths.Count; i++)
            {
                string path = paths[i];
                string parentPathName = GetFolderNameFromPath(path);

                try
                {
                    string songsCount = ListItemsRecycler.Count(a => a.Value.getSongPath() == path).ToString() + " " + "שירים";

                    ListItemSong listItemSong = new ListItemSong();

                    listItemSong.setSongName(parentPathName);       // First line
                    listItemSong.setArtist(songsCount.ToString());  // Second line - Song count
                    listItemSong.setSongPath(path);                 // Third line
                    
                    list.Add(listItemSong);
                }
                catch (Exception ex)
                {
                    string msg = ex.Message;
                }
            }

            return list;
        }

        /// <summary>
        /// Extract the Song objects from main list
        /// </summary>
        /// <returns></returns>
        private List<ListItemSong> GetSongsObjects()
        {
            List<ListItemSong> list = new List<ListItemSong>();


            list = ListItemsRecycler.Select(a => a.Value).ToList();

            list = list.OrderBy(a => a.getSongName()).ToList();

            return list;
        }

        private string GetFolderNameFromPath(string path, string folderSeperator="/")
        {
            if (path.Substring(path.Length - 1) == folderSeperator)
            {
                path = path.Substring(0, path.Length - 1);
            }

            int pos = path.LastIndexOf(folderSeperator);
            path = path.Substring(pos + 1);

            return path;
        }

        #endregion


        private void ScrollPictures_OnClick(object sender, EventArgs e)
        {
            if (ScrollPictures.IsScrollng)
            {
                ScrollPictures.Stop();
            }
            else
            {
                if (isPlayingNow)
                {
                    ScrollPictures.Start();
                }
            }
        }

        private void barSeek_OnProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
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
                seekMusic(CurrentSongPosition);
            }

        }

        private void barVolume_OnChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            if (e.FromUser)
            {
                int diff = (int)((e.Progress - CurrentVolumPosition) / 0.8);
                AudioManager audioManager = (AudioManager)GetSystemService(Context.AudioService);
                int actualVolume = audioManager.GetStreamVolume(Android.Media.Stream.Music);  // AudioManager.STREAM_MUSIC);
                audioManager.SetStreamVolume(Android.Media.Stream.Music, actualVolume + diff, 0);       //   AudioManager.STREAM_MUSIC, AudioManager.FLAG_SHOW_UI);
                //mediaPlayer.SetVolume(0.09f, 0.09f);  // (float)e.Progress, (float)e.Progress) ;           ;      // barVolume.Progress
                CurrentVolumPosition = e.Progress;
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

        private void OnPlayButton(object sender, EventArgs eventArgs)
        {
            if (!isPlayingNow)
            {
                if (barSeek.Progress == 0)
                {
                    LoadSongIntoPlayer(ListPositionIndex, true);
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

            if (ListPositionIndex < ListItemsRecycler.Count-1)
            {
                ListPositionIndex++;

                if (isPlayingNow)   //mediaPlayer.IsPlaying
                {
                    isPlayingNow = false;
                    LoadSongIntoPlayer(ListPositionIndex, true);
                }
                else
                {
                    LoadSongIntoPlayer(ListPositionIndex, false);
                    //// Set the Song props - Name, Artist, Album, Duration
                    //SetSongControls(ListPositionIndex);
                }
            }

            //seekMusic(5000);
        }

        private void PlaySongPrev(object sender, EventArgs eventArgs)
        {
            MusicPause();

            if (ListPositionIndex <= ListItemsRecycler.Count-1 && ListPositionIndex > 0)
            {
                ListPositionIndex--;

                if (isPlayingNow)
                {
                    isPlayingNow = false;
                    LoadSongIntoPlayer(ListPositionIndex, true);
                }
                else
                {
                    LoadSongIntoPlayer(ListPositionIndex, false);
                    //// Set the Song props - Name, Artist, Album, Duration
                    //SetSongControls(ListPositionIndex);
                }
            }

            //seekMusic(-5000);
        }

        private void LoadSongIntoPlayer(int listPositionIndex, bool withPlay = true)
        {

            try
            {
                // Release song sources
                MusicStop();

                if (ListPositionIndex >= ListItemsRecycler.Count || ListPositionIndex<0)
                {
                    return;
                }
                
               
                //int resourceID = Resource.Raw.love_the_one;
                //int resourceID = ListItemsRecycler.get(listPositionIndex).getResourceID();
                //string folderNameMusic = Android.OS.Environment.DirectoryMusic;
                //string folderMusic = Android.OS.Environment.GetExternalStoragePublicDirectory(folderNameMusic).AbsolutePath;
                //string songPath = folderMusic + "/Brad.mp3";
                
                string songPath= ListItemsRecycler[listPositionIndex].Key;
                //string songPath = ListItemsPath[listPositionIndex].Key;

                Android.Net.Uri uri = Android.Net.Uri.Parse(songPath);


                mediaPlayer = MediaPlayer.Create(this, uri);
                //mediaPlayer = MediaPlayer.Create(this, resourceID); 

                if (mediaPlayer != null)
                {
                    //mediaPlayer.SetScreenOnWhilePlaying(true);

                    mediaPlayer.SeekTo(0);
                    barSeek.Max = mediaPlayer.Duration;
                    barSeek.SetProgress(0, false);
                }

                CurrentSongPosition = 0;

                // Set the Song props - Name, Artist, Album, Duration
                SetSongControls(listPositionIndex);

                if (withPlay)
                {
                    MusicPlay();
                }

                //ScrollSongName.StartPosstion();
            }
            catch (Exception ex)
            {
                Utils.Utils.WriteToLog("Error while load a song into Media player.. \n" + ex.Message);
            }



        }

        public void MusicPlay()
        {

            ScrollSongName.Stop();
            ScrollPictures.Stop();

            if (mediaPlayer == null)
            {
                return;
            }

            try
            {
                mediaPlayer.Start();

                isPlayingNow = true;

                btnPlay.SetBackgroundResource(Android.Resource.Drawable.IcMediaPause);

                //ScrollPictures.Start();

                if (lblSongName.Text.Length > SONG_NAME_CHARS_LENGTH_TO_SCROLL)
                {
                    ScrollSongName.Start();
                }

                //setPicsScroll();

                UpdateSongPositionThread();
            }
            catch (Exception ex)
            {
                Utils.Utils.WriteToLog(ex.StackTrace + "\n" + ex.Message);
            }
            
        }

        private void MusicPause()
        {

            ScrollPictures.Stop();
            ScrollSongName.Stop();

            if (ThreadTask != null && ThreadTask.IsAlive)
            {
                ThreadTask.Abort();
            }
            ThreadTask = null;

            if (mediaPlayer != null)    //&& mediaPlayer.IsPlaying
            {
                mediaPlayer.Pause();
                //mediaPlayer.Stop();
            }

            btnPlay.SetBackgroundResource(Android.Resource.Drawable.IcMediaPlay);
        }

        public static bool MusicStopFinal()
        {
            bool result = true;

            if (mediaPlayer != null)
            {
                try
                {
                    mediaPlayer.Stop();
                    mediaPlayer.Release();
                    mediaPlayer = null;
                }
                catch
                {
                    result = false;
                    mediaPlayer = null;
                }
            }

            if (ListItemsPath != null)
            {
                ListItemsPath.Clear();
                ListItemsPath = null;
            }
            if (ListItemsRecycler != null)
            {
                ListItemsRecycler.Clear();
                ListItemsRecycler = null;
            }
            if (ListItemsRecyclerBackup != null)
            {
                //ListItemsRecyclerBackup.Clear();
                //ListItemsRecyclerBackup = null;
            }
            if (Utils.Utils.FilesExtra != null)
            {
                Utils.Utils.FilesExtra.Clear();
                Utils.Utils.FilesExtra = null;
            }

            result = (mediaPlayer == null);

            return result;
        }

        public void MusicStop()
        {

            ScrollPictures.Stop();
            ScrollSongName.Stop();

            if (ThreadTask != null && ThreadTask.IsAlive)
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
            mediaPlayer.SeekTo(interval);
            barSeek.SetProgress(interval, true);      // barSeek.SetProgress(barSeek.Progress + interval);
        }

        /// <summary>
        /// Set the Song props - Name, Artist, Album, Duration
        /// </summary>
        /// <param name="listPositionIndex"></param>
        private void SetSongControls(int listPositionIndex)
        {

            ListItemSong item = ListItemsRecycler[listPositionIndex].Value;

            if (mediaPlayer != null)
            {
                SimpleDateFormat dateFormat = new SimpleDateFormat("mm:ss");
                item.setDuration(dateFormat.Format(new Date(mediaPlayer.Duration)));
            }

            lblSongName.Text = item.getSongName();
            ScrollSongName.SCROLL_END_POINT = lblSongName.Text.Length * 9;
            lblSongArtist.Text = item.getArtist();
            lblAlbum.Text = GetFolderNameFromPath(item.getSongPath());

            //ScrollSongName.StartPosstion();
            

            if (lblSongName.Text.Length > SONG_NAME_CHARS_LENGTH_TO_SCROLL)
            {
                scrHorizonSongName.FullScroll(FocusSearchDirection.Right);
                ScrollSongName.Start();
            }
            else
            {
                scrHorizonSongName.FullScroll(FocusSearchDirection.Right);
            }

            UpdateProgressControls();



            //Drawable drawable;
            //Bitmap bitmap;
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
            ThreadTask = null;

            ThreadTask = new Thread(new ThreadStart(OnThreadRunning));

            //var second = new Thread(new ThreadStart(secondThread));
            //second.Start();
            //btnStart.TouchUpInside += delegate {

            ThreadTask.Start();

        }

        private void OnThreadRunning()
        {
            try
            {
                while (mediaPlayer != null && mediaPlayer.IsPlaying && isPlayingNow)
                {
                    Thread.Sleep(1000);

                    RunOnUiThread(OnPlayingMusic);

                    UpdateProgressControls();
                    
                    //{// ActionOnPlayingMusic);    // =>
                    //int newPosition = mediaPlayer.CurrentPosition;
                    //barSeek.Progress = newPosition;
                    //};
                }

                if (ThreadTask != null)
                {
                    ThreadTask.Abort();
                }
            }
            catch
            {

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

        private void BackupSongList()
        {

            ListItemsRecyclerBackup = new List<KeyValuePair<string, ListItemSong>>();

            for (int i = 0; i < ListItemsRecycler.Count; i++)
            {
                ListItemsRecyclerBackup.Add(ListItemsRecycler[i]);
            }
        }

        private void RestoreSongsList()
        {
            ListItemsRecycler.Clear();

            for (int i = 0; i < ListItemsRecyclerBackup.Count; i++)
            {
                ListItemsRecycler.Add(ListItemsRecyclerBackup[i]);
            }
        }

        private int GetSongIndexInList(string fullSongPath)
        {
            int result = -1;

            for (int i = 0; i < ListItemsRecycler.Count; i++)
            {
                if (ListItemsRecycler[i].Key == fullSongPath)
                //string songPath = ListItemsRecycler[i].Key;
                //if (songPath.Length >= fullSongPath.Length && songPath.Substring(0, fullSongPath.Length) == fullSongPath)
                {
                    result = i;
                    break;
                }
            }

            return result;
        }



        //protected override void OnDestroy()
        //{
        //MusicStop();

        //Toast.MakeText(this, "Destroing Media Player control", ToastLength.Long).Show();

        //base.OnDestroy();
        //}


    }

}
