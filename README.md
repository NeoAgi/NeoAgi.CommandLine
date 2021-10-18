NeoAgi.CommandLine
==================

Emulation of CommandLineParser intending to work more at home in a GenericHost World

```csharp
using NeoAgi.CommandLine;

try {
    T opts = args.GetOps<T>();
} 
catch(Exception) 
{
    // Handle failures
}
```

Implementaiton Details can be found at [NeoAgi.CommandLine](https://github.com/NeoAgi/NeoAgi.CommandLine/tree/main/NeoAgi.CommandLine).

[NeoAgi.CommandLine](https://www.nuget.org/packages/NeoAgi.CommandLine.Extensions.Configuration/) Package is hosted on [nuget.org](https://www.nuget.org/).

NeoAgi.CommandLine.Extensions.Configuration
===========================================

Generic NETCORE Host Provider for NeoAgi.CommandLine GetOps API

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(configuration =>
            {
                configuration.Sources.Clear();
                configuration.AddJsonFile("appsettings.json", optional: false);
                configuration.AddOpts<ConfigType>(args, "AppSettings");
            })
            .ConfigureServices((hostContext, services) =>
            {
                services.Configure<ConfigType>(hostContext.Configuration.GetSection("AppSettings"));
                services.AddHostedService<Worker>();
            });
}
```

Implementaiton Details can be found at [NeoAgi.CommandLine](https://github.com/NeoAgi/NeoAgi.CommandLine/tree/main/NeoAgi.CommandLine).

[NeoAgi.CommandLine](https://www.nuget.org/packages/NeoAgi.CommandLine/) Package is hosted on [nuget.org](https://www.nuget.org/).