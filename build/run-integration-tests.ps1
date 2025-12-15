param(
    [string]$Filter = ""
)

$ErrorActionPreference = "Stop"

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$solutionRoot = Resolve-Path (Join-Path $scriptDir "..")
Set-Location $solutionRoot

$projPath = "SilveR.IntegrationTests/SilveR.IntegrationTests.csproj"
if (-not (Test-Path $projPath)) {
    Write-Error "Could not find integration test project at '$projPath'. Run from repository root or adjust the path."
    exit 1
}

$stopwatch = [System.Diagnostics.Stopwatch]::StartNew()

Write-Host "Running integration tests..." -ForegroundColor Cyan

$filterArg = @()
if ($Filter -ne "") {
    $filterArg += "--filter"
    $filterArg += $Filter
}

dotnet test $projPath @filterArg
$exitCode = $LASTEXITCODE

$stopwatch.Stop()
Write-Host ("Integration tests completed in {0:g}" -f $stopwatch.Elapsed) -ForegroundColor Yellow

exit $exitCode
