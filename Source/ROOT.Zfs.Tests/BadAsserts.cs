using System;
using System.Threading.Tasks;

namespace ROOT.Zfs.Tests;

public static class BadAsserts
{
    [AssertionMethod] // Mark this as an assertion method
    public static Task<T> DoesNotThrow<T>(Func<Task<T>> action)
    {
        return action();
    }
}
[AttributeUsage(AttributeTargets.Method)]
public class AssertionMethodAttribute : Attribute;
