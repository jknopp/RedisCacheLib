using System.Collections.Generic;

namespace CacheStack
{
	public class CustomCacheKey : ValueObject, ICustomCacheKey
	{
		public string Prefix { get; private set; }
		public  string Key { get; private set; }

		private CustomCacheKey()
		{
		}

		public static CustomCacheKey For(string prefix, string key) //TODO should this be 1 parameter that splits the string with logic?
		{
			return new CustomCacheKey {Prefix = prefix, Key = key};
		}

		public static implicit operator string(CustomCacheKey key)
		{
			return key.ToString();
		}

		//public static explicit operator CustomCacheKey(string prefix, string key) //TODO is this needed?
		//{
		//	return For(prefix, key);
		//}

		public override string ToString()
		{
			return $"{Prefix}/{Key}";
		}

		protected override IEnumerable<object> GetAtomicValues()
		{
			yield return Prefix;
			yield return Key;
		}
	}
}
