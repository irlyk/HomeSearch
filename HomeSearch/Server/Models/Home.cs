using HomeSearch.Shared.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace HomeSearch.Server.Models
{
	public class Home
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string? Id { get; set; }

		public List<ImageFile> Screens { get; set; }

		public ImageFile DefaultScreen { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

		public Address HomeAddress { get; set; }

		public decimal Price { get; set; }

		public string OwnerId { get; set; }

		public string Owner { get; set; }

		public Rating Ratings { get; set; }

		public List<string> Tags { get; set; }

		public List<HomeSearch.Server.Models.Reserve> Reserves { get; set; }
	}
}

