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
    }
}
