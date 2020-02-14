using System.Web.Mvc;
using Castle.Windsor;
using Castle.Windsor.Installer;
using RedisCacheLib.Infrastructure.Windsor;

namespace RedisCacheLib
{
	public class WindsorConfig
	{
		public static IWindsorContainer Setup()
		{
			IWindsorContainer container = new WindsorContainer()
				.Install(FromAssembly.This());

			ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(container.Kernel));
			DependencyResolver.SetResolver(new WindsorDependencyResolver(container));
			return container;
		}
	}
}
