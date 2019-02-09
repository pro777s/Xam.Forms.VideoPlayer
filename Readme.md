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
Xam.Forms.VideoPlayer.Android.VideoPlayerRenderer.Init(this);
Xamarin.Forms.Init();
```

In your iOS projects (AppDelegate.cs) call:

```
Xam.Forms.VideoPlayer.iOS.VideoPlayerRenderer.Init();
Xamarin.Forms.Init();
```

In UWP platform:

```
```

For more information, see the folder Samples.

#### Release Notes

1.0.1

[All] Added the event handler PlayError an play error occur.


1.0.0

[All] Added the ability to show/hide window statusbar using IStatusbar interface.

[Android] Added the ability to enter/exit full-screen mode using FullscreenMediaController class. Added the corresponding button in the video player interface.

[All] Implemented the event PlayCompletion announcing the end of playback.

