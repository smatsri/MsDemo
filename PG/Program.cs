using Consul;
using Grpc.Net.Client;
using HelloService.Protos;
using System;
using System.Linq;
using System.Net.Http;

AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
var httpHandler = new HttpClientHandler
{
	// Return `true` to allow certificates that are untrusted/invalid
	ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
};

var channel = GrpcChannel.ForAddress("https://localhost",
	new GrpcChannelOptions { HttpHandler = httpHandler });

var client = new Greeter.GreeterClient(channel);
var reply = await client.SayHelloAsync(new HelloRequest { Name = "GreeterClient" });
Console.WriteLine(reply.Message);

//var client = new ConsulClient(consulConfig =>
//{
//	var address = "http://localhost:8500";
//	consulConfig.Address = new Uri(address);
//});

//var services = await client.Agent.Services();

//var helloAddress = services.Response
//	.Where(a => a.Value.Service == "HelloService")
//	.Select(a => $"https://{a.Value.Address}:{a.Value.Port+1}")
//	.ToArray();


//foreach (var i in Enumerable.Range(1, 100))
//{
//	var addr = helloAddress[i % helloAddress.Length];
//	var helloService = CreateClient(addr);
//	var reply = await helloService.SayHelloAsync(new HelloRequest { Name = "GreeterClient" });
//	Console.WriteLine($"[{addr}]Greeting: {reply.Message}");
//}

//Console.WriteLine("Press any key to exit...");
//Console.ReadKey();



static Greeter.GreeterClient CreateClient(string address)
{
	var httpHandler = new HttpClientHandler
	{
		// Return `true` to allow certificates that are untrusted/invalid
		ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
	};

	var channel = GrpcChannel.ForAddress(address,
		new GrpcChannelOptions { HttpHandler = httpHandler });

	return new Greeter.GreeterClient(channel);
}