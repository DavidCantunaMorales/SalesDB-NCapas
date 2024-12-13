using SecurityLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NSalesMVCPLS.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            // Si ya está autenticado, redirigir al dashboard
            if (Request.Cookies["AuthToken"] != null)
            {
                return RedirectToAction("Dashboard");
            }

            return View(); // Esta es la vista de login (si no está autenticado)
        }

        // Acción para el Dashboard, que contiene las opciones de Usuarios, Categorías y Productos
        public ActionResult Dashboard()
        {
            var authCookie = Request.Cookies["AuthToken"];
            if (authCookie == null)
            {
                return RedirectToAction("Index", "Login");
            }

            string token = authCookie.Value;
            string role = JwtService.GetRoleFromToken(token);

            if (string.IsNullOrEmpty(role))
            {
                return RedirectToAction("Logout"); // Si no se puede obtener el rol, cerrar sesión
            }

            int roleId;
            if (!int.TryParse(role, out roleId))
            {
                return RedirectToAction("Logout"); // Si el rol no es válido, cerrar sesión
            }

            ViewBag.UserRole = roleId;

            return View();
        }

        // Acción de logout
        public ActionResult Logout()
        {
            // Eliminar la cookie de autenticación al cerrar sesión
            if (Request.Cookies["AuthToken"] != null)
            {
                var cookie = new HttpCookie("AuthToken")
                {
                    Expires = DateTime.Now.AddDays(-1) // Establecer la cookie como expirada
                };
                Response.Cookies.Add(cookie);
            }

            return RedirectToAction("Index", "Login");
        }
    }
}
