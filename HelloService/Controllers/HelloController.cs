using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelloService.Controllers
{
	public class HelloController : Controller
	{
		[Route("api/greet/{name}")]
		public IActionResult Index(string name)
		{
			var message = $"hello {name}";
			return Ok(message);
		}
	}
}
