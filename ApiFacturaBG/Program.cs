using MapSql.Extensions;
using Microsoft.Extensions.Configuration;
using Models.Utils;

var builder = WebApplication.CreateBuilder(args);

// 1. Configuración de AppSettings
ConfigurationManager configuration = builder.Configuration;
builder.Services.Configure<AppSettings>(configuration.GetSection(nameof(AppSettings)));
AppSettings appSettings = new();
configuration.GetSection(nameof(AppSettings)).Bind(appSettings);
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMapSql(builder.Configuration);


// Configuración de SecuritySettings
builder.Services.Configure<SecuritySettings>(configuration.GetSection("Security"));
SecuritySettings securitySettings = new();
configuration.GetSection("Security").Bind(securitySettings);


// 2. Configuración de servicios
builder.Services.AddRouting(options => options.LowercaseUrls = true);
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
