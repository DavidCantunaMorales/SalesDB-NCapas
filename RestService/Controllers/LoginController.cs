using BLL;
using Entities;
using RestService.Models;
using SecurityLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace RestService.Controllers
{
    public class LoginController : ApiController
    {
        [HttpPost]
        [Route("login")]
        public IHttpActionResult Login([FromBody] LoginRequest loginRequest)
        {
            var BL = new UsuariosLogic();

            try
            {
                // Autenticar al usuario
                var user = BL.Authenticate(loginRequest.UserName, loginRequest.Password);

                // Generar el token JWT
                var token = JwtService.GenerateToken(user.UserName, user.Email, user.RoleID);

                Console.WriteLine("Token generado: " + token);

                return Ok(new
                {
                    Token = token,
                    UserID = user.UserID,
                    Email = user.Email,
                    Role = user.RoleID,
                    Message = "Login exitoso"
                });


            }
            catch (UnauthorizedAccessException ex) // Excepción específica de la BLL
            {
                return Content(HttpStatusCode.Unauthorized, new { Message = ex.Message });
            }
            catch (Exception ex) // Capturar cualquier otro error
            {
                return InternalServerError(ex);
            }
        }
    }
}
