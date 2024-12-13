using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class RepositoryFactory
    {
        // Metodo para crear una instancia de la clase ERFRepository
        // que implementa la interfaz IRepository y recibe como parametro
        // una instancia de la clase Sales_DBEntities
        public static IRepository CreateRepository()
        {
            // Initializa la coneccion a la base de datos (CREO, INVESTIGAR BIEN)
            var context = new Entities.Sales_DBEntities();
            context.Configuration.ProxyCreationEnabled = false;
            return new EFRepository(context);

        }
    }
}
