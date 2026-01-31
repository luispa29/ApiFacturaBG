using Application.Ports.Driving;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Models.Response;

namespace ApiCobrApp.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class JwtAuthorizationAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var jwtService = context.HttpContext.RequestServices.GetService<IJwtService>();
            var securitySettings = context.HttpContext.RequestServices.GetService<Microsoft.Extensions.Options.IOptions<Models.Utils.SecuritySettings>>()?.Value;
            
            if (jwtService == null)
            {
                context.Result = new JsonResult(RespuestaApi<object>.Error("Servicio de autenticación no disponible")) { StatusCode = 500 };
                return;
            }

            var authHeader = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                context.Result = new JsonResult(RespuestaApi<object>.Error("Token de autorización no proporcionado",401)) { StatusCode = 401 };
                return;
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();
            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();

            try
            {
                // Validación manual para capturar el error exacto
                var key = System.Text.Encoding.UTF8.GetBytes(securitySettings?.JwtSecret ?? "");
                tokenHandler.ValidateToken(token, new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = securitySettings?.JwtIssuer,
                    ValidateAudience = true,
                    ValidAudience = securitySettings?.JwtAudience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(5)
                }, out Microsoft.IdentityModel.Tokens.SecurityToken validatedToken);

                var userId = jwtService.ValidateTokenAndGetUserId(token);
                if (userId == null)
                {
                    context.Result = new JsonResult(RespuestaApi<object>.Error("Token válido pero no se pudo extraer el usuario (error de cifrado o claims)")) { StatusCode = 401 };
                    return;
                }

                context.HttpContext.Items["UserId"] = userId.Value;
            }
            catch (Microsoft.IdentityModel.Tokens.SecurityTokenExpiredException)
            {
                var now = DateTime.UtcNow;
                context.Result = new JsonResult(RespuestaApi<object>.Error($"El token ha expirado. Hora servidor (UTC): {now:yyyy-MM-dd HH:mm:ss}")) { StatusCode = 401 };
            }
            catch (Exception ex)
            {
                context.Result = new JsonResult(RespuestaApi<object>.Error($"Token inválido: {ex.Message}")) { StatusCode = 401 };
            }
        }
    }
}
