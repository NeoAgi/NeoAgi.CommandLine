NeoAgi.CommandLine
==================

Emulation of CommandLineParser intending to work more at home in a GenericHost World

Default Behvior
===============

The following is all that is necessary to parse options or throw a usage diagram:

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

`T` can be any type with a constructor.  Any property decorated with (OptionAttribute)[OptionAttribute.cs] will be parsed according to the following semantics:

* Loop through all `string`s in `args[]`
* If the string begins with `-` use the next ordnal as the value
* Properties marked as `Required` must have a value present
* Optional values can be defaulted by setting the default initializer on the Property (e.g. `public int MaxAge { get; set; } = 99;`)

See [OptionManager.Parse(string[] arr)](OptionManager.cs) for further information.

[GetOps.cs](GetOps.cs) will raise Exceptions if help is requested (e.g. `--help` is the first argument) or if an Option cannot be parsed according to the constraints provided.  

Help
----

To display the help without parsing args, provide `--help` as the first parameter.  This will throw an exception of type [RaiseHelpException](Exceptions/RaiseHelpException.cs)

Capture Parse Output
====================

Output from the parser may be captured by providing an optional TextWriter to GetOpts<T>():

```csharp
ProgramArguments opts = args.GetOps<ProgramArguments>(Console.Out);
```

An exception will be raised to capture control flow.

Handling Parse Errors
=====================

Parse errors may not require the program to end.  Errors encountered in parsing are provided by throwing an exception of type [CommandLineOptionParseException](Exceptions/CommandLineOptionParseException.cs):

```csharp
try
{
    T opts = args.GetOps<T>();
}
catch (CommandLineOptionParseException ex)
{
    foreach(OptionAttributeError error in ex.OptionsWithErrors)
    {
        Console.WriteLine($"{error.Option.FriendlyName} - {error.Reason.ToString()}");
    }
 }
 ```
