#!/usr/bin/env bash

versions=("0.5.x" "0.6.x")

hashuniversalmacos=false

if [ $# -eq 1 ] && [ "$1" == "--hash-universal-macos" ];
then
    hashuniversalmacos=true
fi

# Re-create hashes.txt
> "./build/hashes.txt"

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
            sha256sum $version/Touch-Gestures-$version.zip >> $output
        done
    )

    echo "" >> hashes.txt

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

        oses=("linux" "osx" "win")

        if [ $hashuniversalmacos == true ]; then
            oses=("linux" "win")
        fi

        for os in "${oses[@]}"; do
            for arch in x64 x86 arm64; do

                name="Touch-Gestures.UX-$os-$arch.zip"

                if [ -f "$name" ]; then
                    echo "Computing $name"
                    sha256sum $name >> $output
                fi
            done
        done
    )

    (
        if [ ! -d "./packages" ]; then
            echo "packages directory does not exist, skipping..."
            exit 0
        fi

        cd ./packages

        # Compute all Package Hashes

        if [ $hashuniversalmacos == true ]; then
            echo "Computing Touch-Gestures.UX-osx-universal.tar.gz"
            sha256sum Touch-Gestures.UX-osx-universal.tar.gz >> $output
        fi
    )
)