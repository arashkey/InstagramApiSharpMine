/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * IRANIAN DEVELOPERS
 */
using Newtonsoft.Json;
using InstagramApiSharp.Classes.ResponseWrappers;
using System;
using System.Collections.Generic;
using System.Text;
#pragma warning disable IDE1006 // Naming Styles

namespace InstagramApiSharp.Classes.Models
{
    public class InstaStickers : InstaDefaultResponse
    {
        [JsonProperty("static_stickers")]
        public List<InstaStaticStickers> StaticStickers { get; set; } = new List<InstaStaticStickers>();
        [JsonProperty("version")]
        public int Version { get; set; }
        [JsonProperty("composer_config")]
        public InstaStickerComposerConfig ComposerConfig { get; set; }
    }

    public class InstaStickerComposerConfig
    {
        [JsonProperty("swipe_up_urls")]
        public bool SwipeUpUrls { get; set; }
        [JsonProperty("felix_links")]
        public bool Felix_links { get; set; }
        [JsonProperty("total_ar_effects")]
        public int Total_ar_effects { get; set; }
        [JsonProperty("profile_shop_links")]
        public bool Profile_shopLinks { get; set; }
        [JsonProperty("shopping_link_more_options")]
        public bool ShoppingLink_more_options { get; set; }
        [JsonProperty("shopping_collection_links")]
        public bool Shopping_collectionLinks { get; set; }
        [JsonProperty("shopping_product_collection_links")]
        public bool ShoppingProduct_collectionLinks { get; set; }
        [JsonProperty("shopping_product_links")]
        public bool ShoppingProductLinks { get; set; }
    }

    public class InstaStaticStickers
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("stickers")]
        public List<InstaStickerItem> Stickers { get; set; } = new List<InstaStickerItem>();
        [JsonProperty("include_in_recent")]
        public bool Include_in_recent { get; set; }
        [JsonProperty("keywords")]
        public string[] Keywords { get; set; }
        [JsonProperty("has_attribution")]
        public object Has_attribution { get; set; }
        [JsonProperty("available_in_direct")]
        public bool Available_in_direct { get; set; }
        [JsonProperty("help_text")]
        public string Help_text { get; set; }
        [JsonProperty("bounding_box_enabled")]
        public bool Bounding_box_enabled { get; set; }
        [JsonProperty("minimum_scale")]
        public float Minimum_scale { get; set; }
        [JsonProperty("maximum_scale")]
        public float Maximum_scale { get; set; }
        [JsonProperty("allow_flippability")]
        public bool Allow_flippability { get; set; }
        [JsonProperty("prompt")]
        public string prompt { get; set; }
        [JsonProperty("suggestions")]
        public object[] suggestions { get; set; }
        [JsonProperty("bloks_app")]
        public string bloks_app { get; set; }
        [JsonProperty("reel_media_sticker_limit")]
        public int reel_media_sticker_limit { get; set; }
    }

    public class InstaStickerItem
    {
        [JsonProperty("id")]
        public object id { get; set; }
        [JsonProperty("type")]
        public string type { get; set; }
        [JsonProperty("name")]
        public string name { get; set; }
        [JsonProperty("image_url")]
        public string image_url { get; set; }
        [JsonProperty("image_width_ratio")]
        public float image_width_ratio { get; set; }
        [JsonProperty("tray_image_width_ratio")]
        public float tray_image_width_ratio { get; set; }
        [JsonProperty("image_width")]
        public int image_width { get; set; }
        [JsonProperty("image_height")]
        public int image_height { get; set; }
        [JsonProperty("poll_id")]
        public string poll_id { get; set; }
        [JsonProperty("is_editable")]
        public int is_editable { get; set; }
        [JsonProperty("tallies")]
        public List<InstaStoryTalliesItemResponse> tallies { get; set; } = new List<InstaStoryTalliesItemResponse>();
        [JsonProperty("question")]
        public string question { get; set; }
        [JsonProperty("profile_pic_url")]
        public string profile_pic_url { get; set; }
        [JsonProperty("question_types")]
        public string[] question_types { get; set; }
        [JsonProperty("has_countdowns")]
        public bool has_countdowns { get; set; }
        [JsonProperty("emoji")]
        public string emoji { get; set; }
        [JsonProperty("text")]
        public string text { get; set; }
        [JsonProperty("font_size")]
        public int font_size { get; set; }
        [JsonProperty("text_color")]
        public string text_color { get; set; }
        [JsonProperty("text_background_color")]
        public string text_background_color { get; set; }
        [JsonProperty("text_background_alpha")]
        public int text_background_alpha { get; set; }
        [JsonProperty("text_x")]
        public float text_x { get; set; }
        [JsonProperty("text_y")]
        public float text_y { get; set; }
        [JsonProperty("attribution")]
        public string attribution { get; set; }
        [JsonProperty("hashtag")]
        public InstaStickerHashtag hashtag { get; set; }
    }

    public class InstaStickerHashtag
    {
        [JsonProperty("")]
        public string name { get; set; }
        [JsonProperty("id")]
        public long id { get; set; }
    }

}
#pragma warning restore IDE1006 // Naming Styles
