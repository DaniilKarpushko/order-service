namespace ProductService.Options;

public sealed class DatabaseOptions
{
    public string Host { get; set; } = string.Empty;

    public int Port { get; set; }

    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string ConnectionString => $"Host={Host};Port={Port};Username={Username};Password={Password};";
}