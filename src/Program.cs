using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MyProfile;
using MyProfile.Services.Github;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Obaki.LocalStorageCache;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddHttpClient<IGithubHttpClient, GithubHttpClient>()
                .AddTransientHttpErrorPolicy(policyBuilder =>
                    policyBuilder.WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), 3), (exception, timeSpan, retryCount, context) =>
                    {
                        Console.WriteLine(exception);
                    }));

builder.Services.AddLocalStorageCacheAsSingleton();
if (builder.HostEnvironment.Environment == "Development")
{
    builder.Logging.SetMinimumLevel(LogLevel.Debug);
}
else
{
    builder.Logging.SetMinimumLevel(LogLevel.None);
}

await builder.Build().RunAsync();
