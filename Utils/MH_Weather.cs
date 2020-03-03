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
//using Newtonsoft

namespace ProjTaskReminder.Utils
{
    public class MH_Weather
    {

        public static string URL_LEFT_MOVIES  = "http://www.omdbapi.com/?t=t&y=";  //2001
        public static string URL_RIGHT_MOVIES = "&apikey=c8c455ad";
        //public static string URL_LEFT_WEATHER  = "https://api.apixu.com/v1/current.json?key=bd7acb3eb7fd424fbdd105519181908&q=";
        public static string URL_LEFT_WEATHER  = "http://api.weatherstack.com/current?access_key=f2896ef52242c1e367e2170ce40352ba&query=";
        
        //http://api.weatherstack.com/current
        //  ? access_key = YOUR_ACCESS_KEY
        //  & query = New York
        //
        // optional parameters:
        //  & units = m
        //  & language = en
        //  & callback = MY_CALLBACK

        public static string URL_RIGHT_WEATHER = "";  //"&page=2";

        private bool isSiteWasFound;
        private List<Weather> WeatherList;

        public void MHH_Weather()
        {
            WeatherList = new List<Weather>();
        }


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

        public Weather GetWather(string searchTerm)
        {
            string responseObj = "";
            Uri url=null;
            Weather weather = null;




            isSiteWasFound = false;
            

            try
            {
                url = new Uri(URL_LEFT_WEATHER + searchTerm + URL_RIGHT_WEATHER);

                // Specify the URL to receive the request.
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                // Set some reasonable limits on resources used by this request
                request.MaximumAutomaticRedirections = 4;
                request.MaximumResponseHeadersLength = 4;
                // Set credentials to use for this request.
                request.Credentials = CredentialCache.DefaultCredentials;
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                Console.WriteLine("Content length is {0}", response.ContentLength);
                Console.WriteLine("Content type is {0}", response.ContentType);

                // Get the stream associated with the response.
                Stream receiveStream = response.GetResponseStream();

                // Pipes the stream to a higher level stream reader with the required encoding format. 
                StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);

                //Console.WriteLine("Response stream received.");
                //Console.WriteLine(readStream.ReadToEnd());

                isSiteWasFound = true;

                responseObj = readStream.ReadToEnd();

                response.Close();
                readStream.Close();

                //{ "request":{ "type":"City","query":"Tel Aviv-Yafo, Israel","language":"en","unit":"m"},
                //"location":{ "name":"Tel Aviv-Yafo","country":"Israel","region":"Tel Aviv","lat":"32.068","lon":"34.765",
                //"timezone_id":"Asia\/Jerusalem","localtime":"2020-03-02 19:30","localtime_epoch":1583177400,"utc_offset":"2.0"},
                //"current":{ "observation_time":"05:30 PM","temperature":17,"weather_code":116,"weather_icons":["https:\/\/assets.weatherstack.com\/images\/wsymbols01_png_64\/wsymbol_0004_black_low_cloud.png"],
                //"weather_descriptions":["Partly cloudy"],"wind_speed":10,"wind_degree":311,"wind_dir":"NW","pressure":1021,"precip":0,"humidity":60,"cloudcover":7,"feelslike":17,"uv_index":1,"visibility":10,"is_day":"no"}}
            }
            catch (Exception ex)
            {
                Utils.WriteToLog("Can't reach site: " + url.AbsoluteUri + "\n" + "Error: " + ex.Message);
            }




            isSiteWasFound = true;
            WeatherList = new List<Weather>();


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
                    icon = icon.Replace("[\""+"https:\\/\\/assets", @"https://assets");
                    icon = icon.Replace("[", "");
                    icon = icon.Replace("]", "");
                    icon = icon.Replace("\"", "");
                    weather.setPoster(icon);
                    weather.setIs_day(objectCurrent.GetString("is_day").ToString());
                    weather.setCloud(objectCurrent.GetString("cloudcover"));

                    weather.setLast_update(objectLocation.GetString("localtime"));
                    weather.setCountry(objectLocation.GetString("country"));
                    icon = objectLocation.GetString("name");
                    icon = icon.Replace("[", "");
                    icon = icon.Replace("]", "");
                    icon = icon.Replace("\"", "");
                    weather.setCity(icon);         // "type":"City","query":"Tel Aviv-Yafo, Israel"
                    weather.setRegion(objectLocation.GetString("region"));
                    weather.setLocal_time(objectLocation.GetString("localtime"));
                    weather.setTz_id(objectLocation.GetString("timezone_id"));

                    //"localtime_epoch":1583177400,"utc_offset":"2.0"
                    //wind_degree":311,"wind_dir":"NW","pressure":1021,"precip":0,"humidity":60,"cloudcover":7,"feelslike":17

                    weather.setImageView(Utils.SetWeatherImage(weather.getPoster()));
                    //weather.setImageView(new ImageView(Application.Context));
                    //Android.Net.Uri uri = Android.Net.Uri.Parse(weather.getPoster());
                    //weather.getImageView().SetImageURI(uri);

                    //WeatherList.Add(weather);
                }
                catch (JSONException ex)
                {
                    Utils.WriteToLog(ex.Message);
                    //Log.d("Error: ", ex.getMessage());
                    //ex.printStackTrace();
                }
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

        private void CallbackResponse(IAsyncResult ar)
        {
        
            isSiteWasFound = true;

            object responseObj = ar.AsyncState;

            Org.Json.JSONObject weatherMain = new Org.Json.JSONObject();
            
            Org.Json.JSONArray weatherArray = new Org.Json.JSONArray();
            weatherArray.Put(weatherMain);

            for (int i = 0; i < weatherArray.Length(); i++)
            {
                try
                {
                    Org.Json.JSONObject weatherObject =  weatherArray.GetJSONObject(i);

                    JSONObject objectLocation = weatherObject.GetJSONObject("location");
                    JSONObject objectCurrent = weatherObject.GetJSONObject("current");
                    JSONObject objectCondition = objectCurrent.GetJSONObject("condition");

                    Weather weather = new Weather();

                    weather.setTemperature(objectCurrent.GetString("temp_c"));
                    weather.setDescription(objectCondition.GetString("text"));
                    weather.setWind_kph(objectCurrent.GetString("wind_kph"));
                    weather.setLast_update(objectCurrent.GetString("last_updated"));
                    weather.setPoster(objectCondition.GetString("icon"));
                    weather.setCountry(objectLocation.GetString("country"));
                    weather.setCity(objectLocation.GetString("name"));
                    weather.setRegion(objectLocation.GetString("region"));
                    weather.setLocal_time(objectLocation.GetString("localtime"));
                    weather.setIs_day(objectCurrent.GetString("is_day").ToString());
                    weather.setCloud(objectCurrent.GetString("cloud"));
                    weather.setTz_id(objectLocation.GetString("tz_id"));

                    weather.setImageView(new ImageView(Application.Context));
                    Uri uri = new Uri(weather.getPoster());
                    //weather.getImageView().SetImageURI(uri);

                    WeatherList.Add(weather);
                }
                catch (JSONException ex)
                {
                    Utils.WriteToLog(ex.Message);
                    //Log.d("Error: ", ex.getMessage());
                    //ex.printStackTrace();
                }
            }

            //weatherRecyclerViewAdpter = new WeatherRecyclerViewAdpter(getApplicationContext(), weatherList);
            //listRecycler.setAdapter(weatherRecyclerViewAdpter);
            //weatherRecyclerViewAdpter.notifyDataSetChanged();  // Very Important !!! Otherwise we wont see anything displayed

            //return WeatherList;
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

        
    }
    

}