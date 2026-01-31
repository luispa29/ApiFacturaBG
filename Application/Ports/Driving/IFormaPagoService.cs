using Models.Response;

namespace Application.Ports.Driving
{
    public interface IFormaPagoService
    {
        Task<IEnumerable<FormaPagoResponse>> ObtenerActivos();
    }
}
