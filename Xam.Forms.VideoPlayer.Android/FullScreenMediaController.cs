﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Xam.Forms.VideoPlayer.Android
{
    public class FullScreenMediaController : MediaController
    {
        private static readonly int fullScreenImageId, fullScreenExitImageId;
        private ImageButton ibFullScreen;
        private StatusBar statusBar;
        private TextView tvVideoSize;

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
            int layoutPx = DiPtoPx(40, view.Context);
            int topMarginPx = DiPtoPx(10, view.Context);
            int leftMarginPx = DiPtoPx(5, view.Context);
            LayoutParams layoutParams =
            new LayoutParams(layoutPx, layoutPx, GravityFlags.NoGravity)
            {
                TopMargin = topMarginPx,
                LeftMargin = leftMarginPx,
            };
            int imageId = IsFullScreen() ? fullScreenExitImageId : fullScreenImageId;
            ibFullScreen.SetImageResource(imageId);
            ibFullScreen.ScaleX = 0.5f;
            ibFullScreen.ScaleY = 0.5f;
            AddView(ibFullScreen, layoutParams);
            ibFullScreen.SetOnClickListener(new OnClickListener());
            layoutParams =
            new LayoutParams(DiPtoPx(56, view.Context), DiPtoPx(16, view.Context), GravityFlags.Right)
            {
                TopMargin = DiPtoPx(5, view.Context),
                RightMargin = DiPtoPx(5, view.Context),
            };
            tvVideoSize = new TextView(Context);
            tvVideoSize.SetBackgroundColor(Color.Transparent);
            tvVideoSize.SetTextColor(Color.White);
            tvVideoSize.SetTextSize(ComplexUnitType.Pt, 6);
            tvVideoSize.Gravity = GravityFlags.Right;
            AddView(tvVideoSize, layoutParams);
        }

        public void ShowVideoSize(int width, int height)
        {
            if (tvVideoSize is null)
                return;
            string videoSizeText = width + "x" + height;
            tvVideoSize.SetText(videoSizeText.ToCharArray(), 0, videoSizeText.Length);
        }

        private static int DiPtoPx(float dp, Context context)
        {
            float fpx = TypedValue.ApplyDimension(ComplexUnitType.Dip, dp, context.Resources.DisplayMetrics);
            return (int)Math.Round(fpx);
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
