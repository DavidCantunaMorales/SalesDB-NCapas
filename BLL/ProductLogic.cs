using DAL;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class ProductLogic
    {
        public Products Create(Products newProduct)
        {
            Products result = null;
            using (var r = RepositoryFactory.CreateRepository())
            {
                // Buscar si el nombre de producto existe
                Products res =
                r.Retrieve<Products>(
               p => p.ProductName == newProduct.ProductName);
                if (res == null)
                {
                    // No existe, podemos crearlo
                    result = r.Create(newProduct);
                }
                else
                {
                    // Podríamos aquí lanzar una excepción
                    // para notificar que el producto ya existe.
                    // Podríamos incluso crear una capa de Excepciones
                    // personalizadas y consumirla desde otras
                    // capas.
                }
            }
            return result;
        }

        public Products RetrieveByID(int ID)
        {
            Products result = null;
            using (var r = RepositoryFactory.CreateRepository())
            {
                result = r.Retrieve<Products>(p => p.ProductID == ID);
            }
            return result;
        }


        public bool Update(Products productToUpdate)
        {
            bool result = false;
            using (var r = RepositoryFactory.CreateRepository())
            {
                // Validar que el nombre de producto no exista
                Products temp =
                r.Retrieve<Products>
               (p => p.ProductName == productToUpdate.ProductName
               && p.ProductID != productToUpdate.ProductID);
                if (temp == null)
                {
                    // No existe
                    result = r.Update(productToUpdate);
                }
                else
                {
                    // Podemos implementar alguna lógica para
                    // indicar que no se pudo modificar
                }
            }
            return result;
        }


        public bool Delete(int ID)
        {
            bool result = false;
            // Buscar el producto para ver si tiene existencias
            var product = RetrieveByID(ID);
            if (product != null)
            {
                if (product.UnitsInStock == 0)
                {
                    // Eliminar el producto
                    using (var r = RepositoryFactory.CreateRepository())
                    {
                        result = r.Delete(product);
                    }
                }
                else
                {
                    // Podemos implementar alguna lógica adicional
                    // para indicar que no se pudo eliminar el producto
                }
            }
            else
            {
                // Podemos implementar alguna lógica
                // para indicar que el producto no existe
            }
            return result;
        }


        // Metodo para listar todos los productos segun el nombre
        public List<Products> FilterByCategoryID(int categoryID)
        {
            List<Products> result = null;
            using (var r = RepositoryFactory.CreateRepository())
            {
                result = r.Filter<Products>
                (p => p.CategoryID == categoryID);
            }
            return result;
        }

        // Metodo para listar todos los productos
        public List<Products> RetrieveAllProducts()
        {
            List<Products> result = null;
            using (var r = RepositoryFactory.CreateRepository())
            {
                result = r.RetrieveAll<Products>();
            }
            return result;
        }
    }
}
