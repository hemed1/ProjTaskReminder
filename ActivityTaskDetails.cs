using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using ProjTaskReminder.Data;
using ProjTaskReminder.Model;
using static ProjTaskReminder.Data.DBTaskReminder;

namespace ProjTaskReminder
{
    [Activity(Label = "ActivityTaskDetails")]
    public class ActivityTaskDetails : Activity
    {

        public static Task CurrentTask;

        private EditText txtDetailsTitle;
        private EditText txtDetailsDescription;
        private TextView txtDetailsDate;
        //private TextView txtDetailsDateDay;
        //private TextView txtDetailsTime;
        //private TextView txtDetailsRepeat;
        //private TextView lblDetailsLastUpdateValue;
        //private TextView txtDetailsColor;
        //private EditText txtTakeFocus;
        //private ImageView imgColor1;
        //private ImageView imgColor2;
        //private ImageView imgColor3;
        //private ImageView imgColor4;
        //private ImageView imgColor5;
        //private ImageView imgColor6;
        //private ImageView imgColor7;
        //private ImageView imgColor8;
        private Button btnSave;
        private Button btnDelete;

        //private ToggleButton btnFontBold;
        //private ToggleButton btnFontUnderline;
        //private ToggleButton btnFontStrike;
        //private ToggleButton btnFontBullets;
        //private readonly ToggleButton btnSpanCheck;
        //private Button btnFontBackColor;
        //private Button btnFontForeColor;
        //private Button btnFontName;
        //private Button btnFontSize;
        //private Button btnFlatHtml;



        //private TextView lblDateTime;
        //private CardView cardDetails;
        //private CardView cardDetailsTiming;
        //private CardView cardRichText;
        //private ConstraintLayout layoutMain;

        //public static Drawable ExternalDrawable;
        public static bool isNewMode;
        ////public static DatabaseHandler dbHandler;
        //public DBTaskReminder dbHandler;
        //private bool isShowSaveDialog;
        public static Context context;
        //public static MainActivity mainActivity;
        //private static Menu CurrentMenu;

        //private const int RESULT_EXIT_UPDATED = 0;  // RESULT_OK;
        //private const int RESULT_EXIT_DELETE = 0;   // RESULT_OK;
        //private const int RESULT_EXIT = 1;          // RESULT_CANCELED;
        //private const int RESULT_HTML_EDIT = 996;
        //private int returnResult;
        //private Intent returnedIntent;
        //private bool isSaved;
        //private bool isChangesMade = false;
        //private string oldTitle;
        //private string oldDescription;
        //private string oldDate;
        //private string oldTime;
        //private string oldColor;
        //private bool oldIsArchive;


        //private static PersonalEvents.OnSaveButton listenerSaveButton;
        //private static PersonalEvents.OnDeleteButton listenerDeleteButton;


        //private static int RichTextForeColor = 0;
        //private static int RichTextBackColor = 0;

        //private static SpannableString      currentSpannableString;
        //private static ParcelableSpan[] currentParcelableSpan;
        //private ArrayMap<Object, ParcelableSpan> currentSpannableMap;
        //private MainActivityServices mainActivityServices;
        //private RichTextHandle richTextHandle;

        //public event btnSave_Click2(object sender, EventArgs eventArgs);
        //private static OnSaveButtonInterface listenerSaveButton;

        public static event EventHandler OnSaveButton;
        public static event Action<int, Result, Intent> OnActivityResult;
        private Intent inputIntent;
        private int taskID;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Utils.Utils.LoggerWrite("Enter 5", true);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_task_details);

            Utils.Utils.LoggerWrite("Enter 6", true);

            inputIntent = this.Intent;

            if (!string.IsNullOrEmpty(inputIntent.GetStringExtra("TaskID")))
            {
                taskID = int.Parse(inputIntent.GetStringExtra("TaskID"));
            }


            SetControlsIO();

