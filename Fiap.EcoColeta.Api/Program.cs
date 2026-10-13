using System.Text;
using AutoMapper;
using Fiap.EcoColeta.Api.Data.Contexts;
using Fiap.EcoColeta.Api.Data.Repository;
using Fiap.EcoColeta.Api.Mapping;
using Fiap.EcoColeta.Api.Middleware;
using Fiap.EcoColeta.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

#region Banco de Dados (SQL Server LocalDB)
var connectionString = builder.Configuration.GetConnectionString("DatabaseConnection");
builder.Services.AddDbContext<DatabaseContext>(opt =>
    opt.UseSqlServer(connectionString).EnableSensitiveDataLogging(builder.Environment.IsDevelopment()));
#endregion

#region Repositórios
builder.Services.AddScoped<ITipoResiduoRepository, TipoResiduoRepository>();
builder.Services.AddScoped<IPontoColetaRepository, PontoColetaRepository>();
builder.Services.AddScoped<IColetaRepository, ColetaRepository>();
builder.Services.AddScoped<IAlertaRepository, AlertaRepository>();
#endregion

#region Serviços de domínio
builder.Services.AddScoped<ITipoResiduoService, TipoResiduoService>();
builder.Services.AddScoped<IPontoColetaService, PontoColetaService>();
builder.Services.AddScoped<IColetaService, ColetaService>();
builder.Services.AddScoped<IAlertaService, AlertaService>();
builder.Services.AddScoped<IRelatorioService, RelatorioService>();
builder.Services.AddSingleton<IAuthService, AuthService>();
#endregion

#region AutoMapper
var mapperConfig = new MapperConfiguration(cfg =>
{
    cfg.AllowNullCollections = true;
    cfg.AllowNullDestinationValues = true;
    cfg.AddProfile<MappingProfile>();
});
builder.Services.AddSingleton<IMapper>(mapperConfig.CreateMapper());
#endregion

#region Autenticação JWT
var jwtKey = builder.Configuration["Jwt:Key"]!;
var jwtIssuer = builder.Configuration["Jwt:Issuer"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddAuthorization();
#endregion

builder.Services.AddControllers();

#region Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "EcoColeta API",
        Version = "v1",
        Description = "API RESTful para gestão de resíduos e reciclagem (ESG) — FIAP Fase 4, 2TDS."
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Informe o token JWT no formato: Bearer {seu token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});
#endregion

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

#region Migração automática (Staging/Production em container)
// Em Development o fluxo manual (dotnet ef database update) contra o LocalDB
// continua funcionando normalmente — este bloco só roda fora de Development,
// que é como identificamos os ambientes containerizados (Staging/Production
// no docker-compose, ver ASPNETCORE_ENVIRONMENT). O SQL Server do container
// pode ainda estar inicializando quando a API sobe, então aplicamos algumas
// tentativas com espera crescente antes de desistir.
if (!app.Environment.IsDevelopment())
{
    const int maxTentativas = 8;
    for (var tentativa = 1; tentativa <= maxTentativas; tentativa++)
    {
        try
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            db.Database.Migrate();
            break;
        }
        catch (Exception ex) when (tentativa < maxTentativas)
        {
            var logger = app.Services.GetRequiredService<ILogger<Program>>();
            logger.LogWarning(ex, "Falha ao aplicar migrações (tentativa {Tentativa}/{Max}). Tentando novamente em {Delay}s...",
                tentativa, maxTentativas, tentativa * 3);
            Thread.Sleep(TimeSpan.FromSeconds(tentativa * 3));
        }
    }
}
#endregion

if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "EcoColeta API v1"));
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

// Exposto para permitir o uso de WebApplicationFactory<Program> nos testes de integração (xUnit).
public partial class Program { }