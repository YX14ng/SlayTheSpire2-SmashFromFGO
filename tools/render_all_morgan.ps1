# Orquesta el render de las 3 formas de Morgan:
#   -Mode list   -> lista los clips de animacion de cada modelo (para definir SELECT en render.gd)
#   -Mode render -> medir x3 (acumula crop_union.txt) -> guardar x3 -> copia al mod.
# Los dos modelos Berserker (704020/704030) exportan como Animator\model\model.fbx;
# Aesc (505320) exporta como Animator\chr\chr.fbx. Se normaliza copiando como chr.fbx.
param(
    [ValidateSet("list", "probe", "render", "measure", "save", "debug")][string]$Mode = "render",
    [string]$Only = ""
)
$ErrorActionPreference = "Continue"
$rp = "f:\Programs\SlayTheSpire2-SmashFromFGO\tools\render_project"
$md = "f:\Programs\SlayTheSpire2-SmashFromFGO\MegaDot\MegaDot_v4.5.1-stable_mono_win64_console.exe"
$ex = "f:\Programs\SlayTheSpire2-SmashFromFGO\assets\reference\extracted"
$modChar = "f:\Programs\SlayTheSpire2-SmashFromFGO\MorganBerserker\MorganBerserker\character"

$forms = @(
    @{ id = "704020"; src = "$ex\704020_anim\Animator\model"; fbx = "model.fbx"; dest = "frames_queen" },
    @{ id = "505320"; src = "$ex\505320_anim\Animator\chr"; fbx = "chr.fbx"; dest = "frames_aesc" },
    @{ id = "704030"; src = "$ex\704030_anim\Animator\model"; fbx = "model.fbx"; dest = "frames_winter" }
)
if ($Only) { $forms = @($forms | Where-Object { $_.id -eq $Only }) }

function Stage-Form($f) {
    Get-ChildItem $rp -Filter "*.png" | Where-Object { $_.Name -match '^\d{6}\.png$' } | Remove-Item -Force
    Copy-Item "$($f.src)\$($f.fbx)" "$rp\chr.fbx" -Force
    Copy-Item "$($f.src)\$($f.id).png" "$rp\$($f.id).png" -Force
    Remove-Item "$rp\.godot" -Recurse -Force -ErrorAction SilentlyContinue
    & $md --headless --path $rp --import 2>&1 | Out-Null
}

function Set-Pass([string]$pass) {
    $gd = [IO.File]::ReadAllText("$rp\render.gd")
    $gd = $gd -replace 'const PASS := "(measure|save|list|probe|debug)"', "const PASS := `"$pass`""
    [IO.File]::WriteAllText("$rp\render.gd", $gd, (New-Object System.Text.UTF8Encoding($false)))
}

if ($Mode -eq "list") {
    Set-Pass "list"
    foreach ($f in $forms) {
        Write-Output "=== CLIPS $($f.id) ==="
        Stage-Form $f
        & $md --path $rp 2>&1 | Select-String -Pattern "CLIP:|DONE" | ForEach-Object { $_.Line }
    }
    exit 0
}

if ($Mode -eq "debug") {
    Set-Pass "debug"
    foreach ($f in $forms) {
        Write-Output "=== DEBUG $($f.id) ==="
        Remove-Item "$rp\debug_*.webp" -Force -ErrorAction SilentlyContinue
        Stage-Form $f
        & $md --path $rp 2>&1 | Select-String -Pattern "SCALE|DEBUG|DONE" | ForEach-Object { $_.Line }
        $dbg = "$rp\debug_out\$($f.id)"
        New-Item -ItemType Directory -Force $dbg | Out-Null
        Get-ChildItem "$rp\debug_*.webp" | Move-Item -Destination $dbg -Force
    }
    exit 0
}

if ($Mode -eq "probe") {
    Set-Pass "probe"
    foreach ($f in $forms) {
        Write-Output "=== PROBE $($f.id) ==="
        Stage-Form $f
        & $md --path $rp 2>&1 | Select-String -Pattern "MOTION|SCALE|DONE" | ForEach-Object { $_.Line }
    }
    exit 0
}

if ($Mode -in @("render", "measure")) {
    Remove-Item "$rp\crop_union.txt" -Force -ErrorAction SilentlyContinue
    Set-Pass "measure"
    foreach ($f in $forms) {
        Write-Output "=== MEASURE $($f.id) ==="
        Stage-Form $f
        & $md --path $rp 2>&1 | Select-String -Pattern "MODEL|SCALE|UNION|DONE" | ForEach-Object { $_.Line }
    }
    Write-Output "--- crop_union.txt ---"
    Get-Content "$rp\crop_union.txt"
    if ($Mode -eq "measure") { exit 0 }
}

Set-Pass "save"
foreach ($f in $forms) {
    Write-Output "=== SAVE $($f.id) ==="
    Stage-Form $f
    Remove-Item "$rp\frames" -Recurse -Force -ErrorAction SilentlyContinue
    & $md --path $rp 2>&1 | Select-String -Pattern "CROP|GROUND|CAM_CENTER|frames|DONE" | ForEach-Object { $_.Line }
    $dest = "$modChar\$($f.dest)"
    Remove-Item $dest -Recurse -Force -ErrorAction SilentlyContinue
    Copy-Item "$rp\frames" $dest -Recurse
    $mb = [math]::Round((Get-ChildItem $dest -Recurse -File | Measure-Object -Sum Length).Sum / 1MB, 1)
    Write-Output "  -> $($f.dest): $mb MB"
}
Write-Output "=== TODO LISTO ==="
