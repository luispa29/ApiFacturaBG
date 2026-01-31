using Models.Request;
using Models.Response;

namespace Application.Ports.Driving
{
    public interface IFacturaService
    {
        Task<(int id, string mensaje)> CrearFactura(FacturaRequest facturaRequest);
        Task<FacturaResponse?> ObtenerPorID(int facturaID);
    }
}
