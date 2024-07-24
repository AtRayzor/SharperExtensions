using NetFunctional.Core;

namespace Analyzers.Sample;

[DiscriminatedUnion]
public interface IGenericThing<T>;

public readonly record struct Case1<T>(T Value) : IGenericThing<T>;

public readonly record struct Case2<T>(T Value) : IGenericThing<T>;
