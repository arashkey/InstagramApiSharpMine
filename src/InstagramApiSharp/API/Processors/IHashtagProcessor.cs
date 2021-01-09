using System.Collections.Generic;
using System.Threading.Tasks;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Enums;

namespace InstagramApiSharp.API.Processors
{
    /// <summary>
    ///     Hashtag api functions.
    /// </summary>
    public interface IHashtagProcessor
    {
        /// <summary>
        ///     Seen hashtag story as seen
        /// </summary>
        /// <param name="hashtagId">Hashtag Id</param>
        /// <param name="storyMediaId">Story media identifier</param>
        /// <param name="takenAtUnix">Taken at unix</param>
        Task<IResult<bool>> MarkHashtagStoryAsSeenAsync(string hashtagId, string storyMediaId, long takenAtUnix);
        /// <summary>
        ///     Get medias for hashtag channel
        /// </summary>
        /// <param name="channelId">Channel id</param>
        /// <param name="firstMediaId">First media id</param>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <returns>
        ///     <see cref="InstaMediaList" />
        /// </returns>
        Task<IResult<InstaMediaList>> GetHashtagChannelVideosAsync(string channelId, string firstMediaId, PaginationParameters paginationParameters);

        /// <summary>
        ///     Get hashtag sections
        /// </summary>
        /// <param name="tagname">Tag name</param>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <param name="hashtagSectionType">Section type</param>
        Task<IResult<InstaSectionMedia>> GetHashtagsSectionsAsync(string tagname, PaginationParameters paginationParameters, InstaHashtagSectionType hashtagSectionType = InstaHashtagSectionType.All);

        /// <summary>
        ///     Get Hashtags posts
        /// </summary>
        /// <param name="mediaId">Media Identifier</param>
        Task<IResult<InstaHashtagSearch>> GetHashtagsPostsAsync(string mediaId);

        /// <summary>
        ///     Follow a hashtag
        /// </summary>
        /// <param name="tagname">Tag name</param>
        Task<IResult<bool>> FollowHashtagAsync(string tagname);

        /// <summary>
        ///     Get following hashtags information
        /// </summary>
        /// <param name="userId">User identifier (pk)</param>
        /// <returns>
        ///     List of hashtags
        /// </returns>
        Task<IResult<InstaHashtagSearch>> GetFollowingHashtagsInfoAsync(long userId);

        /// <summary>
        ///     Gets the hashtag information by user tagname.
        /// </summary>
        /// <param name="tagname">Tagname</param>
        /// <returns>Hashtag information</returns>
        Task<IResult<InstaHashtag>> GetHashtagInfoAsync(string tagname);

        /// <summary>
        ///     Get stories of an hashtag
        /// </summary>
        /// <param name="tagname">Tag name</param>
        Task<IResult<InstaHashtagStory>> GetHashtagStoriesAsync(string tagname);

        /// <summary>
        ///     Get recent hashtag media list
        /// </summary>
        /// <param name="tagname">Tag name</param>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        Task<IResult<InstaSectionMedia>> GetRecentHashtagMediaListAsync(string tagname, PaginationParameters paginationParameters);

        /// <summary>
        ///     Get suggested hashtags
        /// </summary>
        /// <returns>
        ///     List of hashtags
        /// </returns>
        Task<IResult<InstaHashtagSearch>> GetSuggestedHashtagsAsync();

        /// <summary>
        ///     Get top (ranked) hashtag media list
        /// </summary>
        /// <param name="tagname">Tag name</param>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        Task<IResult<InstaSectionMedia>> GetTopHashtagMediaListAsync(string tagname, PaginationParameters paginationParameters);

        /// <summary>
        ///     Searches for specific hashtag by search query.
        /// </summary>
        /// <param name="query">Search query</param>
        /// <param name="excludeList">Array of numerical hashtag IDs (ie "17841562498105353") to exclude from the response, allowing you to skip tags from a previous call to get more results</param>
        /// <returns>
        ///     List of hashtags
        /// </returns>
        Task<IResult<InstaHashtagSearch>> SearchHashtagAsync(string query, IEnumerable<long> excludeList = null);
        /// <summary>
        ///     Report an hashtag media
        /// </summary>
        /// <param name="tagname">Tag name</param>
        /// <param name="hashtagId">Tag Id</param>
        /// <param name="mediaId">Media identifier</param>
        Task<IResult<bool>> ReportHashtagMediaAsync(string tagname, string hashtagId, string mediaId);
        /// <summary>
        ///     Unfollow a hashtag
        /// </summary>
        /// <param name="tagname">Tag name</param>
        Task<IResult<bool>> UnFollowHashtagAsync(string tagname);
    }
}