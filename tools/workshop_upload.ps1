# Sube los mods FGO a Steam Workshop de StS2 (appid 2868840) como ITEMS SEPARADOS
# (FGOCore + Mash + Morgan + Artoria = 4 items independientes). Requiere SteamCMD + login.
#
# StS2 carga mods de Workshop recursivamente; cada item contiene UNA carpeta de mod (dll+json+pck).
#
# USO (primer upload de TODOS, quedan PRIVADOS para testear):
#   .\tools\workshop_upload.ps1 -SteamUser TU_USUARIO_STEAM
# Subir/actualizar SOLO uno:
#   .\tools\workshop_upload.ps1 -SteamUser TU_USUARIO -Only MashShielder
# Hacerlos PUBLICOS (tras testear): re-corre con -Visibility 0 (reusa los ids guardados).
#
# IDs: tras el 1er upload de cada item, SteamCMD imprime 'PublishedFileID <n>'. Guardalo en
#   tools\.workshop_id_<Mod>.txt (un numero por archivo) y los siguientes runs ACTUALIZAN ese
#   item en vez de crear duplicados. (Si el archivo no existe, el script crea un item NUEVO.)
param(
    [Parameter(Mandatory)][string]$SteamUser,
    [ValidateSet("0","1","2")][string]$Visibility = "2",   # 0=publico 1=amigos 2=privado
    [string[]]$Only,
    [string]$SteamCmd = "$PSScriptRoot\steamcmd\steamcmd.exe",
    [string]$ModsRoot = "C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\mods"
)
$ErrorActionPreference = "Stop"
$appid = "2868840"
$repo  = Split-Path $PSScriptRoot -Parent
$stage = Join-Path $repo ".workshop_stage"

# --- metadata por mod (titulo + descripcion) ---
$mods = [ordered]@{
    FGOCore = @{
        Title = "FGO Core (libreria) — FGO Servants"
        Desc  = "Libreria de mecanicas compartidas de los mods de Servants de Fate/Grand Order (medidor NP/Overcharge, cambio de formas, Baluarte, vinculo, estrellas de critico). REQUERIDA por los personajes FGO. Requiere tambien BaseLib (suscribite aparte)."
    }
    MashShielder = @{
        Title = "Mash Kyrielight (Shielder) — FGO"
        Desc  = "Mash Kyrielight, la Shielder de Fate/Grand Order, como personaje jugable: muralla (Baluarte), Lord Camelot y proteccion de aliados en co-op. Requiere FGO Core (libreria) + BaseLib."
    }
    MorganBerserker = @{
        Title = "Morgan (Berserker -> Caster) — FGO"
        Desc  = "Morgan, la Reina Hada de Fate/Grand Order: Buster-critico + Maldicion, cambio Berserker->Caster, Reina del Invierno. Requiere FGO Core (libreria) + BaseLib."
    }
    ArtoriaCaster = @{
        Title = "Artoria Caster (Castoria) — FGO"
        Desc  = "Artoria Caster de Fate/Grand Order: soporte crit-caster que reparte Estrellas, Carga NP y defensa a la party en co-op. Requiere FGO Core (libreria) + BaseLib."
    }
}

$targets = if ($Only) { $Only } else { @($mods.Keys) }

foreach ($m in $targets) {
    if (-not $mods.Contains($m)) { Write-Warning "Mod desconocido: $m -- salteo."; continue }
    $src = Join-Path $ModsRoot $m
    foreach ($ext in @("dll","json","pck")) {
        if (-not (Test-Path (Join-Path $src "$m.$ext"))) { throw "Falta $m.$ext en $src -- publica primero ese mod (dotnet publish)." }
    }

    # id guardado (0 = item nuevo)
    $idFile = Join-Path $PSScriptRoot ".workshop_id_$m.txt"
    $id = if (Test-Path $idFile) { (Get-Content $idFile -Raw).Trim() } else { "0" }

    # contenido = una carpeta con la subcarpeta del mod (dll+json+pck)
    $content = Join-Path $stage "$m\content"
    if (Test-Path $content) { Remove-Item -Recurse -Force $content }
    $dst = Join-Path $content $m
    New-Item -ItemType Directory -Force $dst | Out-Null
    Copy-Item (Join-Path $src "$m.dll")  $dst
    Copy-Item (Join-Path $src "$m.json") $dst
    Copy-Item (Join-Path $src "$m.pck")  $dst

    # preview = el mod_image del mod
    $preview = Join-Path $stage "$m\preview.png"
    $imgSrc  = Join-Path $repo "$m\$m\mod_image.png"
    if (Test-Path $imgSrc) { Copy-Item $imgSrc $preview -Force } else { $preview = "" }

    $vdf = Join-Path $stage "$m\item.vdf"
    $desc = ($mods[$m].Desc -replace '"', "'")
    $prevLine = if ($preview) { "    `"previewfile`" `"$($preview -replace '\\','\\')`"" } else { "" }
    @"
"workshopitem"
{
    "appid" "$appid"
    "publishedfileid" "$id"
    "contentfolder" "$($content -replace '\\','\\')"
$prevLine
    "visibility" "$Visibility"
    "title" "$($mods[$m].Title)"
    "description" "$desc"
    "changenote" "Subida via workshop_upload.ps1"
}
"@ | Out-File -FilePath $vdf -Encoding ascii

    Write-Host ""
    Write-Host "==================================================================="
    Write-Host " $m  (item id actual: $id$(if($id -eq '0'){' = NUEVO'}))"
    Write-Host " Si es NUEVO: anota el 'PublishedFileID' que imprime SteamCMD abajo"
    Write-Host " y guardalo en: $idFile"
    Write-Host "==================================================================="
    & $SteamCmd +login $SteamUser +workshop_build_item $vdf +quit
}

Write-Host ""
Write-Host "Listo. Items procesados: $($targets -join ', ')."
Write-Host "Recorda: guarda cada PublishedFileID en tools\.workshop_id_<Mod>.txt para que los"
Write-Host "proximos runs ACTUALICEN el item (no creen duplicados)."
