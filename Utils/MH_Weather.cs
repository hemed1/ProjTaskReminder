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
using Android.Webkit;
using System.Net.WebSockets;
using Org.Json;
using ProjTaskReminder.Model;
using System.Net;
using System.IO;
using System.Timers;
using System.Threading;
using MH_Utils;
//using Newtonsoft

namespace ProjTaskReminder.Utils
{
    public class MH_Weather
    {

        public static string URL_LEFT_MOVIES = "http://www.omdbapi.com/?t=t&y=";  //2001
        public static string URL_RIGHT_MOVIES = "&apikey=c8c455ad";
        //public static string URL_LEFT_WEATHER  = "https://api.apixu.com/v1/current.json?key={0}&q={1}";        //bd7acb3eb7fd424fbdd105519181908&q=";
        public static string URL_LEFT_WEATHER = "http://api.weatherstack.com/current?access_key={0}&query={1}";   // f2896ef52242c1e367e2170ce40352ba
        public static string URL_RIGHT_WEATHER = "";  //"&page=2";
        public static string URL_WEATHER_KEY_ID = "f2896ef52242c1e367e2170ce40352ba";  //"&page=2";



        //http://api.weatherstack.com/current
        //  ? access_key = YOUR_ACCESS_KEY
        //  & query = New York
        //
        // optional parameters:
        //  & units = m
        //  & language = en
        //  & callback = MY_CALLBACK


        public int WEATER_CHANE_PLACE_TIMER_INTERVAL { get; set; }
        private System.Timers.Timer timerChangePlace;
        public event Action<object, int> OnChanePlace;
        public event Action<List<Weather>> OnCompleteLoadData;
        public int currentListIndex;
        public bool IsChangePlaces { get; set; }
        public Activity activity {get; set;}

        private bool isSiteWasFound;
        public List<Weather> WeatherList;
        private Thread ThreadTask;


        public MH_Weather()
        {
            WeatherList = new List<Weather>();
            WEATER_CHANE_PLACE_TIMER_INTERVAL = 60000;
            currentListIndex = 0;
        }

        public void GetAllWeaters()
        {

            ThreadTask = new Thread(new ThreadStart(StartGetAllPlaces));
            ThreadTask.IsBackground = true;

            ThreadTask.Start();
           
        }

        private void StartGetAllPlaces()
        {
            activity.RunOnUiThread(OnGetAlWeaters);

            //{
            //int newPosition = mediaPlayer.CurrentPosition;
            //barSeek.Progress = newPosition;
            //};
        }

        private void OnGetAlWeaters()
        {
            Weather weather;


            weather = GetWather("Ashdod");
            currentListIndex = 0;
            if (weather!=null && OnChanePlace != null && WeatherList.Count>0 && currentListIndex < WeatherList.Count)
            {
                OnChanePlace(WeatherList[currentListIndex], currentListIndex);
            }

            weather = GetWather("Tel Aviv");
            currentListIndex = 1;
            if (weather != null && OnChanePlace != null && WeatherList.Count > 0 && currentListIndex < WeatherList.Count)
            {
                OnChanePlace(WeatherList[currentListIndex], currentListIndex);
            }

            weather = GetWather("Jerusalem");
            currentListIndex = 2;
            if (weather != null && OnChanePlace != null && WeatherList.Count > 0 && currentListIndex < WeatherList.Count)
            {
                OnChanePlace(WeatherList[currentListIndex], currentListIndex);
            }

            weather = GetWather("Gedera");
            currentListIndex = 3;
            if (weather != null && OnChanePlace != null && WeatherList.Count > 0 && currentListIndex < WeatherList.Count)
            {
                OnChanePlace(WeatherList[currentListIndex], currentListIndex);
            }

            ThreadTask.Abort();
            ThreadTask = null;

            currentListIndex = -1;

            if (WeatherList != null && WeatherList.Count > 0 && OnCompleteLoadData !=null)
            {
                OnCompleteLoadData(this.WeatherList);
            }
        }

        public void StartChangePlace()
        {
            currentListIndex = -1;

            //timerChangePlace = new System.Timers.Timer();
            //timerChangePlace.Interval = WEATER_CHANE_PLACE_TIMER_INTERVAL;
            //timerChangePlace.AutoReset = true;
            //timerChangePlace.Elapsed += Timer_onTick;
            //timerChangePlace.Enabled = true;

            IsChangePlaces = true;

            GetAllWeaters();
        }

