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

MACHINE_CONFIG_FILENAME="/opt/mono/etc/mono/4.0/machine.config"

BUILD_CONFIG="Release"
if [ ! -z "$1" ]; then
	BUILD_CONFIG="$1"
fi

BUILD_DIR="${MY_PATH}/../source/SharpFlame.Gui.Windows/bin/${BUILD_CONFIG}"
OUTPUT_DIR="${MY_PATH}/../NAppUpdate-Feed/Winforms"
INSTALLER_DIR="${MY_PATH}/../installer/Winforms"

/opt/mono/bin/xbuild ${MY_PATH}/../source/Eto.Gl/Eto.Gl.csproj /p:Configuration=${BUILD_CONFIG} /t:clean
/opt/mono/bin/xbuild ${MY_PATH}/../source/Eto.Gl/Eto.Gl.csproj /p:Configuration=${BUILD_CONFIG}
if [ $? -gt 0 ]; then
	echo "Failure while building Eto.Gl."
	exit $?
fi

/opt/mono/bin/xbuild ${MY_PATH}/../source/Eto.Gl.Windows/Eto.Gl.Windows.csproj /p:Configuration=${BUILD_CONFIG} /t:clean
/opt/mono/bin/xbuild ${MY_PATH}/../source/Eto.Gl.Windows/Eto.Gl.Windows.csproj /p:Configuration=${BUILD_CONFIG}
if [ $? -gt 0 ]; then
	echo "Failure while building Eto.Gl.Windows."
	exit $?
fi

/opt/mono/bin/xbuild ${MY_PATH}/../source/SharpFlame/SharpFlame.csproj /p:Configuration=${BUILD_CONFIG} /t:clean
/opt/mono/bin/xbuild ${MY_PATH}/../source/SharpFlame/SharpFlame.csproj /p:Configuration=${BUILD_CONFIG}
if [ $? -gt 0 ]; then
	echo "Failure while building SharpFlame."
	exit $?
fi

/opt/mono/bin/xbuild ${MY_PATH}/../source/SharpFlame.Core/SharpFlame.Core.csproj /p:Configuration=${BUILD_CONFIG} /t:clean
/opt/mono/bin/xbuild ${MY_PATH}/../source/SharpFlame.Core/SharpFlame.Core.csproj /p:Configuration=${BUILD_CONFIG}
if [ $? -gt 0 ]; then
	echo "Failure while building SharpFlame.Core."
	exit $?
fi

