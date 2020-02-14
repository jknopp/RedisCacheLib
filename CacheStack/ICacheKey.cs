
namespace CacheStack
{
	public interface ICacheKey
	{
		string Prefix { get; }
		string Key { get; }
	}
}
