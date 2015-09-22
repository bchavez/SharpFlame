[![License](https://img.shields.io/github/license/bchavez/SharpFlame.svg)](https://raw.githubusercontent.com/bchavez/SharpFlame/eto/LICENSE)
SharpFlame
======================

Project Description
-------------------
A modern [Warzone 21000](http://wz2100.net/) map editor based on the original Warzone 2100 [FlaME](https://github.com/flail/flaME) map editor, written in C#.

Status
------------
|         | ![Windows](https://github.com/Turbo87/Font-Awesome/raw/platform-icons/svg/windows.png) Windows| ![Linux](https://github.com/Turbo87/Font-Awesome/raw/platform-icons/svg/linux.png) Linux  | ![Mac](https://github.com/Turbo87/Font-Awesome/raw/platform-icons/svg/apple.png) Mac OS X |
|---------|:------:|:------:|:-------:|
|**Debug**| [![Build status](https://ci.appveyor.com/api/projects/status/729c1ka8irgaevvv/branch/eto?svg=true)](https://ci.appveyor.com/project/bchavez/sharpflame/branch/eto/artifacts) |  N/A | N/A |
|**Download**| [![Release](https://img.shields.io/github/release/bchavez/SharpFlame.svg)](https://github.com/bchavez/SharpFlame/releases) |  N/A  | N/A   |

Downloads are current snapshots of this ETO branch with Native UI. Please consider these downloads as a work in progress until we get our code base stabilized.

### Requirements
* .NET 4.0 or Mono 3.x

### Screenshots
![Screenshot1](https://raw.githubusercontent.com/bchavez/SharpFlame/eto/graphics/Screenshot1.jpg)
![Screenshot1](https://raw.githubusercontent.com/bchavez/SharpFlame/eto/graphics/Screenshot2.jpg)


Building
--------

#### For windows
* `git checkout git@github.com:bchavez/SharpFlame.git`
* `build`

The result of the build process is the executable in `source\SharpFlame.Gui.Windows\bin\Release\SharpFlame.Gui.Windows.exe`.


Contributing
--------
* Please submit [issues here](https://github.com/bchavez/SharpFlame/issues).
* All pull requests ***should*** include associated unit tests in `SharpFlame.Tests`.
* All pull requests ***should not*** break the build.

Road Map
-------

####Milestone 1
	
* Stabilize source code.
* Fix any long-standing issues from original FlaME.
* Cleanup ugly coding parts.
* Refactor and update parsing / serialization algorithms.
* Update all dependencies (OpenGL) etc.
* Remove/update obsolete method calls.
* Solidify and stabilize Linux / Mono support.

####Milestone 2
* Unit test everything for 100% code coverage (or come close).

####Milestone 3
* **DirectGames** map publishing and distribution for Warzone 2100.


##Project Structure and Descriptions

#### `source\SharpFlame`
GUI and application.

####`source\SharpFlame.Core`
Core component library for parsing format structures.

####`source\SharpFlame.Tests`
Unit tests for the application.

####`reference\*`
Original FlaME source code for reference during port to C#.




------------------------
 

Reference
---------
* [FlaME](https://github.com/flail/flaME) - Original Source Code


Created by:
[Brian Chavez](http://bchavez.bitarmory.com).
[René Kistl](http://rene.kistl.at).

---

