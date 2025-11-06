using Azure.Messaging.ServiceBus;
using FluentValidation.AspNetCore;
using Games.Domain.Dependency;
using Games.Domain.Middleware;
using Games.Infrastructure.Data;
using Games.Infrastructure.Dependency;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
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

//Azure Service Bus
builder.Services.AddSingleton(sp => {
    var cfg = sp.GetRequiredService<IConfiguration>();
return new ServiceBusClient(cfg["ServiceBus:ConnectionString"]);
});

builder.Services.AddSingleton(sp => {
    var cfg = sp.GetRequiredService<IConfiguration>();
    var client = sp.GetRequiredService<ServiceBusClient>();
    var queue = cfg["ServiceBus:Queue"] ?? "payments";
    return client.CreateSender(queue);
});

//Banco de Dados
builder.Services.AddDbContext<DbGames>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Validação FluentValidation
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


builder.Services.AddServices();
builder.Services.AddRepositories();

var app = builder.Build();


if (app.Environment.IsDevelopment() || app.Environment.IsProduction()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseMiddleware<ValidationMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseMiddleware<JwtValidationMiddleware>();
app.UseAuthorization();
app.MapControllers();


using (var scope = app.Services.CreateScope()) {
    var dbContext = scope.ServiceProvider.GetRequiredService<DbGames>();
    await dbContext.Database.EnsureCreatedAsync();
}

await app.RunAsync();
