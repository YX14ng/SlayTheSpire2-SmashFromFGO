# Orquesta el render de las variantes visuales de Shuten Douji:
#   602100 = Assassin, apariencia principal
#   504000 = Caster de verano
#
# Modos: list | listdeep | probe | check | render | measure | save | debug
param(
    [ValidateSet("list", "listdeep", "probe", "check", "render", "measure", "save", "debug")]
    [string]$Mode = "list",
    [string]$Only = "",
    [string]$DebugClips = "",
    [int]$DebugFrom = 0,
    [int]$DebugTo = -1,
    [int]$DebugStep = 5
)

$ErrorActionPreference = "Continue"
$repo = Split-Path $PSScriptRoot -Parent
$renderProject = Join-Path $repo "tools\render_project"
$megadot = Join-Path $repo "MegaDot\MegaDot_v4.5.1-stable_mono_win64_console.exe"
$extracted = Join-Path $repo "assets\reference\extracted"
$bundles = Join-Path $repo "assets\reference\bundles"
$characterDir = Join-Path $repo "ShutenDouji\ShutenDouji\character"

$forms = @(
    @{ id = "602100"; name = "Assassin";     dest = "frames" },
    @{ id = "504000"; name = "Caster";       dest = "frames_caster" }
)
if ($Only) { $forms = @($forms | Where-Object { $_.id -eq $Only }) }

function Resolve-Form($form) {
    $base = Join-Path $extracted "$($form.id)_anim\Animator"
    if (-not (Test-Path $base)) { $base = Join-Path $extracted "$($form.id)_anim\FBX_Animator" }
    if (Test-Path (Join-Path $base "chr\chr.fbx")) {
        $form.src = Join-Path $base "chr"
        $form.fbx = "chr.fbx"
    } elseif (Test-Path (Join-Path $base "model\model.fbx")) {
        $form.src = Join-Path $base "model"
        $form.fbx = "model.fbx"
    }
    $form.tex = Join-Path $bundles "$($form.id).png"
}

function Stage-Form($form) {
    Resolve-Form $form
    $sourceFbx = if ($form.src) { Join-Path $form.src $form.fbx } else { "" }
    if (-not $sourceFbx -or -not (Test-Path $sourceFbx)) {
        Write-Output "  [X] falta el FBX exportado de $($form.id)"
        return $false
    }

    Get-ChildItem $renderProject -Filter "*.png" |
        Where-Object { $_.BaseName -match '^[0-9]+' } |
        Remove-Item -Force
    Remove-Item (Join-Path $renderProject "chr.fbx") -Force -ErrorAction SilentlyContinue
    Remove-Item (Join-Path $renderProject "anim.fbx") -Force -ErrorAction SilentlyContinue

    Copy-Item $sourceFbx (Join-Path $renderProject "chr.fbx") -Force
    if (Test-Path $form.tex) {
        Copy-Item $form.tex (Join-Path $renderProject "$($form.id).png") -Force
    }
    Get-ChildItem $form.src -Filter "*.png" | ForEach-Object {
        Copy-Item $_.FullName (Join-Path $renderProject $_.Name) -Force
    }

    Remove-Item (Join-Path $renderProject ".godot") -Recurse -Force -ErrorAction SilentlyContinue
    & $megadot --headless --path $renderProject --import 2>&1 | Out-Null
    return $true
}

function Invoke-Renderer($form, [string]$pass, [string[]]$patterns, [string[]]$extraArgs = @()) {
    if (-not (Stage-Form $form)) { return }
    # La pasada visual necesita el renderizador de GPU. --headless cae al backend de
    # software en esta maquina y vuelve cada fotograma varios minutos mas lento.
    $args = @("--path", $renderProject, "--", "--pass=$pass") + $extraArgs
    & $megadot @args 2>&1 |
        Select-String -Pattern $patterns |
        ForEach-Object { $_.Line }
}

if ($Mode -in @("list", "listdeep", "check", "probe")) {
    foreach ($form in $forms) {
        Write-Output "=== $($Mode.ToUpperInvariant()) $($form.id) $($form.name) ==="
        $patterns = switch ($Mode) {
            "list"     { @("CLIP:", "DONE", "ERROR") }
            "listdeep" { @("MESH:", "BONE:", "BLENDSHAPE:", "DONE", "ERROR") }
            "check"    { @("MODEL", "SCALE", "DONE", "ERROR") }
            "probe"    { @("MOTION", "SCALE", "DONE", "ERROR") }
        }
        Invoke-Renderer $form $Mode $patterns
    }
    exit 0
}

if ($Mode -eq "debug") {
    foreach ($form in $forms) {
        Write-Output "=== DEBUG $($form.id) $($form.name) ==="
        Remove-Item (Join-Path $renderProject "debug_*.webp") -Force -ErrorAction SilentlyContinue
        $extra = @("--debug-from=$DebugFrom", "--debug-to=$DebugTo", "--debug-step=$DebugStep")
        if ($DebugClips) { $extra += "--debug-clips=$DebugClips" }
        Invoke-Renderer $form "debug" @("SCALE", "DEBUG", "DONE", "ERROR") $extra
        $debugDir = Join-Path $renderProject "debug_out\$($form.id)"
        New-Item -ItemType Directory -Force $debugDir | Out-Null
        Get-ChildItem $renderProject -Filter "debug_*.webp" | Move-Item -Destination $debugDir -Force
    }
    exit 0
}

if ($Mode -in @("render", "measure")) {
    Remove-Item (Join-Path $renderProject "crop_union.txt") -Force -ErrorAction SilentlyContinue
    foreach ($form in $forms) {
        Write-Output "=== MEASURE $($form.id) $($form.name) ==="
        Invoke-Renderer $form "measure" @("MODEL", "SCALE", "UNION", "DONE", "ERROR")
    }
    Get-Content (Join-Path $renderProject "crop_union.txt") -ErrorAction SilentlyContinue
    if ($Mode -eq "measure") { exit 0 }
}

foreach ($form in $forms) {
    Write-Output "=== SAVE $($form.id) $($form.name) ==="
    Remove-Item (Join-Path $renderProject "frames") -Recurse -Force -ErrorAction SilentlyContinue
    Invoke-Renderer $form "save" @("CROP", "GROUND", "CAM_CENTER", "frames", "DONE", "ERROR")

    $sourceFrames = Join-Path $renderProject "frames"
    if (-not (Test-Path $sourceFrames)) { continue }
    $destination = Join-Path $characterDir $form.dest
    Remove-Item $destination -Recurse -Force -ErrorAction SilentlyContinue
    Copy-Item $sourceFrames $destination -Recurse
    $bytes = (Get-ChildItem $destination -Recurse -File | Measure-Object -Sum Length).Sum
    Write-Output "  -> $($form.dest): $([math]::Round($bytes / 1MB, 1)) MB"
}
Write-Output "=== SHUTEN LISTA ==="
