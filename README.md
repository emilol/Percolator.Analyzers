# Percolator

Roslyn analyzers wired directly into a solution via project reference instead of published as a package - shorter feedback loop for rules specific to one codebase.

## What's here

- `Percolator.Analyzers` - the analyzers.
- `Percolator.Analyzers.Tests` - tests for them, using `Microsoft.CodeAnalysis.Testing`.
- `Percolator.Domain` / `Percolator.Persistence` (`.Tests`) - a sample app for the analyzers to run against.

## Why it's here

Not all analyzers are general in nature.

I've found colocating analyzers particularly useful on monorepos mid-migration. When an abstraction is only partially replaced, whoever's writing code (human or agent) reaches for whichever version is closest to hand, and the codebase drifts in two directions at once.

Adding a rule that flags the old pattern in the same commit that introduces a new alternative keeps everyone pulling in the same direction for the length of the migration. It shortens the feedback loop for platform developers and can stay up to date as the abstractions evolve.

## Wiring

`Directory.Build.props` adds this to every project except the analyzer project and its own tests:

```xml
<ProjectReference Include="$(MSBuildThisFileDirectory)Percolator.Analyzers\Percolator.Analyzers.csproj"
                   OutputItemType="Analyzer"
                   ReferenceOutputAssembly="false" />
```

`OutputItemType="Analyzer"` loads the project's output as an analyzer, not a normal dependency. `ReferenceOutputAssembly="false"` skips the compile-time reference. `TreatWarningsAsErrors`, set solution-wide, fails the build the moment a rule fires.

To reuse: copy `Percolator.Analyzers.csproj`'s shape, write rules against your own conventions, reference it the same way.

Want a rule from here published as an actual package instead? Open an issue.

### `NoWarn` for RS2008

`EnforceExtendedAnalyzerRules` is on; RS2008 (requires an `AnalyzerReleases.Shipped/Unshipped.md` changelog) is suppressed, since nothing here is published and there are no releases to track. Drop the `NoWarn` and add the changelog if you publish.

## Rules

One page per rule on the [wiki](https://github.com/emilol/Percolator.Analyzers/wiki), linked from each diagnostic via `helpLinkUri`.

## Building and testing

```
dotnet build
dotnet test
```
