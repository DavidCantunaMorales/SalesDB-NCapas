using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLC
{
    public interface ICategoryService
    {
        List<Categories> GetAllCategories();
        Categories GetCategoryById(int id);
        Categories CreateCategory(Categories categories);
        bool UpdateCategory(Categories categories);
        bool DeleteCategory(int id);
    }
}
