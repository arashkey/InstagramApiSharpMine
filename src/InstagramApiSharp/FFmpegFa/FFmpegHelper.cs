#if !WINDOWS_UWP && !NETSTANDARD

/*
 * Credit Ramtin Jokar
 * Github: https://github.com/ramtinak/FFmpegFa/
 * 
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InstagramApiSharp.FFmpegFa
{
    public class FFmpeg
    {
        public bool IsDebugMode { get; set; } = false;
        public string FFmpegStartupPath { get; private set; }

        public event FFmpegProgressChanged OnProgressChanged;

        FFmpegProgress progress = new FFmpegProgress();

        public FFmpeg(string ffmpegStartupPath)
        {
            if (!ffmpegStartupPath.EndsWith("ffmpeg.exe"))
            {
                if (ffmpegStartupPath.EndsWith("\\"))
                    ffmpegStartupPath += @"ffmpeg.exe";
                else
                    ffmpegStartupPath += @"\ffmpeg.exe";
            }
            FFmpegStartupPath = ffmpegStartupPath;
        }
        ~FFmpeg() => Dispose();

        /// <summary>
        ///     Get media information
        /// </summary>
        /// <param name="filePath">File path</param>
        public FFmpegInfo GetInformation(string filePath)
        {
            string name;
            if (filePath.Contains("\\"))
                name = Path.GetFileName(filePath);
            else name = filePath;

            var content = RunCommandAndGetResponse($"-i \"{filePath}\"");
            FFmpegInfo fFmpegInfo = new FFmpegInfo(content, name);
            return fFmpegInfo;
        }


        /// <summary>
        ///     Split video to segemnets,
        ///     Note: OutputFolder most have Write access! 
        /// </summary>
        /// <param name="inputFile">Input video file (mp4 only)</param>
        /// <param name="outputFolder">Output folder (Most have Read/Write Access!!!)</param>
        public async Task<string> SplitVideoAsync(string inputFile, string outputFolder)
        {
            if (string.IsNullOrEmpty(inputFile))
                throw new Exception("Inputfile is empty. Choose an input file.");
            if (string.IsNullOrEmpty(outputFolder))
                throw new Exception("outputFolder is empty. Choose an output folder.");

            var cmd = string.Empty;

            var buffer = File.ReadAllBytes(inputFile);
            string fileSha1 = "";
            using (var cryptoProvider = new SHA1CryptoServiceProvider())
                fileSha1 = BitConverter.ToString(cryptoProvider.ComputeHash(buffer)).Replace("-","");

            var newFolder = $"{outputFolder}\\{Path.GetRandomFileName()}";
            Directory.CreateDirectory(newFolder);

            var output = $"{newFolder}\\{fileSha1}_video_%03d.mp4";

            //cmd = $"-i \"{inputFile}\" -c:v copy -c:a copy -dn -sn -f segment -segment_time 00:00:08 -segment_format mp4 \"{outputFolder}\\video.%03d.mp4\"";
            cmd = $"-i \"{inputFile}\" -c:v copy -an -dn -sn -f segment -segment_time 00:00:10 -segment_format mp4 \"{output}\"";

            Process process = new Process();
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.FileName = FFmpegStartupPath;
            process.StartInfo.Arguments = cmd;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();

            new Thread(new ThreadStart(() => {

                StreamReader sr = process.StandardError;
                Output("Start.....");
                var name = "";
                if (inputFile.Contains("\\"))
                    name = Path.GetFileName(inputFile);
                else name = inputFile;


                progress = new FFmpegProgress();
                bool d = false;
                TimeSpan totalTimeSpan = TimeSpan.FromMilliseconds(0);
                Output("Total:" + totalTimeSpan.TotalMilliseconds);
                //frame=   18 fps=0.0 q=0.0 size=       7kB time=00:00:00.88 bitrate=  68.8kbits/s speed=1.69x   
                while (!sr.EndOfStream)
                {
                    var v = (sr.ReadLine());
                    Output(v);
                    if (v.ToLower().Contains("duration") && !d)
                    {
                        try
                        {
                            FFmpegInfo fFmpegInfo = new FFmpegInfo(v, name);
                            totalTimeSpan = fFmpegInfo.Duration;
                            progress.InputFileInfo = fFmpegInfo;
                            d = true;
                        }
                        catch { }
                    }
                    if (v.Contains("time=") && totalTimeSpan.TotalMilliseconds != 0)
                    {
                        try
                        {
                            SendProgress(totalTimeSpan, v);
                        }
                        catch { }
                    }
                }
                try
                {
                    SendProgress(totalTimeSpan, ".:::END:::.");
                    // 100%
                }
                catch { }
                Output("End.....");

            })).Start();
            ExtractAudio(inputFile, newFolder, fileSha1);

            await Task.Delay(20000);//30 seconds

            return newFolder;
        }
        void ExtractAudio(string inputFile, string outputFolder, string sha1)
        {
            if (string.IsNullOrEmpty(inputFile))
                throw new Exception("Inputfile is empty. Choose an input file.");
            if (string.IsNullOrEmpty(outputFolder))
                throw new Exception("outputFolder is empty. Choose an output folder.");



            var cmd = string.Empty;
            cmd = $"-i \"{inputFile}\" -c:a copy -vn -dn -sn -f mp4 \"{outputFolder}\\{sha1}_xaudiox.mp4\"";

            Process process = new Process();
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.FileName = FFmpegStartupPath;
            process.StartInfo.Arguments = cmd;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();

            new Thread(new ThreadStart(() => {

                StreamReader sr = process.StandardError;
                Output("Start.....");
                var name = "";
                if (inputFile.Contains("\\"))
                    name = Path.GetFileName(inputFile);
                else name = inputFile;


                progress = new FFmpegProgress();
                bool d = false;
                TimeSpan totalTimeSpan = TimeSpan.FromMilliseconds(0);
                Output("Total:" + totalTimeSpan.TotalMilliseconds);
                //frame=   18 fps=0.0 q=0.0 size=       7kB time=00:00:00.88 bitrate=  68.8kbits/s speed=1.69x   
                while (!sr.EndOfStream)
                {
                    var v = (sr.ReadLine());
                    Output(v);
                    if (v.ToLower().Contains("duration") && !d)
                    {
                        try
                        {
                            FFmpegInfo fFmpegInfo = new FFmpegInfo(v, name);
                            totalTimeSpan = fFmpegInfo.Duration;
                            progress.InputFileInfo = fFmpegInfo;
                            d = true;
                        }
                        catch { }
                    }
                    if (v.Contains("time=") && totalTimeSpan.TotalMilliseconds != 0)
                    {
                        try
                        {
                            SendProgress(totalTimeSpan, v);
                        }
                        catch { }
                    }
                }
                try
                {
                    SendProgress(totalTimeSpan, ".:::END:::.");
                    // 100%
                }
                catch { }
                Output("End.....");
            })).Start();

        }
        /// <summary>
        ///     Extract image from video (thumbnail/cover photo)
        /// </summary>
        /// <param name="inputFile">Input video file path</param>
        /// <param name="outputFile">Output image file path</param>
        /// <param name="time">Extract image from specific time span</param>
        public void ExtractImageFromVideo(string inputFile, string outputFile, TimeSpan time)
        {
            if (string.IsNullOrEmpty(inputFile))
                throw new Exception("Inputfile is empty. Choose an input file.");
            if (string.IsNullOrEmpty(outputFile))
                throw new Exception("Outputfile is empty. Choose an output file.");

            if (time == null)
                time = TimeSpan.FromSeconds(6);
            //ffmpeg -i input_file.mp4 -ss 01:23:45 -vframes 1 output.jpg
            //-i input file           the path to the input file
            //-ss 01:23:45            seek the position to the specified timestamp
            //-vframes 1              only handle one video frame
            //output.jpg              output filename, should have a well-known extension
            var cmd = $"-i \"{inputFile}\" -ss {time.EncodeTime()} -vframes 1 \"{outputFile}\"";
            

            Output(cmd);


            Process process = new Process();
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.FileName = FFmpegStartupPath;
            process.StartInfo.Arguments = cmd;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();
            new Thread(new ThreadStart(() => {

                StreamReader sr = process.StandardError;
                Output("Start.....");
                while (!sr.EndOfStream)
                {
                    var v = (sr.ReadLine());
                    Output(v);
                }
                Output("End.....");

            })).Start();
        }

        /// <summary>
        ///     Resize video
        /// </summary>
        /// <param name="inputFile">Input video file path</param>
        /// <param name="outputFile">Output video file path</param>
        /// <param name="newSize">New size</param>
        public void ResizeVideo(string inputFile, string outputFile, VideoSize newSize)
        {
            if (string.IsNullOrEmpty(inputFile))
                throw new Exception("Inputfile is empty. Choose an input file.");
            if (string.IsNullOrEmpty(outputFile))
                throw new Exception("Outputfile is empty. Choose an output file.");

            var cmd = string.Empty;
            //-i input.mp4 -s 480x320 -c:a copy output.mp4
            var wh = string.Format("{0}x{1}", newSize.Width, newSize.Height);

            cmd = $"-i \"{inputFile}\" -s {wh} -c:a copy \"{outputFile}\"";

            Process process = new Process();
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.FileName = FFmpegStartupPath;
            process.StartInfo.Arguments = cmd;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();
            new Thread(new ThreadStart(() => {

                StreamReader sr = process.StandardError;
                Output("Start.....");
                var name = "";
                if (inputFile.Contains("\\"))
                    name = Path.GetFileName(inputFile);
                else name = inputFile;


                progress = new FFmpegProgress();
                bool d = false;
                TimeSpan totalTimeSpan = TimeSpan.FromMilliseconds(0);
                Output("Total:" + totalTimeSpan.TotalMilliseconds);
                //frame=   18 fps=0.0 q=0.0 size=       7kB time=00:00:00.88 bitrate=  68.8kbits/s speed=1.69x   
                while (!sr.EndOfStream)
                {
                    var v = (sr.ReadLine());
                    Output(v);
                    if (v.ToLower().Contains("duration") && !d)
                    {
                        try
                        {
                            FFmpegInfo fFmpegInfo = new FFmpegInfo(v, name);
                            totalTimeSpan = fFmpegInfo.Duration;
                            progress.InputFileInfo = fFmpegInfo;
                            d = true;
                        }
                        catch { }
                    }
                    if (v.Contains("time=") && totalTimeSpan.TotalMilliseconds != 0)
                    {
                        try
                        {
                            SendProgress(totalTimeSpan, v);
                        }
                        catch { }
                    }
                }
                try
                {
                    SendProgress(totalTimeSpan, ".:::END:::.");
                    // 100%
                }
                catch { }
                Output("End.....");
            })).Start();
        }

        /// <summary>
        ///     Crop video or audio
        /// </summary>
        /// <param name="inputFile">Input video/audio file path</param>
        /// <param name="outputFile">Output video/audio file path</param>
        /// <param name="startTime">Start time</param>
        /// <param name="duration">Duration</param>
        public void CropVideoOrAudio(string inputFile, string outputFile, TimeSpan startTime, TimeSpan duration)
        {
            if (string.IsNullOrEmpty(inputFile))
                throw new Exception("Inputfile is empty. Choose an input file.");
            if (string.IsNullOrEmpty(outputFile))
                throw new Exception("Outputfile is empty. Choose an output file.");


            //ffmpeg -i source-file.foo -ss 0 -t 600 first-10-min.m4v
            //ffmpeg -i input.mp4 -ss 00:00:50.0 -codec copy -t 20 output.mp4

            var cmd = $"-i \"{inputFile}\" -ss {startTime.EncodeTime()} -codec copy -t {duration.TotalSeconds} \"{outputFile}\"";


            Process process = new Process();
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.FileName = FFmpegStartupPath;
            process.StartInfo.Arguments = cmd;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();
            new Thread(new ThreadStart(() => {

                StreamReader sr = process.StandardError;
                Output("Start.....");
                var name = "";
                if (inputFile.Contains("\\"))
                    name = Path.GetFileName(inputFile);
                else name = inputFile;


                progress = new FFmpegProgress();
                bool d = false;
                TimeSpan totalTimeSpan = TimeSpan.FromMilliseconds(0);
                //frame=   18 fps=0.0 q=0.0 size=       7kB time=00:00:00.88 bitrate=  68.8kbits/s speed=1.69x   
                while (!sr.EndOfStream)
                {
                    var v = (sr.ReadLine());
                    Output(v);
                    if (v.ToLower().Contains("duration") && !d)
                    {
                        try
                        {
                            FFmpegInfo fFmpegInfo = new FFmpegInfo(v, name);
                            totalTimeSpan = fFmpegInfo.Duration;
                            progress.InputFileInfo = fFmpegInfo;
                            d = true;
                        }
                        catch { }
                    }
                    if (v.Contains("time=") && totalTimeSpan.TotalMilliseconds != 0)
                    {
                        try
                        {
                            SendProgress(totalTimeSpan, v);
                        }
                        catch { }
                    }
                }
                try
                {
                    SendProgress(totalTimeSpan, ".:::END:::.");
                    // 100%
                }
                catch { }
                Output("End.....");
            })).Start();
        }

        string RunCommandAndGetResponse(string command)
        {
            Process process = new Process();
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.FileName = FFmpegStartupPath;
            process.StartInfo.Arguments = command;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();
            StreamReader sr = process.StandardError;
            var content = sr.ReadToEnd();

            return content;
        }
        void SendProgress(TimeSpan totalTime, string content)
        {
            //frame=   18 fps=0.0 q=0.0 size=       7kB time=00:00:00.88 bitrate=  68.8kbits/s speed=1.69x       
            string[] split = content.Split(' ');
            if (!content.Contains(".:::END:::."))
            {
                foreach (var row in split)
                {
                    if (row.Contains("time=") && totalTime.TotalMilliseconds != 0)
                    {
                        var time = row.Split('=');
                        var currentTime = TimeSpan.Parse(time[1]);

                        var percent = (int)/*Math.Round*/((currentTime.TotalMilliseconds / totalTime.TotalMilliseconds) * 100);
                        Output(percent + "%     " + time[1]);
                        progress.Percent = percent;
                        progress.CurrentTime = currentTime;
                    }
                    if (row.Contains("speed=") && totalTime.TotalMilliseconds != 0)
                    {
                        //frame=   18 fps=0.0 q=0.0 size=       7kB time=00:00:00.88 bitrate=  68.8kbits/s speed=1.69x
                        var speed = row.Split('=');
                        progress.CurrentSpeed = speed[1];
                    }
                }
                //frame=   18 fps=0.0 q=0.0 size=       7kB time=00:00:00.88 bitrate=  68.8kbits/s speed=1.69x
                if (content.Contains("frame=") && content.Contains("fps"))
                {
                    var frame = content.Substring(content.IndexOf("frame=") + "frame=".Length);
                    frame = frame.Substring(0, frame.IndexOf("fps"));
                    frame = frame.Trim().TrimEnd().TrimStart();
                    progress.CurrentFrame = int.Parse(frame);
                }
                if (content.Contains("fps=") && content.Contains("q"))
                {
                    var frameRate = content.Substring(content.IndexOf("fps=") + "fps=".Length);
                    frameRate = frameRate.Substring(0, frameRate.IndexOf("q"));
                    frameRate = frameRate.Trim().TrimEnd().TrimStart();
                    progress.CurrentFrameRate = double.Parse(frameRate);
                }
                if (content.Contains("size=") && content.Contains("time"))
                {
                    var size = content.Substring(content.IndexOf("size=") + "size=".Length);
                    size = size.Substring(0, size.IndexOf("time"));
                    size = size.Trim().TrimEnd().TrimStart();
                    progress.CurrentFileSize = size;
                }
                if (content.Contains("bitrate=") && content.Contains("speed"))
                {
                    var bitrate = content.Substring(content.IndexOf("bitrate=") + "bitrate=".Length);
                    bitrate = bitrate.Substring(0, bitrate.IndexOf("speed"));
                    bitrate = bitrate.Trim().TrimEnd().TrimStart();
                    progress.CurrentBitRate = bitrate;
                }
            }
            else
                progress.Percent = 100;

            OnProgressChanged?.Invoke(this, progress);
        }


        void Output(object obj)
        {
            if (obj == null)
                return;;
            if (!IsDebugMode)
                return;
            Debug.WriteLine(obj);
        }

        public void Dispose()
        {
            try
            {
                Process[] workers = Process.GetProcessesByName("ffmpeg");
                foreach (Process worker in workers)
                {
                    try
                    {
                        worker.Kill();
                        worker.WaitForExit();
                        worker.Dispose();
                    }
                    catch { }
                }
            }
            catch { }
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                FFmpegStartupPath = null;
            }
        }
    }
}

#endif
