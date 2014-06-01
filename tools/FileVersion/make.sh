#!/bin/bash
# The MIT License (MIT)
#
# Copyright (c) 2014 Stiftung Maria Ebene
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

BUILD_DIR="${MY_PATH}/bin/${BUILD_CONFIG}"
OUTPUT_DIR="${MY_PATH}"

xbuild ${MY_PATH}/FileVersion.csproj /p:Configuration=${BUILD_CONFIG} /t:clean
xbuild ${MY_PATH}/FileVersion.csproj /p:Configuration=${BUILD_CONFIG}
if [ $? -gt 0 ]; then
	echo "Failure while building FileVersion"
	exit $?
fi

# Make a bundle of the resulting .exe
mkdir -p "${OUTPUT_DIR}"
mkbundle -z -L ${BUILD_DIR}/ -o ${OUTPUT_DIR}/FileVersion ${BUILD_DIR}/FileVersion.exe --machine-config ${MACHINE_CONFIG_FILENAME}
chmod +x ${OUTPUT_DIR}/FileVersion