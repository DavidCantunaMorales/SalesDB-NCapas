using DAL;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    public class Program
    {
        static void Main(string[] args)
        {
            addCategoryAndProducts();
        }

        static void addCategoryAndProducts()
        {
            var categories = new Categories();
            categories.CategoryName = "Electronics";
            categories.Description = "Electronics products";
            
            var product = new Products();
            product.ProductName = "Laptop";
            product.UnitPrice = 1000;
            product.UnitsInStock = 10;

            categories.Products.Add(product);

            using (var repository = RepositoryFactory.CreateRepository())
            {
                repository.Create(categories);
            }

            Console.WriteLine($"Categoria: {categories.CategoryID}, Producto: {product.ProductID}");
            Console.ReadLine();
        }
    }
}
