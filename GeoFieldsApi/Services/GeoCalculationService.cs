namespace GeoFieldsApi.Services
{
    public class GeoCalculationService : IGeoCalculationService
    {
        private const double EarthRadiusMeters = 6371000;

        public double CalculateDistance(double lat1, double lng1, double lat2, double lng2)
        {
            var dLat = ToRadians(lat2 - lat1);
            var dLng = ToRadians(lng2 - lng1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLng / 2) * Math.Sin(dLng / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return EarthRadiusMeters * c;
        }

        public bool IsPointInPolygon(double pointLat, double pointLng, double[][] polygon)
        {
            if (polygon.Length < 3) return false;

            bool inside = false;
            int j = polygon.Length - 1;

            for (int i = 0; i < polygon.Length; i++)
            {
                double xi = polygon[i][1]; // longitude
                double yi = polygon[i][0]; // latitude
                double xj = polygon[j][1]; // longitude
                double yj = polygon[j][0]; // latitude

                if (((yi > pointLat) != (yj > pointLat)) &&
                    (pointLng < (xj - xi) * (pointLat - yi) / (yj - yi) + xi))
                {
                    inside = !inside;
                }
                j = i;
            }

            return inside;
        }

        private double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }
    }
}
