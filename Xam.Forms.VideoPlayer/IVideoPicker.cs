using System;
using System.Threading.Tasks;

namespace Xam.Forms.VideoPlayer
{
    public interface IVideoPicker
    {
        Task<string> GetVideoFileAsync();
    }
}
