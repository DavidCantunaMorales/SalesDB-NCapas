using Newtonsoft.Json;
using NSalesMVCPLS.Models;
using NSalesProxyService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace NSalesMVCPLS.Controllers
{
    public class RegisterController : Controller
    {
        // GET: Register
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        // POST: Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Crear el objeto de solicitud
                    var registerRequest = new
                    {
                        UserName = model.Username,
                        Password = model.Password,
                        Email = model.Email,
                    };

                    // Llamar a la API para registrar al usuario
                    using (var httpClient = new HttpClient())
                    {
                        httpClient.BaseAddress = new Uri("http://localhost:59181/api/User/"); // Cambia por tu URL base
                        var jsonContent = JsonConvert.SerializeObject(registerRequest);
                        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                        var response = await httpClient.PostAsync("/RegisterUser", content);

                        if (response.IsSuccessStatusCode)
                        {
                            // Redirige al login si el registro fue exitoso
                            return RedirectToAction("Index", "Login");
                        }
                        else
                        {
                            var errorResponse = await response.Content.ReadAsStringAsync();
                            ViewData["ErrorMessage"] = $"Error en la API: {errorResponse}";
                        }
                    }
                }
                catch (Exception ex)
                {
                    ViewData["ErrorMessage"] = $"Ocurrió un error: {ex.Message}";
                }
            }

            // Si hay errores en el modelo o en la API
            return View(model);
        }
    }
}