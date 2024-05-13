using PeopleManager.Model;
using PeopleManager.Sdk.Extensions;
using System.Net.Http.Json;
using Vives.Security.Model.Abstractions;

namespace PeopleManager.Sdk
{
    public class PersonSdk
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITokenStore _tokenStore;

        public PersonSdk(IHttpClientFactory httpClientFactory, ITokenStore tokenStore)
        {
            _httpClientFactory = httpClientFactory;
            _tokenStore = tokenStore;
        }
        public async Task<IList<Person>> Find()
        {
            var httpClient = _httpClientFactory.CreateClient("PeopleManagerApi");
            var route = "/api/people";
            var response = await httpClient.GetAsync(route);

            response.EnsureSuccessStatusCode();

            var people = await response.Content.ReadFromJsonAsync<IList<Person>>();

            if (people is null)
            {
                return new List<Person>();
            }

            return people;
        }

        //Get
        public async Task<Person?> Get(int id)
        {
            var httpClient = _httpClientFactory.CreateClient("PeopleManagerApi");
            var bearerToken = _tokenStore.GetToken();
            httpClient.AddAuthorization(bearerToken);

            var route = $"/api/people/{id}";
            var response = await httpClient.GetAsync(route);

            response.EnsureSuccessStatusCode();

            var person = await response.Content.ReadFromJsonAsync<Person>();

            return person;
        }

        //Create
        public async Task<Person?> Create(Person person)
        {
            var httpClient = _httpClientFactory.CreateClient("PeopleManagerApi");
            var bearerToken = _tokenStore.GetToken();
            httpClient.AddAuthorization(bearerToken);

            var route = "/api/people";
            var response = await httpClient.PostAsJsonAsync(route, person);

            response.EnsureSuccessStatusCode();

            var createdPerson = await response.Content.ReadFromJsonAsync<Person>();

            return createdPerson;
        }

        //Update
        public async Task<Person?> Update(int id, Person person)
        {
            var httpClient = _httpClientFactory.CreateClient("PeopleManagerApi");
            var bearerToken = _tokenStore.GetToken();
            httpClient.AddAuthorization(bearerToken);

            var route = $"/api/people/{id}";
            var response = await httpClient.PutAsJsonAsync(route, person);

            response.EnsureSuccessStatusCode();

            var updatedPerson = await response.Content.ReadFromJsonAsync<Person>();

            return updatedPerson;
        }

        //Delete
        public async Task Delete(int id)
        {
            var httpClient = _httpClientFactory.CreateClient("PeopleManagerApi");
            var bearerToken = _tokenStore.GetToken();
            httpClient.AddAuthorization(bearerToken);

            var route = $"/api/people/{id}";
            var response = await httpClient.DeleteAsync(route);

            response.EnsureSuccessStatusCode();
        }
    }
}
