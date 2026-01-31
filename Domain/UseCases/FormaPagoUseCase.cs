using Application.Ports.Driven;
using Application.Ports.Driving;
using Models.Response;

namespace Domain.UseCases
{
    public class FormaPagoUseCase(ISqlPort _sqlPort) : IFormaPagoService
    {
        public async Task<IEnumerable<FormaPagoResponse>> ObtenerActivos()
        {
            try
            {
                return await _sqlPort.ExecuteStoredProcedureAsync<FormaPagoResponse>("SP_FormaPago_ListarActivos", null);
            }
            catch (Exception)
            {
                return Enumerable.Empty<FormaPagoResponse>();
            }
        }
    }
}