            if (isNewMode)
            {

            }
            else 
            {
                SetControlsByObject();
            }

        }

        private void SetControlsIO()
        {
            txtDetailsTitle = FindViewById<EditText>(Resource.Id.txtDetailsTitle);
            txtDetailsDescription = FindViewById<EditText>(Resource.Id.txtDetailsDescription);
            txtDetailsDate = FindViewById<TextView>(Resource.Id.txtDetailsDate);
            //txtDetailsDateDay = FindViewById<TextView>(Resource.Id.txtDetailsDateDay);
            //txtDetailsTime = FindViewById<TextView>(Resource.Id.txtDetailsTime);
            //txtDetailsRepeat = FindViewById<TextView>(Resource.Id.txtDetailsRepeat);
            //TextView lblDetailsDate = FindViewById<TextView>(Resource.Id.lblDetailsDate);
            //TextView lblDetailsTime = FindViewById<TextView>(Resource.Id.lblDetailsTime);
            //lblDetailsLastUpdateValue = FindViewById<TextView>(Resource.Id.lblDetailsLastUpdateValue);
            //txtDetailsColor = FindViewById<TextView>(Resource.Id.txtDetailsColor);
            //txtTakeFocus = FindViewById<EditText>(Resource.Id.txtTakeFocus);
            //imgColor1 = FindViewById<ImageView>(Resource.Id.imgColor1);
            //imgColor2 = FindViewById<ImageView>(Resource.Id.imgColor2);
            //imgColor3 = FindViewById<ImageView>(Resource.Id.imgColor3);
            //imgColor4 = FindViewById<ImageView>(Resource.Id.imgColor4);
            //imgColor5 = FindViewById<ImageView>(Resource.Id.imgColor5);
            //imgColor6 = FindViewById<ImageView>(Resource.Id.imgColor6);
            //imgColor7 = FindViewById<ImageView>(Resource.Id.imgColor7);
            //imgColor8 = FindViewById<ImageView>(Resource.Id.imgColor8);

            btnSave = FindViewById<Button>(Resource.Id.btnSave);
            btnDelete = FindViewById<Button>(Resource.Id.btnDelete);
            //lblDateTime = FindViewById<TextView>(Resource.Id.lblDateTime);
            //cardDetails = FindViewById<CardView>(Resource.Id.cardDetails);
            //cardDetailsTiming = FindViewById<CardView>(Resource.Id.cardDetailsTiming);
            //layoutMain = FindViewById(Resource.Id.layoutMain);

            //btnFontBold = FindViewById<ToggleButton>(Resource.Id.btnFontBold);
            //btnFontBackColor = FindViewById<Button>(Resource.Id.btnFontBackColor);
            //btnFontForeColor = FindViewById<Button>(Resource.Id.btnFontForeColor);
            //btnFontName = FindViewById<Button>(Resource.Id.btnFontName);
            //btnFontSize = FindViewById<Button>(Resource.Id.btnFontSize);
            //btnFontUnderline = FindViewById<ToggleButton>(Resource.Id.btnFontUnderline);
            //btnFontStrike = FindViewById<ToggleButton>(Resource.Id.btnFontStrike);
            //btnFontBullets = FindViewById<ToggleButton>(Resource.Id.btnFontBullets);
            //btnFlatHtml = FindViewById<Button>(Resource.Id.btnFlatHtml);

            //btnSave.Click += btnSave_Click(null, null);

            btnSave.Click += (sender, e) =>           //new EventHandler(SaveRecord); 
            {
                SaveRecord(CurrentTask);
            };

            btnDelete.Click += (sender, e) =>           //new EventHandler(SaveRecord); 
            {
                DeleteRecord(CurrentTask);
            };
        }

        private void DeleteRecord(Task currentTask)
        {
            MainActivity.DBTaskReminder.DB.Delete<TBL_Tasks>(currentTask.getTaskID());  // "ID==" +currentTask.getTaskID().ToString(), null);

            OnActivityResult(999, Result.Ok, inputIntent);

            //SetResult(Result.Ok, inputIntent);

            Finish();
        }

        private bool SaveRecord(Task task)
        {
            bool result = true;

            SetObjectByControls();

            // only insert the data if it doesn't already exist
            var item = new TBL_Tasks();

            try
            {
                item.Title = task.getTitle();
                item.Description = task.getDescription();
                MainActivity.DBTaskReminder.DB.Insert(item);

                inputIntent.PutExtra("Result", "true");

                // Raise the event to the Caller
                if (OnSaveButton != null)
                {
                    //OnSaveButton(null, EventArgs.Empty);
                }

                // Second option to - Raise the event to the Caller
                OnActivityResult(111, Result.Ok, inputIntent);

                //SetResult(Result.Ok, inputIntent);
                
                Finish();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return result;
        }

        //private void OnActivityResult(int requestCode, Result resultCode, Intent data)
        //{
        //    //base.OnActivityReenter(requestCode,resultCode, data);

        //    //FillIn();
        //}

        private void SetObjectByControls()
        {
            CurrentTask.setTitle(txtDetailsTitle.Text);
            CurrentTask.setDescription(txtDetailsDescription.Text);
            CurrentTask.setDate_due(txtDetailsDate.Text);
        }

        private void SetControlsByObject()
        {
            txtDetailsTitle.Text = CurrentTask.getTitle();
            txtDetailsDescription.Text = CurrentTask.getDescription();
            //txtDetailsDate.Text = CurrentTask.getDate_due();
        }

        // Assign the listener implementing events interface that will receive the events
        //public static void OnSaveButton(OnSaveButtonInterface listener)
        //{
        //    listenerSaveButton = listener;
        //}

    }

}



                ////if (MainActivity.ThemColorSet == 1)
                ////{
                ////    txtDetailsTitle.setTextColor(context.getResources().getColor(R.color.CardFieldsTextForegroundColor));
                ////    txtDetailsDescription.setTextColor(context.getResources().getColor(R.color.CardFieldsTextForegroundColor));
                ////    lblDateTime.setTextColor(context.getResources().getColor(R.color.CardFieldsTextForegroundColor));
                ////}
                ////else
                ////{
                ////    txtDetailsTitle.setTextColor(context.getResources().getColor(R.color.CardFieldsTextForegroundColor2));
                ////    txtDetailsDescription.setTextColor(context.getResources().getColor(R.color.CardFieldsTextForegroundColor2));
                ////    lblDateTime.setTextColor(context.getResources().getColor(R.color.CardFieldsTextForegroundColor2));
                ////}

