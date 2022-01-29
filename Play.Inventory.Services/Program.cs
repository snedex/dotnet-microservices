using System.ComponentModel.DataAnnotations;
using Play.Common.MongoDB;
using Play.Inventory.Services.Clients;
using Play.Inventory.Services.Entites;
using Polly;
using Polly.Timeout;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddMongo()
    .AddMongoRepository<InventoryItem>("inventoryItems");

//Jitter for retry logic
Random jitter = new Random();

builder.Services.AddHttpClient<CatalogClient>(c => {
    c.BaseAddress = new Uri("https://localhost:7036/api/");
})
//Order matters here
//This configures the exponential back off per retry
.AddTransientHttpErrorPolicy(p => p.Or<TimeoutRejectedException>().WaitAndRetryAsync(
    5, 
    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) + TimeSpan.FromMilliseconds(jitter.Next(1, 1000)), 
    onRetry: (outcome, timespan, retryattempt) => {
        Console.WriteLine($"warn: Delaying for {timespan.TotalSeconds}, then retry {retryattempt}");
    }
))
//Adding a circuit breaker to avoid overwhelming the broken service and hitting exhaustion
.AddTransientHttpErrorPolicy(p => p.Or<TimeoutRejectedException>().CircuitBreakerAsync(
    3, 
    TimeSpan.FromSeconds(15),
    onBreak: (outcome, timespan) => {
        Console.WriteLine($"warn: Breaking circuit for {timespan.TotalSeconds}s");
    },
    onReset: () => {
        Console.WriteLine($"warn: Restoring connections");
    }
))
//Default timeout policy
.AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(1));

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
