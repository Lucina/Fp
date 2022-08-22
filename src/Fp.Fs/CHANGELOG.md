# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [0.26.0] 2022-1-30

### Added
- Add FileProcessorInfo-accepting Run overloads for FsFormatSingle/FsFormatMulti/Fs

### Changed
- Updated Fp dependency to 0.26.0
- Arg setup now happens in `Processor.Prepare(ProcessorConfiguration?)`
- Format processor wrappers now run cleanup on the wrapped processors
- FsProcessor is now derived from FileProcessor
- Adjusted FsProcessor members (moved commons to FileProcessor, Source removed, supplanted by Info)

### Removed
- FsProcessorInfo (moved to Fp as FileProcessorInfo)

## [0.25.0] 2022-1-18
- Initial version
