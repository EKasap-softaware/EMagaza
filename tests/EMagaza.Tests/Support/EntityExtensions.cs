using EMagaza.Domain.Entities;

namespace EMagaza.Tests.Support;

public static class EntityExtensions
{
    /// <summary>Id'yi normalde veritabanı atar; testlerde elle verebilmek için yansıma kullanılır.</summary>
    public static T WithId<T>(this T entity, string id) where T : Entity
    {
        typeof(Entity).GetProperty(nameof(Entity.Id))!.SetValue(entity, id);
        return entity;
    }
}
