# Parchea los .webp.import generados por el publish para que el .pck no infle:
# compresion VRAM lossy (mode=1, quality 0.85) + mipmaps + size_limit (las texturas
# de frames a resolucion completa ~1900px comen ~1.5GB de VRAM por personaje y
# producen micro-trabas; a 1024 se ven igual con scale ajustado en el .tscn).
# Tras parchear hay que volver a publicar para que el cambio entre al .pck.
param(
    [Parameter(Mandatory)][string]$Dir,
    [int]$SizeLimit = 1024
)
$utf8 = New-Object System.Text.UTF8Encoding($false)
$files = Get-ChildItem $Dir -Recurse -Filter "*.webp.import"
$n = 0
foreach ($f in $files) {
    $t = [IO.File]::ReadAllText($f.FullName)
    $orig = $t
    $t = $t -replace 'compress/mode=\d+', 'compress/mode=1'
    $t = $t -replace 'compress/lossy_quality=[\d.]+', 'compress/lossy_quality=0.85'
    $t = $t -replace 'mipmaps/generate=false', 'mipmaps/generate=true'
    $t = $t -replace 'process/size_limit=\d+', "process/size_limit=$SizeLimit"
    if ($t -ne $orig) {
        [IO.File]::WriteAllText($f.FullName, $t, $utf8)
        $n++
    }
}
Write-Output "$n de $($files.Count) .webp.import parcheados en $Dir (size_limit=$SizeLimit)"
