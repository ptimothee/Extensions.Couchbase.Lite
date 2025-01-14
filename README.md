# Extensions.Couchbase.Lite.Sync

## Features
Abstaction over the Couchbase.Lite package database
- Leverages dependency injection to provide a more modern approach to configure a Couchbase.Lite database with associated sync gateway endpoint(s).
- Simplifies the authentication process when connecting to a sync gateway endpoint.
- Provides convenient operations to reconfigure the sync replication settings dynamically.

## Getting Started

### Prerequisites

1.  This package is available on NuGet.
To install the package you can use the integrated package manager of your IDE, the .NET CLI, or reference the package directly in your project file.

```cmd
    dotnet add package Codemancer.Extensions.Couchbase.Lite
```

2.  For data replication features please note that a Couchbase Sync Gateway is required. For a detailed tutorial on setting up a Couchbase Sync Gateway, please refer to the [official Couchbase documentation](https://docs.couchbase.com/sync-gateway/current/quickstart.html).

### Basic Usage

1. Configuring a Couchbase.Lite database with a sync gateway endpoint

You can register a Couchbase.Lite database with an associated sync gateway on an IServiceCollection as follows:

```csharp
    services.AddCouchbaseLite("my-database")
            .WithSyncGateway(new Uri("wss://{id}.apps.cloud.couchbase.com:4984/{my-endpoint}"), options =>
            {
                options.ScopeName = "{endpoint-scope}";
                options.ConfigureReplication = (username, builder) =>
                {
                    builder.LinkCollection("my-collection", []);
                };
            });
```

-	AddCouchbaseLite("my-database"): Registers a Couchbase.Lite database named "my-database".
-	WithSyncGateway(new Uri("wss://{id}.apps.cloud.couchbase.com:4984/{endpoint-name}"), options => {...}): Configures the Sync Gateway with a WebSocket URI ...
    -	options.ScopeName: Sets the scope name associated with the endpoint.
    -	options.ConfigureReplication: Configures replication settings, 
        - linking a collection named "my-collection" with an initial channel list

You can resolve an IAppService wherever services are injected via DI. Using the IAppService instance retrieve the configured named ISyncGateway as follows:

```csharp
    IAppService appService = serviceProvider.GetRequiredService<IAppService>();
    var syncGateway = appService.GetSyncGateway("{endpoint-name}");
```

#### Sync Gateway Authentication

1. Create credentials according to the auth provider(s) configured on the sync gatway endpoint.

- Using Anonymous Access

```csharp
    var credentials = Credentials.CreateAnonymous();
```

- Using Basic Authentication

```csharp
    var credentials = Credentials.CreateBasic("{username}", "{password}");
```

- Using OpenID Connect (OIDC)
```csharp
    var credentials = Credentials.CreateJwt("{jwt-id-token}");
```

2. Signing In

To sign in to the Sync Gateway using the provided credentials:

```csharp
    await syncGateway.SignInAsync(credentials);
```
Note that this will automatically kick off the replication service on all configured linked collections.

3. Signing Out

To sign out from the Sync Gateway:
```csharp
    await syncGateway.SignOutAsync();
```

Note that this terminates the replication service.

