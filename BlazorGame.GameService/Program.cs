using BlazorGame.GameService.Data;

var builder = WebApplication.CreateBuilder(args);

ConfigureServices(builder.Services, builder.Configuration);

// Charger en mémoire la base de données
builder.Services.AddDbContext<GameDatabaseContext>(opt =>
    opt.UseInMemoryDatabase("GameDb")
        .UseLazyLoadingProxies());

var app = builder.Build();

// Initialiser la base de données
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<GameDatabaseContext>();
    DatabaseInitializer.Initialize(db);
}

ConfigureMiddleware(app);

app.Run();

// Configure les services de l'application et l'injection de dépendances
static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    // Enregistrement des services
    services.AddScoped<DungeonService>();
    services.AddScoped<FightService>();
    services.AddScoped<MonstersService>();    
    services.AddScoped<PlayerService>();
    services.AddScoped<RoomsService>();

    // Configuration des contrôleurs avec des options JSON personnalisées
    services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            options.JsonSerializerOptions.PropertyNamingPolicy = null;
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        });

    // Configuration Cors pour autoriser les requêtes du client Blazor
    services.AddCors(options =>
    {
        options.AddPolicy("AllowBlazorClient", policy =>
        {
            policy.WithOrigins(
                    "http://localhost:5133"
                )
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
    });

    // Configuration de Swagger pour la documentation de l'API
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

// Configure le pipeline de traitement des requêtes HTTP
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
