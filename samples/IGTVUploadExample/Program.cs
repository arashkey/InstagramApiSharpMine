/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */
 
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using InstagramApiSharp.API;
using InstagramApiSharp.API.Builder;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Logger;


// usings>
using InstagramApiSharp.FFmpegFa; // Only exists for .net framework 4.5.2 or newer (wpf, winform, asp.net webform, asp.net mvc)

namespace IGTVUploadExample
{
    class Program
    {
        // IGTV Uploader changed to segmented chunks.
        // We have to split videos to multiple videos and upload it to the instagram's server.
        // You can upload 16:9 or 9:16 videos (horizontal/vertical)

        // I created a tiny FFmpeg wrapper in the following namespace >  InstagramApiSharp.FFmpegFa [which originally created in this repository > https://github.com/ramtinak/FFmpegFa/]
        // But what is FFmpeg? FFmpeg is an cross platform media library which has a lot of feature and one them is creating segmented files from a single media file

        // How to download FFmpeg.exe library? 
        // 64bit: https://ffmpeg.zeranoe.com/builds/win64/static/ffmpeg-20190628-098ab93-win64-static.zip
        // 32bit: https://ffmpeg.zeranoe.com/builds/win32/static/ffmpeg-20190628-098ab93-win32-static.zip
        // Original downloads page> https://ffmpeg.zeranoe.com/builds/

        // Note 1. You have to split videos first and then upload it to the instagram server
        // Note 2. For splitting part you need to setup a folder which has write/read access for output folder [for segmented files]
        // Note 3. Cover/thumbnail photo must be in vertical.
        // Note 4. You don't need to set SetRequestDelay, since it will setting up a delay between every request I decided to create
        //         another classs which will only delay for configuring medias.
        //         

        private static IInstaApi InstaApi;

        readonly static FFmpeg FFmpeg = new FFmpeg("C:\\Libs\\ffmpeg.exe");// creating ffmpeg instance with setting up FFmpeg.exe startup path 


        static void Main(string[] args)
        {
            var result = Task.Run(MainAsync).GetAwaiter().GetResult();
            if (result)
                return;
            Console.ReadKey();
        }
        public static async Task<bool> MainAsync()
        {
            try
            {
                Console.WriteLine("Starting IGTVUploadExample project");

                var videoPath = "E:\\myvideo.mp4"; // only MP4 is accepted

                var coverPhotoPath = "E:\\mycover.jpg";// Must be in Vertical 

                // take a snapshot of video:
                FFmpeg.ExtractImageFromVideo(videoPath, coverPhotoPath, TimeSpan.FromSeconds(2.5));

                var resizedCoverPhotoPath = "E:\\mycoverResized.jpg";
                // we need to resize the thumbnail to an acceptable size 720x1120 is OK  (must be vertical!)
                FFmpeg.ResizeImage(coverPhotoPath, resizedCoverPhotoPath, new ImageSize(720, 1120));
                await Task.Delay(500);;
                coverPhotoPath = resizedCoverPhotoPath;
                var outputFolder = "E:\\VideosToUpload"; // Must have read/write access!!!!

                var segmentedFolderPath = await FFmpeg.SplitVideoAsync(videoPath, outputFolder); // Returns segmented folder path

                await Task.Delay(30000); // wait 30 seconds or more for splitting part

                double length = 0.0; // length/duration is in seconds
                int height = 0, width = 0;
                try
                {
                    var info = FFmpeg.GetInformation(videoPath);// Get video information
                    length = info.Duration.TotalSeconds;
                    if (info.VideoStreams?.Count > 0)
                    {
                        height = info.VideoStreams[0].PixelHeight;
                        width = info.VideoStreams[0].PixelWidth;
                    }
                }
                catch { }

                var userSession = new UserSessionData
                {
                    UserName = "Username",
                    Password = "Password"
                };


                //var delay = RequestDelay.FromSeconds(2, 2);
                var configureDelay = ConfigureMediaDelay.FromSeconds(60, 80); // since we use segmented files, we have to setup a long delay
                // it's related to your files!

                InstaApi = InstaApiBuilder.CreateBuilder()
                    .SetUser(userSession)
                    .UseLogger(new DebugLogger(LogLevel.All)) // use logger for requests and debug messages
                    //.SetRequestDelay(delay)

                    .SetConfigureMediaDelay(configureDelay) // Setting up configuring media delay
                    .Build();

                var loginResult = await InstaApi.LoginAsync();

                if (loginResult.Succeeded)
                {
                    var tvVideo = new InstaTVVideoUpload
                    {
                        VideoThumbnail = new InstaImage { Uri = coverPhotoPath }, // you can set ImageBytes too
                        Height = height,
                        Width = width,
                        Length = length,
                        SegmentedFolderPath = segmentedFolderPath
                    };

                    var uploader = await InstaApi.TVProcessor.UploadSegmentedVideoToTVAsync(tvVideo, "TITLEEEEE", "CAPTION");
                    if (uploader.Succeeded)
                        Console.WriteLine($"Video uploaded successful > {uploader.Value.Url}");
                    else
                        Console.WriteLine($"An error is occured while uploading > {uploader.Info.Message}" );
                }
            }
            catch { }

            return false;
        }
    }
}
