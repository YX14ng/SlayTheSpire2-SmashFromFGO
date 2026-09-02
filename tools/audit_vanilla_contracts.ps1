param(
    [string]$Root = (Split-Path $PSScriptRoot -Parent)
)

$ErrorActionPreference = 'Stop'
$Root = [IO.Path]::GetFullPath($Root)
$failures = New-Object 'System.Collections.Generic.List[string]'
$checks = 0

function Add-Failure([string]$Message) {
    $script:failures.Add($Message)
}

function Read-Utf8([string]$Path) {
    [IO.File]::ReadAllText($Path, [Text.Encoding]::UTF8)
}

function Assert-Token([string]$Text, [string]$Token, [string]$Context) {
    $script:checks++
    if (-not $Text.Contains($Token)) {
        Add-Failure "${Context}: falta el contrato '$Token'"
    }
}

function Get-SourceFiles([string]$Folder) {
    Get-ChildItem -LiteralPath $Folder -Recurse -Filter '*.cs' -File |
        Where-Object { $_.FullName -notmatch '[\\/](bin|obj)[\\/]' }
}

function Convert-ToSlug([string]$Name) {
    # Coincide con StringHelper.Slugify para los nombres PascalCase de estos modelos.
    ([regex]::Replace($Name.Trim(), '([a-z0-9])([A-Z])', '$1_$2')).ToUpperInvariant()
}

$characters = @(
    @{ Folder = 'MashShielder'; Assembly = 'MashShielder'; Pool = 'MashShielderCardPool' },
    @{ Folder = 'MorganBerserker'; Assembly = 'MorganBerserker'; Pool = 'MorganCardPool' },
    @{ Folder = 'ArtoriaCaster'; Assembly = 'ArtoriaCaster'; Pool = 'ArtoriaCardPool' },
    @{ Folder = 'MordredSaber'; Assembly = 'MordredSaber'; Pool = 'MordredCardPool' },
    @{ Folder = 'GilgameshArcher'; Assembly = 'GilgameshArcher'; Pool = 'GilgameshCardPool' },
    @{ Folder = 'OkitaSaber'; Assembly = 'OkitaSaber'; Pool = 'OkitaCardPool' },
    @{ Folder = 'OberonPretender'; Assembly = 'OberonPretender'; Pool = 'OberonCardPool' },
    @{ Folder = 'SiegfriedSaber'; Assembly = 'SiegfriedSaber'; Pool = 'SiegfriedCardPool' },
    @{ Folder = 'Tiamat'; Assembly = 'TiamatBeast'; Pool = 'TiamatCardPool' },
    @{ Folder = 'KagetoraLancer'; Assembly = 'KagetoraLancer'; Pool = 'KagetoraCardPool' },
    @{ Folder = 'ShutenDouji'; Assembly = 'ShutenDouji'; Pool = 'ShutenCardPool' },
    @{ Folder = 'AstolfoRider'; Assembly = 'AstolfoRider'; Pool = 'AstolfoCardPool' }
)

$requiredCharacterTokens = @(
    'CardTag.Strike',
    'CardTag.Defend',
    'CardRarity.Common',
    'CardRarity.Uncommon',
    'CardRarity.Rare',
    'CardType.Attack',
    'CardType.Skill',
    'CardType.Power',
    'ITranscendenceCard',
    'GetTranscendenceTransformedCard',
    'GetUpgradeReplacement',
    'RegisterCharacterMod<',
    'CreateCustomVisuals',
    'NodeFactory<NCreatureVisuals>.CreateFromScene',
    'IModColorfulPhilosophersCardPool'
)

$colorfulKeys = New-Object 'System.Collections.Generic.List[string]'
foreach ($character in $characters) {
    $folder = Join-Path $Root $character.Folder
    $project = Join-Path $folder "$($character.Assembly).csproj"
    $manifest = Join-Path $folder "$($character.Assembly).json"
    $script:checks += 2
    if (-not (Test-Path -LiteralPath $project -PathType Leaf)) {
        Add-Failure "$($character.Assembly): falta el proyecto $project"
        continue
    }
    if (-not (Test-Path -LiteralPath $manifest -PathType Leaf)) {
        Add-Failure "$($character.Assembly): falta el manifest $manifest"
        continue
    }

    try {
        $manifestJson = Read-Utf8 $manifest | ConvertFrom-Json
        $dependencyIds = @($manifestJson.dependencies | ForEach-Object { $_.id })
        foreach ($dependency in @('STS2-RitsuLib', 'FGOCore')) {
            $script:checks++
            if ($dependencyIds -notcontains $dependency) {
                Add-Failure "$($character.Assembly): el manifest no declara $dependency"
            }
        }
    } catch {
        Add-Failure "$($character.Assembly): manifest JSON invalido: $($_.Exception.Message)"
    }

    $sourceFiles = @(Get-SourceFiles $folder)
    $source = ($sourceFiles | ForEach-Object { Read-Utf8 $_.FullName }) -join "`n"
    foreach ($token in $requiredCharacterTokens) {
        Assert-Token $source $token $character.Assembly
    }

    $poolMatches = @($sourceFiles | Where-Object {
        (Read-Utf8 $_.FullName) -match "class\s+$([regex]::Escape($character.Pool))\b"
    })
    $script:checks++
    if ($poolMatches.Count -ne 1) {
        Add-Failure "$($character.Assembly): se esperaban 1 definicion de $($character.Pool), hay $($poolMatches.Count)"
    }

    $poolEntry = "$($character.Assembly.ToUpperInvariant())-$(Convert-ToSlug $character.Pool)"
    $energyColorName = '{0}{1}{2}' -f 'CARD_POOL', [char]0x2234, $poolEntry
    $colorfulKeys.Add("COLORFUL_PHILOSOPHERS.pages.INITIAL.options.$energyColorName.title")
    $colorfulKeys.Add("COLORFUL_PHILOSOPHERS.pages.INITIAL.options.$energyColorName.description")
}

