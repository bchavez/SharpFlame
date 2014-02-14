SharpFlame
======================
----------------------

Project Description
-------------------
A modern [Warzone 21000](http://wz2100.net/) map editor based on the original Warzone 2100 [FlaME](https://github.com/flail/flaME) map editor, written in C#.

### License
* [MIT License](https://github.com/bchavez/Dwolla/blob/master/LICENSE)

### Requirements
* .NET 4.0 or Mono 3.x

### Download & Install
Continuous Integration builds soon.

Building
--------
* Download the source code.
* Compile.

Build Status: ![Build Status](http://www.bitarmory.com/Main/BuildStatus.ashx?Project=SharpFlame)

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


Created by [Brian Chavez](http://bchavez.bitarmory.com).

---

