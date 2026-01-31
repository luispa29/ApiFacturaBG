using Models.Request;
using Models.Response;

namespace Application.Ports.Driving
{
    public interface IProductoService
    {
        Task<(int id, string mensaje)> CrearProducto(ProductoRequest productoRequest);
        Task<(int id, string mensaje)> EditarProducto(ProductoRequest productoRequest);
        Task<(int id, string mensaje)> EliminarProducto(int productoID);
    }
}
