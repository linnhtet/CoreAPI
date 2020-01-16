using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreAPI.Domain;
using Microsoft.AspNetCore.Mvc;

namespace CoreAPI.Controllers
{
    public class ProductsController : Controller
    {
        private List<Product> _product;
        public ProductsController()
        {
            _product = new List<Product>();
            for (int i = 0; i < 5; i++)
            {
                _product.Add(new Product { ID = new Guid().ToString() });
            }
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            return View();
        }
    }
}