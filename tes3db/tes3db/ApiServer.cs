namespace tes3db;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

public class ApiServer
{

    public static void Run(string dbFilePath, string host, int port, bool verbose, bool noBrowser)
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
        builder.Services.AddScoped<ICatalogService, CatalogService>();

        var app = builder.Build();

        app.Urls.Add($"http://{host}:{port}");
        app.UseExceptionHandler();
        app.UseStatusCodePages();
        app.UseDefaultFiles();   // "/" serves wwwroot/index.html
        app.UseStaticFiles();

        app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

        // API endpoints
        app.MapGet("/api/search", async (ICatalogService service, CancellationToken ct) =>
               TypedResults.Ok(await service.RunQueryAsync(SqlQueries.search_object_query, ct)));

        app.MapGet("/api/inventoryobject", async (ICatalogService service, CancellationToken ct) =>
               TypedResults.Ok(await service.RunQueryAsync(SqlQueries.inventory_object_query, ct)));

        app.MapGet("/api/header", async (ICatalogService service, CancellationToken ct) =>
            TypedResults.Ok(await service.RunQueryAsync(SqlQueries.header_query, ct)));

        app.MapGet("/api/interface", async (ICatalogService service, CancellationToken ct) =>
            TypedResults.Ok(await service.GetInterfaceObject(ct)));

        app.MapGet("/api/alchemy/{id}", async (ICatalogService service, string id, CancellationToken ct) =>
            TypedResults.Ok(await service.GetItem(ct, SqlQueries.alchemy_query, id)));

        app.MapGet("/api/apparatus/{id}", async (ICatalogService service, string id, CancellationToken ct) =>
            TypedResults.Ok(await service.GetItem(ct, SqlQueries.apparatus_query, id)));

        app.MapGet("/api/armor/{id}", async (ICatalogService service, string id, CancellationToken ct) =>
            TypedResults.Ok(await service.GetItem(ct, SqlQueries.armor_query, id)));

        app.MapGet("/api/birthsign/{id}", async (ICatalogService service, string id, CancellationToken ct) =>
            TypedResults.Ok(await service.GetBirthsign(ct, id)));

        app.MapGet("/api/book/{id}", async (ICatalogService service, string id, CancellationToken ct) =>
            TypedResults.Ok(await service.GetItem(ct, SqlQueries.book_query, id)));

        app.MapGet("/api/cell/{id}", async (ICatalogService service, string id, CancellationToken ct) =>
            TypedResults.Ok(await service.GetCell(ct, id)));

        app.MapGet("/api/class/{id}", async (ICatalogService service, string id, CancellationToken ct) =>
            TypedResults.Ok(await service.GetClassFactionRace(ct, id, SqlQueries.class_query, SqlQueries.npc_class_query)));

        app.MapGet("/api/clothing/{id}", async (ICatalogService service, string id, CancellationToken ct) =>
            TypedResults.Ok(await service.GetItem(ct, SqlQueries.clothing_query, id)));

        app.MapGet("/api/container/{id}", async (ICatalogService service, string id, CancellationToken ct) =>
            TypedResults.Ok(await service.GetContainerOrCreatureList(ct, id, SqlQueries.container_query, SqlQueries.container_cell_query)));

        app.MapGet("/api/creature/{id}", async (ICatalogService service, string id, CancellationToken ct) =>
            TypedResults.Ok(await service.GetCreature(ct, id)));

        app.MapGet("/api/faction/{id}", async (ICatalogService service, string id, CancellationToken ct) =>
            TypedResults.Ok(await service.GetClassFactionRace(ct, id, SqlQueries.faction_query, SqlQueries.npc_faction_query)));

        app.MapGet("/api/ingredient/{id}", async (ICatalogService service, string id, CancellationToken ct) =>
            TypedResults.Ok(await service.GetItem(ct, SqlQueries.ingredient_query, id)));

        app.MapGet("/api/leveledcreature/{id}", async (ICatalogService service, string id, CancellationToken ct) =>
            TypedResults.Ok(await service.GetContainerOrCreatureList(ct, id, SqlQueries.leveledcreature_query, SqlQueries.cell_inventory_query)));

        app.MapGet("/api/leveleditem/{id}", async (ICatalogService service, string id, CancellationToken ct) =>
            TypedResults.Ok(await service.GetLeveledItem(ct, id)));

        app.MapGet("/api/lockpick/{id}", async (ICatalogService service, string id, CancellationToken ct) =>
            TypedResults.Ok(await service.GetItem(ct, SqlQueries.lockpick_query, id)));

        app.MapGet("/api/npc/{id}", async (ICatalogService service, string id, CancellationToken ct) =>
            TypedResults.Ok(await service.GetNpc(ct, id)));

        app.MapGet("/api/magiceffect/{id}", async (ICatalogService service, string id, CancellationToken ct) =>
            TypedResults.Ok(await service.GetMagicEffect(ct, id)));

        app.MapGet("/api/race/{id}", async (ICatalogService service, string id, CancellationToken ct) =>
            TypedResults.Ok(await service.GetClassFactionRace(ct, id, SqlQueries.race_query, SqlQueries.npc_race_query)));

        Console.WriteLine($"tes3db API listening on http://{host}:{port}  (Ctrl+C to stop)");
        Console.WriteLine($"Try: http://{host}:{port}/");

        var browseHost = host is "0.0.0.0" or "+" or "*" or "::" ? "localhost" : host;
        var url = $"http://{browseHost}:{port}/";

        app.Lifetime.ApplicationStarted.Register(() =>
        {
            Console.WriteLine($"tes3db listening on {url}  (Ctrl+C to stop)");
            if (!noBrowser) OpenBrowser(url);
        });

        app.Run();
    }

    private static void OpenBrowser(string url)
    {
        try
        {
            if (OperatingSystem.IsWindows())
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true })?.Dispose();
            else if (OperatingSystem.IsMacOS())
                Process.Start("open", url)?.Dispose();
            else if (OperatingSystem.IsLinux())
                Process.Start("xdg-open", url)?.Dispose();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Couldn't open a browser automatically ({ex.Message}). Open {url} manually.");
        }
    }

}

