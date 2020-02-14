using System.Dynamic;
using CacheStack;
using RedisCacheLib.Infrastructure;
using ServiceStack.Caching;

namespace RedisCacheLib.Services
{
	public interface IObjectService : IServiceBase<object>
	{
		object GetByName(string name);
	}

	public class ObjectService : ServiceBase<object>, IObjectService
	{
		public ObjectService(ICacheClient cache) : base(cache)
		{
		}

		protected override string GetIdCacheKey(object id)
		{
			return CacheKeys.Objects.ById((int)id);
		}

		public object GetByName(string name)
		{
			return Cache.GetOrCache(CacheKeys.Objects.ByName(name), context =>
			{
				dynamic item = new ExpandoObject();
				item.Id = 2;
				item.Name = "somename";
				if (item != null)
					context.InvalidateOn(TriggerFrom.Id<object>(item.Id));
				return item;
			});
		}
	}
}