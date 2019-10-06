using System;
using EasyNetQ;
using Elasticsearch.EsExtensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCoreWorker
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
            // asp services
            services
                .AddMvc(config => config.Filters.Add(typeof(MyExceptionFilter)))
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // Ioc
            services.AddSingleton(RabbitHutch.CreateBus("host=localhost;username=user;password=password"));
            services.AddHostedService<MessageProcessor>();
            services.UseEsConfiguration(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (!env.IsDevelopment())
                app.UseHsts();
            
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}

public class MyExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        throw new NotImplementedException();
    }
}