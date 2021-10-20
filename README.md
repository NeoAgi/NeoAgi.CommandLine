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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class Program
{
    public static void Main(string[] args)
    {
        try
        {
            CreateHostBuilder(args).Build().Run();
        }
        catch (CommandLineOptionParseException ex)
        {
            foreach(var option in ex.OptionsWithErrors)
            {
                Console.WriteLine($"{option.Option.FriendlyName} - {option.Reason.ToString()}");
            }
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(configuration =>
            {
                configuration.Sources.Clear();
                configuration.AddJsonFile("appsettings.json", optional: false);
                configuration.AddOpts<PrunerConfig>(args, "AppSettings", outputStream: Console.Out);
                // Note: outputStream is only required if capturing the output of the parser is desired
            })
            .ConfigureServices((hostContext, services) =>
            {
                services.Configure<ConfigType>(hostContext.Configuration.GetSection("AppSettings"));
                services.AddHostedService<Worker>();
            });
}
```

Implementaiton Details can be found at [NeoAgi.CommandLine.Extensions.Configuration](https://github.com/NeoAgi/NeoAgi.CommandLine/tree/main/NeoAgi.CommandLine.Extensions.Configuration).

[NeoAgi.CommandLine](https://www.nuget.org/packages/NeoAgi.CommandLine/) Package is hosted on [nuget.org](https://www.nuget.org/).