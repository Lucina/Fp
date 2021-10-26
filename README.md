[![.NET](https://github.com/riina/Fp/actions/workflows/dotnet.yml/badge.svg)](https://github.com/riina/Fp/actions/workflows/dotnet.yml)
[![NuGet](https://img.shields.io/nuget/v/Fp.svg)](https://www.nuget.org/packages/Fp/)

## File processing

| Package                | Release |
|------------------------|---------|
| `Fp`           | [![NuGet](https://img.shields.io/nuget/v/Fp.svg)](https://www.nuget.org/packages/Fp/)|
| `Fp.Plus`    | [![NuGet](https://img.shields.io/nuget/v/Fp.Plus.svg)](https://www.nuget.org/packages/Fp.Plus/)|
| `Fp.Templates` | [![NuGet](https://img.shields.io/nuget/v/Fp.Templates.svg)](https://www.nuget.org/packages/Fp.Templates/) |

[Documentation](https://riina.github.io/Fp) | [Samples](samples)

Fp is designed as a framework for implementing concise data format extractors, primarily for archive containers and 2D rasters.

Many utilities for working with primitive data (including endianness-dependent I/O) are provided. See the [Samples](samples) for more.

### Libraries
* [Fp](src/Fp): Base file processing library
    - Minimum API: .NET Standard 2.0
* [Fp.Plus](src/Fp.Plus): Extension library (e.g. RGBA32 image (through ImageSharp) / PCM WAVE output)
    - Minimum API: .NET Standard 2.0
### Scripting
* [fpx](src/fpx): Script execution program (thin wrapper of [dotnet-script](https://github.com/filipw/dotnet-script))
    - Requires [.NET 5 SDK](https://get.dot.net/) for execution
* [Dereliction](src/Dereliction): Basic Avalonia GUI script editor / testing program
    - Requires [.NET 5 SDK](https://get.dot.net/) for execution

### Details

Processors are a unit worker derived from `Fp.Processor` or one of its descendants that are called to operate on each applicable (by default, based on extension) file path, optionally opening a stream and generating console output or file artifacts. If written to only use compatible APIs (e.g. working with the `FileSystemSource` APIs and not directly using `System.IO.File` etc.), processors can be used to work with synthetic filesystems (e.g. directly working on files in a zip file) and more generally be used to efficiently interact with any binary data regardless of source.

A scripting API also exists that mirrors much of the `Processor` functionality in static, single-context form (multithreading not supported, unlike standard `Processor`s).

Programs intended to extract from a set of files / folders (recursively) to a destination can utilize the default `Fp.Processor.Run<T>` function where `T` is a user-created processor class. All file I/O is managed by the library, the processor will be automatically fed input file paths and can opt to open these files / create new files (automatically placed in the output directory) via the `Processor` class APIs.

In the case of the `Fp.DataProcessor` class, processors are designed to generate a `System.Collections.Generic.IEnumerable<Fp.Data>` where `Fp.Data` represents an arbitrary content result with an associated filename, such as `Fp.Plus.Images.Rgba32Data` for an RGBA 32-bit image, or `Fp.BufferData<byte>` for an arbitrary byte buffer. This can be a useful pattern for designing an arbitrary import scheme for software that needs to be able to work with new file types without unnecessary pipeline changes.
