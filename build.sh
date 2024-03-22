#!/bin/sh

echo ""
echo "Building Touch-Gestures"
echo ""

# build the plugin
dotnet publish Touch-Gestures -c Release -p:noWarn='"NETSDK1138;VSTHRD200"' -o temp/

echo ""
echo "Moving necessary files to build/plugin"
echo ""

mkdir build

# check if directory exists
if [ ! -d "build" ];
then
    echo "Failed to create the 'build' directory"
    exit 1
fi

(
    cd build

    rm -rf build/*

    mkdir plugin

    # check if directory exists
    if [ ! -d "plugin" ];
    then
        echo "Failed to create the 'build/plugin' directory"
        exit 1
    fi
)

mv temp/Touch-Gestures.dll build/plugin/Touch-Gestures.dll
mv temp/Touch-Gestures.pdb build/plugin/Touch-Gestures.pdb
mv temp/Touch-Gestures.Lib.dll build/plugin/Touch-Gestures.Lib.dll
mv temp/Touch-Gestures.Lib.pdb build/plugin/Touch-Gestures.Lib.pdb
mv temp/OpenTabletDriver.External.Common.dll build/plugin/OpenTabletDriver.External.Common.dll
mv temp/OpenTabletDriver.External.Common.pdb build/plugin/OpenTabletDriver.External.Common.pdb
mv temp/Newtonsoft.Json.dll build/plugin/Newtonsoft.Json.dll

if [ $? -ne 0 ];
then
    echo "Failed to move files"
    cd ../../
    exit 1
fi

cd ./build/plugin

zip -r Touch-Gestures.zip ./*

cd ../../

echo ""
echo "Building Touch-Gestures.UX.Desktop"
echo ""

# build the desktop on linux & windows
dotnet publish Touch-Gestures.UX.Desktop -c Release -p:noWarn='"NETSDK1138;VSTHRD200"' -r linux-x64 -o build/ux/linux
dotnet publish Touch-Gestures.UX.Desktop -c Release -p:noWarn='"NETSDK1138;VSTHRD200"' -r linux-arm64 -o build/ux/linux-arm64
dotnet publish Touch-Gestures.UX.Desktop -c Release -p:noWarn='"NETSDK1138;VSTHRD200"' -r linux-arm -o build/ux/linux-arm

dotnet publish Touch-Gestures.UX.Desktop -c Release -p:noWarn='"NETSDK1138;VSTHRD200"' -r win-x64 -o build/ux/win
dotnet publish Touch-Gestures.UX.Desktop -c Release -p:noWarn='"NETSDK1138;VSTHRD200"' -r win-x86 -o build/ux/win-x86
dotnet publish Touch-Gestures.UX.Desktop -c Release -p:noWarn='"NETSDK1138;VSTHRD200"' -r win-arm64 -o build/ux/win-arm64