using System.Web.Mvc;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace RedisCacheLib.Infrastructure.Windsor
{
    // ReSharper disable once UnusedMember.Global
    public class KenticoInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
			//container.Register(Classes.FromThisAssembly()
			//	.BasedOn<IContentService>() //TODO IKenticoSerivce?
			//	.WithService.DefaultInterfaces()
			//	.LifestyleTransient());

			//container.Register(Classes.FromAssemblyNamed("Kentico.Content.Web.Mvc")
   //             .BasedOn<IController>()
   //             .LifestyleTransient());
        }
    }
}