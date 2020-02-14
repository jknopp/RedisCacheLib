using System.Dynamic;
using System.Threading.Tasks;
using CacheStack;
using StackExchange.Redis.Extensions.Core.Abstractions;
using RedisCacheLib.Infrastructure;
using RedisCacheLib.Models;

namespace RedisCacheLib.Services
{
	public interface IUserProfileService : IServiceBase<UserProfile>
	{
		Task<UserProfile> GetByProfileId(int profileId);
		}

	public class UserProfileService : ServiceBase<UserProfile>, IUserProfileService
	{
		public UserProfileService(IRedisDefaultCacheClient cache) : base(cache)
		{
		}

		protected override string GetIdCacheKey(object id)
		{
			return CacheKeys.UserProfiles.ById((int)id);
		}

		public async Task<UserProfile> GetByProfileId(int profileId)
		{
			return await Cache.GetOrCacheAsync(GetIdCacheKey(profileId), context =>
			{
				//var item = Db.SingleOrDefault<User>(new { username });
				var item = new UserProfile { Id = 4, ProfileName = "SomeProfile" };
				if (item != null)
				{
					context.InvalidateOn(TriggerFrom.Id<UserProfile>(item.Id));
				}
				return item;
			});
		}
	}
}