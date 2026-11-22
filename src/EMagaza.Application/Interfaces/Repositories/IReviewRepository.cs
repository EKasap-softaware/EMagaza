using EMagaza.Domain.Entities;

namespace EMagaza.Application.Interfaces.Repositories;

public interface IReviewRepository
{
    Task AddAsync(Review review);
    Task<IReadOnlyList<Review>> GetByProductAsync(string productId);
    Task<IReadOnlyList<Review>> GetRecentAsync(int count);
}
