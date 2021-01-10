/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * IRANIAN DEVELOPERS
 */
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
namespace InstagramApiSharp.API.Processors
{
    public interface ICreativeProcessor
    {
        Task<IResult<bool>> WriteSupportedCapablititiesAsync();
        Task<IResult<InstaStickers>> GetAssetsAsync();
    }
}
