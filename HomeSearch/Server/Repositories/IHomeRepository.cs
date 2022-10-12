using HomeSearch.Server.Models.DataStructures;
using HomeServer = HomeSearch.Server.Models.Home;
using HomeSearch.Shared.Models;
using HomeSearch.Server.Models;
using MongoDB.Driver;

namespace HomeSearch.Server.Repositories
{
	public interface IHomeRepository
	{
        Task<List<HomeServer>> GetAsync();

        Task<HomeServer> GetAsync(string id);

        Task<PagedList<HomeServer>> GetAsync(HomeParameters productParameters);

        Task CreateAsync(HomeServer newHome);

        Task RemoveAsync(string id);

        Task UpdateAsync(string id, HomeServer updatedHome);

        Task<List<HomeServer>> GetAsync(FilterDefinition<Home> filter);
    }
}

