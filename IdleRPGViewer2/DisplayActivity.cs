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

namespace IdleRPGViewer2
{
    [Activity(Label = "Display Activity")]
    public class DisplayActivity : ListActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            string userFilter = "SmithsonianDSP";
            int dateFilter = 6;
            int eventFilter = -1;
            EventRow[] items;
            try
            {
                userFilter = Intent.Extras.GetString("username");
                eventFilter = Intent.Extras.GetInt("eventfilter");
                dateFilter = Intent.Extras.GetInt("daterange");
                items = GetEventRows.GetData(user:userFilter, hoursHistory:dateFilter, eventTypes:eventFilter).ToArray();
            }          
            catch (Exception ex)
            {
                var list = new List<EventRow>();
                list.Add(new EventRow{ EventText = ex.Message });
                items = list.ToArray();
            }


            this.ListAdapter = new EventRowAdapter(this, items.ToList());
            this.ListView.ItemLongClick += (sender, e) =>
             {
                 var selectedFromList = items[e.Position].EventText;
                 ClipboardManager clipboard = (ClipboardManager)GetSystemService(ClipboardService);
                 ClipData clip = ClipData.NewPlainText("EventText", selectedFromList);
                 clipboard.PrimaryClip = clip;
                 Toast.MakeText(this, "EventText Copied", Android.Widget.ToastLength.Short).Show();
             };
        }

        //void OnListItemClick(object sender, AdapterView.ItemClickEventArgs e)
        //{
        //    var listView = sender as ListView;
        //    var t = tableItems[e.Position];
        //    Android.Widget.Toast.MakeText(this, t.Heading, Android.Widget.ToastLength.Short).Show();
        //}
    }
}


