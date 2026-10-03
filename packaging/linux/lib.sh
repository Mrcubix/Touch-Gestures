#!/usr/bin/env bash

# Slightly altered from https://github.com/OpenTabletDriver/OpenTabletDriver/blob/0.6.x/eng/linux/lib.sh

. "$(dirname "${BASH_SOURCE[0]}")/../lib.sh"

### Global variables

PKG_SCRIPT_ROOT="$(readlink -f $(dirname "${BASH_SOURCE[0]}"))"
GENERIC_FILES="$(readlink -f "${PKG_SCRIPT_ROOT}/generic")"

### Helper functions

# From https://github.com/dotnet/install-scripts/blob/main/src/dotnet-install.sh
is_musl_based_distro() {
  (ldd --version 2>&1 || true) | grep -q musl
}

copy_generic_files() {
  local output="${1}"

  echo "Copying generic files..."
  cp -Rv "${GENERIC_FILES}/usr/"* "${output}"
  echo
}

cleanup_linux_build() {
  if [ -d "${OUTPUT}" ]; then
    if [ -d "${OUTPUT}/touch-gestures" ]; then
      echo "Cleaning old build outputs..."
      rm -rf "${OUTPUT}/touch-gestures"
    fi
  fi
}