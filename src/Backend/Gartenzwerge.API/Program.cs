using Gartenzwerge.Application.Customers.Interfaces;
using Gartenzwerge.Application.Customers.Services;
using Gartenzwerge.Application.Customers.Validators;
using Gartenzwerge.Application.OfferedServices.Interfaces;
using Gartenzwerge.Application.OfferedServices.Services;
using Gartenzwerge.Application.Offers.Interfaces;
using Gartenzwerge.Application.Offers.Services;
using Gartenzwerge.Application.OfferItems.Interfaces;
using Gartenzwerge.Application.OfferItems.Services;
using Gartenzwerge.Application.Orders.Interfaces;
using Gartenzwerge.Application.Orders.Services;
using Gartenzwerge.Infrastructure;
using Gartenzwerge.Infrastructure.Identity;
using Gartenzwerge.API.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;

using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog for structured logging.
builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File(
            "logs/gartenzwerge-api-.log",
            rollingInterval: RollingInterval.Day);
});

// Register ASP.NET Core framework services.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token."
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
            Array.Empty<string>()
        }
    });
});
builder.Services.AddAuthorization();
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];
var jwtSecret = builder.Configuration["Jwt:Secret"];

if (string.IsNullOrWhiteSpace(jwtSecret))
{
    throw new InvalidOperationException("JWT secret is not configured.");
}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSecret))
        };
    });

// Register FluentValidation validators.
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateCustomerRequestValidator>();

// Register application services.
// Register application services.
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IOfferedServiceService, OfferedServiceService>();
builder.Services.AddScoped<IOfferService, OfferService>();
builder.Services.AddScoped<IOfferItemService, OfferItemService>();
builder.Services.AddScoped<IOrderService, OrderService>();

// Register infrastructure services such as EF Core and repositories.
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

await IdentitySeeder.SeedRolesAndDevelopmentUsersAsync(app.Services);

app.UseMiddleware<ExceptionMiddleware>();
app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();