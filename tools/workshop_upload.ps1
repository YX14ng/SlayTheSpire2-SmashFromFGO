# Sube los mods FGO a Steam Workshop de StS2 (appid 2868840) como ITEMS SEPARADOS
# (FGOCore + 12 personajes = 13 items independientes). Requiere SteamCMD + login.
# Todo el lote se envia dentro de UNA sesion de SteamCMD para no reconectar por cada item.
#
# StS2 carga mods de Workshop recursivamente; cada item contiene UNA carpeta de mod (dll+json+pck).
# La descripcion de cada item (ES + EN + 简体中文) vive en tools\workshop_desc\<Mod>.txt (UTF-8);
# el VDF tambien se escribe en UTF-8 para no romper el chino.
#
# USO (primer upload de TODOS, quedan PRIVADOS para testear):
#   .\tools\workshop_upload.ps1 -SteamUser TU_USUARIO_STEAM
# Preparar y validar TODO sin conectarse a Steam:
#   .\tools\workshop_upload.ps1 -StageOnly
# Subir/actualizar SOLO uno:
#   .\tools\workshop_upload.ps1 -SteamUser TU_USUARIO -Only MashShielder
# Hacerlos PUBLICOS (tras testear): re-corre con -Visibility 0 (reusa los ids guardados).
#
# IDs: tras el 1er upload de cada item, SteamCMD imprime 'PublishedFileID <n>'. Guardalo en
#   tools\.workshop_id_<Mod>.txt (un numero por archivo) y los siguientes runs ACTUALIZAN ese
#   item en vez de crear duplicados. (Si el archivo no existe, el script crea un item NUEVO.)
param(
    [string]$SteamUser,
    [ValidateSet("","0","1","2")][string]$Visibility = "", # vacio=conservar; 0=publico 1=amigos 2=privado
    [string[]]$Only,
    [string]$ChangeNote = "FGO mod update",
    [switch]$StageOnly,
    [string]$SteamCmd,
    [string]$ModsRoot
)
$ErrorActionPreference = "Stop"
$appid = "2868840"
$repo  = Split-Path $PSScriptRoot -Parent
$SteamCmd = if ($SteamCmd) { $SteamCmd } else { Join-Path $PSScriptRoot 'steamcmd\steamcmd.exe' }
$ModsRoot = if ($ModsRoot) { $ModsRoot } else { Join-Path $repo 'dist' }
$stage = Join-Path $repo ".workshop_stage"
$descDir = Join-Path $PSScriptRoot "workshop_desc"

# titulo por mod (la descripcion se lee de workshop_desc\<Mod>.txt)
$titles = [ordered]@{
    FGOCore         = "FGO Core — shared library / FGO 核心库"
    MashShielder    = "FGO — Mash Kyrielight 玛修·基列莱特 (Shielder)"
    MorganBerserker = "FGO — Morgan 摩根 (Berserker → Caster)"
    ArtoriaCaster   = "FGO — Artoria Caster 卡斯托莉雅 (Caster)"
    MordredSaber    = "FGO - Mordred (Saber of Red)"
    GilgameshArcher = "FGO - Gilgamesh (Archer)"
    OkitaSaber      = "FGO - Okita Souji (Saber)"
    OberonPretender = "FGO - Oberon (Pretender)"
    SiegfriedSaber  = "FGO - Siegfried (Saber)"
    TiamatBeast     = "FGO - Tiamat (Beast)"
    KagetoraLancer  = "FGO — Nagao Kagetora / Uesugi Kenshin (Lancer → Ruler)"
    ShutenDouji     = "FGO — Shuten Douji (Assassin / Caster)"
    AstolfoRider    = "FGO — Astolfo 阿斯托尔福 (Rider)"
}

$targets = if ($Only) { $Only } else { @($titles.Keys) }
$utf8NoBom = New-Object System.Text.UTF8Encoding $false
$uploadQueue = @()

