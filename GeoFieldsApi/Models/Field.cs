namespace GeoFieldsApi.Models
{
    public class Field
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public double Size { get; set; }
        public FieldLocations Locations { get; set; }
    }

    public class FieldLocations
    {
        public double[] Center { get; set; } // [lat, lng]
        public double[][] Polygon { get; set; } // [[lat, lng], ...]
    }

    public class PointRequest
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class DistanceRequest : PointRequest
    {
        public string FieldId { get; set; }
    }

    public class FieldInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}