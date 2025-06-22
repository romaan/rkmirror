using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PsychologistBooking.Domain.Entities;
using PsychologistBooking.Domain.Enums;
using PsychologistBooking.Infrastructure.Data;

var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Development";

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.SetBasePath(AppContext.BaseDirectory);
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
              .AddJsonFile($"appsettings.{environment}.json", optional: true)
              .AddEnvironmentVariables();
    })
    .ConfigureServices((context, services) =>
    {
        var connectionString = context.Configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));
    })
    .Build();

// Start seeding
using var scope = host.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

Console.WriteLine($"Environment: {environment}");
Console.WriteLine("Checking database connection...");

await db.Database.EnsureCreatedAsync();

if (await db.Psychologists.AnyAsync())
{
    Console.WriteLine("Psychologist data already exists. Skipping seeding.");
    return;
}

Console.WriteLine("Generating fake psychologist data...");

var faker = new Faker("en");

var psychologistFaker = new Faker<Psychologist>()
    .CustomInstantiator(f =>
    {
        var builder = Psychologist.Builder()
            .WithName(f.Name.FirstName(), f.Name.LastName())
            .WithType(f.PickRandom<PsychologistType>())
            .WithDescription(f.Lorem.Sentence(8));

        var availableDates = f.Make(f.Random.Int(3, 7), () =>
            f.Date.Soon(14, DateTime.UtcNow.AddDays(1)).Date
                .AddHours(f.Random.Int(9, 17))
                .AddMinutes(f.Random.Bool() ? 0 : 30)
        );

        foreach (var date in availableDates.Distinct())
        {
            builder.AddAvailability(date);
        }

        return builder.Build();
    });

var psychologists = psychologistFaker.Generate(25);

await db.Psychologists.AddRangeAsync(psychologists);
await db.SaveChangesAsync();

Console.WriteLine("Seeded fake psychologist data successfully.");
