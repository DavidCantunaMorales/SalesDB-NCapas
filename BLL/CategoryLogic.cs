using DAL;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class CategoryLogic
    {
        public Categories Create(Categories newCategory)
        {
            using (var r = RepositoryFactory.CreateRepository())
            {
                newCategory = r.Create(newCategory);
            }
            return newCategory;
        }

        public Categories RetrieveByID(int categoryID)
        {
            Categories result = null;
            using (var r = RepositoryFactory.CreateRepository())
            {
                result = r.Retrieve<Categories>(c => c.CategoryID == categoryID);
            }
            return result;
        }

        public bool Update(Categories categoryToUpdate)
        {
            bool result = false;
            using (var r = RepositoryFactory.CreateRepository())
            {
                // Verificar si ya existe una categoría con el mismo nombre
                Categories temp = r.Retrieve<Categories>(c => c.CategoryName == categoryToUpdate.CategoryName
                    && c.CategoryID != categoryToUpdate.CategoryID);

                if (temp == null)
                {
                    // No existe una categoría con el mismo nombre, se puede actualizar
                    result = r.Update(categoryToUpdate);
                }
                else
                {
                    // Podrías lanzar una excepción o manejar el caso en el que el nombre de la categoría ya exista
                }
            }
            return result;
        }

        public bool Delete(int categoryID)
        {
            bool result = false;
            // Buscar la categoría por ID para asegurarse de que existe
            var category = RetrieveByID(categoryID);
            if (category != null)
            {
                using (var r = RepositoryFactory.CreateRepository())
                {
                    result = r.Delete(category);
                }
            }
            else
            {
                // Manejar el caso en el que la categoría no existe
            }
            return result;
        }

        // Método para listar todas las categorías
        public List<Categories> RetrieveAllCategories()
        {
            List<Categories> result = null;
            using (var r = RepositoryFactory.CreateRepository())
            {
                result = r.RetrieveAll<Categories>(); // Método que recupera todas las categorías desde el repositorio
            }
            return result;
        }
    }

}
