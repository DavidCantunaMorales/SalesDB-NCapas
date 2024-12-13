using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLC
{
    public interface ISecurityService
    {
        // Metodos para Roles
        Roles CreateRole(Roles newRole);
        Roles RetrieveRoleID(int ID);
        bool UpdateRole(Roles roleToUpdate);
        bool DeleteRole(int ID);
        List<Roles> RetrieveAllRoles();

        // Metodos para Usuarios
        Usuarios CreateUser(Usuarios newUser);
        Usuarios RetrieveUserID(int userID);
        bool UpdateUser(Usuarios userToUpdate);
        bool DeleteUser(int userID);
        List<Usuarios> RetrieveAllUsers();
    }
}
