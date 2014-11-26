// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Dissidence">
//   Copyright (c) 2014 Florian Maunier
// </copyright>
// <summary>
//   The program.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TwitchModel
{
    using System;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading;
    using System.Threading.Tasks;

    using ThingModel;
    using ThingModel.Builders;
    using ThingModel.WebSockets;

    using TwitchModel.Models;

    /// <summary>
    /// The program.
    /// </summary>
    internal class Program
    {
        #region Static Fields

        /// <summary>
        /// The warehouse.
        /// </summary>
        private static readonly Warehouse Warehouse = new Warehouse();

        /// <summary>
        /// The client.
        /// </summary>
        private static readonly Client Client = new Client("TwitchModel", "ws://localhost:8083/", Warehouse);

        /// <summary>
        /// The configuration.
        /// </summary>
        private static readonly Configuration Configuration = new Configuration();

        /// <summary>
        /// The type stream.
        /// </summary>
        private static readonly ThingType TypeStream =
            BuildANewThingType.Named("Stream")
                .WhichIs("A Twitch.TV Stream")
                .ContainingA.String("avatar")
                .AndA.String("broadcaster")
                .AndA.String("displayName")
                .AndAn.Int("followers")
                .AndA.String("game")
                .AndA.Boolean("live")
                .AndA.String("status")
                .AndAn.Int("viewers")
                .AndAn.Int("views")
                .AndA.Boolean("alwaysontop");

        #endregion

        #region Methods

        /// <summary>
        /// The get channel.
        /// </summary>
        /// <param name="broadcaster">
        /// The broadcaster.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
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

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var channel = await response.Content.ReadAsAsync<Channel>();
                return channel;
            }
        }

        /// <summary>
        /// The get stream.
        /// </summary>
        /// <param name="broadcaster">
        /// The broadcaster.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
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

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var stream = await response.Content.ReadAsAsync<Stream>();
                return stream;
            }
        }

        /// <summary>
        /// The log event.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        private static void LogEvent(object sender, WarehouseEvents.ThingEventArgs args)
        {
            Logger.Debug(args.Thing.ID + " - " + (args.Thing.Boolean("live") ? "Online" : "Offline"));
        }

        /// <summary>
        /// The main.
        /// </summary>
        private static void Main()
        {
            Warehouse.Events.OnNew += LogEvent;
            Warehouse.Events.OnUpdate += LogEvent;

            var timer = new Timer(
                delegate
                    {
                        var tasks = Configuration.Broadcasters.Select(UpdateApi).ToList();
                        Task.WaitAll(tasks.ToArray(), 10000);
                    }, 
                null, 
                0, 
                10000);

            Console.CancelKeyPress += delegate
                {
                    timer.Dispose();
                    Client.Close();
                };

            (new ManualResetEvent(false)).WaitOne();
        }

        /// <summary>
        /// The update API.
        /// </summary>
        /// <param name="broadcaster">
        /// The broadcaster.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private static async Task UpdateApi(string broadcaster)
        {
            var streamObject = await GetStream(broadcaster);
            if (streamObject == null)
            {
                return;
            }

            var live = streamObject.stream != null;

            var channelObject = await GetChannel(broadcaster);
            if (channelObject == null)
            {
                return;
            }

            if (!live)
            {
                streamObject.stream = new StreamDetails { viewers = 0 };
            }

            streamObject.stream.channel = channelObject;

            var stream =
                BuildANewThing.As(TypeStream)
                    .IdentifiedBy(broadcaster)
                    .ContainingA.String("avatar", streamObject.stream.channel.logo)
                    .AndA.String("broadcaster", streamObject.stream.channel.name)
                    .AndA.String("displayName", streamObject.stream.channel.display_name)
                    .AndAn.Int("followers", streamObject.stream.channel.followers)
                    .AndA.String("game", streamObject.stream.channel.game)
                    .AndA.Boolean("live", live)
                    .AndA.String("status", streamObject.stream.channel.status)
                    .AndAn.Int("viewers", streamObject.stream.viewers)
                    .AndAn.Int("views", streamObject.stream.channel.views)
                    .AndA.Boolean("alwaysontop", Configuration.AlwaysOnTop.Contains(broadcaster));

            Warehouse.RegisterThing(stream);

            Client.Send();
        }

        #endregion
    }
}