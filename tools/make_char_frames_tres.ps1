# Genera el SpriteFrames <short>_frames.tres (forma única) desde character/frames/{idle,attack,cast,hurt}.
# La escena <short>_visuals.tscn (scaffold) ya lo referencia. die reusa hurt.
# Uso: .\tools\make_char_frames_tres.ps1 -ProjId MordredSaber -Short mordred
param([Parameter(Mandatory)][string]$ProjId,[Parameter(Mandatory)][string]$Short,[string]$Root='f:\Programs\SlayTheSpire2-SmashFromFGO')
$dir = "$Root\$ProjId\$ProjId\character"
$res = "res://$ProjId/character"
$utf8 = New-Object System.Text.UTF8Encoding($false)
$anims = @(
 @{name="idle";   src="idle";   loop="true";  speed=15.0},
 @{name="attack"; src="attack"; loop="false"; speed=30.0},
 @{name="cast";   src="cast";   loop="false"; speed=15.0},
 @{name="hurt";   src="hurt";   loop="false"; speed=30.0},
 @{name="die";    src="hurt";   loop="false"; speed=30.0}
)
$ext = New-Object System.Text.StringBuilder
$animEntries=@(); $extIds=@{}; $idn=1
foreach($a in $anims){
 $files = Get-ChildItem "$dir\frames\$($a.src)" -Filter "*.webp" -ErrorAction SilentlyContinue | Sort-Object Name
 if($files.Count -eq 0){ throw "sin frames en frames/$($a.src) ($ProjId)" }
 $refs=@()
 foreach($fl in $files){
  $key="$($a.src)/$($fl.Name)"
  if(-not $extIds.ContainsKey($key)){ $id="tex_$idn"; $idn++; $extIds[$key]=$id; [void]$ext.AppendLine("[ext_resource type=`"Texture2D`" path=`"$res/frames/$key`" id=`"$id`"]") }
  $refs += "{`"duration`": 1.0, `"texture`": ExtResource(`"$($extIds[$key])`")}"
 }
 $animEntries += "{`n`"frames`": [$($refs -join ', ')],`n`"loop`": $($a.loop),`n`"name`": &`"$($a.name)`",`n`"speed`": $($a.speed)`n}"
}
$tres = "[gd_resource type=`"SpriteFrames`" load_steps=$idn format=3]`n`n$($ext.ToString())`n[resource]`nanimations = [$($animEntries -join ', ')]`n"
[IO.File]::WriteAllText("$dir\${Short}_frames.tres", $tres, $utf8)
Write-Output "${Short}_frames.tres : $($idn-1) texturas"