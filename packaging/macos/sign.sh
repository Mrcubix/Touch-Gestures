#!/usr/bin/env bash

set -eu

echo "Signing app bundle..."
if hash rcodesign 2>/dev/null; then
  rcodesign sign "${pkg_root}"
elif hash codesign 2>/dev/null; then
  codesign --deep --force --sign - "${pkg_root}"
else
  echo "Warning: neither rcodesign nor codesign found, skipping signing"
fi