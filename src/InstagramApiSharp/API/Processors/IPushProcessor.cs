/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using InstagramApiSharp.Classes;
using InstagramApiSharp.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace InstagramApiSharp.API.Processors
{
    public interface IPushProcessor
    {
        /// <summary>
        ///     Register application for push notifications
        /// </summary>
        Task<IResult<bool>> RegisterPushAsync(InstaPushChannelType pushChannelType = InstaPushChannelType.Mqtt);
    }
}
