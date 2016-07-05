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
    public class EventRowAdapter : BaseAdapter<EventRow>
    {
        List<EventRow> items;
        Activity context;
        public EventRowAdapter(Activity context, List<EventRow> items)
            : base()
        {
            this.context = context;
            this.items = items;
        }
        public override long GetItemId(int position)
        {
            return position;
        }
        public override EventRow this[int position]
        {
            get { return items[position]; }
        }
        public override int Count
        {
            get { return items.Count; }
        }
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var item = items[position];
            View view = convertView;
            if (view == null) // no view to re-use, create new
                view = context.LayoutInflater.Inflate(Resource.Layout.customListView, null);

            view.FindViewById<TextView>(Resource.Id.DateText).Text = item.EventDate.LocalDateTime.ToShortDateString() + " " +
                item.EventDate.LocalDateTime.ToShortTimeString();

            view.FindViewById<TextView>(Resource.Id.EventText).Text = item.EventText;

            FormatListItem(item, view.FindViewById<TextView>(Resource.Id.EventText));
            
            return view;
        }

        private void FormatListItem(EventRow item, TextView eventText)
        {
            if (item.EventType == 1)
            {
                eventText.SetBackgroundColor(Android.Graphics.Color.Yellow);
                eventText.SetTextColor(Android.Graphics.Color.Black);
            }
            else
            {
                eventText.SetBackgroundColor(Android.Graphics.Color.Transparent);

                if (item.EventText.EndsWith("Rare", StringComparison.InvariantCulture))
                {
                    eventText.SetTextColor(Android.Graphics.Color.Yellow);
                }

                else if (item.EventText.EndsWith("Magical", StringComparison.InvariantCulture))
                {
                    eventText.SetTextColor(Android.Graphics.Color.Aqua);
                }

                else if (item.EventText.EndsWith("Epic", StringComparison.InvariantCulture))
                {
                    eventText.SetTextColor(Android.Graphics.Color.Magenta);
                }

                else if (item.EventText.EndsWith("Common", StringComparison.InvariantCulture))
                {
                    eventText.SetTextColor(Android.Graphics.Color.White);
                }

                else
                {
                    eventText.SetTextColor(Android.Graphics.Color.DarkGoldenrod);
                }
            }

        } 


    }
}