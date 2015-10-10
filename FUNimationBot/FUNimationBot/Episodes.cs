using Newtonsoft.Json;
using System.Collections.Generic;

// FUNimation JSON format
namespace FUNimationBot
{
    public class Episode
    {
        [JsonProperty("asset_id")]
        public string AssetId { get; set; }

        [JsonProperty("funimation_id")]
        public string FUNimationId { get; set; }

        [JsonProperty("pubDate")]
        public string PublicationDate { get; set; }

        [JsonProperty("rating")]
        public string Rating { get; set; }

        [JsonProperty("quality")]
        public string Quality { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("duration")]
        public int Duration { get; set; }

        [JsonProperty("simulcast")]
        public string Simulcast { get; set; }

        [JsonProperty("closed_captioning")]
        public string ClosedCaptioning { get; set; }

        [JsonProperty("url")]
        public string URL { get; set; }

        [JsonProperty("promo")]
        public string Promo { get; set; }

        [JsonProperty("show_name")]
        public string ShowName { get; set; }

        [JsonProperty("popularity")]
        public string Popularity { get; set; }

        [JsonProperty("title")]
        public string EpisodeTitle { get; set; }

        [JsonProperty("description")]
        public string EpisodeDescription { get; set; }

        [JsonProperty("sequence")]
        public string Sequence { get; set; }

        [JsonProperty("number")]
        public decimal? EpisodeNumber { get; set; }

        [JsonProperty("video_type")]
        public string VideoType { get; set; }

        [JsonProperty("show_id")]
        public string ShowId { get; set; }

        [JsonProperty("thumbnail")]
        public string Thumbnail { get; set; }

        [JsonProperty("season_id")]
        public string SeasonId { get; set; }

        [JsonProperty("season_number")]
        public string SeasonNumber { get; set; }

        [JsonProperty("genre")]
        public string Genre { get; set; }

        [JsonProperty("releaseDate")]
        public string ReleaseDate { get; set; }

        [JsonProperty("thumbnail_url")]
        public string ThumbnailUrl { get; set; }

        [JsonProperty("thumbnail_small")]
        public string ThumbnailSmall { get; set; }

        [JsonProperty("thumbnail_medium")]
        public string ThumbnailMedium { get; set; }

        [JsonProperty("thumbnail_large")]
        public string ThumbnailLarge { get; set; }

        [JsonProperty("video_url")]
        public string VideoURL { get; set; }

        [JsonProperty("closed_caption_location")]
        public string ClosedCaptionLocation { get; set; }

        // Stands for Ad Interrupts?
        [JsonProperty("aips")]
        public List<string> Aips { get; set; }

        [JsonProperty("dub_sub")]
        public string DubSub { get; set; }

        [JsonProperty("featured")]
        public string Featured { get; set; }

        [JsonProperty("highdef")]
        public string HighDef { get; set; }

        [JsonProperty("has_subtitles")]
        public string HasSubtitles { get; set; }

        [JsonProperty("element_position")]
        public int ElementPosition { get; set; }

        // Seems like a typo
        [JsonProperty("tv_or_move")]
        public string TVOrMovie { get; set; }

        [JsonProperty("rating_system")]
        public string RatingSystem { get; set; }

        [JsonProperty("display_order")]
        public int DisplayOrder { get; set; }

        [JsonProperty("extended_title")]
        public string ExtendedTitle { get; set; }
    }

    public class Episodes
    {
        [JsonProperty("videos")]
        public List<Episode> EpisodesList { get; set; }
    }
}
