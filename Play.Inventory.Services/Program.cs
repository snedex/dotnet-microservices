using Play.Common;
using Play.Common.Identity;
using Play.Common.MassTransit;
using Play.Inventory.Services.Entites;

const string AllowedOriginSetting = "AllowedOrigin";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddMongo()
    .AddMongoRepository<InventoryItem>("inventoryItems")
    .AddMongoRepository<CatalogItem>("catalogItems")
    .AddMassTransitWithRabbitMQ()
    .AddJwtBearerAuthentication();

//Refactored for reference
//HttpClientSetup.Configure(builder.Services);

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
