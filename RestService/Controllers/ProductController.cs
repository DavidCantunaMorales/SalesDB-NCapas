using BLL;
using Entities;
using SecurityLayer;
using SLC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace RestService.Controllers
{
    public class ProductController : ApiController
    {
        private readonly RoleValidationService _roleValidationService;

        // Constructor
        public ProductController()
        {
            _roleValidationService = new RoleValidationService();  // Instancia del servicio de validación de roles
        }

        // Obtener todos los productos (todos los roles pueden)
        [HttpGet]
        public IHttpActionResult GetAllProducts()
        {
            try
            {
                if (!_roleValidationService.IsViewer(Request) && !_roleValidationService.IsAdmin(Request) && !_roleValidationService.IsEditor(Request))
                {
                    return Content(HttpStatusCode.Unauthorized, "No tienes permiso para ver los productos. Solo los roles Admin, Editor y Viewer pueden hacerlo.");
                }

                var productLogic = new ProductLogic();
                var products = productLogic.RetrieveAllProducts();
                return Ok(products);  // Retorna la lista de productos con un 200 OK
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception($"Error al obtener los productos: {ex.Message}"));
            }
        }

        // Obtener un producto por ID (todos los roles pueden)
        [HttpGet]
        public IHttpActionResult GetProduct(int id)
        {
            try
            {
                if (!_roleValidationService.IsViewer(Request) && !_roleValidationService.IsAdmin(Request) && !_roleValidationService.IsEditor(Request))
                {
                    return Content(HttpStatusCode.Unauthorized, "No tienes permiso para ver los detalles del producto. Solo los roles Admin, Editor y Viewer pueden hacerlo.");
                }

                var productLogic = new ProductLogic();
                var product = productLogic.RetrieveByID(id);
                if (product == null)
                {
                    return Content(HttpStatusCode.NotFound, "El producto solicitado no fue encontrado.");
                }

                return Ok(product);  // Retorna el producto encontrado con un 200 OK
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception($"Error al obtener el producto: {ex.Message}"));
            }
        }

        // Crear un producto (solo Admin y Editor pueden)
        [HttpPost]
        public IHttpActionResult CreateProduct(Products product)
        {
            if (!_roleValidationService.IsAdmin(Request) && !_roleValidationService.IsEditor(Request))
            {
                return Content(HttpStatusCode.Unauthorized, "No tienes permiso para crear un producto. Solo los roles Admin y Editor pueden hacerlo.");
            }

            try
            {
                var productLogic = new ProductLogic();
                var newProduct = productLogic.Create(product);
                return Ok(newProduct);  // Retorna el producto creado con un 200 OK
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception($"Error al crear el producto: {ex.Message}"));
            }
        }

        // Actualizar un producto (solo Admin y Editor pueden)
        [HttpPut]
        public IHttpActionResult UpdateProduct(Products product)
        {
            if (!_roleValidationService.IsAdmin(Request) && !_roleValidationService.IsEditor(Request))
            {
                return Content(HttpStatusCode.Unauthorized, "No tienes permiso para actualizar un producto. Solo los roles Admin y Editor pueden hacerlo.");
            }

            try
            {
                var productLogic = new ProductLogic();
                var updatedProduct = productLogic.Update(product);
                if (updatedProduct)
                {
                    return Ok("Producto actualizado exitosamente.");
                }

                return Content(HttpStatusCode.NotFound, "El producto no fue encontrado para actualizar.");
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception($"Error al actualizar el producto: {ex.Message}"));
            }
        }

        // Eliminar un producto (solo Admin y Editor pueden)
        [HttpDelete]
        public IHttpActionResult DeleteProduct(int id)
        {
            if (!_roleValidationService.IsAdmin(Request) && !_roleValidationService.IsEditor(Request))
            {
                return Content(HttpStatusCode.Unauthorized, "No tienes permiso para eliminar un producto. Solo los roles Admin y Editor pueden hacerlo.");
            }

            try
            {
                var productLogic = new ProductLogic();
                var result = productLogic.Delete(id);
                if (result)
                {
                    return Ok("Producto eliminado exitosamente.");
                }
                return Content(HttpStatusCode.NotFound, "El producto no fue encontrado para eliminar o aun tiene stock.");
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception($"Error al eliminar el producto: {ex.Message}"));
            }
        }
    }
}
