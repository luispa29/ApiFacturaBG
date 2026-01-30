using Microsoft.AspNetCore.Mvc;

namespace ApiFacturaBG.Extensions
{
    public static class ControllerExtensions
    {
        public static int GetUserId(this ControllerBase controller)
        {
            if (controller.HttpContext.Items.TryGetValue("UserId", out var userId))
            {
                return (int)userId;
            }
            
            throw new UnauthorizedAccessException("Usuario no autenticado");
        }
    }
}
