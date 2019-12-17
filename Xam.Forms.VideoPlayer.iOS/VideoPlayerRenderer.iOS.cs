using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;

using AVFoundation;
using AVKit;
using CoreMedia;
using Foundation;
using UIKit;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Xam.Forms.VideoPlayer.VideoPlayer),
                          typeof(Xam.Forms.VideoPlayer.iOS.VideoPlayerRenderer))]

namespace Xam.Forms.VideoPlayer.iOS
{
    public class VideoPlayerRenderer : ViewRenderer<VideoPlayer, UIView>
    {
        public new static void Init() { }

        AVPlayer player;
        AVPlayerItem playerItem;
        AVPlayerViewController _playerViewController;       // solely for ViewController property
        NSObject playCompleteNotification, playerItemFailedToPlayToEndTimeNotification;

        public override UIViewController ViewController => _playerViewController;

        protected override void OnElementChanged(ElementChangedEventArgs<VideoPlayer> args)
        {
            base.OnElementChanged(args);

            if (args.NewElement != null)
            {
                if (Control == null)
                {
                    // Create AVPlayerViewController
                    _playerViewController = new AVPlayerViewController();

                    // Set Player property to AVPlayer
                    player = new AVPlayer();
                    _playerViewController.Player = player;

                    // End of play notification
                    playCompleteNotification = NSNotificationCenter.DefaultCenter.AddObserver(
                        AVPlayerItem.DidPlayToEndTimeNotification,
                        OnAVPlayerItemDidPlayToEndTime, player.CurrentItem);

                    //  Play error occured notification
                    playerItemFailedToPlayToEndTimeNotification = 
                        AVPlayerItem.Notifications.ObserveItemFailedToPlayToEndTime(OnAVPlayerItemFailedToPlayToEndTime);

                    // Use the View from the controller as the native control
                    SetNativeControl(_playerViewController.View);
                }

                SetAreTransportControlsEnabled();
                SetSource();

                args.NewElement.UpdateStatus += OnUpdateStatus;
                args.NewElement.PlayRequested += OnPlayRequested;
                args.NewElement.PauseRequested += OnPauseRequested;
                args.NewElement.StopRequested += OnStopRequested;
                args.NewElement.ShowTransportControlsRequested += OnShowTransportControls;
                args.NewElement.HideTransportControlsRequested += OnHideTransportControls;
            }

            if (args.OldElement != null)
            {
                args.OldElement.UpdateStatus -= OnUpdateStatus;
                args.OldElement.PlayRequested -= OnPlayRequested;
                args.OldElement.PauseRequested -= OnPauseRequested;
                args.OldElement.StopRequested -= OnStopRequested;
                args.OldElement.ShowTransportControlsRequested -= OnShowTransportControls;
                args.OldElement.HideTransportControlsRequested -= OnHideTransportControls;
            }
        }

        private void OnAVPlayerItemDidPlayToEndTime(NSNotification notification)
        {
            Element.OnPlayCompletion();
        }

        private void OnAVPlayerItemFailedToPlayToEndTime(object sender, AVPlayerItemErrorEventArgs e)
        {
            Element.OnPlayError(sender, new VideoPlayer.PlayErrorEventArgs(player.Error?.Description));
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (player != null)
            {

                playerItemFailedToPlayToEndTimeNotification.Dispose();
                NSNotificationCenter.DefaultCenter.RemoveObserver(playCompleteNotification);
                player.ReplaceCurrentItemWithPlayerItem(null);
            }
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
                TimeSpan controlPosition = ConvertTime(player.CurrentTime);

                if (Math.Abs((controlPosition - Element.Position).TotalSeconds) > 1)
                {
                    if (Element.AreTransportControlsEnabled)
                    {
                        ((AVPlayerViewController)ViewController).ShowsPlaybackControls = true;
                        Device.StartTimer(TimeSpan.FromSeconds(2), () =>
                        {
                            ((AVPlayerViewController)ViewController).ShowsPlaybackControls = false;
                            return false;
                        });
                    }
                    player.Seek(CMTime.FromSeconds(Element.Position.TotalSeconds, 1));
                }
            }
        }

        private void OnShowTransportControls(object sender, EventArgs args)
        {
            if (Element.AreTransportControlsEnabled)
            {
                    ((AVPlayerViewController)ViewController).ShowsPlaybackControls = true;
            }
        }

        private void OnHideTransportControls(object sender, EventArgs args)
        {
            if (Element.AreTransportControlsEnabled)
            {
                    ((AVPlayerViewController)ViewController).ShowsPlaybackControls = false;
            }
        }

        void SetAreTransportControlsEnabled()
        {
            ((AVPlayerViewController)ViewController).ShowsPlaybackControls = Element.AreTransportControlsEnabled;
        }

