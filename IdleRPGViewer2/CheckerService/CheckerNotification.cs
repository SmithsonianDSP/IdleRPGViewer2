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

        private static readonly int EventNotificationId = 1000;

        public static void NotifyNewEvent(Context packageContext, EventRow[] eventrow = null)
        {

            // Construct a back stack for cross-task navigation:
            TaskStackBuilder stackBuilder = TaskStackBuilder.Create(packageContext);
            stackBuilder.AddParentStack(Java.Lang.Class.FromType(typeof(MainActivity)));

            Intent resultIntent = new Intent(packageContext, typeof(MainActivity));
            stackBuilder.AddNextIntent(resultIntent);

            // Create the PendingIntent with the back stack:            
            PendingIntent resultPendingIntent =
                stackBuilder.GetPendingIntent(0, (int)PendingIntentFlags.UpdateCurrent);


            StringBuilder sb = new StringBuilder();

            //build the notification text
            foreach (var rw in eventrow)
            {
                sb.AppendLine(rw.EventText);
            }

            

            // Build the notification:
            NotificationCompat.Builder builder = new NotificationCompat.Builder(packageContext)
                .SetAutoCancel(true)                        // Dismiss from the notif. area when clicked
                .SetContentIntent(resultPendingIntent)      // Start 2nd activity when the intent is clicked.
                .SetContentTitle("New IdleRPG XP Event")    // Set its title
                .SetNumber(eventrow.Count())                // Display the count in the Content Info
                .SetSmallIcon(Resource.Drawable.Icon04)     // Display this icon
                .SetContentText(sb.ToString());             // The message to display.

            NotificationCompat.InboxStyle inboxStyle = new NotificationCompat.InboxStyle();
            inboxStyle.SetBigContentTitle("New IdleRPG Events:");

            foreach (var rw in eventrow)
            {
                inboxStyle.AddLine(rw.EventText);
            }

            builder.SetStyle(inboxStyle);

            // Finally, publish the notification:
            NotificationManager notificationManager = (NotificationManager)Application.Context.GetSystemService(Context.NotificationService);

            var notification = builder.Build();
            notificationManager.Notify(EventNotificationId, notification);

        }
    }
}