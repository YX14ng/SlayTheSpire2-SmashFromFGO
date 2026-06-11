# Iconos de powers (skills FGO + faces oficiales para las formas) y reliquias
# (items FGO) para ArtoriaCaster. Los que fallen o queden mal se auditan con
# make_contact_sheet.ps1 y se corrigen a mano (flujo Morgan).
Add-Type -AssemblyName System.Drawing
$ErrorActionPreference = "Continue"

$img = "f:\Programs\SlayTheSpire2-SmashFromFGO\ArtoriaCaster\ArtoriaCaster\images"
$cache = "f:\Programs\SlayTheSpire2-SmashFromFGO\assets\reference\ce\icons"
New-Item -ItemType Directory -Force $cache | Out-Null
$skill = "https://static.atlasacademy.io/JP/SkillIcons"
$items = "https://static.atlasacademy.io/JP/Items"
$faces = "https://static.atlasacademy.io/JP/Faces"

$powers = @{
    "critical_stars_power"        = "$skill/skill_00301.png"
    "anti_purge_power"            = "$skill/skill_00305.png"
    "prophecy_caster_form_power"  = "$faces/f_5045002.png"
    "summer_berserker_form_power" = "$faces/f_7047000.png"
    "avalon_form_power"           = "$faces/f_7047002.png"
    "np_manifested_power"         = "$skill/skill_00508.png"
    "sacred_sword_control_power"  = "$skill/skill_00305.png"
    "territory_creation_power"    = "$skill/skill_00419.png"
    "unique_magecraft_power"      = "$skill/skill_00602.png"
    "midsummer_madness_power"     = "$skill/skill_00306.png"
    "faerie_eyes_power"           = "$skill/skill_00603.png"
    "festival_spirit_power"       = "$skill/skill_00311.png"
    "summer_rivalry_power"        = "$skill/skill_00300.png"
    "avalon_benediction_power"    = "$skill/skill_00601.png"
    "two_faces_of_summer_power"   = "$skill/skill_00311.png"
    "sword_instinct_power"        = "$skill/skill_00400.png"
    "guardians_counter_power"     = "$skill/skill_00404.png"
    "spell_reloading_power"       = "$skill/skill_00506.png"
    "next_attack_boost_power"     = "$skill/skill_00300.png"
    "escapade_return_power"       = "$skill/skill_00607.png"
}
$relics = @{
    "artoria_bond"            = "$items/94006404.png"
    "prophecy_child_talisman" = "$items/4001.png"
    "forged_sacred_sword"     = "$items/6502.png"
    "white_shark_float"       = "$faces/f_7047001.png"
    "familiar_owl"            = "$items/6510.png"
    "rabbit_ears_diadem"      = "$faces/f_7047001.png"
    "magic_resistance_amulet" = "$items/6516.png"
    "anti_purge_crystal"      = "$items/6515.png"
    "servant_fes_program"     = "$items/6517.png"
    "detective_magnifier"     = "$items/6531.png"
    "inner_sea_chalice"       = "$items/7998.png"
    # Regla del workflow: starter de mecánica = icono de clase por rareza (Castoria 5★ = oro)
    "selection_staff"         = "https://media.fgo.wiki/5/5e/%E9%87%91%E5%8D%A1Caster.png"
}

function Get-Cached([string]$url) {
    $name = (($url -split '/')[-1] -split '\?')[0]
    $path = "$cache\$name"
    if (-not (Test-Path $path)) { Invoke-WebRequest $url -OutFile $path -UseBasicParsing -UserAgent "ArtoriaMod/1.0" }
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

$fail = @()
foreach ($k in $powers.Keys) {
    try { $c = Get-Cached $powers[$k]; Copy-Item $c "$img\powers\$k.png" -Force; Copy-Item $c "$img\powers\big\$k.png" -Force } catch { $fail += "power $k" }
}
foreach ($k in $relics.Keys) {
    try {
        $c = Get-Cached $relics[$k]
        Copy-Item $c "$img\relics\$k.png" -Force; Copy-Item $c "$img\relics\big\$k.png" -Force
        Make-Outline $c "$img\relics\${k}_outline.png"
    } catch { $fail += "relic $k : $($_.Exception.Message)" }
}
"powers: $($powers.Count), relics: $($relics.Count)"
if ($fail.Count -gt 0) { "FALLOS:"; $fail } else { "SIN FALLOS" }
