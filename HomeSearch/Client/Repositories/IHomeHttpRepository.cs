using System;
using HomeSearch.Client.Features;
using HomeSearch.Shared.Models;

namespace HomeSearch.Client.Repositories
{
    public interface IHomeHttpRepository
    {
        Task<PagingResponse<HomeSmall>> GetHomes(HomeParameters productParameters);
        Task<List<HomeSmall>> GetHomes();
        Task DeleteHome(string id);
        Task<HomeDetailed> GetHome(string id);
        Task<List<HomeSmall>> GetUserHomes();
        Task CreateHome(HomeDetailed homeDetailed);
        Task ReserveHome(Reserve reserve);
        Task<List<Reserve>> GetReserves();
    }
}

