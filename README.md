develop
[![Build status](https://dev.azure.com/supertext/Supertext/_apis/build/status/Supertext.Base/Supertext.Base%20develop%20CI)](https://dev.azure.com/supertext/Supertext/_build/latest?definitionId=15)
master
[![Build Status](https://supertext.visualstudio.com/Supertext/_apis/build/status/Supertext.Base%20Release)](https://dev.azure.com/supertext/Supertext/_build/latest?definitionId=18)

# Introduction 
Supertext.Base contains a collection of useful utilities and helpers. 

## Testing 
*.Test projects should contain Unit Tests only which are fast and dependencies mocked
*.Specs projects should contain Integration tests whose execution time us probably longer

# Getting Started
Each commit at the develp branch will trigger a CI pipeline build. The build runs all unit tests. After the successful pass of build and test, 
a Nuget package will be packed and released. These Nuget packages are available as Prerelease packages and can be tried by loading them via the feed https://supertext.pkgs.visualstudio.com/_packaging/internal/nuget/v3/index.json
Consideration: Prerelease references should not be checked in any consuming project.

# Versioning
The Supertext.Base are versioned after the Semantic Verioning principle (see [SemVer](https://semver.org/)).

Given a version number MAJOR.MINOR.PATCH, increment the:

1. MAJOR version when you make incompatible API changes,
2. MINOR version when you add functionality in a backwards-compatible manner, and
3. PATCH version when you make backwards-compatible bug fixes.
Additional labels for pre-release and build metadata are available as extensions to the MAJOR.MINOR.PATCH format.

## GitVersion
With Supertext.Base GitVersion is a helper to increments versions automatically according to the existing version and available tags.
Further infos under [GitVersion](https://gitversion.readthedocs.io/en/latest/).

# Release procedure
Steps in order to release the Supertext.Base libraries:
1. Initiate the release with a pull request from develop to master
2. After the pull request has been approved, it needs to be merged into the master branch.
3. Discuss and coordinate an upcoming release with your team mates.
4. Set a new Tag at the master branch according to the SemVer principle. 
5. Queue a new build with the "Supertext.Base release" pipeline.
	The package will be released with before the tagged number as package version. It will be available under the same [feed](https://supertext.pkgs.visualstudio.com/_packaging/internal/nuget/v3/index.json).

# Feedback
Feedback and suggestions are always welcome :-)
