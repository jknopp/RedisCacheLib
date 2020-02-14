using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Windsor;
using IDependencyResolver = System.Web.Mvc.IDependencyResolver;

namespace RedisCacheLib.Infrastructure.Windsor
{
	public sealed class WindsorDependencyResolver : IDependencyResolver
	{
		private readonly IWindsorContainer _container;

		public WindsorDependencyResolver(IWindsorContainer container)
		{
			_container = container;
		}

		public object GetService(Type serviceType)
		{
			return _container.Kernel.HasComponent(serviceType)
				? _container.Resolve(serviceType)
				: null;
		}

		public IEnumerable<object> GetServices(Type serviceType)
		{
			return _container.Kernel.HasComponent(serviceType)
				? _container.ResolveAll(serviceType).Cast<object>()
				: new object[] { };
		}
	}
}