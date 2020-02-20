using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Util;
using System.Threading;
using Timer = System.Timers.Timer;
using Android.Text;

namespace ProjTaskReminder.Model
{
    public class Task 
    {
        private int taskID;
        private string title;
        private string description;
        private string descriptionPure;       //CharSequence////Editable
        private string date_due;
        private string time_due;
        private string repeat;
        private string date_last_update;
        private string background_color;
        private Boolean IsArchive;
        private Boolean isSelected;

        //private Thread timer;
        private System.Timers.Timer timer;
        //private Java.Util.Timer timer;
        private TimerTask timer_task;
        private string text;


        public Task()
        {
            date_due = "";
            time_due = "";
            repeat = "פעם אחת";
            description = "";
            descriptionPure = "";
            IsArchive = false;
        }

        public int getTaskID()
        {
            return taskID;
        }

        //public Thread getTimer()   // System.Timers.Timer | Java.Util.Timer
        //{
        //    return timer;
        //}

        public void setTimer(System.Threading.Thread timer)  //Java.Util.Timer timer)
        //public void setTimer(System.Timers.Timer timer)  //Java.Util.Timer timer)
        {
            //this.timer = (timer;
        }

        public System.Timers.Timer getTimer()  // | Java.Util.Timer
        {
            return this.timer;
        }

        public void setTimer(System.Timers.Timer timer)  //Java.Util.Timer timer)
        {
            this.timer = timer;
        }

        public TimerTask getTimer_task()
        {
            return timer_task;
        }

        public void setTimer_task(TimerTask timer_task)
        {
            this.timer_task = timer_task;
        }

        public string getTitle()
        {
            return title;
        }

        public void setTitle(string title)
        {
            this.title = title;
        }

        public string getDescriptionWithHtml()
        {
            description = description.Replace("ltr", "rtl");

            description = trimSpannedText(description);

            return description;
        }

        //public string getDescription()
        //{
        //    return description;
        //}

        public SpannedString getDescription()
        {
            int pos1 = -1;
            int pos2 = -1;



            //Log.d("Task - getDescription - HtmlScript before", "***" + description + "***");
            //Log.d("Task object: getDescription Flat", "*"+descriptionPure+"*");

            // TODO: Bug in Html.fromHtml(), erase font-size changes
            SpannedString spanned = (SpannedString)Html.FromHtml(description);

            SpannedString tmpSpanned=null;       // = richTextReInsertFontSize(description, spanned);

            if (tmpSpanned != null)
            {
                spanned = tmpSpanned;
            }
            else
            {
                spanned = Utils.Utils.trimSpannedText(spanned);
            }

            if (spanned != null)
            {
                //Log.d("Task - getDescription - HtmlScript after", "***" + Html.toHtml(spanned) + "***");
            }

            return spanned;
        }

        //private Spanned richTextReInsertFontSize(string originalDescription, Spanned originalSpanned)
        //{
        //    Spanned spanned = null;
        //    int pos1 = -1;
        //    int pos2 = -1;
        //    int pos3 = -1;



        //    string wordToCheck = "<span style=" + string.valueOf((char)34) + "font-size";
        //    pos1 = originalDescription.indexOf(wordToCheck);

        //    if (pos1 == -1)
        //    {
        //        return originalSpanned;
        //    }
        //    pos2 = pos1 + originalDescription.substring(pos1).indexOf(">");
        //    //string kkk = originalDescription.substring(pos1, pos2+1);
        //    //Log.d("Task - getDescription - Pre", "***"+kkk+"***");
        //    //string tmp =originalDescription.replace(kkk, "<font size="+string.valueOf((char)34)+"27"+string.valueOf((char)34)+">");
        //    //Log.d("Task - getDescription - Pre", "***"+tmp+"***");
        //    pos3 = pos2 + originalDescription.substring(pos2).indexOf("<");
        //    string word = originalDescription.substring(pos2 + 1, pos3);
        //    //kkk = originalDescription.substring(pos3+word.length());
        //    //Log.d("Task - getDescription - post", "***"+kkk+"***");
        //    //        tmp = originalDescription.substring(0, pos1) + "<font size="+
        //    //                                            string.valueOf((char)34)+"27"+string.valueOf((char)34)+">" +word+"</font>"+
        //    //                                            originalDescription.substring(pos3+word.length()+5);
        //    //Log.d("Task - getDescription - Final", "***"+tmp+"***");
        //    //spanned = Html.fromHtml(tmp);

