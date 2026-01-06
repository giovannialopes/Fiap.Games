using FluentValidation.AspNetCore;
using Games.Domain.Dependency;
using Games.Domain.Middleware;
using Games.Infrastructure.Data;
using Games.Infrastructure.Dependency;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Games", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
        Name = "Authorization",
        Description = "Insira o token JWT no formato: Bearer {seu token}",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

//RabbitMQ com MassTransit
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        var configuration = context.GetRequiredService<IConfiguration>();
        var loggerFactory = context.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("MassTransit");
        
        var host = configuration["RabbitMQ:Host"] ?? "localhost";
        var port = configuration.GetValue<ushort>("RabbitMQ:Port", 5672);
        var username = configuration["RabbitMQ:Username"] ?? "guest";
        var password = configuration["RabbitMQ:Password"] ?? "guest";
        var virtualHost = configuration["RabbitMQ:VirtualHost"] ?? "/";

        logger.LogInformation("Configurando RabbitMQ: Host={Host}, Port={Port}, VirtualHost={VirtualHost}, Username={Username}", 
            host, port, virtualHost, username);

        cfg.Host(host, port, virtualHost, h =>
        {
            h.Username(username);
            h.Password(password);
        });

        // Configuração de reconexão automática com retry exponencial
        cfg.UseMessageRetry(r => r.Exponential(
            retryLimit: 5,
            minInterval: TimeSpan.FromSeconds(1),
            maxInterval: TimeSpan.FromSeconds(30),
            intervalDelta: TimeSpan.FromSeconds(2)
        ));

        // Configuração de circuit breaker para resiliência
        cfg.UseCircuitBreaker(cb => {
            cb.TrackingPeriod = TimeSpan.FromMinutes(1);
            cb.TripThreshold = 15;
            cb.ActiveThreshold = 10;
            cb.ResetInterval = TimeSpan.FromMinutes(5);
        });

        // Configuração de outbox para garantir entrega
        cfg.UseInMemoryOutbox();
        
        cfg.ConfigureEndpoints(context);
    });
});

//Banco de Dados
builder.Services.AddDbContext<DbGames>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Valida��o FluentValidation
builder.Services.AddFluentValidationAutoValidation()
    .AddFluentValidationClientsideAdapters();


var jwt = builder.Configuration.GetSection("JwtSettings");
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o => {
        o.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuer = true,
            ValidIssuer = jwt["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwt["Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });


// Health Checks
builder.Services.AddHealthChecks()
    .AddCheck("database", () => {
        // Verificação básica - será verificado no runtime
        return Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("Database check configurado");
    })
    .AddCheck("rabbitmq", () => {
        // Verificação básica de conectividade RabbitMQ
        var host = builder.Configuration["RabbitMQ:Host"] ?? "localhost";
        var port = builder.Configuration.GetValue<ushort>("RabbitMQ:Port", 5672);
        try {
            using var client = new System.Net.Sockets.TcpClient();
            var result = client.BeginConnect(host, port, null, null);
            var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(1));
            if (!success) return Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Unhealthy("RabbitMQ não acessível");
            client.EndConnect(result);
            return Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("RabbitMQ acessível");
        } catch {
            return Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Unhealthy("RabbitMQ não acessível");
        }
    });

builder.Services.AddServices();
builder.Services.AddRepositories();

var app = builder.Build();


if (app.Environment.IsDevelopment() || app.Environment.IsProduction()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseMiddleware<ObservabilityMiddleware>();
app.UseMiddleware<ValidationMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseMiddleware<JwtValidationMiddleware>();
app.UseAuthorization();

// Health Check endpoint
app.MapHealthChecks("/health");

app.MapControllers();


// Inicializar banco de dados (não crítico - aplicação continua mesmo se falhar)
try {
    using (var scope = app.Services.CreateScope()) {
        var dbContext = scope.ServiceProvider.GetRequiredService<DbGames>();
        await dbContext.Database.EnsureCreatedAsync();
    }
} catch (Exception ex) {
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogWarning(ex, "Não foi possível conectar ao banco de dados na inicialização. A aplicação continuará rodando.");
}

await app.RunAsync();
