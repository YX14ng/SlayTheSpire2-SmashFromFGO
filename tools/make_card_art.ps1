# Descarga arte de Craft Essences (Atlas Academy) y lo recorta a los formatos de carta de StS2.
# Entrada: CSV con columnas file,assetId (assetId = id interno del CE, ej. 9400970)
# Salida: MashShielder/MashShielder/images/card_portraits/<file>.png (500x380)
#         MashShielder/MashShielder/images/card_portraits/big/<file>.png (1000x760)
param(
    [Parameter(Mandatory)][string]$MappingCsv,
    [string]$OutDir = "f:\Programs\SlayTheSpire2-SmashFromFGO\MashShielder\MashShielder\images\card_portraits"
)
Add-Type -AssemblyName System.Drawing

$imgDir = $OutDir
New-Item -ItemType Directory -Force "$imgDir\big" | Out-Null
$cache = "f:\Programs\SlayTheSpire2-SmashFromFGO\assets\reference\ce\art"
New-Item -ItemType Directory -Force $cache | Out-Null

function Crop-Card(
    [string]$srcPath,
    [string]$outPath,
    [int]$outW,
    [int]$outH,
    [double]$focusX = 0.5,
    [double]$focusY = 0.28
) {
    $src = [System.Drawing.Image]::FromFile($srcPath)
    try {
        # CE art es 512x875 vertical. Tomamos una franja apaisada con ratio outW:outH
        # centrada en el tercio superior (donde suele estar el sujeto).
        [int]$cropW = $src.Width
        [int]$cropH = [int]([double]$src.Width * $outH / $outW)
        if ($cropH -gt $src.Height) {
            $cropH = $src.Height
            $cropW = [int]([double]$src.Height * $outW / $outH)
        }
        $focusX = [Math]::Max(0.0, [Math]::Min(1.0, $focusX))
        $focusY = [Math]::Max(0.0, [Math]::Min(1.0, $focusY))
        [int]$cropX = [int](($src.Width - $cropW) * $focusX)
        [int]$cropY = [int](($src.Height - $cropH) * $focusY)
        $bmp = New-Object System.Drawing.Bitmap($outW, $outH)
        $g = [System.Drawing.Graphics]::FromImage($bmp)
        $g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
        $dstRect = New-Object System.Drawing.Rectangle(0, 0, $outW, $outH)
        $srcRect = New-Object System.Drawing.Rectangle($cropX, $cropY, $cropW, $cropH)
        $g.DrawImage($src, $dstRect, $srcRect, [System.Drawing.GraphicsUnit]::Pixel)
        $g.Dispose()
        $bmp.Save($outPath, [System.Drawing.Imaging.ImageFormat]::Png)
        $bmp.Dispose()
    } finally { $src.Dispose() }
}

$rows = Import-Csv $MappingCsv
$ok = 0; $fail = @()
foreach ($r in $rows) {
    if ($r.assetId -like "CHARA:*") {
        # charagraph de servant: CHARA:504500a@1 -> CharaGraph/504500/504500a@1.png
        $cid = $r.assetId.Substring(6)
        $svt = $cid.Substring(0, 6)
        $url = "https://static.atlasacademy.io/JP/CharaGraph/$svt/$cid.png"
        $cached = "$cache\chara_$($cid -replace '@','_').png"
    } else {
        $url = "https://static.atlasacademy.io/JP/CharaGraph/$($r.assetId)/$($r.assetId)a.png"
        $cached = "$cache\$($r.assetId).png"
    }
    try {
        if (-not (Test-Path $cached)) { Invoke-WebRequest $url -OutFile $cached }
        $focusX = if ($null -ne $r.cropX -and $r.cropX -ne '') {
            [double]::Parse($r.cropX, [Globalization.CultureInfo]::InvariantCulture)
        } else { 0.5 }
        $focusY = if ($null -ne $r.cropY -and $r.cropY -ne '') {
            [double]::Parse($r.cropY, [Globalization.CultureInfo]::InvariantCulture)
        } else { 0.28 }
        Crop-Card $cached "$imgDir\$($r.file).png" 500 380 $focusX $focusY
        Crop-Card $cached "$imgDir\big\$($r.file).png" 1000 760 $focusX $focusY
        $ok++
    } catch {
        $fail += "$($r.file) ($($r.assetId)): $($_.Exception.Message)"
    }
}
"OK: $ok de $($rows.Count)"
if ($fail.Count -gt 0) { "FALLOS:"; $fail }
