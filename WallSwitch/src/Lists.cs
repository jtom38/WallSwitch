using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WallSwitch
{
    class RSSList
    {
        public int id { get; set; }
        public string tags { get; set; }
        public int file_size { get; set; }
        public string file_url { get; set; }
        public int actual_preview_width { get; set; }
        public int actual_preview_height { get; set; }
        public string sample_url { get; set; }
        public string jpeg_url { get; set; }
        public int jpeg_width { get; set; }
        public int jpeg_height { get; set; }
        public int jpeg_file_size { get; set; }
        public string rating { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public string frames_pending_string { get; set; }
        public string frames_string { get; set; }
        public object flag_detail { get; set; }
    }

    class JSONValues
    {
        public int id { get; set; }
        public string tags { get; set; }
        public int created_at { get; set; }
        public int creator_id { get; set; }
        public string author { get; set; }
        public int change { get; set; }
        public string source { get; set; }
        public int score { get; set; }
        public string md5 { get; set; }
        public int file_size { get; set; }
        public string file_url { get; set; }
        public bool is_shown_in_index { get; set; }
        public string preview_url { get; set; }
        public int preview_width { get; set; }
        public int preview_height { get; set; }
        public int actual_preview_width { get; set; }
        public int actual_preview_height { get; set; }
        public string sample_url { get; set; }
        public int sample_width { get; set; }
        public int sample_height { get; set; }
        public int sample_file_size { get; set; }
        public string jpeg_url { get; set; }
        public int jpeg_width { get; set; }
        public int jpeg_height { get; set; }
        public int jpeg_file_size { get; set; }
        public string rating { get; set; }
        public bool has_children { get; set; }
        public object parent_id { get; set; }
        public string status { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public bool is_held { get; set; }
        public string frames_pending_string { get; set; }
        public List<object> frames_pending { get; set; }
        public string frames_string { get; set; }
        public List<object> frames { get; set; }
        public object flag_detail { get; set; }
    }

    class SQLData
    {
        public int id { get; set; }
        public int siteID { get; set; }
        public string tags { get; set; }
        public string rating { get; set; }
        public string preview_url { get; set; }
        public string jpeg_url { get; set; }
        public int jpeg_width { get; set; }
        public int jpeg_height { get; set; }
        public string date_added { get; set; }
        public int favorite { get; set; }
    }

    class Monitors
    {
        public int MonitorNumber { get; set; }
        public int MonitorWidth { get; set; }
        public int MonitorHeight { get; set; }
    }

    class wallhavenData
    {
        public int id { get; set; }
        public string tags { get; set; }
        public string preview_url { get; set; }
        public string jpeg_url { get; set; }
        public string rating { get; set; }
        public int jpeg_width { get; set; }
        public int jpeg_height { get; set; }
    }

    class tagsData
    {
        public int id { get; set; }
        public string tag { get; set; }
        public string site { get; set; }
    }
}
