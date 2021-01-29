using Microsoft.AspNetCore.Mvc;

namespace HelloService.Controllers
{
	public class HelloController : Controller
	{
		[HttpGet, Route("hello/{name}")]
		//[ProducesResponseType(typeof(string), 200)]
		public GreetResopose Get(string name)
		{
			var message = $"hello {name}";
			return new GreetResopose(message);
		}

		public class GreetResopose
		{
			public GreetResopose(string message)
			{
				Message = message;
			}

			public string Message { get; }
		}
	}
}
