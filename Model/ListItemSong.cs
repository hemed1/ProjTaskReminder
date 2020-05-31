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
using static Android.Resource;

namespace ProjTaskReminder.Model
{
    public class ListItemSong
    {
        private string SongName;
        private string Artist;
        private string Album;
        private string Year;
        private string Duration;
        private int ResourceID;
        private string SongPath;
        private List<Integer> picsToSongResourcesIDs;
        private List<string> picsToSongPathsArray;

        private ImageView ImageItem;



        public ListItemSong()
        {

        }

        public ListItemSong(string songName, string artist, string album)
        {
            this.SongName = songName;
            this.Artist = artist;
            this.Album = album;
            //ImageItem = new ImageView(ActivityMusic.this);
            picsToSongResourcesIDs = new List<Integer>();
            picsToSongPathsArray = new List<string>();
            //ImageItem = new ImageView(this);
        }

        public List<Integer> getPicsToSongResIDsArray()
        {
            return picsToSongResourcesIDs;
        }

        public List<string> getPicsToSongPathsArray()
        {
            return picsToSongPathsArray;
        }

        public string getDuration()
        {
            Duration = (Duration==null) ? string.Empty : Duration.Trim();

            return Duration;
        }

        public void setDuration(string duration)
        {
            Duration = duration;
        }

        public string getSongPath()
        {
            return SongPath;
        }

        public void setSongPath(string songPath)
        {
            SongPath = songPath;
        }

        public int getResourceID()
        {
            return ResourceID;
        }

        public void setResourceID(int resourceID)
        {
            this.ResourceID = resourceID;
        }

        public string getSongName()
        {
            return SongName;
        }

        public void setSongName(string songName)
        {
            SongName = songName;
        }

        public string getArtist()
        {
            return Artist;
        }

        public void setArtist(string artist)
        {
            Artist = artist;
        }

        public string getAlbum()
        {
            return Album;
        }

        public void setAlbum(string album)
        {
            Album = album;
        }

        public string getYear()
        {
            return Year;
        }

        public void setYear(string year)
        {
            Year = year;
        }

        public ImageView getImageItem()
        {
            return ImageItem;
        }

        public void setImageItem(ImageView imageItem)
        {
            ImageItem = imageItem;
        }

        public void setImagePicture(Drawable drawable)
        {
            //ImageItem.SetImageDrawable(drawable);
            //ImageItem.setBackground(drawable);
        }


    }

}