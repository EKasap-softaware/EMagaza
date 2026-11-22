namespace EMagaza.Infrastructure.Persistence.Mongo;

public class MongoDbOptions
{
    public const string SectionName = "Mongo";

    public string ConnectionString { get; set; } = "mongodb://localhost:27018";
    public string Database { get; set; } = "emagaza";
}
