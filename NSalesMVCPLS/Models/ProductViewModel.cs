using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NSalesMVCPLS.Models
{
    public class ProductViewModel
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string CategoryName { get; set; }
        public decimal UnitPrice { get; set; }
        public int UnitsInStock { get; set; }
    }
}