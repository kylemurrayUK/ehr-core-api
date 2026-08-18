using RestSharp;

namespace ehr_core_api.SmokeTests;

public class SmokeTestFixture
{
    public RestClient Client;

    public SmokeTestFixture()
    {
        Client = new RestClient("https://localhost:7192");
    }
}