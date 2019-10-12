using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ocelot.JwtAuthorize;

[assembly: OwinStartup(typeof(MESInterface.Startup))]

namespace MESInterface
{
    public class Startup
    {
        //public void Configuration(IAppBuilder app)
        //{
        //    // 有关如何配置应用程序的详细信息，请访问 https://go.microsoft.com/fwlink/?LinkID=316888
        //}
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTokenJwtAuthorize();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }


        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
            }
            app.UseMvc();
        }
    }
}
