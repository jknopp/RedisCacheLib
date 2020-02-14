using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using StackExchange.Redis.Extensions.Core;
using StackExchange.Redis.Extensions.Core.Abstractions;
using StackExchange.Redis.Extensions.Core.Configuration;
using StackExchange.Redis.Extensions.Core.Implementations;
using StackExchange.Redis.Extensions.Newtonsoft;
using RedisCacheLib.Services;

namespace RedisCacheLib.Infrastructure.Windsor
{
    // ReSharper disable once UnusedMember.Global
    public class InfrastructureInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {

			////https://github.com/ServiceStack/ServiceStack.Redis#redismanagerpool
			////https://docs.servicestack.net/ssl-redis-azure#configuring-our-apphost
			//container.Register(Component.For<IRedisClientCacheManager>()
			//	.ImplementedBy<RedisManagerPool>()
			//	.DependsOn(
			//		Dependency.OnValue("host", ConfigurationManager.ConnectionStrings["ServiceStackRedisConnectionString"].ConnectionString),
			//		Dependency.OnValue("config", new RedisPoolConfig { MaxPoolSize = 40 })) //Default MaxPoolSize
			//	.LifestyleSingleton());

			////https://github.com/castleproject/Windsor/blob/master/docs/registering-components-one-by-one.md#register-existing-instance
			//container.Register(Component.For<ICacheClient>()
			//	.Instance(container.Resolve<IRedisClientCacheManager>().GetCacheClient()));


			//https://github.com/imperugo/StackExchange.Redis.Extensions
			//TODO Use MsgPack
			container.Register(Component.For<ISerializer>()
				.ImplementedBy<NewtonsoftSerializer>()
				.LifestyleSingleton());

			var redisConfig =
				new RedisConfiguration //https://github.com/imperugo/StackExchange.Redis.Extensions#how-to-configure-it
				{
					AbortOnConnectFail = true,
					//KeyPrefix = "_my_key_prefix_",
					Hosts = new[]
					{
						new RedisHost {Host = "LBWCACHE.redis.cache.windows.net", Port = 6380}
						//new RedisHost {Host = "192.168.0.11",  Port =6379},
						//new RedisHost {Host = "192.168.0.12",  Port =6379}
					},
					AllowAdmin = true,
					ConnectTimeout = 3000,
					Database = 0,
					Ssl = true,
					Password = "jJHdGy58If24ptQw1XTe2O005TzkCepGWogR4hk2LsA=",
					//ServerEnumerationStrategy = new ServerEnumerationStrategy
					//{
					//	Mode = ServerEnumerationStrategy.ModeOptions.All,
					//	TargetRole = ServerEnumerationStrategy.TargetRoleOptions.Any,
					//	UnreachableServerAction = ServerEnumerationStrategy.UnreachableServerActionOptions.Throw
					//}
				};
			container.Register(Component.For<RedisConfiguration>()
				.Instance(redisConfig)
				.LifestyleSingleton());

			container.Register(Component.For<IRedisCacheConnectionPoolManager>()
				.ImplementedBy<RedisCacheConnectionPoolManager>()
				.LifestyleSingleton());

			container.Register(Component.For<IRedisCacheClient>()
				.ImplementedBy<RedisCacheClient>()
				.LifestyleSingleton());

			container.Register(Component.For<IRedisDefaultCacheClient>()
				.ImplementedBy<RedisDefaultCacheClient>()
				.LifestyleSingleton());





			container.Register(Component.For<IUserService>().ImplementedBy<UserService>());
			container.Register(Component.For<IUserProfileService>().ImplementedBy<UserProfileService>());

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