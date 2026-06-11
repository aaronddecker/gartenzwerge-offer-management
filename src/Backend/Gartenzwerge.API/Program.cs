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
using FluentValidation;
using FluentValidation.AspNetCore;
using Gartenzwerge.API.Middleware;
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
builder.Services.AddSwaggerGen();
builder.Services.AddAuthorization();

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

app.UseMiddleware<ExceptionMiddleware>();
app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();