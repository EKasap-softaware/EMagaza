using EMagaza.Application.Interfaces.Services;
using EMagaza.Application.Options;
using EMagaza.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EMagaza.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AuthOptions>(configuration.GetSection(AuthOptions.SectionName));

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserManagementService, UserManagementService>();
        services.AddScoped<ICartService, CartService>();
        services.AddScoped<ICouponService, CouponService>();
        services.AddScoped<IFavoriteService, FavoriteService>();
        services.AddScoped<IAddressService, AddressService>();
        services.AddScoped<IReviewService, ReviewService>();
        services.AddScoped<IProductManagementService, ProductManagementService>();
        services.AddScoped<ICheckoutService, CheckoutService>();
        services.AddScoped<IOrderManagementService, OrderManagementService>();
        services.AddScoped<IDashboardService, DashboardService>();
        return services;
    }
}
