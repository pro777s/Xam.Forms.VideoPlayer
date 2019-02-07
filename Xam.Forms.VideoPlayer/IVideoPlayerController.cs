using System;

namespace Xam.Forms.VideoPlayer
{ 
    public interface IVideoPlayerController
    {
        VideoStatus Status { set; get; }

        TimeSpan Duration { set; get; }
    }
}
