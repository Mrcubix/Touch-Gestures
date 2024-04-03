#!/bin/sh

versions=("0.5.x" "0.6.x")

# run ./build-plugin.sh and check exit code
if ! (bash "./build-plugin.sh"); then
    echo "Failed to build plugin"
    exit 1
fi

# run ./build-ux.sh and check exit code
if ! (bash ./build-ux.sh); then
    echo "Failed to build UX"
    exit 1
fi

# Re-create hashes.txt
> "./build/hashes.txt"

# Append all hashes to hashes.txt
(
    cd ./build

    output="../hashes.txt"

    (
        cd ./installer

        # Compute all Plugin Hashes
        for version in "${versions[@]}"
        do
            echo "Computing Touch-Gestures.Installer-$version.zip"
            sha256sum $version/Touch-Gestures.Installer-$version.zip >> $output
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