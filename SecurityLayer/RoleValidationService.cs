using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SecurityLayer
{
    public class RoleValidationService
    {
        // Definición de roles como constantes
        public const string AdminRole = "1";   // ID del rol de Administrador
        public const string EditorRole = "2";  // ID del rol de Editor
        public const string ViewerRole = "3";  // ID del rol de Viewer

        // Método que valida si el usuario tiene el rol de Administrador
        public bool IsAdmin(HttpRequestMessage request)
        {
            return HasRole(request, AdminRole);
        }

        // Método que valida si el usuario tiene el rol de Editor
        public bool IsEditor(HttpRequestMessage request)
        {
            return HasRole(request, EditorRole);
        }

        // Método que valida si el usuario tiene el rol de Viewer
        public bool IsViewer(HttpRequestMessage request)
        {
            return HasRole(request, ViewerRole);
        }

        // Método genérico para validar si el usuario tiene el rol especificado
        private bool HasRole(HttpRequestMessage request, string role)
        {
            var token = request.Headers.Authorization?.Parameter;
            if (string.IsNullOrEmpty(token))
            {
                return false;
            }

            var principal = JwtService.ValidateToken(token);  // Asegúrate de que JwtService tenga este método
            if (principal == null)
            {
                return false;
            }

            var roleClaim = principal.Claims.FirstOrDefault(c => c.Type == "rol");
            return roleClaim != null && roleClaim.Value == role;
        }
    }
}
