# PhantomBreaker
Decode / extract assets from Phantom Breaker series

Code supports .NET Standard 2.0 (and builds to executable in .NET Framework 4.7.2 and .NET 6),
build instructions are available [here](../../../README.md#building-net).

## Basic usage (on Windows)
Just drag a folder of stuff onto `PhantomBreaker.exe`

Output will be saved in a folder next to the original folder, named "fp_output"

## Advanced usage
Use in command prompt / terminal according to following format (`<>` = mandatory, `[]` = optional)
- `<path to PhantomBreaker program> <inputs...> [-o <outputfolder>] [-- [options]]`
  - `inputs...`: paths to input files or folders, separated by spaces.
  - `outputfolder`: path to output folder (auto-created if it doesn't exist).
    - Default `fp_output`
  - `options`: Any of the below options.

## Options

`-g`: Force output only the raw content files (override detection as a
character pack or as containing graphics)

`-c`: Force attempting to extract a character pack's graphics

`-t`: Force attempting to extract standard (non-palette) graphics

## Details

This should work flawlessly on all files, though false positives on
the graphics conversion are possible (the files don't have identifiers).

I haven't figured out how the third graphic format works. It might have
something to do with vfx, also could be animation.

This processor attempts extraction on all files it receives as input.

This processor uses filenames to determine whether to automatically attempt character / RLE graphics conversion. I only have access to names from PBE and PB:BG. The behaviour can be overridden with the above options.
