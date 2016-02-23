using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.Runtime.Serialization.Json;
using System.Xml.Linq;

namespace OfflineVideosInfo
{
    class VideoInfo
    {
        const string entryFile = @"entry.json";
        const string indexFile = @"index.json";
        static string[] videoExtensions = { "*.mp4", "*.flv" };

        public long Id { get; set; }
        public string Title { get; set; }
        public long EpisodeNumber { get; set; }
        public string EpisodeDescription { get; set; }
        public string Description { get; set; }
        public string TypeTag { get; set; }
        public bool IsCompleted { get; set; }
        public long TotalBytes { get; set; }
        public long TotalTimeInMisecs { get; set; }
        public string OriginalUrl { get; set; }
        public string VideoFilePath { get; set; }
        public DateTime FileUpdatedTime { get; set; }

        public static VideoInfo[] TryParseAll(string rootPath)
        {
            var infos = new List<VideoInfo>();

            Trace.TraceInformation("Start to look up video info under root path: {0}", rootPath);
            var avFolders = Directory.GetDirectories(rootPath);
            foreach (var avFolder in avFolders)
            {
                var avInfo = VideoInfo.ParseAvFolder(avFolder);
                infos.AddRange(avInfo);
            }
            Trace.TraceInformation("End look up video info under folder: {0}", rootPath);
            Trace.TraceInformation("  {0} video entries found", infos.Count);

            // TODO: Remove this temp code
            /*
            infos.Add(new VideoInfo() {
                Id = 798797,
                Title = "Video Title",
                Description = "Test video",
                EpisodeNumber = 1,
                EpisodeDescription = "Test video episode 1",
                IsCompleted = false,
                FileUpdatedTime = DateTime.Now,
                OriginalUrl = "http://original.url",
                TypeTag = "",
                VideoFilePath = "D:\\TEMP"
            });
            */

            return infos.ToArray();
        }

        public static void WriteToXml(VideoInfo[] infoList, string xmlFileName)
        {
            XDocument xdoc = new XDocument();
            XElement root = new XElement("VideoInfoList");
            foreach (var info in infoList)
            {
                var xe = new XElement("Info");
                xe.Add(new XElement("Id", info.Id));
                xe.Add(new XElement("Title", info.Title));
                xe.Add(new XElement("EpisodeNumber", info.EpisodeNumber));
                xe.Add(new XElement("EpisodeDescription", info.EpisodeDescription));
                xe.Add(new XElement("Description", info.Description));
                xe.Add(new XElement("TypeTag", info.TypeTag));
                xe.Add(new XElement("IsCompleted", info.IsCompleted));
                xe.Add(new XElement("TotalBytes", info.TotalBytes));
                xe.Add(new XElement("TotalTimeInMisecs", info.TotalTimeInMisecs));
                xe.Add(new XElement("OriginalUrl", info.OriginalUrl));
                xe.Add(new XElement("VideoFilePath", info.VideoFilePath));
                xe.Add(new XElement("FileUpdatedTime", info.FileUpdatedTime));
                root.AddFirst(xe);
            }
            xdoc.Add(root);
            xdoc.Save(xmlFileName, SaveOptions.OmitDuplicateNamespaces);
        }

        public static void WriteUnfinishedVideoToHtml(VideoInfo[] infoList, string htmlFileName)
        {
            XDocument xdoc = new XDocument();
            XElement root = new XElement("Body");
            foreach (var info in infoList)
            {
                if (!info.IsCompleted)
                {
                    var xa = new XElement("a", info.Title);
                    xa.SetAttributeValue("href", info.OriginalUrl);
                    root.Add(new XElement("p", xa));
                }
            }
            xdoc.Add(root);
            xdoc.Save(htmlFileName, SaveOptions.OmitDuplicateNamespaces);
        }

