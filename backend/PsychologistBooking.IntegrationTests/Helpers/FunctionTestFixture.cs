using Microsoft.Extensions.Configuration;
using PsychologistBooking.Application.UseCases;
using PsychologistBooking.Domain.Entities;
using PsychologistBooking.Domain.Enums;
using PsychologistBooking.Domain.Interfaces;
using PsychologistBooking.Infrastructure.Data;
using PsychologistBooking.Infrastructure.Persistence.Repositories;

namespace PsychologistBooking.IntegrationTests.Helpers;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

public class FunctionTestFixture : IDisposable
{
    public IServiceProvider Services { get; }

    public FunctionTestFixture()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Test.json")
            .AddEnvironmentVariables()
            .Build();

        var services = new ServiceCollection();

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(config.GetConnectionString("DefaultConnection"));
        });

        services.AddScoped<IPsychologistRepository, PsychologistRepository>();
        services.AddScoped<IGetPsychologistsUseCase, GetPsychologistsUseCase>();

        Services = services.BuildServiceProvider();

        // Ensure DB is seeded
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.Database.EnsureCreated();
        SeedData(context);
    }

    private void SeedData(AppDbContext context)
    {
        // Check if psychologist "Alice Anderson" of type Clinical exists
        bool aliceExists = context.Psychologists.Any(p =>
            p.FirstName == "Alice" &&
            p.LastName == "Anderson" &&
            p.PsychologistType == PsychologistType.Clinical);

        if (!aliceExists)
        {
            var psych1 = Psychologist.Builder()
                .WithName("Alice", "Anderson")
                .WithType(PsychologistType.Clinical)
                .WithDescription("Experienced clinical psychologist with trauma specialization")
                .AddAvailability(DateTime.UtcNow.Date.AddDays(1))
                .Build();

            context.Psychologists.Add(psych1);
        }

        // Check if psychologist "Bob Brown" of type Anxiety exists
        bool bobExists = context.Psychologists.Any(p =>
            p.FirstName == "Bob" &&
            p.LastName == "Brown" &&
            p.PsychologistType == PsychologistType.Anxiety);

        if (!bobExists)
        {
            var psych2 = Psychologist.Builder()
                .WithName("Bob", "Brown")
                .WithType(PsychologistType.Anxiety)
                .WithDescription("Helps with work-life balance and anxiety")
                .AddAvailability(DateTime.UtcNow.Date.AddDays(2))
                .Build();

            context.Psychologists.Add(psych2);
        }

        context.SaveChanges();
    }

    public void Dispose()
    {
    }
}
