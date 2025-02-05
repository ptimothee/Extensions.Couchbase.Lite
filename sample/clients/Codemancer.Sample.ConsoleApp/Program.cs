using Microsoft.Extensions.DependencyInjection;
using Codemancer.Extensions.Couchbase.Lite.Sync;
using Codemancer.Extensions.Couchbase.Lite.Extensions;
using Codemancer.Extensions.Couchbase.Lite.DependencyInjection;

var services = new ServiceCollection();

services.AddCouchbaseLite("my-database")
        .WithSyncGateway(new Uri("wss://{id}.apps.cloud.couchbase.com:4984/{endpoint-name}"), options =>
        {
            options.ScopeName = "{endpoint-scope}";
            options.ConfigureReplication = (builder, context) =>
            {
                builder.ReplicatorConfiguration.Continuous = true;
                builder.LinkCollection("{collection-name}", []);
            };

            options.Events.OnStatusChanged += (sender, args) =>
            {
                Console.WriteLine($"Activity :: {args.Status.Activity}");
                if (args.Status.Error != null)
                {
                    Console.WriteLine($"Error :: {args.Status.Error}");
                }
                else
                {
                    Console.WriteLine($"Progress :: Total:: {args.Status.Progress.Total} Completed:: {args.Status.Progress.Completed}");
                }
            };
        });

var serviceProvider = services.BuildServiceProvider();

var appService = serviceProvider.GetRequiredService<IAppService>();
var syncGateway = appService.GetSyncGateway("{endpoint-name}");

var credentials = Credentials.CreateJwt("{idToken}");

await syncGateway.SignInAsync(credentials);

Console.WriteLine("Press any key to terminate. ");
Console.ReadLine();

await syncGateway.SignOutAsync();
await syncGateway.DisposeAsync();

