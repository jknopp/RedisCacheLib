using System;

namespace CacheStack
{
	public class CacheEventArgs : EventArgs
	{
		public IHashKey CacheKey { get; set; }
		public Type Type { get; set; }
	}
}
