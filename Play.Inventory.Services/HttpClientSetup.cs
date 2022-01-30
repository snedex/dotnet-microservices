using Play.Inventory.Services.Clients;
using Polly;
using Polly.Timeout;

namespace Play.Inventory.Services
{
    //Class to hold the configuration of the Polly backoff for HttpClient
    //For reference as this is now replaced with rabbitmq
    public class HttpClientSetup
    {
        internal static void Configure(IServiceCollection services)
        {
            //Jitter for retry logic
            Random jitter = new Random();

            services.AddHttpClient<CatalogClient>(c =>
            {
                c.BaseAddress = new Uri("https://localhost:7036/api/");
            })
            //Order matters here
            //This configures the exponential back off per retry
            .AddTransientHttpErrorPolicy(p => p.Or<TimeoutRejectedException>().WaitAndRetryAsync(
                5,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) + TimeSpan.FromMilliseconds(jitter.Next(1, 1000)),
                onRetry: (outcome, timespan, retryattempt) =>
                {
                    Console.WriteLine($"warn: Delaying for {timespan.TotalSeconds}, then retry {retryattempt}");
                }
            ))
            //Adding a circuit breaker to avoid overwhelming the broken service and hitting exhaustion
            .AddTransientHttpErrorPolicy(p => p.Or<TimeoutRejectedException>().CircuitBreakerAsync(
                3,
                TimeSpan.FromSeconds(15),
                onBreak: (outcome, timespan) =>
                {
                    Console.WriteLine($"warn: Breaking circuit for {timespan.TotalSeconds}s");
                },
                onReset: () =>
                {
                    Console.WriteLine($"warn: Restoring connections");
                }
            ))
            //Default timeout policy
            .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(1));
        }
    }
}