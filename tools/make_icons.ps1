# Descarga iconos oficiales de FGO (skills para powers, items para reliquias)
# y los instala en el mod. Genera los _outline de reliquias (silueta blanca).
Add-Type -AssemblyName System.Drawing
$ErrorActionPreference = "Continue"

$img = "f:\Programs\SlayTheSpire2-SmashFromFGO\MashShielder\MashShielder\images"
$cache = "f:\Programs\SlayTheSpire2-SmashFromFGO\assets\reference\ce\icons"
New-Item -ItemType Directory -Force $cache | Out-Null
$skill = "https://static.atlasacademy.io/JP/SkillIcons"
$items = "https://static.atlasacademy.io/JP/Items"

$powers = @{
    "np_charge_power"            = "$skill/skill_00601.png"
    "bulwark_power"              = "$skill/skill_00419.png"
    "shielder_form_power"        = "$skill/skill_00400.png"
    "ortinax_form_power"         = "$skill/skill_00306.png"
    "paladin_form_power"         = "$skill/skill_00300.png"
    "form_shifted_power"         = "$skill/skill_00303.png"
    "intercept_power"            = "$skill/skill_00700.png"
    "provoke_power"              = "$skill/skill_00700.png"
    "mobile_wall_power"          = "$skill/skill_00311.png"
    "defensive_formation_power"  = "$skill/skill_00604.png"
    "iron_will_power"            = "$skill/skill_00400.png"
    "wall_doctrine_power"        = "$skill/skill_00602.png"
    "conceptual_ammo_power"      = "$skill/skill_00506.png"
    "ortinax_servos_power"       = "$skill/skill_00306.png"
    "chaldea_library_power"      = "$skill/skill_00605.png"
    "demi_servant_power"         = "$skill/skill_00403.png"
    "distant_utopia_castle_power" = "$skill/skill_00419.png"
    "homunculus_heart_power"     = "$skill/skill_00305.png"
    "senpai_promise_power"       = "$skill/skill_00601.png"
    "pioneer_of_the_stars_power" = "$skill/skill_00603.png"
    "fou_miracle_power"          = "$skill/skill_00305.png"
    "absolute_wall_power"        = "$skill/skill_00403.png"
    "cover_power"                = "$skill/skill_00700.png"
    "camelot_manifested_power"   = "$skill/skill_00508.png"
}

$relics = @{
    # Icono de clase Shielder (wiki fandom; ya cacheado como Shieldergold.png)
    "round_table_fragment" = "https://static.wikia.nocookie.net/fategrandorder/images/e/e3/Shieldergold.png"
    "lord_camelot_restored" = "$items/7999.png"
    "fou_amulet"           = "$items/6501.png"
    "spare_glasses"        = "$items/6999.png"
    "ortinax_core"         = "$items/6510.png"
    "a_team_diary"         = "$items/6531.png"
    "mash_bond"            = "$items/94006404.png"
}

function Get-Cached([string]$url) {
    $name = ($url -split '/')[-1]
    $path = "$cache\$name"
    if (-not (Test-Path $path)) { Invoke-WebRequest $url -OutFile $path }
    return $path
}

function Make-Outline([string]$srcPath, [string]$outPath) {
    $src = New-Object System.Drawing.Bitmap($srcPath)
    $bmp = New-Object System.Drawing.Bitmap($src.Width, $src.Height)
    for ($y = 0; $y -lt $src.Height; $y++) {
        for ($x = 0; $x -lt $src.Width; $x++) {
            $a = $src.GetPixel($x, $y).A
            if ($a -gt 16) { $bmp.SetPixel($x, $y, [System.Drawing.Color]::FromArgb($a, 255, 255, 255)) }
        }
    }
    $bmp.Save($outPath, [System.Drawing.Imaging.ImageFormat]::Png)
    $bmp.Dispose(); $src.Dispose()
}

$n = 0
foreach ($k in $powers.Keys) {
    $c = Get-Cached $powers[$k]
    Copy-Item $c "$img\powers\$k.png" -Force
    Copy-Item $c "$img\powers\big\$k.png" -Force
    $n++
}
"powers: $n"

$n = 0
foreach ($k in $relics.Keys) {
    $c = Get-Cached $relics[$k]
    Copy-Item $c "$img\relics\$k.png" -Force
    Copy-Item $c "$img\relics\big\$k.png" -Force
    Make-Outline $c "$img\relics\${k}_outline.png"
    $n++
}
"relics: $n"
"LISTO"
