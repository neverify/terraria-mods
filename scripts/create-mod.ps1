param(
    [Parameter(Mandatory = $true, Position = 0)]
    [string]$Name
)

if ([string]::IsNullOrWhiteSpace($Name)) {
    throw "The mod name cannot be empty."
}

$internalName = $Name -replace "\s+", ""
$modId = ($Name -replace "\s+", "-").ToLower()

$destinationDirectory = Join-Path $PSScriptRoot "..\src\$internalName"
if (Test-Path $destinationDirectory) {
    throw "A mod named '$internalName' already exists at '$destinationDirectory'."
}

# Copy the template
$templateDirectory = Join-Path $PSScriptRoot "..\templates\ModTemplate"
Copy-Item -Path $templateDirectory -Destination $destinationDirectory -Recurse -Exclude "obj", "bin"

Get-ChildItem -Path $destinationDirectory -Recurse -File | ForEach-Object {
    # Replace placeholders in the file name
    $newFileName = $_.Name.Replace("ModTemplate", $internalName)
    $newFilePath = Join-Path $_.DirectoryName $newFileName

    if ($newFilePath -ne $_.FullName) {
        Move-Item -Path $_.FullName -Destination $newFilePath
    }

    # Replace placeholders in the file content
    $content = Get-Content -Path $newFilePath -Raw
    $content = $content.
        Replace("Mod Template", $Name).
        Replace("ModTemplate", $internalName).
        Replace("mod-template", $modId)

    Set-Content -Path $newFilePath -Value $content -NoNewline
}

Write-Host "Created mod '$Name' at '$destinationDirectory'."
