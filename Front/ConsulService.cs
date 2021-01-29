using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Front
{
	public class ServiceUrl
	{
		public ServiceUrl(string http, string https)
		{
			Http = http;
			Https = https;
		}

		public string Http { get; }
		public string Https { get; }
	}
	public class ServiceIndex
	{
		private readonly ConcurrentDictionary<string, int> indexes;
		private readonly int size;

		public ServiceIndex(int size = 10)
		{
			indexes = new ConcurrentDictionary<string, int>();
			this.size = size;
		}

		public int Get(string serviceName)
		{
			if (indexes.TryGetValue(serviceName, out int index))
				return 0;

			indexes.AddOrUpdate(serviceName, index, (key, old) => (old + 1) % size);
			return index;

		}
	}

	public class ConsulService
	{
		private readonly IConsulClient client;
		private ConcurrentDictionary<string, ServiceUrl[]> services;

		public ConsulService(IConsulClient client)
		{
			services = new ConcurrentDictionary<string, ServiceUrl[]>();
			this.client = client;
		}

		public ServiceUrl[] GetUrls(string serviceName)
		{
			if (!services.TryGetValue(serviceName, out ServiceUrl[] urls))
				return Array.Empty<ServiceUrl>();

			return urls;
		}


		public async Task UpdateServices()
		{
			var sers = await client.Agent.Services();

			var kvs = sers.Response
				.GroupBy(a => a.Value.Service)
				.ToDictionary(
					a => a.Key, 
					a => a.Select(a=>ToServiceUrl(a.Value))
						  .ToArray()
				);

			services = new ConcurrentDictionary<string, ServiceUrl[]>(kvs);
		}

		private static ServiceUrl ToServiceUrl(AgentService agent)
		{
			var http = $"http://{agent.Address}:{agent.Port}";
			var httpsPort = agent.Port == 80 ? "": ":80";
			var https = $"https://{agent.Address}{httpsPort}";
			return new ServiceUrl(http, https);
		}
	}

	public class ServiceDiscovery
	{
		private readonly ServiceIndex serviceIndex;
		private readonly ConsulService consulService;
		private readonly Timer timer;

		public ServiceDiscovery(ServiceIndex serviceIndex, ConsulService consulService)
		{
			timer = new Timer(TimerCallback);
			this.serviceIndex = serviceIndex;
			this.consulService = consulService;
		}

		public async Task Start()
		{
			await consulService.UpdateServices();
			timer.Change(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(60));
		}

		public ServiceUrl GetUrl(string serviceName)
		{
			var urls = consulService.GetUrls(serviceName);
			if (urls.Length == 0)
				return null;
			var index = serviceIndex.Get(serviceName);

			return urls[index % urls.Length];

		}

		private void TimerCallback(object state)
		{
			try
			{
				consulService.UpdateServices().Wait();
			}
			catch (Exception)
			{
			}
		}
	}

	public static class ServiceDiscoveryExt
	{
		public static IServiceCollection AddServiceDiscovery(this IServiceCollection services)
		{
			services.AddSingleton<ServiceIndex>();
			services.AddSingleton<ConsulService>();
			services.AddSingleton<ServiceDiscovery>();

			return services;
		}

		public static IApplicationBuilder UseServiceDiscovery(this IApplicationBuilder app)
		{
			var sd = app.ApplicationServices.GetRequiredService<ServiceDiscovery>();
			sd.Start().Wait();
			return app;
		}
	}
}