        private static VideoInfo[] ParseAvFolder(string path)
        {
            var infoList = new List<VideoInfo>();

            Trace.TraceInformation("Begin looking for episode under folder: {0}", path);
            var episodeFolders = Directory.GetDirectories(path);
            foreach (var epFolder in episodeFolders)
            {
                var info = ParseEpisode(epFolder);
                infoList.Add(info);
            }
            Trace.TraceInformation("End looking for episodes under folder: {0}", path);
            Trace.TraceInformation("  {0} episodes found", infoList.Count);

            return infoList.ToArray();
        }

        private static VideoInfo ParseEpisode(string path)
        {
            VideoInfo info = null;

            string entryFilePath = Path.Combine(path, entryFile);
            info = ParseEntryJson(entryFilePath);

            var contentFolders = Directory.GetDirectories(path);
            if (contentFolders.Length != 1)
            {
                Trace.TraceWarning("No content folder found under path {0}", path);
                return null;
            }

            var contentFolder = contentFolders.First();

            ParseIndexJson(contentFolder, ref info);
            FindVideoFile(contentFolder, ref info);

            return info;
        }

        private static VideoInfo ParseEntryJson(string entryJsonPath)
        {
            if (!File.Exists(entryJsonPath))
            {
                Trace.TraceError(entryJsonPath + " not found.");
                return null;
            }

            string entryJson = ReadTextFile(entryJsonPath);
            var entry = ParseJsonString<Entry>(entryJson);

            Trace.TraceInformation("Parsing entry.json done successfully. Video Id = {0}, Title = {1}", entry.avid, entry.title);
            var info = new VideoInfo() {
                Id = entry.avid,
                Title = entry.title,
                EpisodeNumber = entry.page_data.page,
                EpisodeDescription = entry.page_data.part,
                TypeTag = entry.type_tag,
                IsCompleted = entry.is_completed,
                TotalBytes = entry.total_bytes,
                TotalTimeInMisecs = entry.total_time_milli
            };

            Trace.TraceInformation("End parsing entry.json file.");

            return info;
        }

        private static VideoInfo ParseIndexJson(string contentFolder, ref VideoInfo info)
        {
            var indexJsonPath = Path.Combine(contentFolder, "index.json");
            if (!File.Exists(indexJsonPath))
            {
                Trace.TraceError(indexJsonPath + " not found.");
                return null;
            }

            string indexJson = ReadTextFile(indexJsonPath);
            var index = ParseJsonString<Index>(indexJson);

            Trace.TraceInformation("Parsing index.json done successfully. ", index.description, index.normal_mrl);
            info.Description = index.description;
            info.OriginalUrl = index.normal_mrl;

            return info;
        }

        private static void FindVideoFile(string contentFolder, ref VideoInfo info)
        {
            string videoFile = null;

            foreach (var ext in videoExtensions)
            {
                Trace.TraceInformation("Looking for {0} files under contentFolder {1}", ext, contentFolder);
                var files = Directory.GetFiles(contentFolder, "*.mp4", SearchOption.TopDirectoryOnly);
                if (files.Count() < 1)
                {
                    Trace.TraceInformation("No {0} files found.", ext);
                }
                else
                {
                    videoFile = files.First();
                    Trace.TraceInformation("Found video file {0}", videoFile);
                    break;
                }
            }

            if (!String.IsNullOrEmpty(videoFile))
            {
                info.VideoFilePath = videoFile;
                var fi = new FileInfo(videoFile);
                info.FileUpdatedTime = fi.LastWriteTime;
            }
        }

        private static string ReadTextFile(string filePath)
        {
            string content;

            Trace.TraceInformation("Reading file: {0}", filePath);
            using (var sr = new StreamReader(filePath))
            {
                content = sr.ReadToEnd();
            }

            return content;
        }

        private static T ParseJsonString<T>(string jsonString)
        {
            Trace.TraceInformation("Parsing Type {0} json string: {1}", typeof(T).ToString(), jsonString);
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            var jsonReader = new DataContractJsonSerializer(typeof(T));
            return (T)jsonReader.ReadObject(stream);
        }
    }
}
