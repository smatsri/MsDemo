using Infrastructure.Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Prometheus;
using System.Net.Http;

namespace Front
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllersWithViews();
			services.AddConsul(Configuration);
			services.AddHealthChecks()
				.AddCheck<HealthChecks.MainCheck>("main health check");
			services.AddServiceDiscovery();

			services.AddHttpClient();
			services.AddTransient(sp =>
			{
				var factroy = sp.GetService<IHttpClientFactory>();
				var sd = sp.GetService<ServiceDiscovery>();
				var url = sd.GetUrl("HelloService");
				var client = factroy.CreateClient();
				return new HelloServiceClient(url.Http, client);
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime lifetime)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}
			app.UseServiceDiscovery();
			app.UseHttpsRedirection();
			app.UseStaticFiles();
			app.UseConsul(lifetime);
			app.UseRouting();
			app.UseHttpMetrics();

			app.UseAuthorization();



			app.UseEndpoints(endpoints =>
			{
				endpoints.MapHealthChecks("/health");
				endpoints.MapControllerRoute(
					name: "default",
					pattern: "{controller=Home}/{action=Index}/{id?}");
				endpoints.MapMetrics();
			});


		}
	}
}
