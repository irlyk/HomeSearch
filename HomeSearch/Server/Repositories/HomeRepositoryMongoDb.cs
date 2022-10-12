using MongoDB.Driver;
using MongoDB.Driver.Linq;
using HomeSearch.Server.Models;
using HomeSearch.Server.Models.DataStructures;
using HomeSearch.Server.Settings;
using HomeSearch.Server.Repositories.Extensions;


namespace HomeSearch.Server.Repositories
{
	public class HomeRepositoryMongoDb : IHomeRepository
	{
		private readonly IMongoCollection<Home> _homeCollection;
		private readonly IMongoCollection<Home> _homeArchiveCollection;

		public HomeRepositoryMongoDb(MongoDbConfig config)
		{
			var mongoClient = new MongoClient(config.ConnectionString);

			var mongoDatabase = mongoClient.GetDatabase(config.Name);

			_homeCollection = mongoDatabase.GetCollection<Home>("Homes");
			_homeArchiveCollection = mongoDatabase.GetCollection<Home>("HomesArchive");
		}

		public async Task<Home?> GetAsync(string id) =>
			await _homeCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task<List<Home>> GetAsync()
        {
            return await _homeCollection.Find(_ => true).ToListAsync();
        }

        public async Task<PagedList<Home>> GetAsync(Shared.Models.HomeParameters homeParameters)
        {
			var homes = await _homeCollection.AsQueryable()
				.Search(homeParameters.SearchTerm)
                .Sort(homeParameters.OrderBy)
                .ToListAsync();
			
            return PagedList<Home>
				.ToPagedList(homes, homeParameters.PageNumber, homeParameters.PageSize);
        }

        public async Task<List<Home>> GetAsync(FilterDefinition<Home> filter)
        {
            return await _homeCollection.Find(filter).ToListAsync();
        }

        public async Task CreateAsync(Home newHome)
        {
            await _homeCollection.InsertOneAsync(newHome);
        }    

        public async Task UpdateAsync(string id, Home updatedHome) =>
            await _homeCollection.ReplaceOneAsync(x => x.Id == id, updatedHome);

        public async Task RemoveAsync(string id)
        {
            var home = await _homeCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
            if (home != null)
            {
                await _homeCollection.DeleteOneAsync(x => x.Id == id);
                await _homeArchiveCollection.InsertOneAsync(home);
            }
        }
    }
}

