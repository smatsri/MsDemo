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
using Microsoft.OpenApi.Models;

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
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApplication3", Version = "v1" });
			});
			services.AddControllers();
			services.AddConsul(configuration);
			services.AddHealthChecks()
				.AddCheck<HealthChecks.MainCheck>("main health check");

		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseSwagger();
				app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApplication3 v1"));
			}

			//app.UseHttpsRedirection();

			app.UseRouting();

			//app.UseAuthorization();



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
			});

		}
	}


}
