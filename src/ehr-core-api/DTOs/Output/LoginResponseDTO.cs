namespace EHRCoreAPI.Dtos.Output;

public class LoginReponse
{
    public string Token {get; set;} = string.Empty;
    public DateTime ExpiresAt {get; set;}

}