////cardDetailsTiming.setOnClickListener(new View.OnClickListener()
////{
////    @Override
////    public void onClick(View v)
////    {
////        cardDetailsTiming.setVisibility(View.INVISIBLE);
////    }
////}
////        cardRichText = FindViewById(Resource.Id.cardRichText);
////        cardRichText.setOnClickListener(new View.OnClickListener()
////                                            {
////                                                @Override
////                                                public void onClick(View v)
////        {
////            cardRichText.setVisibility(View.INVISIBLE);
////        }
////    }
////        );



////        View.OnFocusChangeListener focus = txtDetailsDescription.getOnFocusChangeListener();
////    //focus.onFocusChange(View);

////    txtDetailsDescription.addTextChangedListener(new TextWatcher()
////    {
////        @Override
////            public void beforeTextChanged(CharSequence s, int start, int count, int after)
////        {

////        }

////        @Override
////            public void onTextChanged(CharSequence s, int start, int before, int count)
////        {

////        }

////        @Override
////            public void afterTextChanged(Editable s)
////        {

////        }
////    });

////        btnSave.setOnClickListener(new View.OnClickListener() {
////            @Override
////            public void onClick(View v)
////    {
////        saveTaskDetails();
////    }
////});

////        btnDelete.setOnClickListener(new View.OnClickListener() {
////            @Override
////            public void onClick(View v)
////{
////    deleteTask();
////}
////        });


