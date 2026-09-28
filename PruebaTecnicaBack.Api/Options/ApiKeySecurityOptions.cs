namespace PruebaTecnicaBack.Options;

public class ApiKeySecurityOptions
{
    public const string SectionName = "ApiKey";

    public string HeaderName { get; set; } = "X-API-KEY";
    public string SecretKey { get; set; } = string.Empty;
    
}