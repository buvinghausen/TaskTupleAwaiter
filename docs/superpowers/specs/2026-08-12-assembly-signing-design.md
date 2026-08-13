# TaskTupleAwaiter — Cryptographic (Authenticode) signing

**Status:** Approved design
**Date:** 2026-08-12
**Author:** Brian Buvinghausen (with Claude)

## 1. Scope and non-goals

### In scope

- Authenticode-sign each TFM's `TaskTupleAwaiter.dll` produced by a release build, with
  RFC 3161 timestamping so the signature remains valid after the signing certificate
  itself expires.
- Wire signing into `.github/workflows/release.yml` only (the workflow that produces
  artifacts that actually get published).
- Add a fail-closed signature-verification step before `dotnet nuget push`, so a broken
  or missing DLL signature blocks the release instead of silently shipping unsigned.
- Document the one-time, portal-side setup (Azure Trusted Signing enrollment, identity
  validation, OIDC federation) that has to happen outside of source control.

### Non-goals

- **No author-signing of the `.nupkg`.** Investigated and rejected: Azure Trusted
  Signing issues certificates that rotate every 3 days, but NuGet.org's author-signing
  feature requires registering a single, long-lived certificate ahead of time — the
  NuGet team's own tracking issue states plainly that this makes Trusted Signing
  "not feasible" for nupkg author signing today
  ([NuGetGallery#10027](https://github.com/NuGet/NuGetGallery/issues/10027)). The
  documented community workaround (re-extracting and re-registering the `.cer` before
  every release) is described by a NuGet maintainer as "not much better" than doing
  nothing, and was rejected as too fragile for this project. Instead, this design relies
  on NuGet.org's **automatic repository signature**, which every uploaded package
  already receives for free regardless of author signing — it guarantees the package
  wasn't tampered with in transit from nuget.org, which is the property most consumers
  actually care about. If NuGet.org later supports registering short-lived-certificate
  identities (tracked in the issue above), author-signing the `.nupkg` becomes a
  follow-up, not a blocker.
- **No strong-name (SNK) signing.** Evaluated and rejected. The main historical driver —
  ".NET Framework only lets a strong-named assembly reference other strong-named
  assemblies" — doesn't apply on `netstandard2.0`/`net8.0`/`net9.0` consumers, and this
  package's `net462` target means that constraint isn't *entirely* moot, but it's a
  narrow, legacy scenario (a consumer that is itself strong-named, running on classic
  .NET Framework). SNK also provides assembly **identity**, not publisher **trust** —
  it's not inherently insecure, but plenty of OSS projects check the `.snk` (private key
  included) into source control for build convenience, which does negate any security
  benefit if you go that route. Given the narrow applicability and that Authenticode
  already delivers real publisher-identity assurance, SNK isn't worth the added
  build/key-management complexity for this package.
- **No changes to `ci.yml` or `test.sh`.** Signing is a release-only concern; PR builds
  and local test runs never produce artifacts that leave the machine, so they stay
  unsigned.
- **No traditional CA-issued Authenticode certificate.** Considered and rejected in favor
  of Azure Trusted Signing — see §2.

## 2. Signing service

**Azure Trusted Signing**, Public Trust tier, enrolled under Brian's individual identity
via Microsoft's identity-validation partner flow (a one-time KYC step performed manually
in the Azure portal — outside the scope of anything Claude can perform). Note: Microsoft
documents this service under both "Trusted Signing" and, more recently, "Artifact
Signing" branding at overlapping doc paths (`/azure/trusted-signing/`,
`/azure/artifact-signing/`) — mechanically it's the same certificate-profile/HSM service
described throughout this spec.

Rationale: traditional CA-issued Authenticode certs (Sectigo/DigiCert/SSL.com,
~$70–400+/yr) now require a hardware token or HSM for private-key custody, per the 2023
CA/Browser Forum rule change — this adds real friction to CI automation. Azure Trusted
Signing keeps the private key in a Microsoft-managed HSM that the CI job never touches
directly, costs roughly $10/month, and supports individual (non-corporate) identity
validation, which fits a solo-maintainer OSS project.

**Certificates rotate every 3 days.** This is why timestamping (§3) is a hard
requirement, not an optional nicety: without it, every DLL signed more than 3 days ago
would read as unsigned to anything checking certificate validity against current time.

## 3. Tooling and pipeline integration

