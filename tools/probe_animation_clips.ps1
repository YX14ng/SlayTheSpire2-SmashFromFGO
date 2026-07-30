param(
    [Parameter(Mandatory)][string]$ModelId,
    [string[]]$Clips = @("attack_a", "attack_b", "attack_q", "attack_ex", "spell", "damage_01"),
    [int]$From = 0,
    [int]$To = -1,
    [int]$Step = 5,
    [ValidateSet("debug", "list", "render")][string]$Mode = "debug",
    [switch]$SkipMeasure,
    [switch]$ForceRetarget,
    [string]$Output = ""
)

$ErrorActionPreference = "Stop"

# A native MegaDot access violation must be reported through the captured logs,
# never through a modal Windows dialog that blocks an unattended audit.
if ($null -eq ("AnimationProbe.NativeMethods" -as [type])) {
    Add-Type -TypeDefinition @"
using System.Runtime.InteropServices;
namespace AnimationProbe {
    public static class NativeMethods {
        [DllImport("kernel32.dll")]
        public static extern uint SetErrorMode(uint mode);
    }
}
"@
}
$null = [AnimationProbe.NativeMethods]::SetErrorMode(0x0001 -bor 0x0002)

function Invoke-HiddenProcess {
    param(
        [Parameter(Mandatory)][string]$FilePath,
        [Parameter(Mandatory)][string[]]$Arguments,
        [Parameter(Mandatory)][string]$StdoutPath,
        [Parameter(Mandatory)][string]$StderrPath
    )

    # Some hosts expose both Path and PATH, which makes Start-Process fail.
    # Use a deterministic environment and keep MegaDot fully hidden.
    $startInfo = [Diagnostics.ProcessStartInfo]::new()
    $startInfo.FileName = $FilePath
    $startInfo.Arguments = (($Arguments | ForEach-Object {
        '"' + ([string]$_).Replace('"', '\"') + '"'
    }) -join ' ')
    $startInfo.UseShellExecute = $false
    $startInfo.CreateNoWindow = $true
    $startInfo.RedirectStandardOutput = $true
    $startInfo.RedirectStandardError = $true
    $process = [Diagnostics.Process]::new()
    $process.StartInfo = $startInfo
    if (-not $process.Start()) { throw "No se pudo iniciar $FilePath" }
    $stdoutTask = $process.StandardOutput.ReadToEndAsync()
    $stderrTask = $process.StandardError.ReadToEndAsync()
    $process.WaitForExit()
    $stdout = $stdoutTask.GetAwaiter().GetResult()
    $stderr = $stderrTask.GetAwaiter().GetResult()
    Set-Content -LiteralPath $StdoutPath -Value $stdout -Encoding UTF8
    Set-Content -LiteralPath $StderrPath -Value $stderr -Encoding UTF8
    return $process.ExitCode
}

$repo = Split-Path $PSScriptRoot -Parent
$renderProject = Join-Path $PSScriptRoot "render_project"
$megaDot = Join-Path $repo "MegaDot\MegaDot_v4.5.1-stable_mono_win64.exe"
$megaDotConsole = Join-Path $repo "MegaDot\MegaDot_v4.5.1-stable_mono_win64_console.exe"
$extracted = Join-Path $repo "assets\reference\extracted"
$bundles = Join-Path $repo "assets\reference\bundles"

if (-not (Test-Path -LiteralPath $megaDot)) { throw "Falta MegaDot: $megaDot" }
if ($Step -lt 1) { throw "Step debe ser mayor que cero." }
if ($SkipMeasure -and $Mode -ne "render") { throw "SkipMeasure solo es valido con Mode render." }

$animator = Join-Path $extracted "${ModelId}_anim\Animator"
if (-not (Test-Path -LiteralPath $animator)) {
    $animator = Join-Path $extracted "${ModelId}_anim\FBX_Animator"
}
$candidates = @(
    (Join-Path $animator "chr\chr.fbx"),
    (Join-Path $animator "model\model.fbx")
)
$fbx = $candidates | Where-Object { Test-Path -LiteralPath $_ } | Select-Object -First 1
if (-not $fbx) { throw "No se encontró el FBX animado de $ModelId." }

$driverFbx = $null
if ($ModelId -in @("2800110", "2800120")) {
    $driverFbx = Join-Path $extracted "2800100_anim\Animator\chr\chr.fbx"
    if (-not (Test-Path -LiteralPath $driverFbx)) { throw "Falta el FBX conductor de Oberon base." }
}

$texture = Join-Path $bundles "$ModelId.png"
if (-not (Test-Path -LiteralPath $texture)) { throw "Falta la textura $texture" }

$stageRoot = Join-Path $repo "dist"
$stage = Join-Path $stageRoot ".animation-probe-staging-$([Guid]::NewGuid().ToString('N'))"
$stageFull = [IO.Path]::GetFullPath($stage)
$safeStagePrefix = [IO.Path]::GetFullPath((Join-Path $stageRoot ".animation-probe-staging-"))
if (-not $stageFull.StartsWith($safeStagePrefix, [StringComparison]::OrdinalIgnoreCase)) {
    throw "Ruta temporal insegura: $stageFull"
}
if (-not $Output) {
    $stamp = Get-Date -Format "yyyyMMdd-HHmmss"
    $Output = Join-Path $repo "dist\animation-probes\$ModelId-$stamp"
}

