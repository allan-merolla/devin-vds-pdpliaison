# PDP Liaison Rust Library - Cross-Platform Build Makefile
#
# Usage:
#   make build          - Build for current platform (debug)
#   make release        - Build for current platform (release, optimized)
#   make test           - Run all tests
#   make clippy         - Run linter
#   make clean          - Remove build artifacts
#   make install-targets - Install cross-compilation targets
#   make build-linux    - Build for Linux x86_64 (release)
#   make build-windows  - Build for Windows x86_64 (release)
#   make build-all      - Build for all supported platforms (release)
#   make package        - Build all platforms and package into dist/

CARGO ?= cargo
RELEASE_DIR = target/release
DIST_DIR = dist

# Platform targets
LINUX_TARGET   = x86_64-unknown-linux-gnu
WINDOWS_TARGET = x86_64-pc-windows-gnu
MACOS_TARGET   = x86_64-apple-darwin

.PHONY: build release test clippy clean install-targets \
        build-linux build-windows build-all package

# ── Local Development ──

build:
	$(CARGO) build

release:
	$(CARGO) build --release

test:
	$(CARGO) test

clippy:
	$(CARGO) clippy -- -W clippy::all

clean:
	$(CARGO) clean
	rm -rf $(DIST_DIR)

# ── Cross-Compilation ──

install-targets:
	rustup target add $(LINUX_TARGET)
	rustup target add $(WINDOWS_TARGET)
	@echo ""
	@echo "NOTE: For Windows cross-compilation on Linux, install mingw-w64:"
	@echo "  Ubuntu/Debian: sudo apt-get install gcc-mingw-w64-x86-64"
	@echo "  Fedora:        sudo dnf install mingw64-gcc"
	@echo ""
	@echo "For macOS target (only works on macOS hosts):"
	@echo "  rustup target add $(MACOS_TARGET)"

build-linux:
	$(CARGO) build --release --target $(LINUX_TARGET)
	@echo ""
	@echo "Linux artifacts:"
	@ls -la target/$(LINUX_TARGET)/release/libpdp_liaison.so 2>/dev/null || true
	@ls -la target/$(LINUX_TARGET)/release/libpdp_liaison.a 2>/dev/null || true

build-windows:
	$(CARGO) build --release --target $(WINDOWS_TARGET)
	@echo ""
	@echo "Windows artifacts:"
	@ls -la target/$(WINDOWS_TARGET)/release/pdp_liaison.dll 2>/dev/null || true
	@ls -la target/$(WINDOWS_TARGET)/release/pdp_liaison.dll.lib 2>/dev/null || true
	@ls -la target/$(WINDOWS_TARGET)/release/libpdp_liaison.a 2>/dev/null || true

build-all: build-linux build-windows

# ── Packaging ──

package: build-all
	mkdir -p $(DIST_DIR)/linux-x64
	mkdir -p $(DIST_DIR)/windows-x64
	# Linux
	cp target/$(LINUX_TARGET)/release/libpdp_liaison.so $(DIST_DIR)/linux-x64/ 2>/dev/null || true
	cp target/$(LINUX_TARGET)/release/libpdp_liaison.a  $(DIST_DIR)/linux-x64/ 2>/dev/null || true
	# Windows
	cp target/$(WINDOWS_TARGET)/release/pdp_liaison.dll     $(DIST_DIR)/windows-x64/ 2>/dev/null || true
	cp target/$(WINDOWS_TARGET)/release/pdp_liaison.dll.lib $(DIST_DIR)/windows-x64/ 2>/dev/null || true
	cp target/$(WINDOWS_TARGET)/release/libpdp_liaison.a    $(DIST_DIR)/windows-x64/ 2>/dev/null || true
	# Include C header
	cp pdp_liaison.h $(DIST_DIR)/ 2>/dev/null || true
	@echo ""
	@echo "Package contents:"
	@find $(DIST_DIR) -type f | sort
