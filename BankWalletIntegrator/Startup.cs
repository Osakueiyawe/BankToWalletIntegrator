using BankWalletIntegrator.Integrations;
using BankWalletIntegrator.Interfaces;
using BankWalletIntegrator.Middleware;
using BankWalletIntegrator.Repository;
using BankWalletIntegrator.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BankWalletIntegrator
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

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "BankWalletIntegrator", Version = "v1" });
            });
            services.AddScoped<IAccountUtility, AccountUtility>();
            services.AddScoped(typeof(IFiBridgeUtility<>), typeof(FiBridgeUtility<>));
            services.AddScoped<IFinnacleManager, FinnacleManager>();
            services.AddScoped<IPostingModelUtility, PostingModelUtility>();
            services.AddScoped<IRequestValidation, RequestValidation>();
            services.AddScoped<IBankToWalletMerchants, BankToWalletMerchants>();
            services.AddScoped<ILogUtility, LogUtility>();
            services.AddScoped<IOrangeSenegalIntegration, OrangeSenegalIntegration>();
            services.AddScoped<IMTNGuineaIntegration, MTNGuineaIntegration>();
            services.AddScoped<IMiddlewareRepo, MiddlewareRepo>();
            services.AddScoped<IMiddlewareService, MiddlewareService>();
            services.AddScoped(typeof(IApiUtility<>), typeof(ApiUtility<>));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BankWalletIntegrator v1"));
            }

            app.UseRouting();
            app.UseMiddleware<RequestResponseMiddleware>();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
