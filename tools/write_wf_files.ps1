# Escribe los archivos {path,content} que devuelve un workflow de implementacion.
# Uso: .\tools\write_wf_files.ps1 -OutputFile <ruta del .output del task>
param([Parameter(Mandatory)][string]$OutputFile)
$raw = [System.IO.File]::ReadAllText($OutputFile, [System.Text.Encoding]::UTF8)
$data = $raw | ConvertFrom-Json
$files = if ($data.result -and $data.result.files) { $data.result.files } elseif ($data.files) { $data.files } else { @() }
$utf8 = New-Object System.Text.UTF8Encoding $false
$n = 0
foreach ($f in $files) {
  if (-not $f.path) { continue }
  $dir = Split-Path $f.path -Parent
  if ($dir -and -not (Test-Path $dir)) { New-Item -ItemType Directory -Force $dir | Out-Null }
  [System.IO.File]::WriteAllText($f.path, [string]$f.content, $utf8)
  $n++
}
Write-Output "Escritos $n archivos desde $OutputFile"