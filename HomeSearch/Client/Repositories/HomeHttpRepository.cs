using System;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using HomeSearch.Client.Features;
using HomeSearch.Shared.Models;


namespace HomeSearch.Client.Repositories
{
	public class HomeHttpRepository : IHomeHttpRepository
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options;

        public HomeHttpRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task CreateHome(HomeDetailed homeDetailed)
        {
            var result = await _httpClient.PostAsJsonAsync($"api/home/CreateHome", homeDetailed);
            if (result.StatusCode == System.Net.HttpStatusCode.BadRequest)
                throw new Exception(await result.Content.ReadAsStringAsync());
            result.EnsureSuccessStatusCode();
        }

        public async Task DeleteHome(string id)
        {
            var result = await _httpClient.DeleteAsync($"api/home/delete?id={id}");
            if (result.StatusCode == System.Net.HttpStatusCode.BadRequest)
                throw new Exception(await result.Content.ReadAsStringAsync());
            result.EnsureSuccessStatusCode();
        }

        public async Task<HomeDetailed> GetHome(string id)
        {
            var result = await _httpClient.GetAsync($"api/home/GetHome?id={id}");
            if (!result.IsSuccessStatusCode)
            {
                var content = await result.Content.ReadAsStringAsync();
                throw new ApplicationException(content);
            }
            var home = await result.Content.ReadFromJsonAsync<HomeDetailed>();
            return home;
        }

        public async Task<List<HomeSmall>> GetHomes()
        {
            var result = await _httpClient.GetAsync($"api/home/GetHomes");
            if (!result.IsSuccessStatusCode)
            {
                var content = await result.Content.ReadAsStringAsync();
                throw new ApplicationException(content);
            }
            var homes = await result.Content.ReadFromJsonAsync<List<HomeSmall>>();
            return homes;
        }

        public async Task<PagingResponse<HomeSmall>> GetHomes(HomeParameters homeParameters)
        {
            var queryStringParam = new Dictionary<string, string>
            {
                ["pageNumber"] = homeParameters.PageNumber.ToString(),
                ["searchTerm"] = homeParameters.SearchTerm ?? "",
                ["orderBy"] = homeParameters.OrderBy
            };

            var response = await _httpClient.GetAsync(QueryHelpers.AddQueryString("api/home/GetHomes", queryStringParam));
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ApplicationException(content);
            }

            var pagingResponse = new PagingResponse<HomeSmall>
            {
                Items = JsonSerializer.Deserialize<List<HomeSmall>>(content, _options),
                MetaData = JsonSerializer.Deserialize<MetaData>(response.Headers.GetValues("X-Pagination").First(), _options)
            };
            return pagingResponse;
        }

        public async Task ReserveHome(Reserve reserve)
        {
            var result = await _httpClient.PutAsJsonAsync($"api/home/ReserveHome", reserve);
            if (!result.IsSuccessStatusCode)
            {
                var content = await result.Content.ReadAsStringAsync();
                throw new ApplicationException(content);
            }
            result.EnsureSuccessStatusCode();
        }

        public async Task<List<HomeSmall>> GetUserHomes()
        {
            var result = await _httpClient.GetFromJsonAsync<List<HomeSmall>>("api/home/GetUserHomes");
            return result;
        }

        public async Task<List<Reserve>> GetReserves()
        {
            var result = await _httpClient.GetAsync("api/home/GetUserReserves");
            if (!result.IsSuccessStatusCode)
            {
                var content = await result.Content.ReadAsStringAsync();
                throw new ApplicationException(content);
            }
            var reserves = await result.Content.ReadFromJsonAsync<List<Reserve>>();
            return reserves;
        } 
    }
}

