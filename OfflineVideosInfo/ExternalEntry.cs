using System.Runtime.Serialization;

namespace OfflineVideosInfo
{
    [DataContract]
    class ExternalEntry
    {
        [DataMember]
        public long downloaded_bytes;       // downloaded_bytes should be equal to total_bytes if is_completed = true
        [DataMember]
        public long guessed_total_bytes;
        [DataMember]
        public bool is_completed;
        [DataMember]
        public long prefered_video_quality;
        [DataMember]
        public long seasion_id;
        [DataMember]
        public string title;                // Title to show
        [DataMember]
        public long total_bytes;
        [DataMember]
        public long total_time_milli;       // Video length in milli-seconds
        [DataMember]
        public string type_tag;             // like "lua.mp4.bapi.2", used as subfolder name
        [DataMember]
        public EpisodeData ep;
        [DataMember]
        public SourceData source;
    }

    [DataContract]
    class EpisodeData
    {
        [DataMember]
        public long av_id;
        [DataMember]
        public string cover;
        [DataMember]
        public long danmaku;
        [DataMember]
        public long episode_id;
        [DataMember]
        public string index;                // Episode index
        [DataMember]
        public string index_title;          // Episode title
        [DataMember]
        public long page;
    }

    [DataContract]
    class SourceData
    {
        [DataMember]
        public long av_id;
        [DataMember]
        public long cid;
        [DataMember]
        public bool is_default_source;
        [DataMember]
        public string website;
        [DataMember]
        public string webvideo_id;
    }
}
