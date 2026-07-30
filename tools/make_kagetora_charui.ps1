param([string]$Root = 'F:\Programs\SlayTheSpire2-SmashFromFGO')

Add-Type -AssemblyName System.Drawing
$ErrorActionPreference = 'Stop'

$cache = Join-Path $Root 'assets\reference\ce\art'
$mod = Join-Path $Root 'KagetoraLancer\KagetoraLancer'
$charui = Join-Path $mod 'images\charui'
$character = Join-Path $mod 'character'
$selectSource = Join-Path $cache 'chara_303800b_2.png'
$faceSource = Join-Path $cache 'command_303800.png'

if (-not (Test-Path $selectSource)) {
    Invoke-WebRequest 'https://static.atlasacademy.io/JP/CharaGraph/303800/303800b@2.png' -OutFile $selectSource
}
if (-not (Test-Path $faceSource)) {
    Invoke-WebRequest 'https://static.atlasacademy.io/JP/Servants/Commands/303800/card_servant_1.png' -OutFile $faceSource
}

New-Item -ItemType Directory -Force $charui | Out-Null
Copy-Item $selectSource (Join-Path $charui 'char_select_char_name.png') -Force
Copy-Item $selectSource (Join-Path $character 'select_bg_kagetora.png') -Force

$source = New-Object System.Drawing.Bitmap($selectSource)
$gray = New-Object System.Drawing.Bitmap($source.Width, $source.Height)
$graphics = [System.Drawing.Graphics]::FromImage($gray)
$rows = @(
    [single[]]@(0.299, 0.299, 0.299, 0, 0),
    [single[]]@(0.587, 0.587, 0.587, 0, 0),
    [single[]]@(0.114, 0.114, 0.114, 0, 0),
    [single[]]@(0, 0, 0, 1, 0),
    [single[]]@(0, 0, 0, 0, 1)
)
$matrix = New-Object System.Drawing.Imaging.ColorMatrix(,([single[][]]$rows))
$attributes = New-Object System.Drawing.Imaging.ImageAttributes
$attributes.SetColorMatrix($matrix)
$graphics.DrawImage($source,
    (New-Object System.Drawing.Rectangle(0, 0, $source.Width, $source.Height)),
    0, 0, $source.Width, $source.Height,
    [System.Drawing.GraphicsUnit]::Pixel, $attributes)
$graphics.Dispose(); $attributes.Dispose(); $source.Dispose()
$gray.Save((Join-Path $charui 'char_select_char_name_locked.png'), [System.Drawing.Imaging.ImageFormat]::Png)
$gray.Dispose()

$face = New-Object System.Drawing.Bitmap($faceSource)
foreach ($spec in @(
    @(128, (Join-Path $charui 'character_icon_char_name.png')),
    @(128, (Join-Path $charui 'map_marker_char_name.png')),
    @(256, (Join-Path $mod 'mod_image.png'))
)) {
    $size = [int]$spec[0]
    $output = New-Object System.Drawing.Bitmap($size, $size, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
    $g = [System.Drawing.Graphics]::FromImage($output)
    $g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    $g.DrawImage($face,
        (New-Object System.Drawing.Rectangle(0, 0, $size, $size)),
        (New-Object System.Drawing.Rectangle(0, 0, $face.Width, $face.Height)),
        [System.Drawing.GraphicsUnit]::Pixel)
    $g.Dispose()
    $output.Save([string]$spec[1], [System.Drawing.Imaging.ImageFormat]::Png)
    $output.Dispose()
}
$face.Dispose()

Write-Output 'Kagetora charui: listo'
