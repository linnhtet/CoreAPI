using CoreAPI.Data;
using CoreAPI.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreAPI.Services
{
    public class ProductService : IProductServices
    {
        private readonly DataContext _dataContext;
        
        public ProductService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        public async Task<Product> GetProductByIDAsync(Guid productID)
        {
            return await _dataContext.Products.SingleOrDefaultAsync(p=>p.ID==productID);
        }

        public async Task<List<Product>> GetAllPostsAsync()
        {
            return await _dataContext.Products.ToListAsync();
        }

        public async Task<bool> UpdateProductAsync(Product productToUpdate)
        {
             _dataContext.Products.Update(productToUpdate);
            var updated= await _dataContext.SaveChangesAsync();
            return updated>0;
        }

        public async Task<bool> CreateProductAsync(Product product)
        {
            _dataContext.Products.Add(product);
            var created = await _dataContext.SaveChangesAsync();
            return created > 0;
        }

        public async Task<bool> DeleteProductAsync(Guid productID)
        {
            var product = await GetProductByIDAsync(productID);
            if (product == null)
                return false;

            _dataContext.Remove(product);
            var deleted = await _dataContext.SaveChangesAsync();
            return deleted > 0;
        }
    }
}
