using Catalog.Service.Entities;
using Play.Common.Identity;
using Play.Common.MassTransit;
using Play.Common.MongoDB;
using Play.Common.Settings;

const string AllowedOriginSetting = "AllowedOrigin";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMongo()
    .AddMongoRepository<Item>("items")
    .AddMassTransitWithRabbitMQ()
    .AddJwtBearerAuthentication();

var serviceSettings = builder.Configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors(config => {
        config.WithOrigins(app.Configuration.GetValue(typeof(string), AllowedOriginSetting).ToString())
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
