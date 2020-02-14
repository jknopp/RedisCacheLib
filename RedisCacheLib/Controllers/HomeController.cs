using System.Threading.Tasks;
using System.Web.Mvc;
using RedisCacheLib.Infrastructure;
using RedisCacheLib.Services;

namespace RedisCacheLib.Controllers
{
	public class HomeController : BaseController
	{
		private readonly IUserService _userService;
		private readonly IUserProfileService _userProfileService;
		public HomeController(IUserService userService, IUserProfileService userProfileService)
		{
			_userService = userService;
			_userProfileService = userProfileService;
		}

		public async Task<ActionResult> Index()
		{
			var test = await _userService.GetByNameAsync("SomeName");
			var test3 = await _userProfileService.GetByProfileId(test.UserProfileId);
			//await _userService.SaveAsync(test);


			var test2 = await _userService.GetByUsername("SomeUserName");
			//await _userService.SaveAsync(test2);
			await _userProfileService.SaveAsync(test3);

			return View();
		}

		public ActionResult About()
		{
			ViewBag.Message = "Your application description page.";

			return View();
		}

		public ActionResult Contact()
		{
			ViewBag.Message = "Your contact page.";

			return View();
		}
	}
}