#!/usr/bin/env sh
set -e

SCRIPT_DIR="$(cd "$(dirname "$0")" && pwd)"
SRC_BANK="$SCRIPT_DIR/Build/desktop/WineFox.bank"
SRC_GUIDS="$SCRIPT_DIR/Build/GUIDs.txt"
DEST_DIR="$SCRIPT_DIR/../STS2_WineFox/sfx/characters"
DEST_BANK="$DEST_DIR/WineFox.bank"
DEST_GUIDS="$DEST_DIR/WineFox.guids.txt"

mkdir -p "$DEST_DIR"

echo "Copying bank..."
cp "$SRC_BANK" "$DEST_BANK"

echo "Copying GUIDs..."
cp "$SRC_GUIDS" "$DEST_GUIDS"

echo "Done."
