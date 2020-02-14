using System.Collections.Generic;
using CacheStack;

namespace RedisCacheLib.Models
{
	public class UserProfile : ICacheable
	{
		public int Id { get; set; }
		public string ProfileName { get; set; }
		public string Address { get; set; }


		public int IdForCacheKey => Id;
		public IEnumerable<IHashKeyTrigger> CacheDependencies =>
			new List<IHashKeyTrigger>
			{
				//CacheDependencyOn.Id<UserProfile>(Id),
			};
	}
}