Signing tool: [`dotnet-sign`](https://github.com/dotnet/sign) (package name `sign`), the
.NET Foundation's official tool for Trusted-Signing-backed Authenticode signing. It
installs as a named CLI tool and is invoked directly as `sign` — **not** `dotnet sign`:

```sh
dotnet tool install --tool-path ./sign-cli --prerelease --version <pinned-version> sign
```

A specific, tested version must be pinned via `--version` rather than floating on latest
`--prerelease` — the exact version to pin is an implementation-time decision (check
current releases at `dotnet/sign`), not guessed here.

**`--tool-path` does not add the install directory to `PATH`** — per Microsoft's own
`dotnet tool install` docs, that only happens automatically for `--global` installs.
Later workflow steps must either invoke the tool by its full path (`./sign-cli/sign.exe`
on the `windows-latest` runner this workflow uses) or explicitly append `./sign-cli` to
`GITHUB_PATH` in the install step before any step that runs bare `sign`.

`release.yml` changes (steps in order; unchanged steps omitted):

1. **New — was implicit inside the old single `dotnet pack` step:**
   `dotnet build -c Release /p:Version=${{ github.event.release.tag_name }}`.
   The version property **must** be passed to `build`, not just `pack` — otherwise the
   DLLs get signed with default (unversioned) assembly metadata while only the `.nupkg`
   carries the release version. This is a correction to the original draft, which
   introduced a build step but dropped `/p:Version` from it.
2. **New:** sign each TFM's `TaskTupleAwaiter.dll` under `bin/Release/*` via `sign`,
   with an RFC 3161 timestamp against Trusted Signing's TSA endpoint
   (`http://timestamp.acs.microsoft.com`, per Microsoft's own guidance). Exact `sign`
   subcommand/flag names are not finalized here — pulled from current `dotnet/sign`
   documentation during implementation.
3. `dotnet pack -c Release --no-build /p:Version=${{ github.event.release.tag_name }}
   /p:PackageReleaseNotes="See
   https://github.com/buvinghausen/TaskTupleAwaiter/releases/tag/${{
   github.event.release.tag_name }}"`
   *(`--no-build` required so packing doesn't trigger a fresh unsigned rebuild that
   would overwrite the signed DLLs; both properties otherwise unchanged from today —
   `PackageReleaseNotes` must carry over onto `pack`, since it's release-specific
   metadata that step 1's `build` doesn't need)*.
4. **New:** verify the signed DLLs — **as packed**, not as built (see §4) — the job
   fails here if verification fails.
5. `dotnet nuget push` *(unchanged — no nupkg signing step; see §1 non-goals)*.

### Authentication: GitHub Actions → Azure

OIDC federated credentials via `azure/login`, **not** a long-lived stored secret. This
requires one-time Azure-portal setup plus explicit workflow YAML that the original draft
omitted:

- **Workflow YAML additions:**
  - `permissions: id-token: write` on the job (required for GitHub to issue the OIDC
    token at all).
  - `permissions: contents: read` alongside it — once a `permissions:` block is present,
    GitHub Actions grants *only* what's listed, and `actions/checkout` needs
    `contents: read`.
  - `environment: release` on the job, so the federated credential can be scoped to a
    specific GitHub Actions Environment (subject claim
    `repo:buvinghausen/TaskTupleAwaiter:environment:release`) rather than trusting every
    workflow in the repo. On its own, declaring `environment: release` in a job doesn't
    restrict anything — any job in any workflow file can name that environment and
    match the subject claim. The restriction comes from **deployment protection rules**
    configured on the `release` environment itself (§5), which is what GitHub's own OIDC
    hardening guidance calls out as the actual gate.
- **Azure-portal setup** (see §5): an Azure AD app registration, a federated identity
  credential trusting that subject claim, and a role assignment granting the app
  permission to sign against the Trusted Signing certificate profile.

Resulting non-secret identifiers (tenant ID, subscription ID, client ID, Trusted Signing
endpoint/account/profile names) are stored as GitHub repo variables/secrets, scoped to
the `release` environment — never committed to source.

## 4. Verification and error handling

- Verification must check the **actual bytes about to be published**, not the
  pre-pack build output — packing shouldn't alter DLL bytes, but the fail-closed
  guarantee only holds if we verify what's really inside the `.nupkg`. So step 4
  extracts the just-built `.nupkg` (it's a zip) and verifies each expected payload
  directly:
  - `lib/netstandard2.0/TaskTupleAwaiter.dll`
  - `lib/net462/TaskTupleAwaiter.dll`
  - `lib/net8.0/TaskTupleAwaiter.dll`
  - `lib/net9.0/TaskTupleAwaiter.dll`

  This simultaneously proves every expected TFM is present in the package *and* that
  each one carries a valid, timestamped signature.
- Verify with `signtool verify /pa /tw <dll>` (or the current equivalent) against each
  extracted DLL — `/pa` selects the Default Authenticode Verification Policy, and `/tw`
  is the *verify*-command option that warns/fails when a signature lacks a timestamp.
  (`/td` is a *sign*/*timestamp*-command option — it doesn't apply to `verify` and was
  wrongly used in an earlier draft of this spec.)
- Verification failure fails the workflow **before** `dotnet nuget push` runs — no
  unsigned or broken-signature DLL is ever published.
- One-time manual check after the first real signed release: confirm
  `Get-AuthenticodeSignature` (or `signtool verify`) reports a valid, trusted chain on a
  downloaded DLL, and that it remains valid after the 3-day certificate window passes
  (proving timestamping actually worked).
- No automated test coverage is added for signing — it's infrastructure around the
  release pipeline, not library behavior; the CI/test suite's job (`ci.yml`, `test.sh`)
  is unaffected and unchanged.

## 5. Prerequisites (manual, outside this repo)

Before the workflow change can be merged and used for a real release, Brian needs to
complete, in the Azure portal:

1. Create/select an Azure subscription.
2. Enroll in Azure Trusted Signing (Public Trust), complete individual identity
   validation.
3. Create a certificate profile.
4. Create a GitHub Actions **Environment** named `release` in this repo's settings, with
   a deployment protection rule attached — at minimum a tag pattern restriction (this
   workflow only ever runs off a published GitHub Release's tag), and preferably
   required-reviewer approval too. Without a protection rule, the `environment: release`
   subject-claim scoping in §3 restricts nothing.
5. Register an Azure AD app and configure a federated credential trusting the subject
   claim `repo:buvinghausen/TaskTupleAwaiter:environment:release`.
6. Assign that app the role needed to sign against the certificate profile.
7. Record the resulting IDs/names as GitHub Actions secrets/variables scoped to the
   `release` environment.

These are one-time setup steps, not part of the implementation plan's code changes.
