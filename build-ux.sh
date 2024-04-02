#!/bin/sh

donotzip=false

if [ $# -eq 1 ] && [ "$1" == "--no-zip" ];
then
    donotzip=true
fi

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

find ./build/ux -name "*.pdb" -type f -delete

if [ $donotzip == true ];
then
    echo "Skipping zipping of UX"
    exit 0
fi

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

echo ""
echo "UX built successfully"
echo ""