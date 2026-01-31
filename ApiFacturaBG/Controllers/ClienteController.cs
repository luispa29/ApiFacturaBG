using Application.Ports.Driving;
using Microsoft.AspNetCore.Mvc;
using Models.Request;
using Models.Response;

namespace ApiFacturaBG.Controllers
{
    //[JwtAuthorization]
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController(IClienteService _clienteSrvc) : ControllerBase
    {

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ClienteRequest cliente)
        {
            try
            {
                var (id, mensaje) = await _clienteSrvc.CrearCliente(cliente);

                if (id == 0)
                {
                    return BadRequest(RespuestaApi<object>.Error(mensaje));
                }
                return Ok(RespuestaApi<object>.Exitosa(new { Id = id }, mensaje));
            }
            catch (Exception ex)
            {
                return StatusCode(500, RespuestaApi<object>.Error($"Error al crear cliente: {ex.Message}"));
            }
        }

        [HttpPut]
        public async Task<IActionResult> Edit([FromBody] ClienteRequest cliente)
        {
            try
            {
                var (id, mensaje) = await _clienteSrvc.EditarCliente(cliente);

                if (id == 0)
                {
                    return BadRequest(RespuestaApi<object>.Error(mensaje));
                }
                return Ok(RespuestaApi<object>.Exitosa(new { Id = id }, mensaje));
            }
            catch (Exception ex)
            {
                return StatusCode(500, RespuestaApi<object>.Error($"Error al editar cliente: {ex.Message}"));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var (eliminado, mensaje) = await _clienteSrvc.EliminarCliente(id);

                if (eliminado == 0)
                {
                    return BadRequest(RespuestaApi<object>.Error(mensaje));
                }
                return Ok(RespuestaApi<object>.Exitosa(new { Id = eliminado }, mensaje));
            }
            catch (Exception ex)
            {
                return StatusCode(500, RespuestaApi<object>.Error($"Error al eliminar cliente: {ex.Message}"));
            }
        }
    }
}
