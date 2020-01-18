using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreAPI.Contracts.v1;
using CoreAPI.Contracts.v1.Request;
using CoreAPI.Contracts.v1.Response;
using CoreAPI.Domain;
using CoreAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace CoreAPI.Controllers.v1
{
    public class ProductsController : Controller
    {
        private IProductServices _productServices;
        public ProductsController(IProductServices productServices)
        {
            _productServices = productServices;
        }

        [HttpGet(ApiRoutes.Products.GetALL)]
        public async Task<IActionResult> GetAll()
        {
            return  Ok(await _productServices.GetAllPostsAsync());
        }

        [HttpGet(ApiRoutes.Products.Get)]
        public async Task<IActionResult> Get([FromRoute]Guid productID)
        {
            Product product = await _productServices.GetProductByIDAsync(productID);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [HttpPost(ApiRoutes.Products.Create)]
        public async Task<IActionResult> Create([FromBody] CreateProductRequest productRequest)
        {
            var product = new Product { ID = Guid.NewGuid(),ProductName = productRequest.ProductName };
            await _productServices.CreateProductAsync(product);


            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";
            //var locationUrl = $"{baseUrl}/{ApiRoutes.Products.Get}/{product.ID}";
            var locationUrl = baseUrl + "/" + ApiRoutes.Products.Get.Replace("{productID}", product.ID.ToString());

            var productResponse = new CreateProductResponse { ID = product.ID };

            return Created(locationUrl, productResponse);
        }

        [HttpPut(ApiRoutes.Products.Update)]
        public async Task<IActionResult> Update([FromRoute]Guid productID, [FromBody] UpdateProductRequest updateProduct)
        {
            Product product = new Product { ID = productID, ProductName = updateProduct.Name };
            bool success = await _productServices.UpdateProductAsync(product);
            if (success)
            {
                return Ok(product);
            }
            return NotFound();

        }


        [HttpDelete(ApiRoutes.Products.Delete)]
        public async Task<IActionResult> Delete([FromRoute]Guid productID)
        {
            bool success = await _productServices.DeleteProductAsync(productID);
            if (success)
            {
                return NoContent();
            }
            return NotFound();

        }
    }
}