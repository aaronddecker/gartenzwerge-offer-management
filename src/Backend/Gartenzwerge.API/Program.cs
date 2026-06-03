using Gartenzwerge.Application.Customers.Interfaces;
using Gartenzwerge.Application.Customers.Services;
using Gartenzwerge.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Register ASP.NET Core framework services.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthorization();

// Register application services.
builder.Services.AddScoped<ICustomerService, CustomerService>();

// Register infrastructure services such as EF Core and repositories.
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

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