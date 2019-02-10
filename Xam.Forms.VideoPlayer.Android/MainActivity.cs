using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Xam.Forms.VideoPlayer.Android
{
    [Application]
    public partial class MainActivity : Application, Application.IActivityLifecycleCallbacks
    {
        internal static Context Current { get; private set; }

        public MainActivity(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer) { }

        public override void OnCreate()
        {
            base.OnCreate();
            RegisterActivityLifecycleCallbacks(this);
        }

        public override void OnTerminate()
        {
            base.OnTerminate();
            UnregisterActivityLifecycleCallbacks(this);
        }

        public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
        {
            Current = activity;
        }

        public void OnActivityResumed(Activity activity)
        {
            Current = activity;
        }

        public void OnActivityStarted(Activity activity)
        {
            Current = activity;
        }

        public void OnActivityDestroyed(Activity activity) { }
        public void OnActivityPaused(Activity activity) { }
        public void OnActivitySaveInstanceState(Activity activity, Bundle outState) { }
        public void OnActivityStopped(Activity activity) { }
    }
}