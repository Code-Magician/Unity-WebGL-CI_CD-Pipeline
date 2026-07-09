# Unity WebGL CI/CD Pipeline Using GitHub Actions

This project uses **GitHub Actions** to automatically version, build, and deploy a Unity WebGL project to Netlify.

The pipeline is managed by two workflows:

```text
.github/workflows/version.yml
.github/workflows/webgl.yml
```

---

# Pipeline Overview

```text
                Local Development
                        │
                        ▼
               Push to develop
                        │
                        ▼
                  version.yml
                        │
                        ├── Check commit prefix
                        ├── Update .version only if prefix is valid
                        └── Push chore(webgl) version commit
                        │
                        ▼
                   webgl.yml
                        │
                        ├── Restore Unity Cache
                        ├── Generate Build Metadata
                        ├── Build Unity WebGL
                        └── Deploy to Development Netlify
                        │
                        ▼
                  Test the Build
                        │
                        ▼
          Locally merge develop → main
                        │
                        ▼
                  Push main
                        │
                        ▼
                   webgl.yml
                        │
                        ├── Restore Unity Cache
                        ├── Generate Build Metadata
                        ├── Build Unity WebGL
                        ├── Upload Build Artifact
                        └── Deploy to Production Netlify
```

---

# Requirements

- Unity 6
- Git LFS
- GitHub Repository
- Netlify Account
- Unity License

---

# GitHub Secrets

Navigate to:

```text
Repository
→ Settings
→ Secrets and Variables
→ Actions
→ Repository Secrets
```

Create the following secrets.

| Secret | Description |
|--------|-------------|
| UNITY_EMAIL | Unity Account Email |
| UNITY_PASSWORD | Unity Account Password |
| UNITY_LICENSE | Unity License |
| NETLIFY_AUTH_TOKEN | Netlify Personal Access Token |
| NETLIFY_SITE_ID | Production Netlify Site ID |
| NETLIFY_DEV_SITE_ID | Development Netlify Site ID |

---

# GitHub Variables

Navigate to:

```text
Repository
→ Settings
→ Secrets and Variables
→ Actions
→ Variables
```

Create the following variables.

| Variable | Example | Description |
|----------|---------|-------------|
| UNITY_VERSION | 6000.4.3f1 | Unity version used for the build |
| BUILD_TARGET | WebGL | Unity build target |
| BUILD_OUTPUT | Build/WebGL | Output folder for the WebGL build |
| VERSION_FILE | .version | File that stores the current semantic version |
| MAJOR_PREFIX | major | Commit prefix for major version bumps |
| MINOR_PREFIX | minor | Commit prefix for minor version bumps |
| PATCH_PREFIX | patch | Commit prefix for patch version bumps |

---

# Unity Requirements

The project must include a build script.

Example:

```csharp
BuildScript.BuildWebGL();
```

The workflow automatically calls this build method.

---

# Semantic Versioning

This project uses a `.version` file to track the current version.

Example:

```text
0.1.0
```

Version updates happen only on the `develop` branch.

The `version.yml` workflow checks the latest commit message. If the commit message starts with one of the allowed prefixes, it updates `.version` and creates an automatic version commit.

---

# Version Prefixes

Use these commit prefixes to control version updates.

| Prefix | Example Commit | Version Change |
|--------|----------------|----------------|
| `major:` | `major: redesign full gameplay system` | `1.2.3 → 2.0.0` |
| `minor:` | `minor: add new level system` | `1.2.3 → 1.3.0` |
| `patch:` | `patch: fix player movement bug` | `1.2.3 → 1.2.4` |

The prefix must be at the **start** of the commit message.

Correct:

```bash
git commit -m "patch: fix loading screen issue"
```

```bash
git commit -m "minor: add new inventory UI"
```

```bash
git commit -m "major: rebuild core game loop"
```

Incorrect:

```bash
git commit -m "fix: patch loading screen issue"
```

```bash
git commit -m "updated patch fixes"
```

```bash
git commit -m "Patch: fix loading screen issue"
```

---

# How Development Deployment Works

When you push a commit to `develop` with a valid version prefix:

```bash
git commit -m "patch: fix player controller"
git push origin develop
```

The following happens:

```text
Push to develop
        │
        ▼
version.yml runs
        │
        ▼
.version is updated
        │
        ▼
Automatic commit is created:
chore(webgl): bump version to x.y.z
        │
        ▼
webgl.yml runs
        │
        ▼
Development WebGL build is deployed
```

The automatic version commit looks like this:

```text
chore(webgl): bump version to 0.1.1
```

This commit triggers the WebGL development build.

---

# When Development Build Will Not Run

If you push a normal commit to `develop` without one of these prefixes:

```text
major:
minor:
patch:
```

then `.version` will not be updated, and the WebGL development build will not run.

Example:

```bash
git commit -m "docs: update readme"
git push origin develop
```

Result:

```text
version.yml runs
→ No valid version prefix found
→ .version is not changed
→ webgl.yml does not build/deploy development
```

---

# Build Metadata

Every build automatically generates:

```text
Assets/StreamingAssets/buildinfo.json
```

The metadata contains:

- Version
- Build Number
- Commit SHA
- Source Commit SHA
- Branch
- Build Date
- Unity Version
- Workflow
- Build Configuration
- Target Platform

Do **not** commit this file.

Add it to `.gitignore`:

