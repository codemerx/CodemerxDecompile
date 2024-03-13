#!/bin/sh

mkdir -p CodemerxDecompile.app/Contents/{MacOS,Resources}

script_dir=$(dirname $0)
cp $script_dir/Info.plist CodemerxDecompile.app/Contents
cp $script_dir/codemerx-logo.icns CodemerxDecompile.app/Contents/Resources

cp -Rp engine/CodemerxDecompile/bin/Release/net7.0/osx-arm64/publish/ CodemerxDecompile.app/Contents/MacOS
