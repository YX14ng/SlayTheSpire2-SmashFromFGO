param(
    [string]$Root = 'F:\Programs\SlayTheSpire2-SmashFromFGO'
)

Add-Type -AssemblyName System.Drawing
$ErrorActionPreference = 'Stop'

$resources = @{
    MashShielder='MashShielder\MashShielder'
    MorganBerserker='MorganBerserker\MorganBerserker'
    ArtoriaCaster='ArtoriaCaster\ArtoriaCaster'
    MordredSaber='MordredSaber\MordredSaber'
    GilgameshArcher='GilgameshArcher\GilgameshArcher'
    OkitaSaber='OkitaSaber\OkitaSaber'
    OberonPretender='OberonPretender\OberonPretender'
    SiegfriedSaber='SiegfriedSaber\SiegfriedSaber'
    Tiamat='Tiamat\TiamatBeast'
}
$backupNames = @{
    MashShielder='MashShielder'
    MorganBerserker='MorganBerserker'
    ArtoriaCaster='ArtoriaCaster'
    MordredSaber='MordredSaber'
    GilgameshArcher='GilgameshArcher'
    OkitaSaber='OkitaSaber'
    OberonPretender='OberonPretender'
    SiegfriedSaber='SiegfriedSaber'
    Tiamat='TiamatBeast'
}

$rows = @(Import-Csv (Join-Path $Root 'docs\ART-CARD-IDENTITY.csv'))
$model2dSources = @($rows | Where-Object {
    $_.AssetId -like 'CHARA:*' -or
    $_.Result -in @('official direct CharaGraph', 'official character fallback')
})
$missing = [System.Collections.Generic.List[string]]::new()
$badSize = [System.Collections.Generic.List[string]]::new()
$badSource = [System.Collections.Generic.List[string]]::new()
$mappedArtworkMismatches = [System.Collections.Generic.List[string]]::new()
$badFallback = [System.Collections.Generic.List[string]]::new()
$hashRows = [System.Collections.Generic.List[object]]::new()

foreach ($row in $rows) {
    $portraitRoot = Join-Path $Root "$($resources[$row.Project])\images\card_portraits"
    $normal = Join-Path $portraitRoot "$($row.Card).png"
    $big = Join-Path $portraitRoot "big\$($row.Card).png"

    foreach ($expected in @(@($normal, 500, 380), @($big, 1000, 760))) {
        if (-not (Test-Path $expected[0])) {
            $missing.Add($expected[0])
            continue
        }
        $image = [System.Drawing.Bitmap]::FromFile($expected[0])
        try {
            if ($image.Width -ne $expected[1] -or $image.Height -ne $expected[2]) {
                $badSize.Add("$($expected[0]): $($image.Width)x$($image.Height)")
            }
        } finally { $image.Dispose() }
    }

    if ($row.Backdrop -and -not (Test-Path (Join-Path $Root $row.Backdrop))) {
        $badSource.Add("$($row.Project)/$($row.Card): $($row.Backdrop)")
    }

    if ($row.Result -eq 'official mapped artwork') {
        $referenceNormal = Join-Path $Root "assets\reference\card_backgrounds\$($backupNames[$row.Project])\$($row.Card).png"
        $referenceBig = Join-Path $Root "assets\reference\card_backgrounds\$($backupNames[$row.Project])\big\$($row.Card).png"
        foreach ($pair in @(@($normal, $referenceNormal), @($big, $referenceBig))) {
            if (-not (Test-Path $pair[1])) {
                $mappedArtworkMismatches.Add("missing reference: $($pair[1])")
            } elseif ((Get-FileHash $pair[0] -Algorithm SHA256).Hash -ne (Get-FileHash $pair[1] -Algorithm SHA256).Hash) {
                $mappedArtworkMismatches.Add("$($row.Project)/$($row.Card): $($pair[0])")
            }
        }
    }

    if (Test-Path $normal) {
        $hashRows.Add([pscustomobject]@{
            Project=$row.Project
            Card=$row.Card
            Hash=(Get-FileHash $normal -Algorithm SHA256).Hash
        })
    }
}

foreach ($project in $resources.Keys) {
    $portraitRoot = Join-Path $Root "$($resources[$project])\images\card_portraits"
    $cards = @(Get-ChildItem $portraitRoot -Filter '*.png' -File | Where-Object { $_.BaseName -ne 'card' } | Sort-Object Name)
    if ($cards.Count -eq 0) { continue }

    $fallbackSource = @($cards | Where-Object { $_.BaseName -match '^strike(?:_|$)' } | Select-Object -First 1)
    if ($fallbackSource.Count -eq 0) { $fallbackSource = @($cards | Select-Object -First 1) }
    foreach ($pair in @(
        @($fallbackSource[0].FullName, (Join-Path $portraitRoot 'card.png')),
        @((Join-Path $portraitRoot "big\$($fallbackSource[0].Name)"), (Join-Path $portraitRoot 'big\card.png'))
    )) {
        if (-not (Test-Path $pair[1])) {
            $badFallback.Add("$project missing $($pair[1])")
        } elseif ((Get-FileHash $pair[0] -Algorithm SHA256).Hash -ne (Get-FileHash $pair[1] -Algorithm SHA256).Hash) {
            $badFallback.Add("$project fallback differs: $($pair[1])")
        }
    }
}

$crossDuplicates = @($hashRows |
    Group-Object Hash |
    Where-Object { @($_.Group.Project | Sort-Object -Unique).Count -gt 1 })

[pscustomobject]@{
    ReportRows=$rows.Count
    MissingPairs=$missing.Count
    WrongDimensions=$badSize.Count
    MissingSources=$badSource.Count
    InvalidFallbacks=$badFallback.Count
    Model2DCardSources=$model2dSources.Count
    MappedArtworkMismatches=$mappedArtworkMismatches.Count
    CrossCharacterDuplicateGroups=$crossDuplicates.Count
    ResultCounts=(@($rows | Group-Object Result | ForEach-Object { "$($_.Name)=$($_.Count)" }) -join '; ')
} | Format-List

if ($missing.Count -or $badSize.Count -or $badSource.Count -or $badFallback.Count -or $model2dSources.Count -or $mappedArtworkMismatches.Count) {
    if ($missing.Count) { 'Missing:'; $missing }
    if ($badSize.Count) { 'Wrong dimensions:'; $badSize }
    if ($badSource.Count) { 'Missing sources:'; $badSource }
    if ($badFallback.Count) { 'Invalid fallbacks:'; $badFallback }
    if ($model2dSources.Count) { 'Disallowed 2D model / CharaGraph card sources:'; $model2dSources | Format-Table Project, Card, Result, AssetId -AutoSize }
    if ($mappedArtworkMismatches.Count) { 'Mapped artwork differs from its clean official reference:'; $mappedArtworkMismatches }
    if ($crossDuplicates.Count) {
        'Informational cross-character duplicates (allowed when mappings intentionally reuse official art):'
        foreach ($group in $crossDuplicates) { $group.Group | Format-Table Project, Card -AutoSize }
    }
    exit 1
}
