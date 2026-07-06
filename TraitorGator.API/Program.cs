using Microsoft.EntityFrameworkCore;
using TraitorGator.API.Data;
using TraitorGator.Services.Interfaces;
using TraitorGator.Services;
using TraitorGator.Services.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
     .AddJsonOptions(options =>
     {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.WriteIndented = true;
     });

var databaseProvider = builder.Configuration["Database:Provider"] ?? "Sqlite";
builder.Services.AddDbContext<GameDbContext>(options =>
{
    if (databaseProvider.Equals("Sqlite", StringComparison.OrdinalIgnoreCase))
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
            ?? "Data Source=traitorgator.db";
        options.UseSqlite(connectionString);
    }
    else
    {
        throw new InvalidOperationException($"Okänd databasprovider: {databaseProvider}");
    }
});

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
if (allowedOrigins.Length > 0 || builder.Environment.IsDevelopment())
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("Client", policy =>
        {
            if (allowedOrigins.Length > 0)
            {
                policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod();
            }
            else
            {
                policy
                    .SetIsOriginAllowed(origin =>
                        Uri.TryCreate(origin, UriKind.Absolute, out var uri) &&
                        (uri.Host == "localhost" || uri.Host == "127.0.0.1"))
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            }
        });
    });
}

builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<IQuestionService, QuestionService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    app.Logger.LogInformation("Initializing TraitorGator database.");
    var db = scope.ServiceProvider.GetRequiredService<GameDbContext>();
    await db.Database.EnsureCreatedAsync();
    await GameSeedData.EnsureSeededAsync(db);
    app.Logger.LogInformation("TraitorGator database is ready.");
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

if (allowedOrigins.Length > 0 || app.Environment.IsDevelopment())
{
    app.UseCors("Client");
}

app.UseAuthorization();

app.MapControllers();

app.Run();
