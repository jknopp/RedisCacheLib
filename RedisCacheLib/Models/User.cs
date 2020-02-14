using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RedisCacheLib.Models
{
	public class User
	{
		public int Id { get; set; }
		public string UserName { get; set; }
		public string Name { get; set; }
		public int UserProfileId { get; set; }
	}
}