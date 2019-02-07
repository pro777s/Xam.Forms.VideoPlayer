using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using Xam.Forms.VideoPlayer.UWP;

[assembly: Xamarin.Forms.Dependency(typeof(StatusBar))]

namespace Xam.Forms.VideoPlayer.UWP
{
    class StatusBar : IStatusBar
    {
        public async void HideStatusBar()
        {
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                var statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
                if (statusBar != null)
                {
                    await statusBar.HideAsync();  
                }
            }
        }

        public async void ShowStatusBar()
        {
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                var statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
                if (statusBar != null)
                {
                    await statusBar.ShowAsync();  
                }
            }
        }
    }
}
