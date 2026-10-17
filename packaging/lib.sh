#!/usr/bin/env bash

# This script contains variables and functions that are shared between systems
# using bash shell. Should be sourced by other scripts early on.

set -eu

### Global variables

PREV_PATH=${PWD}
REPO_ROOT="$(git rev-parse --show-toplevel)"

declare -g OUTPUT="build/packages"

PACKAGE_NAME="Touch-Gestures"
PACKAGE_LNAME="Touch-Gestures.UX"
PACKAGE_VERSION="1.2.0"

### Automatically handle errors and exit

handle_error() {
  echo "Build failed!" >&2
  cd "${PREV_PATH}"
  exit 1
}

handle_exit() {
  cd "${PREV_PATH}"
}

trap handle_error ERR
trap handle_exit EXIT

### Helper functions

exit_with_error() {
  echo "$1" >&2
  handle_error
}

shift_arr() {
  local -n arr="$1"
  local shift="${2:-1}"

  arr=("${arr[@]:$shift}")
}

cleanup_build() {
  if [ -d "${OUTPUT}" ]; then
    echo "Cleaning old build outputs..."
    for dir in "${OUTPUT}"/*; do
      dir_name=$(basename "${dir}")
      if [ "${dir_name}" != "userdata" ]; then
        if ! rm -rf "${dir}" 2>/dev/null; then
          exit_with_error "Could not clean old build dirs. Please manually remove contents of ${OUTPUT} folder."
        fi
      fi
    done
  fi
}

create_binary_tarball() {
  local source="${1}"
  local output="${2}"

  local last_pwd="${PWD}"

  output="$(readlink -f "${output}")"
  cd "${source}/.."
  tar -czf "${output}" "$(basename ${source})"

  cd "${last_pwd}"
}

move_to_nested() {
  local source="${1}"
  local nested="${2}"

  local contents="$(echo "${source}"/*)"
  echo "Moving ${source} to ${nested}..."
  mkdir -p "${nested}"
  mv ${contents} "${nested}"
}

copy_pixmap_assets() {
  local output_folder="${1}"

  echo "Copying pixmap assets to '${output_folder}'..."
  mkdir -p "${output_folder}"
  
  # cleanup old pixmap assets
  if [ -d "${output_folder}" ]; then
    rm -rf "${output_folder:?}"/*
  fi

  cp "${REPO_ROOT}/Touch-Gestures.UX/Assets"/*.png "${output_folder}"
}

create_source_tarball() {
  local prefix="${1}"
  git archive --format=tar --prefix="${prefix}/" HEAD
}

create_source_tarball_gz() {
  local prefix="${1}"
  git archive --format=tar.gz --prefix="${prefix}/" HEAD
}
