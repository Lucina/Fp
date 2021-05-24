[![.NET](https://github.com/riina/Fp/actions/workflows/dotnet.yml/badge.svg)](https://github.com/riina/Fp/actions/workflows/dotnet.yml)
[![NuGet](https://img.shields.io/nuget/v/Fp.svg)](https://www.nuget.org/packages/Fp/)

## File processing

| Package                | Release |
|------------------------|---------|
| `Fp`           | [![NuGet](https://img.shields.io/nuget/v/Fp.svg)](https://www.nuget.org/packages/Fp/)|
| `Fp.Plus`    | [![NuGet](https://img.shields.io/nuget/v/Fp.Plus.svg)](https://www.nuget.org/packages/Fp.Plus/)|
| `Fp.Templates` | [![NuGet](https://img.shields.io/nuget/v/Fp.Templates.svg)](https://www.nuget.org/packages/Fp.Templates/) |

[Documentation](https://riina.github.io/Fp) | [Samples](samples)

### Libraries
* [Fp](src/Fp): Base file processing library
    - Minimum API: .NET Standard 2.0
* [Fp.Plus](src/Fp.Plus): Extension library
    - Minimum API: .NET Standard 2.0
### Scripting
* [fpx](src/fpx): Script execution program (thin wrapper of [dotnet-script](https://github.com/filipw/dotnet-script))
    - Requires [.NET 5 SDK](https://get.dot.net/) for execution
* [Dereliction](src/Dereliction): Basic Avalonia GUI script editor / testing program
    - Requires [.NET 5 SDK](https://get.dot.net/) for execution