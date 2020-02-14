using System.Data;
using System.Threading.Tasks;
using CacheStack;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace RedisCacheLib.Infrastructure
{
	public interface ICacheRepositoryBase<T> where T : class, ICacheable
	{
		Task<T> SaveWithCacheTriggerAsync(T item);
		Task<T> GetCachedByIdOrDefaultAsync(int id);
		Task SoftDeleteWithCacheTriggerAsync(T item);
		Task SoftUndeleteWithCacheTriggerAsync(T item);
	}

	public abstract class CacheRepositoryBase<T> : DataRepositoryBase<T>, ICacheRepositoryBase<T> where T : class, ICacheable
	{
		protected IRedisDefaultCacheClient Cache { get; set; }

		protected CacheRepositoryBase(IDbConnection db, IRedisDefaultCacheClient cache) : base(db)
		{
			Cache = cache;
		}

		public virtual async Task<T> SaveWithCacheTriggerAsync(T item)
		{
			var savedItem = await base.SaveAsync(item);

			//TODO If the item is related/dependentOf another object, clear them also
			await Cache.Trigger(TriggerFor.Id(item));

			return savedItem;
		}

		public virtual async Task<T> GetCachedByIdOrDefaultAsync(int id)
		{
			async Task<T> DataLoadMethod()
			{
				return await GetByIdOrDefaultAsync(id);
			}
			return await Cache.GetOrCacheAsync(id, DataLoadMethod);

		}

		public virtual async Task SoftDeleteWithCacheTriggerAsync(T item)
		{
			await SoftDeleteAsync(item);
			await Cache.Trigger(TriggerFor.Id(item));
		}

		public virtual async Task SoftUndeleteWithCacheTriggerAsync(T item)
		{
			await SoftUndeleteAsync(item);
			await Cache.Trigger(TriggerFor.Id(item));
		}
	}
}