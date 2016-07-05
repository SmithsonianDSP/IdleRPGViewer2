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
using Android.App.Job;
using Android.Util;

using JobSchedulerType = Android.App.Job.JobScheduler;

namespace IdleRPGViewer2
{
    [Service(Exported = true, Permission = "android.permission.BIND_JOB_SERVICE")]
    public class CheckerJobService : JobService
    {
        static string Tag = "IdleRPG_JS";
        static int JobNum = 1000;

        static public readonly int MinutesWait = 5;
        static readonly int TimerWait = (MinutesWait * 60 * 1000);  // Convert MinutesWait to milliseconds

        public override void OnCreate()
        {
            base.OnCreate();
            Log.Debug(Tag, "CheckerJobService created");
        }

        public override void OnDestroy()
        {
            Log.Debug(Tag, "CheckerJobService destroyed");
            base.OnDestroy();
        }

        public override StartCommandResult OnStartCommand(Intent intent, Android.App.StartCommandFlags flags, int startId)
        {
            Log.Debug(Tag, "OnStartCommand called at {2}, flags={0}, startid={1}", flags, startId, DateTime.UtcNow);
            InitializeCheckerService();
            return StartCommandResult.Sticky;
        }

        public override bool OnStartJob(JobParameters args)
        {

            try
            {
                Log.Debug(Tag, "CheckerJobService : OnStartJob");
                var events = GetEventRows.GetCheckerEvents();
                Log.Info(Tag, "Events Checked");

                if (events.Count > 0)
                {
                    Log.Info(Tag, "New Events Found");
                    CheckerNotification.NotifyNewEvent(Application.Context, events.ToArray());
                }

                SaveLastCheckedDateTime();

                JobFinished(args, false);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(Tag, ex.Message);
                JobFinished(args, true);
                return false;
            }
        }

        public static void InitializeCheckerService()
        {
            try
            {
                Log.Info(Tag, "CheckerJobService : InitializeCheckerService");

                var jss = (JobSchedulerType)Application.Context.GetSystemService(JobSchedulerService);
                Log.Info(Tag, "Current Pending Jobs: {0}", jss.AllPendingJobs.Count);
                //jss.CancelAll();

                ScheduleJob();

                Log.Info(Tag, "CheckerJobService : InitializeCheckerService Completed");

                Toast.MakeText(Application.Context, "IdleRPG Checker Service Initialized", ToastLength.Long).Show();
            }
            catch (Exception ex)
            {
                Log.Error(Tag, "InitalizeCheckerService " + ex.Message);
                Toast.MakeText(Application.Context, "IdleRPG Checker Service Exception: " + ex.Message, ToastLength.Long).Show();
            }
        }

        public static void ScheduleJob()
        {
            Log.Info(Tag, "CheckerJobService : ScheduleJob - Scheduling Job");

            var compName = new ComponentName(Application.Context, Java.Lang.Class.FromType(typeof(CheckerJobService)));

            var myTask = new JobInfo.Builder(JobNum, compName)
                .SetPeriodic(TimerWait)
                .SetBackoffCriteria((TimerWait / 100), BackoffPolicy.Linear)
                .SetPersisted(true)
                .SetRequiredNetworkType(NetworkType.Any)
                .SetRequiresDeviceIdle(false)
                .Build();

            var tm = (JobSchedulerType)Application.Context.GetSystemService(JobSchedulerService);
            var status = tm.Schedule(myTask);
            Log.Info(Tag, "CheckerJobService : ScheduleJob - Job Scheduled " + (status == JobSchedulerType.ResultSuccess ? "Success" : "Failure"));
        }

        public override bool OnStopJob(JobParameters args)
        {
            Log.Debug(Tag, "CheckerJobService : OnJobStop");
            return true;
        }

        static void SaveLastCheckedDateTime()
        {
            var prefs = Application.Context.GetSharedPreferences("IdleRPGViewer2", FileCreationMode.Private);
            var prefEditor = prefs.Edit();
            prefEditor.PutString("LastChecked", DateTimeOffset.Now.ToString());
            prefEditor.Commit();

            Log.Debug(Tag, "Saved LastChecked DateTime: {0}", DateTimeOffset.Now.ToString());
        }

        public static DateTimeOffset GetLastCheckedDateTime()
        {
            var prefs = Application.Context.GetSharedPreferences("IdleRPGViewer2", FileCreationMode.Private);
            var lastCheckedStr = prefs.GetString("LastChecked", DateTimeOffset.MinValue.ToString());
            return DateTimeOffset.Parse(lastCheckedStr);
        }

    }

}
