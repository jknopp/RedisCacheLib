using System.Net.NetworkInformation;

namespace RedisCacheLib.Infrastructure
{
	public static class CacheKeys
	{
		public static class Users
		{
			public static string ById(int userId)
			{
				return "User/ById/" + userId;
			}

			public static string ByName(string name)
			{
				return "User/ByName/" + name;
			}

			public static string ByUsername(string username)
			{
				return "User/ByUsername/" + username;
			}
		}

		public static class UserProfiles
		{
			public static string ById(int profileId)
			{
				return "UserProfile/ById/" + profileId;
			}
		}
	}
}