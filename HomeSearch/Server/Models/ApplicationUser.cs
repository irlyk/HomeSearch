using System;
using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace HomeSearch.Server.Models
{
	[CollectionName("Users")]
	public class ApplicationUser : MongoIdentityUser<Guid>
	{
	}
}

