param(
    [string]$Root = 'F:\Programs\SlayTheSpire2-SmashFromFGO',
    [string[]]$Projects = @(),
    [string]$PreviewDir = '',
    [int]$MaxCards = 0
)

Add-Type -AssemblyName System.Drawing
$ErrorActionPreference = 'Stop'

$officialNamesByAsset = @{}
$officialCatalog = Join-Path $Root 'assets\reference\ce\ce_names.tsv'
if (Test-Path $officialCatalog) {
    foreach ($line in [System.IO.File]::ReadLines($officialCatalog)) {
        $parts = $line.Split("`t", 3)
        if ($parts.Count -eq 3 -and $parts[1] -and $parts[2]) {
            $officialNamesByAsset[$parts[1]] = $parts[2]
        }
    }
}

$characters = @(
    @{ Project='MashShielder'; Resource='MashShielder'; Code='MashShielderCode'; Base='base'; MapFiles=@(
        'mapping.csv', 'mapping_memes.csv', 'mash_missing_mapping.csv'); Forms=@{
        base='character\frames'; ortinax='character\frames_ortinax'; paladin='character\frames_paladin' }; Backdrops=@{
        base=@('assets\reference\charagraph\mash_base_1.png', 'assets\reference\charagraph\mash_base_final.png');
        ortinax=@('assets\reference\charagraph\mash_ortinax.png');
        paladin=@('assets\reference\charagraph\mash_paladin.png') }; Palettes=@{
        base=@('#151a2f', '#7256a8'); ortinax=@('#0f1626', '#2c64a3'); paladin=@('#3e442b', '#c9b45a') } },
    @{ Project='MorganBerserker'; Resource='MorganBerserker'; Code='MorganBerserkerCode'; Base='queen'; MapFiles=@(
        'mapping_morgan.csv', 'official_replacements_morgan.csv'); Forms=@{
        queen='character\frames_queen'; aesc='character\frames_aesc'; winter='character\frames_winter' }; Backdrops=@{
        queen=@('assets\reference\ce\art\chara_704000a_1.png', 'assets\reference\ce\art\chara_704000a_2.png', 'assets\reference\ce\art\chara_704000b_1.png', 'assets\reference\ce\art\chara_704000b_2.png');
        aesc=@('assets\reference\ce\art\chara_505300a_1.png', 'assets\reference\ce\art\chara_505300a_2.png', 'assets\reference\ce\art\chara_505300b_1.png');
        winter=@('assets\reference\charagraph\704030a.png') }; Palettes=@{
        queen=@('#071923', '#0086a5'); aesc=@('#334964', '#8eaed5'); winter=@('#1b2146', '#7c6bd9') } },
    @{ Project='ArtoriaCaster'; Resource='ArtoriaCaster'; Code='ArtoriaCasterCode'; Base='caster'; MapFiles=@(
        'mapping_artoria.csv'); Forms=@{
        caster='character\frames_caster'; berserker='character\frames_berserker'; avalon='character\frames_avalon' }; Backdrops=@{
        caster=@('assets\reference\ce\art\chara_504500a_1.png', 'assets\reference\ce\art\chara_504500a_2.png', 'assets\reference\ce\art\chara_504500b_2.png');
        berserker=@('assets\reference\ce\art\chara_704700a_1.png', 'assets\reference\ce\art\chara_704700a_2.png');
        avalon=@('assets\reference\ce\art\chara_704700b_1.png', 'assets\reference\ce\art\chara_704700b_2.png') }; Palettes=@{
        caster=@('#193e65', '#66b5e9'); berserker=@('#4c2e68', '#ea9bc3'); avalon=@('#4a3730', '#f0d279') } },
    @{ Project='MordredSaber'; Resource='MordredSaber'; Code='MordredSaberCode'; Base='base'; MapFiles=@(
        'MordredSaber_cards.csv', 'official_replacements_mordred.csv'); Forms=@{
        base='character\frames' }; Backdrops=@{
        base=@('assets\reference\charagraph\100900_cg.png', 'assets\reference\ce\art\chara_100900b_2.png') }; Palettes=@{
        base=@('#3a0b0b', '#e03c2f') } },
    @{ Project='GilgameshArcher'; Resource='GilgameshArcher'; Code='GilgameshArcherCode'; Base='base'; MapFiles=@(
        'GilgameshArcher_cards.csv'); Forms=@{
        base='character\frames' }; Backdrops=@{
        base=@('assets\reference\charagraph\200200_cg.png', 'assets\reference\ce\art\chara_200200a_1.png', 'assets\reference\ce\art\chara_200200a_2.png') }; Palettes=@{
        base=@('#4a2a00', '#ddaa22') } },
    @{ Project='OkitaSaber'; Resource='OkitaSaber'; Code='OkitaSaberCode'; Base='base'; MapFiles=@(
        'OkitaSaber_cards.csv'); Forms=@{
        base='character\frames' }; Backdrops=@{
        base=@('assets\reference\charagraph\102700_cg.png') }; Palettes=@{
        base=@('#233a4a', '#7fc4cf') } },
    @{ Project='OberonPretender'; Resource='OberonPretender'; Code='OberonPretenderCode'; Base='base'; MapFiles=@(
        'OberonPretender_cards.csv'); Forms=@{
        base='character\frames'; winter='character\frames_winter'; vortigern='character\vortigern_static.png' }; Backdrops=@{
        base=@('assets\reference\charagraph\2800100_cg.png');
        winter=@('assets\reference\charagraph\2800100b_at_1.png');
        vortigern=@('OberonPretender\OberonPretender\character\vortigern_static.png') }; Palettes=@{
        base=@('#27313a', '#698896'); winter=@('#101a2e', '#78c8f0'); vortigern=@('#030710', '#0d6d9e') } },
    @{ Project='SiegfriedSaber'; Resource='SiegfriedSaber'; Code='SiegfriedSaberCode'; Base='base'; MapFiles=@(
        'siegfried_cards.csv', 'official_replacements_siegfried.csv'); Forms=@{
        base='character\frames' }; Backdrops=@{
        base=@('assets\reference\ce\art\chara_100800a_2.png', 'assets\reference\ce\art\chara_100800c_1.png') }; Palettes=@{
        base=@('#172b38', '#44a6c4') } },
    @{ Project='Tiamat'; Resource='TiamatBeast'; Code='TiamatCode'; Base='femme'; MapFiles=@(
        'tiamat_cards.csv', 'tiamat_missing_mapping.csv'); Forms=@{
        femme='character\frames_femme'; beast='character\frames_beast' }; Backdrops=@{
        femme=@('assets\reference\charagraph\9935400a.png'); beast=@() }; Palettes=@{
        femme=@('#27314f', '#a78be3'); beast=@('#1d1830', '#6c54aa') } }
)

