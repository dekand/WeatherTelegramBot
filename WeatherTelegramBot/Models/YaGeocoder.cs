namespace WeatherTelegramBot.Models
{
    internal class YaGeocoder
    {
        public GResponse Response { get; set; }
    }

    class GResponse
    {
        public GeoObjectCollection GeoObjectCollection { get; set; }
    }

    class YametaDataProperty
    {
        public GeocoderResponseMetaData GeocoderResponseMetaData { get; set; }
    }
    
    class GeocoderResponseMetaData
    {
        public int Found { get; set; }
    }

    class GeoObjectCollection
    {
        public GeoObjectWrapper[] FeatureMember { get; set; }
        public YametaDataProperty metaDataProperty { get; set; }
    }

    class GeoObjectWrapper
    {
        public GeoObject GeoObject { get; set; }
    }

    class GeoObject
    {
        public string Name { get; set; }
        public Point Point { get; set; }
    }

    class Point
    {
        public string pos { get; set; }
    }
}
