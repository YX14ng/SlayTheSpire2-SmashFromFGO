param(
    [ValidateSet('all', 'main', 'beta')]
    [string]$Branch = 'all',
    [switch]$NoRestore
)

$ErrorActionPreference = 'Stop'
$repo = Split-Path $PSScriptRoot -Parent
$coreProject = 'FGOCore\FGOCore.csproj'
$characterProjects = @(
    'MashShielder\MashShielder.csproj',
    'MorganBerserker\MorganBerserker.csproj',
    'ArtoriaCaster\ArtoriaCaster.csproj',
    'MordredSaber\MordredSaber.csproj',
    'GilgameshArcher\GilgameshArcher.csproj',
    'OkitaSaber\OkitaSaber.csproj',
    'OberonPretender\OberonPretender.csproj',
    'SiegfriedSaber\SiegfriedSaber.csproj',
    'Tiamat\TiamatBeast.csproj',
    'KagetoraLancer\KagetoraLancer.csproj',
    'ShutenDouji\ShutenDouji.csproj',
    'AstolfoRider\AstolfoRider.csproj'
)
$projects = @($coreProject) + $characterProjects
$branches = if ($Branch -eq 'all') { @('main', 'beta') } else { @($Branch) }
$compatibilityPropsPath = Join-Path $repo 'Sts2Compatibility.props'
[xml]$compatibilityProps = Get-Content -Raw $compatibilityPropsPath
$compatibilityPropertyGroup = $compatibilityProps.Project.PropertyGroup
$versions = @{
    main = [string]$compatibilityPropertyGroup.MainSts2Version
    beta = [string]$compatibilityPropertyGroup.BetaSts2Version
}

foreach ($target in @('main', 'beta')) {
    if ([string]::IsNullOrWhiteSpace($versions[$target])) {
        throw "Falta la version $target en $compatibilityPropsPath"
    }
}

function Get-ReferenceRoot([string]$Target) {
    Join-Path $repo ".compat\sts2-$Target-$($versions[$Target])"
}

function Test-ReferenceFixture([string]$Target) {
    $assemblyDir = Join-Path (Get-ReferenceRoot $Target) 'data_sts2_windows_x86_64'
    $requiredReferences = @('sts2.dll', 'sts2.xml', '0Harmony.dll', 'GodotSharp.dll', 'Sentry.dll')
    if ($Target -eq 'beta') {
        $requiredReferences += 'Sentry.Godot.dll'
    }

    foreach ($reference in $requiredReferences) {
        $referencePath = Join-Path $assemblyDir $reference
        if (-not (Test-Path $referencePath -PathType Leaf)) {
            throw "Falta la referencia $Target en $referencePath"
        }
    }
}

$preflightTargets = if ($Branch -eq 'beta') { @('beta') } else { @('main', 'beta') }
foreach ($target in $preflightTargets) {
    Test-ReferenceFixture $target
}

Push-Location $repo
try {
    Write-Host '[source] vanilla/custom-content contract audit'
    & (Join-Path $PSScriptRoot 'audit_vanilla_contracts.ps1') -Root $repo
    if (-not $?) {
        throw 'Fallo la auditoria de contratos vanilla/custom-content'
    }

    foreach ($target in $branches) {
        $referenceRoot = Get-ReferenceRoot $target
        $assemblyDir = Join-Path $referenceRoot 'data_sts2_windows_x86_64'

        $stage = Join-Path $repo ".compat\build-$target\"
        foreach ($project in $projects) {
            Write-Host "[$target] $project"
            $args = @('build', $project, '-c', 'Release', "/p:Sts2TargetBranch=$target", "/p:StagingPath=$stage")
            if ($NoRestore) { $args += '--no-restore' }
            & dotnet @args
            if ($LASTEXITCODE -ne 0) {
                throw "Fallo de compatibilidad en $($target): $project"
            }
        }

        $probeProject = Join-Path $repo 'tools\compatibility_probe\CompatibilityProbe.csproj'
        $probeCore = Join-Path $stage 'FGOCore\FGOCore.dll'
        $characterArtifacts = @($characterProjects | ForEach-Object {
            # Normalizado a '/': en Linux GetFileNameWithoutExtension no corta los '\' literales.
            $projectName = [System.IO.Path]::GetFileNameWithoutExtension(($_ -replace '\\', '/'))
            Join-Path $stage "$projectName\$projectName.dll"
        })
        $probeArtifacts = $characterArtifacts
        Write-Host "[$target] runtime compatibility probe"
        $probeArgs = @('run', '--project', $probeProject)
        if ($NoRestore) { $probeArgs += '--no-restore' }
        $probeArgs += @('--', $target, $target, $assemblyDir, $probeCore)
        $probeArgs += $probeArtifacts
        & dotnet @probeArgs
        if ($LASTEXITCODE -ne 0) {
            throw "Fallo de enlace runtime en $target"
        }

        if ($target -eq 'main') {
            $betaAssemblyDir = Join-Path (Get-ReferenceRoot 'beta') 'data_sts2_windows_x86_64'
            Write-Host "[main -> beta] universal artifact compatibility probe"
            $crossProbeArgs = @('run', '--project', $probeProject)
            if ($NoRestore) { $crossProbeArgs += '--no-restore' }
            $crossProbeArgs += @('--', 'beta', 'main', $betaAssemblyDir, $probeCore)
            $crossProbeArgs += $probeArtifacts
            & dotnet @crossProbeArgs
            if ($LASTEXITCODE -ne 0) {
                throw 'El artefacto universal compilado contra MAIN no enlaza correctamente en BETA'
            }
        }
    }
} finally {
    Pop-Location
}

Write-Host "Matriz completada: $($branches -join ', ')"
