# Iconos de powers/reliquias de Tiamat recortando circulos de las portadas CE ya descargadas
# (card_portraits/big/*.png). Da alpha circular -> _outline limpio. 100% local (sin API/red).
Add-Type -AssemblyName System.Drawing
$ErrorActionPreference = "Stop"
$root = "f:\Programs\SlayTheSpire2-SmashFromFGO\Tiamat\TiamatBeast\images"
$src  = "$root\card_portraits\big"
$sz = 256

# power_icon_name -> CE-art source (sin .png)
$powers = @{
  "tiamat_femme_fatale_power" = "suckle"      # Great Mother — la criadora
  "tiamat_beast_power"        = "chaos_tide"   # Beast of Billows — la Bestia
  "genesis_spent_power"       = "enuma_elis"   # Enuma Dingir — la genesis ya gastada
  "mother_guts_power"         = "carapace"     # Scale of the Stars — coraza/tenacidad
}
# relic_icon_name -> CE-art source
$relics = @{
  "sea_of_life_womb" = "black_mud"     # Slimy Black Bog — el Mar de Vida
  "mothers_tears"    = "black_deluge"  # Beyond the Flood — agua/lagrimas
  "king_hassans_horn"= "broken_horn"   # Talon Decorations of Chaos — cuerno/garras
}

function Make-CircleIcon([string]$srcPng, [string]$outPng, [int]$size) {
  $im = New-Object System.Drawing.Bitmap($srcPng)
  # recorte cuadrado centrado
  $side = [Math]::Min($im.Width, $im.Height)
  $sx = [int](($im.Width - $side) / 2); $sy = [int](($im.Height - $side) / 2)
  $out = New-Object System.Drawing.Bitmap($size, $size, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
  $g = [System.Drawing.Graphics]::FromImage($out)
  $g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
  $g.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
  $g.PixelOffsetMode = [System.Drawing.Drawing2D.PixelOffsetMode]::HighQuality
  $path = New-Object System.Drawing.Drawing2D.GraphicsPath
  $path.AddEllipse(0, 0, $size, $size)
  $g.SetClip($path)
  $dst = New-Object System.Drawing.Rectangle(0, 0, $size, $size)
  $srcRect = New-Object System.Drawing.Rectangle($sx, $sy, $side, $side)
  $g.DrawImage($im, $dst, $srcRect, [System.Drawing.GraphicsUnit]::Pixel)
  $g.Dispose()
  $out.Save($outPng, [System.Drawing.Imaging.ImageFormat]::Png)
  $out.Dispose(); $im.Dispose()
}

function Make-Outline([string]$srcPng, [string]$outPng) {
  $s = New-Object System.Drawing.Bitmap($srcPng)
  $b = New-Object System.Drawing.Bitmap($s.Width, $s.Height, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
  for ($y = 0; $y -lt $s.Height; $y++) { for ($x = 0; $x -lt $s.Width; $x++) {
    $a = $s.GetPixel($x, $y).A
    if ($a -gt 16) { $b.SetPixel($x, $y, [System.Drawing.Color]::FromArgb($a, 255, 255, 255)) }
  } }
  $b.Save($outPng, [System.Drawing.Imaging.ImageFormat]::Png); $b.Dispose(); $s.Dispose()
}

New-Item -ItemType Directory -Force "$root\powers\big" | Out-Null
New-Item -ItemType Directory -Force "$root\relics\big" | Out-Null
$n = 0
foreach ($k in $powers.Keys) {
  $s = "$src\$($powers[$k]).png"
  if (-not (Test-Path $s)) { Write-Output "FALTA fuente: $s"; continue }
  Make-CircleIcon $s "$root\powers\$k.png" $sz
  Copy-Item "$root\powers\$k.png" "$root\powers\big\$k.png" -Force
  $n++
}
Write-Output "powers: $n"
$n = 0
foreach ($k in $relics.Keys) {
  $s = "$src\$($relics[$k]).png"
  if (-not (Test-Path $s)) { Write-Output "FALTA fuente: $s"; continue }
  Make-CircleIcon $s "$root\relics\$k.png" $sz
  Copy-Item "$root\relics\$k.png" "$root\relics\big\$k.png" -Force
  Make-Outline "$root\relics\$k.png" "$root\relics\${k}_outline.png"
  $n++
}
Write-Output "relics: $n"
Write-Output "LISTO"