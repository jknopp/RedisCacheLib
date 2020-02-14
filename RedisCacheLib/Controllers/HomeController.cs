using System.Threading.Tasks;
using System.Web.Mvc;
using RedisCacheLib.Infrastructure;
using RedisCacheLib.Services;

namespace RedisCacheLib.Controllers
{
	public class HomeController : BaseController
	{
		private readonly IUserService _userService;
		public HomeController(IUserService userService)
		{
			_userService = userService;
		}

		public async Task<ActionResult> Index()
		{
			await _userService.GetUserWithProfile();
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