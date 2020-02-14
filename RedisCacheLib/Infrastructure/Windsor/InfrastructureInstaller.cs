using System.Configuration;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using RedisCacheLib.Services;
using ServiceStack.Caching;
using ServiceStack.Redis;

namespace RedisCacheLib.Infrastructure.Windsor
{
    // ReSharper disable once UnusedMember.Global
    public class InfrastructureInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {

			//https://github.com/ServiceStack/ServiceStack.Redis#redismanagerpool
			//https://docs.servicestack.net/ssl-redis-azure#configuring-our-apphost
			container.Register(Component.For<IRedisClientCacheManager>()
				.ImplementedBy<RedisManagerPool>()
				.DependsOn(
					Dependency.OnValue("host", ConfigurationManager.ConnectionStrings["ServiceStackRedisConnectionString"].ConnectionString),
					Dependency.OnValue("config", new RedisPoolConfig { MaxPoolSize = 40 })) //Default MaxPoolSize
				.LifestyleSingleton());

			//https://github.com/castleproject/Windsor/blob/master/docs/registering-components-one-by-one.md#register-existing-instance
			container.Register(Component.For<ICacheClient>()
				.Instance(container.Resolve<IRedisClientCacheManager>().GetCacheClient()));

			container.Register(Component.For<IObjectService>().ImplementedBy<ObjectService>());

			//         container.Register(Component.For<ILogger>()
			//             .ImplementedBy<KenticoLogger>());

			//container.Register(Component.For<IContentService>()
			//	.ImplementedBy<ContentService>());

			//container.Register(Component.For<IDebuggingService>()
			//	.ImplementedBy<DebuggingService>());

			//Redis
			//container.Register(Component.For<IConnectionMultiplexer>()
			//             .ImplementedBy<ConnectionMultiplexer>()
			//             .LifestyleSingleton());

			//         container.Register(Component.For<ISerializer>()
			//             .ImplementedBy<Utf8JsonSerializer>()
			//             .LifestyleSingleton());

			//         container.Register(Component.For<IRedisCacheClient>()
			//             .ImplementedBy<RedisCacheClient>()
			//             .LifestyleSingleton());

			//         container.Register(Component.For<IRedisCacheConnectionPoolManager>()
			//             .ImplementedBy<RedisCacheConnectionPoolManager>()
			//             .LifestyleSingleton());

			//         container.Register(Component.For<IRedisDefaultCacheClient>()
			//             .ImplementedBy<RedisDefaultCacheClient>()
			//             .LifestyleSingleton());
        }
    }
}