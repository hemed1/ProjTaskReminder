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
using ProjTaskRemindet.Utils;
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
        private TextView txtDetailsTime;
        private EditText lblDateTime;
        private Button btnSave;
        private Button btnDelete;
        private Button btnSetDate;
        private DatePicker datePicker1;
        private TimePicker timePicker1;

        private string bkuTitle;
        private string bkuDescription;
        private string bkuDateDue;
        

        public static bool isNewMode;
        private Intent inputIntent;
        private int taskID;
        private bool IsUpdateDialogDate;
        private bool IsSaveNeededBeforeExit = true;


        public static Context context;
        public static event EventHandler OnSaveButton;
        public static event Action<int, Result, Intent> OnExitResult;
        


        ///public static DatabaseHandler dbHandler;
        //public DBTaskReminder dbHandler;
        //private bool isShowSaveDialog;
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
        //private static int RichTextForeColor = 0;
        //private static int RichTextBackColor = 0;
        //private static SpannableString      currentSpannableString;
        //private static ParcelableSpan[] currentParcelableSpan;
        //private ArrayMap<Object, ParcelableSpan> currentSpannableMap;
        //private MainActivityServices mainActivityServices;
        //private RichTextHandle richTextHandle;
        //private TextView txtDetailsDateDay;
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
        //private CardView cardDetails;
        //private CardView cardDetailsTiming;
        //private CardView cardRichText;
        //private ConstraintLayout layoutMain;

        //public static Drawable ExternalDrawable;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            //Utils.Utils.LoggerWrite("Enter 5", true);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_task_details);

            //Utils.Utils.LoggerWrite("Enter 6", true);

            inputIntent = this.Intent;

            if (!string.IsNullOrEmpty(inputIntent.GetStringExtra("TaskID")))
            {
                taskID = int.Parse(inputIntent.GetStringExtra("TaskID"));
            }


            SetControlsIO();

            if (isNewMode)
            {
                newTaskDetails();
            }
            else 
            {
                SetControlsByObject();
            }


            if (isNewMode || (txtDetailsTitle.Text.ToString().Trim().Equals("") && txtDetailsDescription.Text.ToString().Trim().Equals("")))
            {
                txtDetailsDescription.Focusable=true;
                txtDetailsDescription.RequestFocus();
                ////Util.focusOnEditTextControl(txtDetailsDescription);
                //Util.openKeyboard();
            }
            else
            {
                //txtDetailsDescription.RequestFocus();
                txtDetailsDescription.RequestFocusFromTouch();
                //Utils.Utils.closeKeyboard();
            }

            if (txtDetailsDescription.Text.Length > 200)
            {
                txtDetailsDescription.SetSelection(0);
            }
        }

        private void newTaskDetails()
        {
            // Done in 'MainActivity'
            //CurrentTask = MainActivity.newTaskDetails();

            SetControlsByObject();
        }

        private void SetControlsIO()
        {
            txtDetailsTitle = FindViewById<EditText>(Resource.Id.txtDetailsTitle);
            txtDetailsDescription = FindViewById<EditText>(Resource.Id.txtDetailsDescription);
            txtDetailsDate = FindViewById<TextView>(Resource.Id.txtDetailsDate);
            txtDetailsTime = FindViewById<TextView>(Resource.Id.txtDetailsTime);
            lblDateTime = FindViewById<EditText>(Resource.Id.lblDateTime);
            btnSave = FindViewById<Button>(Resource.Id.btnSave);
            btnSetDate = FindViewById<Button>(Resource.Id.btnSetDate);
            btnDelete = FindViewById<Button>(Resource.Id.btnDelete);
            datePicker1 = FindViewById<DatePicker>(Resource.Id.datePicker1);
            timePicker1 = FindViewById<TimePicker>(Resource.Id.timePicker1);
            
            //txtDetailsDateDay = FindViewById<TextView>(Resource.Id.txtDetailsDateDay);
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

            btnDelete.Click += (sender, e) =>           //new EventHandler(DeleteRecord); 
            {
                DeleteRecord(CurrentTask);
            };

            btnSetDate.Click += (sender, e) =>           //new EventHandler(SaveRecord); 
            {
                OpenDatePicker();
            };

            //datePicker1.SetOnDateChangedListener+=OnDateChanged; 
            //{
            //    sender = CurrentTask;
            //    OnDateChange(sender, null);
            //};

            datePicker1.DateChanged += OnDateChanged; //(sender, e) =>           //new EventHandler(SaveRecord); 

            //datePicker1.DateChanged += (sender, e) =>
            //{
            //    OnDateChanged(sender, e);
            //};
            //datePicker1.SetOnDateChangedListener()
            //{
            //    sender = CurrentTask;
            //    OnDateChange(sender, null);
            //};
            //datePicker1.Click += (sender, e) =>           //new EventHandler(SaveRecord); 
            //{
            //    sender = CurrentTask;
            //    OnDateChange(sender, null);
            //};

            timePicker1.TimeChanged += OnTimeChanged;

            IsUpdateDialogDate = true;
            IsSaveNeededBeforeExit = true;
        }

        private void OnDateChanged(object sender, DatePicker.DateChangedEventArgs e)
        {
            if (!IsUpdateDialogDate)
            {
                IsUpdateDialogDate = true;
                return;
            }

            Task task = CurrentTask;    //(Task)sender;

            txtDetailsDate.Text = e.DayOfMonth.ToString().PadLeft(2, '0') + "/" + (e.MonthOfYear+1).ToString().PadLeft(2, '0') + "/" + e.Year.ToString().PadLeft(4, '0');

            lblDateTime.Text = txtDetailsDate.Text + " " + txtDetailsTime.Text;
            
            datePicker1.Visibility = ViewStates.Invisible;

            OpenTimePicker();
        }

        private void OnTimeChanged(object sender, TimePicker.TimeChangedEventArgs e)
        {
            Task task = CurrentTask;    //(Task)sender;

            txtDetailsTime.Text = e.HourOfDay.ToString().PadLeft(2, '0') + ":" + e.Minute.ToString().PadLeft(2, '0');

            lblDateTime.Text = txtDetailsDate.Text + " " + txtDetailsTime.Text;

            timePicker1.Visibility = ViewStates.Invisible;
        }


        private void OpenDatePicker()
        {
            if (!txtDetailsDate.Text.Trim().Equals(""))
            {
                IsUpdateDialogDate = false;
                DateTime currentDate= Utils.Utils.getDateFormatUSA(txtDetailsDate.Text.Trim()).AddMonths(-1);
                datePicker1.UpdateDate(currentDate.Year, currentDate.Month, currentDate.Day);
            }
            datePicker1.Visibility = ViewStates.Visible;
            datePicker1.BringToFront();
        }

        private void OpenTimePicker()
        {
            if (!txtDetailsDate.Text.Trim().Equals("") && !txtDetailsTime.Text.Trim().Equals(""))
            {
                //DateTime currentDate = Utils.Utils.getDateFormatUSA(txtDetailsDate.Text.Trim()+" " + txtDetailsTime.Text.Trim());
                //timePicker1.UpdateDate(currentDate.Year, currentDate.Month, currentDate.Day);
            }
            timePicker1.Visibility = ViewStates.Visible;    //(timePicker1.Visibility == ViewStates.Invisible) ? ViewStates.Visible : ViewStates.Invisible;
            timePicker1.BringToFront();
        }

        //private void OnDateChanged(object sender, DatePicker.IOnDateChangedListener obj)
        //{
        //    Task task = (Task)sender;

        //    lblDateTime.Text = datePicker1.DayOfMonth.ToString().PadLeft(2, '0') + "/" + datePicker1.Month.ToString().PadLeft(2, '0') + "/" + datePicker1.Year.ToString().PadLeft(4, '0');
            
        //    Toast.MakeText(this, task.getDescription() + " - " + lblDateTime.Text, ToastLength.Long).Show();
        //}

        private void DeleteRecord(Task currentTask)
        {
            MainActivity.DBTaskReminder.DB.Delete<TBL_Tasks>(currentTask.getTaskID());  // "ID==" +currentTask.getTaskID().ToString(), null);

            MainActivity.isShowTimerReminder = true;
            MainActivity.MainMessageText = "נמחק";
            MainActivity.showGlobalMessageOnToast();

            IsSaveNeededBeforeExit = false;
            
            inputIntent.PutExtra("Delete", "true");

            SetResult(Result.Ok, inputIntent);

            if (OnExitResult != null)
            {
                OnExitResult(MainActivity.SCREEN_TASK_DETAILS_DELETE, Result.Ok, inputIntent);
            }

            Finish();
        }

        private bool SaveRecord(Task task)
        {
            bool result = true;
            long recorsWasEffected = 0;
            TBL_Tasks item = null;



            IsSaveNeededBeforeExit = false;

            if (!IsDataWasChanged())
            {
                return result;
            }

            SetObjectByControls();

            

            // only insert the data if it doesn't already exist
            if (isNewMode)
            {
                item = new TBL_Tasks();
            }
            else
            {
                item = MainActivity.DBTaskReminder.DB.Get<TBL_Tasks>(task.getTaskID());
                //item = (TBL_Tasks)MainActivity.DBTaskReminder.GetRecordByID(item.ID, MainActivity.DB_TABLE_NAME);
                //item = (TBL_Tasks)CurrentTask.TableRecord;
            }


            try
            {
                item.Title = task.getTitle();
                item.Description = task.getDescriptionWithHtml();
                item.DateDue = task.getDate_due() + " " + task.getTime_due();

                if (isNewMode)
                {
                    recorsWasEffected = MainActivity.DBTaskReminder.RecordInser(item, MainActivity.DB_TABLE_NAME);
                    //MainActivity.DBTaskReminder.DB.Insert(item);
                }
                else
                {
                    recorsWasEffected = MainActivity.DBTaskReminder.RecordUpdate(item);
                    item = MainActivity.DBTaskReminder.DB.Get<TBL_Tasks>(task.getTaskID());
                    task.TableRecord = item;
                    // Set Task object in array
                    //List<KeyValuePair<String, String>> values = MainActivity.SetTaskValuesForDB(CurrentTask);
                    //recorsWasEffected = MainActivity.DBTaskReminder.RecordUpdate(MainActivity.DB_TABLE_NAME, values, new object[] { CurrentTask.getTaskID() });
                }



                // Raise the event to the Caller
                if (recorsWasEffected>0)
                {
                    if (isNewMode)
                    {
                        Object recordUpdated = MainActivity.DBTaskReminder.getLastRecord(MainActivity.DB_TABLE_NAME);
                        CurrentTask.setTaskID(((TBL_Tasks)recordUpdated).ID);
                    }

                    MainActivity.CurrentTask = CurrentTask;
                    MainActivity.isShowTimerReminder = true;
                    MainActivity.MainMessageText = "נשמר";

                    //Toast.MakeText(this, "נשמר", ToastLength.Long).Show();

                    inputIntent.PutExtra("Result", "true");

                    SetResult(Result.Ok, inputIntent);

                    //if (OnSaveButton != null)
                    //{
                    //    OnSaveButton(null, EventArgs.Empty);
                    //}
                    // Second option to - Raise the event to the Caller
                    if (OnExitResult != null)
                    {
                        OnExitResult(MainActivity.SCREEN_TASK_DETAILS_SAVED, Result.Ok, inputIntent);
                    }
                }
                else
                {
                    Toast.MakeText(this, "השמירה נכשלה", ToastLength.Long).Show();
                    SetResult(Result.Canceled, inputIntent);
                }


                Finish();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return result;
        }

        private bool IsDataWasChanged()
        {
            if ((string.IsNullOrEmpty(txtDetailsTitle.Text.Trim()) && string.IsNullOrEmpty(txtDetailsDescription.Text.Trim())) || 
                (txtDetailsTitle.Text==bkuTitle &&
                 txtDetailsDescription.Text==bkuDescription &&
                 lblDateTime.Text == bkuDateDue))
            {
                return false;
            }

            return true;
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

            if (!txtDetailsDate.Text.Trim().Equals(""))
            {
                CurrentTask.setDate_due(lblDateTime.Text.Substring(0, 10));
            }
            else
            {
                CurrentTask.setDate_due("");
            }
            if (!txtDetailsTime.Text.Trim().Equals(""))
            {
                CurrentTask.setTime_due(lblDateTime.Text.Trim().Substring(11, 5));
            }
            else
            {
                CurrentTask.setTime_due("");
            }

            CurrentTask.setDate_last_update(Utils.Utils.getDateFormattedString(Utils.Utils.GetDateNow()));
        }

        private void SetControlsByObject()
        {
            txtDetailsTitle.Text = bkuTitle = CurrentTask.getTitle();
            txtDetailsDescription.Text = bkuDescription = CurrentTask.getDescriptionWithHtml();
            lblDateTime.Text = bkuDateDue = CurrentTask.getDate_due() + " " + CurrentTask.getTime_due();
            txtDetailsDate.Text = CurrentTask.getDate_due();
            txtDetailsTime.Text = CurrentTask.getTime_due();
        }

        protected override void OnDestroy()
        {
            if (IsSaveNeededBeforeExit)
            {
                SaveRecord(CurrentTask);
            }

            //Toast.MakeText(this, "Close Note", ToastLength.Long).Show();

            base.OnDestroy();
        }


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
