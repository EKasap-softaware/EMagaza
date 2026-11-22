namespace EMagaza.Application.Interfaces.Services;

public interface IReviewService
{
    Task AddAsync(string userId, string author, string productId, int rating, string comment);
}
