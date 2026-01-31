using Application.Ports.Driven;
using Application.Ports.Driving;
using Models.Request;
using Models.Response;
using Models.Utils;

namespace Domain.UseCases
{
    public class ProductoUseCase(ISqlPort _sqlPort) : IProductoService
    {
        public async Task<(int id, string mensaje)> CrearProducto(ProductoRequest producto)
        {
            try
            {
                string validarionErrores = ValidarProductoRequest(producto);

                if (validarionErrores.Length > 0)
                {
                    return (0, validarionErrores);
                }

                var existeNombre = await ValidarProductoExistente(producto.Nombre);

                if (existeNombre.Length > 0)
                {
                    return (0, existeNombre);
                }

                var registrarProducto = await _sqlPort.ExecuteNonQueryAsync("SP_Producto_Crear", producto);

                if (registrarProducto > 0)
                {
                    return (registrarProducto, "Producto creado exitosamente.");
                }
                else
                {
                    return (0, "No se pudo crear el producto.");
                }
            }
            catch (Exception ex)
            {

                return (0, "Ocurrió un error inesperado al procesar la solicitud.");
            }

        }

        public async Task<(int id, string mensaje)> EditarProducto(ProductoRequest producto)
        {
            try
            {
                string validarionErrores = ValidarProductoRequest(producto, true);

                if (validarionErrores.Length > 0)
                {
                    return (0, validarionErrores);
                }

                var existeNombre = await ValidarProductoExistente(producto.Nombre, producto.Id);
                var existeId = await ValidarIdExistente(producto.Id);

                if (existeNombre.Length > 0 || existeId.Length > 0)
                {
                    return (0, string.Join(", ", new[] { existeNombre, existeId }.Where(e => e.Length > 0)));
                }

                var editar = await _sqlPort.ExecuteNonQueryAsync("SP_Producto_Actualizar", producto);

                if (editar > 0)
                {
                    return (editar, "Producto actualizado exitosamente.");
                }
                else
                {
                    return (0, "No se pudo actualizar el producto.");
                }
            }
            catch (Exception ex)
            {

                return (0, "Ocurrió un error inesperado al procesar la solicitud.");
            }

        }

        public async Task<(int id, string mensaje)> EliminarProducto(int productoID)
        {
            try
            {
                var existeId = await ValidarIdExistente(productoID);

                if (existeId.Length > 0)
                {
                    return (0, existeId);
                }

                var eliminar = await _sqlPort.ExecuteNonQueryAsync("SP_Producto_Eliminar", new { ProductoID = productoID, EliminacionFisica = false });
                if (eliminar > 0)
                {
                    return (eliminar, "Producto eliminado exitosamente.");
                }
                else
                {
                    return (0, "No se pudo eliminar el producto.");
                }
            }
            catch (Exception)
            {

                return (0, "Ocurrió un error inesperado al procesar la solicitud.");
            }
        }

        public async Task<(List<ProductoResponse> productos, int totalRegistros)> Obtener(int numeroPagina, int tamanoPagina, string filtro, bool soloActivos)
        {
            try
            {
                var resultTypes = new Type[] { typeof(ProductoResponse), typeof(TotalRegistros) };

                var results = await _sqlPort.ExecuteStoredProcedureMultipleAsync(
                    "SP_Producto_Listar",
                    resultTypes,
                     new
                     {
                         NumeroPagina = numeroPagina,
                         TamanoPagina = tamanoPagina,
                         Filtro = filtro,
                         SoloActivos = soloActivos
                     });
                var productos = results[0].Cast<ProductoResponse>().ToList();
                var totalRegistros = (results[1].FirstOrDefault() as TotalRegistros)?.Total ?? 0;
                return (productos, totalRegistros);

            }
            catch (Exception)
            {

                throw;
            }
        }

        public static string ValidarProductoRequest(ProductoRequest productoRequest, bool editar = false)
        {
            var errores = new List<string>();
            
            if (editar && productoRequest.Id <= 0)
            {
                errores.Add("El campo Id es requerido para la edición");
            }
            
            if (productoRequest == null)
            {
                return "El objeto ProductoRequest no puede ser nulo";
            }
            
            if (string.IsNullOrWhiteSpace(productoRequest.Nombre))
            {
                errores.Add("El campo Nombre es requerido");
            }
            
            if (productoRequest.PrecioUnitario < 0)
            {
                errores.Add("El campo PrecioUnitario debe ser mayor o igual a 0");
            }
            
            if (productoRequest.StockActual < 0)
            {
                errores.Add("El campo StockActual debe ser mayor o igual a 0");
            }
            
            return string.Join(", ", errores);
        }

        private async Task<string> ValidarProductoExistente(string Nombre, int? ProductoIDExcluir = null)
        {
            var parameter = new { Nombre, ProductoIDExcluir };

            var existe = await _sqlPort.ExecuteStoredProcedureBoolAsync("SP_Producto_ExistePorNombre", parameter);
            return existe ? "El producto ya existe" : string.Empty;

        }

        private async Task<string> ValidarIdExistente(int ProductoID)
        {
            var parameter = new { ProductoID };

            var existe = await _sqlPort.ExecuteStoredProcedureBoolAsync("SP_Producto_ExistePorID", parameter);
            return !existe ? "El id del producto es incorrecto" : string.Empty;
        }
    }

}
