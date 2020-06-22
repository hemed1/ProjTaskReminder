using Android.App;
using Android.Content;
using Android.Util;
using Android.OS;
using System;
using Java.Util.Jar;



namespace ProjTaskReminder.Utils
{

	/// <summary>
	/// This is a sample started service. When the service is started, it will log a string that details how long 
	/// the service has been running (using Android.Util.Log). This service displays a notification in the notification
	/// tray while the service is active.
	/// </summary>
	//[Service(Name = "com.xamarin.TimestampService", Process = ":timestampservice_process", Exported = true, IsolatedProcess = true)]
	//[IntentFilter(new String[] { "com.xamarin.DemoService" })]
	// By default, a service will start in the same process as an Android application. It is possible to start a service in its own process by setting the ServiceAttribute.IsolatedProcess property to true:
	//[Service(IsolatedProcess = true)]
	// register the service by injecting the following XML element into AndroidManifest.xml
	//[Servie(Exported = true, IsolatedProcess = true, name = "com.meirhemed.service_keepalive")]
	[Service(Exported = false,IsolatedProcess = false , Name = "com.meirhemed.projtaskreminder.mhservicestayalive")]
	public class mhServiceStayAlive : Service
	{
		static readonly string TAG = typeof(mhServiceStayAlive).FullName;
		static readonly int DELAY_BETWEEN_LOG_MESSAGES = 5000;		// milliseconds
		static readonly int NOTIFICATION_ID = 10000;
		static readonly string SERVICE_STARTED_KEY = "has_service_been_started";


		//UtcTimestamper timestamper;
		bool isStarted;
		Android.OS.Handler handler;
		Action runnable;
		Intent InputIntent;

		public override void OnCreate()
		{
			base.OnCreate();
			Log.Info(TAG, "OnCreate: the service is initializing.");

			//timestamper = new UtcTimestamper();
			handler = new Handler();

			runnable = new Action(ReStartActivity);

			// This Action is only for demonstration purposes.
			//runnable = new Action(() =>
			//{
				//if (timestamper != null)
				//{
					Log.Debug(TAG, DateTime.Now.ToLongDateString());
					//handler.PostDelayed(runnable, DELAY_BETWEEN_LOG_MESSAGES);
					//handler.ExecutePost(runnable);
				//}
			//});

			//Message message = messenger = new Messenger(new TimestampRequestHandler(this));
			//message.What = 3;
			//Message msg = Message.Obtain(null, Constants.SAY_HELLO_TO_TIMESTAMP_SERVICE);
		}

		/// <summary>
		/// When Client (Activity) send Intent to work with
		/// </summary>
		/// <param name="intent"></param>
		/// <param name="flags"></param>
		/// <param name="startId"></param>
		/// <returns></returns>
		public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
		{
			InputIntent = intent;

			if (isStarted)
			{
				Log.Info(TAG, "OnStartCommand: This service has already been started.");
			}
			else
			{
				Log.Info(TAG, "OnStartCommand: The service is starting.");

				DispatchNotificationThatServiceIsRunning();

				//handler.PostDelayed(runnable, DELAY_BETWEEN_LOG_MESSAGES);
				//handler.PostDelayed(InputIntent, DELAY_BETWEEN_LOG_MESSAGES);
				//handler.Post(runnable);
				//handler.Post(new Java.Lang.IRunnable());
				//handler.PostAtTime(runnable, 30000);        // MiliSecond



				// Put in the client taht call that servic - Provide the package name and the name of the service with a ComponentName object.
				//ComponentName cn = new ComponentName(REMOTE_SERVICE_PACKAGE_NAME, REMOTE_SERVICE_COMPONENT_NAME);
				//Intent serviceToStart = new Intent();
				//serviceToStart.SetComponent(cn);


				isStarted = true;
			}

			// This tells Android not to restart the service if it is killed to reclaim resources.
			return StartCommandResult.Sticky;
		}


		public override IBinder OnBind(Intent intent)
		{
			// Return null because this is a pure started service. A hybrid service would return a binder that would
			// allow access to the GetFormattedStamp() method.
			return null;    // messenger.Binder;
		}


		public override void OnDestroy()
		{
			// We need to shut things down.
			Log.Debug(TAG, DateTime.Now.ToLongDateString());
			Log.Info(TAG, "OnDestroy: The started service is shutting down.");

			// Stop the handler.
			handler.RemoveCallbacks(runnable);
			//StopSelf();

			// Remove the notification from the status bar.
			NotificationManager notificationManager = (NotificationManager)GetSystemService(Android.Content.Context.NotificationService);
			notificationManager.Cancel(NOTIFICATION_ID);

			handler.Post(runnable);
			//handler.PostDelayed(runnable, DELAY_BETWEEN_LOG_MESSAGES);

			//timestamper = null;
			isStarted = false;

			base.OnDestroy();
		}

		private void ReStartActivity()
		{
			//Android.App.Activity mainActivity = MainActivity;
			string sendeAppName = InputIntent.Package;
			//InputIntent.Notify;

			Android.Net.Uri uri= Android.Net.Uri.Parse("com.meirhemed.projtaskreminder.mainactivity");
			Intent MainActivitIntent = new Intent(Intent.ActionView, uri);
			//Intent MainActivitIntent = new Intent(this, typeof(ProjTaskReminder.MainActivity));
			MainActivitIntent.SetFlags(ActivityFlags.NewTask);

			this.StartActivity(MainActivitIntent);		// InputIntent);

			//handler.PostDelayed(runnable, DELAY_BETWEEN_LOG_MESSAGES);
		}

		/// <summary>
		/// This method will return a formatted timestamp to the client.
		/// </summary>
		/// <returns>A string that details what time the service started and how long it has been running.</returns>
		string GetFormattedTimestamp()
		{
			return "";		// timestamper?.GetFormattedTimestamp();
		}

		[Obsolete]
		void DispatchNotificationThatServiceIsRunning()
		{
			Notification.Builder notificationBuilder = new Notification.Builder(this)
																.SetSmallIcon(Resource.Mipmap.note1)
																.SetContentTitle("Stay alive Service")
																.SetContentText("Stay alive Service is running");
			//.SetSmallIcon(Resource.Drawable.ic_stat_name)
			//.SetContentText(Resources.GetString(Resource.String.notification_text));

			NotificationManager notificationManager = (NotificationManager)GetSystemService(NotificationService);
			notificationManager.Notify(NOTIFICATION_ID, notificationBuilder.Build());
		}
	}

	// This is the message that the service will send to the client.
	//	Message responseMessage = Message.Obtain(null, Constants.RESPONSE_TO_SERVICE);
	//	Bundle dataToReturn = new Bundle();
	//	dataToReturn.PutString(Constants.RESPONSE_MESSAGE_KEY, "This is the result from the service.");
	//responseMessage.Data = dataToReturn;

	//// The msg object here is the message that was received by the service. The service will not instantiate a client,
	//// It will use the client that is encapsulated by the message from the client.
	//Messenger clientMessenger = msg.ReplyTo;
	//if (clientMessenger!= null)
	//{
	//    try
	//    {
	//        clientMessenger.Send(responseMessage);
	//    }
	//    catch (Exception ex)
	//    {
	//        Log.Error(TAG, ex, "There was a problem sending the message.");
	//    }
	//}

}
