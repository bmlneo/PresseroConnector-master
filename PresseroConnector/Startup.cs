using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;

namespace PresseroConnector
{
	public class Startup
	{
		public Startup(IHostingEnvironment env)
		{
			var builder = new ConfigurationBuilder()
				.SetBasePath(env.ContentRootPath)
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
				.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
				.AddEnvironmentVariables();
			Configuration = builder.Build();
		}

		public IConfigurationRoot Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddCors();

			var policy = new AuthorizationPolicyBuilder()
				.RequireAuthenticatedUser()
				.Build();

			// Add framework services.
			services.AddMvc(options =>
			{
				options.Filters.Add(new AuthorizeFilter(policy));
			})
			.AddJsonOptions(options =>
			{
				options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
				options.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
				options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
				options.SerializerSettings.ContractResolver = new DefaultContractResolver();
			});

			services.AddDistributedRedisCache(option =>
			{
				option.Configuration = "192.168.1.207";
				option.InstanceName = "epcache";
			});

			services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(options =>
			{
				options.TokenValidationParameters = new TokenValidationParameters()
				{
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("RiOdumFOLX8wv2iy3F01L8qzoqAgMOycauXXIX2DQ5f40ZXY7VylU3RgJpfT0wBdBlpgnzPnzbfZFfifdw59qQ==")),
					ValidAudience = "pressero.epress.com.au",
					ValidateIssuerSigningKey = true,
					ValidateLifetime = true,
					ValidIssuer = "pressero.epress.com.au"
				};
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
		{
			loggerFactory.AddConsole(Configuration.GetSection("Logging"));
			loggerFactory.AddDebug();

			app.UseForwardedHeaders(new ForwardedHeadersOptions
			{
				ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
			});

			app.UseCors(builder =>
				builder
				.AllowAnyHeader()
				.AllowAnyMethod()
                .AllowAnyOrigin()
#if DEBUG
				.WithOrigins("http://localhost:52611"));
#else
				.AllowAnyOrigin());
				//.WithOrigins("https://dashboard.epress.com.au"));
#endif

			app.UseAuthentication();

			app.UseMvc();
		}
	}
}
