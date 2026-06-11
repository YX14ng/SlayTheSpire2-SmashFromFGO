# Scaffold del mod-librería FGOCore: estructura, props compartidos, assets migrados
# desde MashShielder (arte de memes, iconos de powers compartidos) y localización
# re-prefijada MASHSHIELDER- -> FGOCORE-.
$ErrorActionPreference = "Stop"
$u8 = New-Object System.Text.UTF8Encoding($false)
$repo = "f:\Programs\SlayTheSpire2-SmashFromFGO"
$mash = "$repo\MashShielder"
$core = "$repo\FGOCore"

# --- directorios ---
$dirs = @(
    "$core", "$core\FGOCoreCode", "$core\FGOCoreCode\Extensions",
    "$core\FGOCoreCode\Np", "$core\FGOCoreCode\Block", "$core\FGOCoreCode\Forms",
    "$core\FGOCoreCode\Bond", "$core\FGOCoreCode\Memes",
    "$core\FGOCore", "$core\FGOCore\images\card_portraits\big",
    "$core\FGOCore\images\powers\big",
    "$core\FGOCore\localization\eng", "$core\FGOCore\localization\esp", "$core\FGOCore\localization\zhs"
)
foreach ($d in $dirs) { New-Item -ItemType Directory -Force $d | Out-Null }

# --- props compartidos y export preset (idénticos) ---
Copy-Item "$mash\Directory.Build.props" "$core\" -Force
Copy-Item "$mash\Sts2PathDiscovery.props" "$core\" -Force
Copy-Item "$mash\export_presets.cfg" "$core\" -Force
Copy-Item "$mash\.gitattributes" "$core\" -Force -ErrorAction SilentlyContinue
if (Test-Path "$mash\MashShielder\mod_image.png") { Copy-Item "$mash\MashShielder\mod_image.png" "$core\FGOCore\" -Force }

# --- arte de cartas meme ---
$memes = @("golden_apple", "mapo_tofu", "insufficient_qp", "ten_pull_summon", "black_keys", "exp_ember", "palingenesis", "card")
foreach ($m in $memes) {
    foreach ($sub in @("", "big\")) {
        $src = "$mash\MashShielder\images\card_portraits\$sub$m.png"
        if (Test-Path $src) { Copy-Item $src "$core\FGOCore\images\card_portraits\$sub" -Force } else { "FALTA arte: $sub$m" }
    }
}

# --- iconos de powers compartidos ---
foreach ($p in @("np_charge_power", "bulwark_power", "form_shifted_power", "power")) {
    foreach ($sub in @("", "big\")) {
        $src = "$mash\MashShielder\images\powers\$sub$p.png"
        if (Test-Path $src) { Copy-Item $src "$core\FGOCore\images\powers\$sub" -Force } else { "FALTA icono: $sub$p" }
    }
}
# BondPower: el corazón del medidor de vínculo
Copy-Item "$mash\MashShielder\images\relics\mash_bond.png" "$core\FGOCore\images\powers\bond_power.png" -Force
Copy-Item "$mash\MashShielder\images\relics\big\mash_bond.png" "$core\FGOCore\images\powers\big\bond_power.png" -Force

# --- localización: extraer claves migradas y re-prefijar ---
$memeCards = @("GOLDEN_APPLE", "MAPO_TOFU", "INSUFFICIENT_QP", "TEN_PULL_SUMMON", "BLACK_KEYS", "EXP_EMBER", "PALINGENESIS")
$powers = @("NP_CHARGE_POWER", "BULWARK_POWER", "BOND_POWER", "FORM_SHIFTED_POWER")
foreach ($lang in @("eng", "esp", "zhs")) {
    foreach ($pair in @(@("cards", $memeCards), @("powers", $powers))) {
        $file = $pair[0]; $names = $pair[1]
        $src = Get-Content "$mash\MashShielder\localization\$lang\$file.json" -Raw -Encoding UTF8 | ConvertFrom-Json
        $out = [ordered]@{}
        foreach ($prop in $src.PSObject.Properties) {
            foreach ($n in $names) {
                if ($prop.Name -like "MASHSHIELDER-$n.*") {
                    $out[$prop.Name.Replace("MASHSHIELDER-", "FGOCORE-")] = $prop.Value
                }
            }
        }
        $jsonOut = ($out.GetEnumerator() | ForEach-Object { '  "' + $_.Key + '": ' + ($_.Value | ConvertTo-Json) }) -join ",`n"
        [IO.File]::WriteAllText("$core\FGOCore\localization\$lang\$file.json", "{`n$jsonOut`n}`n", $u8)
        "loc $lang/$file : $($out.Count) claves"
    }
}
"SCAFFOLD LISTO"
