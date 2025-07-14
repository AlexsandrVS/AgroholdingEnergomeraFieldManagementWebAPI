using System.Xml.Linq;
using GeoFieldsApi.Models;
using GeoFieldsApi.Services.GeoFields;

namespace GeoFieldsApi.Services
{
    public class KmlService : IKmlService
    {
        private readonly List<Field> _fields;
        private readonly ILogger<KmlService> _logger;

        public KmlService(ILogger<KmlService> logger)
        {
            _fields = new List<Field>();
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            try
            {
                // Load centroids
                var centroids = await LoadCentroidsAsync("Data/centroids.kml");

                // Load fields polygons
                var polygons = await LoadPolygonsAsync("Data/fields.kml");

                // Combine data
                foreach (var centroid in centroids)
                {
                    if (polygons.TryGetValue(centroid.Key, out var polygon))
                    {
                        var field = new Field
                        {
                            Id = centroid.Key,
                            Name = $"Field_{centroid.Key}",
                            Size = CalculatePolygonArea(polygon),
                            Locations = new FieldLocations
                            {
                                Center = centroid.Value,
                                Polygon = polygon
                            }
                        };
                        _fields.Add(field);
                    }
                }

                _logger.LogInformation($"Loaded {_fields.Count} fields");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing KML data");
                throw;
            }
        }

        private async Task<Dictionary<string, double[]>> LoadCentroidsAsync(string filePath)
        {
            var centroids = new Dictionary<string, double[]>();

            if (!File.Exists(filePath))
            {
                _logger.LogWarning($"Centroids file not found: {filePath}");
                return centroids;
            }

            var kmlContent = await File.ReadAllTextAsync(filePath);
            var doc = XDocument.Parse(kmlContent);
            var ns = XNamespace.Get("http://www.opengis.net/kml/2.2");

            var placemarks = doc.Descendants(ns + "Placemark");

            foreach (var placemark in placemarks)
            {
                var name = placemark.Element(ns + "name")?.Value;
                var coordinates = placemark.Descendants(ns + "coordinates").FirstOrDefault()?.Value?.Trim();

                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(coordinates))
                {
                    var coords = coordinates.Split(',');
                    if (coords.Length >= 2)
                    {
                        if (double.TryParse(coords[1], out var lat) && double.TryParse(coords[0], out var lng))
                        {
                            centroids[name] = new double[] { lat, lng };
                        }
                    }
                }
            }

            return centroids;
        }

        private async Task<Dictionary<string, double[][]>> LoadPolygonsAsync(string filePath)
        {
            var polygons = new Dictionary<string, double[][]>();

            if (!File.Exists(filePath))
            {
                _logger.LogWarning($"Fields file not found: {filePath}");
                return polygons;
            }

            var kmlContent = await File.ReadAllTextAsync(filePath);
            var doc = XDocument.Parse(kmlContent);
            var ns = XNamespace.Get("http://www.opengis.net/kml/2.2");

            var placemarks = doc.Descendants(ns + "Placemark");

            foreach (var placemark in placemarks)
            {
                var name = placemark.Element(ns + "name")?.Value;
                var coordinates = placemark.Descendants(ns + "coordinates").FirstOrDefault()?.Value?.Trim();

                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(coordinates))
                {
                    var points = new List<double[]>();
                    var coordPairs = coordinates.Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var coordPair in coordPairs)
                    {
                        var coords = coordPair.Split(',');
                        if (coords.Length >= 2)
                        {
                            if (double.TryParse(coords[1], out var lat) && double.TryParse(coords[0], out var lng))
                            {
                                points.Add(new double[] { lat, lng });
                            }
                        }
                    }

                    if (points.Count > 0)
                    {
                        polygons[name] = points.ToArray();
                    }
                }
            }

            return polygons;
        }

        private double CalculatePolygonArea(double[][] polygon)
        {
            if (polygon.Length < 3) return 0;

            double area = 0;
            int n = polygon.Length;

            for (int i = 0; i < n; i++)
            {
                int j = (i + 1) % n;
                area += polygon[i][0] * polygon[j][1];
                area -= polygon[j][0] * polygon[i][1];
            }

            area = Math.Abs(area) / 2.0;

            // Convert to square meters (approximate)
            // This is a simplified calculation - for production use proper geospatial libraries
            return area * 111320 * 111320; // rough conversion from degrees to meters
        }

        public List<Field> GetAllFields() => _fields.ToList();

        public Field GetFieldById(string id) => _fields.FirstOrDefault(f => f.Id == id);
    }
}