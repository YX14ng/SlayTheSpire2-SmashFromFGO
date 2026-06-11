# Correcciones del arte de Morgan tras la auditoría visual: charagraphs oficiales para las
# cartas-retrato y CEs re-elegidas para los matches malos. Actualiza mapping_morgan.csv.
Add-Type -AssemblyName System.Drawing
$ErrorActionPreference = "Stop"

$imgDir = "f:\Programs\SlayTheSpire2-SmashFromFGO\MorganBerserker\MorganBerserker\images\card_portraits"
$cache = "f:\Programs\SlayTheSpire2-SmashFromFGO\assets\reference\ce\art"
$tsvPath = "f:\Programs\SlayTheSpire2-SmashFromFGO\assets\reference\ce\ce_names.tsv"
$csvPath = "f:\Programs\SlayTheSpire2-SmashFromFGO\assets\reference\ce\mapping_morgan.csv"

function Crop-Card([string]$srcPath, [string]$outPath, [int]$outW, [int]$outH, [double]$yBias) {
    $src = [System.Drawing.Image]::FromFile($srcPath)
    try {
        [int]$cropW = $src.Width
        [int]$cropH = [int]([double]$src.Width * $outH / $outW)
        if ($cropH -gt $src.Height) { $cropH = $src.Height }
        [int]$cropY = [int](($src.Height - $cropH) * $yBias)
        $bmp = New-Object System.Drawing.Bitmap($outW, $outH)
        $g = [System.Drawing.Graphics]::FromImage($bmp)
        $g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
        $g.DrawImage($src, (New-Object System.Drawing.Rectangle(0, 0, $outW, $outH)), (New-Object System.Drawing.Rectangle(0, $cropY, $cropW, $cropH)), [System.Drawing.GraphicsUnit]::Pixel)
        $g.Dispose()
        $bmp.Save($outPath, [System.Drawing.Imaging.ImageFormat]::Png)
        $bmp.Dispose()
    } finally { $src.Dispose() }
}

# --- charagraphs oficiales (retratos de Morgan/Aesc; yBias bajo = cara) ---
$chara = @{
    "fairy_queen_form"             = "704000a@1"
    "queens_gaze"                  = "704000a@2"
    "roadless_camelot"             = "704000b@2"
    "roadless_camelot_unleashed"   = "704000b@1"
    "queens_mandate"               = "704030a"
    "rain_witch_form"              = "505300a@1"
    "fairy_of_the_rainland"        = "505300a@2"
    "embrace_of_the_lake"          = "505300b@1"
}
# --- CEs re-elegidas (collectionNo) ---
$ceFix = @{
    "a_home_with_morgan"      = 1996  # Tale of Morgania
    "melusines_talon"         = 911   # The Dragon and the Dragon Swordsman
    "twin_replicas"           = 1971  # Pair of White Swords
    "storms_wrath"            = 1679  # Ice Warrior
    "mirror_clans_trick"      = 717   # Uncertain Glass
    "memory_of_the_ash_tree"  = 98    # Golden Millennium Tree
    "extraordinary_tax"       = 217   # Golden Glass
    "fairy_eyes"              = 1090  # Purple Eyes
    "witchs_mark"             = 345   # Witch Under the Moonlight
    "winter_court"            = 763   # Pharaoh's Great Winter Celebration
    "from_the_worlds_end"     = 130   # World's End White
    "saviors_tears"           = 1006  # Winter's Prayer
    "perpetual_winter"        = 934   # Goddesses of the Glittering Snow
}

$tsv = @{}
foreach ($line in (Get-Content $tsvPath -Encoding UTF8)) { $p = $line -split "`t"; if ($p.Count -ge 2) { $tsv[$p[0]] = $p[1] } }

$newMap = @{}
foreach ($k in $chara.Keys) {
    $cid = $chara[$k]
    $base = $cid -replace "[ab]@?\d*$", ""
    $url = "https://static.atlasacademy.io/JP/CharaGraph/$base/$cid.png"
    $cached = "$cache\chara_$($cid -replace '@','_').png"
    if (-not (Test-Path $cached)) { Invoke-WebRequest $url -OutFile $cached -UseBasicParsing; Start-Sleep -Milliseconds 600 }
    Crop-Card $cached "$imgDir\$k.png" 500 380 0.12
    Crop-Card $cached "$imgDir\big\$k.png" 1000 760 0.12
    $newMap[$k] = "CHARA:$cid"
    "OK chara $k <- $cid"
}
foreach ($k in $ceFix.Keys) {
    $aid = $tsv[[string]$ceFix[$k]]
    if (-not $aid) { "SIN assetId: $k ($($ceFix[$k]))"; continue }
    $url = "https://static.atlasacademy.io/JP/CharaGraph/$aid/${aid}a.png"
    $cached = "$cache\$aid.png"
    if (-not (Test-Path $cached)) { Invoke-WebRequest $url -OutFile $cached -UseBasicParsing; Start-Sleep -Milliseconds 600 }
    Crop-Card $cached "$imgDir\$k.png" 500 380 0.28
    Crop-Card $cached "$imgDir\big\$k.png" 1000 760 0.28
    $newMap[$k] = $aid
    "OK ce $k <- $($ceFix[$k]) ($aid)"
}

# actualizar el CSV
$rows = Import-Csv $csvPath
foreach ($r in $rows) { if ($newMap.ContainsKey($r.file)) { $r.assetId = $newMap[$r.file] } }
$out = @("file,assetId") + ($rows | ForEach-Object { "$($_.file),$($_.assetId)" })
[IO.File]::WriteAllText($csvPath, ($out -join "`n") + "`n", (New-Object System.Text.UTF8Encoding($false)))
"CSV actualizado. LISTO"
