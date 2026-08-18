using RestSharp;

namespace ehr_core_api.SmokeTests;

public class SmokeTestFixture
{
    public RestClient _restClient;

    public SmokeTestFixture()
    {
        _restClient = new RestClient("https://localhost:7192");
    }
}