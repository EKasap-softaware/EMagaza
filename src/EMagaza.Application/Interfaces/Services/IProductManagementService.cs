using EMagaza.Application.Models;
using EMagaza.Domain.Entities;

namespace EMagaza.Application.Interfaces.Services;

public interface IProductManagementService
{
    Task<Product> CreateAsync(ProductInput input);
    Task<Product> UpdateAsync(string id, ProductInput input);
    Task DeleteAsync(string id);
}
