param([string[]]$Roots)

$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path $PSScriptRoot -Parent
$languages = @('eng', 'esp', 'zhs', 'kor', 'rus')
if (!$Roots) {
    $Roots = @(
        (Join-Path $repoRoot 'FGOCore\FGOCore\localization'),
        (Join-Path $repoRoot 'MashShielder\MashShielder\localization'),
        (Join-Path $repoRoot 'MorganBerserker\MorganBerserker\localization'),
        (Join-Path $repoRoot 'ArtoriaCaster\ArtoriaCaster\localization'),
        (Join-Path $repoRoot 'MordredSaber\MordredSaber\localization'),
        (Join-Path $repoRoot 'GilgameshArcher\GilgameshArcher\localization'),
        (Join-Path $repoRoot 'OkitaSaber\OkitaSaber\localization'),
        (Join-Path $repoRoot 'OberonPretender\OberonPretender\localization'),
        (Join-Path $repoRoot 'SiegfriedSaber\SiegfriedSaber\localization'),
        (Join-Path $repoRoot 'Tiamat\TiamatBeast\localization'),
        (Join-Path $repoRoot 'KagetoraLancer\KagetoraLancer\localization'),
        (Join-Path $repoRoot 'ShutenDouji\ShutenDouji\localization'),
        (Join-Path $repoRoot 'AstolfoRider\AstolfoRider\localization')
    )
}

$tokenPattern = [regex]'![A-Za-z_][A-Za-z0-9_]*!'
$errors = [System.Collections.Generic.List[string]]::new()

function Read-LocObject([string]$path) {
    try {
        return Get-Content $path -Raw -Encoding UTF8 | ConvertFrom-Json
    } catch {
        $script:errors.Add("JSON invalido: $path :: $($_.Exception.Message)")
        return $null
    }
}

function Property-Map($object) {
    $map = @{}
    if ($null -eq $object) { return $map }
    foreach ($property in $object.PSObject.Properties) {
        $map[$property.Name] = [string]$property.Value
    }
    return $map
}

foreach ($root in $Roots) {
    if (!(Test-Path $root)) {
        $errors.Add("Raiz inexistente: $root")
        continue
    }

    foreach ($language in $languages) {
        if (!(Test-Path (Join-Path $root $language))) {
            $errors.Add("Idioma faltante: $root :: $language")
        }
    }

    $engRoot = Join-Path $root 'eng'
    if (!(Test-Path $engRoot)) { continue }
    $referenceFiles = @(Get-ChildItem $engRoot -Filter '*.json' -File | Sort-Object Name)

    foreach ($language in $languages | Where-Object { $_ -ne 'eng' }) {
        $languageRoot = Join-Path $root $language
        if (!(Test-Path $languageRoot)) { continue }

        $expectedNames = @($referenceFiles | ForEach-Object Name)
        $actualNames = @(Get-ChildItem $languageRoot -Filter '*.json' -File | ForEach-Object Name)
        foreach ($missingFile in @($expectedNames | Where-Object { $_ -notin $actualNames })) {
            $errors.Add("Archivo faltante: $languageRoot\$missingFile")
        }
        foreach ($extraFile in @($actualNames | Where-Object { $_ -notin $expectedNames })) {
            $errors.Add("Archivo extra: $languageRoot\$extraFile")
        }

        foreach ($referenceFile in $referenceFiles) {
            $localizedPath = Join-Path $languageRoot $referenceFile.Name
            if (!(Test-Path $localizedPath)) { continue }

            $reference = Property-Map (Read-LocObject $referenceFile.FullName)
            $localized = Property-Map (Read-LocObject $localizedPath)
            foreach ($key in $reference.Keys) {
                if (!$localized.ContainsKey($key)) {
                    $errors.Add("Clave faltante: $localizedPath :: $key")
                    continue
                }
                $referenceTokens = @($tokenPattern.Matches($reference[$key]) | ForEach-Object Value | Sort-Object)
                $localizedTokens = @($tokenPattern.Matches($localized[$key]) | ForEach-Object Value | Sort-Object)
                if (($referenceTokens -join '|') -ne ($localizedTokens -join '|')) {
                    $errors.Add("Variables distintas: $localizedPath :: $key :: eng=[$($referenceTokens -join ',')] $language=[$($localizedTokens -join ',')]")
                }
            }
            foreach ($key in $localized.Keys) {
                if (!$reference.ContainsKey($key)) {
                    $errors.Add("Clave extra: $localizedPath :: $key")
                }
            }
        }
    }
}

if ($errors.Count -gt 0) {
    $errors | Sort-Object | ForEach-Object { Write-Output $_ }
    Write-Output "TOTAL errores de paridad: $($errors.Count)"
    exit 1
}

Write-Output "Paridad de localizacion OK: $($Roots.Count) proyectos, $($languages.Count) idiomas."
