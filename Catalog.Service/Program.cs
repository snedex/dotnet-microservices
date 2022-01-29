using Play.Common;
using Catalog.Service.Entities;
using MassTransit;
using Catalog.Service.Settings;
using MassTransit.Definition;

var builder = WebApplication.CreateBuilder(args);

var svcSettings = builder.Configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();

builder.Services.AddMongo().AddMongoRepository<Item>("items");

//Setup mass transit configuration, to refactor out later.
builder.Services.AddMassTransit(config => {
    config.UsingRabbitMq((context, configurator) => {
        var settings = builder.Configuration.GetSection(nameof(RabbitMQSettings)).Get<RabbitMQSettings>();
        configurator.Host(settings.Host);
        configurator.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter(svcSettings.ServiceName, false));
    });
});

//Start the mass transit message queue bus
builder.Services.AddMassTransitHostedService();

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
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
