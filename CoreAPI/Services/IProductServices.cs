using CoreAPI.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreAPI.Services
{
    public interface IProductServices
    {
        Task<List<Product>> GetAllPostsAsync();

        Task<Product> GetProductByIDAsync(Guid productID);

        Task<bool> UpdateProductAsync(Product productToUpdate);
        Task<bool> CreateProductAsync(Product product);
        Task<bool> DeleteProductAsync(Guid productID);
    }
}