$coreRoot = Join-Path $Root 'FGOCore'
$coreSource = (@(Get-SourceFiles $coreRoot) | ForEach-Object { Read-Utf8 $_.FullName }) -join "`n"
$coreContracts = @(
    'RegisterTouchOfOrobasRefinementMapping',
    'RegisterArchaicToothTranscendenceMapping',
    'PreparedOrobasUpgradeCompatibility',
    'FgoRelicReplacementStateCompatibility',
    'SeaGlassCompatibility',
    'RunHistorySfxCompatibility',
    'FgoRitsuIntegration',
    'FgoSecondaryResources',
    'FGO_CORE_SECONDARY_RESOURCE_NP_CHARGE',
    'FGO_CORE_SECONDARY_RESOURCE_CRIT_STARS',
    'CreatureCmdCompatibility',
    'LegacyDamageHookCompatibility',
    'FrameResourceSelection[] visible = [current]',
    'ReleaseCombatFrames(NCombatRoom exitingRoom)',
    'Cache.Clear()',
    'HarmonyPatch(typeof(NCombatRoom), nameof(NCombatRoom._ExitTree))'
)
foreach ($token in $coreContracts) {
    Assert-Token $coreSource $token 'FGOCore'
}

$script:checks++
if ($coreSource.Contains('includeAlternates') -or $coreSource.Contains('ResolveGroup(')) {
    Add-Failure 'FGOCore: FormVisuals volvió a incluir precarga de formas alternativas'
}

$lindenLeafPath = Join-Path $Root 'SiegfriedSaber\SiegfriedSaberCode\Relics\LindenLeaf.cs'
$lindenLeafSource = Read-Utf8 $lindenLeafPath
foreach ($token in @(
    '[SavedProperty]',
    'public int StartingScales',
    'StartingScales < MaximumStartingScales',
    'result.UnblockedDamage >= 1',
    'LindenLeafReplacementStatePatch'
)) {
    Assert-Token $lindenLeafSource $token 'Siegfried/LindenLeaf'
}

# Colorful Philosophers creates localization keys from EnergyColorName. The keys live in FGOCore
# so every FGO pool receives all five languages without duplicating them in twelve PCKs.
foreach ($language in @('eng', 'esp', 'zhs', 'kor', 'rus')) {
    $eventsPath = Join-Path $Root "FGOCore\FGOCore\localization\$language\events.json"
    $script:checks++
    if (-not (Test-Path -LiteralPath $eventsPath -PathType Leaf)) {
        Add-Failure "FGOCore/${language}: falta events.json"
        continue
    }
    try {
        $events = Read-Utf8 $eventsPath | ConvertFrom-Json
        $names = @($events.PSObject.Properties | ForEach-Object { $_.Name })
        foreach ($key in $colorfulKeys) {
            $script:checks++
            if ($names -notcontains $key) {
                Add-Failure "FGOCore/$language/events.json: falta '$key'"
            }
        }
    } catch {
        Add-Failure "FGOCore/$language/events.json: JSON invalido: $($_.Exception.Message)"
    }
}

