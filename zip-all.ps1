$SourceDirectory = Join-Path $PSScriptRoot "build"
$DestinationDirectory = Join-Path $PSScriptRoot "zips"

if (-not (Test-Path $DestinationDirectory)) {
    New-Item -ItemType Directory -Path $DestinationDirectory | Out-Null
}

Get-ChildItem -Path $SourceDirectory -Directory | ForEach-Object {
    $Folder = $_.FullName
    $ManifestPath = Join-Path $Folder "manifest.json"

    if (-not (Test-Path $ManifestPath)) {
        Write-Warning "Skipping '$($_.Name)': manifest.json was not found."
        return
    }

    try {
        $Manifest = Get-Content $ManifestPath -Raw | ConvertFrom-Json
        $Version = $Manifest.version

        if ([string]::IsNullOrWhiteSpace($Version)) {
            throw "Skipping '$($_.Name)': Version is missing or empty."
        }
    }
    catch {
        Write-Warning "Skipping '$($_.Name)': Failed to read version from manifest.json. $_"
        return
    }

    $ZipFile = Join-Path $DestinationDirectory ("$($_.Name)-$Version.zip")

    if (Test-Path $ZipFile) {
        Remove-Item $ZipFile -Force
    }

    Compress-Archive -Path $Folder -DestinationPath $ZipFile
}

Write-Host "Finished creating ZIP files."
