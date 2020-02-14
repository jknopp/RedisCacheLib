using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace CacheStack
{
	public static class CacheExtensions
	{
		private static readonly ConcurrentDictionary<string, ConcurrentBag<string>> Triggers = new ConcurrentDictionary<string, ConcurrentBag<string>>();

		/// <summary>
		/// Trigger a notification to clear the cache for items that are watching the specified <c>ICacheTrigger</c>
		/// </summary>
		/// <param name="cache">Cache to work with</param>
		/// <param name="triggers">Triggers to clear cache listeners <remarks>Use the <c>TriggerFor</c> helper</remarks></param>
		public static async Task Trigger(this IRedisDefaultCacheClient cache, params ICacheTrigger[] triggers)
		{
			var keys = new ConcurrentBag<string>();
			foreach (var trigger in triggers)
			{
				AddUniqueKey(keys, trigger.CacheKeyForAnyItem);
				AddUniqueKey(keys, trigger.CacheKeyForIndividualItem);
			}
			foreach (var key in keys)
			{
				// Handle the case where users expect an actual cache key with this name to be cleared
				await cache.RemoveAsync(key);
				await ClearTriggerAsync(cache, key);
			}
		}

		private static void AddUniqueKey(ConcurrentBag<string> keys, string key)
		{
			if (string.IsNullOrEmpty(key))
				return;
			if (keys.Any(x => x == key))
				return;
			keys.Add(key);
		}

		private static async Task ClearTriggerAsync(IRedisDefaultCacheClient cache, string key)
		{
			if (string.IsNullOrEmpty(key))
				return;

			ConcurrentBag<string> keys;
			// Do not need the trigger keys any more since they will be re-added as needed
			Triggers.TryRemove(key, out keys);
			if (keys == null)
				return;

			foreach (var cacheKey in keys)
			{
				await cache.RemoveAsync(cacheKey);
			}
		}

		/// <summary>
		/// Retrieves the object by the specified key from the cache.  If the object does not exist in the cache, it will be added.
		/// </summary>
		/// <typeparam name="T">Type of object to return</typeparam>
		/// <param name="cache">Cache to work with</param>
		/// <param name="key">Cache key for the object</param>
		/// <param name="cacheAction">Action to perform if the object does not exist in the cache.</param>
		/// <param name="cacheProfile">Optional. Cache profile to override default if required.</param>
		/// <returns></returns>
		public static async Task<T> GetOrCacheAsync<T>(this IRedisDefaultCacheClient cache, string key, Func<ICacheContext, T> cacheAction, object cacheProfile = null) where T : class
		{
			var item = await cache.GetAsync<T>(key);
			var eventArgs = new CacheHitEventArgs
				{
					CacheKey = key,
					Type = typeof(T)
				};
			if (item == null)
			{
				CacheStackSettings.OnCacheMiss(cache, eventArgs);
				var context = new CacheContext(cache);
				if (cacheProfile != null)
				{
					context.UseCacheProfile(cacheProfile);
				}
				item = cacheAction(context);

				// No need to cache null values
				if (item != null)
					await cache.CacheAndSetTriggersAsync(context, key, item);
			}
			else
			{
				CacheStackSettings.OnCacheHit(cache, eventArgs);
			}
			return item;
		}

		/// <summary>
		/// Caches the specified item using the context information
		/// </summary>
		/// <typeparam name="T">Type of object to cache</typeparam>
		/// <param name="cache">Cache to store the object</param>
		/// <param name="context">Context information for how to cache the object</param>
		/// <param name="key">Cache key</param>
		/// <param name="item">Item to cache</param>
		public static async Task CacheAndSetTriggersAsync<T>(this IRedisDefaultCacheClient cache, CacheContext context, string key, T item)
		{
			// Don't cache if there is no context
			if (context == null)
				return;

			// Don't cache if there are no profile durations configured
			if (CacheStackSettings.CacheProfileDurations == null)
				return;

			var expiration = CacheStackSettings.CacheProfileDurations(context.CacheProfile);

			if (expiration != TimeSpan.Zero) //Skip if the timespan is 0
			{
				await cache.AddAsync(key, item, expiration);

				// Rip through all other keys for this object type and add the item under those cache keys too
				var itemType = typeof(T);
				if (CacheStackSettings.CacheKeysForObject != null &&
				    CacheStackSettings.CacheKeysForObject.ContainsKey(itemType))
				{
					var keys = CacheStackSettings.CacheKeysForObject[itemType](item).ToList();
					// Only setup the other cache keys if the current key exists in them. Should prevent some undesirable results if caching partial objects
					if (keys.Any(x => x == key))
					{
						foreach (var k in keys)
						{
							if (k == key)
								continue;
							await cache.AddAsync(k, item, expiration);
							AddKeyToTriggers(context, k);
						}
					}
				}
				AddKeyToTriggers(context, key);
			}
		}

		private static void AddKeyToTriggers(CacheContext context, string key)
		{
			foreach (var watch in context.TriggerWatchers)
			{
				var keys = Triggers.GetOrAdd(watch.Name, new ConcurrentBag<string>());
				AddUniqueKey(keys, key);
			}
		}

		/// <summary>
		/// Retrieves the object by the specified key from the cache.  If the object does not exist in the cache, it will be added.
		/// </summary>
		/// <typeparam name="T">Type of object to return</typeparam>
		/// <param name="cache">Cache to work with</param>
		/// <param name="key">Cache key for the object</param>
		/// <param name="cacheAction">Action to perform if the object does not exist in the cache.</param>
		/// <returns></returns>
		public static async Task<T> GetOrCacheStruct<T>(this IRedisDefaultCacheClient cache, string key, Func<ICacheContext, T> cacheAction) where T : struct
		{
			// Wrap the value type as nullable and check the cache
			var item = await cache.GetAsync<T?>(key);
			if (item == null)
			{
				var context = new CacheContext(cache);

				item = cacheAction(context);
				await cache.CacheAndSetTriggersAsync(context, key, item);
			}
			return item.Value;
		}
		
		/// <summary>
		/// Clear all triggers
		/// </summary>
		/// <param name="cache">Cache to work with</param>
		public static void FlushTriggers(this IRedisDefaultCacheClient cache)
		{
			Triggers.Clear();
		}
	}
}