using System.ComponentModel.DataAnnotations;

namespace TaskManager.Api.Options;

// GN-08-R2, IN-02: the React client's address; appsettings.Development.json in development, Cors__ClientUrl in deployment
public class ClientCorsOptions
{
    public const string Section = "Cors";

    [Required, Url]
    public string ClientUrl { get; init; } = string.Empty;
}
