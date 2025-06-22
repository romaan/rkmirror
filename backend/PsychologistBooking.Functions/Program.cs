using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PsychologistBooking.Application.UseCases;
using PsychologistBooking.Domain.Interfaces;
using PsychologistBooking.Infrastructure.Data;
using PsychologistBooking.Infrastructure.Persistence.Repositories;


var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .UseServiceProviderFactory(new AutofacServiceProviderFactory())
    .ConfigureAppConfiguration(config =>
    {
        config.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);
        config.AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        // Register DbContext
        var connStr = context.Configuration["SqlConnectionString"];
        if (string.IsNullOrWhiteSpace(connStr))
        {
            throw new InvalidOperationException("SqlConnectionString is missing in configuration.");
        }

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(connStr);
        });
        services.AddScoped<IPsychologistRepository, PsychologistRepository>();
        services.AddScoped<IGetPsychologistsUseCase, GetPsychologistsUseCase>();
        services.AddSingleton<IGetPsychologistTypesUseCase, GetPsychologistTypesUseCase>();

    })
    .ConfigureContainer<ContainerBuilder>(builder =>
    {
        // Add this to resolve IPsychologistRepository
        builder.RegisterType<PsychologistRepository>()
            .As<IPsychologistRepository>()
            .InstancePerLifetimeScope();
        
    })
    .Build();

await host.RunAsync();