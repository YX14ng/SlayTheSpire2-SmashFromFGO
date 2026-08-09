param([string]$Root = (Split-Path $PSScriptRoot -Parent))

$ErrorActionPreference = 'Stop'
$mods = @(
    'FGOCore',
    'MashShielder',
    'MorganBerserker',
    'ArtoriaCaster',
    'MordredSaber',
    'GilgameshArcher',
    'OkitaSaber',
    'OberonPretender',
    'SiegfriedSaber',
    'TiamatBeast',
    'KagetoraLancer',
    'ShutenDouji',
    'AstolfoRider'
)

function Read-PckDirectory([string]$Path) {
    $stream = [IO.File]::OpenRead($Path)
    $reader = [IO.BinaryReader]::new($stream, [Text.Encoding]::UTF8, $true)
    try {
        $magic = [Text.Encoding]::ASCII.GetString($reader.ReadBytes(4))
        if ($magic -ne 'GDPC') { throw "$Path no es un PCK de Godot" }

        $format = $reader.ReadUInt32()
        if ($format -ne 3) { throw "$Path usa PCK v$format; el auditor conoce v3" }

        $null = $reader.ReadUInt32() # engine major
        $null = $reader.ReadUInt32() # engine minor
        $null = $reader.ReadUInt32() # engine patch
        $null = $reader.ReadUInt32() # pack flags
        $fileBase = $reader.ReadUInt64()
        $directoryOffset = $reader.ReadUInt64()
        $null = $stream.Seek([int64]$directoryOffset, [IO.SeekOrigin]::Begin)

        $fileCount = $reader.ReadUInt32()
        $entries = @{}
        for ($index = 0; $index -lt $fileCount; $index++) {
            $pathLength = $reader.ReadUInt32()
            $entryPath = [Text.Encoding]::UTF8.GetString($reader.ReadBytes($pathLength)).TrimEnd([char]0)
            $offset = $reader.ReadUInt64()
            $size = $reader.ReadUInt64()
            $null = $reader.ReadBytes(16) # MD5 guard stored by Godot
            $flags = $reader.ReadUInt32()
            $entries[$entryPath] = [pscustomobject]@{ Offset = $offset; Size = $size; Flags = $flags }
        }

        [pscustomobject]@{
            Stream = $stream
            Reader = $reader
            FileBase = $fileBase
            Entries = $entries
        }
    }
    catch {
        $reader.Dispose()
        $stream.Dispose()
        throw
    }
}

$hash = [Security.Cryptography.SHA256]::Create()
try {
    foreach ($mod in $mods) {
        $stage = Join-Path $Root "dist\$mod"
        $dllPath = Join-Path $stage "$mod.dll"
        $jsonPath = Join-Path $stage "$mod.json"
        $pckPath = Join-Path $stage "$mod.pck"
        foreach ($required in @($dllPath, $jsonPath, $pckPath)) {
            if (-not (Test-Path -LiteralPath $required -PathType Leaf)) {
                throw "Falta artefacto staged: $required"
            }
        }

        $pck = Read-PckDirectory $pckPath
        try {
            $embeddedDlls = @($pck.Entries.Keys | Where-Object { $_.EndsWith('.dll', [StringComparison]::OrdinalIgnoreCase) })
            if ($embeddedDlls.Count -gt 0) {
                throw "$mod contiene DLL dentro del PCK: $($embeddedDlls -join ', ')"
            }

            $manifestEntry = $pck.Entries["$mod.json"]
            if ($null -eq $manifestEntry) { throw "$mod no contiene $mod.json dentro del PCK" }
            if ($manifestEntry.Size -gt [int]::MaxValue) { throw "Manifest interno demasiado grande: $mod" }

            $null = $pck.Stream.Seek(
                [int64]($pck.FileBase + $manifestEntry.Offset),
                [IO.SeekOrigin]::Begin)
            $internalBytes = $pck.Reader.ReadBytes([int]$manifestEntry.Size)
            $externalBytes = [IO.File]::ReadAllBytes($jsonPath)
            $internalHash = [BitConverter]::ToString($hash.ComputeHash($internalBytes)).Replace('-', '')
            $externalHash = [BitConverter]::ToString($hash.ComputeHash($externalBytes)).Replace('-', '')
            if ($internalHash -ne $externalHash) {
                throw "$mod tiene un manifiesto interno distinto del staged"
            }

            if ($mod -eq 'FGOCore') {
                foreach ($language in @('eng', 'esp', 'zhs', 'kor', 'rus')) {
                    $suffix = "FGOCore/localization/$language/events.json"
                    $eventPaths = @($pck.Entries.Keys | Where-Object {
                        $_.EndsWith($suffix, [StringComparison]::OrdinalIgnoreCase)
                    })
                    if ($eventPaths.Count -ne 1) {
                        throw "FGOCore debe contener exactamente un $suffix; hay $($eventPaths.Count)"
                    }

                    $eventEntry = $pck.Entries[$eventPaths[0]]
                    if ($eventEntry.Flags -ne 0) {
                        throw "El auditor no puede leer el events.json comprimido de FGOCore/$language"
                    }
                    if ($eventEntry.Size -gt [int]::MaxValue) {
                        throw "events.json interno demasiado grande: FGOCore/$language"
                    }
                    $null = $pck.Stream.Seek(
                        [int64]($pck.FileBase + $eventEntry.Offset),
                        [IO.SeekOrigin]::Begin)
                    $eventText = [Text.Encoding]::UTF8.GetString(
                        $pck.Reader.ReadBytes([int]$eventEntry.Size))
                    $colorfulCount = [regex]::Matches(
                        $eventText,
                        'COLORFUL_PHILOSOPHERS\.pages\.INITIAL\.options\.').Count
                    if ($colorfulCount -ne 24) {
                        throw "FGOCore/$language empaqueto $colorfulCount claves de Colorful Philosophers; se esperaban 24"
                    }
                }
            }

            $manifest = [Text.Encoding]::UTF8.GetString($externalBytes) | ConvertFrom-Json
            Write-Host "$mod $($manifest.version): OK ($($pck.Entries.Count) archivos, manifiesto $internalHash)"
        }
        finally {
            $pck.Reader.Dispose()
            $pck.Stream.Dispose()
        }
    }
}
finally {
    $hash.Dispose()
}

Write-Host "Paquetes PCK: OK ($($mods.Count) mods, manifiestos idénticos, cero DLL internas)"
