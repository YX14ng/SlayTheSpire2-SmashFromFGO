# Sube los mods FGO a Steam Workshop de StS2 (appid 2868840) como ITEMS SEPARADOS
# (FGOCore + Mash + Morgan + Artoria = 4 items independientes). Requiere SteamCMD + login.
#
# StS2 carga mods de Workshop recursivamente; cada item contiene UNA carpeta de mod (dll+json+pck).
# La descripcion de cada item (EN + 简体中文) vive en tools\workshop_desc\<Mod>.txt (UTF-8);
# el VDF tambien se escribe en UTF-8 para no romper el chino.
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
$descDir = Join-Path $PSScriptRoot "workshop_desc"

# titulo por mod (la descripcion se lee de workshop_desc\<Mod>.txt)
$titles = [ordered]@{
    FGOCore         = "FGO Core — shared library / FGO 核心库"
    MashShielder    = "FGO — Mash Kyrielight 玛修·基列莱特 (Shielder)"
    MorganBerserker = "FGO — Morgan 摩根 (Berserker → Caster)"
    ArtoriaCaster   = "FGO — Artoria Caster 卡斯托莉雅 (Caster)"
}

$targets = if ($Only) { $Only } else { @($titles.Keys) }
$utf8NoBom = New-Object System.Text.UTF8Encoding $false

foreach ($m in $targets) {
    if (-not $titles.Contains($m)) { Write-Warning "Mod desconocido: $m -- salteo."; continue }
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

    # descripcion (EN + 简体中文) desde archivo; sin comillas dobles para no romper el VDF
    $descFile = Join-Path $descDir "$m.txt"
    if (-not (Test-Path $descFile)) { throw "Falta la descripcion: $descFile" }
    $desc = ([System.IO.File]::ReadAllText($descFile)).Trim() -replace '"', "'"
    $prevLine = if ($preview) { "    `"previewfile`" `"$($preview -replace '\\','\\')`"`n" } else { "" }

    $vdf = Join-Path $stage "$m\item.vdf"
    $vdfContent = @"
"workshopitem"
{
    "appid" "$appid"
    "publishedfileid" "$id"
    "contentfolder" "$($content -replace '\\','\\')"
$prevLine    "visibility" "$Visibility"
    "title" "$($titles[$m])"
    "description" "$desc"
    "changenote" "Subida via workshop_upload.ps1"
}
"@
    [System.IO.File]::WriteAllText($vdf, $vdfContent, $utf8NoBom)

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