function Convert-ToSnakeCase([string]$name) {
    $step1 = [regex]::Replace($name, '([A-Z]+)([A-Z][a-z])', '$1_$2')
    [regex]::Replace($step1, '([a-z0-9])([A-Z])', '$1_$2').ToLowerInvariant()
}

function Get-CardTypes([string]$codeRoot) {
    $result = @{}
    foreach ($file in Get-ChildItem $codeRoot -Recurse -Filter '*.cs' -File) {
        $text = [System.IO.File]::ReadAllText($file.FullName, [System.Text.Encoding]::UTF8)
        $match = [regex]::Match($text, 'public\s+(?:sealed\s+)?class\s+([A-Za-z0-9_]+).*?\(\s*\d+\s*,\s*CardType\.(Attack|Skill|Power)', 'Singleline')
        if ($match.Success) {
            $result[(Convert-ToSnakeCase $match.Groups[1].Value)] = $match.Groups[2].Value
        }
    }
    $result
}

function Get-Form([string]$project, [string]$card, [string]$fallback) {
    switch ($project) {
        'MashShielder' {
            if ($card -match 'ortinax|black_barrel|bunker|amalgam|paradox|cylinder|conceptual|covering_fire|suppressing_shot') { return 'ortinax' }
            if ($card -match 'paladin|galahad|camelot|rhongomyniad|utopia|round_table') { return 'paladin' }
        }
        'MorganBerserker' {
            if ($card -match 'aesc|rain|witch|tonelico|londinium|savior|ash_tree|vivian') { return 'aesc' }
            if ($card -match 'winter|ice|snow|frost|hailstorm|worlds_end|storm') { return 'winter' }
        }
        'ArtoriaCaster' {
            if ($card -match 'avalon|caliburn|sacred_sword|hope_will_camelot') { return 'avalon' }
            if ($card -match 'summer|berserker|spring|surf|ice|festival|vacation|white_hare|hydromancy|meadow') { return 'berserker' }
        }
        'OberonPretender' {
            if ($card -match 'vortigern|threat|abyss|eternal_sleep|ending|end_of_dreams|lie_like|worm|vespers|swallowed|resentment|lullaby_of_the_end|while_the_world_sleeps') { return 'vortigern' }
            if ($card -match 'winter|prince|dragonfly_wings|feather_coat') { return 'winter' }
        }
        'Tiamat' {
            if ($card -match 'beast|tide|deluge|sea|chaos|lahmu|dragon|horn|genesis|overtide|black_mud|ravenous|abyss|brackish|brood|devour|drown|larval|leviathan|mitosis|predatory|spawn|undertow|venom|maw|coil|pluma|nammu') { return 'beast' }
        }
    }
    $fallback
}

