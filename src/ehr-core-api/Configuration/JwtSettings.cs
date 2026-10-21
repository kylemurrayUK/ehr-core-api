namespace EHRCoreAPI.Configuration;

public class JwtSettings
{
    public string Audience {get; set;} = string.Empty;
    public string Issuer {get; set;} = string.Empty;
    public int ExpiryMinutes {get; set;}
    public string Key {get; set;} = string.Empty;
}