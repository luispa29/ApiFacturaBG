using Models.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Ports.Driving
{
    public interface ILoginService
    {
        Task<(int codigo, string mensaje, LoginResponse login)> Login(string userName, string contrasena);
    }
}
