using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Support.V4.App;
using TaskStackBuilder = Android.Support.V4.App.TaskStackBuilder;


namespace IdleRPGViewer2
{
    static class CheckerNotification
    {

        static readonly int eventNotificationId = 1000;

        public static void NotifyNewEvent(Context packageContext, EventRow[] eventrow = null)
        {

            // Construct a back stack for cross-task navigation:
            var stackBuilder = TaskStackBuilder.Create(packageContext);
            stackBuilder.AddParentStack(Java.Lang.Class.FromType(typeof(MainActivity)));

            var resultIntent = new Intent(packageContext, typeof(DisplayActivity));
            resultIntent.PutExtra("username", "SmithsonianDSP");
            resultIntent.PutExtra("eventfilter", -1);
            resultIntent.PutExtra("daterange", 6);

            stackBuilder.AddNextIntent(resultIntent);

            // Create the PendingIntent with the back stack:            
            var resultPendingIntent = stackBuilder.GetPendingIntent(0, (int)PendingIntentFlags.UpdateCurrent);

            //build the notification text
            var sb = new StringBuilder();
            foreach (var rw in eventrow)
            {
                sb.AppendLine(rw.EventText);
            }

            
            // Build the notification:
            var builder = new NotificationCompat.Builder(packageContext)
                .SetAutoCancel(true)                        // Dismiss from the notif. area when clicked
                .SetContentIntent(resultPendingIntent)      // Start 2nd activity when the intent is clicked.
                .SetContentTitle("New IdleRPG XP Event")    // Set its title
                .SetNumber(eventrow.Count())                // Display the count in the Content Info
                .SetSmallIcon(Resource.Drawable.Icon04)     // Display this icon
                .SetContentText(sb.ToString());             // The message to display.

            var inboxStyle = new NotificationCompat.InboxStyle();
            inboxStyle.SetBigContentTitle("New IdleRPG Events:");

            foreach (var rw in eventrow)
            {
                inboxStyle.AddLine(rw.EventText);
            }

            builder.SetStyle(inboxStyle);

            // Finally, publish the notification:
            var notificationManager = (NotificationManager)Application.Context.GetSystemService(Context.NotificationService);

            var notification = builder.Build();
            notificationManager.Notify(eventNotificationId, notification);

        }
    }
}