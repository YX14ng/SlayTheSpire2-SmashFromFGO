# Descarga arte de Craft Essences (Atlas Academy) y lo recorta a los formatos de carta de StS2.
# Entrada: CSV con columnas file,assetId (assetId = id interno del CE, ej. 9400970)
# Salida: MashShielder/MashShielder/images/card_portraits/<file>.png (500x380)
#         MashShielder/MashShielder/images/card_portraits/big/<file>.png (1000x760)
param(
    [Parameter(Mandatory)][string]$MappingCsv
)
Add-Type -AssemblyName System.Drawing

$imgDir = "f:\Programs\SlayTheSpire2-SmashFromFGO\MashShielder\MashShielder\images\card_portraits"
$cache = "f:\Programs\SlayTheSpire2-SmashFromFGO\assets\reference\ce\art"
New-Item -ItemType Directory -Force $cache | Out-Null

function Crop-Card([string]$srcPath, [string]$outPath, [int]$outW, [int]$outH) {
    $src = [System.Drawing.Image]::FromFile($srcPath)
    try {
        # CE art es 512x875 vertical. Tomamos una franja apaisada con ratio outW:outH
        # centrada en el tercio superior (donde suele estar el sujeto).
        [int]$cropW = $src.Width
        [int]$cropH = [int]([double]$src.Width * $outH / $outW)
        if ($cropH -gt $src.Height) { $cropH = $src.Height }
        [int]$cropY = [int](($src.Height - $cropH) * 0.28)
        $bmp = New-Object System.Drawing.Bitmap($outW, $outH)
        $g = [System.Drawing.Graphics]::FromImage($bmp)
        $g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
        $dstRect = New-Object System.Drawing.Rectangle(0, 0, $outW, $outH)
        $srcRect = New-Object System.Drawing.Rectangle(0, $cropY, $cropW, $cropH)
        $g.DrawImage($src, $dstRect, $srcRect, [System.Drawing.GraphicsUnit]::Pixel)
        $g.Dispose()
        $bmp.Save($outPath, [System.Drawing.Imaging.ImageFormat]::Png)
        $bmp.Dispose()
    } finally { $src.Dispose() }
}

$rows = Import-Csv $MappingCsv
$ok = 0; $fail = @()
foreach ($r in $rows) {
    $url = "https://static.atlasacademy.io/JP/CharaGraph/$($r.assetId)/$($r.assetId)a.png"
    $cached = "$cache\$($r.assetId).png"
    try {
        if (-not (Test-Path $cached)) { Invoke-WebRequest $url -OutFile $cached }
        Crop-Card $cached "$imgDir\$($r.file).png" 500 380
        Crop-Card $cached "$imgDir\big\$($r.file).png" 1000 760
        $ok++
    } catch {
        $fail += "$($r.file) ($($r.assetId)): $($_.Exception.Message)"
    }
}
"OK: $ok de $($rows.Count)"
if ($fail.Count -gt 0) { "FALLOS:"; $fail }
