# Parchea los .import de texturas (PNG/WebP) generados por el publish para que el .pck no infle:
# compresion VRAM lossy (mode=1, quality 0.85) + mipmaps + size_limit (las texturas
# de frames a resolucion completa ~1900px comen ~1.5GB de VRAM por personaje y
# producen micro-trabas. Los frames de personaje usan 768 para sostener co-op con
# varios personajes FGO; el resto de las imagenes conserva el limite general de 1024.
# Tras parchear hay que volver a publicar para que el cambio entre al .pck.
param(
    [Parameter(Mandatory)][string]$Dir,
    [int]$SizeLimit = 1024,
    [int]$FrameSizeLimit = 768
)
$utf8 = New-Object System.Text.UTF8Encoding($false)
$files = @(Get-ChildItem $Dir -Recurse -Filter "*.import" | Where-Object {
    [IO.File]::ReadAllText($_.FullName) -match 'importer="texture"'
})
$n = 0
foreach ($f in $files) {
    $t = [IO.File]::ReadAllText($f.FullName)
    $orig = $t
    $t = $t -replace 'compress/mode=\d+', 'compress/mode=1'
    $t = $t -replace 'compress/lossy_quality=[\d.]+', 'compress/lossy_quality=0.85'
    $t = $t -replace 'mipmaps/generate=false', 'mipmaps/generate=true'
    # [\\/] para que el match funcione también en Linux; quality_high/frames NO matchea a
    # propósito (esa variante usa el límite general de 1024).
    $effectiveSizeLimit = if ($f.FullName -match '[\\/]character[\\/]frames') { $FrameSizeLimit } else { $SizeLimit }
    $t = $t -replace 'process/size_limit=\d+', "process/size_limit=$effectiveSizeLimit"
    if ($t -ne $orig) {
        [IO.File]::WriteAllText($f.FullName, $t, $utf8)
        $n++
    }
}
Write-Output "$n de $($files.Count) imports de textura parcheados en $Dir (general=$SizeLimit, frames=$FrameSizeLimit)"
