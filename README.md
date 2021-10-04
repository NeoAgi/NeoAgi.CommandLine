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

See [OptionManager.Parse(string[] arr)](NeoAgi.CommandLine/OptionManager.cs) for further information.

[GetOps.cs](NeoAgi.CommandLine/GetOps.cs) will raise Exceptions if help is requested (e.g. `--help` is the first argument) or if an Option cannot be parsed according to the constraints provided.  

Help
----

To display the help without parsing args, provide `--help` as the first parameter.  

Overriding Default Behaivor
===========================

If desired the entire exit process can be intercepted with a lambda of type `Func<OptionManager, bool>` on `string[].GetOpt()`.  

```csharp
ProgramArguments opts = args.GetOps<ProgramArguments>((manager, exit) => {
    manager.PrintHelpErrors(Console.Error, manager.Errors);
    manager.PrintHelpOptions<ProgramArguments>(Console.Out);

    return false;  // Returning false will suppress the default print handler
});
```

This may be used to intercept the OptionManager just after errors have been set but before invoking the default print template allowing errors to be adjusted, inspected, or removed.  All default template components are availabile if redirection to different streams is desired. 

[OptionManager.PrintHelp<T>](NeoAgi.CommandLine/OptionManager.cs) describes the default usage used.