using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SecurityLayer
{
    public class AuthorizationService
    {
        private readonly JwtService _jwtService;

        // Inyección de dependencia
        public AuthorizationService(JwtService jwtService)
        {
            _jwtService = jwtService;
        }

        public ClaimsPrincipal ValidateRole(string token, string expectedRole)
        {
            if (string.IsNullOrEmpty(token))
            {
                return null; // Si no hay token, devolver null
            }

            // Validar el token
            var principal = _jwtService.ValidateToken(token);
            if (principal == null)
            {
                return null; // Si el token no es válido, devolver null
            }

            // Verificar si el rol del usuario es el esperado
            var roleClaim = principal.Claims.FirstOrDefault(c => c.Type == "rol");
            if (roleClaim == null || roleClaim.Value != expectedRole)
            {
                return null; // Si el rol no es el esperado, devolver null
            }

            return principal; // Si todo es válido, devolver el principal (usuario)
        }
    }
}
