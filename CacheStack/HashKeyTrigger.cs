namespace CacheStack
{
	public interface IHashKeyTrigger
	{
		string HashKey { get; set; }
		string ItemKey { get; set; }
	}

	public class HashKeyTrigger : IHashKeyTrigger
	{
		public string HashKey { get; set; }
		public string ItemKey { get; set; }
	}
}