using System.Collections.Generic;
using CacheStack;
using CacheStack.Helpers;

namespace RedisCacheLib.Models
{
	public class User : ICustomCacheable
	{
		public int Id { get; set; }
		public string UserName { get; set; }
		public string Name { get; set; }
		public int UserProfileId { get; set; }


		public int IdForCacheKey => Id;
		public IEnumerable<IHashKeyTrigger> CacheDependencies =>
			new List<IHashKeyTrigger>
			{
				//TriggerFor.Id<User>(Id),
				//TriggerFor.Id<UserProfile>(UserProfileId)
			};

		public IEnumerable<IHashKeyTrigger> CacheTriggers(params ICacheable[] objects)
		{
			return new List<IHashKeyTrigger>
			{
				TriggerFor.Id(objects[0]),
				TriggerFor.Id(objects[1])
			};
		}

		public IReadOnlyCollection<ICacheKey> CustomCacheKeys =>
			CacheKeyHelper.SetCustomCacheKeys(
				CacheKey.For("ByName", Name),
				CacheKey.For("ByUserName", UserName));
	}
}