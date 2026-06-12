# Audita los .json de localizacion contra los regex de BaseLib SimpleLoc.Simplify:
# reporta strings donde el UpgradeSwapRegex (-...- / +...+) o el PluralizeRegex
# alterarian el texto (en nuestros mods esos matches son SIEMPRE bugs: hay que
# escapar /+ /- o reformular). Las transformaciones de *oro*, $azul$, !var! son
# intencionales y no se reportan.
param([string[]]$Roots = @(
        "f:\Programs\SlayTheSpire2-SmashFromFGO\MashShielder\MashShielder\localization",
        "f:\Programs\SlayTheSpire2-SmashFromFGO\MorganBerserker\MorganBerserker\localization",
        "f:\Programs\SlayTheSpire2-SmashFromFGO\ArtoriaCaster\ArtoriaCaster\localization",
        "f:\Programs\SlayTheSpire2-SmashFromFGO\FGOCore\FGOCore\localization"))

# Copias exactas de los patrones de SimpleLoc.cs (decompilado BaseLib 3.2.1)
$upgradeSwap = [regex]'(?<=^|[^/])(?:(?:-(.+?)-)|(?:\+(.*?[^/])\+))(?:\+(.*?[^/])\+)?'
$goldHighlight = [regex]'(?<=^|[^/])\*({.+?}|.+?(?=$|[\s*.,|}]))\*?'
$diffVariable = [regex]'!(.*?)!'
$pluralize = [regex]'(.*?{)([^{]+?)((?::[^{]*)?}(?:(?:[^{]*?[^{/])|(?:)))\(([^()]+?)\)'

$hits = 0
foreach ($root in $Roots) {
    foreach ($f in Get-ChildItem $root -Recurse -Filter "*.json") {
        $j = Get-Content $f.FullName -Raw -Encoding UTF8 | ConvertFrom-Json
        foreach ($p in $j.PSObject.Properties) {
            $t = [string]$p.Value
            if ($t.StartsWith('#')) { $t = $t.Substring(1) }
            # mismo orden que Simplify: oro y variables primero (afectan a pluralize)
            $sim = $goldHighlight.Replace($t, '[gold]$1[/gold]')
            $sim = $diffVariable.Replace($sim, '{$1:diff()}')
            $plMatch = $pluralize.Match($sim)
            $upMatch = $upgradeSwap.Match($sim)
            if ($upMatch.Success -or $plMatch.Success) {
                $hits++
                $rel = $f.FullName.Replace("f:\Programs\SlayTheSpire2-SmashFromFGO\", "")
                Write-Output "== $rel :: $($p.Name)"
                if ($upMatch.Success) { Write-Output "   UPGRADE-SWAP mangle: <<$($upMatch.Value)>>" }
                if ($plMatch.Success) { Write-Output "   PLURALIZE mangle: <<$($plMatch.Groups[2].Value)}($($plMatch.Groups[4].Value))>>" }
            }
        }
    }
}
Write-Output "TOTAL strings afectadas: $hits"
