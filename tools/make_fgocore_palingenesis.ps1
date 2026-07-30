param([string]$Root = 'F:\Programs\SlayTheSpire2-SmashFromFGO')

Add-Type -AssemblyName System.Drawing
$ErrorActionPreference = 'Stop'
$sourcePath = Join-Path $Root 'assets\reference\ce\icons\7999.png'
$outputRoot = Join-Path $Root 'FGOCore\FGOCore\images\card_portraits'

function Render-Palingenesis([string]$outputPath, [int]$width, [int]$height) {
    $source = [System.Drawing.Bitmap]::FromFile($sourcePath)
    try {
        $canvas = New-Object System.Drawing.Bitmap($width, $height, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
        $graphics = [System.Drawing.Graphics]::FromImage($canvas)
        try {
            $rect = [System.Drawing.Rectangle]::new(0, 0, $width, $height)
            $background = New-Object System.Drawing.Drawing2D.LinearGradientBrush(
                $rect,
                [System.Drawing.Color]::FromArgb(255, 19, 54, 67),
                [System.Drawing.Color]::FromArgb(255, 68, 24, 42),
                0.0)
            $graphics.FillRectangle($background, $rect)
            $background.Dispose()

            $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
            $glowSize = [int]($height * 0.82)
            $glowX = [int](($width - $glowSize) / 2)
            $glowY = [int](($height - $glowSize) / 2)
            for ($i = 5; $i -ge 1; $i--) {
                $inset = [int](($glowSize * (5 - $i)) / 18)
                $alpha = 10 + ($i * 5)
                $glow = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb($alpha, 255, 204, 92))
                $graphics.FillEllipse($glow, $glowX + $inset, $glowY + $inset, $glowSize - 2 * $inset, $glowSize - 2 * $inset)
                $glow.Dispose()
            }

            $targetH = [int]($height * 0.78)
            $targetW = [int]($source.Width * ($targetH / [double]$source.Height))
            $target = [System.Drawing.Rectangle]::new([int](($width - $targetW) / 2), [int](($height - $targetH) / 2), $targetW, $targetH)
            $graphics.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
            $graphics.DrawImage($source, $target)
        } finally { $graphics.Dispose() }
        New-Item -ItemType Directory -Force (Split-Path $outputPath) | Out-Null
        $canvas.Save($outputPath, [System.Drawing.Imaging.ImageFormat]::Png)
        $canvas.Dispose()
    } finally { $source.Dispose() }
}

Render-Palingenesis (Join-Path $outputRoot 'palingenesis.png') 500 380
Render-Palingenesis (Join-Path $outputRoot 'big\palingenesis.png') 1000 760
Write-Output 'Generated Palingenesis from the official FGO Holy Grail item icon (7999).'