function Get-Action([string]$cardType, [string]$cardName) {
    if ($cardName -match 'unleashed|enuma|clarent_blood|mumyou|balmung|lord_camelot|rhongomyniad|roadless') { return 'attack' }
    switch ($cardType) {
        'Attack' { 'attack' }
        'Power' { 'idle' }
        default { 'cast' }
    }
}

function Render-OfficialArtwork(
    [string]$sourcePath,
    [string]$outputPath,
    [int]$width,
    [int]$height,
    [double]$focusX,
    [double]$focusY
) {
    $source = [System.Drawing.Bitmap]::FromFile($sourcePath)
    try {
        $cropW = $source.Width
        $cropH = [int]([double]$source.Width * $height / $width)
        if ($cropH -gt $source.Height) {
            $cropH = $source.Height
            $cropW = [int]([double]$source.Height * $width / $height)
        }
        $focusX = [Math]::Max(0.0, [Math]::Min(1.0, $focusX))
        $focusY = [Math]::Max(0.0, [Math]::Min(1.0, $focusY))
        $cropX = [int](($source.Width - $cropW) * $focusX)
        $cropY = [int](($source.Height - $cropH) * $focusY)

        $canvas = New-Object System.Drawing.Bitmap($width, $height, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
        $graphics = [System.Drawing.Graphics]::FromImage($canvas)
        try {
            $graphics.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
            $destination = [System.Drawing.Rectangle]::new(0, 0, $width, $height)
            $sourceRect = [System.Drawing.Rectangle]::new($cropX, $cropY, $cropW, $cropH)
            $graphics.DrawImage($source, $destination, $sourceRect, [System.Drawing.GraphicsUnit]::Pixel)
        } finally { $graphics.Dispose() }
        New-Item -ItemType Directory -Force (Split-Path $outputPath) | Out-Null
        $canvas.Save($outputPath, [System.Drawing.Imaging.ImageFormat]::Png)
        $canvas.Dispose()
    } finally { $source.Dispose() }
}

function Get-OfficialMappings([hashtable]$character) {
    $result = @{}
    foreach ($file in $character.MapFiles) {
        $path = Join-Path $Root "assets\reference\ce\$file"
        if (-not (Test-Path $path)) { throw "Missing art mapping: $path" }
        foreach ($row in Import-Csv $path) {
            if ($row.file -and $row.assetId) { $result[$row.file] = $row }
        }
    }
    $result
}

function Resolve-OfficialSource([string]$assetId) {
    if ($assetId -like 'CHARA:*') {
        $assetKey = $assetId.Substring(6)
        return Join-Path $Root "assets\reference\ce\art\chara_$($assetKey -replace '@','_').png"
    }
    if ($assetId -like 'ITEM:*') {
        return Join-Path $Root "assets\reference\ce\icons\$($assetId.Substring(5)).png"
    }
    Join-Path $Root "assets\reference\ce\art\$assetId.png"
}

function Get-OfficialName($mapping) {
    if ($mapping.officialName) { return $mapping.officialName }
    $assetId = [string]$mapping.assetId
    if ($officialNamesByAsset.ContainsKey($assetId)) { return $officialNamesByAsset[$assetId] }
    if ($assetId -like 'CHARA:*') { return "FGO CharaGraph $($assetId.Substring(6))" }
    if ($assetId -like 'ITEM:*') { return "FGO item $($assetId.Substring(5))" }
    $assetId
}

$commandCards = @{
    MashShielder=@('arts_mash', 'buster_mash', 'quick_mash')
    MorganBerserker=@('arts_morgan', 'buster_morgan', 'quick_morgan')
    ArtoriaCaster=@('arts_artoria', 'buster_artoria', 'quick_artoria')
    MordredSaber=@('arts_command', 'buster_command', 'quick_command')
    GilgameshArcher=@('arts', 'strike', 'quick')
}

$backupRoot = Join-Path $Root 'assets\reference\card_backgrounds'
$report = [System.Collections.Generic.List[object]]::new()

foreach ($character in $characters) {
    if ($Projects.Count -gt 0 -and $Projects -notcontains $character.Project) { continue }
    $projectRoot = Join-Path $Root $character.Project
    $resourceRoot = Join-Path $projectRoot $character.Resource
    $portraitRoot = Join-Path $resourceRoot 'images\card_portraits'
    $codeRoot = Join-Path $projectRoot "$($character.Code)\Cards"
    $types = Get-CardTypes $codeRoot
    $officialMappings = Get-OfficialMappings $character
    $cards = @(Get-ChildItem $portraitRoot -Filter '*.png' -File | Where-Object { $_.BaseName -ne 'card' } | Sort-Object Name)
    if ($MaxCards -gt 0) { $cards = @($cards | Select-Object -First $MaxCards) }

    foreach ($card in $cards) {
        $knownCommands = @($commandCards[$character.Project])
        if ($knownCommands -contains $card.BaseName) {
            $report.Add([pscustomobject]@{
                Project=$character.Project; Card=$card.BaseName; Result='official command card';
                Form=''; Action=''; AssetId='COMMAND'; OfficialName='FGO Command Card'; Backdrop=''
            })
            continue
        }
        $mapping = if ($officialMappings.ContainsKey($card.BaseName)) { $officialMappings[$card.BaseName] } else { $null }
        if ($null -eq $mapping) {
            throw "Missing official art mapping for $($character.Project)/$($card.BaseName). A 2D model fallback is not allowed."
        }
        if ($mapping.assetId -like 'CHARA:*') {
            throw "Direct CharaGraph is not allowed for card art: $($character.Project)/$($card.BaseName) -> $($mapping.assetId)"
        }

        $cardType = if ($types.ContainsKey($card.BaseName)) { $types[$card.BaseName] } elseif ($card.BaseName -match 'strike|slash|shot|thrust|cut|assault|burst|bash|sweep|ram|attack|blade|sword') { 'Attack' } else { 'Skill' }
        $form = Get-Form $character.Project $card.BaseName $character.Base
        $action = Get-Action $cardType $card.BaseName
        $focusX = 0.5
        $focusY = 0.28
        $backdrop = Resolve-OfficialSource $mapping.assetId
        if (-not (Test-Path $backdrop)) { throw "Missing mapped official source: $backdrop" }
        if ($mapping.cropX) { $focusX = [double]::Parse($mapping.cropX, [Globalization.CultureInfo]::InvariantCulture) }
        if ($mapping.cropY) { $focusY = [double]::Parse($mapping.cropY, [Globalization.CultureInfo]::InvariantCulture) }

        if ($PreviewDir) {
            $normalOutput = Join-Path $PreviewDir "$($character.Project)\$($card.Name)"
            $bigOutput = Join-Path $PreviewDir "$($character.Project)\big\$($card.Name)"
        } else {
            $normalBackup = Join-Path $backupRoot "$($character.Resource)\$($card.Name)"
            $bigBackup = Join-Path $backupRoot "$($character.Resource)\big\$($card.Name)"
            $bigCandidate = Join-Path $portraitRoot "big\$($card.Name)"
            $normalOutput = $card.FullName
            $bigOutput = $bigCandidate
        }

        # The official card-specific illustration is the portrait. Do not paste a
        # second combat sprite of the Servant over it: that obscures the subject and
        # makes every card in a character pool look like the same composition.
        Render-OfficialArtwork $backdrop $normalOutput 500 380 $focusX $focusY
        Render-OfficialArtwork $backdrop $bigOutput 1000 760 $focusX $focusY
        if (-not $PreviewDir) {
            # Estas rutas son la referencia limpia que valida el auditor, no un
            # historial del retrato anterior. Deben seguir siempre al mapeo actual.
            New-Item -ItemType Directory -Force (Split-Path $normalBackup) | Out-Null
            New-Item -ItemType Directory -Force (Split-Path $bigBackup) | Out-Null
            Copy-Item $normalOutput $normalBackup -Force
            Copy-Item $bigOutput $bigBackup -Force
        }
        $backdropName = $backdrop.Substring($Root.Length).TrimStart('\')
        $report.Add([pscustomobject]@{
            Project=$character.Project; Card=$card.BaseName;
            Result='official mapped artwork';
            Form=$form; Action=$action;
            AssetId=$mapping.assetId;
            OfficialName=(Get-OfficialName $mapping);
            Backdrop=$backdropName
        })
    }

    if (-not $PreviewDir -and $cards.Count -gt 0) {
        $fallbackSource = @($cards | Where-Object { $_.BaseName -match '^strike(?:_|$)' } | Select-Object -First 1)
        if ($fallbackSource.Count -eq 0) { $fallbackSource = @($cards | Select-Object -First 1) }
        $fallbackNormal = $fallbackSource[0].FullName
        $fallbackBig = Join-Path $portraitRoot "big\$($fallbackSource[0].Name)"
        foreach ($pair in @(
            @($fallbackNormal, (Join-Path $portraitRoot 'card.png')),
            @($fallbackBig, (Join-Path $portraitRoot 'big\card.png'))
        )) {
            try {
                Copy-Item $pair[0] $pair[1] -Force
            } catch [System.IO.IOException] {
                Write-Warning "Fallback kept because another process has it open: $($pair[1])"
            }
        }
    }
}

$reportPath = if ($PreviewDir) { Join-Path $PreviewDir 'report.csv' } else { Join-Path $Root 'docs\ART-CARD-IDENTITY.csv' }
New-Item -ItemType Directory -Force (Split-Path $reportPath) | Out-Null
$reportRows = @($report)
if (-not $PreviewDir -and $Projects.Count -gt 0 -and (Test-Path $reportPath)) {
    $untouchedRows = @(Import-Csv $reportPath | Where-Object { $Projects -notcontains $_.Project })
    $reportRows = @($untouchedRows) + $reportRows
}
$reportRows | Sort-Object Project, Card | Export-Csv $reportPath -NoTypeInformation -Encoding UTF8
Write-Output "Processed $($report.Count) card portraits. Report rows: $($reportRows.Count). Report: $reportPath"
