using System.Web.Http.Controllers;
using System.Web.Mvc;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace RedisCacheLib.Infrastructure.Windsor
{
    // ReSharper disable once UnusedMember.Global
    public class MvcInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Classes.FromThisAssembly()
                .BasedOn<IController>()
                .LifestyleTransient());

            container.Register(Classes.FromThisAssembly()
                .BasedOn<IHttpController>()
                .LifestyleTransient());

            container.Register(Classes.FromThisAssembly()
                .BasedOn<FilterAttribute>()
                .LifestyleTransient());
        }
    }
}