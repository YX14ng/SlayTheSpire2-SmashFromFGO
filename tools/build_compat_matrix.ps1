param(
    [ValidateSet('all', 'main', 'beta')]
    [string]$Branch = 'all',
    [switch]$NoRestore
)

$ErrorActionPreference = 'Stop'
$repo = Split-Path $PSScriptRoot -Parent
$projects = @(
    'FGOCore\FGOCore.csproj',
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
$branches = if ($Branch -eq 'all') { @('main', 'beta') } else { @($Branch) }

Push-Location $repo
try {
    foreach ($target in $branches) {
        $referenceRoot = if ($target -eq 'main') {
            Join-Path $repo '.compat\sts2-main-0.107.1'
        } else {
            Join-Path $repo '.compat\sts2-beta-0.110.1'
        }
        $assemblyDir = Join-Path $referenceRoot 'data_sts2_windows_x86_64'
        $requiredReferences = @('sts2.dll', '0Harmony.dll', 'GodotSharp.dll')
        if ($target -eq 'beta') {
            $requiredReferences += @('Sentry.dll', 'Sentry.Godot.dll')
        }
        foreach ($reference in $requiredReferences) {
            $referencePath = Join-Path $assemblyDir $reference
            if (-not (Test-Path $referencePath)) {
                throw "Falta la referencia $target en $referencePath"
            }
        }
        $assembly = Join-Path $assemblyDir 'sts2.dll'

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
        $probeAssemblyDir = $assemblyDir
        $probeCore = Join-Path $stage 'FGOCore\FGOCore.dll'
        Write-Host "[$target] runtime compatibility probe"
        $probeArgs = @('run', '--project', $probeProject)
        if ($NoRestore) { $probeArgs += '--no-restore' }
        $probeArgs += @('--', $target, $target, $probeAssemblyDir, $probeCore)
        & dotnet @probeArgs
        if ($LASTEXITCODE -ne 0) {
            throw "Fallo de enlace runtime en $target"
        }

        if ($target -eq 'main') {
            $betaAssemblyDir = Join-Path $repo '.compat\sts2-beta-0.110.1\data_sts2_windows_x86_64'
            Write-Host "[main -> beta] universal artifact compatibility probe"
            $crossProbeArgs = @('run', '--project', $probeProject)
            if ($NoRestore) { $crossProbeArgs += '--no-restore' }
            $crossProbeArgs += @('--', 'beta', 'main', $betaAssemblyDir, $probeCore)
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
