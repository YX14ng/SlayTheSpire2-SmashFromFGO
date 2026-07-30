[CmdletBinding()]
param()

$ErrorActionPreference = "Stop"
$repoRoot = Split-Path $PSScriptRoot -Parent
$descriptionRoot = Join-Path $PSScriptRoot "workshop_desc"
$expectedMods = @(
    "FGOCore",
    "MashShielder",
    "MorganBerserker",
    "ArtoriaCaster",
    "MordredSaber",
    "GilgameshArcher",
    "OkitaSaber",
    "OberonPretender",
    "SiegfriedSaber",
    "TiamatBeast",
    "KagetoraLancer",
    "ShutenDouji",
    "AstolfoRider"
)

$failures = [System.Collections.Generic.List[string]]::new()
$totalCharacters = 0
$spanishHeading = '[h1]ESPA' + [char]0x00D1 + 'OL[/h1]'
$simplifiedChineseHeading = '[h1]' + [char]0x7B80 + [char]0x4F53 + [char]0x4E2D + [char]0x6587 + '[/h1]'

function Add-Failure([string]$message) {
    $failures.Add($message)
}

foreach ($mod in $expectedMods) {
    $descriptionPath = Join-Path $descriptionRoot "$mod.txt"
    $projectDirectory = if ($mod -eq 'TiamatBeast') { 'Tiamat' } else { $mod }
    $manifestPath = Join-Path (Join-Path $repoRoot $projectDirectory) "$mod.json"

    if (-not (Test-Path -LiteralPath $descriptionPath -PathType Leaf)) {
        Add-Failure "${mod}: falta la descripcion."
        continue
    }
    if (-not (Test-Path -LiteralPath $manifestPath -PathType Leaf)) {
        Add-Failure "${mod}: falta el manifest usado para comprobar la version."
        continue
    }

    $text = Get-Content -LiteralPath $descriptionPath -Raw -Encoding UTF8
    $manifest = Get-Content -LiteralPath $manifestPath -Raw -Encoding UTF8 | ConvertFrom-Json
    $totalCharacters += $text.Length

    if ($text.Length -gt 8000) {
        Add-Failure "${mod}: la ficha supera 8000 caracteres ($($text.Length))."
    }
    if ($text -notmatch '^\[h1\]') {
        Add-Failure "${mod}: la ficha no abre con un h1 en ingles."
    }
    if ($text -notmatch [regex]::Escape($spanishHeading)) {
        Add-Failure "${mod}: falta la seccion ESPANOL."
    }
    if ($text -notmatch [regex]::Escape($simplifiedChineseHeading)) {
        Add-Failure "${mod}: falta la seccion en chino simplificado."
    }
    if ($text -match '(?m)^#{1,6}\s' -or $text -match '(?m)^={3,}') {
        Add-Failure "${mod}: contiene encabezados o separadores Markdown en vez de BBCode."
    }
    if ($text -notmatch [regex]::Escape([string]$manifest.version)) {
        Add-Failure "${mod}: no declara la version actual $($manifest.version)."
    }
    if ($text -notmatch 'BaseLib 3\.3\.6\+') {
        Add-Failure "${mod}: no declara BaseLib 3.3.6+."
    }
    if ($text -notmatch 'MAIN 0\.107\.1' -or $text -notmatch 'BETA (public |p.blica )?0\.109\.0') {
        Add-Failure "${mod}: no declara las ramas MAIN/BETA compatibles."
    }
    if ($text -notmatch 'github\.com/YX14ng/SlayTheSpire2-SmashFromFGO') {
        Add-Failure "${mod}: falta el enlace al codigo y reportes."
    }
    if ($text -notmatch 'TYPE-MOON' -or $text -notmatch 'Lasengle' -or $text -notmatch 'Mega Crit') {
        Add-Failure "${mod}: falta el aviso de derechos/no afiliacion."
    }

    if ($mod -ne "FGOCore") {
        $coreDependency = @($manifest.dependencies | Where-Object { $_.id -eq 'FGOCore' } | Select-Object -First 1)
        if ($coreDependency.Count -eq 0) {
            Add-Failure "${mod}: el manifest no declara la dependencia FGOCore."
        } else {
            $coreVersion = ([string]$coreDependency[0].min_version) -replace '^v', ''
            $corePattern = 'FGO Core ' + [regex]::Escape($coreVersion) + '\+'
            if ($text -notmatch $corePattern) {
                Add-Failure "${mod}: la ficha no declara FGO Core ${coreVersion}+, como exige el manifest."
            }
        }
    }

    $tagPatterns = @{
        h1 = @('\[h1\]', '\[/h1\]')
        h2 = @('\[h2\]', '\[/h2\]')
        list = @('\[list\]', '\[/list\]')
        b = @('\[b\]', '\[/b\]')
        url = @('\[url=', '\[/url\]')
    }
    foreach ($tag in $tagPatterns.Keys) {
        $opening = ([regex]::Matches($text, $tagPatterns[$tag][0])).Count
        $closing = ([regex]::Matches($text, $tagPatterns[$tag][1])).Count
        if ($opening -ne $closing) {
            Add-Failure "${mod}: etiquetas $tag desbalanceadas ($opening/$closing)."
        }
    }
}

$actualFiles = @(Get-ChildItem -LiteralPath $descriptionRoot -Filter '*.txt' -File | ForEach-Object BaseName)
foreach ($unexpected in $actualFiles | Where-Object { $_ -notin $expectedMods }) {
    Add-Failure "${unexpected}: existe una descripcion sin mod esperado."
}

if ($failures.Count -gt 0) {
    Write-Host "Auditoria de descripciones: $($failures.Count) hallazgo(s)." -ForegroundColor Red
    $failures | ForEach-Object { Write-Host " - $_" }
    exit 1
}

Write-Host "Auditoria de descripciones: OK - $($expectedMods.Count) fichas, $totalCharacters caracteres, BBCode y versiones coherentes."
