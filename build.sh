#!/bin/sh

echo ""
echo "Building Touch-Gestures"
echo ""

# build the plugin
dotnet publish Touch-Gestures -c Release -p:noWarn='"NETSDK1138;VSTHRD200"' -o temp/

echo ""
echo "Moving necessary files to build/plugin"
echo ""

# check if directory exists
if ! mkdir build;
then
    if [ ! -d "build" ];
    then
        echo "Failed to create the 'build' directory"
        exit 1
    fi
fi

(
    cd build

    rm -rf build/*

    # check if directory exists
    if ! mkdir plugin;
    then
        if [ ! -d "plugin" ];
        then
            echo "Failed to create the 'build/plugin' directory"
            exit 1
        fi
    fi
)

main=("Touch-Gestures.dll"
      "Touch-Gestures.Lib.dll" 
      "OpenTabletDriver.External.Common.dll" 
      "Newtonsoft.Json.dll"
      "Touch-Gestures.pdb" 
      "Touch-Gestures.Lib.pdb"
      "OpenTabletDriver.External.Common.pdb")

for file in "${main[@]}"
do
    # If file exist, move it to the build/plugin directory
    if [ -f "temp/$file" ];
    then
        mv temp/$file build/plugin/$file
    else
        # If the file extension is not .pdb, then exit
        if [[ $file != *.pdb ]];
        then
            echo "Failed to find $file"
            exit 1
        fi
    fi
done

# remove the temp directory
rm -rf temp

cd ./build/plugin

zip -r Touch-Gestures.zip ./*

cd ../../

echo ""
echo "Building Touch-Gestures.UX.Desktop"
echo ""

# build the desktop on linux & windows
platforms=("linux-x64" "linux-arm64" "linux-arm" "win-x64" "win-x86" "win-arm64")

for platform in "${platforms[@]}"
do
    # build the desktop on linux & windows
    dotnet publish Touch-Gestures.UX.Desktop -c Release -p:noWarn='"NETSDK1138;VSTHRD200"' -r $platform -o build/ux/$platform
done