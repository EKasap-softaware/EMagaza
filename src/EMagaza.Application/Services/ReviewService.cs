using EMagaza.Application.Interfaces.Repositories;
using EMagaza.Application.Interfaces.Services;
using EMagaza.Domain.Entities;
using EMagaza.Domain.Exceptions;

namespace EMagaza.Application.Services;

public class ReviewService(IReviewRepository reviews, IProductRepository products) : IReviewService
{
    public async Task AddAsync(string userId, string author, string productId, int rating, string comment)
    {
        if (await products.GetByIdAsync(productId) is null) throw new NotFoundException("Ürün bulunamadı.");

        await reviews.AddAsync(new Review(productId, userId, author, rating, comment));
        await products.AddRatingAsync(productId, rating);   // kartlardaki ortalama puan için özet güncellenir
    }
}
