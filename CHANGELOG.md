# Changelog

## [0.5.5] - 2024-10-02

### Added
- Added `About Paramecium` window.

### Changed
- Updated `README.md`.

### Fixed
- Fixed a bug that could cause the file name in the `Save As` dialog to be the wrong value.

## [0.5.4] - 2024-10-02

### Added
- Added 3 new brain node types. (`Input_PheromoneRedAvgAngle` `Input_PheromoneGreenAvgAngle` `Input_PheromoneBlueAvgAngle`)

### Fixed
- Fixed an incorrect layout in the `Soup Settings` window.

## [0.5.3] - 2024-10-02

### Added
- Diet added. Children cannot efficiently eat anything other than the same food that their parents ate.

### Fixed
- Fixed a bug in which animals were only scanning an area of 3x3 tiles when they should have been scanning an area of 5x5 tiles when determining predation on surrounding plants and animals.

## [0.5.2] - 2024-10-02

### Added
- Added the ability to import and export soup settings.

### Changed
- Updated `README.md`.

## [0.5.1] - 2024-10-02

### Fixed
- Fixed a bug that the color display of output type nodes on the brain diagram was sometimes incorrect.

## [0.5.0] - 2024-10-01

### Added
- Soup settings can now be edited when creating soups.
- Soup settings can now be edited while the soup is running.

### Changed
- When serializing Soup data to JSON, enum is now serialized to a string instead of a number.
- Soup is no longer automatically created when the program is started.

## [0.5.0 indev-7] - 2024-10-01

### Changed
- Soup environment settings have been separated into a `SoupSettings` class.
- Improved algorithm for filling isolated areas with walls.

## [0.5.0 indev-6] - 2024-09-30

### Added
- Added `Object Inspector`.

### Changed
- When generating walls in the soup, areas isolated from the outside world are now filled with walls.

### Fixed
- Fixed a bug that could cause connections to be duplicated when mutating.

## [0.5.0 indev-5] - 2024-09-30

### Added
- Added the ability to create and delete walls.
- Added the ability to move plants and animals.
- When closing a program or attempting to load another soup while the currently running soup is not saved, the program now asks if the current soup should be saved.

### Changed
- Brain Diagram now displays node index and types.
- Improved brain mutations in animals.
- Element consumption rate of animals now changes depending on the action they are performing.

### Fixed
- Fixed a rare crash related to `SoupView` rendering.
- Fixed a bug that caused nodes with no connection in rare cases when animals mutated.

## [0.5.0 indev-4] - 2024-09-29

### Added
- Added pheromones.
- An information overlay now appears when an object is selected.

### Changed
- Changed the version number in `CHANGELOG.md` to match the notation in the program.
- Improved brain mutations in animals.

### Fixed
- Fixed bugs with animal vision.

## [0.5.0 indev-3] - 2024-09-29

### Added
- Added animal brain mutations.

### Changed
- The animal neural network was separated from the `Animal` class into the `Brain` class and its related classes.

## [0.5.0 indev-2] - 2024-09-28

### Added
- Animals are now moved by neural networks.
- Added the ability to select objects and the ability to make the camera follow the selected object.
- Added the ability to save and load soups.
- Added program icon.

## [0.5.0 indev-1] - 2024-09-27

### Added
- Re-created `src/Paramecium` directory.
- Re-created Visual Studio project.
- Added `Soup`, `Plant`, and `Animal`.
- Soup is now rendered in the `SoupView` of `FormMain`.

## [0.5.0 indev-0] - 2024-09-27

### Added
- Added `CHANGELOG.md`.

### Changed
- Updated `README.md`.

### Deleted
- Temporarily deleted `src/Paramecium` directory.
