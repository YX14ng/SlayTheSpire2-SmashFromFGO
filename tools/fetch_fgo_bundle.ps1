# Descarga bundles de modelo de batalla (y textura opcional) de Atlas Academy a
# assets/reference/bundles/<id>.bundle, verificando que sean UnityFS validos.
# Los bundles de AA son UnityFS SIN cifrar (Unity 2022.3). El paso de extraccion del
# FBX animado sigue siendo MANUAL en la GUI de AssetStudioMod (ver docs/ANIMATIONS.md §1).
#
# Uso:
#   tools/fetch_fgo_bundle.ps1 -Ids 9935400,9935410          # uno o varios IDs
#   tools/fetch_fgo_bundle.ps1 -Ids 9935400 -Texture         # + atlas de textura
#   tools/fetch_fgo_bundle.ps1 -Ids 9935400 -Region NA       # otra region (default JP)
param(
    [Parameter(Mandatory)][string[]]$Ids,
    [switch]$Texture,
    [ValidateSet("JP", "NA")][string]$Region = "JP",
    [int]$DelayMs = 800
)
$ErrorActionPreference = "Continue"
$repo = Split-Path $PSScriptRoot -Parent
$bundleDir = Join-Path $repo "assets\reference\bundles"
if (-not (Test-Path $bundleDir)) { New-Item -ItemType Directory -Force $bundleDir | Out-Null }
$base = "https://static.atlasacademy.io/$Region/Servants"

function Test-UnityFs([string]$path) {
    if (-not (Test-Path $path)) { return $false }
    $fs = [IO.File]::OpenRead($path)
    try {
        $buf = New-Object byte[] 7
        $read = $fs.Read($buf, 0, 7)
        if ($read -lt 7) { return $false }
        return ([Text.Encoding]::ASCII.GetString($buf, 0, 7) -eq "UnityFS")
    } finally { $fs.Close() }
}

$first = $true
foreach ($id in $Ids) {
    if (-not $first) { Start-Sleep -Milliseconds $DelayMs }
    $first = $false

    $bundleUrl = "$base/$id/$id"
    $out = Join-Path $bundleDir "$id.bundle"
    Write-Output "== $id =="
    Write-Output "  GET $bundleUrl"
    # curl.exe: sigue redirects (-L), falla en HTTP >=400 (-f), silencioso salvo errores.
    & curl.exe -sfL --max-time 120 -o $out $bundleUrl
    if ($LASTEXITCODE -ne 0) {
        Write-Output "  [X] descarga fallo (exit $LASTEXITCODE) — ¿ID equivocado o sin modelo? Probar la API nice/JP/svt/$id"
        if (Test-Path $out) { Remove-Item $out -Force }
        continue
    }
    $kb = [math]::Round((Get-Item $out).Length / 1KB)
    if (Test-UnityFs $out) {
        Write-Output "  [OK] bundle UnityFS: $kb KB -> assets/reference/bundles/$id.bundle"
    } else {
        Write-Output "  [!] descargado ($kb KB) pero NO empieza con 'UnityFS' — puede ser un 404/HTML o cifrado. Revisar a mano."
    }

    if ($Texture) {
        # AA sirve las texturas bajo /textures/<id>[_NN].png (subcarpeta textures/, NO la raiz).
        # Modelos con varias capas (p.ej. la bestia gigante 9935410) tienen varios atlas
        # (_01/_02/_03). Probamos <id>.png y luego <id>_01.._NN hasta que 404.
        $texBase = "$base/$id/textures"
        $candidates = @("$id.png", "${id}_01.png", "${id}_02.png", "${id}_03.png", "${id}_04.png")
        $got = 0
        foreach ($name in $candidates) {
            $texOut = Join-Path $bundleDir $name
            & curl.exe -sfL --max-time 120 -o $texOut "$texBase/$name" 2>$null
            if ($LASTEXITCODE -eq 0 -and (Test-Path $texOut) -and (Get-Item $texOut).Length -gt 0) {
                $tkb = [math]::Round((Get-Item $texOut).Length / 1KB)
                Write-Output "  [OK] textura: $name ($tkb KB)"
                $got++
            } elseif (Test-Path $texOut) { Remove-Item $texOut -Force }
        }
        if ($got -eq 0) {
            Write-Output "  [i] sin texturas sueltas en /textures/ (el atlas suele venir DENTRO del bundle, lo extrae la GUI)"
        }
    }
}
Write-Output "LISTO. Siguiente paso (MANUAL, GUI): docs/ANIMATIONS.md §1 — AssetStudioMod GUI -> Load <id>.bundle -> Animator chr + AnimationClips -> 'Export Animator + selected AnimationClips' -> assets/reference/extracted/<id>_anim/"
# Salir limpio: los 404 esperados de candidatos de textura inexistentes dejan
# $LASTEXITCODE=22 (curl -f); no es un fallo del script.
exit 0
