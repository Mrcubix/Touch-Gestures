#!/usr/bin/env bash

skiphashing=false

if [ $# -eq 1 ] && [ "$1" == "--skip-hashing" ];
then
    skiphashing=true
fi

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

if [ $skiphashing == false ];
then
    # run ./hash.sh and check exit code
    if ! (bash ./hash.sh); then
        echo "Failed to compute hashes"
        exit 1
    fi
else
    echo "Skipping hash computation"
fi