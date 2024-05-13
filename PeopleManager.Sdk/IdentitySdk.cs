using PeopleManager.Model;
using System.Net.Http.Json;
using PeopleManager.Services.Model.Requests;
using Vives.Security.Model;
using Vives.Services.Model;

namespace PeopleManager.Sdk
{
    public class IdentitySdk
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public IdentitySdk(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<JwtAuthenticationResult> SignIn(UserSignInRequest request)
        {
            var httpClient = _httpClientFactory.CreateClient("PeopleManagerApi");
            var route = "/api/Identity/sign-in";
            var response = await httpClient.PostAsJsonAsync(route, request);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<JwtAuthenticationResult>();

            if (result is null)
            {
                return new JwtAuthenticationResult()
                {
                    Messages = new List<ServiceMessage>
                    {
                        new ServiceMessage { Code = "ApiError", Message = "An API error occurred." }
                    }
                };
            }

            return result;
        }

        public async Task<JwtAuthenticationResult> Register(UserRegisterRequest request)
        {
            var httpClient = _httpClientFactory.CreateClient("PeopleManagerApi");
            var route = "/api/Identity/register";
            var response = await httpClient.PostAsJsonAsync(route, request);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<JwtAuthenticationResult>();

            if (result is null)
            {
                return new JwtAuthenticationResult()
                {
                    Messages = new List<ServiceMessage>
                    {
                        new ServiceMessage { Code = "ApiError", Message = "An API error occurred." }
                    }
                };
            }

            return result;
        }
    }
}
