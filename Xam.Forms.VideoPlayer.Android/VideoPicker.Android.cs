using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Xamarin.Forms;

using Xam.Forms.VideoPlayer.Android;
using Application = Android.App.Application;
//using MainActivity = Xamarin.Forms.Platform.Android.FormsAppCompatActivity;

[assembly: Dependency(typeof(VideoPicker))]

namespace Xam.Forms.VideoPlayer.Android
{
    public class VideoPicker : IVideoPicker
    {
        public static readonly int RequestCode = 1000;
        public static TaskCompletionSource<string> TaskCompletionSource { get; set; }

        public Task<string> GetVideoFileAsync()
        {
            // Define the Intent for getting images
            Intent intent = new Intent();
            intent.SetType("video/*");
            intent.SetAction(Intent.ActionGetContent);

            // Get the MainActivity instance
            var activity = MainActivity.Current as Activity;
            // Start the picker activity (resumes in MainActivity.cs)
            activity.StartActivityForResult(
                Intent.CreateChooser(intent, Resources.SelectVideo), RequestCode);

            //Save the TaskCompletionSource object as a MainActivity property
            TaskCompletionSource = new TaskCompletionSource<string>();

            //Return Task object
            return TaskCompletionSource.Task;
        }

        public static void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (requestCode == RequestCode)
            {
                if ((resultCode == Result.Ok) && (data != null))
                {
                    // Set the filename as the completion of the Task
                    TaskCompletionSource.SetResult(data.DataString);
                }
                else
                {
                    TaskCompletionSource.SetResult(null);
                }
            }
        }
    }
}