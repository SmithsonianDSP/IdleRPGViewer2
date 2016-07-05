using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Collections;

using System.Linq;



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

            Button buttonShow = FindViewById<Button>(Resource.Id.MyButton);
            buttonShow.Click += (sender, e) =>
            {
                var intent = new Intent(this, typeof(DisplayActivity));
                intent.PutExtra("username", filterUser);
                intent.PutExtra("eventfilter", filterEventType);
                intent.PutExtra("daterange", filterDateRange);
                StartActivity(intent);
            };

            Button buttonFilterUser = FindViewById<Button>(Resource.Id.FilterUserNameButton);
            buttonFilterUser.Click += (s, arg) =>
            {
                PopupMenu menu = new PopupMenu(this, buttonFilterUser);
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

            Button buttonFilterEvent = FindViewById<Button>(Resource.Id.FilterEventTypeButton);
            buttonFilterEvent.Click += (s2, arg) =>
            {
                PopupMenu menu = new PopupMenu(this, buttonFilterEvent);
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

            Button buttonDateRange = FindViewById<Button>(Resource.Id.FilterDateRangeButton);
            buttonDateRange.Click += (s, arg) =>
            {
                PopupMenu menu = new PopupMenu(this, buttonDateRange);
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


            Button buttonTest = FindViewById<Button>(Resource.Id.MyButton2);
            buttonTest.Click += (s, arg) =>
            {
                try
                {
                    if (App.Current.checkerService == null)
                    {
                        App.StartCheckerService();
                        Toast.MakeText(this, "Service Started", Android.Widget.ToastLength.Long).Show();
                        
                        CheckerService.SaveLastCheckedDateTime();
                    }
                    else
                    {
                        var lastChecked = CheckerService.GetLastCheckedDateTime().ToLocalTime();
                        var lastCheckedString = lastChecked.Date.ToShortDateString() + " " + lastChecked.DateTime.ToShortTimeString(); 
                        Toast.MakeText(this, "Last Checked: " + lastCheckedString, Android.Widget.ToastLength.Long).Show();
                    }
                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, ex.Message, Android.Widget.ToastLength.Long).Show();
                }
            };



            buttonTest.LongClick += (s, arg) =>
            {
                App.StopCheckerService();
                Toast.MakeText(this, "Service Stopped", Android.Widget.ToastLength.Long).Show();
            };

        }







    }

}

