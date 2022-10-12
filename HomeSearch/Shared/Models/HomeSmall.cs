using System;
namespace HomeSearch.Shared.Models
{
	public class HomeSmall
	{
		public string? Id { get; set; }

		public ImageFile DefaultScreen { get; set; }

		public string Name { get; set; }

		public List<string> Tags { get; set; }

		public decimal Price { get; set; }
	}
}

