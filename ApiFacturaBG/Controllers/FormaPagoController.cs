using ApiCobrApp.Filters;
using ApiFacturaBG.Extensions;
using Application.Ports.Driving;
using Microsoft.AspNetCore.Mvc;
using Models.Response;

namespace ApiFacturaBG.Controllers
{
    [JwtAuthorization]
    [Route("api/[controller]")]
    [ApiController]
    public class FormaPagoController(IFormaPagoService _formaPagoSrvc) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetActive()
        {
            try
            {
                var result = await _formaPagoSrvc.ObtenerActivos();
                return Ok(RespuestaApi<IEnumerable<FormaPagoResponse>>.Exitosa(result, "Medios de pago obtenidos exitosamente"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, RespuestaApi<object>.Error($"Error al obtener medios de pago: {ex.Message}"));
            }
        }
    }
}
