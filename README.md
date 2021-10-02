NeoAgi.CommandLine
==================

Emulation of CommandLineParser intending to work more at home in a GenericHost World

Default Behvior
===============

The following is all that is necessary to parse options or throw a usage diagram:

```csharp
ProgramArguments opts = args.GetOps<ProgramArguments>();
```

[GetOps.cs](GetOps.cs) handles the default interpertation of [ExitDirective.cs](ExitDirective.cs) which returns an exit code of 1 and prints the default usage help.

Overriding Default Behaivor
===========================

If desired the entire exit process can be intercepted with a lambda of type `Action<OptionManager, OptionAttribute, ExitDirective>` on `string[].GetOpt()`.  

```csharp
ProgramArguments opts = args.GetOps<ProgramArguments>((manager, option, exit) => {
    exit.ProcessHelp = false;
    exit.ExitCode = int.MaxValue;

    manager.PrintHelpErrors(Console.Error, option);
    manager.PrintHelpOptions<ProgramArguments>(Console.Out);
});
```

Pay special attention of suppressing the default Help hanlder by setting `exit.ProcessHelp` to `false` if manipulating the output.  Otherwise the default usage will fire in addition to other modifications.  

[OptionManager.PrintHelp<T>](OptionManager.cs#L61) describes the default usage used.