using System.Collections.Generic;
using FX.Configuration;

namespace TwitchModel
{
    internal class Configuration : JsonConfiguration
    {
        public List<string> Broadcasters { get; set; }

        public string ClientId { get; set; }
    }
}