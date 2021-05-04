/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharpMine
 * 
 * IRANIAN DEVELOPERS
 */

using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Enums;
using System;
using System.Threading.Tasks;

namespace InstagramApiSharp.API.Processors
{
    public interface IReelProcessor
    {
        /// <summary>
        ///     Upload reel video
        /// </summary>
        /// <param name="video">Video to upload.<para>Note: Thumbnail is required.</para></param>
        /// <param name="caption">Caption => Optional</param>
        Task<IResult<InstaMedia>> UploadReelVideoAsync(InstaVideoUpload video, string caption);

        /// <summary>
        ///     Upload reel video with progress
        /// </summary>
        /// <param name="progress">Progress action</param>
        /// <param name="video">Video to upload.<para>Note: Thumbnail is required.</para></param>
        /// <param name="caption">Caption => Optional</param>
        Task<IResult<InstaMedia>> UploadReelVideoAsync(Action<InstaUploaderProgress> progress,
            InstaVideoUpload video, string caption);
    }
}