        void SetSource()
        {
            AVAsset asset = null;

            if (Element.Source is UriVideoSource)
            {
                string uri = (Element.Source as UriVideoSource).Uri;

                if (!String.IsNullOrWhiteSpace(uri))
                {
                    //asset = AVAsset.FromUrl(new NSUrl(uri));
                    NSUrl url = null;
                    try
                    {
                        string absUri = new Uri(uri).AbsoluteUri;
                        url = new NSUrl(absUri);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }
                    if (url != null)
                        asset = AVAsset.FromUrl(url);
                }
            }
            else if (Element.Source is FileVideoSource)
            {
                string uri = (Element.Source as FileVideoSource).File;

                if (!String.IsNullOrWhiteSpace(uri))
                {
                    asset = AVAsset.FromUrl(new NSUrl(uri));
                }
            }
            else if (Element.Source is ResourceVideoSource)
            {
                string path = (Element.Source as ResourceVideoSource).Path;

                if (!String.IsNullOrWhiteSpace(path))
                {
                    string directory = Path.GetDirectoryName(path);
                    string filename = Path.GetFileNameWithoutExtension(path);
                    string extension = Path.GetExtension(path).Substring(1);
                    NSUrl url = NSBundle.MainBundle.GetUrlForResource(filename, extension, directory);
                    asset = AVAsset.FromUrl(url);
                }
            }

            if (asset != null)
            {
                //ExploreProperties(asset);
                playerItem = new AVPlayerItem(asset);
            }
            else
            {
                playerItem = null;
            }

            player.ReplaceCurrentItemWithPlayerItem(playerItem);

            if (playerItem != null && Element.AutoPlay)
            {
                player.Play();
            }
        }

        private static void ExploreProperties(AVAsset asset)
        {
            AVMediaSelection[] aVMediaSelections = asset.AllMediaSelections;
            string[] mDataFormats = asset.AvailableMetadataFormats;
            string description = asset.Description;
            AVAssetTrack[] avAssetVideoTracks = asset.GetTracks(AVMediaTypes.Video);
            foreach (AVAssetTrack avAssetVideoTrack in avAssetVideoTracks)
            {
                string cDescription = avAssetVideoTrack.Description;
                string dDescription = avAssetVideoTrack.DebugDescription;
                NSObject[] nsObjects = avAssetVideoTrack.FormatDescriptionsAsObjects;
                string descr = nsObjects[0].Description;
                //NSObject nsoDimension = avAssetVideoTrack.ValueForKey(new NSString("dimension"));
                AVMetadataItem[] avCommonMetadataItems = avAssetVideoTrack.CommonMetadata;
                AVMetadataItem[] avMetadataItems = avAssetVideoTrack.Metadata;
                CMFormatDescription[] cmFormatDescriptions = avAssetVideoTrack.FormatDescriptions;
                foreach (CMFormatDescription cmFormatDescription in cmFormatDescriptions)
                {
                    CMVideoCodecType cmVideoCodecType = cmFormatDescription.VideoCodecType;
                    CMSubtitleFormatType cmSubtitleFormatType = cmFormatDescription.SubtitleFormatType;
                    CMMetadataFormatType cmMetadataFormatType = cmFormatDescription.MetadataFormatType;
                    AudioToolbox.AudioFormat[] atAudioFormats = cmFormatDescription.AudioFormats;
                }
            }
        }

        // Event handler to update status
        void OnUpdateStatus(object sender, EventArgs args)
        {
            VideoStatus videoStatus = VideoStatus.NotReady;

            switch (player.Status)
            {
                case AVPlayerStatus.ReadyToPlay:
                    switch (player.TimeControlStatus)
                    {
                        case AVPlayerTimeControlStatus.Playing:
                            videoStatus = VideoStatus.Playing;
                            break;

                        case AVPlayerTimeControlStatus.Paused:
                            videoStatus = VideoStatus.Paused;
                            break;
                    }
                    break;
            }
            ((IVideoPlayerController)Element).Status = videoStatus;

            if (playerItem != null)
            {
                ((IVideoPlayerController)Element).Duration = ConvertTime(playerItem.Duration);
                ((IElementController)Element).SetValueFromRenderer(VideoPlayer.PositionProperty, ConvertTime(playerItem.CurrentTime));
            }
        }

        TimeSpan ConvertTime(CMTime cmTime)
        {
            return TimeSpan.FromSeconds(Double.IsNaN(cmTime.Seconds) ? 0 : cmTime.Seconds);

        }

        // Event handlers to implement methods
        void OnPlayRequested(object sender, EventArgs args)
        {
            player.Play();
        }

        void OnPauseRequested(object sender, EventArgs args)
        {
            player.Pause();
        }

        void OnStopRequested(object sender, EventArgs args)
        {
            player.Pause();
            player.Seek(new CMTime(0, 1));
        }
    }
}