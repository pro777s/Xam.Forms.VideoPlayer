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
    public static class MainActivity
    {
        public static Activity Current { get; private set; } = null;
        public static readonly int PickImageId = 1000;
        public static TaskCompletionSource<string> PickImageTaskCompletionSource { get; set; }

        internal static void Init(Activity mainActivity)
        {
            Current = mainActivity;
        }
    }
}