using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLC
{
    public interface IProductService
    {
        List<Products> GetProducts();
        Products GetProductById(int id);
        Products CreateProduct(Products product);
        bool UpdateProduct(Products product);
        bool DeleteProduct(int id);
    }
}
