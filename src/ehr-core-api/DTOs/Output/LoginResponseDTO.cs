namespace EHRCoreAPI.Dtos.Output;

public class LoginResponseDTO
{
    public string Token {get; set;} = string.Empty;
    public DateTime ExpiresAt {get; set;}

}