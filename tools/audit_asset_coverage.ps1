param([string]$Root = (Split-Path $PSScriptRoot -Parent))

Add-Type -AssemblyName System.Drawing
$ErrorActionPreference = 'Stop'
$missing = [Collections.Generic.List[string]]::new()
$wrongSize = [Collections.Generic.List[string]]::new()
$totals = [ordered]@{ Projects = 0; Cards = 0; Powers = 0; Relics = 0 }

function Relative([string]$path) {
    $rootPrefix = $Root.TrimEnd('\', '/') + [IO.Path]::DirectorySeparatorChar
    if ($path.StartsWith($rootPrefix, [StringComparison]::OrdinalIgnoreCase)) {
        return $path.Substring($rootPrefix.Length)
    }
    return $path
}

function Assert-Image([string]$path, [int]$width, [int]$height) {
    if (-not (Test-Path $path)) {
        $missing.Add((Relative $path))
        return
    }
    $image = [Drawing.Bitmap]::FromFile($path)
    try {
        if ($width -gt 0 -and $height -gt 0 -and
            ($image.Width -ne $width -or $image.Height -ne $height)) {
            $relative = Relative $path
            $wrongSize.Add("${relative}: $($image.Width)x$($image.Height), esperado ${width}x${height}")
        }
    } finally { $image.Dispose() }
}

function LocalizedNames([string]$file, [string]$prefix) {
    if (-not (Test-Path $file)) { return @() }
    $json = Get-Content $file -Raw | ConvertFrom-Json
    @($json.psobject.Properties.Name | Where-Object { $_ -match '\.title$' } | ForEach-Object {
        ($_ -replace "^$([regex]::Escape($prefix))-", '' -replace '\.title$', '').ToLowerInvariant()
    } | Sort-Object -Unique)
}

$projects = @(Get-ChildItem $Root -Directory | Where-Object {
    $_.Name -notin @('decompiled', 'FGOCore') -and
    (Get-ChildItem $_.FullName -File -Filter '*.csproj' -ErrorAction SilentlyContinue)
})

foreach ($project in $projects) {
    $cardsFile = Get-ChildItem $project.FullName -Recurse -File -Filter 'cards.json' |
        Where-Object { $_.FullName -match '[\\/]localization[\\/]eng[\\/]cards\.json$' } |
        Select-Object -First 1
    if (-not $cardsFile) {
        $missing.Add("$($project.Name): localization/eng/cards.json")
        continue
    }

    $resourceRoot = Split-Path (Split-Path (Split-Path $cardsFile.FullName -Parent) -Parent) -Parent
    $prefix = ((Get-Content (Get-ChildItem $project.FullName -File -Filter '*.json' | Select-Object -First 1).FullName -Raw | ConvertFrom-Json).id).ToUpperInvariant()
    $images = Join-Path $resourceRoot 'images'
    $totals.Projects++

    $cards = @(LocalizedNames $cardsFile.FullName $prefix)
    $totals.Cards += $cards.Count
    foreach ($name in $cards) {
        Assert-Image (Join-Path $images "card_portraits\$name.png") 500 380
        Assert-Image (Join-Path $images "card_portraits\big\$name.png") 1000 760
    }
    Assert-Image (Join-Path $images 'card_portraits\card.png') 500 380
    Assert-Image (Join-Path $images 'card_portraits\big\card.png') 1000 760

    $powers = @(LocalizedNames (Join-Path $cardsFile.DirectoryName 'powers.json') $prefix)
    $totals.Powers += $powers.Count
    foreach ($name in $powers) {
        Assert-Image (Join-Path $images "powers\$name.png") 0 0
        Assert-Image (Join-Path $images "powers\big\$name.png") 0 0
    }

    $relics = @(LocalizedNames (Join-Path $cardsFile.DirectoryName 'relics.json') $prefix)
    $totals.Relics += $relics.Count
    foreach ($name in $relics) {
        Assert-Image (Join-Path $images "relics\$name.png") 0 0
        Assert-Image (Join-Path $images "relics\big\$name.png") 0 0
        Assert-Image (Join-Path $images "relics\${name}_outline.png") 0 0
    }

    $charUiSets = @(
        @('character_icon_char_name.png', 'char_select_char_name.png', 'char_select_char_name_locked.png', 'map_marker_char_name.png'),
        @('char_icon.png', 'char_select.png', 'char_select_locked.png', 'map_marker.png')
    )
    $completeCharUi = $false
    foreach ($set in $charUiSets) {
        if (@($set | Where-Object { -not (Test-Path (Join-Path $images "charui\$_")) }).Count -eq 0) {
            $completeCharUi = $true
            break
        }
    }
    if (-not $completeCharUi) { $missing.Add("$($project.Name): set completo images/charui") }
}

if ($missing.Count -or $wrongSize.Count) {
    if ($missing.Count) { 'Recursos faltantes:'; $missing }
    if ($wrongSize.Count) { 'Dimensiones incorrectas:'; $wrongSize }
    exit 1
}

Write-Output "Assets: OK ($($totals.Projects) personajes, $($totals.Cards) cartas, $($totals.Powers) poderes, $($totals.Relics) reliquias)"