        //    //pos3 = pos3 + 5;    //word.length();    // <span>
        //    Log.d("Task - getDescription - Word", "***" + word + "***");

        //    CharSequence currentText = originalDescription.subSequence(0, originalDescription.length());

        //    Log.d("Task - getDescription - Original", "***" + currentText + "***");

        //    if (originalDescription.substring(0, 20).indexOf("<p dir") > -1)
        //    {
        //        string lead = "<p dir=" + string.valueOf((char)34) + "rtl" + string.valueOf((char)34) + ">";
        //        originalDescription = originalDescription.replace(lead, "");
        //        originalDescription = originalDescription.substring(0, originalDescription.length() - 4);
        //        Log.d("Task - getDescription - There is lead", "***" + originalDescription + "***");
        //        pos3 = pos3 - 6;
        //    }

        //    pos1 = originalDescription.indexOf(wordToCheck);

        //    List<CharSequence> splitedStrings = new ArrayList<>();
        //    splitedStrings.add(originalDescription.subSequence(0, pos1));
        //    splitedStrings.add(word);   //currentText.subSequence(pos1, pos2));
        //    splitedStrings.add(originalDescription.subSequence(pos3, originalDescription.length())); // - 4));  // TODO: lenght of '</p>'

        //    Log.d("Task - getDescription - part 1", "***" + splitedStrings.get(0) + "***");
        //    Log.d("Task - getDescription - part 2", "***" + splitedStrings.get(1) + "***");
        //    Log.d("Task - getDescription - part 3", "***" + splitedStrings.get(2) + "***");

        //    SpannableString spannedBefore = new SpannableString(splitedStrings.get(0));
        //    SpannableString spannedMiddle = new SpannableString(splitedStrings.get(1));
        //    SpannableString spannedAfter = new SpannableString(splitedStrings.get(2));

        //    AbsoluteSizeSpan absoluteSizeSpan = new AbsoluteSizeSpan(30, true);

        //    spannedMiddle.setSpan(absoluteSizeSpan, 0, spannedMiddle.length(), Spanned.SPAN_EXCLUSIVE_EXCLUSIVE);

        //    //spanned = spannedMiddle + spannedMiddle;
        //    SpannableStringBuilder builder = new SpannableStringBuilder();
        //    builder.append(spannedBefore);
        //    builder.append(spannedMiddle);
        //    builder.append(spannedAfter);

        //    SpannableString spannedTest = new SpannableString(originalSpanned);
        //    //Editable editable = (Editable)spannedTest;
        //    CharSequence ggg = spannedTest.subSequence(0, spannedTest.length());
        //    Log.d("Task - getDescription - Find", "***" + ggg + "***");
        //    pos1 = ggg.toString().indexOf(word);
        //    Log.d("Task - getDescription - Direct", string.valueOf(pos1) + "  ***" + spannedTest.toString() + "***");
        //    //pos1 = 4;   //spannedTest.toString().indexOf(word);
        //    spannedTest.setSpan(absoluteSizeSpan, pos1, pos1 + word.length(), Spanned.SPAN_EXCLUSIVE_EXCLUSIVE);

        //    spanned = (Spanned)spannedTest;  // builder;

        //    Log.d("Task - getDescription - Finnalll", "***" + Html.toHtml(spanned) + "***");

        //    return spanned;
        //}

        public string getDescriptionPure()
        {
            //Log.d("Task object: getDescriptionPure", "*"+descriptionPure+"*");
            return this.descriptionPure;
        }

        public void setDescriptionAsIs(string descriptionWitHtmlTags)
        {
            this.description = trimSpannedText(descriptionWitHtmlTags);
        }

