param(
    [switch]$DeployAbsent
)

$srcPath = Join-Path $PSScriptRoot "..\src"
$buildProperties = @()

if ($DeployAbsent) {
    $buildProperties += "-p:DeployAbsent=true"
}

Get-ChildItem -Path $srcPath -Directory | ForEach-Object {
    Write-Host "Building $($_.Name)..." -ForegroundColor Cyan

    dotnet build $_.FullName -c Release @buildProperties

    Write-Host
}
