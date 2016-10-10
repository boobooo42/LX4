﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Swashbuckle.Swagger.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace LexicalAnalyzer
{
    public class Startup
    {
        /* Thanks to:
         * http://www.talkingdotnet.com/add-swagger-to-asp-net-core-web-api/
         */
        private static string GetXmlCommentsPath()
        {
            var app = PlatformServices.Default.Application;
            return System.IO.Path.Combine(
                    app.ApplicationBasePath, "LexicalAnalyzer.xml");
        }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var pathToDoc = "LexicalAnalyzer.xml";

            // Add framework services.
            services.AddMvc();
            services.AddSwaggerGen(c =>
                {
                /* FIXME: Get the swagger version and title working */
                /*
                    c.SwaggerDoc("v1",
                            new Info
                            {
                            Version = "v1",
                            Title = "Lexical Analyzer API",
                            Description = "Internal API for performing language research"
                            });
                            */

                });

            services.ConfigureSwaggerGen(c =>
                {
                    c.IncludeXmlComments(GetXmlCommentsPath());
                    c.DescribeAllEnumsAsStrings();
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
                routes.MapRoute(
                    name: "Web API",
                    template: "api/{controller}/{action=Get}"
                    );
            });

            app.UseSwagger();
            app.UseSwaggerUi();
        }
    }
}
