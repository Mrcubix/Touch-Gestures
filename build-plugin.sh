#!/bin/sh

versions=("0.5.x" "0.6.x")

donotzip=false

if [ $# -eq 1 ] && [ "$1" == "--no-zip" ];
then
    donotzip=true
fi

echo ""
echo "Creating build directory structure"
echo ""

for version in "${versions[@]}"
do
    suffix=""

    # if version is not 0.5.x, suffix plugin project with version
    if [ $version != "0.5.x" ];
    then
        suffix="-$version"
    fi

    # Check if build directory exists
    if [ -d "build" ];
    then
        # Clear the build directory if it exists
        echo "Removing all files in build"
        rm -rf build/plugin/$version/*
        rm -rf build/installer/$version/*
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

        #create installer folder for the specified version
        if ! mkdir -p installer/$version 2> /dev/null;
        then
            if [ ! -d "installer/$version" ];
            then
                echo "Failed to create the 'build/installer/$version' directory"
                exit 1
            fi
        fi
    )

    echo ""
    echo "Building Touch-Gestures"
    echo ""

    echo "Building Touch-Gestures$suffix"

    #Build the plugin, exit on failure
    if ! dotnet publish "Touch-Gestures$suffix" -c Release -p:noWarn='"NETSDK1138;VSTHRD200"' -o temp/plugin/$version;
    then
        echo "Failed to build Touch-Gestures for $version"
        exit 1
    fi

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
        temp=temp/plugin/$version
        build=build/plugin/$version

        if [ -d $temp ] && [ -d $build ];
        then
            cd $temp

            # Move specific files to the build/plugin directory
            for file in "${main[@]}"
            do
                # If file exist, move it to the build/plugin directory
                if [ -f "$file" ];
                then
                    mv $file ../../../$build/$file
                else
                    # If the file extension is not .pdb, then exit
                    if [[ $file != *.pdb ]];
                    then
                        echo "Failed to find $file in $version"
                        exit 1
                    fi
                fi
            done

            #check if donotzip is set to true
            if [ $donotzip == false ];
            then
                # Zip the files
                cd ../../../$build
                zip -r Touch-Gestures-$version.zip ./*
            fi
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

    # if donotzip is set to true, then skip the installer build
    if [ $donotzip == true ];
    then
        echo "Skipping installer build for $version"
        continue
    fi

    echo ""
    echo "Building the installer"
    echo ""

    project="Touch-Gestures.Installer/Touch-Gestures.Installer$suffix.csproj"

    # Build the installer
    if ! dotnet publish $project -c Release -p:noWarn='"NETSDK1138;VSTHRD200"' -o temp/installer/$version;
    then
        echo "Failed to build Touch-Gestures.Installer for $version"
        exit 1
    fi

    mv "temp/installer/$version/Touch-Gestures.Installer.dll" "build/installer/$version/Touch-Gestures.Installer.dll"

    (
        cd build/installer/$version

        # Zip the installer
        if ! zip -r "Touch-Gestures.Installer-$version.zip" "Touch-Gestures.Installer.dll"
        then
            echo "Failed to zip the installer"
            exit 1
        fi
    )

done

# remove the temp directory
rm -rf "temp/"

echo ""
echo "Plugins Build complete"
echo ""
