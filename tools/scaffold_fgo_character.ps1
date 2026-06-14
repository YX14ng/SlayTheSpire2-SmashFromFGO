# Scaffold de un personaje FGO clonando SiegfriedSaber (la plantilla estructural mas limpia).
# Conserva el ESQUELETO (csproj, manifest, MainFile, GlobalUsings, StringExtensions, base Card/Relic/Power,
# las 3 pools, Strike/Defend, configs, character/ tscn, images placeholders) y BORRA el contenido especifico
# de Siegfried (cards/powers/relics/interfaces/DragonTrait/SiegfriedExtensions) para que se implemente nuevo.
# El Character.cs queda clonado pero hay que reescribir su StartingDeck/StartingRelics.
#
# Uso:
#   .\tools\scaffold_fgo_character.ps1 -NewId MordredSaber -NewShort Mordred -NewServant 100900 `
#       -NewName "Mordred - Knight of Treachery" -NewDesc "..."
param(
  [Parameter(Mandatory)][string]$NewId,
  [Parameter(Mandatory)][string]$NewShort,
  [Parameter(Mandatory)][string]$NewServant,
  [string]$NewName = "",
  [string]$NewDesc = "",
  [string]$Root = "f:\Programs\SlayTheSpire2-SmashFromFGO"
)
$ErrorActionPreference = 'Stop'
$OldId = "SiegfriedSaber"; $OldShort = "Siegfried"; $OldServant = "100800"
$src = Join-Path $Root $OldId
$dst = Join-Path $Root $NewId
if (Test-Path $dst) { throw "Ya existe $dst — borralo primero si queres re-scaffoldear." }

Write-Output "== 1. Copiar $OldId -> $NewId (sin obj/bin/.godot/.git/.vs/.uid) =="
robocopy $src $dst /E /XD obj bin .godot .git .vs .idea /XF *.uid *.user | Out-Null
if ($LASTEXITCODE -ge 8) { throw "robocopy fallo ($LASTEXITCODE)" } else { $global:LASTEXITCODE = 0 }

Write-Output "== 2. Borrar contenido especifico de Siegfried (se reimplementa) =="
# Cards: borrar todo menos la base MordredCard (ex SiegfriedCard) y Basic/Strike+Defend.
$codeDir = Join-Path $dst "$($OldId)Code"
Remove-Item -Recurse -Force (Join-Path $codeDir "Cards\Common"),(Join-Path $codeDir "Cards\Uncommon"),`
  (Join-Path $codeDir "Cards\Rare"),(Join-Path $codeDir "Cards\Special") -ErrorAction SilentlyContinue
Get-ChildItem (Join-Path $codeDir "Cards") -Filter "I*.cs" -ErrorAction SilentlyContinue | Remove-Item -Force
# Basic: conservar Strike.cs + Defend.cs; borrar las firmas de Siegfried (DragonbloodCut, BloodBaptism).
Get-ChildItem (Join-Path $codeDir "Cards\Basic") -Filter "*.cs" -ErrorAction SilentlyContinue |
  Where-Object { $_.Name -notin 'Strike.cs','Defend.cs' } | Remove-Item -Force
# Powers: borrar todos menos la base (SiegfriedPower.cs).
Get-ChildItem (Join-Path $codeDir "Powers") -Filter "*.cs" -ErrorAction SilentlyContinue |
  Where-Object { $_.Name -ne "$OldShort`Power.cs" } | Remove-Item -Force
# Relics: borrar todas menos la base (SiegfriedRelic.cs).
Get-ChildItem (Join-Path $codeDir "Relics") -Filter "*.cs" -ErrorAction SilentlyContinue |
  Where-Object { $_.Name -ne "$OldShort`Relic.cs" } | Remove-Item -Force
# Extensions especificas de Siegfried (Mordred tendra las suyas): borrar DragonTrait + SiegfriedExtensions.
Remove-Item -Force (Join-Path $codeDir "Extensions\DragonTrait.cs"),(Join-Path $codeDir "Extensions\$OldShort`Extensions.cs") -ErrorAction SilentlyContinue
# Localization: vaciar (se regenera trilingue).
Get-ChildItem (Join-Path $dst "$OldId\localization") -Recurse -Filter "*.json" -ErrorAction SilentlyContinue | Remove-Item -Force
# card_portraits / icons especificos: dejar solo los placeholders card.png/power.png/relic.png.
foreach ($sub in @("card_portraits","powers","relics")) {
  $d = Join-Path $dst "$OldId\images\$sub"
  if (Test-Path $d) {
    Get-ChildItem $d -Recurse -File -ErrorAction SilentlyContinue |
      Where-Object { $_.Name -notin 'card.png','power.png','relic.png','card.png.import','power.png.import','relic.png.import' } | Remove-Item -Force
  }
}

