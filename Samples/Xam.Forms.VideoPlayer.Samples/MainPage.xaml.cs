using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Xam.Forms.VideoPlayer.Samples
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        async void OnShowVideoLibraryClicked(object sender, EventArgs args)
        {
            Button btn = (Button)sender;
            btn.IsEnabled = false;

            string filename = await DependencyService.Get<IVideoPicker>().GetVideoFileAsync();

            if (!String.IsNullOrWhiteSpace(filename))
            {
                FileVideoSource fileVideoSource = new FileVideoSource
                {
                    File = filename
                };
                await Navigation.PushAsync(new PlayVideoPage(fileVideoSource), true);
            }

            btn.IsEnabled = true;
        }

        private async void ButtonPlayBunny_Clicked(object sender, EventArgs e)
        {
            UriVideoSource uriVideoSurce = new UriVideoSource()
            {
                Uri = "https://archive.org/download/BigBuckBunny_328/BigBuckBunny_512kb.mp4"
            };
            await Navigation.PushAsync(new PlayVideoPage(uriVideoSurce), true);
        }
    }
}