using EPSPlus.Application.Implementation;
using EPSPlus.Application.Interface;
using EPSPlus.Domain.Interfaces;
using EPSPlus.Infrastructure.Persistence;
using EPSPlus.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace EPSPlus.API.Presentation.Extensions;

public static class ServiceRegistration
{
    public static void AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "Fast Api",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Scheme = "Bearer"
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                    new string[] { }
                }
            });
        });

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer(options =>
            {
                //var key = Encoding.UTF8.GetBytes(configuration.GetSection("JWT:Key").Value);
                var jwtKey = configuration.GetSection("JWT:Key").Value;

                if (string.IsNullOrEmpty(jwtKey))
                {
                    throw new ArgumentNullException(nameof(jwtKey), "JWT key is missing in the configuration.");
                }

                var key = Encoding.UTF8.GetBytes(jwtKey);
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = false,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateLifetime = false,
                    ValidateAudience = false,
                    ValidateIssuer = false,
                };
            });

        services.AddCors(options =>
        {
            options.AddPolicy("AllowAllOrigins",
                builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
        });

        services.AddDistributedMemoryCache();
        services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30); 
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        // Register Application Services
        //services.AddScoped<IContributionService, ContributionService>();
        //services.AddScoped<IEmployerService, EmployerService>();
        //services.AddScoped<IMemberService, MemberService>();

        // Register Repositories
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IContributionRepository, ContributionRepository>();
        services.AddScoped<IEmployerRepository, EmployerRepository>();
        services.AddScoped<IMemberRepository, MemberRepository>();

        //other services
        services.AddHttpContextAccessor();

    }
}
