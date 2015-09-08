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

### Screenshots
![Screenshot1](https://raw.githubusercontent.com/bchavez/SharpFlame/eto/graphics/Screenshot1.jpg)
![Screenshot1](https://raw.githubusercontent.com/bchavez/SharpFlame/eto/graphics/Screenshot2.jpg)

### Download & Install

Downloads are current snapshots of this ETO branch with Native UI. Please consider these downloads as a work in progress until we get our code base stabilized.

![Windows](https://github.com/Turbo87/Font-Awesome/raw/platform-icons/svg/windows.png) [Windows Download](http://teamcity.codebetter.com/guestAuth/repository/download/bt1245/.lastSuccessful/SharpFlame.Windows.zip)

![Linux](https://github.com/Turbo87/Font-Awesome/raw/platform-icons/svg/linux.png) [Linux Download](http://teamcity.codebetter.com/guestAuth/repository/download/bt1246/.lastSuccessful/SharpFlame.Linux.zip)

![Mac](https://github.com/Turbo87/Font-Awesome/raw/platform-icons/svg/apple.png) None Yet

Building
--------
* Download the source code.
* Compile.

####Build Status - MS Build
![MS Build Status](http://teamcity.codebetter.com/app/rest/builds/buildType:(id:bt1245)/statusIcon)
####Build Status - Mono
![MS Build Status](http://teamcity.codebetter.com/app/rest/builds/buildType:(id:bt1246)/statusIcon)

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

