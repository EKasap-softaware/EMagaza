using EMagaza.Domain.Entities;

namespace EMagaza.Application.Models;

public record LowStockItem(Product Product, SizeStock Size);
