using Gartenzwerge.Application.Customers.Interfaces;
using Gartenzwerge.Application.Customers.Services;
using Gartenzwerge.Infrastructure;
using FluentValidation;
using FluentValidation.AspNetCore;
using Gartenzwerge.Application.Customers.Validators;
using Gartenzwerge.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Register ASP.NET Core framework services.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthorization();

// Register FluentValidation validators.
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateCustomerRequestValidator>();

// Register application services.
builder.Services.AddScoped<ICustomerService, CustomerService>();

// Register infrastructure services such as EF Core and repositories.
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

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