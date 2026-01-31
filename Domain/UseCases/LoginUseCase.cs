using Application.Ports.Driven;
using Application.Ports.Driving;
using Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.UseCases
{
    public class LoginUseCase(ISqlPort _sqlPort, IJwtService _jwt) : ILoginService
    {
        public async Task<(int codigo, string mensaje, LoginResponse login)> Login(string userName, string contrasena)
        {
            LoginResponse login = new();
            try
            {
                var parameter = new { Username = userName, Contrasena = contrasena };

                var usuario = await _sqlPort.ExecuteStoredProcedureSingleAsync<UsuarioResponse>("SP_Usuario_ValidarCredenciales", parameter);

                if (usuario == null)
                {
                    return (0, "Usuario o contraseña incorrectos.", login);
                }

                login.Username = usuario.Username;
                login.Nombre = usuario.Nombre;
                login.Email = usuario.Email;
                login.Token = _jwt.GenerateToken(usuario.UsuarioID);
                return (1, "Inicio de sesión exitoso.", login);
            }          
            catch (Exception)
            {

                return (0, "Ocurrió un error inesperado al procesar la solicitud.", login);
            }
        }
    }
}
