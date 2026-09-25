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

if (-not (Test-Path -LiteralPath $project)) { throw "Project file not found: $project" }
if (-not (Test-Path -LiteralPath (Join-Path $GamePath 'valheim.exe'))) { throw "Valheim installation not found: $GamePath" }
if (Get-Process -Name 'valheim' -ErrorAction SilentlyContinue) {
    throw 'Close Valheim before installing a new DLL.'
}

# An older local copy can compile successfully even if the art and world-drop
# changes were only partially restored from GitHub. Fail before touching plugins.
$requiredFiles = @(
    'Assets\contract-scroll-simple.png',
    'Assets\cargo-packages.png',
    'ImmersiveTrader\Systems\ContractIconRegistry.cs',
    'ImmersiveTrader\Systems\ContractWorldModel.cs',
    'ImmersiveTrader\Systems\CargoPresentation.cs'
)
foreach ($relative in $requiredFiles) {
    $file = Join-Path $PSScriptRoot $relative
    if (-not (Test-Path -LiteralPath $file)) { throw "Missing mod file: $relative. Fetch the complete update from GitHub." }
}
$projectText = Get-Content -LiteralPath $project -Raw
if (-not $projectText.Contains('ImmersiveTrader.Assets.ContractScroll.png') -or
    -not $projectText.Contains('ImmersiveTrader.Assets.CargoPackages.png')) {
    throw 'The project is missing one or both embedded images. Fetch the latest ImmersiveTrader.csproj from GitHub.'
}
$contractRegistry = Get-Content -LiteralPath (Join-Path $PSScriptRoot 'ImmersiveTrader\Systems\ContractRegistry.cs') -Raw
$cargoRegistry = Get-Content -LiteralPath (Join-Path $PSScriptRoot 'ImmersiveTrader\Systems\TreasureRegistry.cs') -Raw
if (-not $contractRegistry.Contains('ContractWorldModel.Attach') -or
    -not $cargoRegistry.Contains('CargoPresentation.AttachWorldCrate')) {
    throw 'Outdated contract or cargo registration files detected. Fetch the complete update from GitHub.'
}
$rewardRegistry = Get-Content -LiteralPath (Join-Path $PSScriptRoot 'ImmersiveTrader\Systems\RewardRegistry.cs') -Raw
if ($rewardRegistry.Contains('"Ashwood"')) {
    throw 'Outdated reward table refers to the missing Ashwood prefab. Fetch the latest RewardRegistry.cs from GitHub.'
}

dotnet build $project -c $Configuration -p:VALHEIM_INSTALL="$GamePath"
if ($LASTEXITCODE -ne 0) { throw "Build failed. The DLL was not copied." }
if (-not (Test-Path -LiteralPath $output)) { throw "Built DLL not found: $output" }

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
    Write-Host "Duplicate DLL moved: $($duplicate.FullName) -> $destination" -ForegroundColor Yellow
}

Copy-Item -LiteralPath $output -Destination $target -Force
$sourceHash = (Get-FileHash -LiteralPath $output -Algorithm SHA256).Hash
$targetHash = (Get-FileHash -LiteralPath $target -Algorithm SHA256).Hash
if ($sourceHash -ne $targetHash) { throw 'Copied DLL differs from the built DLL.' }
Write-Host "BUILD OK - DLL COPIED: $target" -ForegroundColor Green
Write-Host "SHA256: $targetHash"
