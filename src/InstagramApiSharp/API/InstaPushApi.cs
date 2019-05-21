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
using System.Text;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.IO;
using System.Diagnostics;
using InstagramApiSharp;
using InstagramApiSharp.Helpers;
using InstagramApiSharp.Enums;
using InstagramApiSharp.Logger;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Classes.ResponseWrappers;
//using MQTTnet.Client;
//using MQTTnet.Client.Connecting;
//using MQTTnet.Client.Disconnecting;
//using MQTTnet.Client.Options;
//using MQTTnet.Diagnostics;
//using MQTTnet.Exceptions;
//using MQTTnet.Formatter;
//using MQTTnet.Implementations;
//using MQTTnet.Protocol;
//using MQTTnet.Server;
//using MQTTnet.Server.Status;
//using System.Collections.Concurrent;
//using System.Collections.ObjectModel;
//using MQTTnet;
using System.Threading.Tasks;

namespace InstagramApiSharp.API
{
    public class InstaPushApi
    {
        //const string HOST = "mqtt-mini.facebook.com";
        //const int PORT = 443;

        //private readonly ConcurrentQueue<MqttNetLogMessage> _traceMessages = new ConcurrentQueue<MqttNetLogMessage>();
        //private readonly ObservableCollection<IMqttClientStatus> _sessions = new ObservableCollection<IMqttClientStatus>();

        //private IMqttClient _mqttClient;
        ////private IManagedMqttClient _managedMqttClient;



        //public event InstaPushHandler OnPushed;
        //public IInstaApi InstaApi { get; internal set; }
        //public InstaPushApi(IInstaApi instaApi)
        //{
        //    InstaApi = instaApi;
        //}

        //public async void Connect()
        //{

        //    var factory = new MqttFactory();
        //    _mqttClient = factory.CreateMqttClient();

        //    try
        //    {
        //        await _mqttClient.ConnectAsync(new MqttClientOptionsBuilder().WithTcpServer(HOST, PORT)
        //            .WithClientId(InstaApi.GetCurrentDevice().PushDeviceGuid.ToString()).Build());

        //        _mqttClient.UseApplicationMessageReceivedHandler(MessageReceivedHandler);
        //        //_mqttClient.SubscribeAsync()

        //        "Connected.........".PrintInDebug();
        //    }
        //    catch (Exception ex)
        //    {
        //        "NOT Connect()".PrintInDebug();
        //        ex.Message.PrintInDebug();
        //        ex.StackTrace.PrintInDebug();
        //        ex.Source.PrintInDebug();
        //    }

        //}
        //async Task MessageReceivedHandler(MqttApplicationMessageReceivedEventArgs eventArgs)
        //{
        //    //await _mqttClient.PublishAsync($"reply/{eventArgs.ApplicationMessage.Topic}");
        //    await Task.Delay(1);


        //    "MessageReceivedHandler".PrintInDebug();

        //    eventArgs.ClientId.PrintInDebug();
        //    eventArgs.ProcessingFailed.PrintInDebug();

        //    "APP MSG >|".PrintInDebug();

        //    eventArgs.ApplicationMessage?.ContentType.PrintInDebug();
        //    eventArgs.ApplicationMessage?.Topic.PrintInDebug();
        //    if (eventArgs.ApplicationMessage != null)
        //    {
        //        if (eventArgs.ApplicationMessage.Payload != null)
        //        {
        //            $"MSG: {Encoding.UTF8.GetString(eventArgs.ApplicationMessage.Payload)}".PrintInDebug();
        //        }
        //    }


        //    "".PrintInDebug();
        //    "".PrintInDebug();
        //    "".PrintInDebug();
        //    "".PrintInDebug();

        //}
        //void SendPushEvent()
        //{
        //    OnPushed?.Invoke(InstaApi);
        //}
    }
    public delegate void InstaPushHandler(IInstaApi sender /*, PushType? */);
    public class InstaPushEventsArgs : EventArgs
    {

    }
}
