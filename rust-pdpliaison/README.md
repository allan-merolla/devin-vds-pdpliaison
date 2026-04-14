# PDP Liaison - Rust Common Library

Cross-platform XACML 3.0 Policy Decision Point (PDP) liaison library with C FFI bindings for consumption from multiple languages.

## Platform Support

| Platform | Architecture | Shared Library | Static Library | Status |
|----------|-------------|---------------|---------------|--------|
| Linux    | x86_64      | `libpdp_liaison.so` | `libpdp_liaison.a` | Supported |
| Windows  | x86_64      | `pdp_liaison.dll` + `pdp_liaison.dll.lib` | `pdp_liaison.lib` | Supported |
| macOS    | x86_64      | `libpdp_liaison.dylib` | `libpdp_liaison.a` | Supported |
| macOS    | aarch64     | `libpdp_liaison.dylib` | `libpdp_liaison.a` | Supported |

## Quick Start

### Prerequisites

- [Rust](https://rustup.rs/) 1.85+ (install via `curl --proto '=https' --tlsv1.2 -sSf https://sh.rustup.rs | sh`)

### Build for Current Platform

```bash
cd rust-pdpliaison
cargo build --release
```

Output will be in `target/release/`:
- Linux: `libpdp_liaison.so` and `libpdp_liaison.a`
- Windows: `pdp_liaison.dll`, `pdp_liaison.dll.lib`, and `pdp_liaison.lib`
- macOS: `libpdp_liaison.dylib` and `libpdp_liaison.a`

### Run Tests

```bash
cargo test
```

### Lint

```bash
cargo clippy -- -W clippy::all
```

## Cross-Platform Builds

### Option 1: GitHub Actions (Recommended)

The repository includes CI workflows that automatically build for both Linux and Windows on every push/PR to `main`:

- **CI workflow** (`.github/workflows/rust-build.yml`): Runs tests + clippy, then builds release binaries for Linux and Windows. Artifacts are uploaded and downloadable from the Actions tab.
- **Release workflow** (`.github/workflows/rust-release.yml`): When you push a version tag (e.g., `v0.1.0`), it builds both platforms and creates a GitHub Release with downloadable `.tar.gz` (Linux) and `.zip` (Windows) archives.

**Creating a release:**
```bash
git tag v0.1.0
git push origin v0.1.0
```

Then download the platform-specific packages from the [Releases](../../releases) page.

### Option 2: Cross-Build Script (Linux Host)

Build for multiple platforms from a Linux machine:

```bash
# First time: install cross-compilation targets
./cross-build.sh --install
sudo apt-get install -y gcc-mingw-w64-x86-64  # For Windows cross-compilation

# Build for all platforms
./cross-build.sh all

# Or build for a specific platform
./cross-build.sh linux
./cross-build.sh windows
```

Output goes to `dist/linux-x64/` and `dist/windows-x64/`.

### Option 3: Makefile

```bash
make install-targets    # Install Rust cross-compilation targets
make build-linux        # Build for Linux x86_64
make build-windows      # Build for Windows x86_64 (requires mingw-w64)
make build-all          # Build for all platforms
make package            # Build all and package into dist/
```

### Option 4: Build Natively on Each Platform

On each target machine, install Rust and run:
```bash
cargo build --release
```

This is the simplest approach if you have access to both Windows and Linux build machines.

## Client Integration

### C Header

The `pdp_liaison.h` header file defines the full FFI interface. Include it in your client projects.

### PHP (FFI Extension, PHP 7.4+)

```php
$ffi = FFI::cdef(
    file_get_contents("pdp_liaison.h"),
    "libpdp_liaison.so"  // Linux
    // "pdp_liaison.dll"  // Windows
);

$connector = $ffi->pdp_connector_create_anonymous("https://pdp.example.com/authorize", 2);
$request = $ffi->pdp_request_create();
$ffi->pdp_request_add_element($request,
    "urn:oasis:names:tc:xacml:1.0:subject-category:access-subject",
    "urn:oasis:names:tc:xacml:1.0:subject:subject-id",
    "http://www.w3.org/2001/XMLSchema#string",
    "user@example.com"
);

$error = FFI::new("char*");
$response = $ffi->pdp_connector_evaluate($connector, $request, FFI::addr($error));
$result = $ffi->pdp_response_get_result($response);

// Clean up
$ffi->pdp_response_free($response);
$ffi->pdp_request_free($request);
$ffi->pdp_connector_free($connector);
```

### .NET Framework / .NET 8 (P/Invoke)

```csharp
using System.Runtime.InteropServices;

public static class PdpLiaison
{
    private const string LibName = "pdp_liaison"; // Resolves to pdp_liaison.dll (Windows) or libpdp_liaison.so (Linux)

    [DllImport(LibName)] public static extern IntPtr pdp_connector_create_anonymous(string pdpUrl, int communicationType);
    [DllImport(LibName)] public static extern void pdp_connector_free(IntPtr connector);
    [DllImport(LibName)] public static extern IntPtr pdp_request_create();
    [DllImport(LibName)] public static extern void pdp_request_free(IntPtr request);
    [DllImport(LibName)] public static extern void pdp_request_add_element(IntPtr request,
        string category, string attributeId, string dataType, string value);
    [DllImport(LibName)] public static extern IntPtr pdp_connector_evaluate(IntPtr connector,
        IntPtr request, out IntPtr errorOut);
    [DllImport(LibName)] public static extern int pdp_response_get_result(IntPtr response);
    [DllImport(LibName)] public static extern void pdp_response_free(IntPtr response);
    [DllImport(LibName)] public static extern void pdp_string_free(IntPtr str);
}

// Usage:
var connector = PdpLiaison.pdp_connector_create_anonymous("https://pdp.example.com/authorize", 2);
var request = PdpLiaison.pdp_request_create();
PdpLiaison.pdp_request_add_element(request,
    "urn:oasis:names:tc:xacml:1.0:subject-category:access-subject",
    "urn:oasis:names:tc:xacml:1.0:subject:subject-id",
    "http://www.w3.org/2001/XMLSchema#string",
    "user@example.com");

IntPtr error;
var response = PdpLiaison.pdp_connector_evaluate(connector, request, out error);
int result = PdpLiaison.pdp_response_get_result(response);

PdpLiaison.pdp_response_free(response);
PdpLiaison.pdp_request_free(request);
PdpLiaison.pdp_connector_free(connector);
```

### Java (JNA)

```java
import com.sun.jna.Library;
import com.sun.jna.Native;
import com.sun.jna.Pointer;
import com.sun.jna.ptr.PointerByReference;

public interface PdpLiaison extends Library {
    PdpLiaison INSTANCE = Native.load("pdp_liaison", PdpLiaison.class);

    Pointer pdp_connector_create_anonymous(String pdpUrl, int communicationType);
    void pdp_connector_free(Pointer connector);
    Pointer pdp_request_create();
    void pdp_request_free(Pointer request);
    void pdp_request_add_element(Pointer request,
        String category, String attributeId, String dataType, String value);
    Pointer pdp_connector_evaluate(Pointer connector, Pointer request, PointerByReference errorOut);
    int pdp_response_get_result(Pointer response);
    void pdp_response_free(Pointer response);
    void pdp_string_free(Pointer str);
}

// Usage:
PdpLiaison lib = PdpLiaison.INSTANCE;
Pointer connector = lib.pdp_connector_create_anonymous("https://pdp.example.com/authorize", 2);
Pointer request = lib.pdp_request_create();
lib.pdp_request_add_element(request,
    "urn:oasis:names:tc:xacml:1.0:subject-category:access-subject",
    "urn:oasis:names:tc:xacml:1.0:subject:subject-id",
    "http://www.w3.org/2001/XMLSchema#string",
    "user@example.com");

PointerByReference error = new PointerByReference();
Pointer response = lib.pdp_connector_evaluate(connector, request, error);
int result = lib.pdp_response_get_result(response);

lib.pdp_response_free(response);
lib.pdp_request_free(request);
lib.pdp_connector_free(connector);
```

## Distributing to Clients

### Pre-built Binaries

The easiest approach for your clients:

1. **GitHub Releases**: Push a version tag → CI builds both platforms → clients download from Releases
2. **Manual distribution**: Build on each platform, ship the library files + `pdp_liaison.h`

### What Clients Need

| Platform | Files to distribute | Runtime dependencies |
|----------|-------------------|---------------------|
| Linux    | `libpdp_liaison.so` + `pdp_liaison.h` | None (OpenSSL vendored) |
| Windows  | `pdp_liaison.dll` + `pdp_liaison.dll.lib` + `pdp_liaison.h` | MSVC runtime (usually pre-installed) |

The library vendors OpenSSL (`openssl = { features = ["vendored"] }`) and supports both `rustls` and `native-tls`, so there are **no external TLS dependencies** to install on client machines.

## Project Structure

```
rust-pdpliaison/
├── Cargo.toml           # Dependencies and build config
├── Makefile             # Cross-platform build targets
├── cross-build.sh       # Cross-compilation helper script
├── pdp_liaison.h        # C FFI header for client consumption
├── src/
│   ├── lib.rs           # Library root, public API
│   ├── types.rs         # XACML 3.0 enums and structs
│   ├── constants.rs     # XACML/SAML/SOAP namespace URIs
│   ├── request.rs       # AuthorizationRequest + MultiRequest builders
│   ├── xml_builder.rs   # XACML XML / SAML / SOAP serialization
│   ├── json_builder.rs  # XACML JSON serialization
│   ├── connector.rs     # PdpConnector (HTTP client)
│   ├── response.rs      # Response parsing + deny-biased result logic
│   ├── error.rs         # Error types
│   └── ffi.rs           # C FFI layer (extern "C" functions)
└── tests/
    └── integration_tests.rs  # 51+ integration tests
```
