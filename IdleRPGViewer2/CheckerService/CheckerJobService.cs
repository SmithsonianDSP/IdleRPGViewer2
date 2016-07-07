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
        static string logTag = "IdleRPG_JS";
        static int jobNum = 1000;

        //static public readonly int jobIntervalMinutes = 10;
        //static readonly int jobIntervalMs = (jobIntervalMinutes * 60 * 1000);  // Convert to milliseconds

        public override void OnCreate()
        {
            base.OnCreate();
            Log.Debug(logTag, "CheckerJobService created");
        }

        public override void OnDestroy()
        {
            Log.Debug(logTag, "CheckerJobService destroyed");
            base.OnDestroy();
        }

        public override StartCommandResult OnStartCommand(Intent intent, Android.App.StartCommandFlags flags, int startId)
        {
            Log.Debug(logTag, "OnStartCommand called at {2}, flags={0}, startid={1}", flags, startId, DateTime.UtcNow);
            InitializeCheckerService();
            return StartCommandResult.Sticky;
        }

        public override bool OnStartJob(JobParameters args)
        {

            try
            {
                Log.Debug(logTag, "CheckerJobService : OnStartJob");
                var events = GetEventRows.GetCheckerEvents();
                Log.Info(logTag, "Events Checked");

                if (events.Count > 0)
                {
                    Log.Info(logTag, "New Events Found");
                    CheckerNotification.NotifyNewEvent(Application.Context, events.ToArray());
                }

                SaveLastCheckedDateTime();

                JobFinished(args, false);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(logTag, ex.Message);
                JobFinished(args, true);
                return false;
            }
        }

        public static void InitializeCheckerService()
        {
            try
            {
                Log.Info(logTag, "CheckerJobService : InitializeCheckerService");

                var jss = (JobSchedulerType)Application.Context.GetSystemService(JobSchedulerService);
                Log.Info(logTag, "Current Pending Jobs: {0}", jss.AllPendingJobs.Count);
                //jss.CancelAll();

                ScheduleJob();

                Log.Info(logTag, "CheckerJobService : InitializeCheckerService Completed");

                Toast.MakeText(Application.Context, "IdleRPG Checker Service Initialized", ToastLength.Long).Show();
            }
            catch (Exception ex)
            {
                Log.Error(logTag, "InitalizeCheckerService " + ex.Message);
                Toast.MakeText(Application.Context, "IdleRPG Checker Service Exception: " + ex.Message, ToastLength.Long).Show();
            }
        }



        public static void CancelJobs()
        {
            Log.Info(logTag, "Cancelling all scheduled jobs");
            var jss = (JobSchedulerType)Application.Context.GetSystemService(JobSchedulerService);
            Log.Info(logTag, "Current Pending Jobs: {0}", jss.AllPendingJobs.Count);
            jss.CancelAll();
            Log.Info(logTag, "All pending jobs canceled");
        }


        public static void ScheduleJob()
        {
            Log.Info(logTag, "CheckerJobService : ScheduleJob - Scheduling Job");

            var compName = new ComponentName(Application.Context, Java.Lang.Class.FromType(typeof(CheckerJobService)));

            var prefs = Application.Context.GetSharedPreferences("IdleRPGViewer2", FileCreationMode.Private);
            var jobIntervalMinutes = prefs.GetInt("CheckFrequency", 15);    // Get the CheckFrequency from the user settings
            var jobIntervalMs = (jobIntervalMinutes * 60 * 1000);           // Convert to milliseconds

            var myTask = new JobInfo.Builder(jobNum, compName)
                .SetPeriodic(jobIntervalMs)
                .SetBackoffCriteria((jobIntervalMs / 100), BackoffPolicy.Linear)
                .SetPersisted(true)
                .SetRequiredNetworkType(NetworkType.Any)
                .SetRequiresDeviceIdle(false)
                .Build();

            var jss = (JobSchedulerType)Application.Context.GetSystemService(JobSchedulerService);
            var status = jss.Schedule(myTask);

            Log.Info(logTag, "CheckerJobService : ScheduleJob - Job Scheduled " + (status == JobSchedulerType.ResultSuccess ? "Success" : "Failure"));
        }



        public override bool OnStopJob(JobParameters args)
        {
            Log.Debug(logTag, "CheckerJobService : OnJobStop");
            return true;
        }



        static void SaveLastCheckedDateTime()
        {
            var prefs = Application.Context.GetSharedPreferences("IdleRPGViewer2", FileCreationMode.Private);
            var prefEditor = prefs.Edit();
            prefEditor.PutString("LastChecked", DateTimeOffset.Now.ToString());
            prefEditor.Commit();

            Log.Debug(logTag, "Saved LastChecked DateTime: {0}", DateTimeOffset.Now.ToString());
        }

        public static DateTimeOffset GetLastCheckedDateTime()
        {
            var prefs = Application.Context.GetSharedPreferences("IdleRPGViewer2", FileCreationMode.Private);
            var lastCheckedStr = prefs.GetString("LastChecked", DateTimeOffset.MinValue.ToString());
            return DateTimeOffset.Parse(lastCheckedStr);
        }

    }

}
