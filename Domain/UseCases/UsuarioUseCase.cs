using Application.Ports.Driven;
using Application.Ports.Driving;
using Models.Request;

namespace Domain.UseCases
{
    public class UsuarioUseCase(ISqlPort _sqlPort) : IUsuarioService
    {
        public async Task<(int id, string mensaje)> CrearUsuario(UsuarioRequest usuarioRequest)
        {
            try
            {
                string validarionErrores = ValidarUsuarioRequest(usuarioRequest);

                if (validarionErrores.Length > 0)
                {
                    return (0, validarionErrores);
                }

                var existeNombre = await ValidarUsuarioExistente(usuarioRequest.Username);
                var existeCorreo = await ValidarCorreoExistente(usuarioRequest.Username);

                if (existeNombre.Length > 0 || existeCorreo.Length > 0)
                {
                    return (0, string.Join(", ", new[] { existeNombre, existeCorreo }.Where(e => e.Length > 0)));
                }

                var registrarUsuario= await _sqlPort.ExecuteNonQueryAsync("SP_Usuario_Crear", usuarioRequest);

                if(registrarUsuario > 0)
                {
                    return (registrarUsuario, "Usuario creado exitosamente.");
                }
                else
                {
                    return (0, "No se pudo crear el usuario.");
                }
            }
            catch (Exception ex)
            {

                return (0, "Ocurrió un error inesperado al procesar la solicitud.");
            }

        }


        public string ValidarUsuarioRequest(UsuarioRequest usuarioRequest)
        {
            var errores = new List<string>();
            if (usuarioRequest == null)
            {
                return "El objeto UsuarioRequest no puede ser nulo";
            }
            if (string.IsNullOrWhiteSpace(usuarioRequest.Username))
            {
                errores.Add("El campo NombreUsuario es requerido");
            }
            if (string.IsNullOrWhiteSpace(usuarioRequest.Contrasena))
            {
                errores.Add("El campo Contrasena es requerido");
            }
            if (string.IsNullOrWhiteSpace(usuarioRequest.Nombre))
            {
                errores.Add("El campo Nombre es requerido");
            }
            if (string.IsNullOrWhiteSpace(usuarioRequest.Email))
            {
                errores.Add("El campo CorreoElectronico es requerido");
            }
            return string.Join(", ", errores);
        }

        private async Task<string> ValidarUsuarioExistente(string Username, int? UsuarioIDExcluir = null)
        {
            var parameter = new { Username, UsuarioIDExcluir };

            var existe = await _sqlPort.ExecuteStoredProcedureBoolAsync("SP_Usuario_ExistePorUsername", parameter);
            return existe ? "El usuario ya existe" : string.Empty;

        }

        private async Task<string> ValidarCorreoExistente(string Email, int? UsuarioIDExcluir = null)
        {
            var parameter = new { Email, UsuarioIDExcluir };

            var existe = await _sqlPort.ExecuteStoredProcedureBoolAsync("SP_Usuario_ExistePorEmail", parameter);
            return existe ? "El correo ya existe" : string.Empty;
        }
    }

}
