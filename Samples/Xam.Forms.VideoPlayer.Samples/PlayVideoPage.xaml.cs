using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Xam.Forms.VideoPlayer.Samples
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PlayVideoPage : ContentPage
	{
        private VideoSource _videoSource;

        public PlayVideoPage(VideoSource videoSource)
        {
            _videoSource = videoSource;
            InitializeComponent();
            //Device.StartTimer(TimeSpan.FromSeconds(30), () => { videoPlayer.Position += TimeSpan.FromMinutes(1); return false; });  
            //Device.StartTimer(TimeSpan.FromSeconds(10), () => { videoPlayer.Pause(); videoPlayer.ShowTransportControls(); return false; });
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            videoPlayer.Source = _videoSource;
            if (Device.RuntimePlatform == Device.Android)
            {
                //NavigationPage.SetHasNavigationBar(this, false);
                DependencyService.Get<IStatusBar>().HideStatusBar();
            }
        }

        private async void VideoPlayer_PlayCompletion(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void VideoPlayer_PlayError(object sender, VideoPlayer.PlayErrorEventArgs e)
        {
            await Navigation.PopAsync();
        }

        protected override void OnDisappearing()
        {
            if (Device.RuntimePlatform == Device.Android)
            {
                //NavigationPage.SetHasNavigationBar(this, true);
                DependencyService.Get<IStatusBar>().ShowStatusBar();
            }
            videoPlayer.Stop();
            base.OnDisappearing();
        }

        private void VideoPlayer_BufferingStart(object sender, EventArgs e)
        {

        }

        private void VideoPlayer_BufferingEnd(object sender, EventArgs e)
        {

        }
    }
}