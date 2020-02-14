using System;
using CacheStack;
using Castle.Windsor;
using StackExchange.Redis.Extensions.Core.Abstractions;
using RedisCacheLib.Infrastructure;

namespace RedisCacheLib
{
	public class CacheConfig
	{
		public static void Initialize(IWindsorContainer container)
		{
			CacheStackSettings.CacheClient = container.Resolve<IRedisDefaultCacheClient>();
			// All of our routes are unique and not shared, so we can use the route name instead of reflection to get a unique cache key
			CacheStackSettings.UseRouteNameForCacheKey = true;
			CacheStackSettings.CacheProfileDurations = profile =>
			{
				// Can get these values from a db, web.config, or anywhere else
				if (profile != null)
				{
					switch ((CacheProfile)profile)
					{
						case CacheProfile.None:
							return TimeSpan.Zero;
						case CacheProfile.Light:
							return TimeSpan.FromSeconds(5);
						case CacheProfile.Heavy:
							return TimeSpan.FromMinutes(60);
						// ReSharper disable once RedundantCaseLabel
						case CacheProfile.Default:
						default:
							break;
					}
				}
				return TimeSpan.FromMinutes(15);
			};

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
		}
	}
}
