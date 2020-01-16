using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreAPI.Contracts.v1;
using CoreAPI.Domain;
using Microsoft.AspNetCore.Mvc;

namespace CoreAPI.Controllers.v1
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

        [HttpGet(ApiRoutes.Products.GetALL)]
        public IActionResult GetAll()
        {
            return Ok(_product ); 
        }
    }
}