        public void setDescription(string descriptionWitHtmlTags)
        {

            //if (Build.VERSION.SDK_INT >= Build.VERSION_CODES.N)
            //{
            // Html.FROM_HTML_MODE_COMPACT));
            // Html.FROM_HTML_MODE_COMPACT
            // Html.FROM_HTML_MODE_LEGACY - we are using this flag to give a consistent behaviour
            // Html.TO_HTML_PARAGRAPH_LINES_CONSECUTIVE );
            // Html.TO_HTML_PARAGRAPH_LINES_CONSECUTIVE

            this.description = trimSpannedText(descriptionWitHtmlTags);

            // Make it Flat string
//             Spanned htmlStr = Html.fromHtml(this.description);

///            this.descriptionPure = htmlStr.toString().trim();

            //Log.d("Task object: setDescription Direct WithHtml", "*"+description+"*");
            //Log.d("Task object: setDescription Direct Flat", "*"+descriptionPure+"*");
        }

        //public void setDescription(Spanned spannedDescription)  // gettype  'Editable' / 'SpannableString'
        //{
        //    string htmlStr = Html.toHtml(spannedDescription);       //, Html.TO_HTML_PARAGRAPH_LINES_INDIVIDUAL);     //Html.TO_HTML_PARAGRAPH_LINES_CONSECUTIVE );

        //    htmlStr = trimSpannedText(htmlStr);

        //    htmlStr = htmlStr.replace("ltr", "rtl");

        //    this.description = htmlStr;

        //    this.descriptionPure = spannedDescription.toString().trim();

        //    Log.d("Task object: setDescription Html", "***" + description + "***");
        //    //Log.d("Task object: setDescription Html, descriptionPure", "*"+descriptionPure+"*");
        //}


        public void setDescriptionPure(string puredString)
        {
            descriptionPure = puredString;
        }

        public string getDate_due()
        {
            return date_due.Trim();
        }

        public void setDate_due(string date_due)
        {
            if (date_due != null)
            {
                this.date_due = date_due.Trim();
            }
            else
            {
                this.date_due = string.Empty;
            }
        }

        public string getDateOnly_last_update()
        {
            return date_last_update.Substring(0, 10);
        }

        public string getDate_last_update()
        {
            return date_last_update;
        }

        public void setDate_last_update(string date_last_update)
        {
            this.date_last_update = date_last_update;
        }

        public string getBackgroundColor()
        {
            return background_color;
        }

        public void setBackgroundColor(string BackgroundColor)
        {
            this.background_color = BackgroundColor;
        }

        public string getRepeat()
        {
            return repeat;
        }

        public void setRepeat(string repeatNew)
        {
            repeat = repeatNew;
        }


        public string getTime_due()
        {
            return time_due.Trim();
        }

        public void setTime_due(string time)
        {
            this.time_due = time.Trim();
            //string.format("%02d", hour) + ":" + string.format("%02d", minute)
        }

        public void setTaskID(int taskID)
        {
            this.taskID = taskID;
        }

        public Boolean getIsArchive()
        {
            return this.IsArchive;
            //return (this.IsArchive) ? "1" :"0";
        }

        public void setIsArchive(Boolean value)
        {
            this.IsArchive = value;
        }

        public DateTime? getDate()
        {
            string strDate;
            string strTime;
            DateTime? result=null;


            if (!((this.date_due != null && !this.getDate_due().Equals("")) && (this.time_due != null && !this.getTime_due().Equals(""))))
            {
                return result;
            }

            strDate = this.getDate_due();
            strTime = this.getTime_due();

            //Log.d("Task - getDate - Date: ", "*"+strDate+" "+strTime+"*");

            return Utils.Utils.getDateFromString(strDate + " " + strTime);
        }

        private string trimSpannedText()
        {
            return trimSpannedText();
        }

        private string trimSpannedText(string text)
        {
            this.text = text;
            int pos = text.LastIndexOf("</p>");

            if (pos > -1)
            {
                //Log.d("Task object: Trim pos /n", string.valueOf(text.lastIndexOf("\n")));
                //Log.d("Task object: Trim pos </p>", string.valueOf(pos));
                text = text.Substring(0, pos + 4);    // Without spaces after this word
            }

            return text;
        }

        public Boolean getIsSelected()
        {
            return isSelected;
        }

        public void setIsSelected(Boolean value)
        {
            isSelected = value;
        }

        public void resetHtml()
        {
            ///ISpanned htmlStr = Html.FromHtml(this.description);

            // Make it Flat string
            ///this.descriptionPure = htmlStr.toString().trim();
            this.description = this.descriptionPure;
        }

    }
}