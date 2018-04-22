using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SafeToNet.Prototype.Core.Interfaces;
using Newtonsoft.Json;

namespace SafeToNet.Prototype.Api
{
    /// <summary>
    /// Class Startup.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        /// <value>The configuration.</value>
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        /// <summary>
        /// Configures the services (dependency injection).
        /// </summary>
        /// <param name="services">The services.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<Core.Configuration.WitAiConfiguration>(Configuration.GetSection("WitAiConfiguration"));
            services.Configure<Core.Configuration.Food2ForkConfiguration>(Configuration.GetSection("Food2ForkConfiguration"));

            services.AddSingleton<INlpClient, ExternalClients.WitAi.WitAiClient>();
            services.AddSingleton<IRecipeAggregatorClient, ExternalClients.Food2Fork.Food2ForkClient>();

            services.AddSingleton<ISearchBusiness, Business.SearchBusiness>();

            services.AddCors();

            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });

            services.AddSwaggerGen(cfg =>
            {
                cfg.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info
                {
                    Title = "SafeToNet Prototype API v1",
                    Version = "v1"
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// <summary>
        /// Configures the application middlewares.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <param name="env">The env.</param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // app.Use(async (ctx, next) =>
            // {
            //     try
            //     {
            //         await next();
            //     }
            //     catch (Exception e)
            //     {
            //         if (ctx.Response.HasStarted) throw;

            //         ctx.Response.StatusCode = 500;
            //         ctx.Response.ContentType = "application/json";
                    
            //         var serializer = new JsonSerializer();
            //         using (var textWritter = new System.IO.StreamWriter(ctx.Response.Body))
            //             serializer.Serialize(textWritter, e);
            //     }
            // });

            app.UseCors(builder => builder
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowAnyOrigin());

            app.UseSwagger();
            app.UseSwaggerUI(c => 
                c.SwaggerEndpoint("v1/swagger.json", "SafeToNet Prototype API V1"));

            app.UseHsts();
            
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";
                spa.UseReactDevelopmentServer(npmScript: "start");

                // if (env.IsDevelopment())
                // {
                //     spa.UseReactDevelopmentServer(npmScript: "start");
                // }
            });
        }
    }
}
