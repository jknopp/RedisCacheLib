using System.Web.Mvc;
using Castle.Windsor;
using StackExchange.Redis.Extensions.Core.Abstractions;

namespace RedisCacheLib.Infrastructure
{
	public abstract class BaseController : Controller
	{
		private IWindsorContainer WindsorContainer { get; }
		//protected ILog Log { get; private set; }
		protected IRedisDefaultCacheClient Cache { get; private set; }

		protected BaseController()
		{
			//Log = LogManager.GetLogger(GetType());
			var container = System.Web.HttpContext.Current.ApplicationInstance as IContainerAccessor;
			WindsorContainer = container?.Container;

			if (WindsorContainer != null)
			{
				Cache = WindsorContainer.Resolve<IRedisDefaultCacheClient>();
			}
		}
	}
}