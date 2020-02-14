using System.Data;
using System.Threading.Tasks;
using CacheStack;
using StackExchange.Redis.Extensions.Core.Abstractions;
using RedisCacheLib.Infrastructure;
using RedisCacheLib.Models;

namespace RedisCacheLib.Repositories
{
	public interface IUserCacheRepository : ICacheRepositoryBase<User>
	{
		Task<User> GetByUsernameAsync(string username);
		Task<User> GetByNameAsync(string name);
	}

	public class UserCacheRepository : CacheRepositoryBase<User>, IUserCacheRepository
	{
		public UserCacheRepository(IDbConnection db, IRedisDefaultCacheClient cache) : base(db, cache)
		{
		}

		public async Task<User> GetByUsernameAsync(string username)
		{
			User DataLoadMethod()
			{
				return new User { Id = 2, Name = "SomeName", UserName = "SomeUserName", UserProfileId = 4 };
			}
			return await Cache.GetOrCacheAsync(CacheKey.For("ByUserName", username), DataLoadMethod);
		}

		public async Task<User> GetByNameAsync(string name)
		{
			async Task<User> DataLoadMethod()
			{
				return await Task.FromResult(new User { Id = 19, Name = "AnotherName", UserName = "AnotherUserName", UserProfileId = 54 });
			}
			return await Cache.GetOrCacheAsync(CacheKey.For("ByName", name), DataLoadMethod, CacheProfile.Light);
		}
	}
}