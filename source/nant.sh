#!/bin/sh

export PATH=/Library/Frameworks/Mono.framework/Versions/Current/bin:$PATH
export PKG_CONFIG_PATH=/opt/local/lib/pkgconfig:$PKG_CONFIG_PATH

if [[ "$OSTYPE" == "linux-gnu" ]]; then
        build="linux"
elif [[ "$OSTYPE" == "darwin"* ]]; then
        build="osx"
fi

mono --runtime="v4.0.30319" tools/nant/NAnt.exe -D:platform=$build -targetframework:mono-4.0 "$@"
