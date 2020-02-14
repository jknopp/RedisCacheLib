using System.Threading.Tasks;
using CacheStack;
using StackExchange.Redis.Extensions.Core.Abstractions;
using RedisCacheLib.Infrastructure;
using RedisCacheLib.Models;

namespace RedisCacheLib.Services
{
	public interface IUserService : IServiceBase<User>
	{
		Task<User> GetByNameAsync(string name);
		Task<User> GetByUsername(string username);
	}

	public class UserService : ServiceBase<User>, IUserService
	{
		public UserService(IRedisDefaultCacheClient cache) : base(cache)
		{
		}

		protected override string GetIdCacheKey(object id)
		{
			return CacheKeys.Users.ById((int)id);
		}

		public async Task<User> GetByUsername(string username)
		{
			User DataLoadMethod(ICacheContext context)
			{
				var item = new User { Id = 2, Name = "SomeName", UserName = "SomeUserName", UserProfileId = 4 };
				if (item != null)
				{
					context.InvalidateOn(TriggerFrom.Id<User>(item.Id));
					context.InvalidateOn(TriggerFrom.Id<UserProfile>(item.UserProfileId));
				}
				return item;
			}
			return await Cache.GetOrCacheAsync(CacheKeys.Users.ByUsername(username), DataLoadMethod);
		}

		public async Task<User> GetByNameAsync(string name)
		{
			User DataLoadMethod(ICacheContext context)
			{
				var item = new User { Id = 2, Name = "SomeName", UserName = "SomeUserName", UserProfileId = 4 };
				if (item != null)
				{
					context.InvalidateOn(TriggerFrom.Id<User>(item.Id));
					context.InvalidateOn(TriggerFrom.Id<UserProfile>(item.UserProfileId));
				}
				return item;
			}
			return await Cache.GetOrCacheAsync(CacheKeys.Users.ByName(name), DataLoadMethod, CacheProfile.Light);
		}

		
	}
}