# These are the vanilla consumers whose closed assumptions have caused crashes, missing choices,
# empty rewards or silent UI for custom characters. If Mega Crit changes one, stop the matrix and
# force a fresh review of the corresponding compatibility layer.
[xml]$compatibility = Get-Content -Raw -LiteralPath (Join-Path $Root 'Sts2Compatibility.props')
$betaVersion = [string]$compatibility.Project.PropertyGroup.BetaSts2Version
$vanillaRoots = @(
    @{ Name = 'MAIN'; Path = (Join-Path $Root 'decompiled') },
    @{ Name = 'BETA'; Path = (Join-Path $Root ".compat\decompiled-beta-$betaVersion") }
)
$vanillaContracts = @(
    @{ File = 'LargeCapsule.cs'; Tokens = @('CardTag.Strike', 'CardTag.Defend', '.First(') },
    @{ File = 'DustyTome.cs'; Tokens = @('CardRarity.Ancient', 'NextItem(items)') },
    @{ File = 'SeaGlass.cs'; Tokens = @('Character.CardPool', 'CardRarity.Common', 'CardRarity.Uncommon', 'CardRarity.Rare') },
    @{ File = 'TouchOfOrobas.cs'; Tokens = @('_upgradedRelic', 'ModelDb.Relic<Circlet>()') },
    @{ File = 'ArchaicTooth.cs'; Tokens = @('TranscendenceUpgrades', 'GetTranscendenceTransformedCard') },
    @{ File = 'YummyCookie.cs'; Tokens = @('ModelDb.AllCharacters', 'characterModel is Defect') },
    @{ File = 'ColorfulPhilosophers.cs'; Tokens = @('CardPoolColorOrder', 'EnergyColorName.ToUpperInvariant()') },
    @{ File = 'NMapPointHistoryEntry.cs'; Tokens = @('GetSmallHitSfx', 'GetBigHitSfx', 'return new List<string>()') }
)
foreach ($runtime in $vanillaRoots) {
    $script:checks++
    if (-not (Test-Path -LiteralPath $runtime.Path -PathType Container)) {
        Add-Failure "$($runtime.Name): falta el decompilado $($runtime.Path)"
        continue
    }
    foreach ($contract in $vanillaContracts) {
        $matches = @(Get-ChildItem -LiteralPath $runtime.Path -Recurse -Filter $contract.File -File)
        $script:checks++
        if ($matches.Count -ne 1) {
            Add-Failure "$($runtime.Name): se esperaba 1 $($contract.File), hay $($matches.Count)"
            continue
        }
        $text = Read-Utf8 $matches[0].FullName
        foreach ($token in $contract.Tokens) {
            Assert-Token $text $token "$($runtime.Name)/$($contract.File)"
        }
    }
}

# A new literal/reflection call bypasses compile-time checking. It must be placed in an audited
# compatibility file and covered by the runtime probe before it is allowed into the matrix.
$reflectionAllowList = @(
    'FGOCore\FGOCoreCode\Compatibility\BaseLibCharacterSelectCompatibility.cs',
    'FGOCore\FGOCoreCode\Compatibility\CardCmdCompatibility.cs',
    'FGOCore\FGOCoreCode\Compatibility\CreatureCmdCompatibility.cs',
    'FGOCore\FGOCoreCode\Compatibility\LegacyDamageHookCompatibility.cs',
    'FGOCore\FGOCoreCode\Compatibility\OrobasStarterUpgradeCompatibility.cs',
    'FGOCore\FGOCoreCode\SceneFactoryHardening.cs',
    'FGOCore\FGOCoreCode\CardRewardHardening.cs'
)
$allSourceFiles = @(
    Get-SourceFiles $coreRoot
    foreach ($character in $characters) { Get-SourceFiles (Join-Path $Root $character.Folder) }
)
$reflectionPattern = 'AccessTools\.(Field|Method|Property)|typeof\s*\([^)]*\)\s*\.\s*Get(Field|Method|Property|Constructor)\s*\('
foreach ($file in $allSourceFiles) {
    # Normalizado a '\' para que la allowlist (escrita con backslashes) matchee también en Linux.
    $relative = $file.FullName.Substring($Root.Length).TrimStart('\', '/').Replace('/', '\')
    $lines = [IO.File]::ReadAllLines($file.FullName, [Text.Encoding]::UTF8)
    foreach ($line in $lines) {
        $code = ($line -replace '//.*$', '').Trim()
        if ($code -match $reflectionPattern) {
            $script:checks++
            if ($reflectionAllowList -notcontains $relative) {
                Add-Failure "${relative}: reflexion no inventariada; agregue una comprobacion al compatibility_probe"
            }
        }
        if ($code -match '\bAttackCommand\.FromCard\s*\(' -and
            $relative -ne 'FGOCore\FGOCoreCode\Compatibility\CreatureCmdCompatibility.cs') {
            Add-Failure "${relative}: llamada directa a AttackCommand.FromCard; use CreatureCmdCompatibility"
        }
        if ($code -match '\bResourceLoader\.Load\s*<') {
            Add-Failure "${relative}: carga sincronica de recurso detectada; use la ruta endurecida/asincronica"
        }
    }
}

if ($failures.Count -gt 0) {
    Write-Host "Auditoria de contratos vanilla: $($failures.Count) fallo(s)" -ForegroundColor Red
    foreach ($failure in $failures) { Write-Host "  - $failure" -ForegroundColor Red }
    exit 1
}

Write-Host "Auditoria de contratos vanilla OK: $checks comprobaciones, $($characters.Count) personajes, $($vanillaRoots.Count) runtimes"
