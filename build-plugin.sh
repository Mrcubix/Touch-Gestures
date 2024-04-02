#!/bin/sh

version="0.5.x"

echo ""
echo "Building Touch-Gestures"
echo ""

#Build the plugin, exit on failure
if ! dotnet publish Touch-Gestures -c Release -p:noWarn='"NETSDK1138;VSTHRD200"' -o temp/$version;
then
    echo "Failed to build Touch-Gestures for $version"
    exit 1
fi

echo ""
echo "Creating build directory structure"
echo ""

# Check if build directory exists
if [ -d "build" ];
then
    # Clear the build directory if it exists
    echo "Removing all files in build"
    rm -rf build/plugin/$version/*
else
    # Attempt to create the build directory if it does not exist
    if ! mkdir build 2> /dev/null;
    then
        if [ ! -d "build" ];
        then
            echo "Failed to create the 'build' directory"
            exit 1
        fi
    fi
fi

(
    cd build

    #create plugin folder for the specified version
    if ! mkdir -p plugin/$version 2> /dev/null;
    then
        if [ ! -d "plugin/$version" ];
        then
            echo "Failed to create the 'build/plugin/$version' directory"
            exit 1
        fi
    fi
)

echo ""
echo "Moving necessary files to build/plugin"
echo ""

main=("Touch-Gestures.dll"
      "Touch-Gestures.Lib.dll" 
      "OpenTabletDriver.External.Common.dll" 
      "Newtonsoft.Json.dll"
      "StreamJsonRpc.dll"
      "Touch-Gestures.pdb" 
      "Touch-Gestures.Lib.pdb"
      "OpenTabletDriver.External.Common.pdb")

(
    temp=temp/$version
    build=../../build/plugin/$version

    if [ -d $temp ] && [ -d $build ];
    then
        cd $temp

        # Move specific files to the build/plugin directory
        for file in "${main[@]}"
        do
            # If file exist, move it to the build/plugin directory
            if [ -f "$file" ];
            then
                mv $file $build/$file
            else
                # If the file extension is not .pdb, then exit
                if [[ $file != *.pdb ]];
                then
                    echo "Failed to find $file in $version"
                    exit 1
                fi
            fi
        done

        # Zip the files
        cd $build
        zip -r Touch-Gestures-$version.zip ./*
    else
        echo "Failed to find temp or build folder for $version"
        exit 1
    fi
)

if [ $? -ne 0 ];
then
    echo "Failed to move necessary files to build/plugin"
    exit 1
fi

# remove the temp directory
rm -rf "temp/$version"