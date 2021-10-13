using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace JVTWpf
{
    public class VideoClip : INotifyPropertyChanged
    {
        private string _filePath;
        private string _outputName;
        private bool _encode;
        private bool _merge;
        private bool _multiTrackAudio;
        private bool _mergeAudioTracks;
        public bool HasAudio = false;
        private int _volume;
        private int _bitrate;
        private TimeSpan _start;
        private TimeSpan _end;
        private TimeSpan _length;
        private BitmapSource _thumbnail;
       // public BitmapSource ThumbnailStart;
        public BitmapSource thumbnail
        {
            get
            {
              //  if (_thumbnail == null)
              //      return ThumbnailStart;
              //  else
                    return _thumbnail;
            }
            set
            {
                _thumbnail = value;
                NotifyPropertyChanged("thumbnail");
            }
        }

        public int bitRate
        {
            get { return _bitrate; }
            set
            {
                _bitrate = value;
                NotifyPropertyChanged("bitRate");
            }
        }
        public string filePath
        {
            get { return _filePath; }
            set
            {
                _filePath = value;
                NotifyPropertyChanged("filePath");
            }
        }
        public string OutputName
        {
            get { return _outputName; }
            set
            {
                _outputName = value;
                NotifyPropertyChanged("OutputName");
            }
        }
        public bool Merge
        {
            get { return _merge; }
            set
            {
                _merge = value;
                NotifyPropertyChanged("Merge");
            }
        }
        public bool Encode
        {
            get { return _encode; }
            set
            {
                _encode = value;
                NotifyPropertyChanged("Encode");
            }
        }
        public bool MultiTrackAudio
        {
            get { return _multiTrackAudio; }
            set
            {
                _multiTrackAudio = value;
                NotifyPropertyChanged("MultiTrackAudio");
            }
        }
        public bool MergeAudioTracks
        {
            get { return _mergeAudioTracks; }
            set
            {
                _mergeAudioTracks = value;
                NotifyPropertyChanged("MergeAudioTracks");
            }
        }
        public int Volume
        {
            get { return _volume; }
            set
            {
                _volume = value;
                NotifyPropertyChanged("Volume");
            }
        }
        public TimeSpan Start
        {
            get { return _start; }
            set
            {
                _start = value;
                NotifyPropertyChanged("Start");
            }
        }
        public TimeSpan End
        {
            get { return _end; }
            set
            {
                _end = value;
                NotifyPropertyChanged("End");
            }
        }
        public TimeSpan Length
        {
            get { return _length; }
            set
            {
                _length = value;
                NotifyPropertyChanged("Length");
            }
        }

        public string inputFileCodec = "";

        public bool forceOverwrite = false;

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            forceOverwrite = true;
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
