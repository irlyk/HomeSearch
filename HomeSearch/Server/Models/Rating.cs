using System;
namespace HomeSearch.Server.Models
{
	public class Rating
	{
		public decimal CurrentRating { get; set; }

		public Dictionary<string, int> UserScores { get; set; }
	}
}

