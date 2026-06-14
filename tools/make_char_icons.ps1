# Iconos de powers/relics de un personaje, recortando circulos de sus propias portadas CE.
# Lee los ENTRY de powers.json/relics.json (eng) -> nombres de icono; cicla por las portadas big/.
# 100% local. Uso: .\tools\make_char_icons.ps1 -ProjId MordredSaber
param([Parameter(Mandatory)][string]$ProjId, [string]$Root='f:\Programs\SlayTheSpire2-SmashFromFGO')
Add-Type -AssemblyName System.Drawing
$ErrorActionPreference='Stop'
$img = "$Root\$ProjId\$ProjId\images"
if (-not (Test-Path $img)) { $img = "$Root\Tiamat\TiamatBeast\images" } # (no aplica; salvaguarda)
$prefix = "$($ProjId.ToUpper())-"
$loc = "$Root\$ProjId\$ProjId\localization\eng"
$sz = 256

# pool de fuentes = portadas big (excluye card.png)
$pool = @(Get-ChildItem "$img\card_portraits\big\*.png" -ErrorAction SilentlyContinue | Where-Object { $_.Name -ne 'card.png' })
if ($pool.Count -eq 0) { Write-Output "${ProjId}: sin portadas de origen"; exit }

function Entries($file) {
  if (-not (Test-Path $file)) { return @() }
  $t = [System.IO.File]::ReadAllText($file, [System.Text.Encoding]::UTF8)
  [regex]::Matches($t, [regex]::Escape($prefix)+'([A-Z0-9_]+)\.title') | ForEach-Object { $_.Groups[1].Value.ToLowerInvariant() } | Select-Object -Unique
}
function PickSrc($name) { $h=0; foreach($c in $name.ToCharArray()){ $h=($h*31 + [int]$c) % 2147483647 }; $pool[$h % $pool.Count].FullName }

function CircleIcon($srcPng, $out, $size) {
  $im = New-Object System.Drawing.Bitmap($srcPng)
  $side = [Math]::Min($im.Width, $im.Height); $sx=[int](($im.Width-$side)/2); $sy=[int](($im.Height-$side)/2)
  $o = New-Object System.Drawing.Bitmap($size,$size,[System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
  $g = [System.Drawing.Graphics]::FromImage($o)
  $g.InterpolationMode=[System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic; $g.SmoothingMode=[System.Drawing.Drawing2D.SmoothingMode]::AntiAlias; $g.PixelOffsetMode='HighQuality'
  $path = New-Object System.Drawing.Drawing2D.GraphicsPath; $path.AddEllipse(0,0,$size,$size); $g.SetClip($path)
  $g.DrawImage($im,(New-Object System.Drawing.Rectangle(0,0,$size,$size)),(New-Object System.Drawing.Rectangle($sx,$sy,$side,$side)),[System.Drawing.GraphicsUnit]::Pixel)
  $g.Dispose(); $o.Save($out,[System.Drawing.Imaging.ImageFormat]::Png); $o.Dispose(); $im.Dispose()
}
function Outline($srcPng,$out){ $s=New-Object System.Drawing.Bitmap($srcPng); $b=New-Object System.Drawing.Bitmap($s.Width,$s.Height,[System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
  for($y=0;$y -lt $s.Height;$y++){for($x=0;$x -lt $s.Width;$x++){ $a=$s.GetPixel($x,$y).A; if($a -gt 16){$b.SetPixel($x,$y,[System.Drawing.Color]::FromArgb($a,255,255,255))} }}
  $b.Save($out,[System.Drawing.Imaging.ImageFormat]::Png); $b.Dispose(); $s.Dispose() }

New-Item -ItemType Directory -Force "$img\powers\big","$img\relics\big" | Out-Null
$np=0
foreach ($name in (Entries "$loc\powers.json")) {
  if (Test-Path "$img\powers\$name.png") { } # sobrescribe igual
  CircleIcon (PickSrc $name) "$img\powers\$name.png" $sz
  Copy-Item "$img\powers\$name.png" "$img\powers\big\$name.png" -Force; $np++
}
$nr=0
foreach ($name in (Entries "$loc\relics.json")) {
  CircleIcon (PickSrc $name) "$img\relics\$name.png" $sz
  Copy-Item "$img\relics\$name.png" "$img\relics\big\$name.png" -Force
  Outline "$img\relics\$name.png" "$img\relics\${name}_outline.png"; $nr++
}
Write-Output "$ProjId : powers=$np relics=$nr (desde $($pool.Count) portadas)"