using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace CacheStack
{
	public static class CacheExtensions
	{
		//private static readonly ConcurrentDictionary<IHashKeyTrigger, ConcurrentBag<IHashKeyTrigger>> Triggers = new ConcurrentDictionary<IHashKeyTrigger, ConcurrentBag<IHashKeyTrigger>>();

		/// <summary>
		/// Trigger a notification to clear the cache for items that are watching the specified <c>ICacheTrigger</c>
		/// </summary>
		/// <param name="cache">Cache to work with</param>
		/// <param name="triggers">Triggers to clear cache listeners <remarks>Use the <c>TriggerFor</c> helper</remarks></param>
		public static async Task Trigger(this IRedisDefaultCacheClient cache, params IHashKeyTrigger[] triggers)
		{
			var keys = new ConcurrentBag<IHashKeyTrigger>();
			foreach (var trigger in triggers)
			{
				AddUniqueKey(keys, trigger);
			}
			foreach (var cacheKey in keys)
			{
				// Handle the case where users expect an actual cache key with this name to be cleared
				//await cache.RemoveAsync(key);
				if (string.IsNullOrEmpty(cacheKey.ItemKey))
				{
					var hashes = await cache.SearchKeysAsync(cacheKey.HashKey);
					await cache.RemoveAllAsync(hashes);
				}
				else
				{
					await cache.HashDeleteAsync(cacheKey.HashKey, cacheKey.ItemKey);
				}
			}
		}

		//TODO is this needed?
		private static void AddUniqueKey(ConcurrentBag<IHashKeyTrigger> keys, IHashKeyTrigger trigger)
		{
			if (string.IsNullOrEmpty(trigger.HashKey))
				return;
			if (keys.Any(x => x.HashKey == trigger.HashKey) || keys.Any(x => x.ItemKey == trigger.ItemKey))
				return;
			keys.Add(trigger);
		}

		//TODO Clean these methods up to share code
		/// <summary>
		/// Retrieves the object by the specified key from the cache.  If the object does not exist in the cache, it will be added.
		/// </summary>
		/// <typeparam name="T">Type of object to return</typeparam>
		/// <param name="cache">Cache to work with</param>
		/// <param name="idForCacheKey">Id for the cache key for the object</param>
		/// <param name="cacheAction">Action to perform if the object does not exist in the cache.</param>
		/// <param name="cacheProfile">Optional. Cache profile to override default if required.</param>
		/// <returns></returns>
		public static async Task<T> GetOrCacheAsync<T>(this IRedisDefaultCacheClient cache, int idForCacheKey, Func<Task<T>> cacheAction, object cacheProfile = null) where T : class, ICacheable
		{
			var type = typeof(T);
			var hasCustomCacheKeys = typeof(ICustomCacheable).IsAssignableFrom(type);

			//https://instagram-engineering.com/storing-hundreds-of-millions-of-simple-key-value-pairs-in-redis-1091ae80f74c
			T item;
			string key;
			string field;
			if (hasCustomCacheKeys)
			{
				key = $"{type.FullName}:0"; // Caching an object with Custom Cache Keys so we don't explicitly know the hashset ID, cache in hashset:0 with other custom cache key objects
				field = $"Id:{idForCacheKey}";

				//Will need to do a search of the hashset keys to figure out if the keys contain the key we have
				var kvps = await cache.HashScanAsync<T>(key, $"*{field}*"); //Default page size 10, the larger the hashset the larger this # should be
				var kvp = kvps.SingleOrDefault(); //There can only be 1 key in the hash containing the cache key
				item = kvp.Value;
			}
			else
			{
				key = $"{type.FullName}:{ Math.Round((double)(1000 + idForCacheKey) / 1000) }"; // Caching an object in hashset with ID for scalability
				field = $"Id:{idForCacheKey}";
				item = await cache.HashGetAsync<T>(key, field);
			}

			var eventArgs = new CacheEventArgs //TODO Add hashkey to event
			{
				CacheKey = new HashKey { Key = key, Field = field },
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
				item = await cacheAction();

				// No need to cache null values
				if (item != null)
				{
					if (hasCustomCacheKeys)
					{
						field = $"{field}||{string.Join("||", ((ICustomCacheable)item).CustomCacheKeys)}";
					}
					await cache.CacheAsync(context, key, field, item);
				}
			}
			else
			{
				CacheStackSettings.OnCacheHit(cache, eventArgs);
			}
			return item;
		}

		/// <summary>
		/// Retrieves the object by the specified key from the cache.  If the object does not exist in the cache, it will be added.
		/// </summary>
		/// <typeparam name="T">Type of object to return</typeparam>
		/// <param name="cache">Cache to work with</param>
		/// <param name="idForCacheKey">Id for the cache key for the object</param>
		/// <param name="cacheAction">Action to perform if the object does not exist in the cache.</param>
		/// <param name="cacheProfile">Optional. Cache profile to override default if required.</param>
		/// <returns></returns>
		public static async Task<T> GetOrCacheAsync<T>(this IRedisDefaultCacheClient cache, int idForCacheKey, Func<T> cacheAction, object cacheProfile = null) where T : class, ICacheable
		{
			var type = typeof(T);
			var hasCustomCacheKeys = typeof(ICustomCacheable).IsAssignableFrom(type);

			//https://instagram-engineering.com/storing-hundreds-of-millions-of-simple-key-value-pairs-in-redis-1091ae80f74c
			T item;
			string hashKey;
			string idKey;
			if (hasCustomCacheKeys)
			{
				hashKey = $"{type.FullName}:0"; // Caching an object with Custom Cache Keys so we don't explicitly know the hashset ID, cache in hashset:0 with other custom cache key objects
				idKey = $"Id:{idForCacheKey}";

				//Will need to do a search of the hashset keys to figure out if the keys contain the key we have
				var kvps = await cache.HashScanAsync<T>(hashKey, $"*{idKey}*"); //Default page size 10, the larger the hashset the larger this # should be
				var kvp = kvps.SingleOrDefault(); //There can only be 1 key in the hash containing the cache key
				item = kvp.Value;
			}
			else
			{
				hashKey = $"{type.FullName}:{ Math.Round((double)(1000 + idForCacheKey) / 1000) }"; // Caching an object in hashset with ID for scalability
				idKey = $"Id:{idForCacheKey}";
				item = await cache.HashGetAsync<T>(hashKey, idKey);
			}

			var eventArgs = new CacheEventArgs //TODO Add hash key
			{
				CacheKey = new HashKey { Key = hashKey, Field = idKey },
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
				item = cacheAction();

				// No need to cache null values
				if (item != null)
				{
					if (hasCustomCacheKeys)
					{
						idKey = $"{idKey}||{string.Join("||", ((ICustomCacheable)item).CustomCacheKeys)}";
					}
					await cache.CacheAsync(context, hashKey, idKey, item);
				}
			}
			else
			{
				CacheStackSettings.OnCacheHit(cache, eventArgs);
			}
			return item;
		}

		/// <summary>
		/// Retrieves the object by the specified key from the cache.  If the object does not exist in the cache, it will be added.
		/// </summary>
		/// <typeparam name="T">Type of object to return</typeparam>                   
		/// <param name="cache">Cache to work with</param>
		/// <param name="customCacheKey">Custom cache key for the object</param>
		/// <param name="cacheAction">Action to perform if the object does not exist in the cache.</param>
		/// <param name="cacheProfile">Optional. Cache profile to override default if required.</param>
		/// <returns></returns>
		public static async Task<T> GetOrCacheAsync<T>(this IRedisDefaultCacheClient cache, ICustomCacheKey customCacheKey, Func<Task<T>> cacheAction, object cacheProfile = null) where T : class, ICustomCacheable
		{
			//TODO Check if custom cache key is associated with T
			var type = typeof(T);
			//https://instagram-engineering.com/storing-hundreds-of-millions-of-simple-key-value-pairs-in-redis-1091ae80f74c
			var hashKey = $"{type.FullName}:0"; // Caching an object with Custom Cache Keys so we don't explicitly know the hashset ID, cache in hashset:0 with other custom cache key objects

			//Will need to do a search of the hashset keys to figure out if the keys contain the key we have
			var kvps = await cache.HashScanAsync<T>(hashKey, $"*{customCacheKey}*"); //Default page size 10, the larger the hashset the larger this # should be
			var kvp = kvps.SingleOrDefault(); //There can only be 1 key in the hash containing the cache key
			var item = kvp.Value;


			var eventArgs = new CacheEventArgs //TODO Add hashkey to event
			{
				CacheKey = new HashKey { Key = hashKey, Field = customCacheKey.ToString() },
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
				item = await cacheAction();

				// No need to cache null values
				if (item != null)
				{
					var key = $"Id:{item.IdForCacheKey}||{string.Join("||", item.CustomCacheKeys)}";
					await cache.CacheAsync<T>(context, hashKey, key, item);
				}
			}
			else
			{
				CacheStackSettings.OnCacheHit(cache, eventArgs);
			}
			return item;
		}

		/// <summary>
		/// Retrieves the object by the specified key from the cache.  If the object does not exist in the cache, it will be added.
		/// </summary>
		/// <typeparam name="T">Type of object to return</typeparam>
		/// <param name="cache">Cache to work with</param>
		/// <param name="customCacheKey">Custom cache key for the object</param>
		/// <param name="cacheAction">Action to perform if the object does not exist in the cache.</param>
		/// <param name="cacheProfile">Optional. Cache profile to override default if required.</param>
		/// <returns></returns>
		public static async Task<T> GetOrCacheAsync<T>(this IRedisDefaultCacheClient cache, ICustomCacheKey customCacheKey, Func<T> cacheAction, object cacheProfile = null) where T : class, ICustomCacheable
		{
			var type = typeof(T);
			//https://instagram-engineering.com/storing-hundreds-of-millions-of-simple-key-value-pairs-in-redis-1091ae80f74c
			var hashKey = $"{type.FullName}:0"; // Caching an object with Custom Cache Keys so we don't explicitly know the hashset ID, cache in hashset:0 with other custom cache key objects

			//Will need to do a search of the hashset keys to figure out if the keys contain the key we have
			var kvps = await cache.HashScanAsync<T>(hashKey, $"*{customCacheKey}*"); //Default page size 10, the larger the hashset the larger this # should be
			var kvp = kvps.SingleOrDefault(); //There can only be 1 key in the hash containing the cache key
			var item = kvp.Value;


			var eventArgs = new CacheEventArgs //TODO Add hashkey to event
			{
				CacheKey = new HashKey { Key = hashKey, Field = customCacheKey.ToString() },
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
				item = cacheAction();

				// No need to cache null values
				if (item != null)
				{
					//item.CacheDependencies.ForEach(dependency => context.InvalidateOn(dependency)); //TODO Async? Remove?
					var key = $"Id:{item.IdForCacheKey}||{string.Join("||", item.CustomCacheKeys)}";
					await cache.CacheAsync<T>(context, hashKey, key, item);
				}
			}
			else
			{
				CacheStackSettings.OnCacheHit(cache, eventArgs);
			}
			return item;
		}
		//TODO ^^^^^^^^^^^^^^^

		/// <summary>
		/// Caches the specified item using the context information
		/// </summary>
		/// <param name="cache">Cache to store the object</param>
		/// <param name="context">Context information for how to cache the object</param>
		/// <param name="hashKey">Hash key for object type</param>
		/// <param name="key">Cache key</param>
		/// <param name="item">Item to cache</param>
		public static async Task CacheAsync(this IRedisDefaultCacheClient cache, CacheContext context, string hashKey, string key, ICacheable item)
		{
			var expiration = GetCacheExpiration(context);
			if (!expiration.HasValue) return;

			await cache.HashSetAsync(hashKey, key, item);
		}

		public static async Task CacheAsync<T>(this IRedisDefaultCacheClient cache, CacheContext context, string hashKey, string key, ICustomCacheable item) where T : class, ICustomCacheable
		{
			var expiration = GetCacheExpiration(context);
			if (!expiration.HasValue) return;

			key = AddAdditionalHashCacheKeysForObject<T>(key, item);
			await cache.HashSetAsync(hashKey, key, item);
		}

		private static string AddAdditionalHashCacheKeysForObject<T>(string key, ICustomCacheable item) where T : class, ICustomCacheable
		{
			// Rip through all other keys for this object type and add the item under those cache keys too
			var itemType = typeof(T);
			if (CacheStackSettings.CacheKeysForObject != null &&
				CacheStackSettings.CacheKeysForObject.ContainsKey(itemType))
			{
				var keys = CacheStackSettings.CacheKeysForObject[itemType](item).ToList();
				// Only setup the other cache keys if the current key exists in them. Should prevent some undesirable results if caching partial objects
				//if (keys.Any(x => x.ToString() == key))
				//{
				foreach (var k in keys)
				{
					if (key.Contains($"||{k}"))
						continue;
					key += $"||{k}";	
				}
				//}
			}
			return key;
		}

		private static TimeSpan? GetCacheExpiration(CacheContext context)
		{
			// Don't cache if there is no context or are no profile durations configured
			if (context == null || CacheStackSettings.CacheProfileDurations == null)
			{
				return null;
			}
			var expiration = CacheStackSettings.CacheProfileDurations(context.CacheProfile);
			if (expiration == TimeSpan.Zero)
			{
				return null;
			}
			return expiration;
		}


	}
}