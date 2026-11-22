using EMagaza.Application.Interfaces.Repositories;
using EMagaza.Domain.Entities;

namespace EMagaza.Tests.Fakes;

public class FakeReviewRepository : IReviewRepository
{
    public List<Review> Reviews { get; } = new();

    public Task AddAsync(Review review)
    {
        Reviews.Add(review);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<Review>> GetByProductAsync(string productId) =>
        Task.FromResult<IReadOnlyList<Review>>(Reviews.Where(r => r.ProductId == productId).ToList());

    public Task<IReadOnlyList<Review>> GetRecentAsync(int count) =>
        Task.FromResult<IReadOnlyList<Review>>(Reviews.Take(count).ToList());
}
