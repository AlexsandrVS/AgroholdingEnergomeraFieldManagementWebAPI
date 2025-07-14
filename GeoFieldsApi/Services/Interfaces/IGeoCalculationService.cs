namespace GeoFieldsApi.Services
{
    public interface IGeoCalculationService
    {
        double CalculateDistance(double lat1, double lng1, double lat2, double lng2);
        bool IsPointInPolygon(double pointLat, double pointLng, double[][] polygon);
    }
}