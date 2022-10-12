using System;
using HomeSearch.Shared.Models;
using System.Xml.Linq;
using MongoDB.Driver;

namespace HomeSearch.Server.Models.ModelExtensions
{
	public static class HomeExtension
	{
        public static HomeDetailed ToHomeDetailed(this Home home )
        {
            try
            {
                return new HomeDetailed
                {
                    Id = home.Id,
                    Name = home.Name,
                    Screens = home.Screens,
                    DefaultScreen = home.DefaultScreen,
                    Rating = home.Ratings.CurrentRating,
                    Details = home.Description,
                    Price = home.Price,
                    HomeAddress = new Shared.Models.Address
                    {
                        City = home.HomeAddress.City,
                        Street = home.HomeAddress.Street,
                        Home = home.HomeAddress.Home
                    },
                    Tags = home.Tags,
                    Reserves = home.Reserves != null ? home.Reserves.Select(x => new Shared.Models.Reserve
                    {
                        DateStart = x.DateStart,
                        DateEnd = x.DateEnd
                    }).ToList() : null
                };
            }
            catch
            {
                throw new Exception($"Can't convert Home to HomeDetailed:\n{home}");
            }
        }

        public static HomeSmall ToHomeSmall(this Home home)
        {
            try
            {
                return new HomeSmall
                {
                    Id = home.Id,
                    Name = home.Name,
                    DefaultScreen = home.DefaultScreen,
                    Tags = home.Tags,
                    Price = home.Price
                };
            }
            catch
            {
                throw new Exception($"Can't convert Home to HomeSmall:\n{home}");
            }
        }

        public static Home GetHomeFromHomeDetailed(this HomeDetailed homeDetailed)
        {
            try
            {
                return new Home
                {
                    Name = homeDetailed.Name,
                    Screens = homeDetailed.Screens,
                    DefaultScreen = homeDetailed.DefaultScreen,
                    Description = homeDetailed.Details,
                    Ratings = new Rating { CurrentRating = 0 },
                    Price = homeDetailed.Price,
                    HomeAddress = new Models.Address
                    {
                        City = homeDetailed.HomeAddress.City,
                        Street = homeDetailed.HomeAddress.Street,
                        Home = homeDetailed.HomeAddress.Home
                    },
                    Tags = homeDetailed.Tags,
                    Reserves = homeDetailed.Reserves.Select(x => new Reserve
                    {
                        DateStart = x.DateStart,
                        DateEnd = x.DateEnd
                    }).ToList()
                };
            }
            catch
            {
                throw new Exception($"Can't convert HomeDetailed to Home:\n{homeDetailed}");
            }
        }

        public static bool IsReserved(this Home home, Reserve reserve)
        {
            if (home.Reserves == null)
                return false;

            foreach (var homeReserve in home.Reserves)
            {
                if (reserve.DateStart > homeReserve.DateStart && reserve.DateStart < homeReserve.DateEnd)
                    return true;

                if (reserve.DateEnd > homeReserve.DateStart && reserve.DateEnd < homeReserve.DateEnd)
                    return true;
            }

            return false;
        }

        public static FilterDefinition<Home> GetHomesByUserIdInReservesFilter(string userId)
        {
            var filter = Builders<Home>.Filter.Where(h => h.Reserves.Any(r => r.UserId == userId));
            return filter;
        }
    }
}

