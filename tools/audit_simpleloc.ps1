# Audita ambiguedades de SimpleLoc sin reportar su sintaxis intencional.
# Solo las cadenas que empiezan con # pasan por SimpleLoc.TrySimplify.
param([string[]]$Roots)

$repoRoot = Split-Path $PSScriptRoot -Parent
if (!$Roots) {
    $Roots = @(
        (Join-Path $repoRoot "MashShielder\MashShielder\localization"),
        (Join-Path $repoRoot "MorganBerserker\MorganBerserker\localization"),
        (Join-Path $repoRoot "ArtoriaCaster\ArtoriaCaster\localization"),
        (Join-Path $repoRoot "MordredSaber\MordredSaber\localization"),
        (Join-Path $repoRoot "GilgameshArcher\GilgameshArcher\localization"),
        (Join-Path $repoRoot "OkitaSaber\OkitaSaber\localization"),
        (Join-Path $repoRoot "OberonPretender\OberonPretender\localization"),
        (Join-Path $repoRoot "SiegfriedSaber\SiegfriedSaber\localization"),
        (Join-Path $repoRoot "Tiamat\TiamatBeast\localization"),
        (Join-Path $repoRoot "KagetoraLancer\KagetoraLancer\localization"),
        (Join-Path $repoRoot "ShutenDouji\ShutenDouji\localization"),
        (Join-Path $repoRoot "AstolfoRider\AstolfoRider\localization"),
        (Join-Path $repoRoot "FGOCore\FGOCore\localization")
    )
}

# Copias exactas de los patrones de SimpleLoc.cs (decompilado BaseLib 3.2.1)
$upgradeSwap = [regex]'(?<=^|[^/])(?:(?:-(.+?)-)|(?:\+(.*?[^/])\+))(?:\+(.*?[^/])\+)?'
$goldHighlight = [regex]'(?<=^|[^/])\*({.+?}|.+?(?=$|[\s*.,|}]))\*?'
$blueHighlight = [regex]'(?<=^|[^/])\$({.+?}|.+?(?=$|[\s$.,|}]))\$?'
$diffVariable = [regex]'!(.*?)!'
$pluralize = [regex]'(.*?{)([^{]+?)((?::[^{]*)?}(?:(?:[^{]*?[^{/])|(?:)))\(([^()]+?)\)'

$hits = 0
foreach ($root in $Roots) {
    foreach ($f in Get-ChildItem $root -Recurse -Filter "*.json") {
        $j = Get-Content $f.FullName -Raw -Encoding UTF8 | ConvertFrom-Json
        foreach ($p in $j.PSObject.Properties) {
            $t = [string]$p.Value
            if (!$t.StartsWith('#')) { continue }
            $t = $t.Substring(1)
            # mismo orden que Simplify: oro y variables primero (afectan a pluralize)
            $sim = $goldHighlight.Replace($t, '[gold]$1[/gold]')
            $sim = $blueHighlight.Replace($sim, '[blue]$1[/blue]')
            $sim = $diffVariable.Replace($sim, '{$1:diff()}')

            # Una palabra destacada sin delimitador claro puede atravesar un bloque +mejora+.
            # Al ocultar ese bloque queda una etiqueta RichText abierta y el juego imprime
            # literalmente su cierre automatico [/center]. Reproducir el upgrade swap permite
            # detectar el resultado real que recibe la carta.
            $rendered = $upgradeSwap.Replace($sim, {
                param($m)
                $base = $m.Groups[1].Value
                $upgraded = $m.Groups[2].Value + $m.Groups[3].Value
                return "{IfUpgraded:show:$upgraded|$base}"
            })
            $badTags = @()
            foreach ($tag in @('gold', 'blue')) {
                $opens = ([regex]::Matches($rendered, "\[$tag\]")).Count
                $closes = ([regex]::Matches($rendered, "\[/$tag\]")).Count
                if ($opens -ne $closes) {
                    $badTags += "$tag ($opens/$closes)"
                }
            }

            # Un sufijo plural legitimo es corto y no contiene espacios. Una aclaracion
            # como !D!(hasta 2) debe escribirse !D!/(hasta 2).
            $badPlural = $pluralize.Matches($sim) |
                Where-Object { $_.Groups[4].Value -notmatch '^[^\s()]{1,8}$' } |
                Select-Object -First 1

            # Los bloques +texto+ y -base-+mejora+ son sintaxis valida. El caso
            # sospechoso es un + literal pegado a un numero/variable: /+!Var!.
            $badUpgrade = $upgradeSwap.Matches($sim) |
                Where-Object {
                    $_.Value -notmatch '^-[^-]+-\+.*\+$' -and
                    $_.Value -match '^\+[0-9{]'
                } |
                Select-Object -First 1

            if ($badUpgrade -or $badPlural -or $badTags.Count -gt 0) {
                $hits++
                $rel = $f.FullName.Replace("$repoRoot\", "")
                Write-Output "== $rel :: $($p.Name)"
                if ($badUpgrade) { Write-Output "   LITERAL +/- sin escape: <<$($badUpgrade.Value)>>" }
                if ($badPlural) { Write-Output "   PARENTESIS sin escape: <<$($badPlural.Groups[2].Value)}($($badPlural.Groups[4].Value))>>" }
                if ($badTags.Count -gt 0) { Write-Output "   ETIQUETAS DESBALANCEADAS: $($badTags -join ', ')" }
            }
        }
    }
}
Write-Output "TOTAL ambiguedades: $hits"
