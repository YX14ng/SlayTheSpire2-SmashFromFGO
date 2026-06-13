# Orquesta el render de las 2 formas de Tiamat (Beast II, Babylonia).
# IDs de modelo VERIFICADOS en Atlas Academy (HTTP 200 + UnityFS, 2026-06-13):
#   9935400 = Femme Fatale (humanoide, mujer con cuernos). enemyCollectionDetail, CN 149.
#             ⚠️ Su puppet SOLO trae 3 clips: wait/spell/damage_01 — NO tiene attack ni die.
#             Por eso CLIP_OVERRIDE de render.gd mapea su "attack" -> "spell" (stand-in).
#   9935410 = Forma Bestia/Dragónica (Beast II gigante). type=enemy, trait superGiant.
#             Puppet COMPLETO: 8 clips (wait, attack_a/a02/b/b02/q, spell, damage_01).
#             ⚠️⚠️ TIENE 3 ATLAS DE TEXTURA (9935410_01/_02/_03.png). render.gd hoy aplica UN
#             solo atlas a todas las superficies -> la bestia NO renderiza bien hasta que el
#             renderer soporte multi-atlas (mapear surface->atlas). Por ahora queda BLOQUEADA;
#             la forma Femme Fatale (1 atlas) sí renderiza con el pipeline actual.
#   Escala: por superGiant el modelo viene gigante; render.gd normaliza por joint_head
#           (15.0/head_raw.y), así que la escala se auto-corrige (igual que Morgan Berserker).
#
# PRERREQUISITO (paso manual GUI, una vez por forma — ver docs/ANIMATIONS.md §1):
#   tools/fetch_fgo_bundle.ps1 -Ids 9935400,9935410   (ya descarga las bundles a assets/reference/bundles)
#   AssetStudioMod GUI -> Load <id>.bundle -> Animator 'chr' + TODOS los AnimationClips
#   -> click derecho "Export Animator + selected AnimationClips" -> assets/reference/extracted/<id>_anim/
#   (debe quedar Animator\chr\chr.fbx pesado, con los clips). Las texturas las uso de
#   assets/reference/bundles/ (atlas original de AA, coincide con las UV del FBX).
#
# Modos: -Mode list (clips/mesh/bones) | probe (movimiento) | debug (snaps) | render (medir+guardar)
param(
    [ValidateSet("list", "probe", "render", "measure", "save", "debug")][string]$Mode = "render",
    [string]$Only = ""
)
$ErrorActionPreference = "Continue"
$rp = "f:\Programs\SlayTheSpire2-SmashFromFGO\tools\render_project"
$md = "f:\Programs\SlayTheSpire2-SmashFromFGO\MegaDot\MegaDot_v4.5.1-stable_mono_win64_console.exe"
$ex = "f:\Programs\SlayTheSpire2-SmashFromFGO\assets\reference\extracted"
$bn = "f:\Programs\SlayTheSpire2-SmashFromFGO\assets\reference\bundles"
$modChar = "f:\Programs\SlayTheSpire2-SmashFromFGO\Tiamat\Tiamat\character"

# tex: atlas descargado de AA (assets/reference/bundles). La bestia usa _01 (PARCIAL —
# faltan _02/_03 hasta tener multi-atlas en render.gd).
$forms = @(
    @{ id = "9935400"; src = "$ex\9935400_anim\Animator\chr"; fbx = "chr.fbx"; tex = "$bn\9935400.png";    dest = "frames_femme" },
    @{ id = "9935410"; src = "$ex\9935410_anim\Animator\chr"; fbx = "chr.fbx"; tex = "$bn\9935410_01.png"; dest = "frames_beast" }
)
if ($Only) { $forms = @($forms | Where-Object { $_.id -eq $Only }) }

