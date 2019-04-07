using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Xam.Forms.VideoPlayer.Android
{
    public class FullScreenMediaController : MediaController
    {
        private static readonly int fullScreenImageId, fullScreenExitImageId;
        private ImageButton ibFullScreen;
        private StatusBar statusBar;

        public FullScreenMediaController(Context context) : base(context) 
        {
            statusBar = new StatusBar();
        }

        static FullScreenMediaController()
        {
            fullScreenImageId = GetResourceDrawableId("ic_fullscreen");
            fullScreenExitImageId = GetResourceDrawableId("ic_fullscreen_exit");
        }

        public override void SetAnchorView(View view)
        {
            base.SetAnchorView(view);
            ibFullScreen = new ImageButton(Context);
            ibFullScreen.SetBackgroundColor(Color.Transparent);
            LayoutParams layoutParams =
            new LayoutParams(120, 120, GravityFlags.NoGravity)
            {
                TopMargin = 30,
                LeftMargin = 15,
            };
            int imageId = IsFullScreen() ? fullScreenExitImageId : fullScreenImageId;
            ibFullScreen.SetImageResource(imageId);
            ibFullScreen.ScaleX = 0.5f;
            ibFullScreen.ScaleY = 0.5f;
            AddView(ibFullScreen, layoutParams);
            ibFullScreen.SetOnClickListener(new OnClickListener());
        }

        private static int GetResourceDrawableId(string imageFileName)
        {
            return (int)typeof(Resource.Drawable).GetField(imageFileName).GetValue(null);
        }

        private class OnClickListener : Java.Lang.Object, IOnClickListener
        {
            void IOnClickListener.OnClick(View view)
            {
                FullScreenMediaController mediaController = view.Parent as FullScreenMediaController;
                mediaController.InvertScreenMode();
            }
        }

        private void InvertScreenMode()
        {
            if (IsFullScreen())
            {
            var activity = Context as Activity;
                activity.RequestedOrientation = ScreenOrientation.Portrait;
                ExitFromFullScreen();
            }
            else
            {
               var activity = Context as Activity;
                activity.RequestedOrientation = ScreenOrientation.Landscape;
                EnterToFullScreen();
            }
        }

        private void EnterToFullScreen()
        {
            ibFullScreen.SetImageResource(fullScreenExitImageId);
            statusBar.HideStatusBar();
        }

        private void ExitFromFullScreen()
        {
            ibFullScreen.SetImageResource(fullScreenImageId);
            statusBar.ShowStatusBar();
        }

        public static bool IsFullScreen()
        {
            var activity = MainActivity.Current as Activity;
            var attrs = activity.Window.Attributes;
            return (attrs.Flags & WindowManagerFlags.Fullscreen) == WindowManagerFlags.Fullscreen;
        }
    }
}
