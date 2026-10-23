namespace EHRCoreAPI.Models;

public record AuthResult
{
    public LoginOutcome loginOutcome {get; set;}
    public string? Token {get; set;}
    public DateTime? ExpiredAt {get; set;}
}