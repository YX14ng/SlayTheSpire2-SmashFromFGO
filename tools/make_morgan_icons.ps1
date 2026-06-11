# Iconos de powers (skills FGO) y reliquias (items FGO) para MorganBerserker,
# + los iconos de los powers compartidos nuevos de FGOCore (curse/guts/blessing).
Add-Type -AssemblyName System.Drawing
$ErrorActionPreference = "Continue"

$img = "f:\Programs\SlayTheSpire2-SmashFromFGO\MorganBerserker\MorganBerserker\images"
$coreImg = "f:\Programs\SlayTheSpire2-SmashFromFGO\FGOCore\FGOCore\images"
$cache = "f:\Programs\SlayTheSpire2-SmashFromFGO\assets\reference\ce\icons"
New-Item -ItemType Directory -Force $cache | Out-Null
$skill = "https://static.atlasacademy.io/JP/SkillIcons"
$items = "https://static.atlasacademy.io/JP/Items"

$powers = @{
    "fairy_queen_form_power"        = "$skill/skill_00300.png"
    "rain_witch_form_power"         = "$skill/skill_00607.png"
    "winter_queen_form_power"       = "$skill/skill_00404.png"
    "np_manifested_power"           = "$skill/skill_00508.png"
    "madness_enhancement_power"     = "$skill/skill_00306.png"
    "fairy_eyes_power"              = "$skill/skill_00603.png"
    "fairy_of_the_rainland_power"   = "$skill/skill_00601.png"
    "territory_creation_power"      = "$skill/skill_00419.png"
    "item_construction_power"       = "$skill/skill_00506.png"
    "winter_court_power"            = "$skill/skill_00605.png"
    "charisma_of_adversity_power"   = "$skill/skill_00303.png"
    "curse_of_cernunnos_power"      = "$skill/skill_00403.png"
    "sovereign_of_two_faces_power"  = "$skill/skill_00311.png"
    "spriggan_vigil_power"          = "$skill/skill_00400.png"
    "perpetual_winter_power"        = "$skill/skill_00602.png"
    "fae_blood_pact_power"          = "$skill/skill_00305.png"
    "worlds_end_guts_power"         = "$skill/skill_00404.png"
}
$corePowers = @{
    "curse_power"               = "https://static.atlasacademy.io/JP/BuffIcons/bufficon_521.png" # icono de ESTADO real de FGO (feedback: skill_00403 es otra cosa)
    "guts_power"                = "$skill/skill_00305.png"
    "overcharge_blessing_power" = "$skill/skill_00508.png"
}
$relics = @{
    "worlds_end_coronation"     = "$items/6524.png"
    "existence_tax"             = "$items/6531.png"
    "spriggan_treasury"         = "$items/6515.png"
    "mirror_clan_glass"         = "$items/6999.png"
    "bottled_mors"              = "$items/6517.png"
    "habetrot_thread"           = "$items/6501.png"
    "lady_of_the_lake_chalice"  = "$items/7998.png"
    "morgan_summon_seal"        = "$items/4001.png"
    "morgan_bond"               = "$items/94006404.png"
    # Regla del workflow: starter de mecánica = icono de clase por rareza (Morgan 5★ = oro)
    "queens_scepter"            = "https://static.wikia.nocookie.net/fategrandorder/images/9/9d/Berserkergold.png?format=original"
}

function Get-Cached([string]$url) {
    $name = (($url -split '/')[-1] -split '\?')[0]
    $path = "$cache\$name"
    if (-not (Test-Path $path)) { Invoke-WebRequest $url -OutFile $path -UseBasicParsing -UserAgent "MorganMod/1.0" }
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
foreach ($k in $corePowers.Keys) {
    try { $c = Get-Cached $corePowers[$k]; Copy-Item $c "$coreImg\powers\$k.png" -Force; Copy-Item $c "$coreImg\powers\big\$k.png" -Force } catch { $fail += "core $k" }
}
foreach ($k in $relics.Keys) {
    try {
        $c = Get-Cached $relics[$k]
        Copy-Item $c "$img\relics\$k.png" -Force; Copy-Item $c "$img\relics\big\$k.png" -Force
        Make-Outline $c "$img\relics\${k}_outline.png"
    } catch { $fail += "relic $k : $($_.Exception.Message)" }
}
"powers: $($powers.Count + $corePowers.Count), relics: $($relics.Count)"
if ($fail.Count -gt 0) { "FALLOS:"; $fail } else { "SIN FALLOS" }