using ApiCobrApp.Filters;
using ApiFacturaBG.Extensions;
using Application.Ports.Driving;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Request;
using Models.Response;

namespace ApiFacturaBG.Controllers
{
   [JwtAuthorization]
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController(IUsuarioService _usuarioSrvc) : ControllerBase
    {

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UsuarioRequest usuario)
        {
            try
            {
                //var userId = this.GetUserId();
                var (id, mensaje) = await _usuarioSrvc.CrearUsuario(usuario);

                if (id == 0)
                {
                    return BadRequest(RespuestaApi<object>.Error(mensaje));
                }
                return Ok(RespuestaApi<object>.Exitosa(new { Id = id }, mensaje));
            }
            catch (Exception ex)
            {
                return StatusCode(500, RespuestaApi<object>.Error($"Error al crear usuario: {ex.Message}"));
            }
        }
        
        [HttpPut]
        public async Task<IActionResult> Edit([FromBody] UsuarioUpdateRequest usuario)
        {
            try
            {
                //var userId = this.GetUserId();
                var (id, mensaje) = await _usuarioSrvc.EditarUsuario(usuario);

                if (id == 0)
                {
                    return BadRequest(RespuestaApi<object>.Error(mensaje));
                }
                return Ok(RespuestaApi<object>.Exitosa(new { Id = id }, mensaje));
            }
            catch (Exception ex)
            {
                return StatusCode(500, RespuestaApi<object>.Error($"Error al editar usuario: {ex.Message}"));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var (eliminado, mensaje) = await _usuarioSrvc.EliminarUsuario(id);

                if (eliminado == 0)
                {
                    return BadRequest(RespuestaApi<object>.Error(mensaje));
                }
                return Ok(RespuestaApi<object>.Exitosa(new { Id = eliminado }, mensaje));
            }
            catch (Exception ex)
            {
                return StatusCode(500, RespuestaApi<object>.Error($"Error al eliminar usuario: {ex.Message}"));
            }
        }
    }
}
