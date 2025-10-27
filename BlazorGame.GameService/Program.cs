var builder = WebApplication.CreateBuilder(args);

// Configurer les services et l'injection de dépendances
ConfigureServices(builder.Services, builder.Configuration);

// Charger en mémoire la base de données avec EF Core et Lazy Loading
builder.Services.AddDbContext<GameDatabaseContext>(opt =>
    opt.UseInMemoryDatabase("GameDb")
        .UseLazyLoadingProxies());

var app = builder.Build();

// Initialiser la base de données avec des données par défaut
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<GameDatabaseContext>();
    DatabaseInitializer.Initialize(db);
}

// Configurer le pipeline de traitement des requêtes HTTP
ConfigureMiddleware(app);

app.Run();

/// <summary>
/// Configure les services de l'application et l'injection de dépendances.
/// </summary>
static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    // Enregistrement des services métiers
    services.AddScoped<DungeonsService>();
    services.AddScoped<FightService>();
    services.AddScoped<MonstersService>();
    services.AddScoped<PlayerService>();
    services.AddScoped<RoomsService>();

    // Configuration des contrôleurs avec options JSON
    services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            options.JsonSerializerOptions.PropertyNamingPolicy = null;
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        });

    // Configuration CORS pour autoriser le client Blazor
    services.AddCors(options =>
    {
        options.AddPolicy("AllowBlazorClient", policy =>
        {
            policy.WithOrigins("http://localhost:5133")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
    });

    // Configuration Swagger pour la documentation de l'API
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new()
        {
            Title = "Bladebound API v1",
            Version = "v1",
        });
    });
}

/// <summary>
/// Configure le pipeline de middleware HTTP.
/// </summary>
static void ConfigureMiddleware(WebApplication app)
{
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseCors("AllowBlazorClient");
    app.UseRouting();
    app.UseAuthorization();

    app.MapControllers();
}
