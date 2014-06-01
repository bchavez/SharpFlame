#!/bin/bash
# The MIT License (MIT)
#
# Copyright (c) 2014 The SharpFlame Authors.
#
# Permission is hereby granted, free of charge, to any person obtaining a copy
# of this software and associated documentation files (the "Software"), to deal
# in the Software without restriction, including without limitation the rights
# to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
# copies of the Software, and to permit persons to whom the Software is
# furnished to do so, subject to the following conditions:

# The above copyright notice and this permission notice shall be included in
# all copies or substantial portions of the Software.
#
# THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
# IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
# FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
# AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
# LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
# OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
# THE SOFTWARE.
#
# Usage:
# 
# $ ./make-gtk.sh <Debug|Release>

MY_PATH="`dirname \"$0\"`"              # relative
MY_PATH="`( cd \"$MY_PATH\" && pwd )`"  # absolutized and normalized
if [ -z "$MY_PATH" ] ; then
  # error; for some reason, the path is not accessible
  # to the script (e.g. permissions re-evaled after suid)
  exit 1  # fail
fi

MACHINE_CONFIG_FILENAME="/etc/mono/4.0/machine.config"

BUILD_CONFIG="Release"
if [ ! -z "$1" ]; then
	BUILD_CONFIG="$1"
fi

BUILD_DIR="${MY_PATH}/../source/SharpFlame.Gui.Linux/bin/${BUILD_CONFIG}"
OUTPUT_DIR="${MY_PATH}/../NAppUpdate-Feed/Gtk"
INSTALLER_DIR="${MY_PATH}/../installer/Gtk"

xbuild ${MY_PATH}/../source/Eto.Gl/Eto.Gl.csproj /p:Configuration=${BUILD_CONFIG} /t:clean
xbuild ${MY_PATH}/../source/Eto.Gl/Eto.Gl.csproj /p:Configuration=${BUILD_CONFIG}
if [ $? -gt 0 ]; then
	echo "Failure while building Eto.Gl."
	exit $?
fi

xbuild ${MY_PATH}/../source/Eto.Gl.Gtk/Eto.Gl.Gtk.csproj /p:Configuration=${BUILD_CONFIG} /t:clean
xbuild ${MY_PATH}/../source/Eto.Gl.Gtk/Eto.Gl.Gtk.csproj /p:Configuration=${BUILD_CONFIG}
if [ $? -gt 0 ]; then
	echo "Failure while building Eto.Gl.Gtk."
	exit $?
fi

xbuild ${MY_PATH}/../source/SharpFlame/SharpFlame.csproj /p:Configuration=${BUILD_CONFIG} /t:clean
xbuild ${MY_PATH}/../source/SharpFlame/SharpFlame.csproj /p:Configuration=${BUILD_CONFIG}
if [ $? -gt 0 ]; then
	echo "Failure while building SharpFlame."
	exit $?
fi

xbuild ${MY_PATH}/../source/SharpFlame.Core/SharpFlame.Core.csproj /p:Configuration=${BUILD_CONFIG} /t:clean
xbuild ${MY_PATH}/../source/SharpFlame.Core/SharpFlame.Core.csproj /p:Configuration=${BUILD_CONFIG}
if [ $? -gt 0 ]; then
	echo "Failure while building SharpFlame.Core."
	exit $?
fi

rm -f ${BUILD_DIR}/*
xbuild ${MY_PATH}/../source/SharpFlame.Gui.Linux/SharpFlame.Gui.Linux.csproj /p:Configuration=${BUILD_CONFIG} /t:clean
xbuild ${MY_PATH}/../source/SharpFlame.Gui.Linux/SharpFlame.Gui.Linux.csproj /p:Configuration=${BUILD_CONFIG}
if [ $? -gt 0 ]; then
	echo "Failure while building SharpFlame.Gui.Linux."
	exit $?
fi

# Get the version.
VERSION=$(${MY_PATH}/../tools/FileVersion/FileVersion ${BUILD_DIR}/SharpFlame.Gui.Linux.exe)

# Make a bundle of the resulting .exe
mkdir -p "${OUTPUT_DIR}"
mkbundle -z -L ${BUILD_DIR}/ -o ${OUTPUT_DIR}/SharpFlame ${BUILD_DIR}/SharpFlame.Gui.Linux.exe ${BUILD_DIR}/*.dll --machine-config ${MACHINE_CONFIG_FILENAME}
chmod +x ${OUTPUT_DIR}/SharpFlame

# Update the UpdateFeed.xml checksums.
${MY_PATH}/../tools/feedChecksumUpdater.py -d ${OUTPUT_DIR}
if [ $? -gt 0 ]; then
	echo "Failure while updating the feeds checksums."
	exit $?
fi

# Copy the Result to makeself and make a SFX
mkdir -p "${INSTALLER_DIR}"
cp ${OUTPUT_DIR}/SharpFlame ${MY_PATH}/../pkg/makeself/
makeself ${MY_PATH}/../pkg/makeself ${INSTALLER_DIR}/SharpFlame-${VERSION}.run "SFX installer for SME-Rauchfrei" ./install.sh
if [ $? -gt 0 ]; then
	echo "Failure while creating the installer."
	exit $?
fi

exit $?