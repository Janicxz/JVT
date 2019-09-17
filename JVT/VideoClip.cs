using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVT
{
    public class VideoClip
    {
        public string filePath;
        public string OutputName;
        public bool Merge;
        public bool Encode;
        public bool MultiTrackAudio;
        public bool MergeAudioTracks;
        public int Volume;
        public TimeSpan Start;
        public TimeSpan End;
    }
}
