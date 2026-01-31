using ApiCobrApp.Filters;
using ApiFacturaBG.Extensions;
using Application.Ports.Driving;
using Microsoft.AspNetCore.Mvc;
using Models.Request;
using Models.Response;

namespace ApiFacturaBG.Controllers
{
   // [JwtAuthorization]
    [Route("api/[controller]")]
    [ApiController]
    public class FacturaController(IFacturaService _facturaSrvc) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] FacturaFiltroRequest filtros)
        {
            try
            {
                var (facturas, total) = await _facturaSrvc.ListarFacturas(filtros);

                return Ok(RespuestaApi<IEnumerable<FacturaResponse>>.ExitosaConPaginacion(
                    facturas, 
                    total,
                    filtros.Pagina, 
                    filtros.RegistrosPorPagina, 
                    "Facturas listadas exitosamente"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, RespuestaApi<object>.Error($"Error al listar facturas: {ex.Message}"));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] FacturaRequest factura)
        {
            try
            {
                var (id, mensaje) = await _facturaSrvc.CrearFactura(factura);

                if (id == 0)
                {
                    return BadRequest(RespuestaApi<object>.Error(mensaje));
                }
                return Ok(RespuestaApi<object>.Exitosa(new { Id = id }, mensaje));
            }
            catch (Exception ex)
            {
                return StatusCode(500, RespuestaApi<object>.Error($"Error al crear factura: {ex.Message}"));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var factura = await _facturaSrvc.ObtenerPorID(id);

                if (factura == null)
                {
                    return NotFound(RespuestaApi<object>.Error($"La factura con ID {id} no existe"));
                }

                return Ok(RespuestaApi<FacturaResponse>.Exitosa(factura, "Factura obtenida exitosamente"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, RespuestaApi<object>.Error($"Error al obtener factura: {ex.Message}"));
            }
        }
    }
}
