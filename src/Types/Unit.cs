using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.Wasm;

namespace DotNetCoreFunctional;

public struct Unit()
{
    public static Unit Value => new();
}
