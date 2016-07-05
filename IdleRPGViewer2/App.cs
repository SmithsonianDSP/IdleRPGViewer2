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

namespace IdleRPGViewer2
{

    public class App
    {
        // events
        public event EventHandler<ServiceConnectedEventArgs> CheckerServiceConnected = delegate { };

        // declarations
        protected readonly string logTag = "IdleRPG";
        protected static CheckerServiceConnection checkerServiceConnection;

        // properties

        public static App Current
        {
            get { return current; }
        }
        static App current;

        public CheckerService checkerService
        {
            get
            {
                if (checkerServiceConnection.Binder == null)
                    return null;
                // note that we use the ServiceConnection to get the Binder, and the Binder to get the Service here
                return checkerServiceConnection.Binder.Service;
            }
        }

        #region Application context

        static App()
        {
            current = new App();
        }
      

        protected App()
        {
            // create a new service connection so we can get a binder to the service
            checkerServiceConnection = new CheckerServiceConnection(null);

            //// this event will fire when the Service connectin in the OnServiceConnected call 
            checkerServiceConnection.ServiceConnected += (object sender, ServiceConnectedEventArgs e) => {

                // we will use this event to notify MainActivity when to start updating the UI
                this.CheckerServiceConnected(this, e);
            };
        }

        public static void StartCheckerService()
        {

            new Task(() => {

                Application.Context.StartService(new Intent(Android.App.Application.Context, typeof(CheckerService)));

                Intent checkerServiceIntent = new Intent(Android.App.Application.Context, typeof(CheckerService));

                Application.Context.BindService(checkerServiceIntent, checkerServiceConnection, Bind.AutoCreate);
            }).Start();
        }

        public static void StopCheckerService()
        {
            // Unbind from the LocationService; otherwise, StopSelf (below) will not work:
            if (checkerServiceConnection != null)
            {
                Android.App.Application.Context.UnbindService(checkerServiceConnection);
            }

            checkerServiceConnection = new CheckerServiceConnection(null);

            // Stop the LocationService:
            if (Current.checkerService != null)
            {
                Current.checkerService.StopSelf();
            }
        }

        #endregion

    }
}