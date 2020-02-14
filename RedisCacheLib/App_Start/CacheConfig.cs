using System;
using CacheStack;
using Castle.Windsor;
using ServiceStack.Caching;

namespace RedisCacheLib
{
	public class CacheConfig
	{
		public static void Initialize(IWindsorContainer container)
		{
			CacheStackSettings.CacheClient = container.Resolve<ICacheClient>();
			// All of our routes are unique and not shared, so we can use the route name instead of reflection to get a unique cache key
			CacheStackSettings.UseRouteNameForCacheKey = true;
			CacheStackSettings.CacheProfileDurations = profile => TimeSpan.FromMinutes(15); // Same as default.

			// Share same objects between different cache keys
			//CacheStackSettings.CacheKeysForObject.Add(typeof(User), item => {
			//	var userItem = item as User;
			//	var keys = new List<string>
			//	{
			//		CacheKeys.Users.ById(userItem.Id),
			//		CacheKeys.Users.ByUsername(userItem.Username)
			//	};
			//	return keys;
			//});

			//CacheStackSettings.CacheProfileDurations = profile => {
			//	// Can get these values from a db, web.config, or anywhere else
			//	switch ((CacheProfile)profile)
			//	{
			//		case CacheProfile.Profile1:
			//			return TimeSpan.FromSeconds(1);
			//		case CacheProfile.Profile2:
			//			return TimeSpan.FromMinutes(60);
			//		default:
			//			return TimeSpan.FromMinutes(15);
			//	}
			//};


	}
		// Somewhere else in your solution
		//public enum CacheProfile
		//{
		//	Profile1,
		//	Profile2,
		//}
	}
}
