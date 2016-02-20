// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Dissidence">
//   Copyright (c) 2014 Florian Maunier
// </copyright>
// <summary>
//   The main TwitchModel logic.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TwitchModel
{
    using System;
    using System.Collections.Generic;
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
    ///     The main TwitchModel logic.
    /// </summary>
    internal class Program
    {
        #region Static Fields

        /// <summary>
        ///     The configuration provider.
        /// </summary>
        private static readonly Configuration Configuration = new Configuration();

        /// <summary>
        ///     The ThingModel warehouse.
        /// </summary>
        private static readonly Warehouse Warehouse = new Warehouse();

        /// <summary>
        ///     The ThingModel client.
        /// </summary>
        private static readonly Client Client = new Client("TwitchModel", Configuration.ThingModelURI, Warehouse);

        /// <summary>
        ///     The ThingModel stream type definition.
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
                .AndAn.Int("views");

        private static readonly ThingType TypeBroadcaster =
            BuildANewThingType.Named("Broadcaster")
                .WhichIs("A Twitch.TV Broadcaster")
                .ContainingA.String("name", "Name")
                .WhichIs("The name of the broadcaster");

        private static readonly List<string> Broadcasters = new List<string>();

        #endregion

        #region Methods

        /// <summary>
        ///     This method fetches the channel info from Twitch's API, and deserializes it.
        /// </summary>
        /// <param name="broadcaster">
        ///     The broadcaster's account name.
        /// </param>
        /// <returns>
        ///     An asynchronous <see cref="Task" /> which will return a <see cref="Channel" /> object.
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
        ///     This method fetches the stream info from Twitch's API, and deserializes it.
        /// </summary>
        /// <param name="broadcaster">
        ///     The broadcaster's account name.
        /// </param>
        /// <returns>
        ///     An asynchronous <see cref="Task" /> which will return a <see cref="Stream" /> object.
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
        ///     A ThingModel callback event to process new broadcaster requests.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="args">
        ///     The args.
        /// </param>
        private static void OnNewThing(object sender, WarehouseEvents.ThingEventArgs args)
        {
            if (!args.Thing.Type.Is(TypeBroadcaster) || Broadcasters.Contains(args.Thing.String("broadcaster"))) return;

            Logger.Debug("New Broadcaster: " + args.Thing.String("broadcaster"));
            Broadcasters.Add(args.Thing.String("broadcaster"));
        }

        /// <summary>
        ///     A ThingModel callback event to log stream status changes.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="args">
        ///     The args.
        /// </param>
        private static void OnUpdatedThing(object sender, WarehouseEvents.ThingEventArgs args)
        {
            if (args.Thing.Type.Is(TypeStream)) Logger.Debug(args.Thing.ID + " - " + (args.Thing.Boolean("live") ? "Online" : "Offline"));
        }

        /// <summary>
        ///     A ThingModel callback event to process broadcaster deletion requests.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="args">
        ///     The args.
        /// </param>
        private static void OnDeletedThing(object sender, WarehouseEvents.ThingEventArgs args)
        {
            if (!args.Thing.Type.Is(TypeBroadcaster) || !Broadcasters.Contains(args.Thing.String("broadcaster"))) return;

            Logger.Debug("Broadcaster deleted: " + args.Thing.String("broadcaster"));
            Broadcasters.Remove(args.Thing.String("broadcaster"));
        }

        /// <summary>
        ///     The main method.
        /// </summary>
        private static void Main()
        {
            Warehouse.Events.OnNew += OnNewThing;
            Warehouse.Events.OnUpdate += OnUpdatedThing;
            Warehouse.Events.OnDelete += OnDeletedThing;

            var timer = new Timer(
                delegate
                    {
                        var tasks = Broadcasters.Select(UpdateApi).ToArray();
                        Task.WaitAll(tasks, 10000);
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
        ///     The worker task, which will get the <see cref="Channel" /> and <see cref="Stream" /> objects, and merge them into a
        ///     thing, before sending it to the ThingModel broker.
        /// </summary>
        /// <param name="broadcaster">
        ///     The broadcaster's name.
        /// </param>
        /// <returns>
        ///     An asynchronous <see cref="Task" /> without any return value.
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
                    .AndAn.Int("views", streamObject.stream.channel.views);

            Warehouse.RegisterThing(stream);

            Client.Send();
        }

        #endregion
    }
}