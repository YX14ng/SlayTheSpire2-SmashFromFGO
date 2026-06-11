# Charui para ArtoriaCaster (v1): char select + locked (grises) + iconos de cara
# desde los face icons OFICIALES de Atlas (f_50450xx, sin calibracion de crop) +
# iconos de energia copiados de Morgan + mod_image.
Add-Type -AssemblyName System.Drawing
$ErrorActionPreference = "Stop"

$ref = "f:\Programs\SlayTheSpire2-SmashFromFGO\assets\reference"
$mod = "f:\Programs\SlayTheSpire2-SmashFromFGO\ArtoriaCaster\ArtoriaCaster"
$charui = "$mod\images\charui"
New-Item -ItemType Directory -Force $charui | Out-Null

# --- face icon oficial (asc 3 = regalia, igual que form_caster) ---
$faceUrl = "https://static.atlasacademy.io/JP/Faces/f_5045002.png"
$facePng = "$ref\face_5045002.png"
if (-not (Test-Path $facePng)) { Invoke-WebRequest $faceUrl -OutFile $facePng }

# --- char select (charagraph regalia) + locked (grises) ---
Copy-Item "$ref\chara_504500b_1.png" "$charui\char_select_char_name.png" -Force
$src = New-Object System.Drawing.Bitmap("$ref\chara_504500b_1.png")
$gray = New-Object System.Drawing.Bitmap($src.Width, $src.Height)
$g = [System.Drawing.Graphics]::FromImage($gray)
$rows = @(
    [single[]]@(0.299, 0.299, 0.299, 0, 0),
    [single[]]@(0.587, 0.587, 0.587, 0, 0),
    [single[]]@(0.114, 0.114, 0.114, 0, 0),
    [single[]]@(0, 0, 0, 1, 0),
    [single[]]@(0, 0, 0, 0, 1)
)
$matrix = New-Object System.Drawing.Imaging.ColorMatrix(,([single[][]]$rows))
$attrs = New-Object System.Drawing.Imaging.ImageAttributes
$attrs.SetColorMatrix($matrix)
$g.DrawImage($src, (New-Object System.Drawing.Rectangle(0, 0, $src.Width, $src.Height)), 0, 0, $src.Width, $src.Height, [System.Drawing.GraphicsUnit]::Pixel, $attrs)
$g.Dispose()
$gray.Save("$charui\char_select_char_name_locked.png", [System.Drawing.Imaging.ImageFormat]::Png)
$gray.Dispose(); $src.Dispose()

# --- icono de cara 128 + map marker + mod_image 256 ---
$face = New-Object System.Drawing.Bitmap($facePng)
foreach ($spec in @(@(128, "$charui\character_icon_char_name.png"), @(128, "$charui\map_marker_char_name.png"), @(256, "$mod\mod_image.png"))) {
    $size = $spec[0]; $out = $spec[1]
    $icon = New-Object System.Drawing.Bitmap($size, $size)
    $g2 = [System.Drawing.Graphics]::FromImage($icon)
    $g2.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
    $g2.DrawImage($face, (New-Object System.Drawing.Rectangle(0, 0, $size, $size)), (New-Object System.Drawing.Rectangle(0, 0, $face.Width, $face.Height)), [System.Drawing.GraphicsUnit]::Pixel)
    $g2.Dispose()
    $icon.Save($out, [System.Drawing.Imaging.ImageFormat]::Png)
    $icon.Dispose()
}
$face.Dispose()

# --- energia (mismos iconos que Morgan: azul-dorado pega con Castoria) ---
$morganUi = "f:\Programs\SlayTheSpire2-SmashFromFGO\MorganBerserker\MorganBerserker\images\charui"
Copy-Item "$morganUi\big_energy.png" "$charui\big_energy.png" -Force
Copy-Item "$morganUi\text_energy.png" "$charui\text_energy.png" -Force
"LISTO"
