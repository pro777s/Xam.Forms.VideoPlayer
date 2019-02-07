using System;
using System.Threading.Tasks;
using Android.Content;
using Xamarin.Forms;

using Xam.Forms.VideoPlayer.Android;
//using MainActivity = Xamarin.Forms.Platform.Android.FormsAppCompatActivity;

[assembly: Dependency(typeof(VideoPicker))]

namespace Xam.Forms.VideoPlayer.Android
{
    public class VideoPicker : IVideoPicker
    {
        public Task<string> GetVideoFileAsync()
        {
            // Define the Intent for getting images
            Intent intent = new Intent();
            intent.SetType("video/*");
            intent.SetAction(Intent.ActionGetContent);

            // Get the MainActivity instance
            if (MainActivity.Current == null)
                return null;
            var activity = MainActivity.Current;

            // Start the picture-picker activity (resumes in MainActivity.cs)
            activity.StartActivityForResult(
                Intent.CreateChooser(intent, Resources.SelectVideo),
                MainActivity.PickImageId);

            //Save the TaskCompletionSource object as a MainActivity property
            MainActivity.PickImageTaskCompletionSource = new TaskCompletionSource<string>();

            //Return Task object
            return MainActivity.PickImageTaskCompletionSource.Task;
        }
    }
}