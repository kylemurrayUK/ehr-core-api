namespace EHRCoreAPI.Models;

public record AuthResult
{
    public LoginOutcome LoginOutcome {get; set;}
    public string? Token {get; set;}
    public DateTime? ExpiresAt {get; set;}
}