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
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            videoPlayer.Source = _videoSource;
            if (Device.RuntimePlatform == Device.Android)
            {
                DependencyService.Get<IStatusBar>().HideStatusBar();
            }
        }

        private async void VideoPlayer_PlayCompletion(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}