#!/usr/bin/env bash

set -eu
. "$(dirname "${BASH_SOURCE[0]}")/../lib.sh"
. "$(dirname "${BASH_SOURCE[0]}")/lib.sh"

declare -g SIGNED="false"

PKG_SCRIPT_ROOT="$(readlink -f $(dirname "${BASH_SOURCE[0]}"))"
RUNTIMES=("osx-arm64" "osx-x64")
SHARED_NATIVE_DEPS=("libAvaloniaNative.dylib" "libHarfBuzzSharp.dylib" "libSkiaSharp.dylib")

print_help() {
  echo "Usage: ${BASH_SOURCE[0]} [OPTIONS]..."
  echo
  echo "Platform-specific options:"
  echo "  --output <output_dir>         Directory to write the package to"
  echo "  --runtime <runtime>           Runtime to build for"
  echo "                                Supported runtimes: osx-arm64, osx-x64"
  echo "  --version <version>           Version of the package"
  echo "  --signed <true|false>         Whether to sign MacOS binaries (default: ${SIGNED})"
  echo
  echo "Remarks:"
  echo "  Anything after '--', if it is specified, will be passed to dotnet publish as-is."
}

remaining_args=("$@")      # remaining args after parsing
is_extra_args=false  # whether we've reached '--'
extra_args=()        # args after '--'

while [ ${#remaining_args[@]} -gt 0 ]; do
  if $is_extra_args; then
    extra_args=("${remaining_args[@]}")
    break
  fi

  case "${remaining_args[0]}" in
    --output=*)
      OUTPUT="${remaining_args[0]#*=}"
      ;;
    --output)
      OUTPUT="${remaining_args[1]}"
      shift_arr "remaining_args"
      ;;
    --version=*)
      VERSION="${remaining_args[0]#*=}"
      ;;
    --version)
      VERSION="${remaining_args[1]}"
      shift_arr "remaining_args"
      ;;
    --signed=*)
      SIGNED="${remaining_args[0]#*=}"
      ;;
    --signed)
      SIGNED="${remaining_args[1]}"
      shift_arr "args"
    ;;
    --)
      is_extra_args=true
      ;;
    *)
      echo "Unknown option: ${remaining_args[0]}" >&2
      print_help
      exit 1
      ;;
  esac
  shift_arr "remaining_args"
done

cd "${REPO_ROOT}"

cleanup_macos_build

echo -e "\nPreparing package..."

pkg_file="${PACKAGE_LNAME}-osx-universal.tar.gz"
pkg_root="${OUTPUT}/${PACKAGE_NAME}.app"

echo "Copying binaries..."
mkdir -p "${pkg_root}/Contents/MacOS"

for runtime in "${RUNTIMES[@]}"; do
  if [ ! -d "build/ux/${runtime}" ]; then
    echo "build/ux/${runtime} does not exist"
    exit 1
  fi
  
  cp -r "build/ux/${runtime}" "${pkg_root}/Contents/MacOS/"
done

echo "Copying shared libraries..."

# Move shared libraries
mkdir -p "${pkg_root}/Contents/MacOS/shared"

for shared_dep in "${SHARED_NATIVE_DEPS[@]}"; do
  cp "build/ux/osx-x64/${shared_dep}" "${pkg_root}/Contents/MacOS/shared/${shared_dep}"
done

# Remove the dylib from the other runtimes
for runtime in "${RUNTIMES[@]}"; do
  rm -rf "${pkg_root}/Contents/MacOS/${runtime}/"*.dylib

  # Instead symlink the shared dylib
  for shared_dep in "${SHARED_NATIVE_DEPS[@]}"; do
    ln -snf ../shared/"${shared_dep}" "${pkg_root}/Contents/MacOS/${runtime}/${shared_dep}"
  done
done

cp "${PKG_SCRIPT_ROOT}/touchgestures-launch" "${pkg_root}/Contents/MacOS/"

echo "Copying MacOS assets..."
mkdir -p "${pkg_root}/Contents/Resources"
cp "${PKG_SCRIPT_ROOT}/Icon.icns" "${pkg_root}/Contents/Resources/"
cp "${PKG_SCRIPT_ROOT}/Info.plist" "${pkg_root}/Contents/"

if [ "${SIGNED}" == "true" ]; then
  echo "Signing app bundle..."
  if hash rcodesign 2>/dev/null; then
    rcodesign sign "${pkg_root}"
  elif hash codesign 2>/dev/null; then
    codesign --deep --force --sign - "${pkg_root}"
  else
    echo "Warning: neither rcodesign nor codesign found, skipping signing"
  fi
fi

echo "Creating tarball..."
create_binary_tarball "${pkg_root}" "${OUTPUT}/${pkg_file}"

echo -e "\nPackaging finished! Package created at '${OUTPUT}/${pkg_file}'"
