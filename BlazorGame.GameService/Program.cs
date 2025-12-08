using BlazorGame.GameService.Data;
using BlazorGame.GameService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Configurer les services et l'injection de dépendances
ConfigureServices(builder.Services, builder.Configuration);

// Charger en mémoire la base de données avec EF Core et Lazy Loading
builder.Services.AddDbContext<GameDatabaseContext>(opt =>
    opt.UseInMemoryDatabase("GameDb")
        .UseLazyLoadingProxies());

var app = builder.Build();

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
    services.AddScoped<ActionService>();
    services.AddScoped<GameSessionService>();

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
            policy.WithOrigins("http://localhost:5000")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
    });

    // Configuration de l'authentification JWT
    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.Authority = "http://localhost:8180/realms/efrei-realm";
            options.RequireHttpsMetadata = false; // Pour développement uniquement
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = "http://localhost:8180/realms/efrei-realm",
                ValidateAudience = true,
                ValidAudience = "account",
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                RoleClaimType = System.Security.Claims.ClaimTypes.Role
            };

            // Transformer les claims pour extraire les rôles du token
            options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
            {
                OnTokenValidated = context =>
                {
                    var claimsIdentity = context.Principal?.Identity as System.Security.Claims.ClaimsIdentity;
                    if (claimsIdentity != null)
                    {
                        // Extraire les rôles du claim "roles" et les ajouter comme role claims
                        var rolesClaim = claimsIdentity.FindFirst("roles");
                        if (rolesClaim != null)
                        {
                            var roles = System.Text.Json.JsonSerializer.Deserialize<string[]>(rolesClaim.Value);
                            if (roles != null)
                            {
                                foreach (var role in roles)
                                {
                                    claimsIdentity.AddClaim(new System.Security.Claims.Claim(
                                        System.Security.Claims.ClaimTypes.Role, role));
                                }
                            }
                        }
                    }
                    return System.Threading.Tasks.Task.CompletedTask;
                }
            };
        });

    // Configuration de l'autorisation avec politiques par rôle
    services.AddAuthorization(options =>
    {
        options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
        options.AddPolicy("PlayerOnly", policy => policy.RequireRole("Player"));
        options.AddPolicy("AdminOrPlayer", policy => policy.RequireRole("Admin", "Player"));
    });

    // Configuration Swagger pour la documentation de l'API
    services.AddEndpointsApiExplorer();
    services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new()
        {
            Title = "Bladebound API v1",
            Version = "v1",
            Description = "API REST pour le jeu Bladebound : gestion des donjons, joueurs, monstres et sessions de jeu",
        });

        // Inclure les commentaires XML pour la documentation
        var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            c.IncludeXmlComments(xmlPath);
        }
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
    app.UseAuthentication(); // Ajouté pour l'authentification JWT
    app.UseAuthorization();

    app.MapControllers();
}
