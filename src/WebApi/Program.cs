using Application;

using Infrastructure;
using Infrastructure.Database;

using WebApi;
using WebApi.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Configure configuration sources, including command line
builder.Configuration.AddCommandLine(args, new Dictionary<string, string?>
{
    { "--seed", "SeedDataOnStartup" },  // CLI arg '--seed' maps to configuration key 'SeedDataOnStartup'
    { "-s", "SeedDataOnStartup" },  // CLI arg '-s' maps to configuration key 'SeedDataOnStartup'
}!);

builder.AddLogger();
builder.AddTracing();

builder.Services
       .AddApplication()
       .AddPresentation(builder.Configuration)
       .AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApiConfig();
    app.UseSwaggerUi();
    app.ApplyMigrations();
}

app.UseLogger();

app.UseHttpsRedirection();

app.UseExceptionHandler();

await app.SeedApplicationBaseDataAsync();

app.UseCors("CorsPolicy");

app.UseAuthentication();

app.UseAuthorization();

app.MapAppControllers();

await app.RunAsync();