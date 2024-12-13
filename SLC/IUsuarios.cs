using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLC
{
    public interface IUsuarios
    {

        // Metodos para Usuarios
        Usuarios AuthenticateUser(string username, string password);
        Usuarios CreateUser(Usuarios newUser);
        Usuarios RetrieveUserByID(int userID);
        bool UpdateUser(Usuarios userToUpdate);
        bool DeleteUser(int userID);
        List<Usuarios> RetrieveAllUsers();
    }
}
