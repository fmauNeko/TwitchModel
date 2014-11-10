using System.Collections.Generic;
using FX.Configuration;

namespace TwitchModel
{
    class Configuration : JsonConfiguration
    {
        public List<string> Broadcasters { get; set; }

        public string ClientId { get; set; }
    }
}
