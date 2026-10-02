param(
    [switch]$DeployNewMods
)

$srcPath = Join-Path $PSScriptRoot "..\src"
$buildProperties = @()

if ($DeployNewMods) {
    $buildProperties += "-p:DeployAbsent=true"
}

Get-ChildItem -Path $srcPath -Directory | ForEach-Object {
    $modId = $_.Name
    $csproj = Join-Path $_.FullName "$modId.csproj"

    if (Test-Path $csproj) {
        Write-Host "Building $modId..." -ForegroundColor Cyan

        dotnet build $csproj -c Release @buildProperties

        Write-Host
    }
    else {
        Write-Host "Skipping $modId (no $modId.csproj found)" -ForegroundColor Yellow
    }
}
