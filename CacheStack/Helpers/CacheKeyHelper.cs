using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CacheStack.Helpers
{
	public static class CacheKeyHelper
	{
		public static IReadOnlyCollection<ICacheKey> SetCustomCacheKeys(params ICacheKey[] keys)
		{
			return new ReadOnlyCollection<ICacheKey>(keys);
		}
	}
}
