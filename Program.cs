using LoginApi.Data;
using LoginApi.Services;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Diagnostics;

using LoginApi.Filters;
using System.Text;
using DotNetEnv;

Env.Load();

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

var dbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? throw new InvalidOperationException("DB_HOST must be provided");
var dbPort = int.Parse(Environment.GetEnvironmentVariable("DB_PORT") ?? throw new InvalidOperationException("DB_PORT must be provided"));
var dbName = Environment.GetEnvironmentVariable("DB_NAME") ?? throw new InvalidOperationException("DB_NAME must be provided");
var dbUser = Environment.GetEnvironmentVariable("DB_USER") ?? throw new InvalidOperationException("DB_USER must be provided");
var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD") ?? throw new InvalidOperationException("DB_PASSWORD must be provided");

var connectionString =
    $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPassword}";

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddScoped<JwtService>();

builder.Services.AddAutoMapper(typeof(Program).Assembly);

builder.Services.AddControllers();

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ApiResponseWrapperFilter>();
});

var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY")
                   ?? throw new InvalidOperationException("JWT Key must be provided");
var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER")
                   ?? throw new InvalidOperationException("JWT Issuer must be provided");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtIssuer,
                    ValidAudience = jwtIssuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                };
            });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exceptionHandler = context.Features.Get<IExceptionHandlerFeature>();
        var ex = exceptionHandler?.Error;

        context.Response.StatusCode = ex switch
        {
            BadHttpRequestException => 400,
            _ => 500
        };

        await context.Response.WriteAsJsonAsync(new { error = ex?.Message });
    });
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
