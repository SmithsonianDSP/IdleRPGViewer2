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
using Android.Util;

namespace IdleRPGViewer2
{
    [Activity(Label = "Settings")]
    public class SettingsActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.SettingsPage);

            LoadUserSettings();

            var prefs = GetSharedPreferences("IdleRPGViewer2", FileCreationMode.Private);

            var switchNotifications = FindViewById<Switch>(Resource.Id.notificationsSwitch1);
            switchNotifications.CheckedChange += (sender, args) => { NotificationsChanged(); };

            var buttonNotificationFreq = FindViewById<Button>(Resource.Id.settingsCheckFrequencyButton);
            buttonNotificationFreq.Click += (s, arg) =>
            {
                var menu = new PopupMenu(this, buttonNotificationFreq);
                menu.Inflate(Resource.Menu.checkfreq_popup_menu);

                menu.MenuItemClick += (s1, arg1) =>
                {

                    switch (arg1.Item.ItemId)
                    {
                        case Resource.Id.check5minutes:
                            SetCheckerFrequency(5);
                            break;
                        case Resource.Id.check10minutes:
                            SetCheckerFrequency(10);
                            break;
                        case Resource.Id.check15minutes:
                            SetCheckerFrequency(15);
                            break;
                        case Resource.Id.check30minutes:
                            SetCheckerFrequency(30);
                            break;
                        case Resource.Id.check60minutes:
                            SetCheckerFrequency(60);
                            break;
                        default:
                            break;
                    }

                    buttonNotificationFreq.Text = arg1.Item.TitleFormatted.ToString();
                };

                menu.Show();
            };
            buttonNotificationFreq.Enabled = switchNotifications.Checked;

            var switchOnlyIfIGainXP = FindViewById<Switch>(Resource.Id.notificationsOnlyGained);
            switchOnlyIfIGainXP.CheckedChange += (sender, args) =>
            {
                var prefsEdit = prefs.Edit();
                prefsEdit.PutBoolean("OnlyIfXPGained", switchOnlyIfIGainXP.Checked);
                prefsEdit.Commit();
            };
            switchOnlyIfIGainXP.Enabled = switchNotifications.Checked;

            var switchIsYammajr = FindViewById<Switch>(Resource.Id.notificationUsername);
            switchIsYammajr.CheckedChange += (sender, args) =>
            {
                var prefsEdit = prefs.Edit();
                prefsEdit.PutString("NotifyUsername", (switchIsYammajr.Checked ? "yammajr" : "SmithsonianDSP"));
                prefsEdit.Commit();
            };
            switchIsYammajr.Enabled = switchNotifications.Checked;
        }

        protected void NotificationsChanged()
        {
            // If set to disabled: disable other related controls & cancel any scheduled jobs.
            // If set to enabled:  enable other related controls & schedule jobs
            var switchCheckerOn = FindViewById<Switch>(Resource.Id.notificationsSwitch1);

            var prefs = Application.Context.GetSharedPreferences("IdleRPGViewer2", FileCreationMode.Private);
            var prefsEditor = prefs.Edit();
            prefsEditor.PutBoolean("ChecksEnabled", switchCheckerOn.Checked);
            prefsEditor.Commit();

            var buttonNotificationFreq = FindViewById<Button>(Resource.Id.settingsCheckFrequencyButton);
            buttonNotificationFreq.Enabled = switchCheckerOn.Checked;

            var switchOnlyIfIGainXP = FindViewById<Switch>(Resource.Id.notificationsOnlyGained);
            switchOnlyIfIGainXP.Enabled = switchCheckerOn.Checked;

            var switchIsYammajr = FindViewById<Switch>(Resource.Id.notificationUsername);
            switchIsYammajr.Enabled = switchCheckerOn.Checked;

            if (switchCheckerOn.Checked)
            {
                CheckerJobService.ScheduleJob();
            }
            else
            {
                CheckerJobService.CancelJobs();
            }
        }


        protected void LoadUserSettings()
        {
            var prefs = Application.Context.GetSharedPreferences("IdleRPGViewer2", FileCreationMode.Private);

            var switchCheckerOn = FindViewById<Switch>(Resource.Id.notificationsSwitch1);
            switchCheckerOn.Checked = prefs.GetBoolean("ChecksEnabled", false);

            var buttonNotificationFreq = FindViewById<Button>(Resource.Id.settingsCheckFrequencyButton);
            buttonNotificationFreq.Text = string.Format("{0} minutes", prefs.GetInt("CheckFrequency", 15));

            var switchOnlyIfIGainXP = FindViewById<Switch>(Resource.Id.notificationsOnlyGained);
            switchOnlyIfIGainXP.Checked = prefs.GetBoolean("OnlyIfXPGained", false);

            var switchIsYammajr = FindViewById<Switch>(Resource.Id.notificationUsername);
            switchIsYammajr.Checked = (prefs.GetString("NotifyUsername", "SmithsonianDSP") == "yammajr");

            Log.Debug("IdleRPG", "LoadUserSettings: ChecksEnabled: {0}, CheckFrequency: {1}, OnlyIfXPGained: {2}, NotifyUsername: {3}", 
                prefs.GetBoolean("ChecksEnabled", false), 
                prefs.GetInt("CheckFrequency", 15), 
                prefs.GetBoolean("OnlyIfXPGained", false), 
                prefs.GetString("NotifyUsername", "SmithsonianDSP"));
        }

        protected void SetCheckerFrequency(int checkFrequency)
        {
            var prefs = Application.Context.GetSharedPreferences("IdleRPGViewer2", FileCreationMode.Private);
            var prefsEditor = prefs.Edit();
            prefsEditor.PutInt("CheckFrequency", checkFrequency);
            prefsEditor.Commit();

            CheckerJobService.ScheduleJob();
        }

    }
}