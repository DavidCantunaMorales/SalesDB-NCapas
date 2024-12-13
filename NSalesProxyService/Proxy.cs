using Entities;
using Newtonsoft.Json;
using SLC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace NSalesProxyService
{
    public class Proxy
    {
        private readonly string BaseAddress = "http://localhost:59181";

        // Método para hacer una solicitud POST
        public async Task<T> SendPost<T, PostData>(string requestURI, PostData data, string bearerToken)
        {
            T result = default(T);
            using (var client = new HttpClient())
            {
                try
                {
                    // Construir URL absoluto
                    requestURI = BaseAddress + requestURI;

                    // Configurar encabezados de la solicitud
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    if (!string.IsNullOrEmpty(bearerToken))
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
                    }

                    // Serializar el cuerpo de la solicitud
                    var jsonData = JsonConvert.SerializeObject(data);
                    var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                    // Enviar la solicitud POST
                    HttpResponseMessage response = await client.PostAsync(requestURI, content);

                    // Leer y deserializar la respuesta
                    var resultWebAPI = await response.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<T>(resultWebAPI);
                }
                catch (Exception ex)
                {
                    // Manejar la excepción según sea necesario
                }
            }
            return result;
        }


        public async Task<T> SendPost2<T, PostData>(string requestURI, PostData data)
        {
            T Result = default(T);
            using (var Client = new HttpClient())
            {
                try
                {
                    // URL Absoluto
                    requestURI = BaseAddress + requestURI;
                    Client.DefaultRequestHeaders.Accept.Clear();
                    Client.DefaultRequestHeaders.Accept.Add
                     (new MediaTypeWithQualityHeaderValue("application/json"));
                    var JSONData = JsonConvert.SerializeObject(data);
                    HttpResponseMessage Response =
                     await Client.PostAsync(requestURI,
                    new StringContent(JSONData.ToString(),
                     Encoding.UTF8, "application/json"));
                    var ResultWebAPI = await Response.Content.ReadAsStringAsync();
                    Result = JsonConvert.DeserializeObject<T>(ResultWebAPI);
                }
                catch (Exception)
                {
                    // Manejar la excepción
                }
            }
            return Result;
        }


        // Método para hacer una solicitud GET
        public async Task<T> SendGet<T>(string requestURI, string bearerToken)
        {
            T Result = default(T);
            using (var Client = new HttpClient())
            {
                try
                {
                    requestURI = BaseAddress + requestURI; // URL Absoluto 

                    // Agregar el encabezado Authorization con el Bearer token
                    Client.DefaultRequestHeaders.Accept.Clear();
                    Client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));

                    if (!string.IsNullOrEmpty(bearerToken))
                    {
                        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
                    }

                    var ResultJSON = await Client.GetStringAsync(requestURI);
                    Result = JsonConvert.DeserializeObject<T>(ResultJSON);
                }
                catch (Exception ex)
                {
                    // Manejar la excepción  
                }
            }
            return Result;
        }

        // Método para hacer una solicitud PUT
        public async Task<bool> SendPut<T, PutData>(string requestURI, PutData data, string bearerToken)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    requestURI = BaseAddress + requestURI;

                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    if (!string.IsNullOrEmpty(bearerToken))
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
                    }

                    var jsonData = JsonConvert.SerializeObject(data);
                    var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PutAsync(requestURI, content);

                    return response.IsSuccessStatusCode; // Considera éxito si el código de estado es 2xx
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error en SendPut: {ex.Message}");
                }
            }
            return false;
        }


        // Método para hacer una solicitud DELETE
        public async Task<bool> SendDelete(string requestURI, string bearerToken)
        {
            using (var client = new HttpClient())
            {
                try
                {
                    // Construir URL absoluto
                    requestURI = BaseAddress + requestURI;

                    // Configurar encabezados de la solicitud
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    if (!string.IsNullOrEmpty(bearerToken))
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
                    }

                    // Enviar la solicitud DELETE
                    HttpResponseMessage response = await client.DeleteAsync(requestURI);

                    // Retornar true si la solicitud fue exitosa
                    return response.IsSuccessStatusCode;
                }
                catch (Exception ex)
                {
                    // Manejar la excepción según sea necesario
                }
            }
            return false; // Retornar false en caso de error
        }


        // Login Methods
        public LoginResponse Authenticate(string username, string password)
        {
            LoginResponse result = null;
            Task.Run(async () => result = await AuthenticateAsync(username, password)).Wait();
            return result;
        }
        public async Task<LoginResponse> AuthenticateAsync(string username, string password)
        {
            var loginRequest = new
            {
                UserName = username,
                Password = password
            };

            var response = await SendPost2<LoginResponse, object>("/login", loginRequest);
            return response;
        }

        public class LoginResponse
        {
            public string Token { get; set; }
            public int UserID { get; set; }
            public string Email { get; set; }
            public int Role { get; set; }
            public string Message { get; set; }
        }


        ///////////////////// Category Methods   /////////////////////

        // Método para recuperar todas las categorías con autenticación
        public List<Categories> GetAllCategories(string authToken)
        {
            List<Categories> result = null;
            Task.Run(async () => result = await GetAllCategoriesAsync(authToken)).Wait();
            return result;
        }
        public async Task<List<Categories>> GetAllCategoriesAsync(string authToken)
        {
            string requestURI = $"/api/Category/GetAllCategories";
            return await SendGet<List<Categories>>(requestURI, authToken);
        }

        // Método para crear una categoría con autenticación
        public Categories CreateCategory(Categories newCategory, string authToken)
        {
            Categories result = null;
            Task.Run(async () => result = await CreateCategoryAsync(newCategory, authToken)).Wait();
            return result;
        }
        public async Task<Categories> CreateCategoryAsync(Categories newCategory, string authToken)
        {
            return await SendPost<Categories, Categories>("/api/Category/CreateCategory", newCategory, authToken);
        }


        // Método para recuperar una categoría por ID con autenticación
        public Categories RetrieveCategoryById(int id, string authToken)
        {
            Categories result = null;
            Task.Run(async () => result = await RetrieveCategoryByIdAsync(id, authToken)).Wait();
            return result;
        }
        public async Task<Categories> RetrieveCategoryByIdAsync(int id, string authToken)
        {
            return await SendGet<Categories>($"/api/Category/GetCategory/{id}", authToken);
        }


        // Método para actualizar una categoría con autenticación
        public bool UpdateCategory(Categories category, string authToken)
        {
            bool result = false;
            Task.Run(async () => result = await UpdateCategoryAsync(category, authToken)).Wait();
            return result;
        }
        public async Task<bool> UpdateCategoryAsync(Categories category, string authToken)
        {
            return await SendPut<bool, Categories>("/api/Category/UpdateCategory", category, authToken);
        }

        // Método para eliminar una categoría con autenticación
        public bool DeleteCategory(int id, string authToken)
        {
            bool result = false;
            Task.Run(async () => result = await DeleteCategoryAsync(id, authToken)).Wait();
            return result;
        }
        public async Task<bool> DeleteCategoryAsync(int id, string authToken)
        {
            return await SendDelete($"/api/Category/DeleteCategory/{id}", authToken);
        }


        /////////////////////  Product Methods   /////////////////////

        // Método para crear un producto de manera sincrónica
        public Products CreateProduct(Products newProduct, string authToken)
        {
            Products result = null;
            Task.Run(async () => result = await CreateProductAsync(newProduct, authToken)).Wait();
            return result;
        }
        public async Task<Products> CreateProductAsync(Products newProduct, string authToken)
        {
            return await SendPost<Products, Products>("/api/Product/CreateProduct", newProduct, authToken);
        }

        // Método para recuperar un producto por ID de manera sincrónica con autenticación
        public Products RetrieveProductById(int id, string authToken)
        {
            Products result = null;
            Task.Run(async () => result = await RetrieveProductByIdAsync(id, authToken)).Wait();
            return result;
        }
        public async Task<Products> RetrieveProductByIdAsync(int id, string authToken)
        {
            return await SendGet<Products>($"/api/Product/GetProduct/{id}", authToken);
        }

        // Método para actualizar un producto de manera sincrónica con autenticación
        public bool UpdateProduct(Products product, string authToken)
        {
            bool result = false;
            Task.Run(async () => result = await UpdateProductAsync(product, authToken)).Wait();
            return result;
        }

        public async Task<bool> UpdateProductAsync(Products product, string authToken)
        {
            return await SendPut<bool, Products>("/api/Product/UpdateProduct", product, authToken);
        }

        // Método para eliminar un producto de manera sincrónica con autenticación
        public bool DeleteProduct(int id, string authToken)
        {
            bool result = false;
            Task.Run(async () => result = await DeleteProductAsync(id, authToken)).Wait();
            return result;
        }
        public async Task<bool> DeleteProductAsync(int id, string authToken)
        {
            return await SendDelete($"/api/Product/DeleteProduct/{id}", authToken);
        }

        // Método para recuperar todos los productos de manera sincrónica con autenticación
        public List<Products> RetrieveAllProducts(string authToken)
        {
            List<Products> result = null;
            Task.Run(async () => result = await RetrieveAllProductsAsync(authToken)).Wait();
            return result;
        }
        public async Task<List<Products>> RetrieveAllProductsAsync(string authToken)
        {
            return await SendGet<List<Products>>("/api/Product/GetAllProducts", authToken);
        }

        /////////////////////  Users Methods   /////////////////////



        // Método para crear un usuario de manera sincrónica con autenticación
        public Usuarios CreateUser(Usuarios newUser, string authToken)
        {
            Usuarios result = null;
            Task.Run(async () => result = await CreateUserAsync(newUser, authToken)).Wait();
            return result;
        }
        public async Task<Usuarios> CreateUserAsync(Usuarios newUser, string authToken)
        {
            return await SendPost<Usuarios, Usuarios>("/api/User/CreateUser", newUser, authToken);
        }

        // Método para recuperar todos los usuarios de manera sincrónica con autenticación
        public List<Usuarios> RetrieveAllUsers(string authToken)
        {
            List<Usuarios> result = null;
            Task.Run(async () => result = await RetrieveAllUsersAsync(authToken)).Wait();
            return result;
        }
        public async Task<List<Usuarios>> RetrieveAllUsersAsync(string authToken)
        {
            return await SendGet<List<Usuarios>>("/api/User/GetAllUsers", authToken);
        }

        // Método para recuperar un usuario por ID de manera sincrónica con autenticación
        public Usuarios RetrieveUserById(int userId, string authToken)
        {
            Usuarios result = null;
            Task.Run(async () => result = await RetrieveUserByIdAsync(userId, authToken)).Wait();
            return result;
        }
        public async Task<Usuarios> RetrieveUserByIdAsync(int userId, string authToken)
        {
            return await SendGet<Usuarios>($"/api/User/GetUser/{userId}", authToken);
        }

        // Método para actualizar un usuario de manera sincrónica con autenticación
        public bool UpdateUser(Usuarios user, string authToken)
        {
            bool result = false;
            Task.Run(async () => result = await UpdateUserAsync(user, authToken)).Wait();
            return result;
        }
        public async Task<bool> UpdateUserAsync(Usuarios user, string authToken)
        {
            return await SendPut<bool, Usuarios>("/api/User/UpdateUser", user, authToken);
        }

        // Método para desactivar un usuario de manera sincrónica con autenticación
        public bool DesactivateUser(int userId, string authToken)
        {
            bool result = false;
            Task.Run(async () => result = await DesactivateUserAsync(userId, authToken)).Wait();
            return result;
        }
        public async Task<bool> DesactivateUserAsync(int userId, string authToken)
        {
            return await SendDelete($"/api/User/DesactivateUser/{userId}", authToken);
        }
    }
}
