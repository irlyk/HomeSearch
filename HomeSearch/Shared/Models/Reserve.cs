using System;
using System.ComponentModel.DataAnnotations;

namespace HomeSearch.Shared.Models
{
	public class Reserve
	{
		public string? HotelName { get; set; }

		[Required]
		public string HomeId { get; set; }

		[Required]
		public DateTime DateStart { get; set; } = DateTime.Now;

		[Required]
		public DateTime DateEnd { get; set; } = DateTime.Now.AddDays(1);
	}
}

