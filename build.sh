#!/bin/sh

versions=("0.5.x" "0.6.x")

echo ""
echo "Building Touch-Gestures"
echo ""

# build the plugin, exit on failure
dotnet publish Touch-Gestures -c Debug -p:noWarn='"NETSDK1138;VSTHRD200"' -o temp/0.5.x || exit 1
dotnet publish Touch-Gestures.0.6.x -c Debug -p:noWarn='"NETSDK1138;VSTHRD200"' -o temp/0.6.x || exit 1

echo ""
echo "Creating build directory structure"
echo ""

# Check if build directory exists
if [ -d "build" ];
then
    # Clear the build directory if it exists
    echo "Removing all files in build"
    rm -rf build/*
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

    #create plugin folder for each version
    for version in "${versions[@]}"
    do
        if ! mkdir -p plugin/$version 2> /dev/null;
        then
            if [ ! -d "plugin/$version" ];
            then
                echo "Failed to create the 'build/plugin/$version' directory"
                exit 1
            fi
        fi
    done
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

# Interate throught 0.5.x and 0.6.x
for version in "${versions[@]}"
do
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
done

if [ $? -ne 0 ];
then
    echo "Failed to move necessary files to build/plugin"
    exit 1
fi

# remove the temp directory
rm -rf temp

echo ""
echo "Building Touch-Gestures.UX.Desktop"
echo ""

# build the desktop on linux & windows
platforms=("linux-x64" "linux-arm64" "linux-arm" "win-x64" "win-x86" "win-arm64" "osx-x64" "osx-arm64")

for platform in "${platforms[@]}"
do
    # build the desktop on linux & windows
    dotnet publish Touch-Gestures.UX.Desktop -c Release -p:noWarn='"NETSDK1138;VSTHRD200"' -r $platform -o build/ux/$platform
done

find ./build/ux -name "OpenTabletDriver*.pdb" -type f -delete

# zip all the files
(
    cd ./build/ux

    for f in *; do
        if [ -d "$f" ]; then
            echo "Zipping $f"
            (
                cd "$f"
                zip -r "../Touch-Gestures.UX-$f.zip" *
            )
        fi
    done
)

# Append all hashes to hashes.txt
(
    cd ./build

    output="../hashes.txt"

    (
        cd ./plugin

        # Compute all Plugin Hashes
        for version in "${versions[@]}"
        do
            echo "Computing Touch-Gestures-$version.zip"
            sha256sum $version/Touch-Gestures-$version.zip > $output
        done
    )

    echo "" >> hashes.txt

    (
        cd ./ux

        # Compute all UX Hashes

        for os in win linux osx; do
            for arch in x64 x86 arm64; do

                name="Touch-Gestures.UX-$os-$arch.zip"

                echo "Computing $name"

                if [ -f "$name" ]; then
                    sha256sum $name >> $output
                fi
            done
        done
    )
)