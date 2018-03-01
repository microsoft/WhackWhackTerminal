# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [1.0] - 2018-3-5
### Added
- Service API
- Ability to set shell path

### Changed
- Terminal directory now changes whenever the solution changes

### Fixed
- Alt key combos no longer print character to terminal
- Text now correctly themes on all color configurations

## [0.2.8] - 2017-9-22
### Added
- Option to pass command line parameters to terminal.

### Changed
- Extension no longer turns off local machine lockdown for the entire process.

## [0.2.7] - 2017-9-15
### Added
- Font options in the CSS template.

### Changed
- Ctrl+O, Ctrl+P, and Ctrl+Tab are now handled by VS.

### Fixed
- Goto error now correctly focuses the editor.

## [0.2.6] - 2017-8-11
### Fixed
- Fixed problem with relative paths and ~ in path name.

## [0.2.5] - 2017-8-10
### Added
- Options menu, user can select default shell and choose a custom CSS file.
- Error links, build errors can be clicked to take you to source location.

### Fixed
- Terminal was not focusing when the window gained focus.

## 0.2.4 - 2017-8-7
### Changed
- No longer rely on internal APIs.

### Fixed
- Fix issue where terminal window would not correctly capture focus.

## 0.2.3 - 2017-7-29
### Fixed
- Fix resize that previous release broke.

## 0.2.2 - 2017-7-29
### Added
- Copy and paste support (Ctrl+C/Ctrl+V and right-click).

### Fixed
- Exit no longer zombies the terminal, exiting the terminal restarts.
- Reduced massive extension size.
- Opening two instances no longer zombies the shell.

## 0.2.1 - 2017-7-25
### Changed
- Cursor now blinks by default.

### Fixed
- Unthemed cursor.

# 0.2.0 - 2017-7-25
- Initial MVP release


[1.0]: https://github.com/Microsoft/WhackWhackTerminal/compare/v0.2.8...v1.0
[0.2.8]:https://github.com/Microsoft/WhackWhackTerminal/compare/v0.2.7...v0.2.8
[0.2.7]:https://github.com/Microsoft/WhackWhackTerminal/compare/v0.2.6...v0.2.7
[0.2.6]:https://github.com/Microsoft/WhackWhackTerminal/compare/v0.2.5...v0.2.6
[0.2.5]:https://github.com/Microsoft/WhackWhackTerminal/compare/v0.2.4...v0.2.5