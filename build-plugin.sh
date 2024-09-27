#!/usr/bin/env bash

# ------------------------- Variables ------------------------- #

declare -rA versions=(
    ["0.5.x"]="OTD05"
    ["0.6.x"]="OTD06"
)

declare -rA suffixes=(
    ["OTD05"]=""
    ["OTD06"]="-0.6.x"
)

main=("Touch-Gestures.dll"
      "Touch-Gestures.Lib.dll" 
      "OpenTabletDriver.External.Common.dll" 
      "Newtonsoft.Json.dll"
      "StreamJsonRpc.dll"
      "Touch-Gestures.pdb" 
      "Touch-Gestures.Lib.pdb"
      "OpenTabletDriver.External.Common.pdb")

# ------------------------- Arguments ------------------------- #

donotzip=false

if [ $# -eq 1 ] && [ "$1" == "--no-zip" ];
then
    donotzip=true
fi

# ------------------------- Functions ------------------------- #

create_build_structure () {
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
}

create_plugin_structure () {
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
}

move_files () {
    (
        local temp=temp/plugin/$version
        local build=build/plugin/$version

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
}

build_installer () {
    echo ""
    echo "Building the installer"
    echo ""

    project="Touch-Gestures.Installer$suffix/Touch-Gestures.Installer$suffix.csproj"

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
}

# ------------------------- Main ------------------------- #

echo ""
echo "Creating build directory structure"
echo ""

create_build_structure

for version in "${!versions[@]}"
do
    # Get suffix for the version (0.5.x -> OTD05 -> "")
    version_code=${versions[${version}]}
    suffix=${suffixes[${version_code}]}

    create_plugin_structure

    echo ""
    echo "Building Touch-Gestures"
    echo ""

    echo "Building Touch-Gestures$suffix"

    #Build the plugin, exit on failure
    if ! dotnet publish "Touch-Gestures$suffix" -c Release -p:noWarn='"NETSDK1138;VSTHRD200"' -o temp/plugin/$version ;
    then
        echo "Failed to build Touch-Gestures for $version"
        exit 1
    fi

    echo ""
    echo "Moving necessary files to build/plugin"
    echo ""

    # Move files contained in variable main to build/plugin
    
    if ! move_files;
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

    build_installer $version $suffix

done

# remove the temp directory
rm -rf "temp/"

echo ""
echo "Plugins Build complete"
echo ""