rm -f ${BUILD_DIR}/*
/opt/mono/bin/xbuild ${MY_PATH}/../source/SharpFlame.Gui.Windows/SharpFlame.Gui.Windows.csproj /p:Configuration=${BUILD_CONFIG} /t:clean
/opt/mono/bin/xbuild ${MY_PATH}/../source/SharpFlame.Gui.Windows/SharpFlame.Gui.Windows.csproj /p:Configuration=${BUILD_CONFIG}
if [ $? -gt 0 ]; then
	echo "Failure while building SharpFlame.Gui.Windows."
	exit $?
fi

# A quick hack to add the ApplicationIcon which doesn't work with xbuild.
cd ${MY_PATH}/../source/SharpFlame.Gui.Windows
/opt/mono/bin/mcs /noconfig /debug:pdbonly /optimize+ /out:obj/${BUILD_CONFIG}/SharpFlame.Gui.Windows.exe /resource:obj/${BUILD_CONFIG}/SharpFlame.Gui.Windows.Properties.Resources.resources \
	../SharpFlame.Core/GlobalAssemblyInfo.cs EtoCustom/WinPanelHandler.cs Startup.cs Properties/AssemblyInfo.cs Properties/Resources.Designer.cs Properties/Settings.Designer.cs obj/${BUILD_CONFIG}/.NETFramework,Version=v4.0.AssemblyAttribute.cs \
	/target:winexe /define:TRACE /nostdlib /platform:x86 \
	/reference:../packages/Appccelerate.EventBroker.2.0.84/lib/Net40/Appccelerate.EventBroker.dll \
	/reference:../packages/Appccelerate.Fundamentals.1.0.29/lib/Net40/Appccelerate.Fundamentals.dll \
	/reference:../packages/Ninject.3.0.1.10/lib/net40/Ninject.dll \
	/reference:../packages/Ninject.Extensions.ContextPreservation.3.0.0.8/lib/net40/Ninject.Extensions.ContextPreservation.dll \
	/reference:../packages/Ninject.Extensions.Logging.3.0.1.0/lib/net40/Ninject.Extensions.Logging.dll \
	/reference:../packages/Ninject.Extensions.Logging.nlog2.3.0.1.0/lib/net40/Ninject.Extensions.Logging.NLog2.dll \
	/reference:../packages/Ninject.Extensions.NamedScope.3.0.0.5/lib/net40/Ninject.Extensions.NamedScope.dll \
	/reference:../packages/NLog.2.1.0/lib/net40/NLog.dll \
	/reference:/opt/mono/lib/mono/4.0/System.dll \
	/reference:/opt/mono/lib/mono/4.0/Microsoft.CSharp.dll \
	/reference:../packages/OpenTK.1.1.1508.5724/lib/NET40/OpenTK.dll \
	/reference:/opt/mono/lib/mono/4.0/System.Drawing.dll \
	/reference:/opt/mono/lib/mono/4.0/System.Windows.Forms.dll \
	/reference:../packages/Eto-20140420/net40/Eto.dll \
	/reference:../packages/Eto-20140420/net40/Eto.Platform.Windows.dll \
	/reference:/opt/mono/lib/mono/4.0/System.Core.dll \
	/reference:${MY_PATH}/../source/SharpFlame.Core/bin/${BUILD_CONFIG}/SharpFlame.Core.dll \
	/reference:${MY_PATH}/../source/Eto.Gl/bin/${BUILD_CONFIG}/Eto.Gl.dll \
	/reference:${MY_PATH}/../source/Eto.Gl.Windows/bin/${BUILD_CONFIG}/Eto.Gl.Windows.dll \
	/reference:${MY_PATH}/../source/SharpFlame/bin/${BUILD_CONFIG}/SharpFlame.dll \
	/reference:/opt/mono/lib/mono/4.0/mscorlib.dll \
	/warn:4  /win32icon:${MY_PATH}/../source/SharpFlame/flaME.ico
if [ $? -gt 0 ]; then
	echo "Failure while compiling the .exe with the icon."
	exit $?
fi

cp obj/${BUILD_CONFIG}/SharpFlame.Gui.Windows.exe ${BUILD_DIR}
cd ${MY_PATH}

# Make a bundle of the resulting .exe
mkdir -p "${OUTPUT_DIR}"
/opt/mono/bin/mono ${MY_PATH}/../tools/ILRepack.exe /out:${OUTPUT_DIR}/SharpFlame.exe ${BUILD_DIR}/SharpFlame.Gui.Windows.exe ${BUILD_DIR}/*.dll

# Update the UpdateFeed.xml checksums.
${MY_PATH}/../tools/feedChecksumUpdater.py -d ${OUTPUT_DIR}
if [ $? -gt 0 ]; then
	echo "Failure while updating the feeds checksums."
	exit $?
fi

# Get the version.
VERSION=$(${MY_PATH}/../tools/FileVersion/FileVersion ${OUTPUT_DIR}/SharpFlame.exe)

# Make the Installer
cp ${OUTPUT_DIR}/SharpFlame.exe ${MY_PATH}/../pkg/nsis/
cd ${MY_PATH}/../pkg/nsis
makensis -DOUTFILE="${MY_PATH}/../installer/Winforms/SharpFlame-${VERSION}.exe" \
		-DTOP_SRCDIR="${MY_PATH}/../" \
		-DPACKAGE_NAME="SharpFlame" \
		-DPACKAGE_VERSION="${VERSION}" \
		SharpFlame.nsi

if [ $? -gt 0 ]; then
	echo "Failure while creating the installer."
	exit $?
fi

exit $?