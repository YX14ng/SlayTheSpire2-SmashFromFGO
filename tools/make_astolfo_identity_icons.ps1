param([string]$Root = 'F:\Programs\SlayTheSpire2-SmashFromFGO')

Add-Type -AssemblyName System.Drawing
$ErrorActionPreference = 'Stop'

$art = Join-Path $Root 'assets\reference\ce\art'
$mod = Join-Path $Root 'AstolfoRider\AstolfoRider'
$powers = Join-Path $mod 'images\powers'
$relics = Join-Path $mod 'images\relics'
New-Item -ItemType Directory -Force $powers,"$powers\big",$relics,"$relics\big" | Out-Null

$sources = @{
    face = Join-Path $art 'command_400400.png'
    mount = Join-Path $art '9300860.png'
    plush = Join-Path $art '9805510.png'
    stars = Join-Path $art '9805520.png'
    trifas = Join-Path $art '9403170.png'
    adventure = Join-Path $art '9305750.png'
    reason = Join-Path $art '9308370.png'
}
foreach ($source in $sources.Values) {
    if (-not (Test-Path $source)) { throw "Falta recurso oficial: $source" }
}

function Select-Source([string]$name) {
    if ($name -match 'hippogriff|evasion|impossible|world_reverse|riding|gallop|achilles') { return $sources.mount }
    if ($name -match 'star|quick|critical|feather|scale') { return $sources.stars }
    if ($name -match 'trifas|argalia|paladin|shield|block') { return $sources.trifas }
    if ($name -match 'adventure|humor|improvisation|luck|good_deeds|oath|chalice') { return $sources.adventure }
    if ($name -match 'reason|manual|luna|caprice|usage|controller|forgotten') { return $sources.reason }
    if ($name -match 'borrowed') { return $sources.plush }
    return $sources.face
}

function Render-Circle([string]$sourcePath, [string]$outputPath, [int]$size = 256) {
    $source = [System.Drawing.Bitmap]::FromFile($sourcePath)
    try {
        $side = [Math]::Min($source.Width, $source.Height)
        $sourceRect = New-Object System.Drawing.Rectangle(
            [int](($source.Width - $side) / 2), [int](($source.Height - $side) * 0.22), $side, $side)
        $output = New-Object System.Drawing.Bitmap($size, $size, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
        $graphics = [System.Drawing.Graphics]::FromImage($output)
        try {
            $graphics.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
            $graphics.SmoothingMode = [System.Drawing.Drawing2D.SmoothingMode]::AntiAlias
            $clip = New-Object System.Drawing.Drawing2D.GraphicsPath
            $clip.AddEllipse(3, 3, $size - 6, $size - 6)
            $graphics.SetClip($clip)
            $graphics.DrawImage($source,
                (New-Object System.Drawing.Rectangle(0, 0, $size, $size)),
                $sourceRect, [System.Drawing.GraphicsUnit]::Pixel)
            $clip.Dispose()
        } finally { $graphics.Dispose() }
        $output.Save($outputPath, [System.Drawing.Imaging.ImageFormat]::Png)
        $output.Dispose()
    } finally { $source.Dispose() }
}

function Render-Outline([string]$sourcePath, [string]$outputPath) {
    $source = [System.Drawing.Bitmap]::FromFile($sourcePath)
    try {
        $output = New-Object System.Drawing.Bitmap($source.Width, $source.Height, [System.Drawing.Imaging.PixelFormat]::Format32bppArgb)
        for ($y = 0; $y -lt $source.Height; $y++) {
            for ($x = 0; $x -lt $source.Width; $x++) {
                $alpha = $source.GetPixel($x, $y).A
                if ($alpha -gt 16) {
                    $output.SetPixel($x, $y, [System.Drawing.Color]::FromArgb($alpha, 255, 255, 255))
                }
            }
        }
        $output.Save($outputPath, [System.Drawing.Imaging.ImageFormat]::Png)
        $output.Dispose()
    } finally { $source.Dispose() }
}

function Model-Names([string]$file) {
    $json = Get-Content $file -Raw -Encoding UTF8 | ConvertFrom-Json
    @($json.PSObject.Properties | Where-Object Name -like '*.title' | ForEach-Object {
        ($_.Name -replace '^ASTOLFORIDER-','' -replace '\.title$','').ToLowerInvariant()
    })
}

foreach ($name in Model-Names (Join-Path $mod 'localization\eng\powers.json')) {
    $output = Join-Path $powers "$name.png"
    Render-Circle (Select-Source $name) $output
    Copy-Item $output (Join-Path $powers "big\$name.png") -Force
}

foreach ($name in Model-Names (Join-Path $mod 'localization\eng\relics.json')) {
    $output = Join-Path $relics "$name.png"
    Render-Circle (Select-Source $name) $output
    Copy-Item $output (Join-Path $relics "big\$name.png") -Force
    Render-Outline $output (Join-Path $relics "${name}_outline.png")
}

Write-Output "Astolfo identity icons: listos"
