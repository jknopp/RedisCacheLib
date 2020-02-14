using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Castle.Windsor;

namespace RedisCacheLib
{
	public class MvcApplication : HttpApplication, IContainerAccessor
	{
		private static IWindsorContainer _container;

		// usage: (HttpContext.Current.ApplicationInstance as IContainerAccessor).Container;
		public IWindsorContainer Container => _container;

		protected void Application_Start()
		{
			_container = WindsorConfig.Setup();
			AreaRegistration.RegisterAllAreas();
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
			CacheConfig.Initialize(Container);
		}

		protected void Application_End()
		{
			_container.Dispose();
		}
	}
}
