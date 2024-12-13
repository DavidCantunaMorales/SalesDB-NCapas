using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using NSalesMVCPLS.Models;
using Newtonsoft.Json.Linq;
using NSalesProxyService;
using System.Net.Mail;
using System.Net;
using BLL;
using SecurityLayer;

namespace NSalesMVCPLS.Controllers
{
    public class LoginController : Controller
    {
        private readonly Proxy _proxy;
        private readonly UsuariosLogic _usuariosLogic;
        private const int MaxLoginAttempts = 3; // Máximo de intentos fallidos
        private const int LockoutDuration = 1; // Duración del bloqueo en minutos

        public LoginController()
        {
            _proxy = new Proxy();
            _usuariosLogic = new UsuariosLogic(); // Inyecta la lógica de usuarios
        }

        // Vista para mostrar el formulario de inicio de sesión
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Verifica si el usuario está bloqueado
                if (IsUserLockedOut(model.Username))
                {
                    ViewData["ErrorMessage"] = $"Has superado el número máximo de intentos. Por favor, espera {LockoutDuration} minutos.";
                    // Log de intento fallido por bloqueo
                    Logger.LogMessage($"Intento fallido: Usuario {model.Username} bloqueado debido a intentos fallidos.");

                    return View(model);
                }

                try
                {
                    // Llama a la función de autenticación
                    var response = _proxy.Authenticate(model.Username, model.Password);

                    if (response != null && !string.IsNullOrEmpty(response.Token))
                    {
                        // Si la autenticación es exitosa, resetea los intentos fallidos
                        ResetFailedAttempts(model.Username);

                        // Crear una cookie para almacenar el token
                        var authCookie = new HttpCookie("AuthToken", response.Token)
                        {
                            HttpOnly = true, // Más seguro, evita acceso por scripts
                            Secure = Request.IsSecureConnection, // Solo para HTTPS
                            Expires = DateTime.Now.AddMinutes(20) // Configura la expiración de la cookie
                        };

                        // Agregar la cookie a la respuesta HTTP
                        Response.Cookies.Add(authCookie);

                        // Log de inicio de sesión exitoso
                        Logger.LogMessage($"Inicio de sesión exitoso para el usuario: {model.Username}");


                        // Redirige al Dashboard Principal
                        return RedirectToAction("Dashboard", "Home");
                    }
                    else
                    {
                        var userEmail = _usuariosLogic.GetEmailFromUsername(model.Username);  // Obtener el correo del usuario
                        // Si la autenticación falla, incrementa el contador de intentos fallidos
                        IncrementFailedAttempts(model.Username, userEmail);  // Pasa el correo del usuario aquí

                        // Log de intento fallido
                        Logger.LogMessage($"Intento fallido de inicio de sesión para el usuario: {model.Username}");

                        ViewData["ErrorMessage"] = response?.Message ?? "Error al iniciar sesión. Por favor, verifica tus credenciales.";
                    }
                }
                catch (Exception ex)
                {
                    // Manejar cualquier excepción
                    ViewData["ErrorMessage"] = $"Ocurrió un error: {ex.Message}";

                    // Log del error
                    Logger.LogMessage($"Error al intentar iniciar sesión para el usuario {model.Username}: {ex.Message}");
                }
            }
            return View(model);
        }

        // Acción para cerrar sesión
        public ActionResult Logout()
        {
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }

        // Verifica si el usuario está bloqueado
        private bool IsUserLockedOut(string username)
        {
            var failedAttempts = Session[username + "_FailedAttempts"] as int?;
            var lockoutTime = Session[username + "_LockoutTime"] as DateTime?;

            if (failedAttempts >= MaxLoginAttempts)
            {
                if (lockoutTime.HasValue && lockoutTime.Value.AddMinutes(LockoutDuration) > DateTime.Now)
                {
                    // Usuario bloqueado
                    return true;
                }
                else
                {
                    // Resetea intentos fallidos si el tiempo de bloqueo ha pasado
                    ResetFailedAttempts(username);
                    return false;
                }
            }
            return false;
        }

        // Incrementa el contador de intentos fallidos
        private void IncrementFailedAttempts(string username, string userEmail)
        {
            var failedAttempts = Session[username + "_FailedAttempts"] as int? ?? 0;
            Session[username + "_FailedAttempts"] = failedAttempts + 1;

            // Si alcanza el máximo de intentos, bloquea al usuario por un tiempo
            if (failedAttempts + 1 >= MaxLoginAttempts)
            {
                Session[username + "_LockoutTime"] = DateTime.Now;
                // Enviar alerta por correo cuando se alcance el límite de intentos
                SendAlertEmail(username, userEmail);  // Aquí se pasa el correo del usuario
            }
        }

        // Resetea el contador de intentos fallidos
        private void ResetFailedAttempts(string username)
        {
            Session[username + "_FailedAttempts"] = 0;
            Session[username + "_LockoutTime"] = null;
        }

        private void SendAlertEmail(string username, string userEmail)
        {
            try
            {
                // Configuración del correo
                var fromAddress = new MailAddress("davidcantuna20@gmail.com", "Security"); // Correo de origen
                var toAddress = new MailAddress(userEmail, "Usuario"); // Correo del usuario al cual se le envía la alerta
                const string subject = "Alerta de Intentos Fallidos de Inicio de Sesión"; // Asunto del correo
                string body = $"Se ha detectado {MaxLoginAttempts} intentos fallidos para tu cuenta: {username}. Si no fuiste tú, por favor contacta al soporte."; // Cuerpo del correo

                // Configuración del cliente SMTP (Gmail)
                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com", // Servidor SMTP de Gmail
                    Port = 587, // Puerto de Gmail para TLS
                    EnableSsl = true, // Habilitamos SSL/TLS
                    DeliveryMethod = SmtpDeliveryMethod.Network, // Método de entrega de la red
                    UseDefaultCredentials = false, // No usamos las credenciales predeterminadas
                    Credentials = new NetworkCredential("davidcantuna20@gmail.com", "yvgl ldmd kpsz ukpl") // Credenciales de Gmail (reemplaza con tu contraseña de aplicación)
                };

                // Creamos el mensaje con la configuración de correo
                using (var message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = subject, // Asunto
                    Body = body // Cuerpo del mensaje
                })
                {
                    // Enviamos el correo
                    smtp.Send(message);
                }
            }
            catch (Exception ex)
            {
                // Manejo de errores si el envío falla
                ViewData["ErrorMessage"] = $"No se pudo enviar el correo de alerta: {ex.Message}";
            }
        }
    }
}
