namespace RedisCacheLib.Infrastructure
{
	public static class CacheKeys
	{
		public static class Objects
		{
			public static string ById(int objectId)
			{
				return "Object/ById/" + objectId;
			}
			public static string ByName(string name)
			{
				return "Object/ByName/" + name;
			}
		}
	}
}