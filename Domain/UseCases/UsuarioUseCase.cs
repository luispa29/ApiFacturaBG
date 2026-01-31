using Application.Ports.Driven;
using Application.Ports.Driving;
using Microsoft.AspNetCore.Components.Forms;
using Models.Request;

namespace Domain.UseCases
{
    public class UsuarioUseCase(ISqlPort _sqlPort) : IUsuarioService
    {
        public async Task<(int id, string mensaje)> CrearUsuario(UsuarioRequest usuario)
        {
            try
            {
                string validarionErrores = ValidarUsuarioRequest(usuario);

                if (validarionErrores.Length > 0)
                {
                    return (0, validarionErrores);
                }

                var existeNombre = await ValidarUsuarioExistente(usuario.Username);
                var existeCorreo = await ValidarCorreoExistente(usuario.Username);

                if (existeNombre.Length > 0 || existeCorreo.Length > 0)
                {
                    return (0, string.Join(", ", new[] { existeNombre, existeCorreo }.Where(e => e.Length > 0)));
                }

                var registrarUsuario = await _sqlPort.ExecuteNonQueryAsync("SP_Usuario_Crear", usuario);

                if (registrarUsuario > 0)
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
        public async Task<(int id, string mensaje)> EditarUsuario(UsuarioUpdateRequest usuario)
        {
            try
            {
                var usuarioRequest = new UsuarioRequest
                {
                    Id = usuario.Id,
                    Username = usuario.Username,
                    Contrasena = usuario.ActualizarContrasena ? usuario.NuevaContrasena : string.Empty,
                    Nombre = usuario.Nombre,
                    Email = usuario.Email
                };
                string validarionErrores = ValidarUsuarioRequest(usuarioRequest, true, usuario.ActualizarContrasena);

                if (validarionErrores.Length > 0)
                {
                    return (0, validarionErrores);
                }

                var existeNombre = await ValidarUsuarioExistente(usuario.Username, usuario.Id);
                var existeCorreo = await ValidarCorreoExistente(usuario.Username, usuario.Id);
                var existeId = await ValidarIdExistente(usuario.Id);

                if (existeNombre.Length > 0 || existeCorreo.Length > 0 || existeId.Length > 0)
                {
                    return (0, string.Join(", ", new[] { existeNombre, existeCorreo, existeId }.Where(e => e.Length > 0)));
                }


                var editar = await _sqlPort.ExecuteNonQueryAsync("SP_Usuario_Actualizar", usuario);

                if (editar > 0)
                {
                    return (editar, "Usuario aactualizado exitosamente.");
                }
                else
                {
                    return (0, "No se pudo actualizar el usuario.");
                }
            }
            catch (Exception ex)
            {

                return (0, "Ocurrió un error inesperado al procesar la solicitud.");
            }

        }


        public static string ValidarUsuarioRequest(UsuarioRequest usuarioRequest, bool editar = false, bool actualizarConstrasena = true)
        {
            var errores = new List<string>();
            if (editar && usuarioRequest.Id <= 0)
            {
                errores.Add("El campo Id es requerido para la edición");
            }
            if (usuarioRequest == null)
            {
                return "El objeto UsuarioRequest no puede ser nulo";
            }
            if (string.IsNullOrWhiteSpace(usuarioRequest.Username))
            {
                errores.Add("El campo NombreUsuario es requerido");
            }
            if (string.IsNullOrWhiteSpace(usuarioRequest.Contrasena) && actualizarConstrasena)
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
        private async Task<string> ValidarIdExistente(int UsuarioID)
        {
            var parameter = new { UsuarioID };

            var existe = await _sqlPort.ExecuteStoredProcedureBoolAsync("SP_Usuario_ExistePorID", parameter);
            return !existe ? "El id del usuario es incorrecto" : string.Empty;
        }

        public async Task<(int id, string mensaje)> EliminarUsuario(int usuarioID)
        {
            try
            {
                var existeId = await ValidarIdExistente(usuarioID);

                if (existeId.Length > 0)
                {
                    return (0, existeId);
                }

                var eliminar = await _sqlPort.ExecuteNonQueryAsync("SP_Usuario_Eliminar", new { UsuarioID = usuarioID, EliminacionFisica = false });
                if (eliminar > 0)
                {
                    return (eliminar, "Usuario eliminado exitosamente.");
                }
                else
                {
                    return (0, "No se pudo eliminado el usuario.");
                }
            }
            catch (Exception)
            {

                return (0, "Ocurrió un error inesperado al procesar la solicitud.");
            }
        }
    }

}
