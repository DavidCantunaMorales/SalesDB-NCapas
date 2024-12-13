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
    public class CategoryController : ApiController
    {
        private readonly RoleValidationService _roleValidationService;

        // Constructor
        public CategoryController()
        {
            _roleValidationService = new RoleValidationService();  // Instancia del servicio de validación de roles
        }

        // Crear una categoría (solo Admin y Editor pueden)
        [HttpPost]
        public IHttpActionResult CreateCategory(Categories category)
        {
            if (!_roleValidationService.IsAdmin(Request) && !_roleValidationService.IsEditor(Request))
            {
                return Content(HttpStatusCode.Unauthorized, "No tienes permiso para crear una categoría. Solo los roles Admin y Editor pueden hacerlo.");
            }

            try
            {
                var categoryLogic = new CategoryLogic();
                var createdCategory = categoryLogic.Create(category);
                return Ok(createdCategory);  // Retorna la categoría creada con un 200 OK
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception($"Error al crear la categoría: {ex.Message}"));
            }
        }

        // Eliminar una categoría (solo Admin y Editor pueden)
        [HttpDelete]
        public IHttpActionResult DeleteCategory(int id)
        {
            if (!_roleValidationService.IsAdmin(Request) && !_roleValidationService.IsEditor(Request))
            {
                return Content(HttpStatusCode.Unauthorized, "No tienes permiso para eliminar una categoría. Solo los roles Admin y Editor pueden hacerlo.");
            }

            try
            {
                var categoryLogic = new CategoryLogic();
                var result = categoryLogic.Delete(id);
                if (result)
                {
                    return Ok("Categoría eliminada exitosamente.");
                }
                return Content(HttpStatusCode.NotFound, "Categoría no encontrada.");
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception($"Error al eliminar la categoría: {ex.Message}"));
            }
        }

        // Obtener una categoría por ID (todos los roles pueden)
        [HttpGet]
        public IHttpActionResult GetCategory(int id)
        {
            try
            {
                var categoryLogic = new CategoryLogic();
                var category = categoryLogic.RetrieveByID(id);
                if (category == null)
                {
                    return Content(HttpStatusCode.NotFound, "La categoría solicitada no fue encontrada.");
                }
                return Ok(category);  // Retorna la categoría encontrada con un 200 OK
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception($"Error al obtener la categoría: {ex.Message}"));
            }
        }

        // Actualizar una categoría (solo Admin y Editor pueden)
        [HttpPut]
        public IHttpActionResult UpdateCategory(Categories category)
        {
            if (!_roleValidationService.IsAdmin(Request) && !_roleValidationService.IsEditor(Request))
            {
                return Content(HttpStatusCode.Unauthorized, "No tienes permiso para actualizar una categoría. Solo los roles Admin y Editor pueden hacerlo.");
            }

            try
            {
                var categoryLogic = new CategoryLogic();
                var result = categoryLogic.Update(category);
                if (result)
                {
                    return Ok("Categoría actualizada exitosamente.");
                }
                return Content(HttpStatusCode.NotFound, "La categoría no fue encontrada para actualizar.");
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception($"Error al actualizar la categoría: {ex.Message}"));
            }
        }

        // Obtener todas las categorías (todos los roles pueden)
        [HttpGet]
        public IHttpActionResult GetAllCategories()
        {
            try
            {
                var categoryLogic = new CategoryLogic();
                var categories = categoryLogic.RetrieveAllCategories();
                return Ok(categories);  // Retorna la lista de categorías con un 200 OK
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception($"Error al obtener las categorías: {ex.Message}"));
            }
        }
    }
}
