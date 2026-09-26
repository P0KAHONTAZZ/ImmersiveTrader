# Pobiera najnowsze zmiany z GitHuba i buduje/instaluje mod.
# Uzycie: powershell -ExecutionPolicy Bypass -File .\Update-And-Build.ps1
$ErrorActionPreference = 'Stop'
Set-Location $PSScriptRoot

if (Get-Process -Name 'valheim' -ErrorAction SilentlyContinue) { throw 'Zamknij Valheim przed aktualizacja.' }

$branch = (git rev-parse --abbrev-ref HEAD).Trim()
git fetch origin
if ($LASTEXITCODE -ne 0) { throw 'git fetch nie powiodl sie.' }

$dirty = git status --porcelain --untracked-files=no
if ($dirty) {
    Write-Host 'Masz lokalne zmiany w sledzonych plikach:' -ForegroundColor Yellow
    Write-Host $dirty
    throw 'Przerwano, zeby nic nie nadpisac. Wklej ten komunikat Claude.'
}

git pull --ff-only origin $branch
if ($LASTEXITCODE -ne 0) { throw 'git pull nie powiodl sie (historia sie rozjechala). Wklej ten komunikat Claude.' }

Write-Host "Aktualny commit: $(git log --oneline -1)" -ForegroundColor Cyan
& (Join-Path $PSScriptRoot 'Build-And-Install-Verified.ps1')
