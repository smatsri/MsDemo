using Microsoft.AspNetCore.Mvc;

namespace ConsulDemo.Controllers
{
	[Route("home")]
	public class HomeController : Controller
	{
		public IActionResult Index()
		{
			return Ok("welcome home");
		}
	}
}
