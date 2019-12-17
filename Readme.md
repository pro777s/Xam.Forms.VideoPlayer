# Xam.Forms.VideoPlayer control for Xamarin Forms

#### Setup
* Available on NuGet: https://www.nuget.org/packages/Xam.Forms.VideoPlayer/ 
* Install in your .Net Standard 2.0 and client projects.

**Platform Support**

|Platform|Supported|Version|Renderer|
| ------------------- | :-----------: | :-----------: | :------------------: |
|Xamarin.iOS Unified|Yes|iOS 10.1+|UIView|
|Xamarin.Android|Yes|API 21+|ARelativeLayout|
|UWP|Yes|10.0|MediaElement|

#### Usage

In your Android projects (MainActivity.cs) call:

```
Xam.Forms.VideoPlayer.Android.VideoPlayerRenderer.Init();
Xamarin.Forms.Init();
```

In your iOS projects (AppDelegate.cs) call:

```
Xam.Forms.VideoPlayer.iOS.VideoPlayerRenderer.Init();
Xamarin.Forms.Init();
```

In UWP platform:

```
Xamarin.Forms.DependencyService.Register<VideoPicker>();
Xam.Forms.VideoPlayer.UWP.VideoPlayerRenderer.Init();
Xamarin.Forms.Forms.Init(e);
```

For more information, see the folder Samples.

#### Release Notes

1.0.7

[All] Removed ShowTransportControls property. Instead, the ShowTransportControls() and HideTransportControls() methods are added.
[UWP] Project compilation error due to the lack of a Xam.Forms.VideoPlayer.UWP.xr.xml file has been fixed.

1.0.6

[All] ShowTransportControls property added.
[Android] Fixed bug when setting the AreTransportControlsEnabled property.

1.0.5

[Android] Fixed a potential error that occurred when on get video info. 

1.0.4

[All] Fixed a potential error that occurred when calculating the remaining playback time.
[Android] Increased font size to display video size.
[Android] Fixed a error that occurred when getting info about tracks

1.0.3

[Android] Fixed positioning of the button for changing the visibility of the statusbar. Added video size output. 

1.0.2

[Android] Changed class MainActivity. Now it implements the interface IActivityLifecycleCallbacks. 
[All] Made refactoring and minor bugs fixed.

1.0.1

[All] Added the event handler PlayError an play error occur.

1.0.0

[All] Added the ability to show/hide window statusbar using IStatusbar interface.
[Android] Added the ability to enter/exit full-screen mode using FullscreenMediaController class. Added the corresponding button in the video player interface.
[All] Implemented the event PlayCompletion announcing the end of playback.
