# Compone las command cards oficiales de FGO (capas del visor de Atlas Academy:
# card_bg + retrato card_servant + card_icon + card_txt) y las recorta a los
# formatos de carta de StS2 (carta completa centrada con relleno difuminado).
param(
    [Parameter(Mandatory)][string]$SvtId,
    [Parameter(Mandatory)][string]$OutDir,
    [Parameter(Mandatory)][string]$Suffix,
    [double]$IconScale = 1.1,
    [double]$TxtScale = 0.78
)
Add-Type -AssemblyName System.Drawing
$cache = "f:\Programs\SlayTheSpire2-SmashFromFGO\assets\reference\ce\art"
$cmd = "$cache\cmdcard"
$portrait = "$cache\command_$SvtId.png"
if (-not (Test-Path $portrait)) {
    Invoke-WebRequest "https://static.atlasacademy.io/JP/Servants/Commands/$SvtId/card_servant_1.png" -OutFile $portrait
}

function Compose-Card([string]$type, [string]$outPath) {
    $bg = New-Object System.Drawing.Bitmap("$cmd\card_bg_$type.png")
    $por = New-Object System.Drawing.Bitmap($portrait)
    $icon = New-Object System.Drawing.Bitmap("$cmd\card_icon_$type.png")
    $txt = New-Object System.Drawing.Bitmap("$cmd\card_txt_$type.png")
    $H = 875; $W = [int](133 / 170 * $H)
    $cv = New-Object System.Drawing.Bitmap($W, $H)
    $g = [System.Drawing.Graphics]::FromImage($cv)
    $g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    $g.DrawImage($bg, 0, 0, $W, $H)
    $pw = [int](512 / 875 * $H * 0.96); $ph = [int]($H * 0.96)
    $g.DrawImage($por, [int](($W - $pw) / 2), [int](($H - $ph) / 2), $pw, $ph)
    $iw = [int]($W * $IconScale); $ih = [int](133 / 185 * $iw)
    $g.DrawImage($icon, [int](($W - $iw) / 2), $H - $ih, $iw, $ih)
    $tw = [int]($W * $TxtScale); $th = [int](73 / 150 * $tw)
    $g.DrawImage($txt, [int](($W - $tw) / 2), [int]($H - $th * 1.15), $tw, $th)
    $g.Dispose()
    $cv.Save($outPath, [System.Drawing.Imaging.ImageFormat]::Png)
    $cv.Dispose(); $bg.Dispose(); $por.Dispose(); $icon.Dispose(); $txt.Dispose()
}

function LetterBox([string]$srcPath, [string]$outPath, [int]$outW, [int]$outH) {
    # carta completa centrada; los lados se rellenan con la misma carta ampliada y oscurecida
    $src = New-Object System.Drawing.Bitmap($srcPath)
    $bmp = New-Object System.Drawing.Bitmap($outW, $outH)
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    # fondo: zoom central oscurecido
    $zw = $outW; $zh = [int]($src.Height * ($outW / $src.Width))
    $g.DrawImage($src, 0, [int](($outH - $zh) / 2), $zw, $zh)
    $dark = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(150, 0, 0, 0))
    $g.FillRectangle($dark, 0, 0, $outW, $outH)
    # carta completa al frente
    $ch = $outH; $cw = [int]($src.Width * ($outH / $src.Height))
    $g.DrawImage($src, [int](($outW - $cw) / 2), 0, $cw, $ch)
    $g.Dispose()
    $bmp.Save($outPath, [System.Drawing.Imaging.ImageFormat]::Png)
    $bmp.Dispose(); $src.Dispose()
}

New-Item -ItemType Directory -Force "$OutDir\big" | Out-Null
foreach ($t in @("buster", "arts", "quick")) {
    $full = "$env:TEMP\cmdcard_full_${t}_$SvtId.png"
    Compose-Card $t $full
    LetterBox $full "$OutDir\${t}_$Suffix.png" 500 380
    LetterBox $full "$OutDir\big\${t}_$Suffix.png" 1000 760
    "${t}_$Suffix ok"
}
