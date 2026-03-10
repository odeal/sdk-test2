#!/usr/bin/env bash
set -euo pipefail

DIR='/Users/mustafa.senturk/Downloads/nuget(10)'
cd "$DIR"
PKG="package.nupkg"

CERT_IDENTITY="https://github.com/odeal/sdk-test2/.github/workflows/nuget.yml@refs/heads/main"
CERT_ISSUER="https://token.actions.githubusercontent.com"

# echo "Verify signature"

# cosign verify-blob \
#   --bundle sign.bundle.json \
#   --certificate-identity "$CERT_IDENTITY" \
#   --certificate-oidc-issuer "$CERT_ISSUER" \
#   "$PKG"

echo "Verify SBOM"

pwd 

cosign verify-blob-attestation \
  --bundle sbom.bundle.json \
  --type spdx \
  --certificate-identity "$CERT_IDENTITY" \
  --certificate-oidc-issuer "$CERT_ISSUER" \
  "$PKG" -d 2>&1 | tee /tmp/log.txt

# echo "Verify vulnerabilities"

# cosign verify-blob-attestation \
#   --bundle vuln.bundle.json \
#   --type vuln \
#   --certificate-identity "$CERT_IDENTITY" \
#   --certificate-oidc-issuer "$CERT_ISSUER" \
#   "$PKG"

# echo "OK"