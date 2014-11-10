using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using ThingModel;
using ThingModel.Builders;
using ThingModel.WebSockets;
using TwitchModel.Models;

namespace TwitchModel
{
    internal class Program
    {
        private static readonly ThingType TypeStream = BuildANewThingType.Named("Stream")
                .WhichIs("A Twitch.TV Stream")
                .ContainingA.String("avatar")
                .AndA.String("broadcaster")
                .AndA.String("displayName")
                .AndAn.Int("followers")
                .AndA.String("game")
                .AndA.Boolean("live")
                .AndA.String("status")
                .AndAn.Int("viewers")
                .AndAn.Int("views");

        private static readonly Warehouse Warehouse = new Warehouse();

        private static readonly Client Client = new Client("TwitchModel", "ws://localhost:8083/", Warehouse);

        private static readonly Configuration Configuration = new Configuration();

        private static async Task<Channel> GetChannel(string broadcaster)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://api.twitch.tv/kraken/");
                client.DefaultRequestHeaders.Add("Client-ID", Configuration.ClientId);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/vnd.twitchtv.v3+json"));

                var response = await client.GetAsync("channels/" + broadcaster);

                if (!response.IsSuccessStatusCode) return null;

                var channel = await response.Content.ReadAsAsync<Channel>();
                return channel;
            }
        }

        private static async Task<Stream> GetStream(string broadcaster)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://api.twitch.tv/kraken/");
                client.DefaultRequestHeaders.Add("Client-ID", Configuration.ClientId);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/vnd.twitchtv.v3+json"));

                var response = await client.GetAsync("streams/" + broadcaster);

                if (!response.IsSuccessStatusCode) return null;

                var stream = await response.Content.ReadAsAsync<Stream>();
                return stream;
            }
        }

        private static void LogEvent(object sender, WarehouseEvents.ThingEventArgs args)
        {
            Logger.Debug(args.Thing.ID + " - " + (args.Thing.Boolean("live") ? "Online" : "Offline"));
        }

        private static void UpdateApi(Object bcaster)
        {
            var broadcaster = (String) bcaster;

            var streamTask = GetStream(broadcaster);
            streamTask.Wait(5000);

            var streamObject = streamTask.Result;
            if (streamObject == null) return;

            var live = (streamObject.stream != null);

            var channelTask = GetChannel(broadcaster);
            channelTask.Wait(5000);

            var channelObject = channelTask.Result;
            if (channelObject == null) return;

            if (!live)
                streamObject.stream = new StreamDetails { viewers = 0 };

            streamObject.stream.channel = channelObject;

            var stream = BuildANewThing.As(TypeStream)
                .IdentifiedBy(broadcaster)
                .ContainingA.String("avatar", streamObject.stream.channel.logo)
                .AndA.String("broadcaster", streamObject.stream.channel.name)
                .AndA.String("displayName", streamObject.stream.channel.display_name)
                .AndAn.Int("followers", streamObject.stream.channel.followers)
                .AndA.String("game", streamObject.stream.channel.game)
                .AndA.Boolean("live", live)
                .AndA.String("status", streamObject.stream.channel.status)
                .AndAn.Int("viewers", streamObject.stream.viewers)
                .AndAn.Int("views", streamObject.stream.channel.views);

            Warehouse.RegisterThing(stream);

            Client.Send();
        }

        private static void Main()
        {
            Warehouse.Events.OnNew += LogEvent;
            Warehouse.Events.OnUpdate += LogEvent;

            var timers = new List<Timer>();

            foreach (var broadcaster in Configuration.Broadcasters)
            {
                timers.Add(new Timer(UpdateApi, broadcaster, 5000, 30000));
                Thread.Sleep(1000);
            }

            Console.CancelKeyPress += delegate
            {
                foreach (var t in timers)
                    t.Dispose();

                Client.Close();
            };

            Thread.Sleep(Timeout.Infinite);
        }
    }
}