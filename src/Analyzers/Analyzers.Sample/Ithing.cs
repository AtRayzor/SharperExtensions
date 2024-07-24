using NetFunctional.Core;

namespace Analyzers.Sample;

[DiscriminatedUnion]
public interface IThing;

public readonly record struct Number(int Value) : IThing;

public readonly record struct Letter(char Char) : IThing;

public readonly record struct Animal(string Name) : IThing;