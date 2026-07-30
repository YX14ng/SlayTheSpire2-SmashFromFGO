# Regenera los SpriteFrames de las formas de Oberon desde sus carpetas renderizadas.
$dir = "f:\Programs\SlayTheSpire2-SmashFromFGO\OberonPretender\OberonPretender\character"
$res = "res://OberonPretender/character"
$utf8 = New-Object System.Text.UTF8Encoding($false)

function New-FramesTres([string]$framesFolder, [string]$outFile) {
    $anims = @(
        @{ name = "idle"; src = "idle"; loop = "true"; speed = 15.0 },
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
                $id = "tex_$idn"
                $idn++
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

function New-StaticFramesTres([string]$textureFile, [string]$outFile) {
    $names = @("idle", "attack", "cast", "hurt", "die")
    $entries = foreach ($name in $names) {
        $loop = if ($name -eq "idle") { "true" } else { "false" }
        "{`n`"frames`": [{`"duration`": 1.0, `"texture`": ExtResource(`"tex_static`")}],`n`"loop`": $loop,`n`"name`": &`"$name`",`n`"speed`": 1.0`n}"
    }
    $tres = "[gd_resource type=`"SpriteFrames`" load_steps=2 format=3]`n`n[ext_resource type=`"Texture2D`" path=`"$res/$textureFile`" id=`"tex_static`"]`n`n[resource]`nanimations = [$($entries -join ', ')]`n"
    [IO.File]::WriteAllText("$dir\$outFile", $tres, $utf8)
    Write-Output "$outFile : textura estatica $textureFile"
}

New-FramesTres "frames_winter" "oberon_frames_winter.tres"
New-StaticFramesTres "vortigern_static.png" "oberon_frames_vortigern.tres"
Write-Output "LISTO"
