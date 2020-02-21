using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Text;
using Android.Views;
using Android.Widget;

using ProjTaskReminder.Utils;


namespace ProjTaskReminder.Utils
{
    public class MH_Notification
    {

        public Context context;
        public Activity activity;


        public static Android.Support.V4.App.NotificationCompat.Builder NotificationBuilder;
        public static PendingIntent         intentSnooze;   // Snooze inside the notification
        public static PendingIntent         intentAction;
        public static int                   notifCounter;
        public static int                   NOTIFIC_REQUEST_CODE_ACTION = 98;
        public static int                   NOTIFIC_REQUEST_CODE_SNOOZE = 97;
        private static string               CHANNEL_ID = "תזכורות רגילות";
        private static string               WAKEUP_SCREEN_TAG = "MY_TAG:TASK";


        public static string                SNOOZE_WORD = "actionSnooze";
        public static string                ACTION_WORD = "actionUpdateTask";



        public MH_Notification(Activity activity, Context context)
        {
            this.activity = activity;
            this.context = context;
        }

        private PendingIntent createIntentSnooze(int objectID)
        {
            PendingIntent snoozePendingIntent;

            ////ActivityNotification.clickContext = context;

            Intent snoozeIntent = new Intent(context, typeof(Activity));    // ActivityNotification.class);

            snoozeIntent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);      // .FLAG_ACTIVITY_NEW_TASK | .FLAG_ACTIVITY_CLEAR_TASK
            snoozeIntent.PutExtra("TaskID", objectID);   // EXTRA_NOTIFICATION_ID
            snoozeIntent.SetAction(SNOOZE_WORD);

            snoozePendingIntent = PendingIntent.GetActivity(context, NOTIFIC_REQUEST_CODE_SNOOZE, snoozeIntent, PendingIntentFlags.UpdateCurrent);  // PendingIntent.FLAG_UPDATE_CURRENT);

            // Connect to external device
            //snoozePendingIntent = PendingIntent.getBroadcast(context, NOTIFIC_REQUEST_CODE_SNOOZE, snoozeIntent, PendingIntent.FLAG_UPDATE_CURRENT);

            intentSnooze = snoozePendingIntent;

            return snoozePendingIntent;
        }

        public PendingIntent createIntentAction(int objectID)
        {
            PendingIntent pendingIntent;

            //ActivityNotification.clickContext = context;

            // Create an explicit intent for an Activity in your app
            Intent actionIntent = new Intent(context, typeof(Activity));        // ActivityNotification.class);

            actionIntent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);   // | Intent.FLAG_ACTIVITY_CLEAR_TASK);  FLAG_ACTIVITY_BROUGHT_TO_FRONT
            actionIntent.PutExtra("TaskID", objectID);  // EXTRA_NOTIFICATION_ID
            actionIntent.SetAction(ACTION_WORD);

            //intent.setFlags(Intent.FLAG_ACTIVITY_CLEAR_TOP);

            pendingIntent = PendingIntent.GetActivity(context, NOTIFIC_REQUEST_CODE_ACTION, actionIntent, PendingIntentFlags.UpdateCurrent);    //.FLAG_UPDATE_CURRENT);   //  FLAG_ONE_SHOT);

            // Connect to external device
            //PendingIntent pendingIntent = PendingIntent.getBroadcast(context, 0, intent, PendingIntent.FLAG_UPDATE_CURRENT);

            intentAction = pendingIntent;

