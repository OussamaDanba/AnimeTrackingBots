using System.Collections.Generic;

// FUNimation JSON format
namespace FUNimationBot
{
    public class Episode
    {
        public string asset_id { get; set; }
        public string funimation_id { get; set; }
        public string pubDate { get; set; }
        public string rating { get; set; }
        public string quality { get; set; }
        public string language { get; set; }
        public int duration { get; set; }
        public string simulcast { get; set; }
        public string closed_captioning { get; set; }
        public string url { get; set; }
        public string promo { get; set; }
        public string show_name { get; set; }
        public string popularity { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string sequence { get; set; }
        public decimal? number { get; set; }
        public string video_type { get; set; }
        public string show_id { get; set; }
        public string thumbnail { get; set; }
        public string season_id { get; set; }
        public string season_number { get; set; }
        public string genre { get; set; }
        public string releaseDate { get; set; }
        public string thumbnail_url { get; set; }
        public string thumbnail_small { get; set; }
        public string thumbnail_medium { get; set; }
        public string thumbnail_large { get; set; }
        public string video_url { get; set; }
        public string closed_caption_location { get; set; }
        public List<string> aips { get; set; }
        public string dub_sub { get; set; }
        public string featured { get; set; }
        public string highdef { get; set; }
        public string has_subtitles { get; set; }
        public int element_position { get; set; }
        public string tv_or_move { get; set; }
        public string rating_system { get; set; }
        public int display_order { get; set; }
        public string extended_title { get; set; }
    }

    public class Episodes
    {
        public List<Episode> EpisodesList { get; set; }
    }
}
