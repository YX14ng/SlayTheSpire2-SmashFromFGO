# Charui + sprites de forma estáticos para MorganBerserker (v1 sin animaciones).
param(
    [int]$FaceX = 256, [int]$FaceY = 110, [int]$Side = 170
)
Add-Type -AssemblyName System.Drawing
$ErrorActionPreference = "Stop"

$cache = "f:\Programs\SlayTheSpire2-SmashFromFGO\assets\reference\ce\art"
$mod = "f:\Programs\SlayTheSpire2-SmashFromFGO\MorganBerserker\MorganBerserker"
$charui = "$mod\images\charui"

# --- sprites de combate por forma (charagraph completo, fondo transparente) ---
Copy-Item "$cache\chara_704000b_2.png" "$mod\character\form_queen.png" -Force
Copy-Item "$cache\chara_505300a_1.png" "$mod\character\form_aesc.png" -Force
Copy-Item "$cache\chara_704030a.png" "$mod\character\form_winter.png" -Force

# --- char select (charagraph final) + locked (grises) ---
Copy-Item "$cache\chara_704000b_2.png" "$charui\char_select_char_name.png" -Force
$src = New-Object System.Drawing.Bitmap("$cache\chara_704000b_2.png")
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
$gray.Dispose()

# --- icono de cara (de la 1ra ascensión) + map marker ---
$face = New-Object System.Drawing.Bitmap("$cache\chara_704000a_1.png")
$x = [Math]::Max(0, [Math]::Min($face.Width - $Side, $FaceX - [int]($Side / 2)))
$y = [Math]::Max(0, [Math]::Min($face.Height - $Side, $FaceY - [int]($Side / 2)))
"crop cara: x=$x y=$y side=$Side de $($face.Width)x$($face.Height)"
$icon = New-Object System.Drawing.Bitmap(128, 128)
$g2 = [System.Drawing.Graphics]::FromImage($icon)
$g2.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
$g2.DrawImage($face, (New-Object System.Drawing.Rectangle(0, 0, 128, 128)), (New-Object System.Drawing.Rectangle($x, $y, $Side, $Side)), [System.Drawing.GraphicsUnit]::Pixel)
$g2.Dispose()
foreach ($n in @("character_icon_char_name.png", "map_marker_char_name.png")) {
    $icon.Save("$charui\$n", [System.Drawing.Imaging.ImageFormat]::Png)
}
$icon.Dispose(); $face.Dispose(); $src.Dispose()
"LISTO"