        public void StotChangePlace()
        {
            currentListIndex = 0;

            //timerChangePlace.Stop();
            //timerChangePlace.Close();
            //timerChangePlace.Dispose();
            //timerChangePlace = null;
            //timerScroll.Start();

            IsChangePlaces = false;

            if (ThreadTask != null && ThreadTask.IsAlive)
            {
                ThreadTask.Abort();
                ThreadTask = null;
            }

            // Show first city
            if (WeatherList!= null && WeatherList.Count > 0 && OnChanePlace != null && currentListIndex < WeatherList.Count)
            {
                OnChanePlace(WeatherList[currentListIndex], currentListIndex);
            }

            currentListIndex = -1;
        }

        private void Timer_onTick(object sender, ElapsedEventArgs e)
        {
            currentListIndex++;

            if (currentListIndex>-1 && currentListIndex> WeatherList.Count-1)
            {
                currentListIndex = 0;
            }
            // Refreh current weather
            GetWather(WeatherList[currentListIndex].getCity());

            if (OnChanePlace != null)
            {
                OnChanePlace(WeatherList[currentListIndex], currentListIndex);
            }

        }

        public Weather GetWather(string searchTerm)
        {
            string responseObj = "";
            Uri url=null;
            Weather weather = null;


            isSiteWasFound = false;
            

            try
            {
                string urlAddress = string.Format(URL_LEFT_WEATHER, URL_WEATHER_KEY_ID, searchTerm);        // + URL_RIGHT_WEATHER
                
                url = new Uri(urlAddress);

                // Specify the URL to receive the request.
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                // Set some reasonable limits on resources used by this request
                request.MaximumAutomaticRedirections = 4;
                request.MaximumResponseHeadersLength = 4;
                // Set credentials to use for this request.
                request.Credentials = CredentialCache.DefaultCredentials;
                
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                //Console.WriteLine("Content length is {0}", response.ContentLength);
                //Console.WriteLine("Content type is {0}", response.ContentType);

                // Get the stream associated with the response.
                Stream receiveStream = response.GetResponseStream();

                // Pipes the stream to a higher level stream reader with the required encoding format. 
                StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);

                isSiteWasFound = true;

                responseObj = readStream.ReadToEnd();

                response.Close();
                readStream.Close();

                //{ "request":{ "type":"City","query":"Tel Aviv-Yafo, Israel","language":"en","unit":"m"},
                //"location":{ "name":"Tel Aviv-Yafo","country":"Israel","region":"Tel Aviv","lat":"32.068","lon":"34.765",
                //"timezone_id":"Asia\/Jerusalem","localtime":"2020-03-02 19:30","localtime_epoch":1583177400,"utc_offset":"2.0"},
                //"current":{ "observation_time":"05:30 PM","temperature":17,"weather_code":116,"weather_icons":["https:\/\/assets.weatherstack.com\/images\/wsymbols01_png_64\/wsymbol_0004_black_low_cloud.png"],
                //"weather_descriptions":["Partly cloudy"],"wind_speed":10,"wind_degree":311,"wind_dir":"NW","pressure":1021,"precip":0,"humidity":60,"cloudcover":7,"feelslike":17,"uv_index":1,"visibility":10,"is_day":"no"}}

                isSiteWasFound = true;

                weather = CallbackResponse(responseObj);

            }
            catch (Exception ex)
            {
                MH_Utils.Utils.WriteToLog("Can't reach site: " + url.AbsoluteUri + "\n" + "Error: " + ex.Message);
            }




            //weatherRecyclerViewAdpter = new WeatherRecyclerViewAdpter(getApplicationContext(), weatherList);
            //listRecycler.setAdapter(weatherRecyclerViewAdpter);
            //weatherRecyclerViewAdpter.notifyDataSetChanged();  // Very Important !!! Otherwise we wont see anything displayed


            //System.Net.WebRequest webRequest = System.Net.WebRequest.Create(url);
            //webRequest.Method = "GET";
            //object state=1;
            //var response = await webRequest.GetResponse();
            //System.Net.WebResponse response = webRequest.GetResponse();  //.BeginGetResponse(CallbackResponse, state);
            //Android.Net.NetworkRequest networkRequest = new Android.Net.NetworkRequest();


            return weather;
        }

