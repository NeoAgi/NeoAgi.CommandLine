NeoAgi.CommandLine
==================

Emulation of CommandLineParser intending to work more at home in a GenericHost World

Default Behvior
===============

The following is all that is necessary to parse options or throw a usage diagram:

```csharp
using NeoAgi.CommandLine;

T opts = args.GetOps<T>();
```

`T` can be any type with a constructor.  Any property decorated with (OptionAttribute)[OptionAttribute.cs] will be parsed according to the following semantics:

* Loop through all `string`s in `args[]`
* If the string begins with `-` use the next ordnal as the value
* Properties marked as `Required` must have a value present
* Optional values can be defaulted by setting the default initializer on the Property (e.g. `public int MaxAge { get; set; } = 99;`)

See [OptionManager.Parse(string[] arr)](NeoAgi.CommandLine/OptionManager.cs#L17) for further information.

[GetOps.cs](NeoAgi.CommandLine/GetOps.cs) handles the default interpertation of [ExitDirective.cs](NeoAgi.CommandLine/ExitDirective.cs) which returns an exit code of 1 and prints the default usage help.

Help
----

To display the help without parsing args, provide `--help` as the first parameter.  

Overriding Default Behaivor
===========================

If desired the entire exit process can be intercepted with a lambda of type `Action<OptionManager, ExitDirective>` on `string[].GetOpt()`.  

```csharp
ProgramArguments opts = args.GetOps<ProgramArguments>((manager, exit) => {
    exit.ProcessHelp = false;
    exit.ExitCode = int.MaxValue;

    manager.PrintHelpErrors(Console.Error, manager.Errors);
    manager.PrintHelpOptions<ProgramArguments>(Console.Out);
});
```

Pay special attention of suppressing the default Help hanlder by setting `exit.ProcessHelp` to `false` if manipulating the output.  Otherwise the default usage will fire in addition to other modifications.  

[OptionManager.PrintHelp<T>](NeoAgi.CommandLine/OptionManager.cs#L67) describes the default usage used.