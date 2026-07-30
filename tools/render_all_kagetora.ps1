# Orquesta el render de las dos identidades de Kagetora:
#   303800 = Nagao Kagetora (forma inicial)
#   901820 = Uesugi Kenshin, tercera ascension (forma permanente)
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
$characterDir = Join-Path $repo "KagetoraLancer\KagetoraLancer\character"

$forms = @(
    @{ id = "303800"; name = "Kagetora"; dest = "frames_kagetora" },
    @{ id = "901820"; name = "Kenshin";  dest = "frames_kenshin" }
)
if ($Only) { $forms = @($forms | Where-Object { $_.id -eq $Only }) }

function Resolve-Form($form) {
    # Preferir el export auditado con clips. FBX_Animator es la salida del CLI 0.19
    # sin AnimationClip (se conserva solo como evidencia del bug del upstream).
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

function Set-Pass([string]$pass) {
    $path = Join-Path $renderProject "render.gd"
    $gd = [IO.File]::ReadAllText($path)
    $gd = $gd -replace 'const PASS := "(measure|save|list|listdeep|probe|check|debug|faceexp)"', "const PASS := `"$pass`""
    [IO.File]::WriteAllText($path, $gd, (New-Object System.Text.UTF8Encoding($false)))
}

if ($Mode -eq "list") {
    Set-Pass "list"
    foreach ($form in $forms) {
        Write-Output "=== CLIPS $($form.id) $($form.name) ==="
        if (Stage-Form $form) {
            & $megadot --path $renderProject 2>&1 |
                Select-String -Pattern "CLIP:|MESH:|DONE" |
                ForEach-Object { $_.Line }
        }
    }
    exit 0
}

if ($Mode -eq "listdeep") {
    Set-Pass "listdeep"
    foreach ($form in $forms) {
        Write-Output "=== RIG $($form.id) $($form.name) ==="
        if (Stage-Form $form) {
            & $megadot --path $renderProject 2>&1 |
                Select-String -Pattern "MESH:|BONE:|BLENDSHAPE:|DONE" |
                ForEach-Object { $_.Line }
        }
    }
    exit 0
}

if ($Mode -eq "check") {
    Set-Pass "check"
    foreach ($form in $forms) {
        Write-Output "=== CHECK $($form.id) $($form.name) ==="
        if (Stage-Form $form) {
            & $megadot --path $renderProject 2>&1 |
                Select-String -Pattern "MODEL|SCALE|DONE|ERROR" |
                ForEach-Object { $_.Line }
        }
    }
    exit 0
}

if ($Mode -eq "probe") {
    Set-Pass "probe"
    foreach ($form in $forms) {
        Write-Output "=== PROBE $($form.id) $($form.name) ==="
        if (Stage-Form $form) {
            & $megadot --path $renderProject 2>&1 |
                Select-String -Pattern "MOTION|SCALE|DONE" |
                ForEach-Object { $_.Line }
        }
    }
    exit 0
}

if ($Mode -eq "debug") {
    foreach ($form in $forms) {
        Write-Output "=== DEBUG $($form.id) $($form.name) ==="
        if (-not (Stage-Form $form)) { continue }
        Remove-Item (Join-Path $renderProject "debug_*.webp") -Force -ErrorAction SilentlyContinue
        $debugArgs = @(
            "--path", $renderProject,
            "--",
            "--pass=debug",
            "--debug-from=$DebugFrom",
            "--debug-to=$DebugTo",
            "--debug-step=$DebugStep"
        )
        if ($DebugClips) { $debugArgs += "--debug-clips=$DebugClips" }
        & $megadot @debugArgs 2>&1 |
            Select-String -Pattern "SCALE|DEBUG|DONE|ERROR" |
            ForEach-Object { $_.Line }
        $debugDir = Join-Path $renderProject "debug_out\$($form.id)"
        New-Item -ItemType Directory -Force $debugDir | Out-Null
        Get-ChildItem $renderProject -Filter "debug_*.webp" | Move-Item -Destination $debugDir -Force
    }
    exit 0
}

if ($Mode -in @("render", "measure")) {
    Remove-Item (Join-Path $renderProject "crop_union.txt") -Force -ErrorAction SilentlyContinue
    Set-Pass "measure"
    foreach ($form in $forms) {
        Write-Output "=== MEASURE $($form.id) $($form.name) ==="
        if (Stage-Form $form) {
            & $megadot --path $renderProject 2>&1 |
                Select-String -Pattern "MODEL|SCALE|UNION|DONE" |
                ForEach-Object { $_.Line }
        }
    }
    Get-Content (Join-Path $renderProject "crop_union.txt") -ErrorAction SilentlyContinue
    if ($Mode -eq "measure") { exit 0 }
}

Set-Pass "save"
foreach ($form in $forms) {
    Write-Output "=== SAVE $($form.id) $($form.name) ==="
    if (-not (Stage-Form $form)) { continue }
    Remove-Item (Join-Path $renderProject "frames") -Recurse -Force -ErrorAction SilentlyContinue
    & $megadot --path $renderProject 2>&1 |
        Select-String -Pattern "CROP|GROUND|CAM_CENTER|frames|DONE" |
        ForEach-Object { $_.Line }

    $destination = Join-Path $characterDir $form.dest
    Remove-Item $destination -Recurse -Force -ErrorAction SilentlyContinue
    Copy-Item (Join-Path $renderProject "frames") $destination -Recurse
    $bytes = (Get-ChildItem $destination -Recurse -File | Measure-Object -Sum Length).Sum
    Write-Output "  -> $($form.dest): $([math]::Round($bytes / 1MB, 1)) MB"
}
Write-Output "=== KAGETORA/KENSHIN LISTO ==="
