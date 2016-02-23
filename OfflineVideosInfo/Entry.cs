using System.Runtime.Serialization;

namespace OfflineVideosInfo
{
    [DataContract]
    class Entry
    {
        [DataMember]
        public long avid;                   // folder name
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
        public long spid;
        [DataMember]
        public string title;                // Title to show
        [DataMember]
        public long total_bytes;
        [DataMember]
        public long total_time_milli;       // Video length in milli-seconds
        [DataMember]
        public string type_tag;             // like "lua.mp4.bapi.2", used as subfolder name
        [DataMember]
        public PageData page_data;
    }

    [DataContract]
    class PageData
    {
        [DataMember]
        public long cid;
        [DataMember]
        public bool downloadable;
        [DataMember]
        public string downloadable_detail;
        [DataMember]
        public bool has_alias;
        [DataMember]
        public long page;
        [DataMember]
        public string part;                // Episode name
        [DataMember]
        public string raw_vid;
        [DataMember]
        public long tid;
        [DataMember]
        public string type;
        [DataMember]
        public string vid;
    }
}
