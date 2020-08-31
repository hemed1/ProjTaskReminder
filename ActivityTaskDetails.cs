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
        private Button btnSetColor;
        private DatePicker datePicker1;
        private TimePicker timePicker1;

        private string bkuTitle;
        private string bkuDescription;
        private string bkuDateDue;
        private string bkuBackColor;


        public static bool isNewMode;
        private Intent inputIntent;
        private int taskID;
        private bool IsUpdateDialogDate;
        private bool IsSaveNeededBeforeExit = true;


        public static Context context;
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
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_task_details);

            inputIntent = this.Intent;
            if (inputIntent!=null && inputIntent.GetIntExtra("TaskID", 0)==0)
            {
                taskID = inputIntent.GetIntExtra("TaskID", 0);
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
                txtDetailsDescription.Focusable = true;
                txtDetailsDescription.RequestFocus();
                //txtDetailsDescription.RequestFocusFromTouch();
                //MH_Utils.Utils.closeKeyboard();
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
            MH_Utils.Utils.ClientContext = this.ApplicationContext;
            MH_Utils.Utils.ClientActivity = this;

            txtDetailsTitle = FindViewById<EditText>(Resource.Id.txtDetailsTitle);
            txtDetailsDescription = FindViewById<EditText>(Resource.Id.txtDetailsDescription);
            txtDetailsDate = FindViewById<TextView>(Resource.Id.txtDetailsDate);
            txtDetailsTime = FindViewById<TextView>(Resource.Id.txtDetailsTime);
            lblDateTime = FindViewById<EditText>(Resource.Id.lblDateTime);
            btnSave = FindViewById<Button>(Resource.Id.btnSave);
            btnSetDate = FindViewById<Button>(Resource.Id.btnSetDate);
            btnSetColor = FindViewById<Button>(Resource.Id.btnSetColor);
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

            btnSetColor.Click += (sender, e) =>           //new EventHandler(SaveRecord); 
            {
                SetBackColor();
            };

            //btnSetColor.Click += SetBackColor();

            MH_Utils.Utils.OnColorPickerChanged += Utils_OnColorPickerChanged;

            //datePicker1.SetOnDateChangedListener+=OnDateChanged; 
            //{
            //    sender = CurrentTask;
            //    OnDateChange(sender, null);
            //};

            try
            {
                datePicker1.DateChanged += OnDateChanged; //(sender, e) =>           //new EventHandler(SaveRecord); 
            }
            catch (Exception ex)
            {

            }

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

        [Obsolete]
        private void Utils_OnColorPickerChanged(Android.Graphics.Color colorPicked)  
        {
            string colorStr = colorPicked.ToArgb().ToString();            
            

            txtDetailsDescription.SetBackgroundColor(colorPicked);
            txtDetailsDescription.DrawingCacheBackgroundColor = colorPicked;
        }

        private void SetBackColor()
        {
            MH_Utils.Utils.ShowColorDialog(this);

            //return null;
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
                DateTime currentDate= MH_Utils.Utils.getDateFormatUSA(txtDetailsDate.Text.Trim()).AddMonths(-1);
                datePicker1.UpdateDate(currentDate.Year, currentDate.Month, currentDate.Day);
            }
            datePicker1.Visibility = ViewStates.Visible;
            datePicker1.BringToFront();
        }

        private void OpenTimePicker()
        {
            if (!txtDetailsDate.Text.Trim().Equals("") && !txtDetailsTime.Text.Trim().Equals(""))
            {
                //DateTime currentDate = MH_Utils.Utils.getDateFormatUSA(txtDetailsDate.Text.Trim()+" " + txtDetailsTime.Text.Trim());
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

            MainActivity.isShowTimerReminder = true;

            if (MainActivity.DBTaskReminder.DeleteRecord(currentTask.getTaskID()))
            {
                MainActivity.MainMessageText = "נמחק";
            }
            else
            {
                MainActivity.MainMessageText = "חלה תקלה במחיקה";
            }

            MainActivity.showGlobalMessageOnToast();

            IsSaveNeededBeforeExit = false;
            
            inputIntent.PutExtra("Delete", "true");

            ////SetResult(Result.Ok, inputIntent);

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
                item = (TBL_Tasks)MainActivity.DBTaskReminder.GetRecordByID(task.getTaskID(), MainActivity.DB_TABLE_NAME);
            }


            try
            {
                item.Title = task.getTitle();
                item.Description = task.getDescriptionWithHtml();
                item.DateDue = task.getDate_due() + " " + task.getTime_due();
                item.CardBackColor = task.getBackgroundColor();
                item.LastUpdateDate = task.getDate_last_update();

                if (isNewMode)
                {
                    recorsWasEffected = MainActivity.DBTaskReminder.RecordInser(item, MainActivity.DB_TABLE_NAME);
                    //MainActivity.DBTaskReminder.DB.Insert(item);
                }
                else
                {
                    recorsWasEffected = MainActivity.DBTaskReminder.RecordUpdate(item);
                    item = (TBL_Tasks)MainActivity.DBTaskReminder.GetRecordByID(task.getTaskID(), MainActivity.DB_TABLE_NAME);
                    task.TableRecord = item;
                }


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

                    inputIntent.PutExtra("Result", "true");

                    ////SetResult(Result.Ok, inputIntent);

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

        [Obsolete]
        private bool IsDataWasChanged()
        {
            string descControlColorNow = txtDetailsDescription.DrawingCacheBackgroundColor.ToArgb().ToString();

            if ((string.IsNullOrEmpty(txtDetailsTitle.Text.Trim()) && string.IsNullOrEmpty(txtDetailsDescription.Text.Trim())) || 
                (txtDetailsTitle.Text==bkuTitle &&
                 txtDetailsDescription.Text==bkuDescription &&
                 lblDateTime.Text == bkuDateDue))
            {
                if (!string.IsNullOrEmpty(bkuBackColor) && descControlColorNow != bkuBackColor)
                {
                    return true;
                }
                return false;
            }


            return true;
        }

        //private void OnActivityResult(int requestCode, Result resultCode, Intent data)
        //{
        //    //base.OnActivityReenter(requestCode,resultCode, data);

        //    //FillIn();
        //}

        [Obsolete]
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

            CurrentTask.setDate_last_update(MH_Utils.Utils.getDateFormattedString(MH_Utils.Utils.GetDateNow()));


            Android.Graphics.Color colorDefault = new Android.Graphics.Color(ApplicationContext.GetColor(Resource.Color.details_background_main));
            Android.Graphics.Color colorNow = txtDetailsDescription.DrawingCacheBackgroundColor;
            string colorStr = colorNow.ToArgb().ToString();

            if (colorNow != colorDefault)
            {
                CurrentTask.setBackgroundColor(colorStr);
            }

        }

        [Obsolete]
        private void SetControlsByObject()
        {
            txtDetailsTitle.Text = bkuTitle = CurrentTask.getTitle();
            txtDetailsDescription.Text = bkuDescription = CurrentTask.getDescriptionWithHtml();
            lblDateTime.Text = bkuDateDue = CurrentTask.getDate_due() + " " + CurrentTask.getTime_due();
            txtDetailsDate.Text = CurrentTask.getDate_due();
            txtDetailsTime.Text = CurrentTask.getTime_due();
            //bkuBackColor = "";

            Android.Graphics.Color colorDefault = new Android.Graphics.Color(ApplicationContext.GetColor(Resource.Color.details_background_main));

            if (!string.IsNullOrEmpty(CurrentTask.getBackgroundColor()))
            {
                Android.Graphics.Color colorWhite = new Android.Graphics.Color(ApplicationContext.GetColor(Resource.Color.CardBackgroundColor));
                int colorTask = int.Parse(CurrentTask.getBackgroundColor().Trim());
                Android.Graphics.Color color = new Android.Graphics.Color(colorTask);
                bkuBackColor = colorTask.ToString();
                if (colorTask != colorWhite)
                {
                    txtDetailsDescription.SetBackgroundColor(color);
                    txtDetailsDescription.DrawingCacheBackgroundColor = color;
                }
            }
            else
            {
                txtDetailsDescription.SetBackgroundColor(colorDefault);
                txtDetailsDescription.DrawingCacheBackgroundColor = colorDefault;
                bkuBackColor = colorDefault.ToArgb().ToString();
            }
        }

        protected override void OnDestroy()
        {
            if (IsSaveNeededBeforeExit)
            {
                SaveRecord(CurrentTask);
            }

            base.OnDestroy();
        }


    }

}







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
