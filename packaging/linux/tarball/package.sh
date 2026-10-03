#!/usr/bin/env bash

PREFIX="usr/local"

output="${1}"
runtime="${2}"
version="${3}"

MOVE_RULES_TO_ETC="true"

. "${GENERIC_FILES}/package.sh" "${output}" "${runtime}" "${version}"

PKG_FILE="${PACKAGE_LNAME}-${runtime}.tar.gz"

echo "Creating binary tarball '${output}/${PKG_FILE}'..."
mv "${output}/${runtime}" "${output}/${PACKAGE_LNAME}"
create_binary_tarball "${output}/${PACKAGE_LNAME}" "${output}/${PKG_FILE}"
