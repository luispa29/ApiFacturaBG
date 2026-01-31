using Application.Ports.Driven;
using Application.Ports.Driving;
using Models.Request;
using Models.Response;
using Models.Utils;

namespace Domain.UseCases
{
    public class ClienteUseCase(ISqlPort _sqlPort) : IClienteService
    {
        public async Task<(int id, string mensaje)> CrearCliente(ClienteRequest cliente)
        {
            try
            {
                string validarionErrores = ValidarClienteRequest(cliente);

                if (validarionErrores.Length > 0)
                {
                    return (0, validarionErrores);
                }

                var existeIdentificacion = await ValidarClienteExistentePorIdentificacion(cliente.Identificacion);
                var existeEmail = await ValidarClienteExistentePorEmail(cliente.Email);

                if (existeIdentificacion.Length > 0 || existeEmail.Length > 0)
                {
                    return (0, string.Join(", ", new[] { existeIdentificacion, existeEmail }.Where(e => e.Length > 0)));
                }

                var registrarCliente = await _sqlPort.ExecuteNonQueryAsync("SP_Cliente_Crear", cliente);

                if (registrarCliente > 0)
                {
                    return (registrarCliente, "Cliente creado exitosamente.");
                }
                else
                {
                    return (0, "No se pudo crear el cliente.");
                }
            }
            catch (Exception ex)
            {

                return (0, "Ocurrió un error inesperado al procesar la solicitud.");
            }

        }

        public async Task<(int id, string mensaje)> EditarCliente(ClienteRequest cliente)
        {
            try
            {
                string validarionErrores = ValidarClienteRequest(cliente, true);

                if (validarionErrores.Length > 0)
                {
                    return (0, validarionErrores);
                }

                var existeIdentificacion = await ValidarClienteExistentePorIdentificacion(cliente.Identificacion, cliente.Id);
                var existeEmail = await ValidarClienteExistentePorEmail(cliente.Email, cliente.Id);
                var existeId = await ValidarIdExistente(cliente.Id);

                if (existeIdentificacion.Length > 0 || existeEmail.Length > 0 || existeId.Length > 0)
                {
                    return (0, string.Join(", ", new[] { existeIdentificacion, existeEmail, existeId }.Where(e => e.Length > 0)));
                }

                var editar = await _sqlPort.ExecuteNonQueryAsync("SP_Cliente_Actualizar", cliente);

                if (editar > 0)
                {
                    return (editar, "Cliente actualizado exitosamente.");
                }
                else
                {
                    return (0, "No se pudo actualizar el cliente.");
                }
            }
            catch (Exception ex)
            {

                return (0, "Ocurrió un error inesperado al procesar la solicitud.");
            }

        }

        public async Task<(int id, string mensaje)> EliminarCliente(int clienteID)
        {
            try
            {
                var existeId = await ValidarIdExistente(clienteID);

                if (existeId.Length > 0)
                {
                    return (0, existeId);
                }

                var eliminar = await _sqlPort.ExecuteNonQueryAsync("SP_Cliente_Eliminar", new { ClienteID = clienteID, EliminacionFisica = false });
                if (eliminar > 0)
                {
                    return (eliminar, "Cliente eliminado exitosamente.");
                }
                else
                {
                    return (0, "No se pudo eliminar el cliente.");
                }
            }
            catch (Exception)
            {

                return (0, "Ocurrió un error inesperado al procesar la solicitud.");
            }
        }

        public static string ValidarClienteRequest(ClienteRequest clienteRequest, bool editar = false)
        {
            var errores = new List<string>();
            
            if (editar && clienteRequest.Id <= 0)
            {
                errores.Add("El campo Id es requerido para la edición");
            }
            
            if (clienteRequest == null)
            {
                return "El objeto ClienteRequest no puede ser nulo";
            }
            
            if (string.IsNullOrWhiteSpace(clienteRequest.Identificacion))
            {
                errores.Add("El campo Identificacion es requerido");
            }
            
            if (string.IsNullOrWhiteSpace(clienteRequest.Nombre))
            {
                errores.Add("El campo Nombre es requerido");
            }
            
            if (string.IsNullOrWhiteSpace(clienteRequest.Email))
            {
                errores.Add("El campo email es requerido");
            }
            
            return string.Join(", ", errores);
        }

        private async Task<string> ValidarClienteExistentePorIdentificacion(string Identificacion, int? ClienteIDExcluir = null)
        {
            var parameter = new { Identificacion, ClienteIDExcluir };

            var existe = await _sqlPort.ExecuteStoredProcedureBoolAsync("SP_Cliente_ExistePorIdentificacion", parameter);
            return existe ? "La identificación ya existe" : string.Empty;

        }

        private async Task<string> ValidarClienteExistentePorEmail(string Email, int? ClienteIDExcluir = null)
        {
            var parameter = new { Email, ClienteIDExcluir };

            var existe = await _sqlPort.ExecuteStoredProcedureBoolAsync("SP_Cliente_ExistePorEmail", parameter);
            return existe ? "El email ya existe" : string.Empty;
        }

        private async Task<string> ValidarIdExistente(int ClienteID)
        {
            var parameter = new { ClienteID };

            var existe = await _sqlPort.ExecuteStoredProcedureBoolAsync("SP_Cliente_ExistePorID", parameter);
            return !existe ? "El id del cliente es incorrecto" : string.Empty;
        }
    }

}
