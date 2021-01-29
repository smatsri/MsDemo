using Front.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Front.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly HelloServiceClient helloService;

		public HomeController(ILogger<HomeController> logger, HelloServiceClient helloService)
		{
			_logger = logger;
			this.helloService = helloService;
		}

		public async Task<IActionResult> Index()
		{
			var response =  await helloService.HelloAsync("asd");
			return Ok(response);
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