        private Weather CallbackResponse(string responseObj)
        {
            Weather weather = null;
            

            Org.Json.JSONObject weatherMain = new Org.Json.JSONObject(responseObj);

            Org.Json.JSONArray weatherArray = new Org.Json.JSONArray();
            weatherArray.Put(weatherMain);


            for (int i = 0; i < weatherArray.Length(); i++)
            {
                try
                {
                    Org.Json.JSONObject weatherObject = weatherArray.GetJSONObject(i);

                    JSONObject objectLocation = weatherObject.GetJSONObject("location");
                    JSONObject objectCurrent = weatherObject.GetJSONObject("current");

                    //JSONObject objectrequest = objectCurrent.GetJSONObject("request");
                    //"request":{ "type":"City","query":"Tel Aviv-Yafo, Israel","language":"en","unit":"m"}

                    weather = new Weather();

                    weather.setTemperature(objectCurrent.GetString("temperature"));
                    weather.setDescription(objectCurrent.GetString("weather_descriptions"));
                    weather.setWind_kph(objectCurrent.GetString("wind_speed"));
                    string icon = objectCurrent.GetString("weather_icons");
                    icon = icon.Replace("[\"" + "https:\\/\\/assets", @"https://assets");
                    icon = icon.Replace("[", "");
                    icon = icon.Replace("]", "");
                    icon = icon.Replace("\"", "");
                    icon = icon.Replace(@"\/", "//");
                    icon = icon.Replace(@"\\", "//");
                    //Android.Net.Uri uri = Android.Net.Uri.Parse(icon);
                    //Android.Net.Uri uri = Android.Net.Uri.Parse(icon);
                    //icon = uri.Path;
                    //icon = uri.AbsolutePath;
                    weather.setPoster(icon);
                    weather.setIs_day(objectCurrent.GetString("is_day").ToString());
                    weather.setCloud(objectCurrent.GetString("cloudcover"));

                    weather.setLast_update(objectLocation.GetString("localtime"));
                    weather.setCountry(objectLocation.GetString("country"));
                    icon = objectLocation.GetString("name").Trim();
                    icon = icon.Replace(@"[", "");
                    icon = icon.Replace("[", "");
                    icon = icon.Replace(@"]", "");
                    icon = icon.Replace("]", "");
                    icon = icon.Replace("\"", "");
                    icon = icon.Replace("\"", "");
                    icon = icon.Replace("[\"", "");
                    //icon = icon.Replace('\'', (char)32); 
                    weather.setCity(icon.Trim());         // "type":"City","query":"Tel Aviv-Yafo, Israel"
                    weather.setRegion(objectLocation.GetString("region"));
                    weather.setLocal_time(objectLocation.GetString("localtime"));
                    weather.setTz_id(objectLocation.GetString("timezone_id"));

                    //"localtime_epoch":1583177400,"utc_offset":"2.0"
                    //wind_degree":311,"wind_dir":"NW","pressure":1021,"precip":0,"humidity":60,"cloudcover":7,"feelslike":17

                    try
                    {
                        //weather.setImageView(Utils.GetImageViewFromhUrl(weather.getPoster()));
                        //weather.setImageView(new ImageView(Application.Context));
                        //Android.Net.Uri uri = Android.Net.Uri.Parse(weather.getPoster());
                        //weather.getImageView().SetImageURI(uri);
                    }
                    catch
                    {

                    }

                    if (WeatherList.Count < 4)
                    {
                        WeatherList.Add(weather);
                    }
                    else
                    {
                        WeatherList[currentListIndex] = weather;
                    }
                }
                catch (JSONException ex)
                {
                    MH_Utils.Utils.WriteToLog(ex.Message);
                    //Log.d("Error: ", ex.getMessage());
                    //ex.printStackTrace();
                }
            }


            return weather;

        }



        //new Response.ErrorListener()
        //                {
        //                    @Override
        //                    public void onErrorResponse(VolleyError error)
        //        {
        //            if (error != null)
        //            {
        //                isSiteWasFound = false;
        //                string message = "לא מצא נתונים מתאימים";               //error.getMessage();
        //                                                                        //Log.d("app", error.getMessage());
        //                View view = getLayoutInflater().inflate(R.layout.activity_main, null);
        //                Snackbar.make(view, message, Snackbar.LENGTH_LONG).setAction("Action", null).show();
        //            }
        //        }
        //    });


        //        if (isSiteWasFound)
        //        {
        //queue.add(jsonObjectRequest);
        //        }


        //public async Task Login(string url)
        //{
        //    try
        //    {
        //        var uri = new Uri(url);

        //        HttpClient myClient = new HttpClient();

        //        var response = await myClient.GetAsync(uri);
        //        if (response.IsSuccessStatusCode)
        //        {
        //            var content = await response.Content.ReadAsStringAsync();
        //            var Item = JsonConvert.DeserializeObject<UserDetails>(content);
        //            string userid = Item.UserID;

        //            int roleid = Item.RoleID;
        //        }
        //        else
        //        {
        //            Application.Current.Properties["response"] = response;
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine(ex);
        //    }
        //}       
    }


}