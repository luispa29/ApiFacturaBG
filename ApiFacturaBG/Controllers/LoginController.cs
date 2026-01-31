using Application.Ports.Driving;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Request;
using Models.Response;

namespace ApiFacturaBG.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController(ILoginService _loginSrvc) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Login([FromQuery] string userName, [FromQuery] string contrasena)
        {
            try
            {
                //var userId = this.GetUserId();
                var (id, mensaje,login) = await _loginSrvc.Login(userName,contrasena);

                if (id == 0)
                {
                    return BadRequest(RespuestaApi<object>.Error(mensaje));
                }
                return Ok(RespuestaApi<object>.Exitosa(login, mensaje));
            }
            catch (Exception ex)
            {
                return StatusCode(500, RespuestaApi<object>.Error($"Error al crear usuario: {ex.Message}"));
            }
        }
    }
}
