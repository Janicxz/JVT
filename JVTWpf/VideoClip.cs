using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JVTWpf
{
    public class VideoClip
    {
        public string filePath { get; set; }
        public string OutputName { get; set; }
        public bool Merge { get; set; }
        public bool Encode { get; set; }
        public bool MultiTrackAudio { get; set; }
        public bool MergeAudioTracks { get; set; }
        public int Volume { get; set; }
        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
        public TimeSpan Length { get; set; }
    }
}
