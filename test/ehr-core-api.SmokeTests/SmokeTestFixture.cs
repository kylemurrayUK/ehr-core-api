using EHRCoreAPI.Dtos;
using EHRCoreAPI.Dtos.Output;
using RestSharp;
using RestSharp.Authenticators;

namespace ehr_core_api.SmokeTests;

public class SmokeTestFixture : IAsyncLifetime
{
    // InitializeAsync ensures Client isn't null so a null warning is safe here
    public RestClient Client {get; private set;} = null!;
    public const string BaseUrl = "https://localhost:7192";
    
    // TODO remove hardcoded login credentials.
    public const string ValidEmailForLogin = "adminemail@test.com";
    public const string ValidPasswordForLogin = "Test@678";

    public async Task InitializeAsync()
    {        
        string token = await LoginAndReturnTokenString();
        var authenticator = new JwtAuthenticator(token);
        var options = new RestClientOptions(BaseUrl)
        {
            Authenticator = authenticator
        };
        Client = new RestClient(options);
    }

    private async Task<string> LoginAndReturnTokenString()
    {
        using var client = new RestClient(BaseUrl);
        LoginRequestDTO ValidLoginDetails = new()
        {
            Email = ValidEmailForLogin,
            Password = ValidPasswordForLogin   
        };
        var request = new RestRequest("/api/Auth/Login");
        request.AddJsonBody(ValidLoginDetails);

        var response = await client.ExecutePostAsync<LoginResponseDTO>(request);

        if (!response.IsSuccessful)
        {
            throw new InvalidOperationException($"Smoke test login failed: {response.StatusCode} - {response.ErrorMessage}");
        }
        if(response.Data is null)
        {
            throw new InvalidOperationException("response data is null");
        }

        return response.Data.Token;
    }

    public Task DisposeAsync()
    {
        Client.Dispose();
        return Task.CompletedTask;
    }
}