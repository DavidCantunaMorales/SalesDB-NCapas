using DAL;
using Entities;
using SecurityLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class UsuariosLogic
    {

        public Usuarios Authenticate(string username, string password)
        {
            using (var r = RepositoryFactory.CreateRepository())
            {
                // Recuperar el usuario activo por nombre de usuario
                Usuarios user = r.Retrieve<Usuarios>(u => u.UserName == username && u.IsActive);

                if (user == null)
                {
                    throw new UnauthorizedAccessException("Usuario no encontrado o inactivo.");
                }

                // Verificar si la contraseña coincide con el hash
                if (!PasswordHasher.VerifyPassword(password, user.Password))
                {
                    throw new UnauthorizedAccessException("Contraseña incorrecta.");
                }

                return user; // Usuario autenticado correctamente
            }
        }

        // Método para obtener el correo electrónico del usuario desde la base de datos
        public string GetEmailFromUsername(string username)
        {
            // Aquí debes hacer la consulta a la base de datos para obtener el correo del usuario.
            // Esto depende de cómo estás almacenando a los usuarios, por ejemplo con Entity Framework o un repositorio.

            using (var r = RepositoryFactory.CreateRepository())
            {
                // Recuperar el usuario activo con el nombre de usuario proporcionado
                Usuarios user = r.Retrieve<Usuarios>(u => u.UserName == username && u.IsActive);

                // Verificar si el usuario fue encontrado y devolver su correo electrónico
                if (user != null && !string.IsNullOrEmpty(user.Email))
                {
                    return user.Email; // Si el usuario existe y tiene un correo electrónico, retorna el correo
                }
                // Si no se encuentra el usuario o no tiene correo, retorna null
                return null;
            }
        }

        public Usuarios Create(Usuarios newUser)
        {
            Usuarios Result = null;

            using (var r = RepositoryFactory.CreateRepository())
            {
                // Verificar si ya existe un usuario con el mismo nombre de usuario, contraseña o correo electrónico
                var existingUser = r.Retrieve<Usuarios>(u => u.UserName == newUser.UserName || u.Password == newUser.Password || u.Email == newUser.Email);

                if (existingUser == null)
                {
                    // Hashear la contraseña antes de guardar el usuario
                    newUser.Password = PasswordHasher.HashPassword(newUser.Password);

                    // Crear el usuario
                    Result = r.Create(newUser);

                    // Crear un registro de auditoría
                    CreateAuditRecord(newUser.UserID, "Create", "Usuarios", newUser.UserID);
                }
                else
                {
                    throw new InvalidOperationException("Ya existe un usuario con el mismo nombre de usuario, contraseña o correo electrónico.");
                }
            }

            return Result;
        }


        public Usuarios Register(Usuarios newUser)
        {
            Usuarios Result = null;

            using (var r = RepositoryFactory.CreateRepository())
            {
                // Verificar si ya existe un usuario con el mismo nombre de usuario, contraseña o correo electrónico
                var existingUser = r.Retrieve<Usuarios>(u => u.UserName == newUser.UserName || u.Password == newUser.Password || u.Email == newUser.Email);

                if (existingUser == null)
                {
                    // Establecer el RoleID por defecto si no está especificado
                    if (newUser.RoleID <= 0) // Asumiendo que RoleID no debe ser 0 o negativo
                    {
                        newUser.RoleID = 1; // Rol predeterminado: Usuario Administrador
                    }

                    newUser.IsActive = true; // Activar el usuario por defecto

                    // Hashear la contraseña antes de guardar el usuario
                    newUser.Password = PasswordHasher.HashPassword(newUser.Password);

                    // Crear el usuario
                    Result = r.Create(newUser);

                    // Crear un registro de auditoría
                    CreateAuditRecord(newUser.UserID, "Create", "Usuarios", newUser.UserID);
                }
                else
                {
                    throw new InvalidOperationException("Ya existe un usuario con el mismo nombre de usuario, contraseña o correo electrónico.");
                }
            }

            return Result;
        }


        public Usuarios RetrieveByID(int userID)
        {
            Usuarios result = null;
            using (var r = RepositoryFactory.CreateRepository())
            {
                result = r.Retrieve<Usuarios>(u => u.UserID == userID);
            }
            return result;
        }

        public bool Update(Usuarios userToUpdate)
        {
            using (var r = RepositoryFactory.CreateRepository())
            {
                // Verificar si ya existe otro usuario con el mismo nombre de usuario, contraseña o correo electrónico
                var existingUser = r.Retrieve<Usuarios>(u => (u.UserName == userToUpdate.UserName || u.Password == userToUpdate.Password || u.Email == userToUpdate.Email) && u.UserID != userToUpdate.UserID);
                if (existingUser != null)
                {
                    throw new InvalidOperationException("Ya existe otro usuario con el mismo nombre de usuario, contraseña o correo electrónico.");
                }

                bool result = r.Update(userToUpdate);
                if (result)
                {
                    CreateAuditRecord(userToUpdate.UserID, "Update", "Usuarios", userToUpdate.UserID);
                }
                return result;
            }
        }

        public bool Delete(int userID)
        {
            bool result = false;
            var user = RetrieveByID(userID);
            if (user != null)
            {
                using (var r = RepositoryFactory.CreateRepository())
                {
                    user.IsActive = false;
                    result = r.Update(user);
                    if (result)
                    {
                        CreateAuditRecord(user.UserID, "Delete", "Usuarios", userID);
                    }
                }
            }
            return result;
        }

        public List<Usuarios> RetrieveAllUsers()
        {
            List<Usuarios> result = null;
            using (var r = RepositoryFactory.CreateRepository())
            {
                result = r.RetrieveAll<Usuarios>();
            }
            return result;
        }

        private void CreateAuditRecord(int userID, string action, string tableName, int recordID)
        {
            using (var r = RepositoryFactory.CreateRepository())
            {
                var audit = new Auditoria
                {
                    UserID = userID,
                    Action = action,
                    TableName = tableName,
                    RecordID = recordID,
                    ActionDate = DateTime.Now
                };
                r.Create(audit);
            }
        }
    }
}