            return pendingIntent;
        }

    public Android.Support.V4.App.NotificationCompat.Action replyAction(int objectID)
    {
        NotificationCompat.Action action = null;


        string NOTI_KEY_TEXT_REPLY = "key_text_reply";
        string replyCaption = "השב";

        Intent replyIntent = new Intent(context, typeof(Activity));     // ActivityNotification.class);

        //replyIntent.setFlags(Intent.FLAG_ACTIVITY_NEW_TASK | Intent.FLAG_ACTIVITY_CLEAR_TASK);
        Android.App.RemoteInput remoteInput = new Android.App.RemoteInput.Builder(NOTI_KEY_TEXT_REPLY).SetLabel(replyCaption).Build();
        PendingIntent replyPendingIntent = PendingIntent.GetBroadcast(context, NOTIFIC_REQUEST_CODE_ACTION, replyIntent, PendingIntentFlags.UpdateCurrent); ;   // PendingIntent.FLAG_UPDATE_CURRENT);

        //NotificationCompat.Action action = new NotificationCompat.Action.Builder(icon, replyCaption, replyPendingIntent).addRemoteInput(remoteInput).build();

        //To receive user input from the notification's reply UI, call RemoteInput.getResultsFromIntent(), passing it the Intent received by your BroadcastReceiver:
        //private CharSequence getMessageText(Intent intent)
        //{
        //    Bundle remoteInput = RemoteInput.getResultsFromIntent(intent);
        //    if (remoteInput != null) {
        //        return remoteInput.getCharSequence(KEY_TEXT_REPLY);
        //}

        return action;
    }

    public void createNotificationChannel()
    {
        try
        {
            // Create the NotificationChannel, but only on API 26+ because
            // Just from android API 26 and above
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)   //.SDK_INT >= Build.VERSION_CODES.O)
            {
                //Log.d("Notification", "Enter createNotificationChannel()");
                CHANNEL_ID = context.GetString(Resource.String.notification_channel_id);
                //CHANNEL_ID = NotificationManager.ACTION_NOTIFICATION_POLICY_CHANGED;      //ACTION_NOTIFICATION_POLICY_ACCESS_GRANTED_CHANGED;
               
                string CHANNEL_NAME = context.GetString(Resource.String.notification_channel_description);
                //CharSequence CHANNEL_NAME = context.GetString(Resource.String.notification_channel_description);
                
                Android.App.NotificationImportance importance = NotificationManager.ImportanceHigh;    //.IMPORTANCE_HIGH;       //NotificationCompat.PRIORITY_MAX;           // NotificationCompat.PRIORITY_HIGH            // NotificationManager.IMPORTANCE_DEFAULT;   //NotificationCompat.IMPORTANCE_DEFAULT;

                NotificationChannel channel = new NotificationChannel(CHANNEL_ID, CHANNEL_NAME, importance);

                //channel.setDescription(CHANNEL_NAME.toString());
                channel.EnableLights(true);
                channel.EnableVibration(true);
                channel.LightColor =Android.Graphics.Color.Blue;
                channel.LockscreenVisibility = NotificationVisibility.Public;   // Notification.VISIBILITY_PUBLIC);

                // Register the channel with the system; you can't change the importance
                // or other notification behaviors after this
                NotificationManager notificationManager = (NotificationManager)context.GetSystemService(Context.NotificationService);    // NotificationManager);  //Context.NOTIFICATION_SERVICE
                notificationManager.CreateNotificationChannel(channel);
            }
        }
        catch (Exception ex)
        {
            Utils.WriteToLog("Error in Util createNotificationChannel()" + "\n"+ex.Message);
        }

    }
                                                                                            // CharSequence
    public NotificationCompat.Builder createNotificationBuilder(string title, string description, int icon, int notificationID)  // CharSequence, SpannableString
        {

        createNotificationChannel();

        intentAction = createIntentAction(notificationID);

        // Snooze inside the notification
        intentSnooze = createIntentSnooze(notificationID);

        string spannedDescription = description;
        //SpannedString spannedDescription = description;

        string descriptionPureHtml = spannedDescription;
        //string descriptionPureHtml = Html.ToHtml(spannedDescription);

        string descriptionPure = spannedDescription.ToString().Trim();

        Utils.WriteToLog("description in notification: " + descriptionPureHtml);

        ////List<string> descriptionLines = new List<string>();
        ////String[] descriptionLines2 = descriptionPureHtml.Split(Utils.LINE_SEPERATOR);       //"<br>")
        ////NotificationCompat.InboxStyle descriptionLinesStyle = new NotificationCompat.InboxStyle();

        //descriptionLines.add(description);

        ////for (int i = 0; i < descriptionLines2.Length; i++)
        ////{
        ////        //Log.d("Split description in notification2", descriptionLines2[i]);
        ////    //((NotificationCompat.InboxStyle)descriptionLinesStyle).AddLine(HdescriptionLines2[i]);
        ////    //((NotificationCompat.InboxStyle)descriptionLinesStyle).AddLine(Html.FromHtml(descriptionLines2[i]));
        ////}

        int color = Android.Graphics.Color.Blue;

        try
        {
            NotificationBuilder = new NotificationCompat.Builder(context, CHANNEL_ID)
                        .SetSmallIcon(icon)
                        //.setLargeIcon(BitmapFactory.decodeResource(context.getResources(), largeIconID))
                        .SetContentTitle(title)
                        .SetContentText(description)
                        //.setSubText("Notes")
                        .SetNumber(notificationID)
                        .SetPriority(NotificationCompat.PriorityHigh)       //.PRIORITY_HIGH)              // PRIORITY_MAX  //.PRIORITY_DEFAULT
                                                                            //.setSound(RingtoneManager.getDefaultUri(RingtoneManager.TYPE_ALARM))
                                                                            // Set the intent that will fire when the user taps the notification
                        .SetContentIntent(intentAction)
                        // Set the intent that will fire when the user taps the snoozer
                        .AddAction(Resource.Mipmap.note1, context.GetString(Resource.String.notification_snooze_message), intentSnooze)
                        //.addAction(replyAction(notificationID))
                        .SetVisibility((int)NotificationVisibility.Public)   // Notification..VISIBILITY_PUBLIC)
                        .SetColor(color)
                        .SetAutoCancel(true);
                    //.setStyle(new NotificationCompat.BigTextStyle().bigText("This is the expandable content text"))
                    //.SetStyle(descriptionLinesStyle);
            //                                              .setStyle(new NotificationCompat.InboxStyle()
            //                                                                            .addLine("This is the first line")
            //                                                                            .addLine("This is the second line")
            //                                                                            .addLine("This is the third line"));
            // Issue the initial notification with progress bar
            //int PROGRESS_MAX = 100;
            //int PROGRESS_CURRENT = 0;
            //setProgress(PROGRESS_MAX, PROGRESS_CURRENT, false);
            // When done, update the notification one more time to remove the progress bar
            //.setContentText("Download complete")
            //.setProgress(0,0,false);
            //notificationManager.notify(notificationId, builder.build());
            //.setAutoCancel(true)
            //.setVibrate(new long[] {500, 500, 600, 500, 500});


            Bundle bundle = new Bundle();
            bundle.PutInt("TaskID", notificationID);
            NotificationBuilder.SetExtras(bundle);

            showNotification(NotificationBuilder, notificationID);

        }
        catch (Exception ex)
        {
            Utils.WriteToLog("Error in Util createNotificationBuilderl()" + "\n" + ex.Message);
        }


        return NotificationBuilder;
    }

    public void showNotification(NotificationCompat.Builder builder, int notificationID)
    {

        NotificationManagerCompat notificationManager = NotificationManagerCompat.From(context);

        Notification notification = builder.Build();

        //notification.getChannelId()

        //Bundle bundle = builder.getExtras();
        //bundle.getInt("TaskID");
        //notificationId = ((Intent)intentAction.getIntentSender()).getIntExtra("TaskID", 0);

        //++notifCounter
        notifCounter = notificationID;
        //notificationID = ++notifCounter;

        // notificationId is a unique int for each notification that you must define
        notificationManager.Notify(notificationID, notification);
    }

    }
}