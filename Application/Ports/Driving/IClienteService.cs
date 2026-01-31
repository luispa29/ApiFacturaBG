using Models.Request;
using Models.Response;

namespace Application.Ports.Driving
{
    public interface IClienteService
    {
        Task<(int id, string mensaje)> CrearCliente(ClienteRequest clienteRequest);
        Task<(int id, string mensaje)> EditarCliente(ClienteRequest clienteRequest);
        Task<(int id, string mensaje)> EliminarCliente(int clienteID);
        Task<(List<ClienteResponse> clientes, int totalRegistros)> Obtener(int numeroPagina, int tamanoPagina, string filtro, bool soloActivos);
    }
}
