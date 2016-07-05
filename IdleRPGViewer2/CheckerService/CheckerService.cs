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
using System.Threading.Tasks;
using System.Threading;
using Android.Util;
using IdleRPGViewer2;

using JobSchedulerType = Android.App.Job.JobScheduler;
using Export = Java.Interop.ExportAttribute;
using Android.App.Job;
using Android.Text;
using Android.Gms.Gcm;

[Service]
public class CheckerService : Service
{
    static public readonly int MinutesWait = 5;

    static readonly int TimerWait = (MinutesWait * 60 * 1000);  // Convert MinutesWait to milliseconds

    static public DateTimeOffset lastChecked = DateTimeOffset.Now;

    IBinder binder;
    List<EventRow> events;

    public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
    {
        Log.Debug("IdleRPG", "OnStartCommand called at {2}, flags={0}, startid={1}", flags, startId, DateTime.UtcNow);

        InitializeCheckerService();

        return StartCommandResult.Sticky;
    }

    public override IBinder OnBind(Intent intent)
    {
        binder = new CheckerServiceBinder(this);
        return binder;
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        events = null;
    }



    public void InitializeCheckerService()
    {
        try
        {
            Log.Info("IdleRPG", "CheckerService : InitializeCheckerService");

            GcmNetworkManager.GetInstance(Application.Context).CancelAllTasks(Java.Lang.Class.FromType(typeof(CheckerGcmService)));

            ScheduleTask();

            Log.Debug("IdleRPG", "CheckerService : InitializeCheckerService Completed");

            Toast.MakeText(Application.Context, "IdleRPG Checker Service Initialized", ToastLength.Long).Show();
        }
        catch (Exception ex)
        {
            Log.Error("IdleRPG", ex.Message);
            Toast.MakeText(Application.Context, "IdleRPG Checker Service Exception: " + ex.Message, ToastLength.Long).Show();
        }
    }


    public static void ScheduleTask()
    {
        Log.Debug("IdleRPG", "CheckerService : ScheduleTask - Scheduling Task");

        var myTask = new PeriodicTask.Builder()
            .SetPeriod(MinutesWait * 60)
            .SetFlex(MinutesWait * 30)
            .SetPersisted(true)
            .SetTag("IdleRpg")
            .SetService(Java.Lang.Class.FromType(typeof(CheckerGcmService)))
            .SetUpdateCurrent(true)
            .Build();

        GcmNetworkManager.GetInstance(Application.Context).Schedule(myTask);

        Log.Info("IdleRPG", "CheckerService : ScheduleTask - Task Scheduled");
    }



    public static void SaveLastCheckedDateTime()
    {
        var prefs = Application.Context.GetSharedPreferences("IdleRPGViewer2", FileCreationMode.Private);
        var prefEditor = prefs.Edit();
        prefEditor.PutString("LastChecked", DateTimeOffset.Now.ToString());
        prefEditor.Commit();
    }

    public static DateTimeOffset GetLastCheckedDateTime()
    {
        var prefs = Application.Context.GetSharedPreferences("IdleRPGViewer2", FileCreationMode.Private);
        var lastCheckedStr = prefs.GetString("LastChecked", DateTimeOffset.MinValue.ToString());
        return DateTimeOffset.Parse(lastCheckedStr);
    }

}





public class ServiceConnectedEventArgs : EventArgs
{
    public IBinder Binder { get; set; }
}

public class CheckerServiceConnection : Java.Lang.Object, IServiceConnection
{
    public event EventHandler<ServiceConnectedEventArgs> ServiceConnected = delegate { };

    public CheckerServiceBinder Binder
    {
        get { return this.binder; }
        set { this.binder = value; }
    }
    protected CheckerServiceBinder binder;

    public CheckerServiceConnection(CheckerServiceBinder binder)
    {
        if (binder != null)
        {
            this.binder = binder;
        }
    }

    // This gets called when a client tries to bind to the Service with an Intent and an 
    // instance of the ServiceConnection. The system will locate a binder associated with the 
    // running Service 
    public void OnServiceConnected(ComponentName name, IBinder service)
    {
        // cast the binder located by the OS as our local binder subclass
        CheckerServiceBinder serviceBinder = service as CheckerServiceBinder;
        if (serviceBinder != null)
        {
            this.binder = serviceBinder;
            this.binder.IsBound = true;

            // raise the service connected event
            this.ServiceConnected(this, new ServiceConnectedEventArgs { Binder = service });

            // now that the Service is bound, we can start gathering some location data
            serviceBinder.Service.InitializeCheckerService();
        }
    }

    // This will be called when the Service unbinds, or when the app crashes
    public void OnServiceDisconnected(ComponentName name)
    {
        this.binder.IsBound = false;
        Log.Debug("IdleRPG", "Service unbound");
    }
}

public class CheckerServiceBinder : Binder
{
    public CheckerService Service
    {
        get { return this.service; }
    }
    protected CheckerService service;

    public bool IsBound { get; set; }

    // constructor
    public CheckerServiceBinder(CheckerService service)
    {
        this.service = service;
    }
}