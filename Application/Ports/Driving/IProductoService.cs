using Models.Request;
using Models.Response;

namespace Application.Ports.Driving
{
    public interface IProductoService
    {
        Task<(int id, string mensaje)> CrearProducto(ProductoRequest productoRequest);
    }
}
