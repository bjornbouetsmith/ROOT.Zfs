using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace ROOT.Zfs.Tests;

public static class BadAsserts
{
    [AssertionMethod] 
    public static void DoesNotThrow(Action action)
    {
        try
        {
            action();
        }
        catch (Exception ex)
        {
            // Throw a failure signal recognized by MSTest
            throw new AssertFailedException($"Expected no exception, but got: {ex.Message}");
        }
    }

    [AssertionMethod] // Mark this as an assertion method
    public static T DoesNotThrow<T>(Func<T> action)
    {
        try
        {
            return action();
        }
        catch (Exception ex)
        {
            // Throw a failure signal recognized by MSTest
            throw new AssertFailedException($"Expected no exception, but got: {ex.Message}");
        }
    }
    [AssertionMethod] // Mark this as an assertion method
    public static Task<T> DoesNotThrow<T>(Func<Task<T>> action)
    {
        try
        {
            return action();
        }
        catch (Exception ex)
        {
            // Throw a failure signal recognized by MSTest
            throw new AssertFailedException($"Expected no exception, but got: {ex.Message}");
        }
    }
}

// The key is that the class name must be AssertionMethodAttribute
public class AssertionMethodAttribute : Attribute { }
