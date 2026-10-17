#!/usr/bin/env bash

set -eu

echo "Signing app bundle..."
if hash codesign 2>/dev/null; then
  codesign --deep --force --sign - "${pkg_root}"
else
  echo "Warning: codesign not found, skipping signing"
fi