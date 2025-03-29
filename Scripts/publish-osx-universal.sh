#!/usr/bin/env bash

SCRIPT_DIR=$( cd -- "$( dirname -- "${BASH_SOURCE[0]}" )" &> /dev/null && pwd )
DEVELOPER_ID_APPLICATION=-

cd $SCRIPT_DIR || exit
mkdir -p Builds/osx-universal

echo "Make .app"
mkdir -p Builds/osx-universal/Worlds.app
./generate_entitlements.sh -o Builds/osx-universal/Entitlements.plist --sandbox --network-client

echo "Publishing arm64"
dotnet publish Source/Worlds -o Builds/osx-arm64 --self-contained -r osx-arm64 -p:PublishSingleFile=true -p:PublishTrimmed=true -p:PublishAot=true -p:RadishIsShippingBuild=true -p:SolutionDir=$SCRIPT_DIR

echo "Publishing x64"
dotnet publish Source/Worlds -o Builds/osx-x64 --self-contained -r osx-x64 -p:PublishSingleFile=true -p:PublishTrimmed=true -p:PublishAot=true -p:RadishIsShippingBuild=true -p:SolutionDir=$SCRIPT_DIR

echo "Lipo universal binary"
mkdir -p Builds/osx-universal/Worlds.app/Contents/MacOS
lipo -create -output Builds/osx-universal/Worlds.app/Contents/MacOS/Worlds Builds/osx-arm64/Worlds Builds/osx-x64/Worlds

echo "Copy libs"
cp -f Libraries/osx/*.dylib Builds/osx-universal/Worlds.app/Contents/MacOS

echo "Copy extra files"
mkdir -p Builds/osx-universal/Worlds.app/Contents/Resources/paks
cp -f Source/Worlds/MacAppInfo.plist Builds/osx-universal/Worlds.app/Contents/Info.plist
cp -f Source/Worlds/gamecontrollerdb.txt Builds/osx-universal/Worlds.app/Contents/Resources/gamecontrollerdb.txt
cp -f Source/Worlds/icon-macos.icns Builds/osx-universal/Worlds.app/Contents/Resources/worlds.icns
cp -f Builds/paks/*.rpk Builds/osx-universal/Worlds.app/Contents/Resources/paks

echo "Codesign"
codesign --deep -s $DEVELOPER_ID_APPLICATION --entitlements Builds/osx-universal/Entitlements.plist -o runtime Builds/osx-universal/Worlds.app
rm Builds/osx-universal/Entitlements.plist
