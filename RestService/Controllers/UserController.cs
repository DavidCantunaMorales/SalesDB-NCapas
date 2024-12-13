using BLL;
using Entities;
using SecurityLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace RestService.Controllers
{
    public class UserController : ApiController
    {
        private readonly RoleValidationService _roleValidationService;

        // Constructor
        public UserController()
        {
            _roleValidationService = new RoleValidationService();  // Instancia del servicio de validación de roles
        }

        // Método para crear un nuevo usuario (solo Admin puede)
        [HttpPost]
        public IHttpActionResult CreateUser(Usuarios newUser)
        {
            if (!_roleValidationService.IsAdmin(Request))
            {
                return Content(HttpStatusCode.Unauthorized, "No tienes permiso para crear un usuario. Solo el rol Admin puede hacerlo.");
            }

            try
            {
                var userLogic = new UsuariosLogic();
                var createdUser = userLogic.Create(newUser);
                return Ok(createdUser);  // Retorna el usuario creado con un 200 OK
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception($"Error al crear el usuario: {ex.Message}"));
            }
        }


        [HttpPost]
        [Route("RegisterUser")]
        public IHttpActionResult RegisterUser(Usuarios newUser)
        {
            try
            {
                var userLogic = new UsuariosLogic();
                var createdUser = userLogic.Register(newUser);
                return Ok(createdUser);  // Retorna el usuario creado con un 200 OK
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception($"Error al registrar el usuario: {ex.Message}"));
            }
        }


        // Método para obtener todos los usuarios (solo Admin puede)
        [HttpGet]
        public IHttpActionResult GetAllUsers()
        {
            if (!_roleValidationService.IsAdmin(Request))
            {
                return Content(HttpStatusCode.Unauthorized, "No tienes permiso para obtener todos los usuarios. Solo el rol Admin puede hacerlo.");
            }

            try
            {
                var userLogic = new UsuariosLogic();
                var users = userLogic.RetrieveAllUsers();
                return Ok(users);  // Retorna la lista de usuarios con un 200 OK
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception($"Error al obtener los usuarios: {ex.Message}"));
            }
        }

        // Método para obtener un usuario por ID (solo Admin puede)
        [HttpGet]
        [Route("api/User/GetUser/{userID}")]
        public IHttpActionResult GetUser(int userID)
        {
            if (!_roleValidationService.IsAdmin(Request))
            {
                return Content(HttpStatusCode.Unauthorized, "No tienes permiso para obtener el usuario. Solo el rol Admin puede hacerlo.");
            }

            try
            {
                var userLogic = new UsuariosLogic();
                var user = userLogic.RetrieveByID(userID);
                if (user == null)
                {
                    return Content(HttpStatusCode.NotFound, "El usuario solicitado no fue encontrado.");
                }

                return Ok(user);  // Retorna el usuario encontrado con un 200 OK
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception($"Error al obtener el usuario: {ex.Message}"));
            }
        }

        // Método para actualizar un usuario (solo Admin puede)
        [HttpPost]
        public IHttpActionResult UpdateUser(Usuarios userToUpdate)
        {
            if (!_roleValidationService.IsAdmin(Request))
            {
                return Content(HttpStatusCode.Unauthorized, "No tienes permiso para actualizar un usuario. Solo el rol Admin puede hacerlo.");
            }

            try
            {
                var userLogic = new UsuariosLogic();
                var updatedUser = userLogic.Update(userToUpdate);
                if (updatedUser)
                {
                    return Ok("Usuario actualizado exitosamente.");
                }

                return Content(HttpStatusCode.NotFound, "El usuario no fue encontrado para actualizar.");
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception($"Error al actualizar el usuario: {ex.Message}"));
            }
        }

        // Método para desactivar un usuario (solo Admin puede)
        [HttpDelete]
        [Route("api/User/DesactivateUser/{userID}")]
        public IHttpActionResult DesactivateUser(int userID)
        {
            if (!_roleValidationService.IsAdmin(Request))
            {
                return Content(HttpStatusCode.Unauthorized, "No tienes permiso para desactivar un usuario. Solo el rol Admin puede hacerlo.");
            }

            try
            {
                var userLogic = new UsuariosLogic();
                var user = userLogic.RetrieveByID(userID);

                if (user == null)
                {
                    return Content(HttpStatusCode.NotFound, "El usuario no fue encontrado.");
                }

                // Cambiar el estado de 'IsActive' a 0 (inactivo)
                user.IsActive = false;
                var result = userLogic.Update(user);

                if (result)
                {
                    return Ok("Usuario desactivado exitosamente.");
                }

                return Content(HttpStatusCode.InternalServerError, "Hubo un error al desactivar el usuario.");
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception($"Error al desactivar el usuario: {ex.Message}"));
            }
        }
    }
}
