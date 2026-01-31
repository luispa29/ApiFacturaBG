using ApiCobrApp.Filters;
using ApiFacturaBG.Extensions;
using Application.Ports.Driving;
using Microsoft.AspNetCore.Mvc;
using Models.Request;
using Models.Response;

namespace ApiFacturaBG.Controllers
{
    //[JwtAuthorization]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController(IProductoService _productoSrvc) : ControllerBase
    {

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductoRequest producto)
        {
            try
            {
                var (id, mensaje) = await _productoSrvc.CrearProducto(producto);

                if (id == 0)
                {
                    return BadRequest(RespuestaApi<object>.Error(mensaje));
                }
                return Ok(RespuestaApi<object>.Exitosa(new { Id = id }, mensaje));
            }
            catch (Exception ex)
            {
                return StatusCode(500, RespuestaApi<object>.Error($"Error al crear producto: {ex.Message}"));
            }
        }

        [HttpPut]
        public async Task<IActionResult> Edit([FromBody] ProductoRequest producto)
        {
            try
            {
                var (id, mensaje) = await _productoSrvc.EditarProducto(producto);

                if (id == 0)
                {
                    return BadRequest(RespuestaApi<object>.Error(mensaje));
                }
                return Ok(RespuestaApi<object>.Exitosa(new { Id = id }, mensaje));
            }
            catch (Exception ex)
            {
                return StatusCode(500, RespuestaApi<object>.Error($"Error al editar producto: {ex.Message}"));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var (eliminado, mensaje) = await _productoSrvc.EliminarProducto(id);

                if (eliminado == 0)
                {
                    return BadRequest(RespuestaApi<object>.Error(mensaje));
                }
                return Ok(RespuestaApi<object>.Exitosa(new { Id = eliminado }, mensaje));
            }
            catch (Exception ex)
            {
                return StatusCode(500, RespuestaApi<object>.Error($"Error al eliminar producto: {ex.Message}"));
            }
        }
    }
}
