using Megabin_Web.Interfaces;
using Megabin_Web.Services;
using Microsoft.EntityFrameworkCore;
using Megabin_Web.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Configure PostgreSQL Database
builder.Services.AddDbContext<Megabin_Web.Data.AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Configure OpenRouteService Options
builder.Services.Configure<OpenRouteServiceOptions>(
    builder.Configuration.GetSection("OpenRouteService")
);

// Configure Services with HttpClient
builder.Services.AddHttpClient<IOpenRouteService, OpenRouteService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
