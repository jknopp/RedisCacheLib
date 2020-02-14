using System;

namespace CacheStack
{
	public class CacheEventArgs : EventArgs
	{
		public string CacheKey { get; set; }
		public Type Type { get; set; }
	}
}
