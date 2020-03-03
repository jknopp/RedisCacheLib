using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CacheStack.Helpers
{
	public static class CacheKeyHelper
	{
		public static IReadOnlyCollection<ICustomCacheKey> SetCustomCacheKeys(params ICustomCacheKey[] keys)
		{
			return new ReadOnlyCollection<ICustomCacheKey>(keys);
		}
	}
}
