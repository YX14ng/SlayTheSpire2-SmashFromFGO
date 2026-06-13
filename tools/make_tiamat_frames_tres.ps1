# Regenera los SpriteFrames .tres de las 2 formas de Tiamat desde las carpetas de frames (webp).
# Femme Fatale (humano) = frames_femme -> tiamat_frames_human.tres (el codigo lo referencia asi).
# Bestia = frames_beast -> tiamat_frames_beast.tres.
$dir = "f:\Programs\SlayTheSpire2-SmashFromFGO\Tiamat\Tiamat\character"
$res = "res://Tiamat/character"
$utf8 = New-Object System.Text.UTF8Encoding($false)

function New-FramesTres([string]$framesFolder, [string]$outFile, [double]$idleSpeed = 30.0) {
    $anims = @(
        @{ name = "idle"; src = "idle"; loop = "true"; speed = $idleSpeed },
        @{ name = "attack"; src = "attack"; loop = "false"; speed = 30.0 },
        @{ name = "cast"; src = "cast"; loop = "false"; speed = 15.0 },
        @{ name = "hurt"; src = "hurt"; loop = "false"; speed = 30.0 },
        @{ name = "die"; src = "hurt"; loop = "false"; speed = 30.0 }
    )
    $ext = New-Object System.Text.StringBuilder
    $animEntries = @()
    $extIds = @{}
    $idn = 1
    foreach ($a in $anims) {
        $files = Get-ChildItem "$dir\$framesFolder\$($a.src)" -Filter "*.webp" | Sort-Object Name
        if ($files.Count -eq 0) { throw "sin frames webp en $framesFolder/$($a.src)" }
        $frameRefs = @()
        foreach ($fl in $files) {
            $key = "$($a.src)/$($fl.Name)"
            if (-not $extIds.ContainsKey($key)) {
                $id = "tex_$idn"; $idn++
                $extIds[$key] = $id
                [void]$ext.AppendLine("[ext_resource type=`"Texture2D`" path=`"$res/$framesFolder/$key`" id=`"$id`"]")
            }
            $frameRefs += "{`"duration`": 1.0, `"texture`": ExtResource(`"$($extIds[$key])`")}"
        }
        $animEntries += "{`n`"frames`": [$($frameRefs -join ', ')],`n`"loop`": $($a.loop),`n`"name`": &`"$($a.name)`",`n`"speed`": $($a.speed)`n}"
    }
    $tres = "[gd_resource type=`"SpriteFrames`" load_steps=$idn format=3]`n`n$($ext.ToString())`n[resource]`nanimations = [$($animEntries -join ', ')]`n"
    [IO.File]::WriteAllText("$dir\$outFile", $tres, $utf8)
    Write-Output "$outFile : $($idn - 1) texturas"
}

New-FramesTres "frames_femme" "tiamat_frames_human.tres"
New-FramesTres "frames_beast" "tiamat_frames_beast.tres"
Write-Output "LISTO"
