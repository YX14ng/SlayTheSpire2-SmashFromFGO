param([string]$Root = (Split-Path $PSScriptRoot -Parent))

$ErrorActionPreference = 'Stop'
$pattern = [regex]'vfx/[A-Za-z0-9_\-/]+'
$catalogue = [Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)

function Relative([string]$path) {
    $rootPrefix = $Root.TrimEnd('\', '/') + [IO.Path]::DirectorySeparatorChar
    if ($path.StartsWith($rootPrefix, [StringComparison]::OrdinalIgnoreCase)) {
        return $path.Substring($rootPrefix.Length)
    }
    return $path
}

$decompiled = Join-Path $Root 'decompiled'
foreach ($file in (Get-ChildItem $decompiled -Recurse -File -Filter '*.cs')) {
    $text = [IO.File]::ReadAllText($file.FullName)
    foreach ($match in $pattern.Matches($text)) {
        [void]$catalogue.Add($match.Value)
    }
}

$projects = @(Get-ChildItem $Root -Directory | Where-Object {
    $_.Name -ne 'decompiled' -and
    (Get-ChildItem $_.FullName -File -Filter '*.csproj' -ErrorAction SilentlyContinue)
})
$references = 0
$invalid = [Collections.Generic.List[object]]::new()
foreach ($project in $projects) {
    foreach ($file in (Get-ChildItem $project.FullName -Recurse -File -Filter '*.cs' |
        Where-Object { $_.FullName -notmatch '[\\/](obj|bin)[\\/]' })) {
        $text = [IO.File]::ReadAllText($file.FullName)
        foreach ($match in $pattern.Matches($text)) {
            $references++
            if (-not $catalogue.Contains($match.Value)) {
                $relative = Relative $file.FullName
                $line = 1 + ($text.Substring(0, $match.Index).Split("`n").Count - 1)
                $invalid.Add([pscustomobject]@{ Path = $match.Value; File = $relative; Line = $line })
            }
        }
    }
}

if ($invalid.Count -gt 0) {
    Write-Output 'VFX inexistentes:'
    $invalid | Sort-Object Path,File,Line | Format-Table -AutoSize
    exit 1
}

$unique = [Collections.Generic.HashSet[string]]::new([StringComparer]::Ordinal)
foreach ($project in $projects) {
    foreach ($file in (Get-ChildItem $project.FullName -Recurse -File -Filter '*.cs' |
        Where-Object { $_.FullName -notmatch '[\\/](obj|bin)[\\/]' })) {
        foreach ($match in $pattern.Matches([IO.File]::ReadAllText($file.FullName))) {
            [void]$unique.Add($match.Value)
        }
    }
}

Write-Output "VFX: OK ($references referencias, $($unique.Count) rutas unicas, $($projects.Count) proyectos)"
