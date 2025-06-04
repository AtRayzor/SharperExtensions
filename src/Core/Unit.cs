namespace SharperExtensions;

/// <summary>
/// Represents a unit type with no meaningful value, similar to void in other languages.
/// </summary>
/// <remarks>
/// The Unit type is typically used to represent operations with no return value or as a placeholder in generic contexts.
/// </remarks>
public struct Unit
{
    /// <summary>
    /// Gets a static instance of the <see cref="Unit"/> type.
    /// </summary>
    /// <remarks>
    /// Provides a convenient way to create a default <see cref="Unit"/> value without instantiating a new object.
    /// </remarks>
    public static Unit Value => new();
}