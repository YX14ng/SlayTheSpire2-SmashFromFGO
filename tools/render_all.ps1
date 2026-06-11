# Orquesta el re-render sin clipping de las 3 formas:
# medir x3 (acumula crop_union.txt) -> guardar x3 (recortado al union) -> copia al mod.
$ErrorActionPreference = "Continue"
$rp = "f:\Programs\SlayTheSpire2-SmashFromFGO\tools\render_project"
$md = "f:\Programs\SlayTheSpire2-SmashFromFGO\MegaDot\MegaDot_v4.5.1-stable_mono_win64_console.exe"
$ex = "f:\Programs\SlayTheSpire2-SmashFromFGO\assets\reference\extracted"
$modChar = "f:\Programs\SlayTheSpire2-SmashFromFGO\MashShielder\MashShielder\character"

$forms = @(
    @{ id = "800100"; src = "$ex\800100_anim\Animator\chr"; dest = "frames" },
    @{ id = "800150"; src = "$ex\800150_anim\Animator\chr"; dest = "frames_ortinax" },
    @{ id = "800200"; src = "$ex\800200_anim\Animator\chr"; dest = "frames_paladin" }
)

function Stage-Form($f) {
    Get-ChildItem $rp -Filter "8*.png" | Remove-Item -Force
    Copy-Item "$($f.src)\chr.fbx" "$rp\chr.fbx" -Force
    Copy-Item "$($f.src)\$($f.id).png" "$rp\$($f.id).png" -Force
    Remove-Item "$rp\.godot" -Recurse -Force -ErrorAction SilentlyContinue
    & $md --headless --path $rp --import 2>&1 | Out-Null
}

function Set-Pass([string]$pass) {
    $gd = [IO.File]::ReadAllText("$rp\render.gd")
    $gd = $gd -replace 'const PASS := "(measure|save)"', "const PASS := `"$pass`""
    [IO.File]::WriteAllText("$rp\render.gd", $gd, (New-Object System.Text.UTF8Encoding($false)))
}

Remove-Item "$rp\crop_union.txt" -Force -ErrorAction SilentlyContinue

Set-Pass "measure"
foreach ($f in $forms) {
    Write-Output "=== MEASURE $($f.id) ==="
    Stage-Form $f
    & $md --path $rp 2>&1 | Select-String -Pattern "MODEL|SCALE|UNION|DONE" | ForEach-Object { $_.Line }
}
Write-Output "--- crop_union.txt ---"
Get-Content "$rp\crop_union.txt"

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
