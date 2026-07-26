using System;
using System.IO;

namespace Percolator.Analyzers.Tests;

internal static class Sample
{
    public static string For<T>() => For(typeof(T));
    public static string For(Type type) =>
        File.ReadAllText(Path.Combine("SampleBoilerplate", $"{type.Name.Split('`')[0]}.cs"));
    
}
