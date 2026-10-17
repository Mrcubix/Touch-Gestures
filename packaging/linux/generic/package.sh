#!/usr/bin/env sh

# check if build/ux exists
if [ ! -d "./build/ux" ]; then
  echo "build/ux directory does not exist"
  echo "UX apps need to be built first"
  exit 1
fi

output="$(readlink -f "${1}")"
runtime="${2}"
version="${3}"

PKG_FILE=$runtime

# Check if output directory exists
if [ -z "${output}" ]; then
  echo "Output directory not specified"
  exit 1
elif [ -z "${runtime}" ]; then
  echo "No runtime specified"
  exit 1
elif [ -z "${version}" ]; then
  echo "No version specified, defaulting to 1.0.0"
  version="1.0.0"
fi

if [ ! -d "${output}" ]; then
  echo "Output directory does not exist"
  exit 1
fi

# Get output directory for the package
binaries="build/ux/${runtime}"
PREFIX="${PREFIX:-usr}"

# strip last slash if present
output="${output%/}"

mkdir -p "${output}/${runtime}/${PREFIX}"

copy_generic_files "${output}/${runtime}/${PREFIX}"

echo "Copying binaries..."

mkdir -p "${output}/${runtime}/${PREFIX}/lib/touch-gestures"

# Move all binaries to the output directory
for binary in "${binaries}"/*; do
  cp -v "${binary}" "${output}/${runtime}/${PREFIX}/lib/touch-gestures/$(basename "${binary}")"
done

# Create doc directory for the license
mkdir -p "${output}/${runtime}/${PREFIX}/share/doc/touch-gestures"
cp -v "${REPO_ROOT}/LICENSE" "${output}/${runtime}/${PREFIX}/share/doc/touch-gestures/LICENSE"

copy_pixmap_assets "${output}/${runtime}/${PREFIX}/share/pixmaps"

if [ "${PREFIX}" != "usr" ]; then
  echo "Patching wrapper scripts to point to '/${runtime}/${PREFIX}/bin/touch-gestures'..."

  for exe in "${output}/${runtime}/${PREFIX}/bin"/*; do
    sed -i "s|/usr/lib|/${PREFIX}/lib|" "${exe}"
  done

  sed -i "s|/usr/share|/${PREFIX}/share|" "${output}/${runtime}/${PREFIX}/share/applications/touch-gestures.desktop"
fi

sed -i "s|VERSION_PLACEHOLDER|${version}|" "${output}/${runtime}/${PREFIX}/share/applications/touch-gestures.desktop"