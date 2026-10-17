#!/usr/bin/env bash

set -eu
. "$(dirname "${BASH_SOURCE[0]}")/lib.sh"

PACKAGE_GEN="generic"
RUNTIME=""
VERSION="${PACKAGE_VERSION:-"1.0.0"}"

mkdir -p build/packages

print_help() {
  echo "Usage: ${BASH_SOURCE[0]} [OPTIONS]..."
  echo
  echo "Platform-specific options:"
  echo "  --package <package_type>      Package generation script to run after build"
  echo "                                (see eng/linux/* for available package types)"
  echo "  --output <output_dir>         Directory to write the package to"
  echo "  --runtime <runtime>           Runtime to build for"
  echo
  echo "Remarks:"
  echo "  Anything after '--', if it is specified, will be passed to dotnet publish as-is."
}

args=("$@")          # copy args
remaining_args=("$@")    # remaining args after parsing
is_extra_args=false  # whether we've reached '--'
extra_args=()        # args after '--'

while [ ${#remaining_args[@]} -gt 0 ]; do
  if $is_extra_args; then
    extra_args=("${remaining_args[@]}")
    break
  fi

  case "${remaining_args[0]}" in
    --package=*)
      PACKAGE_GEN="${remaining_args[0]#*=}"
      ;;
    --package)
      PACKAGE_GEN="${remaining_args[1]}"
      shift_arr "remaining_args"
      ;;
    --output=*)
      OUTPUT="${remaining_args[0]#*=}"
      ;;
    --output)
      OUTPUT="${remaining_args[1]}"
      shift_arr "remaining_args"
      ;;
    --runtime=*)
      RUNTIME="${remaining_args[0]#*=}"
      ;;
    --runtime)
      RUNTIME="${remaining_args[1]}"
      shift_arr "remaining_args"
      ;;
    --version=*)
      VERSION="${remaining_args[0]#*=}"
      ;;
    --version)
      VERSION="${remaining_args[1]}"
      shift_arr "remaining_args"
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

cleanup_linux_build

if [ -n "${PACKAGE_GEN}" ]; then
  echo -e "\nCreating package with type '${PACKAGE_GEN}'..."

  package_script="${PKG_SCRIPT_ROOT}/${PACKAGE_GEN}/package.sh"
  if [ ! -f "${package_script}" ]; then
    exit_with_error "Could not find package generation script: ${package_script}"
  fi

  echo -e "Running package generation script: ${package_script}\n"

  # child package.sh should expect to be run from the repo root and should write
  # the filename of the package generated to PKG_FILE
  . "${package_script}" "${OUTPUT}" "${RUNTIME}" "${VERSION}" "${extra_args[@]}"

  echo -e "\nPackaging finished! Package created at '${OUTPUT}/${PKG_FILE}'"
fi