using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using Xam.Forms.VideoPlayer.iOS;

[assembly: Xamarin.Forms.Dependency(typeof(StatusBar))]

namespace Xam.Forms.VideoPlayer.iOS
{
    public class StatusBar : IStatusBar
    {
        public StatusBar()
        {
        }

        #region IStatusBar implementation

        public void HideStatusBar()
        {
            UIApplication.SharedApplication.StatusBarHidden = true;
        }

        public void ShowStatusBar()
        {
            UIApplication.SharedApplication.StatusBarHidden = false;
        }

        #endregion
    }
}