# Analiza la salida del pase "probe" del renderer: ventana minima con 95% del
# movimiento + curva de movimiento por frame, por modelo/clip.
param([Parameter(Mandatory)][string]$ProbeOutput)
$lines = Get-Content $ProbeOutput
$model = ""
$data = [ordered]@{}
foreach ($l in $lines) {
    if ($l -match '=== PROBE (\d+)') { $model = $Matches[1]; continue }
    if ($l -match '^MOTION (\S+) (\d+) ([\d.]+)') {
        $key = "$model/$($Matches[1])"
        if (-not $data.Contains($key)) { $data[$key] = New-Object 'System.Collections.Generic.List[double]' }
        $data[$key].Add([double]$Matches[3])
    }
}
foreach ($key in $data.Keys) {
    $v = $data[$key]
    $tot = ($v | Measure-Object -Sum).Sum
    $best = $null
    for ($a = 0; $a -lt $v.Count; $a++) {
        $acc = 0.0
        for ($b = $a; $b -lt $v.Count; $b++) {
            $acc += $v[$b]
            if ($acc -ge $tot * 0.95) {
                if ($null -eq $best -or ($b - $a) -lt ($best[1] - $best[0])) { $best = @($a, $b) }
                break
            }
        }
    }
    Write-Output "== $key  ventana95=[$($best[0]),$($best[1])]"
    for ($i = 0; $i -lt $v.Count; $i += 10) {
        $hi = [Math]::Min($i + 9, $v.Count - 1)
        $row = ($v[$i..$hi] | ForEach-Object { "{0,5:F2}" -f $_ }) -join " "
        Write-Output ("  {0,3}: {1}" -f $i, $row)
    }
}
