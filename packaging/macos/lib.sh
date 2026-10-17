cleanup_macos_build() {
  if [ -d "${OUTPUT}" ]; then
    if [ -d "${OUTPUT}/Touch-Gestures.app" ]; then
      echo "Cleaning old build outputs..."
      rm -rf "${OUTPUT}/Touch-Gestures.app"
    fi
  fi
}