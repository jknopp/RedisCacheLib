using System;
using System.Linq;

namespace CacheStack
{
	public class TriggerFor
	{
		/// <summary>
		/// Triggers cache invalidation for the object with the specified id.  Also will trigger for anything listening for .Any&gt;T&lt;()
		/// </summary>
		/// <typeparam name="T">Type of object to invalidate cache for</typeparam>
		/// <param name="item">The object that has changed</param>
		/// <returns></returns>
		public static IHashKeyTrigger Id<T>(T item) where T : ICacheable
		{
			var type = typeof(T);
			var hasCustomCacheKeys = typeof(ICustomCacheable).IsAssignableFrom(type);
			string hashKeyForIndividualItem;
			string idKeyForIndividualItem;
			//string hashKeyForAnyItem = $"{type.FullName}:*";
			if (hasCustomCacheKeys)
			{
				hashKeyForIndividualItem = $"{type.FullName}:0"; // Caching an object with Custom Cache Keys so we don't explicitly know the hashset ID, cache in hashset:0 with other custom cache key objects
				idKeyForIndividualItem = $"Id:{item.IdForCacheKey}||{string.Join("||", ((ICustomCacheable)item).CustomCacheKeys)}";
			}
			else
			{
				hashKeyForIndividualItem = $"{type.FullName}:{ Math.Round((double)(1000 + item.IdForCacheKey) / 1000) }"; // Caching an object in hashset with ID for scalability
				idKeyForIndividualItem = $"Id:{item.IdForCacheKey}";
			}

			if (hasCustomCacheKeys && CacheStackSettings.CacheKeysForObject != null &&
			    CacheStackSettings.CacheKeysForObject.ContainsKey(type))
			{
				var keys = CacheStackSettings.CacheKeysForObject[type]((ICustomCacheable)item).ToList();
				// Only setup the other cache keys if the current key exists in them. Should prevent some undesirable results if caching partial objects
				//if (keys.Any(x => x.ToString() == key))
				//{
				foreach (var k in keys)
				{
					if (idKeyForIndividualItem.Contains($"||{k}"))
						continue;
					idKeyForIndividualItem += $"||{k}";
				}
				//}
			}

			return new HashKeyTrigger
			{
				HashKey = hashKeyForIndividualItem,
				ItemKey = idKeyForIndividualItem
			};
		}

		/// <summary>
		/// Triggers cache invalidation of the specified object type, based on the property and value of the property specified.
		/// </summary>
		/// <typeparam name="T">Type of object to invalidate cache for</typeparam>
		/// <typeparam name="TValue">Property to watch</typeparam>
		/// <param name="property">Property to watch</param>
		/// <param name="value">Value of property that has changed</param>
		/// <returns></returns>
		//public static ICacheRelationTrigger Relation<T, TValue>(Expression<Func<T, TValue>> property, object value)
		//{
		//	//TODO Figure out how this would work
		//	var member = property.Body as MemberExpression ?? ((UnaryExpression)property.Body).Operand as MemberExpression;

		//	if (member == null)
		//		throw new Exception("Unable to get property name. Type: " + typeof(T).FullName + " Property: " + property.Name);

		//	return Relation(typeof(T), member.Member.Name, value);
		//}

		//private static ICacheRelationTrigger Relation(Type type, string referencedColumnName, object value)
		//{
		//	var name = type.FullName + "__" + referencedColumnName;
		//	return new CacheTrigger
		//	{
		//		//CacheKeyForAnyItem = type.FullName + CacheContext.AnySuffix,
		//		//CacheKeyForIndividualItem = name + CacheContext.IdSuffix + value
		//	};
		//}

		/// <summary>
		/// Triggers cache invalidation for a specific cache hashkey key
		/// </summary>
		/// <param name="name">Hashkey to invalidate from cache</param>
		/// <returns></returns>
		//public static ICacheNameTrigger Name<T>(string name) where T : ICacheable
		//{
		//	//TODO Figure out how this would work
		//	//var type = typeof(T);
		//	//var hasCustomCacheKeys = typeof(ICustomCacheKeys).IsAssignableFrom(type);
		//	//string hashKeyForIndividualItem;
		//	//string idKeyForIndividualItem;
		//	//string hashKeyForAnyItem = $"{type.FullName}:*";
		//	//if (hasCustomCacheKeys)
		//	//{
		//	//	hashKeyForIndividualItem = $"{type.FullName}:0"; // Caching an object with Custom Cache Keys so we don't explicitly know the hashset ID, cache in hashset:0 with other custom cache key objects
		//	//	idKeyForIndividualItem = $"*{name}*";
		//	//}
		//	//else
		//	//{
		//	//	hashKeyForIndividualItem = $"{type.FullName}:{ Math.Round((double)(1000 + item.IdForCacheKey) / 1000) }"; // Caching an object in hashset with ID for scalability
		//	//	idKeyForIndividualItem = $"Id:{item.IdForCacheKey}";
		//	//}
		//	return new CacheTrigger
		//			   {
		//				   //CacheKeyForIndividualItem = name
		//			   };
		//}

		/// <summary>
		/// Triggers cache invalidation for a specific cache key
		/// </summary>
		/// <typeparam name="T">Type of object to invalidate all hashkeys caches for</typeparam>
		/// <returns></returns>
		public static IHashKeyTrigger All<T>() where T : ICacheable
		{
			return new HashKeyTrigger
			{
				HashKey = $"{ typeof(T).FullName }:*"
			};
		}
	}
}