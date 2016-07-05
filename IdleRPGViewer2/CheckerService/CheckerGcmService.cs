using System;

using Android.App;
using Android.Content;
using Android.Gms.Gcm;
using Android.Util;

namespace IdleRPGViewer2
{
    [Service(Exported = true, Permission = "com.google.android.gms.permission.BIND_NETWORK_TASK_SERVICE")]
    [IntentFilter(new String[] { "com.google.android.gms.gcm.ACTION_TASK_READY" })]
    class CheckerGcmService : GcmTaskService
    {
        public override int OnRunTask(TaskParams param)
        {
            try
            {
                Log.Info("IdleRPG", "CheckerGcmService : OnTaskRun");
                var events = GetEventRows.GetCheckerEvents();
                Log.Info("IdleRPG", "Events Checked");

                if (events.Count > 0)
                {
                    Log.Info("IdleRPG", "New Events Found");
                    CheckerNotification.NotifyNewEvent(Application.Context, events.ToArray());
                }
                

                CheckerService.SaveLastCheckedDateTime();
                Log.Info("IdleRPG", "Checked DateTime Saved");
                CheckerService.ScheduleTask();

                return GcmNetworkManager.ResultSuccess;
            }
            catch (Exception ex)
            {
                Log.Error("IdleRPG", ex.Message);
                return GcmNetworkManager.ResultReschedule;
            }
        }
    }
}