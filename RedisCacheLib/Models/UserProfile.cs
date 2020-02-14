using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RedisCacheLib.Models
{
	public class UserProfile
	{
		public int Id { get; set; }
		public string ProfileName { get; set; }
		public string Address { get; set; }
	}
}