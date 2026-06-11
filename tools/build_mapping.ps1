# Extrae los matches del output del workflow y arma el CSV file,assetId
param(
    [string]$WorkflowOutput = "C:\Users\YX14n\AppData\Local\Temp\claude\f--Programs-SlayTheSpire2-SmashFromFGO\7f54f922-c0e0-4a93-a0c8-17cb7f741072\tasks\wftrc1kyu.output",
    [string]$Tsv = "f:\Programs\SlayTheSpire2-SmashFromFGO\assets\reference\ce\ce_names.tsv",
    [string]$OutCsv = "f:\Programs\SlayTheSpire2-SmashFromFGO\assets\reference\ce\mapping.csv"
)
$data = Get-Content $WorkflowOutput -Raw | ConvertFrom-Json
$found = $data.result.matches
Write-Output "matches: $($found.Count)"

$map = @{}
foreach ($line in (Get-Content $Tsv)) {
    $p = $line -split "`t"
    if ($p.Count -ge 2) { $map[$p[0]] = $p[1] }
}

$rows = @("file,assetId")
$missing = @()
$dupes = @()
$seen = @{}
foreach ($x in $found) {
    if ($x.duplicate) { $dupes += $x.file }
    if ($seen.ContainsKey($x.file)) { continue }
    $seen[$x.file] = $true
    $cn = "$($x.collectionNo)"
    if ($map.ContainsKey($cn)) { $rows += "$($x.file),$($map[$cn])" }
    else { $missing += "$($x.file) (CE $cn)" }
}
[IO.File]::WriteAllLines($OutCsv, $rows)
Write-Output "filas CSV: $($rows.Count - 1)"
if ($missing.Count -gt 0) { Write-Output "SIN MAPEO: $($missing -join ', ')" }
if ($dupes.Count -gt 0) { Write-Output "DUPLICADOS SIN RESOLVER: $($dupes -join ', ')" }