```gitignore
Assets/StreamingAssets/buildinfo.json
```

---

# Branch Strategy

```text
main
develop
```

---

## develop

Used for development builds.

Development deployment only happens when a commit pushed to `develop` starts with:

```text
major:
minor:
patch:
```

A valid versioned commit will:

- Run `version.yml`
- Update `.version`
- Create a `chore(webgl)` version commit
- Run `webgl.yml`
- Restore Unity Cache
- Generate Build Metadata
- Build Unity WebGL
- Deploy to the Development Netlify site

---

## main

Used for production builds.

Production deployment does **not** run `version.yml`.

When changes are merged into `main` and pushed, `webgl.yml` always runs and deploys the production build using the latest `.version` file already merged from `develop`.

A push to `main` will:

- Restore Unity Cache
- Generate Build Metadata
- Build Unity WebGL
- Upload the production build artifact
- Deploy to the Production Netlify site

---

# Development Workflow

## 1. Clone the repository

```bash
git clone https://github.com/Code-Magician/Unity-WebGL-CI_CD-Pipeline.git
```

---

## 2. Create your changes

Work locally as normal.

---

## 3. Commit with a version prefix

Use one of these prefixes:

```text
major:
minor:
patch:
```

Example:

```bash
git add .

git commit -m "patch: fix WebGL loading issue"
```

---

## 4. Push to develop

```bash
git push origin develop
```

This starts the versioning workflow.

If the commit has a valid version prefix:

- `.version` is updated
- a `chore(webgl)` commit is created
- the development build starts automatically
- the build is deployed to the Development Netlify site

---

## 5. Test the Development Build

Verify everything works correctly on the Development deployment.

Development site example:

```text
https://your-dev-site.netlify.app/
```

---

## 6. Locally merge develop into main

Once testing is complete, merge `develop` into `main` locally.

```bash
git checkout main

git pull origin main

git merge develop
```

---

## 7. Push main for production deployment

```bash
git push origin main
```

This automatically starts the production deployment.

The production build uses the latest `.version` file that was merged from `develop`.

---

# Production Deployment Flow

Production deployment should be done by locally merging `develop` into `main`, then pushing `main`.

```text
Test development build
        │
        ▼
git checkout main
        │
        ▼
git pull origin main
        │
        ▼
git merge develop
        │
        ▼
git push origin main
        │
        ▼
Production build starts automatically
```

This ensures the production branch receives the latest:

- Project files
- `.version` file
- Workflow files
- Build configuration

---

# Git LFS

Install Git LFS:

```bash
git lfs install
```

Pull LFS assets:

```bash
git lfs pull
```

---

# Build Cache

The workflow caches Unity's:

```text
Library/
```

directory to reduce build times.

---

# Production Artifacts

Production builds are uploaded as GitHub Artifacts.

Retention:

```text
2 Days
```

Development builds do **not** upload artifacts to reduce GitHub Actions storage usage.

---

# Netlify Deployments

This project uses two separate Netlify sites:

| Site | Purpose |
|------|---------|
| Development Netlify Site | Used for testing development builds |
| Production Netlify Site | Used for final production builds |

The Development deployment uses:

```text
NETLIFY_DEV_SITE_ID
```

The Production deployment uses:

```text
NETLIFY_SITE_ID
```

Both deployments use the same:

```text
NETLIFY_AUTH_TOKEN
```

---

# GitHub Deployments Tab

GitHub may show a **Deployments** tab when Netlify deployment status is reported back to GitHub.

Netlify may also create unique deploy preview URLs like:

```text
https://deploy-id--your-site-name.netlify.app/
```

The main Development site URL and Production site URL should still update when `production-deploy: true` is used in the Netlify deployment step.

---

# Monitoring

## Workflow Runs

```text
Repository
↓
Actions
```

---

## Deployments

```text
Repository
↓
Deployments
```

---

## Artifacts

```text
Repository
↓
Actions
↓
Workflow Run
↓
Artifacts
```

---

## Caches

```text
Repository
↓
Actions
↓
Caches
```

---

# Skipping the GitHub Workflow

Sometimes you may only update documentation or repository files and do not want to trigger a new workflow.

To skip the workflow, include **`[skip ci]`** or **`[ci skip]`** anywhere in your commit message.

Example:

```bash
git add .

git commit -m "docs: update README [skip ci]"

git push origin develop
```

or:

```bash
git commit -m "docs: fix typos [ci skip]"
```

> **Note:** This only skips the workflow for that specific commit. Your next commit without the skip keyword will trigger the workflow as usual.

---

# Current Features

- Automated Semantic Versioning
- Version Prefix Support
- Automated Unity WebGL Build
- Development Deployment
- Production Deployment
- Unity Library Cache
- Git LFS Support
- Automatic Build Metadata
- Netlify Deployment
- Production Build Artifacts
- Build Time Logging
- Development and Production Site Separation
- Skip CI Support

---

# Workflow Summary

```text
Push versioned commit → develop
        │
        ▼
version.yml
        │
        ▼
Update .version
        │
        ▼
Push chore(webgl) commit
        │
        ▼
webgl.yml
        │
        ▼
Build
        │
        ▼
Deploy Development
        │
        ▼
Test
        │
        ▼
Locally merge develop → main
        │
        ▼
Push main
        │
        ▼
webgl.yml
        │
        ▼
Build
        │
        ▼
Upload Artifact
        │
        ▼
Deploy Production
```