////        btnFontBold.setOnClickListener(new View.OnClickListener() {
////            @Override
////            public void onClick(View v)
////{
////    richTextHandle.richTextBold(txtDetailsDescription);
////}
////        });
////        btnFontBackColor.setOnClickListener(new View.OnClickListener() {
////            @Override
////            public void onClick(View v)
////{
////    richTextBackColor(txtDetailsDescription);
////}
////        });
////        btnFontForeColor.setOnClickListener(new View.OnClickListener() {
////            @Override
////            public void onClick(View v)
////{
////    richTextForeColor(txtDetailsDescription);
////}
////        });
////        btnFontName.setOnClickListener(new View.OnClickListener() {
////            @Override
////            public void onClick(View v)
////{
////    richTextHandle.richTextFontName(txtDetailsDescription);
////}
///        });
////        btnFontSize.setOnClickListener(new View.OnClickListener() {
////            @Override
////            public void onClick(View v)
////{
////    if (!MainActivity.IsManualHtml)
////    {
////        richTextHandle.richTextFontSize(txtDetailsDescription);
////    }
////    else
////    {
////        string newHtml = richTextHandle.richTextFontSize(txtDetailsDescription);
////        Log.d("Come Back 1 - change font size", newHtml);
////        CurrentTask.setDescription(newHtml);
////        //txtDetailsDescription.setText(CurrentTask.getDescription());    //, TextView.BufferType.SPANNABLE);
////        //Log.d("Come Back direct from control - change font size", Html.toHtml(txtDetailsDescription.getText()));
////    }
////}
////        });

////        btnFontUnderline.setOnClickListener(new View.OnClickListener() {
////            @Override
////            public void onClick(View v)
////{
////    richTextHandle.richTextUnderline(txtDetailsDescription);
////}
////        });

////        btnFontStrike.setOnClickListener(new View.OnClickListener() {
////            @Override
////            public void onClick(View v)
////{
////    richTextHandle.richTextStrike(txtDetailsDescription);
////}
////        });

////        btnFontBullets.setOnClickListener(new View.OnClickListener() {
////            @Override
////            public void onClick(View v)
////{
////    string newHml = richTextHandle.richTextBullets(txtDetailsDescription);
////    CurrentTask.setDescription(newHml);
////}
////        });

////        btnFlatHtml.setOnClickListener(new View.OnClickListener()
////        {
////            @Override
////            public void onClick(View v)
////{
////    Intent intent = new Intent(ActivityTaskDetails.this, ActivityEditHtml.class);
////                intent.putExtra("HtmlText", CurrentTask.getDescriptionWithHtml());

////                Log.d("HtmlText", CurrentTask.getDescriptionWithHtml());

////                ActivityEditHtml.CurrentTask = CurrentTask;

////                startActivityForResult(intent, RESULT_HTML_EDIT);
////                //startActivity(intent);
////            }
////        });


////        lblDateTime.setOnClickListener(new View.OnClickListener() {
////            @Override
////            public void onClick(View v)
////{
////    openTimingCard();
////}
////        });

////        Util.activity = this;
////        Util.context = ActivityTaskDetails.this;

////        lblDetailsDate.setOnClickListener(new View.OnClickListener() {
////            @Override
////            public void onClick(View v)
////{
////    ShowDateDialog();
////}
////        });
////        txtDetailsDate.setOnClickListener(new View.OnClickListener() {
////            @Override
////            public void onClick(View v)
////{
////    ShowDateDialog();
////}
////        });


////        Util.OnDateDialogResult(new PersonalEvents.OnDateDialogSet()
////        {
////            @Override
////            public void setDateDialogResult(string dateResult, Date date)
////{
////    Log.d("setDateDialogResult", dateResult + "***" + string.valueOf(date));
////    string dateDay = " יום " + Util.getDateDayName(date);
////    txtDetailsDateDay.setText(dateDay);
////    txtDetailsDate.setText(dateResult);

////    ShowTimeDialog();
////}
////        });

