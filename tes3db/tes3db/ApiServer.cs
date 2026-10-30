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
        //create read-only connection string to the SQLite database
        var connString = new SqliteConnectionStringBuilder
        {
            DataSource = dbFilePath,
            Mode = SqliteOpenMode.ReadOnly
        }.ToString();

        //Configure and run kestrel web server to serve the API
        //Empty args so ASP.NET doesn't try to interpret tes3db's own command-line flags
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            Args = Array.Empty<string>(),
            ContentRootPath = AppContext.BaseDirectory,
            WebRootPath = Path.Combine(AppContext.BaseDirectory, "wwwroot")
        });
        builder.Logging.SetMinimumLevel(verbose ? LogLevel.Information : LogLevel.Warning);
        builder.Services.ConfigureHttpJsonOptions(o => o.SerializerOptions.PropertyNamingPolicy = null);

        var app = builder.Build();
        app.Urls.Add($"http://{host}:{port}");
        app.UseDefaultFiles();   // "/" serves wwwroot/index.html
        app.UseStaticFiles();
        app.UseExceptionHandler(errorApp => errorApp.Run(async ctx =>
        {
            ctx.Response.StatusCode = 500;
            await ctx.Response.WriteAsJsonAsync(new { error = "Internal server error." });
        }));

        app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

        Console.WriteLine($"tes3db API listening on http://{host}:{port}  (Ctrl+C to stop)");
        Console.WriteLine($"Try: http://{host}:{port}/api/tables");
        app.Run();
    }

}
