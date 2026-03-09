# UnfilteredDevOps.SecureHttpClient

A secure HTTP client wrapper for .NET with built-in retry logic, timeout handling, and comprehensive logging capabilities designed for supply chain security scenarios.

## Features

- **Automatic Retry**: Exponential backoff for transient failures (5xx, 408)
- **Timeout Handling**: Configurable request timeouts
- **Structured Logging**: Built-in ILogger support
- **Custom Headers**: Support for default headers
- **Resilience**: Powered by Polly

## Installation

```bash
dotnet add package UnfilteredDevOps.SecureHttpClient
```

## Usage

```csharp
using UnfilteredDevOps.SecureHttpClient;

var options = new HttpClientOptions
{
    BaseAddress = "https://api.example.com",
    Timeout = TimeSpan.FromSeconds(30),
    MaxRetryAttempts = 3,
    RetryDelay = TimeSpan.FromMilliseconds(500)
};

using var client = new SecureHttpClient(options);
var response = await client.GetAsync("/endpoint");
```

## Security & Supply Chain

This package is built with supply chain security as a first-class concern:

### Package Signature Verification

Every release is cryptographically signed using Cosign with GitLab OIDC tokens (keyless signing).

**Verify signature (example for 1.0.29, branch `main`):**

```bash
# Download the package (or use dotnet nuget download)
curl -sfL \
  https://www.nuget.org/api/v2/package/UnfilteredDevOps.SecureHttpClient/1.0.29 \
  -o UnfilteredDevOps.SecureHttpClient.1.0.29.nupkg

# Download the signature bundle from GitLab artifacts
# Replace JOB_ID if you want a different pipeline run
curl -o UnfilteredDevOps.SecureHttpClient.1.0.29.nupkg.cosign.bundle \
  https://gitlab.com/unfiltered-devops/gitlab/supply-chain/-/jobs/12810732500/file/packages/UnfilteredDevOps.SecureHttpClient.1.0.29.nupkg.cosign.bundle

# Verify with Cosign (keyless mode with GitLab OIDC)
cosign verify-blob --bundle UnfilteredDevOps.SecureHttpClient.1.0.29.nupkg.cosign.bundle \
  --certificate-identity-regexp "https://gitlab.com/unfiltered-devops/gitlab/supply-chain//.gitlab-ci.yml@refs/(heads|tags)/main" \
  --certificate-oidc-issuer "https://gitlab.com" \
  UnfilteredDevOps.SecureHttpClient.1.0.29.nupkg -d
```

Expected output:
```
Verified OK
```

### SLSA Provenance

Build metadata adhering to SLSA v1 framework is included in each release:

- **Source**: Repository and commit SHA
- **Build**: Build timestamp, runner information
- **Dependencies**: Exact versions used during compilation

Access provenance: [GitLab Artifacts](https://gitlab.com/unfiltered-devops/gitlab/supply-chain/-/jobs/)

### Software Bill of Materials (SBOM)

Complete dependency inventory in multiple formats:

- **CycloneDX JSON**: Standard format for dependency tracking
- **SPDX JSON**: Linux Foundation format for compliance
- **Text Report**: Human-readable dependency list

Download SBOM files from [GitLab Artifacts](https://gitlab.com/unfiltered-devops/gitlab/supply-chain/-/jobs/)

### Transparency Log

All signatures are logged to Rekor (sigstore transparency log):

```bash
rekor-cli search --artifact UnfilteredDevOps.SecureHttpClient.1.0.1.nupkg
```

## Building & Testing

```bash
# Restore dependencies
dotnet restore

# Build
dotnet build -c Release

# Run tests
dotnet test

# Generate package
dotnet pack -c Release
```

## Requirements

- .NET 8.0 or later
- Microsoft.Extensions.Logging.Abstractions 8.0+
- Polly 8.2+

## License

MIT License - See LICENSE file for details

## Verify This Package

Before using this package, verify its authenticity:

1. Check the signature using Cosign (see Security section above)
2. Review the SBOM for unexpected dependencies
3. Check Rekor transparency log for build metadata
4. Verify the SLSA provenance metadata

---

**Built with supply chain security by UnfilteredDevOps**
