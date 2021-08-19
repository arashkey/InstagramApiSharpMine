using System.Threading;
using System.Threading.Tasks;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Enums;

namespace InstagramApiSharp.API.Processors
{
    /// <summary>
    ///     Comments api functions.
    /// </summary>
    public interface ICommentProcessor
    {
        /// <summary>
        ///     Check offensive text for caption
        /// </summary>
        /// <param name="captionText">Caption text</param>
        Task<IResult<InstaOffensiveText>> CheckOffensiveCaptionAsync(string captionText);

        /// <summary>
        ///     Check offensive text for comment
        /// </summary>
        /// <param name="mediaId">Media identifier</param>
        /// <param name="commentText">Comment text</param>
        Task<IResult<InstaOffensiveText>> CheckOffensiveTextAsync(string mediaId, string commentText);

        /// <summary>
        ///     Block an user from commenting to medias
        /// </summary>
        /// <param name="userIds">User ids (pk)</param>
        Task<IResult<bool>> BlockUserCommentingAsync(params long[] userIds);

        /// <summary>
        ///     Comment media
        /// </summary>
        /// <param name="mediaId">Media id</param>
        /// <param name="text">Comment text</param>
        /// <param name="containerModule">Container module</param>
        /// <param name="feedPosition">Feed position</param>
        /// <param name="isCarouselBumpedPost">Is carousel post?</param>
        /// <param name="carouselIndex">Carousel index</param>
        Task<IResult<InstaComment>> CommentMediaAsync(string mediaId, string text,
            InstaCommentContainerModuleType containerModule = InstaCommentContainerModuleType.FeedTimeline,
            uint feedPosition = 0, bool isCarouselBumpedPost = false, int? carouselIndex = null);

        /// <summary>
        ///     Delete media comment
        /// </summary>
        /// <param name="mediaId">Media id</param>
        /// <param name="commentId">Comment id</param>
        Task<IResult<bool>> DeleteCommentAsync(string mediaId, string commentId);

        /// <summary>
        ///     Delete media comments(multiple)
        /// </summary>
        /// <param name="mediaId">Media id</param>
        /// <param name="commentIds">Comment id</param>
        Task<IResult<bool>> DeleteMultipleCommentsAsync(string mediaId, params string[] commentIds);

        /// <summary>
        ///     Disable media comments
        /// </summary>
        /// <param name="mediaId">Media id</param>
        Task<IResult<bool>> DisableMediaCommentAsync(string mediaId);

        /// <summary>
        ///     Allow media comments
        /// </summary>
        /// <param name="mediaId">Media id</param>
        Task<IResult<bool>> EnableMediaCommentAsync(string mediaId);

        /// <summary>
        ///     Get blocked users from commenting
        /// </summary>
        Task<IResult<InstaUserShortList>> GetBlockedCommentersAsync();

        /// <summary>
        ///     Get media comments likers
        /// </summary>
        /// <param name="mediaId">Media id</param>
        Task<IResult<InstaLikersList>> GetMediaCommentLikersAsync(string mediaId);

        /// <summary>
        ///     Get media comments
        /// </summary>
        /// <param name="mediaId">Media id</param>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <param name="targetCommentId">Target comment id</param>
        Task<IResult<InstaCommentList>>
            GetMediaCommentsAsync(string mediaId, PaginationParameters paginationParameters, string targetCommentId = "");

        /// <summary>
        ///     Get media comments
        /// </summary>
        /// <param name="mediaId">Media id</param>
        /// <param name="paginationParameters">Pagination parameters: next id and max amount of pages to load</param>
        /// <param name="targetCommentId">Target comment id</param>
        /// <param name="cancellationToken">Cancellation token</param>
        Task<IResult<InstaCommentList>>
            GetMediaCommentsAsync(string mediaId, PaginationParameters paginationParameters, 
            CancellationToken cancellationToken, string targetCommentId = "");

        /// <summary>
        ///     Get media inline comments
        /// </summary>
        /// <param name="mediaId">Media id</param>
        /// <param name="targetCommentId">Target comment id</param>
        /// <param name="paginationParameters">Maximum amount of pages to load and start id</param>
        Task<IResult<InstaInlineCommentList>>
           GetMediaRepliesCommentsAsync(string mediaId, string targetCommentId, PaginationParameters paginationParameters);

        /// <summary>
        ///     Get media inline comments
        /// </summary>
        /// <param name="mediaId">Media id</param>
        /// <param name="targetCommentId">Target comment id</param>
        /// <param name="paginationParameters">Maximum amount of pages to load and start id</param>
        /// <param name="cancellationToken">Cancellation token</param>
        Task<IResult<InstaInlineCommentList>>
           GetMediaRepliesCommentsAsync(string mediaId, string targetCommentId, PaginationParameters paginationParameters, 
            CancellationToken cancellationToken);

        /// <summary>
        ///     Like media comment
        /// </summary>
        /// <param name="commentId">pass Pk.Tostring() for commentId</param>
        Task<IResult<bool>> LikeCommentAsync(string commentId);

        /// <summary>
        ///     Inline comment media
        /// </summary>
        /// <param name="mediaId">Media id</param>
        /// <param name="targetCommentId">Target comment id</param>
        /// <param name="text">Comment text</param>
        /// <param name="containerModule">Container module</param>
        /// <param name="feedPosition">Feed position</param>
        /// <param name="isCarouselBumpedPost">Is carousel post?</param>
        /// <param name="carouselIndex">Carousel index</param>
        /// <param name="inventorySource">Inventory source</param>
        Task<IResult<InstaComment>> ReplyCommentMediaAsync(string mediaId, string targetCommentId, string text,
            InstaCommentContainerModuleType containerModule = InstaCommentContainerModuleType.FeedTimeline,
            uint feedPosition = 0, bool isCarouselBumpedPost = false, int? carouselIndex = null,
            InstaMediaInventorySource inventorySource = InstaMediaInventorySource.MediaOrAdd);

        /// <summary>
        ///     Report media comment
        /// </summary>
        /// <param name="mediaId">Media id</param>
        /// <param name="commentId">Comment id</param>
        Task<IResult<bool>> ReportCommentAsync(string mediaId, string commentId);

        /// <summary>
        ///     Unblock an user from commenting to medias
        /// </summary>
        /// <param name="userIds">User ids (pk)</param>
        Task<IResult<bool>> UnblockUserCommentingAsync(params long[] userIds);

        /// <summary>
        ///     Unlike media comment
        /// </summary>
        /// <param name="commentId">Comment id</param>
        Task<IResult<bool>> UnlikeCommentAsync(string commentId);

        /// <summary>
        ///     Translate comment or captions
        ///     <para>Note: use this function to translate captions too! (i.e: <see cref="InstaCaption.Pk"/>)</para>
        /// </summary>
        /// <param name="commentIds">Comment id(s) (Array of <see cref="InstaComment.Pk"/>)</param>
        Task<IResult<InstaTranslateList>> TranslateCommentAsync(params long[] commentIds);
    }
}