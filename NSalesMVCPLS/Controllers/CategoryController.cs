using NSalesMVCPLS.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Entities;
using NSalesProxyService;
using System.Web.Security;

namespace NSalesMVCPLS.Controllers
{
    public class CategoryController : Controller
    {
        private readonly Proxy _proxy;

        public CategoryController()
        {
            // Inicializa el proxy para interactuar con la API
            _proxy = new Proxy();
        }

        [RoleAuthorize("1", "2", "3")]
        public ActionResult Index()
        {
            try
            {
                // Recupera el token de la cookie
                var authCookie = Request.Cookies["AuthToken"];
                if (authCookie == null || string.IsNullOrEmpty(authCookie.Value))
                {
                    // Si no hay token, redirige al login
                    return RedirectToAction("Dashboard", "Home");
                }

                // Llamada a la API para obtener todas las categorías utilizando el proxy y pasando el token
                var categories = _proxy.GetAllCategories(authCookie.Value);

                if (categories == null || !categories.Any())
                {
                    ViewBag.ErrorMessage = "No se encontraron categorías.";
                    return View(new List<Categories>());
                }

                // Retorna los datos a la vista
                return View(categories);
            }
            catch (Exception ex)
            {
                // Manejo de errores
                ViewBag.ErrorMessage = $"Ocurrió un error: {ex.Message}";
                return View(new List<Categories>());
            }
        }

        // GET: Category/Details/5
        [RoleAuthorize("1", "2", "3")]
        public ActionResult Details(int id)
        {
            try
            {
                // Recupera el token de autenticación de la cookie
                var authCookie = Request.Cookies["AuthToken"];
                if (authCookie == null || string.IsNullOrEmpty(authCookie.Value))
                {
                    // Si no hay token, redirige al login
                    return RedirectToAction("Index", "Category");
                }

                // Llamada a la API para recuperar la categoría por su ID
                var category = _proxy.RetrieveCategoryById(id, authCookie.Value);
                Console.WriteLine(category);
                if (category == null)
                {
                    ViewBag.ErrorMessage = "La categoría no fue encontrada.";
                    return View();
                }

                // Pasa la categoría a la vista para mostrarla
                return View(category);
            }
            catch (Exception ex)
            {
                // Manejo de errores
                ViewBag.ErrorMessage = $"Ocurrió un error: {ex.Message}";
                return View();
            }
        }
        // GET: Category/Create
        [RoleAuthorize("1", "2")]
        public ActionResult Create()
        {
            return View();
        }

        [RoleAuthorize("1", "2")]
        // POST: Category/Create
        [HttpPost]
        public ActionResult Create(Categories newCategory)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Recupera el token de autenticación desde la cookie
                    var authCookie = Request.Cookies["AuthToken"];
                    if (authCookie == null || string.IsNullOrEmpty(authCookie.Value))
                    {
                        return RedirectToAction("Index", "Category");
                    }

                    // Llamada al proxy para crear la nueva categoría
                    Categories createdCategory = _proxy.CreateCategory(newCategory, authCookie.Value);



                    if (createdCategory != null)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Error al crear la categoría.";
                        return View(newCategory);
                    }
                }
                return View(newCategory);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Ocurrió un error: {ex.Message}";
                return View(newCategory);
            }
        }

        [RoleAuthorize("1", "2")]
        // GET: Category/Edit/5
        public ActionResult Edit(int id)
        {
            try
            {
                // Recupera el token de autenticación desde la cookie
                var authCookie = Request.Cookies["AuthToken"];
                if (authCookie == null || string.IsNullOrEmpty(authCookie.Value))
                {
                    return RedirectToAction("Index", "Category");
                }

                // Llamada al proxy para obtener los detalles de la categoría
                var category = _proxy.RetrieveCategoryById(id, authCookie.Value);

                if (category == null)
                {
                    ViewBag.ErrorMessage = "Categoría no encontrada.";
                    return RedirectToAction("Index");
                }

                return View(category);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Ocurrió un error: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        [RoleAuthorize("1", "2")]
        // POST: Category/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, Categories updatedCategory)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Recupera el token de autenticación desde la cookie
                    var authCookie = Request.Cookies["AuthToken"];
                    if (authCookie == null || string.IsNullOrEmpty(authCookie.Value))
                    {
                        return RedirectToAction("Index", "Category");
                    }

                    // Llamada al proxy para actualizar la categoría
                    bool success = _proxy.UpdateCategory(updatedCategory, authCookie.Value);

                    if (success)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Error al editar la categoría.";
                        return View(updatedCategory);
                    }
                }
                return View(updatedCategory);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Ocurrió un error: {ex.Message}";
                return View(updatedCategory);
            }
        }

        [RoleAuthorize("1", "2")]
        // GET: Category/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                // Recupera el token de autenticación desde la cookie
                var authCookie = Request.Cookies["AuthToken"];
                if (authCookie == null || string.IsNullOrEmpty(authCookie.Value))
                {
                    return RedirectToAction("Index", "Category");
                }

                // Llamada al proxy para obtener los detalles de la categoría
                var category = _proxy.RetrieveCategoryById(id, authCookie.Value);

                if (category == null)
                {
                    ViewBag.ErrorMessage = "Categoría no encontrada.";
                    return RedirectToAction("Index");
                }

                return View(category);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Ocurrió un error: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        [RoleAuthorize("1", "2")]
        // POST: Category/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // Recupera el token de autenticación desde la cookie
                var authCookie = Request.Cookies["AuthToken"];
                if (authCookie == null || string.IsNullOrEmpty(authCookie.Value))
                {
                    return RedirectToAction("Index", "Category");
                }

                // Llamada al proxy para eliminar la categoría
                bool success = _proxy.DeleteCategory(id, authCookie.Value);

                if (success)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.ErrorMessage = "Error al eliminar la categoría.";
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
