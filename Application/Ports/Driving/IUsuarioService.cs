using Models.Request;
using Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Ports.Driving
{
    public interface IUsuarioService
    {
        Task<(int id, string mensaje)> CrearUsuario(UsuarioRequest usuarioRequest);
        Task<(int id, string mensaje)> EditarUsuario(UsuarioUpdateRequest usuarioRequest);
        Task<(int id, string mensaje)> EliminarUsuario(int usuarioID);
        Task<(List<UsuarioResponse> usuarios, int totalRegistros)> Obtener(int numeroPagina, int tamanoPagina, string filtro, bool soloActivos);
    }
}
