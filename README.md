main

[![Build status](https://dev.azure.com/supertext/Supertext/_apis/build/status/Supertext.Base/Supertext.Base%20develop%20CI)](https://dev.azure.com/supertext/Supertext/_build/latest?definitionId=15)

release

[![Build Status](https://supertext.visualstudio.com/Supertext/_apis/build/status/Supertext.Base%20Release)](https://dev.azure.com/supertext/Supertext/_build/latest?definitionId=18)

# Introduction
Supertext.Base contains a collection of useful utilities and helpers.

## Testing
*.Test projects should contain unit tests only, which are fast and have their dependencies mocked.
*.Specs projects should contain integration tests, whose execution time is probably longer.

# Getting Started
Each commit on the main branch triggers a CI pipeline build.
The build runs all unit tests.
After build and tests have passed, a NuGet package is packed and released.
These packages are available as prerelease packages and can be tried by loading them from the feed https://supertext.pkgs.visualstudio.com/_packaging/internal/nuget/v3/index.json.
Consideration: prerelease references should not be checked in to any consuming project.

# Versioning
Supertext.Base is versioned according to the Semantic Versioning principle (see [SemVer](https://semver.org/)).

Given a version number MAJOR.MINOR.PATCH, increment the:

1. MAJOR version when you make incompatible API changes,
2. MINOR version when you add functionality in a backwards-compatible manner, and
3. PATCH version when you make backwards-compatible bug fixes.

Additional labels for pre-release and build metadata are available as extensions to the MAJOR.MINOR.PATCH format.

In practice the MAJOR version tracks the .NET major version that the libraries target, so 8.0.x targets .NET 8, 9.0.x targets .NET 9 and 10.0.x targets .NET 10.

## GitVersion
GitVersion is a helper that increments versions automatically according to the existing version and the available tags.
Further infos under [GitVersion](https://gitversion.readthedocs.io/en/latest/).

# Release procedure
Steps in order to release the Supertext.Base libraries:

1. Merge the pull request into the main branch.
2. Discuss and coordinate the upcoming release with your team mates.
3. Set a new tag on the main branch according to the SemVer principle. You can do this via https://github.com/Supertext/Supertext.Base/releases/new. Write also something regarding the release.
4. Queue a new build with the [Supertext.Base release](https://dev.azure.com/supertext/Supertext/_build?definitionId=18) pipeline.
	The package will be released with the before tagged number as package version. It will be available under the same [feed](https://supertext.pkgs.visualstudio.com/_packaging/internal/nuget/v3/index.json).

# Feedback
Feedback and suggestions are always welcome :-)
