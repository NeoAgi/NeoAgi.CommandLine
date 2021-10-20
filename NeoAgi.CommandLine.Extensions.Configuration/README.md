NeoAgi.CommandLine.Extensions.Configuration
===========================================

Emulation of CommandLineParser intending to work more at home in a GenericHost World

Default Behavior
===============

The following is an example to load NeoAgi.CommandLine into a Generic Host provider (Web or Console):

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

`T` must be a valid type that works with `NeoAgi.CommandLine.GetOpts<T>()`.  The current API requires two calls to propery work as expected:

1. `configuration.AddOpts<ConfigType>(args, "AppSettings")` parses `args` into the IConfiguration hierarchy using `NeoAgi.CommandLine.GetOpts<T>()` constrained by `ConfigType`.  At this point `Configuration["AppSettings:SomePropertyNameFromConfigType"]` will be accessable.
1. `services.Configure<ConfigType>(hostContext.Configuration.GetSection("AppSettings"));` injects `IOption<ConfigType>` into the DI Container using the values set into `Configuration[]` above.  

`services.Configure` is only necessary if injecting into the DI container is desired.

Configuration Examples
==============

The example above assumes the following setup:

appsettings.json
-----------------

```json
{
  "AppSettings": {
    "FileLocation": "Default",
    "Category":  "Default"
  }
}
```

ConfigType.cs
--------------

```csharp
public class ConfigType
    {
        [Option(FriendlyName = "File Location", ShortName = "l", LongName = "location", Description = "Path of the File to Parse", Required = true)]
        public string FileLocation { get; set; } = string.Empty;
        [Option(FriendlyName = "Category", ShortName = "c", LongName = "category", Description = "Name of the Category", Required = false)]
        public string Category { get; set; } = string.Empty;
    }
```

Load Order
-----------

As appsettings.json is loaded first, the IOption<ConfigType> loaded into the DI container will output `Default` for `FileLocation` if `configuration.AddOpts<ConfigType>(args, "AppSettings");` is omitted.  Once added `FileLocation` will parse the `--location` or `-l` parameter provided to the command line overriding what is stored in appsettings.json.  

This behavior is exactly like [netcore default Configuration](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-5.0#default-configuration-1).  The call to `configuration.AddOpts<T>(string[], string)` can be used as a direct replacement to `AddCommandLine(string[])`, and can be reordered to have Environment Variables or other configuration providers to override symbols loaded in if desired as long as the namespace injected into Configuration is maintained (e.g. Pay close attention to how `Configuration.GetSection("foo")` is used).