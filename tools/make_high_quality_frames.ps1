param(
    [string[]]$Mods = @(
        "MashShielder",
        "MorganBerserker",
        "ArtoriaCaster",
        "MordredSaber",
        "GilgameshArcher",
        "OkitaSaber",
        "OberonPretender",
        "SiegfriedSaber",
        "Tiamat",
        "KagetoraLancer",
        "ShutenDouji",
        "AstolfoRider"
    )
)

$ErrorActionPreference = "Stop"
Set-StrictMode -Version Latest

$repoRoot = Split-Path $PSScriptRoot -Parent
$utf8 = New-Object System.Text.UTF8Encoding($false)
$totalResources = 0
$totalFrames = 0

foreach ($mod in $Mods) {
    $modRoot = Join-Path $repoRoot $mod
    if (-not (Test-Path -LiteralPath $modRoot -PathType Container)) {
        throw "No existe el mod: $modRoot"
    }

    if (-not (Test-Path -LiteralPath (Join-Path $modRoot "project.godot") -PathType Leaf)) {
        throw "No se encontró project.godot dentro de $modRoot"
    }

    $manifestPath = Get-ChildItem -LiteralPath $modRoot -File -Filter "*.json" |
        Where-Object { $_.Name -ne "launchSettings.json" } | Select-Object -First 1
    if ($null -eq $manifestPath) {
        throw "No se encontró el manifiesto del mod dentro de $modRoot"
    }
    $manifest = [IO.File]::ReadAllText($manifestPath.FullName) | ConvertFrom-Json
    $resourceRoot = Join-Path $modRoot $manifest.id
    $characterDir = Join-Path $resourceRoot "character"
    $highDir = Join-Path $characterDir "quality_high"
    $resources = @(Get-ChildItem -LiteralPath $characterDir -File -Filter "*frames*.tres")
    $modResources = 0
    $modFrames = 0

    foreach ($resource in $resources) {
        $text = [IO.File]::ReadAllText($resource.FullName)
        $matches = [regex]::Matches(
            $text,
            'res://(?<id>[^/]+)/character/(?<path>[^"\r\n]+\.webp)')
        if ($matches.Count -eq 0) {
            Write-Output "SKIP $mod/$($resource.Name): no contiene fotogramas WebP"
            continue
        }

        $rewritten = $text
        $seen = New-Object 'System.Collections.Generic.HashSet[string]'
        foreach ($match in $matches) {
            $resourceId = $match.Groups['id'].Value
            $relativePath = $match.Groups['path'].Value
            if (-not $seen.Add($relativePath)) { continue }

            $source = Join-Path $characterDir ($relativePath -replace '/', '\')
            if (-not (Test-Path -LiteralPath $source -PathType Leaf)) {
                throw "Falta el fotograma referenciado: $source"
            }

            $destination = Join-Path $highDir ($relativePath -replace '/', '\')
            $destinationDir = Split-Path $destination -Parent
            [IO.Directory]::CreateDirectory($destinationDir) | Out-Null
            Copy-Item -LiteralPath $source -Destination $destination -Force

            $logicalSource = "res://$resourceId/character/$relativePath"
            $logicalHigh = "res://$resourceId/character/quality_high/$relativePath"
            $rewritten = $rewritten.Replace($logicalSource, $logicalHigh)
            $modFrames++
        }

        [IO.Directory]::CreateDirectory($highDir) | Out-Null
        $destinationResource = Join-Path $highDir $resource.Name
        [IO.File]::WriteAllText($destinationResource, $rewritten, $utf8)
        $modResources++
    }

    $totalResources += $modResources
    $totalFrames += $modFrames
    Write-Output "$mod`: $modResources recursos HD, $modFrames fotogramas"
}

Write-Output "TOTAL: $totalResources recursos HD, $totalFrames fotogramas"
