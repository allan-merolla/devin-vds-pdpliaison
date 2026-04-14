#!/usr/bin/env bash
#
# cross-build.sh - Build PDP Liaison for multiple platforms
#
# Usage:
#   ./cross-build.sh              # Build for all supported targets
#   ./cross-build.sh linux        # Build for Linux x86_64 only
#   ./cross-build.sh windows      # Build for Windows x86_64 only (requires mingw-w64)
#   ./cross-build.sh --install    # Install required cross-compilation toolchains
#
# Output goes to dist/<platform>/ with the compiled libraries + C header.
#

set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
cd "$SCRIPT_DIR"

DIST_DIR="dist"
HEADER_FILE="pdp_liaison.h"

LINUX_TARGET="x86_64-unknown-linux-gnu"
WINDOWS_GNU_TARGET="x86_64-pc-windows-gnu"

RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

info()  { echo -e "${GREEN}[INFO]${NC}  $*"; }
warn()  { echo -e "${YELLOW}[WARN]${NC}  $*"; }
error() { echo -e "${RED}[ERROR]${NC} $*" >&2; }

# ── Install cross-compilation prerequisites ──
install_targets() {
    info "Installing Rust cross-compilation targets..."
    rustup target add "$LINUX_TARGET"
    rustup target add "$WINDOWS_GNU_TARGET"

    info ""
    info "Rust targets installed. You also need platform linkers:"
    info ""
    info "  For Windows cross-compilation from Linux:"
    info "    Ubuntu/Debian: sudo apt-get install -y gcc-mingw-w64-x86-64"
    info "    Fedora/RHEL:   sudo dnf install -y mingw64-gcc"
    info "    Arch:          sudo pacman -S mingw-w64-gcc"
    info ""
    info "  For native Windows builds, use the GitHub Actions workflow"
    info "  or build directly on Windows with MSVC toolchain."
    info ""
}

# ── Build for a specific target ──
build_target() {
    local target="$1"
    local label="$2"
    local out_dir="$3"

    info "Building for $label ($target)..."

    if cargo build --release --target "$target"; then
        mkdir -p "$DIST_DIR/$out_dir"

        # Copy shared libraries
        local src="target/$target/release"
        cp "$src"/libpdp_liaison.so   "$DIST_DIR/$out_dir/" 2>/dev/null || true
        cp "$src"/libpdp_liaison.a    "$DIST_DIR/$out_dir/" 2>/dev/null || true
        cp "$src"/pdp_liaison.dll     "$DIST_DIR/$out_dir/" 2>/dev/null || true
        cp "$src"/pdp_liaison.dll.lib "$DIST_DIR/$out_dir/" 2>/dev/null || true
        cp "$src"/pdp_liaison.lib     "$DIST_DIR/$out_dir/" 2>/dev/null || true
        cp "$src"/libpdp_liaison.dylib "$DIST_DIR/$out_dir/" 2>/dev/null || true

        # Include the C header
        if [ -f "$HEADER_FILE" ]; then
            cp "$HEADER_FILE" "$DIST_DIR/$out_dir/"
        fi

        info "  -> Output: $DIST_DIR/$out_dir/"
        ls -la "$DIST_DIR/$out_dir/"
        echo ""
    else
        error "Build failed for $label ($target)"
        error "Make sure you have the required linker installed."
        error "Run: $0 --install  for setup instructions."
        return 1
    fi
}

# ── Build Linux ──
build_linux() {
    build_target "$LINUX_TARGET" "Linux x86_64" "linux-x64"
}

# ── Build Windows (cross-compile from Linux using mingw) ──
build_windows() {
    # Check for mingw linker
    if ! command -v x86_64-w64-mingw32-gcc &>/dev/null; then
        warn "mingw-w64 cross-compiler not found."
        warn "Install it with: sudo apt-get install -y gcc-mingw-w64-x86-64"
        warn ""
        warn "Alternatively, build natively on Windows or use the GitHub Actions workflow."
        warn "Attempting build anyway (will fail if linker is missing)..."
    fi
    build_target "$WINDOWS_GNU_TARGET" "Windows x86_64 (GNU)" "windows-x64"
}

# ── Main ──
main() {
    info "PDP Liaison Cross-Platform Build"
    info "================================"
    echo ""

    # Check prerequisites
    if ! command -v cargo &>/dev/null; then
        error "Rust/Cargo not found. Install from https://rustup.rs"
        exit 1
    fi

    case "${1:-all}" in
        --install|-i)
            install_targets
            ;;
        linux)
            build_linux
            ;;
        windows)
            build_windows
            ;;
        all)
            build_linux
            build_windows || warn "Windows cross-compilation failed (this is expected without mingw-w64)"
            ;;
        *)
            echo "Usage: $0 [linux|windows|all|--install]"
            exit 1
            ;;
    esac

    if [ -d "$DIST_DIR" ]; then
        echo ""
        info "Build complete. Distribution packages:"
        find "$DIST_DIR" -type f | sort | while read -r f; do
            size=$(du -h "$f" | cut -f1)
            echo "  $size  $f"
        done
    fi
}

main "$@"
