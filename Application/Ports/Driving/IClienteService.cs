using Models.Request;
using Models.Response;

namespace Application.Ports.Driving
{
    public interface IClienteService
    {
        Task<(int id, string mensaje)> CrearCliente(ClienteRequest clienteRequest);
    }
}
