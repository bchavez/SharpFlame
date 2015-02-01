#!/bin/sh
if [ ! -d "$HOME/.local/SharpFlame" ]; then
	mkdir -p $HOME/.local/SharpFlame
fi

if [ ! -d "$HOME/.local/share/icons/hicolor/16x16/apps" ]; then
	mkdir -p "$HOME/.local/share/icons/hicolor/16x16/apps"
fi

if [ ! -d "$HOME/.local/share/icons/hicolor/32x32/apps" ]; then
	mkdir -p "$HOME/.local/share/icons/hicolor/32x32/apps"
fi

if [ ! -d "$HOME/.local/share/icons/hicolor/48x48/apps" ]; then
	mkdir -p "$HOME/.local/share/icons/hicolor/16x16/apps"
fi

if [ ! -d "$HOME/.local/share/icons/hicolor/48x48/apps" ]; then
	mkdir -p "$HOME/.local/share/icons/hicolor/48x48/apps"
fi

if [ ! -d "$HOME/.local/share/icons/hicolor/256x256/apps" ]; then
	mkdir -p "$HOME/.local/share/icons/hicolor/256x256/apps"
fi

if [ ! -d "$HOME/.local/share/applications/" ]; then
	mkdir -p "$HOME/.local/share/applications/"
fi

cp SharpFlame $HOME/.local/SharpFlame
cp share/applications/SharpFlame.desktop $HOME/.local/share/applications/
cp share/icons/hicolor/16x16/apps/flaME.png $HOME/.local/share/icons/hicolor/16x16/apps/
cp share/icons/hicolor/32x32/apps/flaME.png $HOME/.local/share/icons/hicolor/32x32/apps/
cp share/icons/hicolor/48x48/apps/flaME.png $HOME/.local/share/icons/hicolor/48x48/apps/
cp share/icons/hicolor/256x256/apps/flaME.png $HOME/.local/share/icons/hicolor/256x256/apps/

