# Corrige strings de localizacion que el SimpleLoc de BaseLib mangla:
# +...+ y -...- literales se emparejan como sintaxis de upgrade-swap, y un
# parentesis tras una variable dispara el pluralize. Escapes: /+ /- /(
# Tambien generaliza los textos de FGOCore que decian "Mash" hardcodeado.
$utf8 = New-Object System.Text.UTF8Encoding($false)
$root = "f:\Programs\SlayTheSpire2-SmashFromFGO"

$fixes = @(
    # --- Mash: cards eng ---
    @{ f = "MashShielder\MashShielder\localization\eng\cards.json"; pairs = @(
        @("Consume ALL your Block: +1 damage", "Consume ALL your Block: /+1 damage"),
        @("*NP *Charge (without spending it)", "*NP *Charge /(without spending it)")) },
    # --- Mash: cards esp ---
    @{ f = "MashShielder\MashShielder\localization\esp\cards.json"; pairs = @(
        @("Consume TODO tu Bloqueo: +1 de da", "Consume TODO tu Bloqueo: /+1 de da"),
        @("*Carga *NP (sin gastarla)", "*Carga *NP /(sin gastarla)")) },
    # --- Mash: cards zhs ---
    # OJO: con UN solo par, @(@(...)) se APLANA en PowerShell y el foreach itera
    # strings ($pair[0] = primer caracter -> corrupcion). Coma unaria obligatoria.
    @{ f = "MashShielder\MashShielder\localization\zhs\cards.json"; pairs = @(
        , @("额外+1（最多+!MaxBonus!）", "额外/+1（最多/+!MaxBonus!）")) },
    # --- Mash: ancients eng ---
    @{ f = "MashShielder\MashShielder\localization\eng\ancients.json"; pairs = @(
        , @("A shield-bearer from another saga. Your walls will be tested here, Demi-Servant.",
            "A shield/-bearer from another saga. Your walls will be tested here, Demi/-Servant.")) },
    # --- Mash: relics (vinculo) ---
    @{ f = "MashShielder\MashShielder\localization\eng\relics.json"; pairs = @(
        @("(+2, Elites +3, Bosses +5)", "(/+2, Elites /+3, Bosses /+5)"),
        @("rest sites (+1)", "rest sites (/+1)")) },
    @{ f = "MashShielder\MashShielder\localization\esp\relics.json"; pairs = @(
        @("(+2, élites +3, jefes +5)", "(/+2, élites /+3, jefes /+5)"),
        @("fogatas (+1)", "fogatas (/+1)")) },
    @{ f = "MashShielder\MashShielder\localization\zhs\relics.json"; pairs = @(
        @("（+2，精英+3，头目+5）", "（/+2，精英/+3，头目/+5）"),
        @("篝火时+1。", "篝火时/+1。")) },
    # --- Morgan: cards eng ---
    @{ f = "MorganBerserker\MorganBerserker\localization\eng\cards.json"; pairs = @(
        @(", +!Bonus! if your HP is at 75% or less, and +!Bonus! more", ", /+!Bonus! if your HP is at 75% or less, and /+!Bonus! more"),
        @("*NP *Charge (up to 2 times per turn)", "*NP *Charge /(up to 2 times per turn)"),
        @(", +!PerTen! per 10 Charge", ", /+!PerTen! per 10 Charge"),
        @(", +1 if your Charge reaches 100", ", /+1 if your Charge reaches 100"),
        @(", +1 if your Charge reaches 200", ", /+1 if your Charge reaches 200")) },
    # --- Morgan: cards esp ---
    @{ f = "MorganBerserker\MorganBerserker\localization\esp\cards.json"; pairs = @(
        @(", +!Bonus! si tu Vida está al 75% o menos, y +!Bonus! más", ", /+!Bonus! si tu Vida está al 75% o menos, y /+!Bonus! más"),
        @("*Carga *NP (máx. 2 veces por turno)", "*Carga *NP /(máx. 2 veces por turno)"),
        @(", +!PerTen! por cada 10 de Carga", ", /+!PerTen! por cada 10 de Carga"),
        @(", +1 si tu Carga llega a 100", ", /+1 si tu Carga llega a 100"),
        @(", +1 si tu Carga llega a 200", ", /+1 si tu Carga llega a 200")) },
    # --- Morgan: cards zhs ---
    @{ f = "MorganBerserker\MorganBerserker\localization\zhs\cards.json"; pairs = @(
        @("额外+!Bonus!，不高于50%时再+!Bonus!", "额外/+!Bonus!，不高于50%时再/+!Bonus!"),
        @("额外+!PerTen!", "额外/+!PerTen!"),
        @("消耗满100时再+1张", "消耗满100时再/+1张"),
        @("消耗满200时再+1张", "消耗满200时再/+1张")) },
    # --- Morgan: relics (vinculo) ---
    @{ f = "MorganBerserker\MorganBerserker\localization\eng\relics.json"; pairs = @(
        @("(+2, Elites +3, Bosses +5)", "(/+2, Elites /+3, Bosses /+5)"),
        @("rest sites (+1)", "rest sites (/+1)")) },
    @{ f = "MorganBerserker\MorganBerserker\localization\esp\relics.json"; pairs = @(
        @("(+2, élites +3, jefes +5)", "(/+2, élites /+3, jefes /+5)"),
        @("fogatas (+1)", "fogatas (/+1)")) },
    @{ f = "MorganBerserker\MorganBerserker\localization\zhs\relics.json"; pairs = @(
        @("（+2，精英+3，头目+5）", "（/+2，精英/+3，头目/+5）"),
        @("篝火时+1。", "篝火时/+1。")) },
    # --- FGOCore: powers genericos (decian "Mash" para todos los personajes) ---
    @{ f = "FGOCore\FGOCore\localization\eng\powers.json"; pairs = @(
        @("Mash's Bond level this run. Grants bonuses at the start of each combat (see the Bond Gauge relic).",
          "Bond level this run. Grants bonuses at the start of each combat (see the Bond relic)."),
        @("Mash changed form this combat.", "This character changed form this combat.")) },
    @{ f = "FGOCore\FGOCore\localization\esp\powers.json"; pairs = @(
        @("Nivel de Vínculo de Mash en esta partida. Otorga bonos al inicio de cada combate (ver la reliquia Medidor de Vínculo).",
          "Nivel de Vínculo en esta partida. Otorga bonos al inicio de cada combate (ver la reliquia de Vínculo)."),
        @("Mash cambió de forma este combate.", "Este personaje cambió de forma este combate.")) },
    @{ f = "FGOCore\FGOCore\localization\zhs\powers.json"; pairs = @(
        @("玛修本局游戏的羁绊等级。每场战斗开始时提供加成（详见羁绊计量器遗物）。",
          "本局游戏的羁绊等级。每场战斗开始时提供加成（详见羁绊遗物）。"),
        @("玛修本场战斗变换过形态。", "该角色本场战斗变换过形态。")) }
)

foreach ($fx in $fixes) {
    $p = Join-Path $root $fx.f
    $t = [IO.File]::ReadAllText($p)
    foreach ($pair in $fx.pairs) {
        $old = $pair[0]; $new = $pair[1]
        $n = ([regex]::Matches($t, [regex]::Escape($old))).Count
        if ($n -eq 0) { Write-Output "SIN MATCH: $($fx.f) :: $old"; continue }
        $t = $t.Replace($old, $new)
        Write-Output "ok ($n): $($fx.f) :: $old"
    }
    [IO.File]::WriteAllText($p, $t, $utf8)
}
Write-Output "--- re-audit ---"
& "$root\tools\audit_simpleloc.ps1"