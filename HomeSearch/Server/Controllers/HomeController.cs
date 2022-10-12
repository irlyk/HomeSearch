using HomeSearch.Server.Models;
using HomeSearch.Server.Models.ModelExtensions;
using HomeSearch.Server.Repositories;
using HomeSearch.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace HomeSearch.Server.Controllers
{

	[ApiController]
	[Route("api/[controller]/[action]")]
	public class HomeController : ControllerBase
	{
		private readonly IHomeRepository _homeManager;
		private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(IHomeRepository homeService, UserManager<ApplicationUser> userManager)
		{
			_homeManager = homeService;
			_userManager = userManager;
		}

        [HttpGet]
        public async Task<IActionResult> GetHome(string id)
        {
            var home = await _homeManager.GetAsync(id);
            return Ok(home?.ToHomeDetailed());
        }

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> DeleteHome(string id)
        {
            var home = await _homeManager.GetAsync(id);
            if (home != null && home.Owner == User.Identity.Name)
            {
                await _homeManager.RemoveAsync(id);
                return Ok();
            }
            else
            {
                return NotFound("Дом не найден");
            }
        }

        [HttpGet]
		public async Task<IActionResult> GetHomes([FromQuery] HomeParameters homeParameters)
        {
			var homes = await _homeManager.GetAsync(homeParameters);
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(homes.MetaData));
            return Ok(homes.Select(home => home.ToHomeSmall()));
        }

		[Authorize]
		[HttpGet]
		public async Task<IActionResult> GetUserHomes()
		{
			var homes = await _homeManager.GetAsync();
			var usersHomes = homes.Where(x => x.Owner == User.Identity.Name);
			return Ok(usersHomes.Select(x => x.ToHomeSmall()));
		}

		[Authorize]
		[HttpPost]
		public async Task<IActionResult> CreateHome([FromBody] HomeDetailed newHomeDetailed)
		{
			var homes = await _homeManager.GetAsync();
			if (homes.Any(home => home.Name == newHomeDetailed.Name))
			{
				return BadRequest("Отель уже существует");
			}
			try
			{
				var home = newHomeDetailed.GetHomeFromHomeDetailed();

				var user = await _userManager.GetUserAsync(User);
				home.Owner = user.UserName;
				home.OwnerId = user.Id.ToString();

                await _homeManager.CreateAsync(home);
				return Ok();
			}
			catch
			{
				return BadRequest("Ошибка при сохранении. Проверьте правильность введенных данных");
			}
		}

		[Authorize]
		[HttpPut]
		public async Task<IActionResult> ReserveHome([FromBody] Shared.Models.Reserve reserve)
		{
			try
			{
                var home = await _homeManager.GetAsync(reserve.HomeId);

				if (home == null)
					return BadRequest("Отель не найден");

				var userId = _userManager.GetUserId(User);
				var serverReserve = new Server.Models.Reserve
				{
					UserId = userId,
					DateStart = reserve.DateStart,
					DateEnd = reserve.DateEnd
				};

				if (home.IsReserved(serverReserve))
					return BadRequest("Уже забронирован");

                if (home.Reserves == null)
                    home.Reserves = new List<Server.Models.Reserve>();

				home.Reserves.Add(serverReserve);
				await _homeManager.UpdateAsync(home.Id, home);

				return Ok();
            }
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return BadRequest();
			}
        }

		[HttpGet]
		[Authorize]
		public async Task<IActionResult> GetUserReserves()
		{
			var userId = _userManager.GetUserId(User);
			var filter = HomeExtension.GetHomesByUserIdInReservesFilter(userId);
			var homes = await _homeManager.GetAsync(filter);
			var userReserves = homes.SelectMany(x =>
				x.Reserves.Where(r => r.UserId == userId).Select(c =>
				new Shared.Models.Reserve
				{
					HomeId = x.Id,
					HotelName = x.Name,
					DateStart = c.DateStart,
					DateEnd = c.DateEnd
				}));
            return Ok(userReserves);
		}
	}
}

