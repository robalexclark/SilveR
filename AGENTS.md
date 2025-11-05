# Repository Guidelines

## Project Structure & Module Organization
SilveR.sln ties together the `SilveR/` web host, `SilveR.UnitTests/`, and `SilveR.IntegrationTests/`. The web app follows MVC: controllers/validators/viewmodels under similarly named folders, Razor views in `SilveR/Views`, static assets in `SilveR/wwwroot`, and Electron + R helpers in `SilveR/Scripts`, `SilveR/R`, and `SilveR/setup`. Keep generated outputs under `SilveR/bin` or `SilveR.IntegrationTests/bin/.../ActualResults` out of source; only curated fixtures in `SilveR.IntegrationTests/ExpectedResults` should be committed.

## Build, Test, and Development Commands
- `dotnet restore SilveR.sln` — syncs all SDK 8.0 dependencies, matching the pipeline's UseDotNet setup.
- `dotnet build SilveR.sln -c Release` — validates the solution and runs analyzers defined in `SilveR.ruleset`.
- `dotnet run --project SilveR/SilveR.csproj` — launches the Kestrel host at http://localhost:5000; use Electron via `electronize start` after installing `ElectronNET.CLI`.
- `dotnet test SilveR.UnitTests/SilveR.UnitTests.csproj` — quick xUnit coverage for controllers, models, and helpers.
- `dotnet test SilveR.IntegrationTests/SilveR.IntegrationTests.csproj --logger trx` — regenerates HTML actuals under `bin/Debug/net8.0/ActualResults`; compare with `ExpectedResults`.
- `pwsh build/summarize-trx.ps1 -SearchRoot . -OutPath test-failures-summary.txt` — mirrors the Azure pipeline's failure digest.

## Coding Style & Naming Conventions
Target .NET 8 with nullable disabled, so add explicit `ArgumentNullException.ThrowIfNull` guards when touching public APIs. Use four-space indentation, PascalCase for types/methods, camelCase for locals/fields, and keep Razor pages aligned with their model names (e.g., `SummaryStatistics.cshtml` <-> `SummaryStatisticsViewModel`). R script filenames in `SilveR/Scripts` must mirror the StatsModel names they serve to keep the worker lookup deterministic.

## Testing Guidelines
xUnit drives both unit and integration suites; append `Tests` to class names (e.g., `GraphicalAnalysisTests`). Unit tests prefer Moq and EF Core InMemory contexts; integration tests depend on the seeded `SilveR.IntegrationTests/SilveR.db` and `_test dataset.xlsx`. When updating statistical logic, refresh the corresponding expected HTML snapshot and document differences in the PR. Keep new tests idempotent so `dotnet test --logger trx` stays clean on CI.

## Commit & Pull Request Guidelines
Recent history (`Restore build/summarize-trx.ps1`, `Fix spelling errors and db test issue`) shows short, imperative summaries; follow that style, optionally adding a body that notes tests run (`dotnet test`, `electronize build`). Reference issue IDs where applicable and flag any dependency bumps. PRs should include: purpose, screenshots for UI-facing Razor changes, notes on R package adjustments, and confirmation that both unit and integration suites plus `summarize-trx.ps1` completed. Describe any migration steps (e.g., copying `R/` assets) so reviewers can reproduce locally.

## Security & Configuration Tips
Do not commit secrets; integration tests already use UserSecrets (`SilveR.IntegrationTests.csproj`). Keep `appsettings.Production.json` values external and prefer environment variables. The bundled Windows R runtime lives under `SilveR/R`; macOS/Linux contributors must run the scripts inside `SilveR/setup` to align package versions. When sharing logs, scrub Serilog outputs of patient or study identifiers before uploading artifacts.
