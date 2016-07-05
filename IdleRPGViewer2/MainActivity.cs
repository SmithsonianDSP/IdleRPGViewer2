using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Collections;

using System.Linq;
using JobSchedulerType = Android.App.Job.JobScheduler;
using Android.Util;

namespace IdleRPGViewer2
{
    [Activity(Label = "IdleRPG Viewer", MainLauncher = true, Icon = "@drawable/icon02")]
    public class MainActivity : Activity 
    {

        string filterUser = "SmithsonianDSP";
        int filterEventType = -1;
        int filterDateRange = 6;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.Main);

            var buttonShow = FindViewById<Button>(Resource.Id.MyButton);
            buttonShow.Click += (sender, e) =>
            {
                var intent = new Intent(this, typeof(DisplayActivity));
                intent.PutExtra("username", filterUser);
                intent.PutExtra("eventfilter", filterEventType);
                intent.PutExtra("daterange", filterDateRange);
                StartActivity(intent);
            };

            var buttonFilterUser = FindViewById<Button>(Resource.Id.FilterUserNameButton);
            buttonFilterUser.Click += (s, arg) =>
            {
                var menu = new PopupMenu(this, buttonFilterUser);
                menu.Inflate(Resource.Menu.user_popup_menu);

                menu.MenuItemClick += (s1, arg1) =>
                {

                    switch (arg1.Item.ItemId)
                    {
                        case Resource.Id.userfilterAll:
                            filterUser = "*";
                            break;
                        case Resource.Id.userfilterYam:
                            filterUser = "yammajr";
                            break;
                        default:
                            filterUser = "SmithsonianDSP";
                            break;
                    }

                    buttonFilterUser.Text = arg1.Item.TitleFormatted.ToString();
                };

                menu.Show();
            };

            var buttonFilterEvent = FindViewById<Button>(Resource.Id.FilterEventTypeButton);
            buttonFilterEvent.Click += (s2, arg) =>
            {
                var menu = new PopupMenu(this, buttonFilterEvent);
                menu.Inflate(Resource.Menu.event_type_popup_menu);

                menu.MenuItemClick += (s1, arg1) =>
                {
                    switch (arg1.Item.ItemId)
                    {
                        case Resource.Id.eventfilterItems:
                            filterEventType = 2;
                            break;
                        case Resource.Id.eventfilterXP:
                            filterEventType = 1;
                            break;
                        default:
                            filterEventType = -1;
                            break;
                    }

                    buttonFilterEvent.Text = arg1.Item.TitleFormatted.ToString();
                };
                menu.Show();
            };

            var buttonDateRange = FindViewById<Button>(Resource.Id.FilterDateRangeButton);
            buttonDateRange.Click += (s, arg) =>
            {
                var menu = new PopupMenu(this, buttonDateRange);
                menu.Inflate(Resource.Menu.daterange_popup_menu);

                menu.MenuItemClick += (s1, arg1) =>
                {
                    switch (arg1.Item.ItemId)
                    {
                        case Resource.Id.daterange12hours:
                            filterDateRange = 12;
                            break;
                        case Resource.Id.daterange24hours:
                            filterDateRange = 24;
                            break;
                        case Resource.Id.daterange48hours:
                            filterDateRange = 48;
                            break;
                        default:
                            filterDateRange = 6;
                            break;
                    }

                    buttonDateRange.Text = arg1.Item.TitleFormatted.ToString();
                };
                menu.Show();
            };


            var buttonTest = FindViewById<Button>(Resource.Id.MyButton2);
            buttonTest.Click += (s, arg) =>
            {
                try
                {
                    var jss = (JobSchedulerType)Application.Context.GetSystemService(JobSchedulerService);

                    if (jss.AllPendingJobs.Count() == 0) 
                        CheckerJobService.InitializeCheckerService();
                        //CheckerGcmService.InitializeCheckerService();

                    var lastChecked = CheckerJobService.GetLastCheckedDateTime().ToLocalTime();
                    var lastCheckedString = lastChecked.Date.ToShortDateString() + " " + lastChecked.DateTime.ToShortTimeString(); 
                    Toast.MakeText(this, "Last Checked: " + lastCheckedString, Android.Widget.ToastLength.Long).Show();
                 
                }
                catch (Exception ex)
                {
                    Log.Error("IdleRPG", ex.Message);
                    Toast.MakeText(this, ex.Message, Android.Widget.ToastLength.Long).Show();
                }
            };


            buttonTest.LongClick += (s, arg) =>
            {
                var jss = (JobSchedulerType)GetSystemService(JobSchedulerService);
                jss.CancelAll();

                Toast.MakeText(this, "All Jobs Stopped", Android.Widget.ToastLength.Long).Show();
            };

        }

    }

}

