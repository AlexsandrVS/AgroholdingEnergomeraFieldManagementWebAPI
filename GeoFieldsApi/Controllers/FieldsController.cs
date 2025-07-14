using Microsoft.AspNetCore.Mvc;
using GeoFieldsApi.Models;
using GeoFieldsApi.Services;
using GeoFieldsApi.Services.GeoFields;

namespace GeoFieldsApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FieldsController : ControllerBase
    {
        private readonly IKmlService _kmlService;
        private readonly IGeoCalculationService _geoService;

        public FieldsController(
            IKmlService kmlService,
            IGeoCalculationService geoService,
            ILogger<FieldsController> logger)
        {
            _kmlService = kmlService;
            _geoService = geoService;
        }

        /// <summary>
        /// 1. Получение всех элементов fields
        /// </summary>
        [HttpGet]
        public ActionResult<List<Field>> GetAllFields()
        {
            try
            {
                var fields = _kmlService.GetAllFields();
                return Ok(fields);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// 2. Получение площади поля по идентификатору
        /// </summary>
        [HttpGet("{id}/size")]
        public ActionResult<double> GetFieldSize(string id)
        {
            try
            {
                var field = _kmlService.GetFieldById(id);
                if (field == null)
                {
                    return NotFound($"Field with id '{id}' not found");
                }

                return Ok(field.Size);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// 3. Получение расстояния от центра поля до точки
        /// </summary>
        [HttpPost("{id}/distance")]
        public ActionResult<double> GetDistanceToPoint(string id, [FromBody] PointRequest request)
        {
            try
            {
                var field = _kmlService.GetFieldById(id);
                if (field == null)
                {
                    return NotFound($"Field with id '{id}' not found");
                }

                var distance = _geoService.CalculateDistance(
                    field.Locations.Center[0], field.Locations.Center[1],
                    request.Latitude, request.Longitude);

                return Ok(distance);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// 4. Получение принадлежности точки к полям
        /// </summary>
        [HttpPost("point-in-field")]
        public ActionResult<object> CheckPointInField([FromBody] PointRequest request)
        {
            try
            {
                var fields = _kmlService.GetAllFields();

                foreach (var field in fields)
                {
                    if (_geoService.IsPointInPolygon(request.Latitude, request.Longitude, field.Locations.Polygon))
                    {
                        return Ok(new FieldInfo
                        {
                            Id = field.Id,
                            Name = field.Name
                        });
                    }
                }

                return Ok(false);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }
    }
}