param(
    [ValidateSet("list", "listdeep", "probe", "check", "render", "measure", "save", "debug")]
    [string]$Mode = "list",
    [string]$DebugClips = "",
    [int]$DebugFrom = 0,
    [int]$DebugTo = -1,
    [int]$DebugStep = 5,
    # Override del binario (p.ej. MegaDotLinux/MegaDot_v4.5.1-stable_mono_linux.x86_64).
    [string]$MegaDot = ""
)

$ErrorActionPreference = "Continue"
$repo = Split-Path $PSScriptRoot -Parent
$renderProject = Join-Path $repo "tools\render_project"
$megadot = if ($MegaDot) { $MegaDot } else { Join-Path $repo "MegaDot\MegaDot_v4.5.1-stable_mono_win64_console.exe" }
$source = Join-Path $repo "assets\reference\extracted\400400_anim\Animator\chr"
$texture = Join-Path $repo "assets\reference\bundles\400400.png"
$characterDir = Join-Path $repo "AstolfoRider\AstolfoRider\character"

function Stage-Astolfo {
    if (-not (Test-Path (Join-Path $source "chr.fbx"))) {
        Write-Output "[X] falta el FBX oficial exportado de 400400"
        return $false
    }
    Get-ChildItem $renderProject -Filter "*.png" |
        Where-Object { $_.BaseName -match '^[0-9]+' } | Remove-Item -Force
    Remove-Item (Join-Path $renderProject "chr.fbx") -Force -ErrorAction SilentlyContinue
    Remove-Item (Join-Path $renderProject "anim.fbx") -Force -ErrorAction SilentlyContinue
    Copy-Item (Join-Path $source "chr.fbx") (Join-Path $renderProject "chr.fbx") -Force
    if (Test-Path $texture) { Copy-Item $texture (Join-Path $renderProject "400400.png") -Force }
    Get-ChildItem $source -Filter "*.png" | ForEach-Object {
        Copy-Item $_.FullName (Join-Path $renderProject $_.Name) -Force
    }
    Remove-Item (Join-Path $renderProject ".godot") -Recurse -Force -ErrorAction SilentlyContinue
    & $megadot --headless --path $renderProject --import 2>&1 | Out-Null
    return $true
}

function Invoke-Renderer([string]$pass, [string[]]$patterns, [string[]]$extra = @()) {
    if (-not (Stage-Astolfo)) { return }
    $args = @("--path", $renderProject, "--", "--pass=$pass") + $extra
    & $megadot @args 2>&1 | Select-String -Pattern $patterns | ForEach-Object { $_.Line }
}

if ($Mode -in @("list", "listdeep", "check", "probe")) {
    $patterns = switch ($Mode) {
        "list" { @("CLIP:", "DONE", "ERROR") }
        "listdeep" { @("MESH:", "BONE:", "BLENDSHAPE:", "DONE", "ERROR") }
        "check" { @("MODEL", "SCALE", "DONE", "ERROR") }
        "probe" { @("MOTION", "SCALE", "DONE", "ERROR") }
    }
    Invoke-Renderer $Mode $patterns
    exit 0
}

if ($Mode -eq "debug") {
    Remove-Item (Join-Path $renderProject "debug_*.webp") -Force -ErrorAction SilentlyContinue
    $extra = @("--debug-from=$DebugFrom", "--debug-to=$DebugTo", "--debug-step=$DebugStep")
    if ($DebugClips) { $extra += "--debug-clips=$DebugClips" }
    Invoke-Renderer "debug" @("SCALE", "DEBUG", "DONE", "ERROR") $extra
    $debugDir = Join-Path $renderProject "debug_out\400400"
    New-Item -ItemType Directory -Force $debugDir | Out-Null
    Get-ChildItem $renderProject -Filter "debug_*.webp" | Move-Item -Destination $debugDir -Force
    exit 0
}

if ($Mode -in @("render", "measure")) {
    Remove-Item (Join-Path $renderProject "crop_union.txt") -Force -ErrorAction SilentlyContinue
    Invoke-Renderer "measure" @("MODEL", "SCALE", "UNION", "DONE", "ERROR")
    Get-Content (Join-Path $renderProject "crop_union.txt") -ErrorAction SilentlyContinue
    if ($Mode -eq "measure") { exit 0 }
}

Remove-Item (Join-Path $renderProject "frames") -Recurse -Force -ErrorAction SilentlyContinue
Invoke-Renderer "save" @("CROP", "GROUND", "CAM_CENTER", "frames", "DONE", "ERROR")
$frames = Join-Path $renderProject "frames"
if (Test-Path $frames) {
    $destination = Join-Path $characterDir "frames"
    if (Test-Path $destination) { Remove-Item $destination -Recurse -Force }
    Copy-Item $frames $destination -Recurse
    $bytes = (Get-ChildItem $destination -Recurse -File | Measure-Object -Sum Length).Sum
    Write-Output "Astolfo 400400 -> frames: $([math]::Round($bytes / 1MB, 1)) MB"
}
