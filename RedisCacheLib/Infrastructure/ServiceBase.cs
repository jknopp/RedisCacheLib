using System;
using System.Threading.Tasks;
using CacheStack;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace RedisCacheLib.Infrastructure
{
	public interface IServiceBase<T> where T : class
	{
		Task<T> SaveAsync(T item);
		//Task<T> GetByIdOrDefaultAsync(object id);
		Task SoftDeleteAsync(T item);
		Task SoftUndeleteAsync(T item);
	}

	public abstract class ServiceBase<T> : IServiceBase<T> where T : class
	{
		protected IRedisDefaultCacheClient Cache { get; set; }

		protected ServiceBase(IRedisDefaultCacheClient cache)
		{
			Cache = cache;
		}

		protected abstract string GetIdCacheKey(object id);
		protected object GetId(T item)
		{
			var type = item.GetType();
			var idProperty = type.GetProperty("Id");
			if (idProperty == null)
				throw new Exception("Object does not have Id property. Type: " + type);

			return idProperty.GetValue(item);
		}

		public virtual async Task<T> SaveAsync(T item)
		{
			//var savedItem = Db.Save(item);

			var id = GetId(item);

			await Cache.Trigger(TriggerFor.Id<T>(id));

			//return savedItem;
			return item;
		}

		//public virtual async Task<T> GetByIdOrDefaultAsync(object id)
		//{
		//	return await Cache.GetOrCacheAsync(GetIdCacheKey(id), context =>
		//	{
		//		var item = Db.GetByIdOrDefault<T>(id);
		//		if (item != null)
		//			context.InvalidateOn(TriggerFrom.Id<T>(id));
		//		return item;
		//	});
		//}

		public virtual async Task SoftDeleteAsync(T item)
		{
			var id = GetId(item);

			//Db.Execute("update [{0}] set IsDeleted=1 where Id=@Id".Fmt(
			//	Db.GetTableName<T>()
			//), new
			//{
			//	id
			//});

			await Cache.Trigger(TriggerFor.Id<T>(id));
		}

		public virtual async Task SoftUndeleteAsync(T item)
		{
			var id = GetId(item);

			//Db.Execute("update [{0}] set IsDeleted=0 where Id=@Id".Fmt(
			//	Db.GetTableName<T>()
			//), new
			//{
			//	id
			//});

			await Cache.Trigger(TriggerFor.Id<T>(id));
		}
	}
}