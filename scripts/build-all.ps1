param(
    [switch]$DeployAbsent
)

$ErrorActionPreference = "Stop"

$srcPath = Join-Path $PSScriptRoot "..\src"
$buildProperties = @()

if ($DeployAbsent) {
    $buildProperties += "-p:DeployAbsent=true"
}

$hasFailed = $false

Get-ChildItem -Path $srcPath -Directory | ForEach-Object {
    Write-Host "Building $($_.Name)..." -ForegroundColor Cyan

    dotnet build $_.FullName -c Release @buildProperties

    if ($LASTEXITCODE -ne 0) {
        $hasFailed = $true
    }

    Write-Host
}

if ($hasFailed) {
    Write-Host "Some builds failed." -ForegroundColor Red
    exit 1
}
else {
    Write-Host "All builds succeeded." -ForegroundColor Green
}
