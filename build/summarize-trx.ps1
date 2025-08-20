param(
    [string]$SearchRoot = (Get-Location).Path,
    [string]$OutPath = "test-failures-summary.txt",
    [switch]$IncludeStackTrace
)

Write-Host "Searching for .trx files under: $SearchRoot"
$trxFiles = Get-ChildItem -Path $SearchRoot -Recurse -Filter *.trx -ErrorAction SilentlyContinue

if (-not $trxFiles -or $trxFiles.Count -eq 0) {
    $msg = "No .trx files found under $SearchRoot."
    Write-Warning $msg
    Set-Content -Path $OutPath -Value $msg
    exit 0
}

$lines = New-Object System.Collections.Generic.List[string]
$totalFailed = 0

foreach ($file in $trxFiles) {
    try {
        [xml]$xml = Get-Content -Path $file.FullName
    } catch {
        Write-Warning "Failed to load TRX: $($file.FullName) - $($_.Exception.Message)"
        continue
    }

    # Use GetElementsByTagName to avoid namespace headaches
    $results = $xml.GetElementsByTagName('UnitTestResult') | Where-Object { $_.outcome -eq 'Failed' }
    if (-not $results -or $results.Count -eq 0) { continue }

    $rel = Resolve-Path -Path $file.FullName -Relative
    $lines.Add("=== Failures in: $rel ===") | Out-Null

    foreach ($r in $results) {
        $testName = $r.testName
        $duration = $r.duration

        # Error message and optional stack trace
        $errMsg = $null
        $stack = $null
        if ($r.Output -and $r.Output.ErrorInfo) {
            $errMsg = $r.Output.ErrorInfo.Message
            $stack = $r.Output.ErrorInfo.StackTrace
        }

        if (-not $errMsg) { $errMsg = '(no error message provided)' }

        # First line of error message to keep things compact
        $firstLine = ($errMsg -split "`r?`n")[0]
        $lines.Add("- $testName ($duration): $firstLine") | Out-Null

        if ($IncludeStackTrace -and $stack) {
            $lines.Add("  stack: ") | Out-Null
            foreach ($sline in ($stack -split "`r?`n")) {
                $lines.Add("    $sline") | Out-Null
            }
        }

        $totalFailed++
    }

    $lines.Add("") | Out-Null
}

if ($totalFailed -eq 0) {
    $lines.Clear()
    $lines.Add("All tests passed. No failures found in TRX files under $SearchRoot.") | Out-Null
}

# Ensure output directory exists
$outDir = Split-Path -Path $OutPath -Parent
if ($outDir -and -not (Test-Path $outDir)) {
    New-Item -ItemType Directory -Path $outDir -Force | Out-Null
}

Set-Content -Path $OutPath -Value $lines
Write-Host "Wrote failure summary to: $OutPath (total failures: $totalFailed)"

