/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Classes.ResponseWrappers;
using InstagramApiSharp.Enums;
using System.Threading.Tasks;

namespace InstagramApiSharp.API.Processors
{
    /// <summary>
    ///     Discover api functions.
    /// </summary>
    public interface IDiscoverProcessor
    {
        /// <summary>
        ///     Clear Recent searches
        /// </summary>
        Task<IResult<bool>> ClearRecentSearchsAsync();
        /// <summary>
        ///     Dismiss suggestions
        /// </summary>
        /// <param name="targetIdHashtagIdOrStoryId">Target id, hashtag id or story id</param>
        /// <param name="type">Type of suggestions</param>
        Task<IResult<bool>> DismissSuggestionAsync(string targetIdHashtagIdOrStoryId, string type = "tag");
        /// <summary>
        ///     Report an media in Explore page
        /// </summary>
        /// <param name="userId">User Id/Pk (owner of the content)</param>
        /// <param name="mediaId">Media Identifier</param>
        /// <param name="exploreSourceToken">Explore source token (From <see cref="InstaMedia.ExploreSourceToken"/>)</param>
        Task<IResult<bool>> ExploreReportAsync(string userId, string mediaId, string exploreSourceToken);
        /// <summary>
        ///     Hide a search entity
        /// </summary>
        /// <param name="userId">User id/pk to hide</param>
        Task<IResult<bool>> HideSearchEntityAsync(long userId);
        /// <summary>
        ///     Get dynamic searches [supports hashtags and users]
        /// </summary>
        Task<IResult<InstaDynamicSearch>> GetDynamicSearchesAsync();
        /// <summary>
        ///     Get discover user chaining list 
        /// </summary>
        Task<IResult<InstaUserChainingList>> GetChainingUsersAsync();
        /// <summary>
        ///     Get discover user chaining list for specific user
        /// </summary>
        /// <param name="userId">User id (pk)</param>
        Task<IResult<InstaUserChainingList>> GetChainingUsersAsync(long userId);

        /// <summary>
        ///     Get recent searches
        /// </summary>
        Task<IResult<InstaDiscoverRecentSearches>> GetRecentSearchesAsync();

        /// <summary>
        /// Get top searches
        /// </summary>
        /// <param name="querry">querry string of the search</param>
        /// <param name="searchType">Search type(only blended and users works)</param>
        /// <param name="timezone_offset">Timezone offset of the search region (GMT Offset * 60 * 60 - Like Tehran GMT +3:30 = 3.5* 60*60 = 12600)</param>
        /// <returns></returns>
        Task<IResult<InstaDiscoverTopSearches>> GetTopSearchesAsync(string querry = "", InstaDiscoverSearchType searchType = InstaDiscoverSearchType.Users, int timezone_offset = 12600);

        /// <summary>
        ///     Get suggested searches
        /// </summary>
        /// <param name="searchType">Search type(only blended and users works)</param>
        Task<IResult<InstaDiscoverSuggestedSearches>> GetSuggestedSearchesAsync(InstaDiscoverSearchType searchType =
            InstaDiscoverSearchType.Users);
        /// <summary>
        ///     Search user people
        /// </summary>
        /// <param name="query">Query to search</param>
        /// <param name="count">Count</param>
        Task<IResult<InstaDiscoverSearchResult>> SearchPeopleAsync(string query, int count = 50);
        #region Other functions

        /// <summary>
        ///     Sync your phone contact list to instagram
        ///     <para>Note:You can find your friends in instagram with this function</para>
        /// </summary>
        /// <param name="instaContacts">Contact list</param>
        Task<IResult<InstaContactUserList>> SyncContactsAsync(params InstaContact[] instaContacts);
        /// <summary>
        ///     Sync your phone contact list to instagram
        ///     <para>Note:You can find your friends in instagram with this function</para>
        /// </summary>
        /// <param name="instaContacts">Contact list</param>
        Task<IResult<InstaContactUserList>> SyncContactsAsync(InstaContactList instaContacts);

        #endregion Other functions

        ///// <summary>
        ///// NOT COMPLETE
        ///// </summary>
        //Task<IResult<object>> DiscoverPeopleAsync();

    }
}
