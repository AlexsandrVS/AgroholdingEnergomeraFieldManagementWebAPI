using GeoFieldsApi.Models;

namespace GeoFieldsApi.Services.GeoFields
{
    public interface IKmlService
    {
        Task InitializeAsync();
        List<Field> GetAllFields();
        Field GetFieldById(string id);
    }
}