foreach ($m in $targets) {
    if (-not $titles.Contains($m)) { Write-Warning "Mod desconocido: $m -- salteo."; continue }
    $src = Join-Path $ModsRoot $m
    foreach ($ext in @("dll","json","pck")) {
        if (-not (Test-Path (Join-Path $src "$m.$ext"))) { throw "Falta $m.$ext en $src -- publica primero ese mod (dotnet publish)." }
    }

    # id guardado (0 = item nuevo)
    $idFile = Join-Path $PSScriptRoot ".workshop_id_$m.txt"
    $id = if (Test-Path $idFile) { (Get-Content $idFile -Raw).Trim() } else { "0" }

    # Conserva la visibilidad del ultimo upload salvo que se indique una nueva.
    $previousVdf = Join-Path $stage "$m\item.vdf"
    $previousVisibility = if (Test-Path $previousVdf) {
        $match = [regex]::Match([System.IO.File]::ReadAllText($previousVdf), '"visibility"\s+"([0-2])"')
        if ($match.Success) { $match.Groups[1].Value } else { "2" }
    } else { "2" }
    $effectiveVisibility = if ($Visibility) { $Visibility } else { $previousVisibility }

    # contenido = una carpeta con la subcarpeta del mod (dll+json+pck)
    $content = Join-Path $stage "$m\content"
    if (Test-Path $content) { Remove-Item -Recurse -Force $content }
    $dst = Join-Path $content $m
    New-Item -ItemType Directory -Force $dst | Out-Null
    Copy-Item (Join-Path $src "$m.dll")  $dst
    Copy-Item (Join-Path $src "$m.json") $dst
    Copy-Item (Join-Path $src "$m.pck")  $dst

    # descripcion DEFAULT (ingles) desde archivo; sin comillas dobles para no romper el VDF.
    # NOTA: SteamCMD solo setea UNA descripcion (la default/ingles). Las descripciones por
    # idioma (es/zh) se ponen aparte (editor web o herramienta Steamworks).
    # PREVIEW: NO se toca aca a proposito -- el usuario puso fondos/iconos a mano en la web;
    # re-subir el preview los pisaria.
    $descFile = Join-Path $descDir "$m.txt"
    if (-not (Test-Path $descFile)) { throw "Falta la descripcion: $descFile" }
    $desc = ([System.IO.File]::ReadAllText($descFile)).Trim() -replace '"', "'"

    $vdf = Join-Path $stage "$m\item.vdf"
    $vdfContent = @"
"workshopitem"
{
    "appid" "$appid"
    "publishedfileid" "$id"
    "contentfolder" "$($content -replace '\\','\\')"
    "visibility" "$effectiveVisibility"
    "title" "$($titles[$m])"
    "description" "$desc"
    "changenote" "$($ChangeNote -replace '"', "'")"
}
"@
    [System.IO.File]::WriteAllText($vdf, $vdfContent, $utf8NoBom)

    Write-Host ""
    Write-Host "==================================================================="
    Write-Host " Preparado: $m  (item id: $id$(if($id -eq '0'){' = NUEVO'}))"
    Write-Host "==================================================================="
    $uploadQueue += [pscustomobject]@{ Mod = $m; Vdf = $vdf; Id = $id; IdFile = $idFile }
}

if ($uploadQueue.Count -eq 0) { throw "No hay items validos para procesar." }

if ($StageOnly) {
    Write-Host ""
    Write-Host "Preparacion terminada sin conectar con Steam. Items: $($uploadQueue.Mod -join ', ')."
    return
}

if ([string]::IsNullOrWhiteSpace($SteamUser)) {
    throw "SteamUser es obligatorio salvo cuando se usa -StageOnly."
}

# SteamCMD acepta varios comandos consecutivos. Iniciar sesion una sola vez evita que
# una publicacion completa abra y cierre diez sesiones con la misma cuenta de Steam.
$steamArgs = @("+login", $SteamUser)
foreach ($item in $uploadQueue) {
    $steamArgs += @("+workshop_build_item", $item.Vdf)
}
$steamArgs += "+quit"

Write-Host ""
Write-Host "Conectando una sola vez para publicar $($uploadQueue.Count) item(s)..."
& $SteamCmd @steamArgs 2>&1 | Tee-Object -Variable steamOutput
$steamExitCode = if ($null -eq $LASTEXITCODE) { 0 } else { $LASTEXITCODE }
$successCount = [regex]::Matches(($steamOutput -join "`n"), 'Committing update\.\.\.Success').Count

if ($steamExitCode -ne 0 -or $successCount -ne $uploadQueue.Count) {
    throw "SteamCMD confirmo $successCount de $($uploadQueue.Count) updates (exit code $steamExitCode)."
}

foreach ($item in $uploadQueue) {
    if ($item.Id -ne "0") { continue }
    $updatedVdf = [System.IO.File]::ReadAllText($item.Vdf)
    $idMatch = [regex]::Match($updatedVdf, '"publishedfileid"\s+"([1-9][0-9]+)"')
    if ($idMatch.Success) {
        [System.IO.File]::WriteAllText($item.IdFile, $idMatch.Groups[1].Value + "`n", $utf8NoBom)
    }
}

Write-Host ""
Write-Host "Listo. Items publicados en una sola sesion: $($uploadQueue.Mod -join ', ')."
