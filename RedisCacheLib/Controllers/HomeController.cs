using System.Web.Mvc;
using RedisCacheLib.Infrastructure;
using RedisCacheLib.Services;

namespace RedisCacheLib.Controllers
{
	public class HomeController : BaseController
	{
		private IObjectService _objectService;
		public HomeController(IObjectService objectService)
		{
			_objectService = objectService;
		}

		public ActionResult Index()
		{
			_objectService.GetByName("somename");
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