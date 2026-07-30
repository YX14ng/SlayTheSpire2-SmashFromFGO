param(
    [string[]]$Only = @(),
    [string]$Output = "",
    [switch]$NoContactSheets
)

$ErrorActionPreference = "Stop"
$repo = Split-Path $PSScriptRoot -Parent
if (-not $Output) { $Output = Join-Path $repo "dist\animation-audit" }

$python = Get-Command python -ErrorAction SilentlyContinue
if ($null -eq $python) { throw "No se encontró Python en PATH." }

& $python.Source -c "import PIL" 2>$null
if ($LASTEXITCODE -ne 0) { throw "Falta Pillow. Instalar con: python -m pip install Pillow" }

$profileArguments = @(
    (Join-Path $PSScriptRoot "apply_animation_profiles.py"),
    "--repo", $repo
)
foreach ($id in $Only) { $profileArguments += @("--only", $id) }
& $python.Source @profileArguments
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

& $python.Source (Join-Path $PSScriptRoot "audit_character_presentation.py") --repo $repo
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

$arguments = @(
    (Join-Path $PSScriptRoot "audit_animation_frames.py"),
    "--repo", $repo,
    "--output", $Output
)
foreach ($id in $Only) { $arguments += @("--only", $id) }
if ($NoContactSheets) { $arguments += "--no-contact-sheets" }

& $python.Source @arguments
exit $LASTEXITCODE
