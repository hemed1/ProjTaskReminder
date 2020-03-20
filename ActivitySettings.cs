using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;

namespace ProjTaskReminder
{

    [Activity(Label = "ActivitySettings", Theme = "@style/AppTheme.NoActionBar")]
    public class ActivitySettings : AppCompatActivity
    {
        private EditText txtSettingMusicPath;
        private EditText txtSettingNewsUrl;
        private EditText txtSettingWeatherUrl;
        private Button btnSettingSave;
        private Button btnSettingCancel;


        public static Context context;
        private Intent inputIntent;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_setting);

            // Create your application here
            inputIntent = this.Intent;

            SetControlsIO();

            if (!string.IsNullOrEmpty(inputIntent.GetStringExtra("MusicPath")))
            {
                txtSettingMusicPath.Text = inputIntent.GetStringExtra("MusicPath");
            }
            if (!string.IsNullOrEmpty(inputIntent.GetStringExtra("NewsUrl")))
            {
                txtSettingNewsUrl.Text = inputIntent.GetStringExtra("NewsUrl");
            }
            if (!string.IsNullOrEmpty(inputIntent.GetStringExtra("WeatherUrl")))
            {
                txtSettingWeatherUrl.Text = inputIntent.GetStringExtra("WeatherUrl");
            }


        }

        private void SetControlsIO()
        {
            txtSettingMusicPath = (EditText)FindViewById(Resource.Id.txtSettingMusicPath);
            txtSettingNewsUrl = (EditText)FindViewById(Resource.Id.txtSettingNewsUrl);
            txtSettingWeatherUrl = (EditText)FindViewById(Resource.Id.txtSettingWeatherUrl);
            btnSettingSave = (Button)FindViewById(Resource.Id.btnSettingSave);
            btnSettingCancel = (Button)FindViewById(Resource.Id.btnSettingCancel);

            btnSettingSave.Click += btnSettingSave_click;
            btnSettingCancel.Click += btnSettingCancel_click;


        }

        private void btnSettingCancel_click(object sender, EventArgs e)
        {
            SetResult(Result.Canceled, inputIntent);
            Finish();
        }

        private void btnSettingSave_click(object sender, EventArgs e)
        {
            inputIntent.PutExtra("MusicPath", txtSettingMusicPath.Text);
            inputIntent.PutExtra("NewsUrl", txtSettingNewsUrl.Text);
            inputIntent.PutExtra("WeatherUrl", txtSettingWeatherUrl.Text);

            SetResult(Result.Ok, inputIntent);
            Finish();
        }
    }
}