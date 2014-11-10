namespace TwitchModel.Models
{
    internal class StreamDetails
    {
        // ReSharper disable InconsistentNaming

        public long _id { get; set; }
        public string game { get; set; }
        public int viewers { get; set; }
        public string created_at { get; set; }
        public object _links { get; set; }
        public object preview { get; set; }
        public Channel channel { get; set; }

        // ReSharper restore InconsistentNaming
    }
}