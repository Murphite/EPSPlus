
using EPSPlus.API.Presentation.Extensions;
using EPSPlus.API.Presentation.Middlewares;
using EPSPlus.Infrastructure.DbInitializer;
using Hangfire;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text.Json.Serialization;
using EPSPlus.API.Presentation.Extensions;
using EPSPlus.Application.Interface;

var builder = WebApplication.CreateBuilder(args);

// Configure services
builder.Services.AddControllers()
    .AddNewtonsoftJson()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbServices(builder.Configuration);
builder.Services.AddServices(builder.Configuration);
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "EPSPlus API", Version = "v1" });
});

// Logging
builder.Logging.AddSerilog();

builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration)
                 .ReadFrom.Services(services)
                 .Enrich.FromLogContext()
                 .WriteTo.Console()
                 .WriteTo.File("./logs/log-.txt", rollingInterval: RollingInterval.Day);
});

builder.Services.AddCustomHangfire(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins(
            "https://api.paystack.co",
            "http://localhost:3000",
            "https://lyvads-admin-dashboard.vercel.app",
            "https://radiksez.admin.lyvads.com"
        )
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});


// Build app
var app = builder.Build();

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseCors("AllowSpecificOrigins");
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseMiddleware<ExceptionMiddleware>();
app.UseHangfireDashboard();

app.MapControllers();

await DbInitializer.Run(app);

app.Run();

