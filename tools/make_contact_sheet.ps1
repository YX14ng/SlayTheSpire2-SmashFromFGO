# Hoja de contactos: grilla etiquetada de todos los PNG de una carpeta, para auditar arte en lote.
param(
    [Parameter(Mandatory)][string]$Dir,
    [string]$OutPrefix = "$env:TEMP\contact_sheet",
    [int]$Cols = 6,
    [int]$PerSheet = 42
)
Add-Type -AssemblyName System.Drawing

$thumbW = 150; $thumbH = 114; $labelH = 16
$cellW = $thumbW + 6; $cellH = $thumbH + $labelH + 8
$files = Get-ChildItem $Dir -Filter *.png | Sort-Object Name
$font = New-Object System.Drawing.Font("Consolas", 7)
$brush = [System.Drawing.Brushes]::White
$sheet = 0
for ($start = 0; $start -lt $files.Count; $start += $PerSheet) {
    $batch = $files[$start..([Math]::Min($start + $PerSheet - 1, $files.Count - 1))]
    $rowCount = [Math]::Ceiling($batch.Count / $Cols)
    $bmp = New-Object System.Drawing.Bitmap(($Cols * $cellW), ($rowCount * $cellH))
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.Clear([System.Drawing.Color]::FromArgb(255, 20, 20, 30))
    $g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    for ($i = 0; $i -lt $batch.Count; $i++) {
        $x = ($i % $Cols) * $cellW + 3; $y = [Math]::Floor($i / $Cols) * $cellH + 3
        $img = [System.Drawing.Image]::FromFile($batch[$i].FullName)
        $g.DrawImage($img, $x, $y, $thumbW, $thumbH)
        $img.Dispose()
        $g.DrawString($batch[$i].BaseName, $font, $brush, $x, ($y + $thumbH + 1))
    }
    $g.Dispose()
    $out = "${OutPrefix}_$sheet.png"
    $bmp.Save($out, [System.Drawing.Imaging.ImageFormat]::Png)
    $bmp.Dispose()
    $out
    $sheet++
}
