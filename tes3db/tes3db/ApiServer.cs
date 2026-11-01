namespace tes3db;

using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public class ApiServer
{

    public static void Run(string dbFilePath, string host, int port, bool verbose)
    {
        

        //Configure and run kestrel web server to serve the API
        //Empty args so ASP.NET doesn't try to interpret the tes3db command-line flags
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            Args = Array.Empty<string>(),
            ContentRootPath = AppContext.BaseDirectory,
            WebRootPath = Path.Combine(AppContext.BaseDirectory, "wwwroot")
        });
        builder.Logging.SetMinimumLevel(verbose ? LogLevel.Information : LogLevel.Warning);
        builder.Services.ConfigureHttpJsonOptions(o => o.SerializerOptions.PropertyNamingPolicy = null);
        builder.Services.AddProblemDetails();

        builder.Services.AddSingleton<IDbConnectionFactory>(new SqliteConnectionFactory(dbFilePath));
        builder.Services.AddScoped<ICatalogRepository, CatalogRepository>();

        var app = builder.Build();
        app.Urls.Add($"http://{host}:{port}");
        app.UseExceptionHandler();
        app.UseStatusCodePages();
        app.UseDefaultFiles();   // "/" serves wwwroot/index.html
        app.UseStaticFiles();

        app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

        // API endpoints
        app.MapGet("/api/search", async (ICatalogRepository repo, CancellationToken ct) =>
               TypedResults.Ok(await repo.GetSearchObjectsAsync(ct)));

        app.MapGet("/api/header", async (ICatalogRepository repo, CancellationToken ct) =>
            TypedResults.Ok(await repo.GetHeadersAsync(ct)));

        Console.WriteLine($"tes3db API listening on http://{host}:{port}  (Ctrl+C to stop)");
        Console.WriteLine($"Try: http://{host}:{port}/");
        app.Run();
    }

}

