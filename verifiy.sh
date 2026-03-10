#!/usr/bin/env bash
set -euo pipefail

DIR='/Users/mustafa.senturk/Downloads/nuget(10)'
cd "$DIR"
PACKAGE="package.nupkg"

OIDC_ISSUER="https://token.actions.githubusercontent.com"
OIDC_IDENTITY="https://github.com/odeal/sdk-test2/.github/workflows/nuget.yml@refs/heads/main"

echo "=== Verifying NuGet package signature ==="

cosign verify-blob \
  --bundle sign.bundle.json \
  --certificate-identity "$OIDC_IDENTITY" \
  --certificate-oidc-issuer "$OIDC_ISSUER" \
  "$PACKAGE"

echo "✔ Package signature verified"


echo
echo "=== Verifying SBOM attestation ==="

cosign verify-blob-attestation \
  --bundle sbom.bundle.json \
  --type spdx \
  --certificate-identity "$OIDC_IDENTITY" \
  --certificate-oidc-issuer "$OIDC_ISSUER" \
  "$PACKAGE" -d  # | jq . 

echo "✔ SBOM attestation verified"


# echo
# echo "=== Verifying vulnerability attestation ==="

# cosign verify-blob-attestation \
#   --bundle vuln.bundle.json \
#   --type vuln \
#   --certificate-identity "$OIDC_IDENTITY" \
#   --certificate-oidc-issuer "$OIDC_ISSUER" \
#   "$PACKAGE" | jq .

# echo "✔ Vulnerability attestation verified"


# echo
# echo "=== Vulnerability summary ==="
# jq '.matches | length' vuln.json