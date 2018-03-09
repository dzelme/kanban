using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ESL.CO.React.JiraIntegration;
using Microsoft.Extensions.Logging;
using NetEscapades.Extensions.Logging;

namespace ESL.CO.React
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
            services.AddSingleton<IJiraClient, JiraClient>();
            services.AddSingleton<IAppSettings, AppSettings>();
            services.AddMemoryCache();
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)//, ILoggerFactory loggerFactory)
        {
            /// >>> Changes console output to file - can be used for logging?
            //Console.SetOut(new System.IO.StreamWriter(@"C:\Users\adzelme\source\repos\ESL.CO.Panelis\ESL.CO.React\data\ConsoleOutput.txt"));
            //Console.WriteLine("test123");

            /// >>> unfinished logger implementation
            //var options = new LoggerFilterOptions();
            //options.Rules.Add(new LoggerFilterRule(null, "Microsoft", LogLevel.None, null));
            //options.Rules.Add(new LoggerFilterRule(null, "System", LogLevel.None, null));
            //options.Rules.Add(new LoggerFilterRule(null, "Engine", LogLevel.None, null));
            //var loggerFactory = new LoggerFactory(new List<ILoggerProvider>(), options);
            //loggerFactory
            //    .AddFilter(new FilterLoggerSettings
            //    {
            //        { "Microsoft", LogLevel.None },
            //        { "System", LogLevel.None },
            //        { "Default", LogLevel.None },
            //        { "ESL.CO.React.JiraIntegration.JiraClient", LogLevel.Trace },

            //    });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true,
                    ReactHotModuleReplacement = true
                });
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

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });
        }
    }
}
