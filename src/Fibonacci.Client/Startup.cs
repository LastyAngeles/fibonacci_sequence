using EasyNetQ;
using Fibonacci.Calculation.Services;
using Fibonacci.Client.Options;
using Fibonacci.Client.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Fibonacci.Client;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.Configure<AppOptions>(Configuration.GetSection("AppSettings"));

        services.AddTransient<IHostCalculationService, HostCalculationService>();
        services.AddCalculationServices();
        services.AddSingleton(_ => RabbitHutch.CreateBus(Configuration["RabbitMqConnectionString"]));

        services.AddHttpClient("FibonacciHttpClient");
        services.AddHostedService<ConsumeCalculationBackgroundService>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        else
        {
            app.UseExceptionHandler("/Error");
        }

        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapDefaultControllerRoute();
        });
    }
}