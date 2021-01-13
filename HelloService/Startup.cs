using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Microsoft.Extensions.Configuration;
using System.IO;
using Infrastructure.Consul;
using Prometheus;
using HelloService.Services;

namespace ConsulDemo
{
	public class Startup
	{
		private readonly IConfiguration configuration;

		public Startup(IConfiguration configuration)
		{
			this.configuration = configuration;
		}


		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers();
			services.AddConsul(configuration);
			services.AddHealthChecks()
				.AddCheck<HealthChecks.MainCheck>("main health check");

			services.AddGrpc();
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseConsul(lifetime);
			app.UseRouting();
			app.UseHttpMetrics();


			app.Use((ctx, next) =>
			{
				Console.WriteLine($"[{DateTime.Now:dd/MM HH:mm:ss}]{ctx.Request.Path}");
				return next();
			});


			app.UseEndpoints(endpoints =>
			{
				endpoints.MapHealthChecks("/health");
				endpoints.MapControllers();
				endpoints.MapMetrics();
				endpoints.MapGrpcService<GreeterService>();
			});

        }
	}


}
