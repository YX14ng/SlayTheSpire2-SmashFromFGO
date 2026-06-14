# Render de los 5 personajes nuevos (forma única c/u) desde sus FBX exportados.
# IDs: 100800 Siegfried, 100900 Mordred, 200200 Gilgamesh, 102700 Okita, 2800100 Oberon.
# Modos: list (clips) | probe | measure (crop) | save (render+copia al mod) | debug
param([ValidateSet("list","probe","render","measure","save","debug")][string]$Mode="list",[string]$Only="")
$ErrorActionPreference="Continue"
$R="f:\Programs\SlayTheSpire2-SmashFromFGO"
$rp="$R\tools\render_project"
$md="$R\MegaDot\MegaDot_v4.5.1-stable_mono_win64_console.exe"
$ex="$R\assets\reference\extracted"; $bn="$R\assets\reference\bundles"
$forms=@(
 @{id="100800"; name="Siegfried"; modChar="$R\SiegfriedSaber\SiegfriedSaber\character"},
 @{id="100900"; name="Mordred";   modChar="$R\MordredSaber\MordredSaber\character"},
 @{id="200200"; name="Gilgamesh"; modChar="$R\GilgameshArcher\GilgameshArcher\character"},
 @{id="102700"; name="Okita";     modChar="$R\OkitaSaber\OkitaSaber\character"},
 @{id="2800100";name="Oberon";    modChar="$R\OberonPretender\OberonPretender\character"}
)
foreach($f in $forms){
 $base="$ex\$($f.id)_anim\Animator"
 if(Test-Path "$base\chr\chr.fbx"){ $f.src="$base\chr"; $f.fbx="chr.fbx" }
 else { $f.src="$base\model"; $f.fbx="model.fbx" }
 $f.tex="$bn\$($f.id).png"; $f.dest="frames"
}
if($Only){ $forms=@($forms|Where-Object{$_.id -eq $Only}) }

function Stage($f){
 Get-ChildItem $rp -Filter "*.png" | Where-Object{$_.BaseName -match '^[0-9]'} | ForEach-Object{Remove-Item $_.FullName -Force}
 Remove-Item "$rp\chr.fbx" -Force -ErrorAction SilentlyContinue
 if(-not(Test-Path "$($f.src)\$($f.fbx)")){ Write-Output "  [X] falta $($f.src)\$($f.fbx)"; return $false }
 Copy-Item "$($f.src)\$($f.fbx)" "$rp\chr.fbx" -Force
 if(Test-Path $f.tex){ Copy-Item $f.tex "$rp\$($f.id).png" -Force }
 Get-ChildItem "$($f.src)" -Filter "*.png" | ForEach-Object{ Copy-Item $_.FullName "$rp\$($_.Name)" -Force }
 Remove-Item "$rp\.godot" -Recurse -Force -ErrorAction SilentlyContinue
 & $md --headless --path $rp --import 2>&1 | Out-Null
 return $true
}
function SetPass($p){ $g=[IO.File]::ReadAllText("$rp\render.gd"); $g=$g -replace 'const PASS := "(measure|save|list|probe|debug|faceexp)"',"const PASS := `"$p`""; [IO.File]::WriteAllText("$rp\render.gd",$g,(New-Object System.Text.UTF8Encoding($false))) }

if($Mode -eq "list"){ SetPass "list"; foreach($f in $forms){ Write-Output "=== CLIPS $($f.id) $($f.name) ==="; if(Stage $f){ & $md --path $rp 2>&1 | Select-String -Pattern "CLIP:|MESH:|DONE" | ForEach-Object{$_.Line} } }; exit 0 }
if($Mode -eq "probe"){ SetPass "probe"; foreach($f in $forms){ Write-Output "=== PROBE $($f.id) ==="; if(Stage $f){ & $md --path $rp 2>&1 | Select-String -Pattern "MOTION|SCALE|DONE" | ForEach-Object{$_.Line} } }; exit 0 }
if($Mode -in @("render","measure")){ Remove-Item "$rp\crop_union.txt" -Force -ErrorAction SilentlyContinue; SetPass "measure"
 foreach($f in $forms){ Write-Output "=== MEASURE $($f.id) ==="; if(Stage $f){ & $md --path $rp 2>&1 | Select-String -Pattern "MODEL|SCALE|UNION|DONE" | ForEach-Object{$_.Line} } }
 Get-Content "$rp\crop_union.txt" -ErrorAction SilentlyContinue; if($Mode -eq "measure"){ exit 0 } }
SetPass "save"
foreach($f in $forms){ Write-Output "=== SAVE $($f.id) $($f.name) ==="; if(-not(Stage $f)){continue}
 Remove-Item "$rp\frames" -Recurse -Force -ErrorAction SilentlyContinue
 & $md --path $rp 2>&1 | Select-String -Pattern "CROP|GROUND|frames|DONE" | ForEach-Object{$_.Line}
 $dest="$($f.modChar)\$($f.dest)"; Remove-Item $dest -Recurse -Force -ErrorAction SilentlyContinue; Copy-Item "$rp\frames" $dest -Recurse
 $mb=[math]::Round((Get-ChildItem $dest -Recurse -File|Measure-Object -Sum Length).Sum/1MB,1); Write-Output "  -> $($f.name)\$($f.dest): $mb MB" }
Write-Output "=== TODO LISTO ==="