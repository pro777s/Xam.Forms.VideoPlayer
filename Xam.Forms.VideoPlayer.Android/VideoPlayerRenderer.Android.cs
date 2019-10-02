using System;
using System.ComponentModel;
using System.IO;

using Android.App;
using Android.Content;
using Android.Widget;
using NSUri = Android.Net.Uri;
using ARelativeLayout = Android.Widget.RelativeLayout;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xam.Forms.VideoPlayer;
using Xam.Forms.VideoPlayer.Android;
using Android.Graphics;
using Android.Media;
using System.Collections.Generic;
using System.Linq;
using Android.Runtime;
using Java.Util;
using Color = Android.Graphics.Color;

[assembly: ExportRenderer(typeof(VideoPlayer), typeof(VideoPlayerRenderer))]

namespace Xam.Forms.VideoPlayer.Android
{
    public class MediaPlayerVideoSizeChangedListener : Java.Lang.Object, MediaPlayer.IOnVideoSizeChangedListener
    {
        void MediaPlayer.IOnVideoSizeChangedListener.OnVideoSizeChanged(MediaPlayer mp, int width, int height)
        {

        }
    }

    public class MediaPlayerInfoListener : Java.Lang.Object, MediaPlayer.IOnInfoListener
    {
        public bool OnInfo(MediaPlayer mp, [GeneratedEnum] MediaInfo what, int extra)
        {
            try
            {
                MediaPlayer.TrackInfo[] trackInfoArray = mp.GetTrackInfo();
                if (trackInfoArray == null)
                    return true;
                for (int i = 0; i < trackInfoArray.Length; i++)
                {
                    // you can switch out the language comparison logic to whatever works for you
                    if (trackInfoArray[i].TrackType == MediaTrackType.Audio
                        && trackInfoArray[i].Language == Locale.Default.ISO3Language)
                    {
                        mp.SelectTrack(i);
                        break;
                    }
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            return true;
        }
    }

    public class VideoPlayerRenderer : ViewRenderer<VideoPlayer, ARelativeLayout>
    {
        VideoView videoView;
        FullScreenMediaController mediaController;    // Used to display transport controls
        bool isPrepared;
        int videoHeight, videoWidth;

        public VideoPlayerRenderer(Context context) : base(context)
        {
        }

        public static void Init() { }

        protected override void OnElementChanged(ElementChangedEventArgs<VideoPlayer> args)
        {
            base.OnElementChanged(args);

            if (args.NewElement != null)
            {
                if (Control == null)
                {
                    // Save the VideoView for future reference
                    videoView = new VideoView(Context);
                    videoView.SetBackgroundColor(Color.Transparent);
                    videoView.Info += VideoView_Info;
                    videoView.Completion += VideoView_Completion;
                    videoView.Error += VideoView_Error;

                    // Put the VideoView in a RelativeLayout
                    ARelativeLayout relativeLayout = new ARelativeLayout(Context);
                    relativeLayout.AddView(videoView);

                    // Center the VideoView in the RelativeLayout
                    ARelativeLayout.LayoutParams layoutParams =
                        new ARelativeLayout.LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent);
                    layoutParams.AddRule(LayoutRules.CenterInParent);
                    videoView.LayoutParameters = layoutParams;

                    // Handle a VideoView event
                    videoView.Prepared += OnVideoViewPrepared;

                    SetNativeControl(relativeLayout);
                }

                SetAreTransportControlsEnabled();
                SetSource();

                args.NewElement.UpdateStatus += OnUpdateStatus;
                args.NewElement.PlayRequested += OnPlayRequested;
                args.NewElement.PauseRequested += OnPauseRequested;
                args.NewElement.StopRequested += OnStopRequested;
            }

            if (args.OldElement != null)
            {
                args.OldElement.UpdateStatus -= OnUpdateStatus;
                args.OldElement.PlayRequested -= OnPlayRequested;
                args.OldElement.PauseRequested -= OnPauseRequested;
                args.OldElement.StopRequested -= OnStopRequested;
            }
        }

        private void VideoView_Error(object sender, MediaPlayer.ErrorEventArgs e)
        {
            Element.OnPlayError(sender, new VideoPlayer.PlayErrorEventArgs(e.What.ToString()));
        }

        private void VideoView_Completion(object sender, EventArgs e)
        {
            Element.OnPlayCompletion();
        }

        private void VideoView_Info(object sender, MediaPlayer.InfoEventArgs e)
        {
            MediaPlayer mp = e.Mp;
            videoHeight = mp.VideoHeight;
            videoWidth = mp.VideoWidth;
            //mp.SetOnVideoSizeChangedListener(new MediaPlayerVideoSizeChangedListener());
            //MediaPlayer.TrackInfo[] trackInfoArray = mp.GetTrackInfo();
            //if (trackInfoArray != null)
            //{
            //    MediaPlayer.TrackInfo videoTrack = new List<MediaPlayer.TrackInfo>(trackInfoArray)
            //    .Where(x => x.TrackType == MediaTrackType.Video).FirstOrDefault();
            //    if (videoTrack != null)
            //    {
            //        int descrFlags = videoTrack.DescribeContents();
            //        MediaFormat mediaFormat = videoTrack.Format;
            //    }
            //}
            mediaController.ShowVideoSize(videoWidth, videoHeight);
        }

        protected override void Dispose(bool disposing)
        {
            if (Control != null && videoView != null)
            {
                videoView.Prepared -= OnVideoViewPrepared;
            }
            if (Element != null)
            {
                Element.UpdateStatus -= OnUpdateStatus;
            }

            base.Dispose(disposing);
        }

        void OnVideoViewPrepared(object sender, EventArgs args)
        {
            isPrepared = true;
            ((IVideoPlayerController)Element).Duration = TimeSpan.FromMilliseconds(videoView.Duration);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            base.OnElementPropertyChanged(sender, args);

            if (args.PropertyName == VideoPlayer.AreTransportControlsEnabledProperty.PropertyName)
            {
                SetAreTransportControlsEnabled();
            }
            else if (args.PropertyName == VideoPlayer.SourceProperty.PropertyName)
            {
                SetSource();
            }
            else if (args.PropertyName == VideoPlayer.PositionProperty.PropertyName)
            {
                if (Math.Abs(videoView.CurrentPosition - Element.Position.TotalMilliseconds) > 1000)
                {
                    videoView.SeekTo((int)Element.Position.TotalMilliseconds);
                }
            }
        }

        void SetAreTransportControlsEnabled()
        {
            if (Element.AreTransportControlsEnabled)
            {
                mediaController = new FullScreenMediaController(Context);
                mediaController.SetMediaPlayer(videoView);
                videoView.SetMediaController(mediaController);
            }
            else
            {
                videoView.SetMediaController(null);

                if (mediaController != null)
                {
                    mediaController.SetMediaPlayer(null);
                    mediaController = null;
                }
            }
        }

        void SetSource()
        {
            isPrepared = false;
            bool hasSetSource = false;

            if (Element.Source is UriVideoSource)
            {
                string uri = (Element.Source as UriVideoSource).Uri;

                if (!String.IsNullOrWhiteSpace(uri))
                {
                    videoView.SetVideoURI(NSUri.Parse(uri));
                    hasSetSource = true;
                }
            }
            else if (Element.Source is FileVideoSource)
            {
                string filename = (Element.Source as FileVideoSource).File;

                if (!String.IsNullOrWhiteSpace(filename))
                {
                    videoView.SetVideoPath(filename);
                    hasSetSource = true;
                }
            }
            else if (Element.Source is ResourceVideoSource)
            {
                string package = Context.PackageName;
                string path = (Element.Source as ResourceVideoSource).Path;

                if (!String.IsNullOrWhiteSpace(path))
                {
                    string filename = System.IO.Path.GetFileNameWithoutExtension(path).ToLowerInvariant();
                    string uri = "android.resource://" + package + "/raw/" + filename;
                    videoView.SetVideoURI(NSUri.Parse(uri));
                    hasSetSource = true;
                }
            }
              
            if (hasSetSource && Element.AutoPlay)
            {
                videoView.Start();
            }
        }

        // Event handler to update status
        void OnUpdateStatus(object sender, EventArgs args)
        {
            VideoStatus status = VideoStatus.NotReady;

            if (isPrepared)
            {
                status = videoView.IsPlaying ? VideoStatus.Playing : VideoStatus.Paused;
            }

            ((IVideoPlayerController)Element).Status = status;

            // Set Position property
            TimeSpan timeSpan = TimeSpan.FromMilliseconds(videoView.CurrentPosition);
            ((IElementController)Element).SetValueFromRenderer(VideoPlayer.PositionProperty, timeSpan);
        }

        // Event handlers to implement methods
        void OnPlayRequested(object sender, EventArgs args)
        {
            videoView.Start();
        }

        void OnPauseRequested(object sender, EventArgs args)
        {
            videoView.Pause();
        }

        void OnStopRequested(object sender, EventArgs args)
        {
            videoView.StopPlayback();
        }
    }
}