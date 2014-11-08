using System;
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
        private static async Task<Channel> GetChannel(string broadcaster)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://api.twitch.tv/kraken/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/vnd.twitchtv.v2+json"));

                HttpResponseMessage response = await client.GetAsync("channels/" + broadcaster);
                if (response.IsSuccessStatusCode)
                {
                    Channel channel = await response.Content.ReadAsAsync<Channel>();
                    return channel;
                }
            }

            return null;
        }

        private static async Task<Stream> GetStream(string broadcaster)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://api.twitch.tv/kraken/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/vnd.twitchtv.v2+json"));

                HttpResponseMessage response = await client.GetAsync("streams/" + broadcaster);
                if (response.IsSuccessStatusCode)
                {
                    Stream stream = await response.Content.ReadAsAsync<Stream>();
                    return stream;
                }
            }

            return null;
        }

        private static void LogEvent(object sender, WarehouseEvents.ThingEventArgs args)
        {
            Logger.Debug(args.Thing.ID + " - " + (args.Thing.Boolean("live") ? "Online" : "Offline"));
        }

        private static void Main()
        {
            BuildANewThingType.ThingTypePropertyBuilder typeStream = BuildANewThingType.Named("Stream")
                .WhichIs("A Twitch.TV Stream")
                .ContainingA.Boolean("live");

            var warehouse = new Warehouse();
            warehouse.Events.OnNew += LogEvent;
            warehouse.Events.OnUpdate += LogEvent;

            var client = new Client("TwitchModel", "ws://localhost:8083/", warehouse);

            var checkTimer = new Timer(delegate
            {
                Task<Stream> task = GetStream("fmaunier");
                task.Wait();

                BuildANewThing.ThingPropertyBuilder stream = BuildANewThing.As(typeStream)
                    .IdentifiedBy("fmaunier")
                    .ContainingA.Boolean("live", task.Result.stream != null);

                warehouse.RegisterThing(stream);

                client.Send();
            }, null, 0, 10000);

            Console.CancelKeyPress += delegate
            {
                checkTimer.Dispose();
                client.Close();
            };

            Thread.Sleep(Timeout.Infinite);
        }
    }
}