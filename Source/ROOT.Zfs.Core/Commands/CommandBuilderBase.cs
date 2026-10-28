using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ROOT.Zfs.Core.Commands;

/// <summary>
/// Base class for command builders. Holds the binary configuration injected into all command classes.
/// </summary>
internal abstract class CommandBuilderBase
{
    protected CommandBuilderBase(IZfsBinaries binaries)
    {
        Binaries = binaries ?? throw new ArgumentNullException(nameof(binaries));
    }

    /// <summary>
    /// The binary locations used by this command builder.
    /// </summary>
    protected IZfsBinaries Binaries { get; }

    /// <summary>
    /// Creates an argument exception from validation errors.
    /// </summary>
    protected ArgumentException ToArgumentException(IList<string> errors, object args, [CallerArgumentExpression("args")] string nameOfArgs = "")
    {
        return new ArgumentException(string.Join(Environment.NewLine, errors), $"{args.GetType().Name} {nameOfArgs}");
    }
}