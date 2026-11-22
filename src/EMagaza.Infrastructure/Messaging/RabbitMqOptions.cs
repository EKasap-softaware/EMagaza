namespace EMagaza.Infrastructure.Messaging;

public class RabbitMqOptions
{
    public const string SectionName = "RabbitMq";

    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 5673;
    public int ManagementPort { get; set; } = 15673;
    public string User { get; set; } = "guest";
    public string Password { get; set; } = "guest";
}
