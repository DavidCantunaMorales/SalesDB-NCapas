using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLC
{
    public interface IService
    {
        // Metodos para Productos
        Products CreateProduct(Products newProduct);
        Products RetrieveProductID(int ID);
        bool UpdateProduct(Products productToUpdate);
        bool DeleteProduct(int ID);
        List<Products> FilterProductsByCategoryID(int ID);
        List<Products> RetrieveAllProducts();


        // Metodos para Categorias
        Categories CreateCategory(Categories newCategory);
        Categories RetrieveCategoryID(int categoryID); 
        bool UpdateCategory(Categories categoryToUpdate);
        bool DeleteCategory(int categoryID);
        List<Categories> RetrieveAllCategories();
    }
}
