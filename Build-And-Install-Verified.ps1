param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Debug',
    [string]$GamePath = 'C:\Program Files (x86)\Steam\steamapps\common\Valheim'
)

$ErrorActionPreference = 'Stop'
$project = Join-Path $PSScriptRoot 'ImmersiveTrader\ImmersiveTrader.csproj'
$output = Join-Path $PSScriptRoot "ImmersiveTrader\bin\$Configuration\netstandard2.1\ImmersiveTrader.dll"
$plugins = Join-Path $GamePath 'BepInEx\plugins'
$target = Join-Path $plugins 'ImmersiveTrader.dll'

if (-not (Test-Path -LiteralPath $project)) { throw "Brak projektu: $project" }
if (-not (Test-Path -LiteralPath (Join-Path $GamePath 'valheim.exe'))) { throw "Nie znaleziono Valheim: $GamePath" }

dotnet build $project -c $Configuration -p:VALHEIM_INSTALL="$GamePath"
if ($LASTEXITCODE -ne 0) { throw "Kompilacja nie powiodla sie. DLL nie zostala skopiowana." }
if (-not (Test-Path -LiteralPath $output)) { throw "Brak zbudowanej DLL: $output" }

$backup = Join-Path $GamePath 'BepInEx\disabled-ImmersiveTrader'
$duplicates = @(Get-ChildItem -LiteralPath $plugins -Filter '*.dll' -Recurse -File | Where-Object {
    if ($_.FullName -eq $target) { return $false }
    try { [System.Reflection.AssemblyName]::GetAssemblyName($_.FullName).Name -eq 'ImmersiveTrader' }
    catch { $false }
})
foreach ($duplicate in $duplicates) {
    New-Item -ItemType Directory -Path $backup -Force | Out-Null
    $destination = Join-Path $backup ($duplicate.BaseName + '-' + (Get-Date -Format 'yyyyMMdd-HHmmss-fff') + $duplicate.Extension)
    Move-Item -LiteralPath $duplicate.FullName -Destination $destination
    Write-Host "Druga kopia DLL przeniesiona: $($duplicate.FullName) -> $destination" -ForegroundColor Yellow
}

Copy-Item -LiteralPath $output -Destination $target -Force
$sourceHash = (Get-FileHash -LiteralPath $output -Algorithm SHA256).Hash
$targetHash = (Get-FileHash -LiteralPath $target -Algorithm SHA256).Hash
if ($sourceHash -ne $targetHash) { throw 'Skopiowana DLL rozni sie od zbudowanej.' }
Write-Host "BUILD OK - DLL SKOPIOWANA: $target" -ForegroundColor Green
Write-Host "SHA256: $targetHash"
