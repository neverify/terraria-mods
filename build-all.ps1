$srcPath = Join-Path $PSScriptRoot "src"

if (-not (Test-Path $srcPath)) {
    Write-Error "Could not find 'src' folder at: $srcPath"
    exit 1
}

Get-ChildItem -Path $srcPath -Directory | ForEach-Object {
    $modId = $_.Name
    $csproj = Join-Path $_.FullName "$modId.csproj"

    if (Test-Path $csproj) {
        Write-Host "Building $modId..." -ForegroundColor Cyan

        dotnet build $csproj -c Release

        if ($LASTEXITCODE -ne 0) {
            Write-Host "Build failed for $modId" -ForegroundColor Red
        }
        else {
            Write-Host "Build succeeded for $modId" -ForegroundColor Green
        }

        Write-Host
    }
    else {
        Write-Host "Skipping $modId (no $modId.csproj found)" -ForegroundColor Yellow
    }
}