Write-Output "== 3. Renombrar archivos/carpetas Siegfried -> $NewShort =="
function RenameLeaf($path, $old, $new) {
  if (Test-Path $path) { Rename-Item -LiteralPath $path -NewName ((Split-Path $path -Leaf) -replace [regex]::Escape($old), $new) -Force }
}
# Base class files + Character + pools + character/ assets (hacerlo ANTES de renombrar carpetas padre).
RenameLeaf (Join-Path $codeDir "Cards\$OldShort`Card.cs") $OldShort $NewShort
RenameLeaf (Join-Path $codeDir "Relics\$OldShort`Relic.cs") $OldShort $NewShort
RenameLeaf (Join-Path $codeDir "Powers\$OldShort`Power.cs") $OldShort $NewShort
foreach ($f in @("$OldShort.cs","$OldShort`CardPool.cs","$OldShort`RelicPool.cs","$OldShort`PotionPool.cs")) {
  RenameLeaf (Join-Path $codeDir "Character\$f") $OldShort $NewShort
}
# character/ tscn + tres + pngs (siegfried_* -> short_*)
$charDir = Join-Path $dst "$OldId\character"
if (Test-Path $charDir) {
  Get-ChildItem $charDir -File | Where-Object { $_.Name -match $OldShort.ToLower() } | ForEach-Object {
    Rename-Item -LiteralPath $_.FullName -NewName ($_.Name -replace $OldShort.ToLower(), $NewShort.ToLower()) -Force
  }
}
# Carpetas: Code, asset, csproj, json
Rename-Item -LiteralPath $codeDir -NewName "$($NewId)Code" -Force
Rename-Item -LiteralPath (Join-Path $dst $OldId) -NewName $NewId -Force
RenameLeaf (Join-Path $dst "$OldId.csproj") $OldId $NewId
RenameLeaf (Join-Path $dst "$OldId.json") $OldId $NewId

Write-Output "== 4. Reemplazar texto en archivos (.cs .csproj .json .tscn .tres .cfg .props .godot .gitignore .gitattributes) =="
$exts = '*.cs','*.csproj','*.json','*.tscn','*.tres','*.cfg','*.props','*.godot','*.gitignore','*.gitattributes','*.md'
$files = Get-ChildItem $dst -Recurse -Include $exts -File | Where-Object { $_.FullName -notmatch '\\(obj|bin|\.godot)\\' }
foreach ($f in $files) {
  $t = [System.IO.File]::ReadAllText($f.FullName, [System.Text.Encoding]::UTF8)
  $t = $t -creplace 'SIEGFRIEDSABER', ($NewId.ToUpper())
  $t = $t -creplace 'SiegfriedSaber', $NewId
  $t = $t -creplace 'Siegfried', $NewShort
  $t = $t -creplace 'siegfried', $NewShort.ToLower()
  $t = $t.Replace($OldServant, $NewServant)
  [System.IO.File]::WriteAllText($f.FullName, $t, (New-Object System.Text.UTF8Encoding $false))
}

Write-Output "== 5. Manifest (id/name/description) =="
$mj = Join-Path $dst "$NewId.json"
$man = Get-Content -Raw $mj | ConvertFrom-Json
$man.id = $NewId
if ($NewName) { $man.name = $NewName }
if ($NewDesc) { $man.description = $NewDesc }
($man | ConvertTo-Json -Depth 10) | Set-Content -Path $mj -Encoding utf8

Write-Output "== LISTO: $NewId scaffoldeado =="
Write-Output "Pendiente: implementar Cards/Powers/Relics + reescribir Character\$NewShort.cs (StartingDeck/Relics) + loc trilingue."
Get-ChildItem $dst -Recurse -Filter "*.cs" | Measure-Object | ForEach-Object { "  .cs conservados (esqueleto): $($_.Count)" }