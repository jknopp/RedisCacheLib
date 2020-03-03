
namespace CacheStack
{
	public interface ICustomCacheKey
	{
		string Prefix { get; }
		string Key { get; }
	}
}
