using System.Web.Mvc;
using CacheStack;
using Castle.Windsor;
using ServiceStack.Caching;

namespace RedisCacheLib.Infrastructure
{
	public abstract class BaseController : Controller, IWithCacheContext
	{
		private IWindsorContainer WindsorContainer { get; }
		//protected ILog Log { get; private set; }
		protected ICacheClient Cache { get; private set; }
		/// <summary>
		/// Used to set the cache context for donut cached actions
		/// </summary>
		public ICacheContext CacheContext { get; private set; }

		protected BaseController()
		{
			//Log = LogManager.GetLogger(GetType());
			var container = System.Web.HttpContext.Current.ApplicationInstance as IContainerAccessor;
			WindsorContainer = container?.Container;

			if (WindsorContainer != null)
			{
				Cache = WindsorContainer.Resolve<ICacheClient>();
				CacheContext = new CacheContext(Cache);
			}
		}
	}
}