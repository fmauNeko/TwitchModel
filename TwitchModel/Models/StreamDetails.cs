using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitchModel.Models
{
    class StreamDetails
    {
        public long _id { get; set; }
        public string game { get; set; }
        public int viewers { get; set; }
        public string created_at { get; set; }
        public object _links { get; set; }
        public object preview { get; set; }
        public Channel channel { get; set; }
    }
}
