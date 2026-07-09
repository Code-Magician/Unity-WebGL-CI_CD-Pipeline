# Unity WebGL CI/CD Pipeline Using Github Actions

This project uses **GitHub Actions** to automatically build and deploy a Unity WebGL project to Netlify.

The entire pipeline is managed by a single workflow:

```
.github/workflows/webgl.yml
```

---

# Pipeline Overview

```
                Local Development
                        │
                        ▼
               Push to develop
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
            Merge develop → main
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

Navigate to

```
Repository
→ Settings
→ Secrets and Variables
→ Actions
→ Repository Secrets
```

Create the following secrets.

| Secret | Description |
|---------|-------------|
| UNITY_EMAIL | Unity Account Email |
| UNITY_PASSWORD | Unity Account Password |
| UNITY_LICENSE | Unity License |
| NETLIFY_AUTH_TOKEN | Netlify Personal Access Token |
| NETLIFY_SITE_ID | Production Netlify Site ID |
| NETLIFY_DEV_SITE_ID | Development Netlify Development Site ID |

---

# GitHub Variables

Navigate to

```
Repository
→ Settings
→ Secrets and Variables
→ Actions
→ Variables
```

Create the following variables.

| Variable | Example |
|----------|---------|
| UNITY_VERSION | 6000.4.3f1 |
| BUILD_TARGET | WebGL |
| BUILD_OUTPUT | Build/WebGL |

---

# Unity Requirements

The project includes a build script.

Example:

```csharp
BuildScript.BuildWebGL();
```

The workflow automatically calls this build method.

---

# Build Metadata

Every build automatically generates

```
Assets/StreamingAssets/buildinfo.json
```

The metadata contains

- Version
- Build Number
- Commit SHA
- Branch
- Build Date
- Unity Version
- Workflow
- Build Configuration
- Target Platform

Do **not** commit this file.

Add it to

```
.gitignore
```

```
Assets/StreamingAssets/buildinfo.json
```

---

# Branch Strategy

```
main
develop
```

## develop

Used for development.

Every push automatically:

- Restores Unity Cache
- Generates Build Metadata
- Builds Unity WebGL
- Deploys to the Development Netlify site

---

## main

Used for production.

Every push automatically:

- Restores Unity Cache
- Generates Build Metadata
- Builds Unity WebGL
- Uploads the production build artifact
- Deploys to the Production Netlify site

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

## 3. Commit

```bash
git add .

git commit -m "feat: describe your changes"
```

---

## 4. Push to develop

```bash
git push origin develop
```

This automatically starts the GitHub Action.

The workflow will

- Restore Unity Cache
- Generate Build Metadata
- Build the project
- Deploy to the Development Netlify site

---

## 5. Test the Development Build

Verify everything works correctly on the Development deployment.

---

## 6. Merge develop into main

Once testing is complete

```bash
git checkout main

git pull origin main

git merge develop

git push origin main
```

This automatically starts the Production deployment.

---

# Git LFS

Install Git LFS

```bash
git lfs install
```

Pull LFS assets

```bash
git lfs pull
```

---

# Build Cache

The workflow caches Unity's

```
Library/
```

directory

to reduce build times.

---

# Production Artifacts

Production builds are uploaded as GitHub Artifacts.

Retention:

```
2 Days
```

Development builds do **not** upload artifacts to reduce GitHub Actions storage usage.

---

# Monitoring

## Workflow Runs

```
Repository
↓
Actions
```

## Artifacts

```
Repository
↓
Actions
↓
Workflow Run
↓
Artifacts
```

## Caches

```
Repository
↓
Actions
↓
Caches
```

---

# Skipping the GitHub Workflow

Sometimes you may only update documentation or repository files and don't want to trigger a new build.

To skip the GitHub Actions workflow, include **`[skip ci]`** or **`[ci skip]`** anywhere in your commit message.

Example:

```bash
git add .

git commit -m "docs: update README [skip ci]"

git push origin develop
```

or

```bash
git commit -m "docs: fix typos [ci skip]"
```

> **Note:** This only skips the workflow for that specific commit. Your next commit without the skip keyword will trigger the workflow as usual.


# Current Features

- Automated Unity WebGL Build
- Development Deployment
- Production Deployment
- Unity Library Cache
- Git LFS Support
- Automatic Build Metadata
- Netlify Deployment
- Production Build Artifacts
- Build Time Logging

---

# Workflow Summary

```
Push → develop
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
Merge develop → main
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