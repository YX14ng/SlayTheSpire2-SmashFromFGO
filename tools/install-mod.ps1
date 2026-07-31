<#
.SYNOPSIS
  Instala mods FGO desde el STAGING (dist/) a la carpeta mods/ del juego.

  Separación workspace/juego (método iryuko/sts2-mod-dev): el build/publish va SIEMPRE a
  dist/<ModId>/ (nunca a la carpeta del juego). ESTE script es la única vía para llevar un
  mod a la carpeta del juego. Para deploy directo al juego sin script: build/publish con
  /p:DeployToGame=true (no recomendado; rompe Workshop-only si lo dejás).

.EXAMPLE
  ./tools/install-mod.ps1 -Mod TiamatBeast   # dist/TiamatBeast -> <juego>/mods/TiamatBeast
  ./tools/install-mod.ps1 -All               # instala todo lo que haya en dist/
  ./tools/install-mod.ps1 -Clean             # saca los mods FGO del juego (restaura Workshop-only)
#>
param(
    [string]$Mod,
    [switch]$All,
    [switch]$Clean,
    [string]$GameMods = "C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\mods"
)

$ErrorActionPreference = 'Stop'
$RepoRoot = Resolve-Path (Join-Path $PSScriptRoot '..')
$Dist = Join-Path $RepoRoot 'dist'

# Lista canónica de mods FGO (para -Clean y para validar nombres).
$FgoMods = @('FGOCore','MashShielder','MorganBerserker','ArtoriaCaster','MordredSaber',
             'GilgameshArcher','OkitaSaber','OberonPretender','SiegfriedSaber','TiamatBeast',
             'KagetoraLancer','ShutenDouji','AstolfoRider')

function Install-One([string]$name) {
    $src = Join-Path $Dist $name
    if (-not (Test-Path $src)) { Write-Warning "dist/$name no existe (publicalo primero) -- salto"; return }
    $dst = Join-Path $GameMods $name
    New-Item -ItemType Directory -Force $dst | Out-Null
    $copied = 0
    foreach ($ext in 'dll','json','pck') {
        Get-ChildItem -Path $src -Filter "*.$ext" -File -ErrorAction SilentlyContinue | ForEach-Object {
            Copy-Item $_.FullName -Destination $dst -Force; $copied++
        }
    }
    if ($copied -eq 0) { Write-Warning "  ${name}: dist/ existe pero sin dll/json/pck" }
    else { Write-Host "  instalado: $name ($copied archivos -> $dst)" }
}

if ($Clean) {
    Write-Host "Restaurando Workshop-only: sacando mods FGO de la carpeta del juego..."
    foreach ($m in $FgoMods) {
        $p = Join-Path $GameMods $m
        if (Test-Path $p) { Remove-Item -Recurse -Force $p; Write-Host "  removido: $m" }
    }
    Write-Host "Listo. La carpeta mods/ del juego no tiene mods FGO locales."
    return
}

if (-not (Test-Path $Dist)) { throw "No existe dist/ ($Dist). Publica primero: dotnet publish -c Release" }

if ($All) {
    Write-Host "Instalando todo el staging dist/ -> $GameMods"
    Get-ChildItem -Path $Dist -Directory | ForEach-Object { Install-One $_.Name }
} elseif ($Mod) {
    Install-One $Mod
} else {
    Write-Host "Uso: install-mod.ps1 -Mod <ModId> | -All | -Clean"
    Write-Host "Mods FGO conocidos: $($FgoMods -join ', ')"
}
