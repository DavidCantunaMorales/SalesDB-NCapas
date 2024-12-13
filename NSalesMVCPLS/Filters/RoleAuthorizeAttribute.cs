using SecurityLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NSalesMVCPLS.Filters
{
    public class RoleAuthorizeAttribute : AuthorizeAttribute
    {
        private readonly string[] _allowedRoles;

        public RoleAuthorizeAttribute(params string[] roles)
        {
            _allowedRoles = roles;
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            // Verifica si hay una cookie de autenticación
            var authCookie = httpContext.Request.Cookies["AuthToken"];
            if (authCookie == null)
            {
                return false; // No autenticado
            }

            string token = authCookie.Value;
            string role = JwtService.GetRoleFromToken(token);

            if (string.IsNullOrEmpty(role))
            {
                return false; // Token inválido o sin rol
            }

            // Verifica si el rol del usuario está en los roles permitidos
            return _allowedRoles.Contains(role);
        }


        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            var authCookie = filterContext.HttpContext.Request.Cookies["AuthToken"];
            if (authCookie != null && !string.IsNullOrEmpty(authCookie.Value))
            {
                string token = authCookie.Value;
                string role = JwtService.GetRoleFromToken(token);

                // Verifica el controlador y la acción actual
                string controllerName = filterContext.RouteData.Values["controller"].ToString();
                string actionName = filterContext.RouteData.Values["action"].ToString();

                // Si el rol es 3, redirigir dependiendo del controlador y acción
                if (role == "3")
                {
                    if (actionName == "Create" || actionName == "Edit" || actionName == "Delete")
                    {
                        // Redirige a Index si está intentando hacer una acción no permitida
                        if (controllerName == "Category")
                        {
                            filterContext.Result = new RedirectToRouteResult(
                                new System.Web.Routing.RouteValueDictionary(
                                    new { controller = "Category", action = "Index" }
                                )
                            );
                        }
                        else if (controllerName == "Product")
                        {
                            filterContext.Result = new RedirectToRouteResult(
                                new System.Web.Routing.RouteValueDictionary(
                                    new { controller = "Product", action = "Index" }
                                )
                            );
                        }
                    }
                    else
                    {
                        // Permitir solo acciones GET (mostrar detalles o listado)
                        return;
                    }
                }
                else
                {
                    // Redirige al Dashboard si no está autorizado
                    filterContext.Result = new RedirectToRouteResult(
                        new System.Web.Routing.RouteValueDictionary(
                            new { controller = "Home", action = "Dashboard" }
                        )
                    );
                }
            }
            else
            {
                // Si no hay token, redirige al login o a otra página
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary(
                        new { controller = "Login", action = "Index" }
                    )
                );
            }
        }

    }
}