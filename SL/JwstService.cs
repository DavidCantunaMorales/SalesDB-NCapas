using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SL
{
    public static class JwtService
    {
        private const string SecretKey = "TuClaveSuperSecretaLargaDe32CaracteresOMas!"; // Cambia esta clave por algo seguro y almacénala de forma segura
        private const int TokenExpirationMinutes = 30; // Duración del token en minutos

        public static string GenerateToken(string username, string email, int idRol)
        {
            var key = Encoding.UTF8.GetBytes(SecretKey);
            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username), // Nombre de usuario
                new Claim(ClaimTypes.Email, email), // Email del usuario
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // Identificador único del token
                new Claim("rol", idRol.ToString()) // Añadir el IDRol como string
            };

            var token = new JwtSecurityToken(
                issuer: "TuAplicacion",
                audience: "TuAplicacion",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(TokenExpirationMinutes),
                signingCredentials: credentials
            );

            string generatedToken = new JwtSecurityTokenHandler().WriteToken(token);

            Console.WriteLine($"Token generado: {generatedToken}"); // Depuración para ver el token generado

            return generatedToken;
        }


        // Método para validar el token y devolver los datos del usuario
        public static ClaimsPrincipal ValidateToken(string token)
        {
            var key = Encoding.UTF8.GetBytes(SecretKey);
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = "TuAplicacion",
                    ValidAudience = "TuAplicacion",
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                }, out SecurityToken validatedToken);

                Console.WriteLine("Token validado con éxito.");  // Confirmación de validación

                // Mostramos las claims extraídas del token
                foreach (var claim in principal.Claims)
                {
                    Console.WriteLine($"Claim: {claim.Type} - {claim.Value}");
                }

                return principal; // Si el token es válido, devuelve el principal (usuario)
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error de validación: {ex.Message}");  // Mensaje de error si el token no es válido
                return null; // Si el token no es válido, devuelve null
            }
        }
    }
}
