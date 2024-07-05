using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SimpleTdo.DataAccess;
using System;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Добавляем IHttpContextAccessor в сервисы
builder.Services.AddHttpContextAccessor();

// Добавляем сервисы для Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SimpleTdo API", Version = "v1" });

    // Добавляем секцию авторизации в Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Введите JWT токен следующим образом: Bearer {токен}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Добавляем сервисы для работы с контроллерами и базами данных
builder.Services.AddControllers();
builder.Services.AddScoped<NotesDbContext>();
builder.Services.AddScoped<UsersDbContext>();

// Получаем ключ для подписи JWT из конфигурации
var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]);

// Настраиваем аутентификацию и авторизацию
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero // Optional: reduce the allowed clock skew
    };
});

// Добавляем политику CORS для разрешения запросов с определённого источника
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Создаём и применяем миграции баз данных
using var scope = app.Services.CreateScope();
await using var dbContext = scope.ServiceProvider.GetRequiredService<NotesDbContext>();
await dbContext.Database.MigrateAsync();

using var userScope = app.Services.CreateScope();
await using var usersDbContext = userScope.ServiceProvider.GetRequiredService<UsersDbContext>();
await usersDbContext.Database.MigrateAsync();

// Добавляем Swagger в разработке
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "SimpleTdo API v1");
    });
}

// Настройка промежуточных компонентов
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Запуск приложения
app.Run();