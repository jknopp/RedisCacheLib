using System.Collections.Generic;
using Newtonsoft.Json;

namespace CacheStack
{
	public interface ICustomCacheable : ICacheable
	{
		[JsonIgnore]
		IReadOnlyCollection<ICacheKey> CustomCacheKeys { get; }
	}
}
