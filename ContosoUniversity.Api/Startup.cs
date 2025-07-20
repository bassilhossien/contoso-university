using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Rewrite;
using ContosoUniversity.Common;
using ContosoUniversity.Common.Data;
using ContosoUniversity.Common.Interfaces;
//using Swashbuckle.AspNetCore.Swagger;
using Microsoft.OpenApi.Models;
using AutoMapper;
using ContosoUniversity.Data.DbContexts;
using Microsoft.Extensions.Hosting;

namespace ContosoUniversity.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment CurrentEnvironment { get; }

        public Startup(IWebHostEnvironment env, IConfiguration config)
        {
            CurrentEnvironment = env;
            Configuration = config;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCustomizedContext(Configuration, CurrentEnvironment)
                .AddAutoMapper(cfg =>
                {
                    cfg.AddProfile<ApiProfile>();
                })
                .AddCustomizedMvc(CurrentEnvironment)
                .AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Contoso University API", Version = "v1" });
                });

            services.AddCustomizedApiAuthentication(Configuration);
            services.AddScoped<UnitOfWork<ApiContext>, UnitOfWork<ApiContext>>();
            services.AddScoped<IDbInitializer, ApiInitializer>();
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory, IDbInitializer dbInitializer)
        {
            if (CurrentEnvironment.IsDevelopment())
            {
                dbInitializer.Initialize();
                app.UseDeveloperExceptionPage();
            }
            // else
            // {
            //     app.UseRewriter(new RewriteOptions().AddRedirectToHttps());
            // }
            app.UseAuthentication()
                .UseDefaultFiles()
                .UseStaticFiles()
                .UseSwagger()
                .UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Contoso API V1");
                })
                .UseMvcWithDefaultRoute();
        }

        public void ConfigureTesting(IApplicationBuilder app, IDbInitializer dbInitializer)
        {
            dbInitializer.Initialize();
            app.UseAuthentication()
                // .UseRewriter(new RewriteOptions().AddRedirectToHttps())
                .UseMvcWithDefaultRoute();
        }
    }
}
