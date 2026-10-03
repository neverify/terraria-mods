$SourceDirectory = Join-Path $PSScriptRoot "..\build"
$DestinationDirectory = Join-Path $PSScriptRoot "..\zips"

if (-not (Test-Path $DestinationDirectory)) {
    New-Item -ItemType Directory -Path $DestinationDirectory | Out-Null
}

Remove-Item (Join-Path $DestinationDirectory "*.zip")

$ZipCount = 0

Get-ChildItem -Path $SourceDirectory -Directory | ForEach-Object {
    $ManifestPath = Join-Path $_.FullName "manifest.json"

    try {
        $Manifest = Get-Content $ManifestPath -Raw | ConvertFrom-Json
        $Version = $Manifest.version

        if ([string]::IsNullOrWhiteSpace($Version)) {
            throw "Skipping '$($_.Name)': Version is missing or empty."
        }
    }
    catch {
        Write-Warning "Skipping '$($_.Name)': Failed to read version from manifest.json."
        return
    }

    $ZipFile = Join-Path $DestinationDirectory ("$($_.Name)-$Version.zip")

    Compress-Archive -Path $_.FullName -DestinationPath $ZipFile

    $ZipCount += 1
}

Write-Host "Created $ZipCount ZIP files."
