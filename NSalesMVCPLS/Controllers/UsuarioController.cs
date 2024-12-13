using Entities;
using NSalesMVCPLS.Filters;
using NSalesProxyService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NSalesMVCPLS.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly Proxy _proxy;

        public UsuarioController()
        {
            // Inicializa el proxy para interactuar con la API
            _proxy = new Proxy();
        }

        // GET: Usuario
        [RoleAuthorize("1")]
        public ActionResult Index()
        {
            try
            {
                var authCookie = Request.Cookies["AuthToken"];
                if (authCookie == null || string.IsNullOrEmpty(authCookie.Value))
                {
                    return RedirectToAction("Dashboard", "Home");
                }

                var usuarios = _proxy.RetrieveAllUsers(authCookie.Value);

                if (usuarios == null || !usuarios.Any())
                {
                    ViewBag.ErrorMessage = "No se encontraron usuarios.";
                    return View(new List<Usuarios>());
                }

                return View(usuarios);
            } 
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Ocurrió un error: {ex.Message}";
                return View(new List<Usuarios>());
            }
        }

        // GET: Usuario/Details/5
        [RoleAuthorize("1")]
        public ActionResult Details(int id)
        {
            try
            {
                // Recupera el token de autenticación de la cookie
                var authCookie = Request.Cookies["AuthToken"];
                if (authCookie == null || string.IsNullOrEmpty(authCookie.Value))
                {
                    // Si no hay token, redirige al login
                    return RedirectToAction("Dashboard", "Home");
                }

                // Llamada a la API para recuperar el usuario por su ID
                var usuario = _proxy.RetrieveUserById(id, authCookie.Value);

                if (usuario == null)
                {
                    ViewBag.ErrorMessage = "El usuario no fue encontrado.";
                    return View();
                }

                // Pasa el usuario a la vista para mostrarlo
                return View(usuario);
            }
            catch (Exception ex)
            {
                // Manejo de errores
                ViewBag.ErrorMessage = $"Ocurrió un error: {ex.Message}";
                return View();
            }
        }

        // GET: Usuario/Create
        [RoleAuthorize("1")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Usuario/Create
        [HttpPost]
        [RoleAuthorize("1")]
        public ActionResult Create(Usuarios newUsuario)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Recupera el token de autenticación desde la cookie
                    var authCookie = Request.Cookies["AuthToken"];
                    if (authCookie == null || string.IsNullOrEmpty(authCookie.Value))
                    {
                        return RedirectToAction("Dashboard", "Home");
                    }

                    // Llamada al proxy para crear el nuevo usuario
                    Usuarios createdUsuario = _proxy.CreateUser(newUsuario, authCookie.Value);

                    if (createdUsuario != null)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Error al crear el usuario.";
                        return View(newUsuario);
                    }
                }
                return View(newUsuario);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Ocurrió un error: {ex.Message}";
                return View(newUsuario);
            }
        }


        // GET: Usuario/Edit/5
        [RoleAuthorize("1")]
        public ActionResult Edit(int id)
        {
            try
            {
                // Recupera el token de autenticación desde la cookie
                var authCookie = Request.Cookies["AuthToken"];
                if (authCookie == null || string.IsNullOrEmpty(authCookie.Value))
                {
                    return RedirectToAction("Dashboard", "Home");
                }

                // Llamada al proxy para obtener los detalles del usuario
                var usuario = _proxy.RetrieveUserById(id, authCookie.Value);

                if (usuario == null)
                {
                    ViewBag.ErrorMessage = "Usuario no encontrado.";
                    return RedirectToAction("Index");
                }

                return View(usuario);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Ocurrió un error: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        // POST: Usuario/Edit/5
        [HttpPost]
        [RoleAuthorize("1")]
        public ActionResult Edit(int id, Usuarios updatedUsuario)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Recupera el token de autenticación desde la cookie
                    var authCookie = Request.Cookies["AuthToken"];
                    if (authCookie == null || string.IsNullOrEmpty(authCookie.Value))
                    {
                        return RedirectToAction("Dashboard", "Home");
                    }

                    // Llamada al proxy para actualizar el usuario
                    bool success = _proxy.UpdateUser(updatedUsuario, authCookie.Value);

                    if (success)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Error al editar el usuario.";
                        return View(updatedUsuario);
                    }
                }
                return View(updatedUsuario);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Ocurrió un error: {ex.Message}";
                return View(updatedUsuario);
            }
        }

        // GET: Usuario/Delete/5
        [RoleAuthorize("1")]
        public ActionResult Delete(int id)
        {
            try
            {
                // Recupera el token de autenticación desde la cookie
                var authCookie = Request.Cookies["AuthToken"];
                if (authCookie == null || string.IsNullOrEmpty(authCookie.Value))
                {
                    return RedirectToAction("Dashboard", "Home");
                }

                // Llamada al proxy para obtener los detalles del usuario
                var usuario = _proxy.RetrieveUserById(id, authCookie.Value);

                if (usuario == null)
                {
                    ViewBag.ErrorMessage = "Usuario no encontrado.";
                    return RedirectToAction("Index");
                }

                return View(usuario);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Ocurrió un error: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        // POST: Usuario/Delete/5
        [HttpPost]
        [RoleAuthorize("1")]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // Recupera el token de autenticación desde la cookie
                var authCookie = Request.Cookies["AuthToken"];
                if (authCookie == null || string.IsNullOrEmpty(authCookie.Value))
                {
                    return RedirectToAction("Dashboard", "Home");
                }

                // Llamada al proxy para eliminar el usuario
                bool success = _proxy.DesactivateUser(id, authCookie.Value);

                if (success)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.ErrorMessage = "Error al eliminar el usuario.";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Ocurrió un error: {ex.Message}";
                return RedirectToAction("Index");
            }
        }
    }
}
