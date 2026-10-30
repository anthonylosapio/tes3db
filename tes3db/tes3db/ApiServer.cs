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

        // API endpoint to get a list of all searchable objects in the database
        app.MapGet("/api/search", () => StaticDispatcher.HandleSearch(AppConfiguration.DatabaseConnectionString));
        app.MapGet("/api/header", () => StaticDispatcher.HandleHeader(AppConfiguration.DatabaseConnectionString));

        Console.WriteLine($"tes3db API listening on http://{host}:{port}  (Ctrl+C to stop)");
        Console.WriteLine($"Try: http://{host}:{port}/");
        app.Run();
    }

    public static class StaticDispatcher
    {

        public static async Task<IResult> HandleSearch(string connString) {
            try
            {
                using var connection = new SqliteConnection(connString);
                await connection.OpenAsync();
                using var command = connection.CreateCommand();
                command.CommandText = SqlQueries.search_object_query;
                using var reader = await command.ExecuteReaderAsync();

                var results = new List<object>();
                while (await reader.ReadAsync())
                {
                    results.Add(new
                    {
                        id = reader.GetString(0),
                        name = reader.GetString(1),
                        type = reader.GetString(2)
                    });
                }

                return Results.Ok(results);
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    detail: ex.Message,
                    statusCode: 500,
                    title: "Internal Server Error"
                );
            }
        }

        public static async Task<IResult> HandleHeader(string connString)
        {
            try
            {
                using var connection = new SqliteConnection(connString);
                await connection.OpenAsync();
                using var command = connection.CreateCommand();
                command.CommandText = SqlQueries.header_query;
                using var reader = await command.ExecuteReaderAsync();

                var results = new List<object>();
                while (await reader.ReadAsync())
                {
                    results.Add(new
                    {
                        expansion = reader.GetString(0),
                        author = reader.GetString(1),
                        description = reader.GetString(2)
                    });
                }

                return Results.Ok(results);
            }
            catch (Exception ex)
            {
                return Results.Problem(
                    detail: ex.Message,
                    statusCode: 500,
                    title: "Internal Server Error"
                );
            }
        }

    }


}
