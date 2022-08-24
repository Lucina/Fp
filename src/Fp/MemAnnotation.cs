using System;

namespace Fp;

/// <summary>
/// Represents a memory annotation.
/// </summary>
/// <param name="Offset">Byte offset.</param>
/// <param name="Length">Byte length.</param>
/// <param name="Label">Label.</param>
/// <param name="Color">Label color.</param>
public readonly record struct MemAnnotation(int Offset, int Length, string? Label, ConsoleColor Color);
