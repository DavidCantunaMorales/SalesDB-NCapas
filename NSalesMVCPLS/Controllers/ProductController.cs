using Entities;
using NSalesMVCPLS.Filters;
using NSalesMVCPLS.Models;
using NSalesProxyService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NSalesMVCPLS.Controllers
{
    public class ProductController : Controller
    {
        private readonly Proxy _proxy;

        public ProductController()
        {
            // Inicializa el proxy para interactuar con la API
            _proxy = new Proxy();
        }

        [RoleAuthorize("1", "2", "3")]
        public ActionResult Index()
        {
            try
            {
                var authCookie = Request.Cookies["AuthToken"];
                if (authCookie == null || string.IsNullOrEmpty(authCookie.Value))
                {
                    return RedirectToAction("Index", "Login");
                }

                // Llamada a la API para obtener todos los productos
                var products = _proxy.RetrieveAllProducts(authCookie.Value);

                if (products == null || !products.Any())
                {
                    ViewBag.ErrorMessage = "No se encontraron productos.";
                    return View(new List<ProductViewModel>());
                }

                // Llamada a la API para obtener las categorías
                var categories = _proxy.GetAllCategories(authCookie.Value);

                // Crear la lista de ProductViewModel combinando productos y categorías
                var productViewModels = products.Select(product =>
                {
                    var category = categories.FirstOrDefault(c => c.CategoryID == product.CategoryID);
                    return new ProductViewModel
                    {
                        ProductID = product.ProductID,
                        ProductName = product.ProductName,
                        CategoryName = category?.CategoryName ?? "Desconocida", // Si no encuentra categoría, asigna "Desconocida"
                        UnitPrice = product.UnitPrice,
                        UnitsInStock = product.UnitsInStock
                    };
                }).ToList();

                // Pasar el listado de ProductViewModel a la vista
                return View(productViewModels);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Ocurrió un error: {ex.Message}";
                return View(new List<ProductViewModel>());
            }
        }

        [RoleAuthorize("1", "2", "3")]
        public ActionResult Details(int id)
        {
            try
            {
                var authCookie = Request.Cookies["AuthToken"];
                if (authCookie == null || string.IsNullOrEmpty(authCookie.Value))
                {
                    return RedirectToAction("Index", "Login");
                }

                var product = _proxy.RetrieveProductById(id, authCookie.Value);
                Console.WriteLine(product);

                if (product == null)
                {
                    ViewBag.ErrorMessage = "El producto no fue encontrado.";
                    return View();
                }

                return View(product);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Ocurrió un error: {ex.Message}";
                return View();
            }
        }

        [RoleAuthorize("1", "2")]
        [HttpGet]
        public ActionResult Create()
        {
            try
            {
                var authCookie = Request.Cookies["AuthToken"];
                if (authCookie == null || string.IsNullOrEmpty(authCookie.Value))
                {
                    return RedirectToAction("Index", "Login");
                }

                // Llamada a la API o base de datos para obtener las categorías
                var categories = _proxy.GetAllCategories(authCookie.Value);

                // Convertir las categorías a un formato que DropDownListFor pueda utilizar
                var categorySelectList = new SelectList(categories, "CategoryID", "CategoryName");

                // Pasar las categorías al ViewBag
                ViewBag.Categories = categorySelectList;

                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Ocurrió un error: {ex.Message}";
                return View();
            }
        }

        [RoleAuthorize("1", "2")]
        [HttpPost]
        public ActionResult Create(Products newProduct)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var authCookie = Request.Cookies["AuthToken"];
                    if (authCookie == null || string.IsNullOrEmpty(authCookie.Value))
                    {
                        return RedirectToAction("Index", "Login");
                    }

                    var createdProduct = _proxy.CreateProduct(newProduct, authCookie.Value);


                    if (createdProduct != null)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Error al crear el producto.";
                        return View(newProduct);
                    }
                }
                return View(newProduct);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Ocurrió un error: {ex.Message}";
                return View(newProduct);
            }
        }

        [RoleAuthorize("1", "2")]
        public ActionResult Edit(int id)
        {
            try
            {
                var authCookie = Request.Cookies["AuthToken"];
                if (authCookie == null || string.IsNullOrEmpty(authCookie.Value))
                {
                    return RedirectToAction("Index", "Login");
                }

                var product = _proxy.RetrieveProductById(id, authCookie.Value);

                if (product == null)
                {
                    ViewBag.ErrorMessage = "Producto no encontrado.";
                    return RedirectToAction("Index");
                }

                // Obtener las categorías para el dropdown
                var categories = _proxy.GetAllCategories(authCookie.Value); // Asegúrate de que este método existe en tu API

                ViewBag.Categories = new SelectList(categories, "CategoryID", "CategoryName", product.CategoryID);

                return View(product);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Ocurrió un error: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        [RoleAuthorize("1", "2")]
        [HttpPost]
        public ActionResult Edit(int id, Products updatedProduct)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var authCookie = Request.Cookies["AuthToken"];
                    if (authCookie == null || string.IsNullOrEmpty(authCookie.Value))
                    {
                        return RedirectToAction("Index", "Login");
                    }

                    bool success = _proxy.UpdateProduct(updatedProduct, authCookie.Value);

                    if (success)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Error al editar el producto.";
                        return View(updatedProduct);
                    }
                }
                return View(updatedProduct);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Ocurrió un error: {ex.Message}";
                return View(updatedProduct);
            }
        }

        [RoleAuthorize("1", "2")]
        public ActionResult Delete(int id)
        {
            try
            {
                var authCookie = Request.Cookies["AuthToken"];
                if (authCookie == null || string.IsNullOrEmpty(authCookie.Value))
                {
                    return RedirectToAction("Index", "Login");
                }

                var product = _proxy.RetrieveProductById(id, authCookie.Value);

                if (product == null)
                {
                    ViewBag.ErrorMessage = "Producto no encontrado.";
                    return RedirectToAction("Index");
                }

                return View(product);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Ocurrió un error: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        [RoleAuthorize("1", "2")]
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                var authCookie = Request.Cookies["AuthToken"];
                if (authCookie == null || string.IsNullOrEmpty(authCookie.Value))
                {
                    return RedirectToAction("Index", "Login");
                }

                bool success = _proxy.DeleteProduct(id, authCookie.Value);

                if (success)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.ErrorMessage = "Error al eliminar el producto.";
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
