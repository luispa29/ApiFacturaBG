using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using System.Globalization;
using Application.Ports.Driving;
using Models.Utils;

namespace Domain.Services
{
    public class JwtService : IJwtService
    {
        private readonly SecuritySettings _securitySettings;
        private readonly IAesEncryptionService _aesEncryptionService;
        private const string KeyId = "FacturaBG-Default-Key";

        public JwtService(IOptions<SecuritySettings> securitySettings, IAesEncryptionService aesEncryptionService)
        {
            _securitySettings = securitySettings.Value;
            _aesEncryptionService = aesEncryptionService;
        }

        public string GenerateToken(int userId)
        {
            var encryptedUserId = _aesEncryptionService.Encrypt(userId.ToString(CultureInfo.InvariantCulture));

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_securitySettings.JwtSecret))
            {
                KeyId = KeyId
            };

            var now = DateTime.UtcNow;

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(JwtRegisteredClaimNames.Sub, encryptedUserId),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                IssuedAt = now,
                NotBefore = now,
                Expires = now.AddHours(_securitySettings.JwtExpirationHours),
                Issuer = _securitySettings.JwtIssuer,
                Audience = _securitySettings.JwtAudience,
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public int? ValidateTokenAndGetUserId(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_securitySettings.JwtSecret))
            {
                KeyId = KeyId
            };

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = true,
                    ValidIssuer = _securitySettings.JwtIssuer,
                    ValidateAudience = true,
                    ValidAudience = _securitySettings.JwtAudience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(5)
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;

                var subClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Sub || x.Type == "sub");
                if (subClaim == null) return null;

                var encryptedUserId = subClaim.Value;
                var userIdString = _aesEncryptionService.Decrypt(encryptedUserId);
                
                return int.TryParse(userIdString, out int userId) ? userId : null;
            }
            catch (Exception ex)
            {
                // Registramos el error internamente si es necesario
                System.Diagnostics.Debug.WriteLine($"Error validando token: {ex.Message}");
                throw; // Re-lanzamos para que el filtro capture el mensaje exacto
            }
        }
    }
}
