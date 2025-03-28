﻿using EPSPlus.Application.Implementation;
using EPSPlus.Application.Interface;
using EPSPlus.Domain.Interfaces;
using EPSPlus.Infrastructure.Persistence;
using EPSPlus.Infrastructure.Repositories;
using EPSPlus.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Hangfire;


namespace EPSPlus.API.Presentation.Extensions;

public static class ServiceRegistration
{
    public static IServiceCollection AddCustomHangfire(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHangfire(config =>
        {
            config.UseSimpleAssemblyNameTypeSerializer()
                  .UseRecommendedSerializerSettings()
                  .UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection"));
        });

        services.AddHangfireServer();

        return services;  // ✅ Make sure to return IServiceCollection
    }

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
        services.AddScoped<IContributionService, ContributionService>();
        services.AddScoped<IContributionJobService, ContributionJobService>();
        services.AddScoped<IEmployerService, EmployerService>();
        services.AddScoped<IMemberService, MemberService>();
        services.AddScoped<ITransactionService, TransactionService>();
        services.AddScoped<IAuthService, AuthService>();

        // Register Repositories
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IRepository, Repository>();
        services.AddScoped<IAdminRepository, AdminRepository>();
        services.AddScoped<IContributionRepository, ContributionRepository>();
        services.AddScoped<IEmployerRepository, EmployerRepository>();
        services.AddScoped<IMemberRepository, MemberRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();

        //other services
        services.AddHttpContextAccessor();

    }
}
