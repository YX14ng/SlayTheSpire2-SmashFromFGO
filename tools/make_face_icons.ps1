# Recorta la cara del charagraph para el icono del panel superior y el marcador de mapa.
# Uso: .\make_face_icons.ps1 [-Source <png>] [-FaceX 210] [-FaceY 95] [-Side 180]
#   FaceX/FaceY = centro de la cara en px del charagraph; Side = lado del cuadrado de recorte.
param(
    [string]$Source = "f:\Programs\SlayTheSpire2-SmashFromFGO\assets\reference\charagraph\mash_base_final.png",
    [string]$OutDir = "f:\Programs\SlayTheSpire2-SmashFromFGO\MashShielder\MashShielder\images\charui",
    [int]$FaceX = 210,
    [int]$FaceY = 95,
    [int]$Side = 180,
    [int]$OutSize = 128
)
Add-Type -AssemblyName System.Drawing

$src = New-Object System.Drawing.Bitmap($Source)
$x = [Math]::Max(0, [Math]::Min($src.Width - $Side, $FaceX - [int]($Side / 2)))
$y = [Math]::Max(0, [Math]::Min($src.Height - $Side, $FaceY - [int]($Side / 2)))
$rect = New-Object System.Drawing.Rectangle($x, $y, $Side, $Side)
"crop: x=$x y=$y side=$Side (centro $FaceX,$FaceY) de $($src.Width)x$($src.Height)"

$out = New-Object System.Drawing.Bitmap($OutSize, $OutSize)
$g = [System.Drawing.Graphics]::FromImage($out)
$g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
$g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::HighQuality
$g.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
$g.DrawImage($src, (New-Object System.Drawing.Rectangle(0, 0, $OutSize, $OutSize)), $rect, [System.Drawing.GraphicsUnit]::Pixel)
$g.Dispose()

foreach ($name in @("character_icon_char_name.png", "map_marker_char_name.png")) {
    $out.Save("$OutDir\$name", [System.Drawing.Imaging.ImageFormat]::Png)
    "OK $name"
}
$out.Dispose(); $src.Dispose()
"LISTO"