////        Util.OnDateDialogCancel(new PersonalEvents.OnDateDialogCancel()
////        {
////            @Override
////            public void setDateDialogCancel()
////{
////    FocusNothingToCloseKeyboard();
////}
////        });

////        Util.OnTimeDialogResult(new PersonalEvents.OnTimeDialogSet()
////        {
////            @Override
////            public void setTimeDialogResult(string timeResult)
////{
////    txtDetailsTime.setText(timeResult);
////    txtTakeFocus.setShowSoftInputOnFocus(false);
////    txtTakeFocus.requestFocus();
////    Util.closeKeyboard();
////}
////        });


////        lblDetailsTime.setOnClickListener(new View.OnClickListener() {
////            @Override
////            public void onClick(View v)
////{
////    ShowTimeDialog();
////}
////        });
////        txtDetailsTime.setOnClickListener(new View.OnClickListener() {
////            @Override
////            public void onClick(View v)
////{
////    ShowTimeDialog();
////}
////        });

////        Resources resources = getResources();

////imgColor1.setDrawingCacheBackgroundColor(resources.getColor(R.color.card_color1));  //Util.getColor(string.valueOf(R.color.card_color1)));
////        imgColor1.setBackgroundColor(resources.getColor(R.color.card_color1));
////        imgColor2.setDrawingCacheBackgroundColor(resources.getColor(R.color.card_color2));
////        imgColor2.setBackgroundColor(resources.getColor(R.color.card_color2));
////        imgColor3.setDrawingCacheBackgroundColor(resources.getColor(R.color.card_color3));
////        imgColor3.setBackgroundColor(resources.getColor(R.color.card_color3));
////        imgColor4.setDrawingCacheBackgroundColor(resources.getColor(R.color.card_color4));
////        imgColor4.setBackgroundColor(resources.getColor(R.color.card_color4));
////        imgColor5.setDrawingCacheBackgroundColor(resources.getColor(R.color.card_color5));
////        imgColor5.setBackgroundColor(resources.getColor(R.color.card_color5));
////        imgColor6.setDrawingCacheBackgroundColor(resources.getColor(R.color.card_color6));
////        imgColor6.setBackgroundColor(resources.getColor(R.color.card_color6));
////        imgColor7.setDrawingCacheBackgroundColor(resources.getColor(R.color.card_color7));
////        imgColor7.setBackgroundColor(resources.getColor(R.color.card_color7));
////        imgColor8.setDrawingCacheBackgroundColor(resources.getColor(R.color.card_color8));
////        imgColor8.setBackgroundColor(resources.getColor(R.color.card_color8));

////        imgColor1.setOnClickListener(new View.OnClickListener()
////        {
////            @Override
////            public void onClick(View v)
////{
////    setBackColor(imgColor1);
////}
////        });
////        imgColor2.setOnClickListener(new View.OnClickListener() {
////            @Override
////            public void onClick(View v)
////{
////    setBackColor(imgColor2);
////}
////        });
////        imgColor3.setOnClickListener(new View.OnClickListener() {
////            @Override
////            public void onClick(View v)
////{
////    setBackColor(imgColor3);
////}
////        });
////        imgColor4.setOnClickListener(new View.OnClickListener() {
////            @Override
////            public void onClick(View v)
////{
////    setBackColor(imgColor4);
////}
////        });
////        imgColor5.setOnClickListener(new View.OnClickListener() {
////            @Override
////            public void onClick(View v)
////{
////    setBackColor(imgColor5);
////}
////        });
////        imgColor6.setOnClickListener(new View.OnClickListener() {
////            @Override
////            public void onClick(View v)
////{
////    setBackColor(imgColor6);
////}
////        });
////        imgColor7.setOnClickListener(new View.OnClickListener()
////        {
////            @Override
////            public void onClick(View v)
////{
////    setBackColor(imgColor7);
////}
////        });
////        imgColor8.setOnClickListener(new View.OnClickListener()
////        {
////            @Override
////            public void onClick(View v)
////{
////    setBackColor(imgColor8);
////}
////        });

////        }