try {
    New-Item -ItemType Directory -Path $stageFull -Force | Out-Null
    New-Item -ItemType Directory -Path $Output -Force | Out-Null
    foreach ($name in @("project.godot", "render.tscn", "render.gd")) {
        Copy-Item -LiteralPath (Join-Path $renderProject $name) -Destination (Join-Path $stageFull $name)
    }
    Copy-Item -LiteralPath $fbx -Destination (Join-Path $stageFull "chr.fbx")
    if ($driverFbx) { Copy-Item -LiteralPath $driverFbx -Destination (Join-Path $stageFull "anim.fbx") }
    Copy-Item -LiteralPath $texture -Destination (Join-Path $stageFull "$ModelId.png")
    foreach ($sourceTexture in (Get-ChildItem -LiteralPath (Split-Path $fbx -Parent) -Filter "*.png" -File)) {
        Copy-Item -LiteralPath $sourceTexture.FullName -Destination (Join-Path $stageFull $sourceTexture.Name) -Force
    }

    $importStdout = Join-Path $stageFull "import.stdout.log"
    $importStderr = Join-Path $stageFull "import.stderr.log"
    $importArgs = @(
        "--headless", "--path", $stageFull,
        "--log-file", (Join-Path $stageFull "import.megadot.log"),
        "--import", "--quit-after", "600"
    )
    $importExitCode = Invoke-HiddenProcess -FilePath $megaDotConsole -Arguments $importArgs `
        -StdoutPath $importStdout -StderrPath $importStderr
    Copy-Item -LiteralPath $importStdout -Destination (Join-Path $Output "import.stdout.log") -Force -ErrorAction SilentlyContinue
    Copy-Item -LiteralPath $importStderr -Destination (Join-Path $Output "import.stderr.log") -Force -ErrorAction SilentlyContinue
    if ($importExitCode -ne 0 -or -not (Test-Path -LiteralPath (Join-Path $stageFull "chr.fbx.import"))) {
        $details = Get-Content -LiteralPath $importStderr -Raw -ErrorAction SilentlyContinue
        throw "MegaDot no pudo importar el FBX de $ModelId (exit $importExitCode). $details"
    }

    $clipArg = "--debug-clips=$($Clips -join ',')"
    $passes = if ($Mode -eq "render") {
        if ($SkipMeasure) { @("save") } else { @("measure", "save") }
    } else {
        @($Mode)
    }
    $lastStdout = ""
    foreach ($pass in $passes) {
        $logStem = if ($Mode -eq "render") { "render-$pass" } else { "probe" }
        $stdout = Join-Path $stageFull "$logStem.stdout.log"
        $stderr = Join-Path $stageFull "$logStem.stderr.log"
        $engineLog = Join-Path $stageFull "$logStem.megadot.log"
        $megaDotArgs = @(
            "--path", $stageFull,
            "--log-file", $engineLog,
            "--quit-after", "3600", "--",
            "--pass=$pass", $clipArg, "--debug-from=$From", "--debug-to=$To", "--debug-step=$Step"
        )
        if ($ForceRetarget) { $megaDotArgs += "--force-retarget" }
        $probeExitCode = Invoke-HiddenProcess -FilePath $megaDot -Arguments $megaDotArgs `
            -StdoutPath $stdout -StderrPath $stderr
        Copy-Item -LiteralPath $stdout -Destination (Join-Path $Output "$logStem.stdout.log") -Force -ErrorAction SilentlyContinue
        Copy-Item -LiteralPath $stderr -Destination (Join-Path $Output "$logStem.stderr.log") -Force -ErrorAction SilentlyContinue
        Copy-Item -LiteralPath $engineLog -Destination (Join-Path $Output "$logStem.megadot.log") -Force -ErrorAction SilentlyContinue
        if ($probeExitCode -ne 0) {
            $details = Get-Content -LiteralPath $stderr -Raw -ErrorAction SilentlyContinue
            throw "MegaDot no pudo completar $pass para $ModelId (exit $probeExitCode). $details"
        }
        $lastStdout = $stdout
    }

    if ($Mode -eq "list") {
        Get-Content -LiteralPath $lastStdout
        Write-Output "PROBE_OUTPUT=$([IO.Path]::GetFullPath($Output))"
        return
    }

    if ($Mode -eq "render") {
        $framesDir = Join-Path $stageFull "frames"
        $renders = Get-ChildItem -LiteralPath $framesDir -Filter "*.webp" -File -Recurse -ErrorAction SilentlyContinue
        if ($renders.Count -eq 0) { throw "El render de produccion no produjo fotogramas." }
        Copy-Item -LiteralPath $framesDir -Destination $Output -Recurse -Force
        $cropFile = Join-Path $stageFull "crop_union.txt"
        if (Test-Path -LiteralPath $cropFile) {
            Copy-Item -LiteralPath $cropFile -Destination (Join-Path $Output "crop_union.txt") -Force
        }
        Write-Output "RENDER_OUTPUT=$([IO.Path]::GetFullPath((Join-Path $Output 'frames')))"
        Write-Output "RENDER_FRAMES=$($renders.Count)"
        return
    }

    $renders = Get-ChildItem -LiteralPath $stageFull -Filter "debug_*.webp" -File
    if ($renders.Count -eq 0) { throw "El probe no produjo fotogramas." }
    foreach ($render in $renders) {
        Copy-Item -LiteralPath $render.FullName -Destination (Join-Path $Output $render.Name) -Force
    }
    Write-Output "PROBE_OUTPUT=$([IO.Path]::GetFullPath($Output))"
    Write-Output "PROBE_FRAMES=$($renders.Count)"
}
finally {
    if (Test-Path -LiteralPath $stageFull) {
        $verified = [IO.Path]::GetFullPath($stageFull)
        if ($verified.StartsWith($safeStagePrefix, [StringComparison]::OrdinalIgnoreCase)) {
            Remove-Item -LiteralPath $verified -Recurse -Force
        }
    }
}
