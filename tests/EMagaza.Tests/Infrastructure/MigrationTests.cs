using EMagaza.Infrastructure.Persistence.Migrations;

namespace EMagaza.Tests.Infrastructure;

public class MigrationTests
{
    private static IReadOnlyList<IMigration> AllMigrations() =>
        typeof(IMigration).Assembly.GetTypes()
            .Where(t => t is { IsAbstract: false, IsInterface: false } && typeof(IMigration).IsAssignableFrom(t))
            .Select(t => (IMigration)Activator.CreateInstance(t)!)
            .ToList();

    [Fact]
    public void Versions_AreUnique() =>
        Assert.Equal(AllMigrations().Count, AllMigrations().Select(m => m.Version).Distinct().Count());

    [Fact]
    public void Versions_StartAtOne_AndHaveNoGaps()
    {
        var versions = AllMigrations().Select(m => m.Version).Order().ToList();

        Assert.Equal(Enumerable.Range(1, versions.Count), versions);
    }

    [Fact]
    public void EveryMigration_HasDescription() =>
        Assert.All(AllMigrations(), m => Assert.False(string.IsNullOrWhiteSpace(m.Description)));
}
