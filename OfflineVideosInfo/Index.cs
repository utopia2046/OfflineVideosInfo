using System.Runtime.Serialization;

namespace OfflineVideosInfo
{
    [DataContract]
    class Index
    {
        [DataMember]
        public long available_period_milli;
        [DataMember]
        public string description;              // Info
        [DataMember]
        public string from;
        [DataMember]
        public string index_mrl;
        [DataMember]
        public bool is_downloaded;
        [DataMember]
        public bool is_resolved;
        [DataMember]
        public bool is_stub;
        [DataMember]
        public long local_proxy_type;
        [DataMember]
        public bool need_faad;
        [DataMember]
        public bool need_membuf;
        [DataMember]
        public bool need_ringbuf;
        [DataMember]
        public string normal_mrl;                 // Original downloaded video url
        [DataMember]
        public long parsed_milli;
        [DataMember]
        public bool prefer_vlc;
        [DataMember]
        public long psedo_bitrate;
        [DataMember]
        public string type_tag;
        [DataMember]
        public string user_agent;
        [DataMember]
        public PlayerCodeConfig[] player_codec_config_list;
        [DataMember]
        public Segment[] segment_list;
    }

    [DataContract]
    class PlayerCodeConfig
    {
        [DataMember]
        public long media_codec_direct;
        [DataMember]
        public string player;
        [DataMember]
        public bool use_ijk_media_codec;
        [DataMember]
        public bool use_list_player;
        [DataMember]
        public bool use_mdeia_codec;
        [DataMember]
        public bool use_open_max_il;
    }

    [DataContract]
    class Segment
    {
        [DataMember]
        public long bytes;
        [DataMember]
        public long duration;
        [DataMember]
        public string url;
    }
}