function Stage-Form($f) {
    Get-ChildItem $rp -Filter "*.png" | Where-Object { $_.Name -match '^\d{6,7}\.png$' } | Remove-Item -Force
    if (-not (Test-Path "$($f.src)\$($f.fbx)")) {
        Write-Output "  [X] falta $($f.src)\$($f.fbx) — extraer primero con la GUI (ver cabecera del script)"
        return $false
    }
    Copy-Item "$($f.src)\$($f.fbx)" "$rp\chr.fbx" -Force
    Copy-Item $f.tex "$rp\$($f.id).png" -Force
    Remove-Item "$rp\.godot" -Recurse -Force -ErrorAction SilentlyContinue
    & $md --headless --path $rp --import 2>&1 | Out-Null
    return $true
}

function Set-Pass([string]$pass) {
    $gd = [IO.File]::ReadAllText("$rp\render.gd")
    $gd = $gd -replace 'const PASS := "(measure|save|list|probe|debug|faceexp)"', "const PASS := `"$pass`""
    [IO.File]::WriteAllText("$rp\render.gd", $gd, (New-Object System.Text.UTF8Encoding($false)))
}

if ($Mode -eq "list") {
    Set-Pass "list"
    foreach ($f in $forms) {
        Write-Output "=== CLIPS $($f.id) ==="
        if (Stage-Form $f) { & $md --path $rp 2>&1 | Select-String -Pattern "CLIP:|MESH:|DONE" | ForEach-Object { $_.Line } }
    }
    exit 0
}

if ($Mode -eq "debug") {
    Set-Pass "debug"
    foreach ($f in $forms) {
        Write-Output "=== DEBUG $($f.id) ==="
        Remove-Item "$rp\debug_*.webp" -Force -ErrorAction SilentlyContinue
        if (-not (Stage-Form $f)) { continue }
        & $md --path $rp 2>&1 | Select-String -Pattern "SCALE|DEBUG|DONE" | ForEach-Object { $_.Line }
        $dbg = "$rp\debug_out\$($f.id)"
        New-Item -ItemType Directory -Force $dbg | Out-Null
        Get-ChildItem "$rp\debug_*.webp" -ErrorAction SilentlyContinue | Move-Item -Destination $dbg -Force
    }
    exit 0
}

if ($Mode -eq "probe") {
    Set-Pass "probe"
    foreach ($f in $forms) {
        Write-Output "=== PROBE $($f.id) ==="
        if (Stage-Form $f) { & $md --path $rp 2>&1 | Select-String -Pattern "MOTION|SCALE|DONE" | ForEach-Object { $_.Line } }
    }
    exit 0
}

if ($Mode -in @("render", "measure")) {
    Remove-Item "$rp\crop_union.txt" -Force -ErrorAction SilentlyContinue
    Set-Pass "measure"
    foreach ($f in $forms) {
        Write-Output "=== MEASURE $($f.id) ==="
        if (Stage-Form $f) { & $md --path $rp 2>&1 | Select-String -Pattern "MODEL|SCALE|UNION|DONE" | ForEach-Object { $_.Line } }
    }
    Write-Output "--- crop_union.txt ---"
    Get-Content "$rp\crop_union.txt" -ErrorAction SilentlyContinue
    if ($Mode -eq "measure") { exit 0 }
}

Set-Pass "save"
foreach ($f in $forms) {
    Write-Output "=== SAVE $($f.id) ==="
    if (-not (Stage-Form $f)) { continue }
    Remove-Item "$rp\frames" -Recurse -Force -ErrorAction SilentlyContinue
    & $md --path $rp 2>&1 | Select-String -Pattern "CROP|GROUND|CAM_CENTER|frames|DONE" | ForEach-Object { $_.Line }
    $dest = "$modChar\$($f.dest)"
    Remove-Item $dest -Recurse -Force -ErrorAction SilentlyContinue
    Copy-Item "$rp\frames" $dest -Recurse
    $mb = [math]::Round((Get-ChildItem $dest -Recurse -File | Measure-Object -Sum Length).Sum / 1MB, 1)
    Write-Output "  -> $($f.dest): $mb MB"
}
Write-Output "=== TODO LISTO ==="
