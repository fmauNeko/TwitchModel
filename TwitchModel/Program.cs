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
    class Program
    {
        private static bool _keepRunning = true;

        static async Task<Channel> GetChannel(string broadcaster)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://api.twitch.tv/kraken/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.twitchtv.v2+json"));

                HttpResponseMessage response = await client.GetAsync("channels/" + broadcaster);
                if (response.IsSuccessStatusCode)
                {
                    var channel = await response.Content.ReadAsAsync<Channel>();
                    return channel;
                }
            }

            return null;
        }

        static async Task<Stream> GetStream(string broadcaster)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://api.twitch.tv/kraken/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.twitchtv.v2+json"));

                HttpResponseMessage response = await client.GetAsync("streams/" + broadcaster);
                if (response.IsSuccessStatusCode)
                {
                    var stream = await response.Content.ReadAsAsync<Stream>();
                    return stream;
                }
            }

            return null;
        }

        static void Main()
        {
            var typeStream = BuildANewThingType.Named("Stream")
                .WhichIs("A Twitch.TV Stream")
                .ContainingA.Boolean("live");

            var warehouse = new Warehouse();
            warehouse.Events.OnNew += (sender, args) => Console.WriteLine(string.Format("[{0:dd/MM/yyyy HH:mm:ss}] ", DateTime.Now) + args.Thing.ID + " - " + args.Thing.Boolean("live"));
            warehouse.Events.OnUpdate += (sender, args) => Console.WriteLine(string.Format("[{0:dd/MM/yyyy HH:mm:ss}] ", DateTime.Now) + args.Thing.ID + " - " + args.Thing.Boolean("live"));

            var client = new Client("TwitchModel", "ws://localhost:8083/", warehouse);

            var checkTimer = new Timer(delegate
            {
                var task = GetStream("fmaunier");
                task.Wait();

                var stream = BuildANewThing.As(typeStream)
                .IdentifiedBy("fmaunier")
                .ContainingA.Boolean("live", task.Result.stream != null);

                warehouse.RegisterThing(stream);

                client.Send();
            }, null, 0, 10000);

            Console.CancelKeyPress += delegate(object sender, ConsoleCancelEventArgs eventArgs)
            {
                eventArgs.Cancel = true;
                _keepRunning = false;
            };

            while (_keepRunning) {  }

            checkTimer.Dispose();
            client.Close();
        }
    }
}
