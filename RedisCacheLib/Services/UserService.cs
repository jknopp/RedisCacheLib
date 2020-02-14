using System.Threading.Tasks;
using RedisCacheLib.Repositories;

namespace RedisCacheLib.Services
{
	public interface IUserService
	{
		Task GetUserWithProfile();
	}

	public class UserService : IUserService
	{
		private readonly IUserCacheRepository _userRepository;
		private readonly IUserProfileCacheRepository _userProfileRepository;

		public UserService(IUserCacheRepository userRepository, IUserProfileCacheRepository userProfileRepository)
		{
			_userRepository = userRepository;
			_userProfileRepository = userProfileRepository;
		}

		public async Task GetUserWithProfile()
		{
			var test = await _userRepository.GetByNameAsync("SomeName");
			var test3 = await _userProfileRepository.GetByProfileId(4);

			//var test4 = await _userRepository.GetCachedByIdOrDefaultAsync(test.Id);

			var test2 = await _userRepository.GetByUsernameAsync("SomeUserName");
			await _userRepository.SaveWithCacheTriggerAsync(test2);

			await _userProfileRepository.SaveWithCacheTriggerAsync(test3);
		}
	}
}