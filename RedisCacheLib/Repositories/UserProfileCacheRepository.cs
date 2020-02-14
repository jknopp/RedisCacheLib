using System.Data;
using System.Threading.Tasks;
using CacheStack;
using StackExchange.Redis.Extensions.Core.Abstractions;
using RedisCacheLib.Infrastructure;
using RedisCacheLib.Models;

namespace RedisCacheLib.Repositories
{
	public interface IUserProfileCacheRepository : ICacheRepositoryBase<UserProfile>
	{
		Task<UserProfile> GetByProfileId(int profileId);
	}

	public class UserProfileCacheRepository : CacheRepositoryBase<UserProfile>, IUserProfileCacheRepository
	{
		public UserProfileCacheRepository(IDbConnection db, IRedisDefaultCacheClient cache) : base(db, cache)
		{
		}

		public async Task<UserProfile> GetByProfileId(int profileId)
		{
			return await Cache.GetOrCacheAsync(profileId, () => new UserProfile { Id = 4, ProfileName = "SomeProfile" });
		}
	}
}