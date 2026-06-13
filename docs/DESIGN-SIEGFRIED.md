# DESIGN-SIEGFRIED — diseño del personaje (Saber, FGO) para StS2

Consolidado del veredicto del panel de jueces (`assets/reference/new_chars/siegfried_verdict.json`):
**GANA la propuesta A** (8.4) con los parches **P1-P10** aplicados y **4 injertos de B** (P6-P9).
Kit/assets en `siegfried_kit.json` / `siegfried_assets.json`. Diseñado con `sts2-mechanics-design`.
Manifest id **`SiegfriedSaber`** (inmutable). Lore investigado JP+中文 (regla WORKFLOW-FGO §2).

## 1. Identidad en una frase
**El héroe que sólo cae si le exponen la espalda: una armadura que anula casi todo (Sangre de Dragón)
pero deja pasar UN golpe por turno (la Hoja de Tilo), y un loop AoE de Balmung que se afila cuanto
más gruesa lleva la armadura.** Farmer/bruiser Buster anti-todo, taciturno, sin Resistencia Mágica.

## 2. Sin formas (justificado)
Modelo de batalla ÚNICO `100800` para asc 1-4 (100810/100820 = 404; sin swap visual). Una "forma"
sin cambio de modelo violaría §5 de la skill → **NO hay FormPower**. El traje de verano `100830`
(Super Cool Biz) queda como **skin cosmética opcional vía ModConfig** (no infraestructura FormVisuals).

## 3. Mecánica central — Sangre de Dragón (SdD) + la Hoja de Tilo
- **Sangre de Dragón (SdD)** = poder-contador NUEVO (FGOCore): **reduce cada golpe entrante en SdD**
  (escamas; reducción por-golpe, NO bloqueo que se gasta). Empieza el combate en **2** (starter).
  Persiste entre turnos (es su piel, no bloqueo). Es la identidad tanque-bruiser END A.
- **La Hoja de Tilo** (starter relic): el **primer golpe que te ALCANZA cada turno IGNORA la SdD**
  (el punto débil de la espalda) — la debilidad canónica hecha regla. Orden de hooks: **post-Bloqueo,
  pre-Guts** ("primer golpe que te alcanza", no el primero absoluto).
- **Anti-batería-AFK (P2, crítico):** el +NP del proc exige que **el golpe reducido aún inflija ≥1 de
  daño** tras la SdD. Un golpe ANULADO del todo = la armadura trabajó gratis → **sin NP** (el jugador
  empuja, no acampa). La inmunidad estilo Intangible vs golpes chicos queda (análogo vanilla), pero
  deja de ser NP-positiva.
- **Presupuesto AGREGADO del trigger defensivo (P3):** *ningún trigger defensivo paga más de 2 monedas
  a la vez* (lección P1 Mash en agregado). Caps por fuente Y tope agregado.

## 4. NP — Balmung: Desatado (carta-NP)
- AoE Buster, `ConsumeAllForNpCard` (FGOCore). Daño base alto a TODOS.
- **Overcharge fiel (P6, injerto B):** `+1 por cada 10 sobre el mínimo; +2 en su lugar si tu SdD ≥3`.
  Es el supereffective anti-dragón real leído como *"la sangre del dragón afila el filo"* (él ES el
  dragón) — sin importar Marca ni segundo contador. **Glow dorado cuando SdD ≥3.** Ej. a 300 con SdD≥3:
  ~64 AoE; sin SdD: ~44 → el escalado pleno se GANA manteniendo la armadura (la lente del diseño).
- **Refund como ÚNICO rider plano (P1 Morgan):** la EX da +20 NP a sí mismo (Rank-Up, ver §6). NADA de
  riders por frecuencia (no Marca-que-dobla-la-siguiente: ese loop auto-alimentado de B se descartó).
- **Doble-refund como CÓDIGO (P5):** flag mod-local `_npRefundedThisTurn`; si ya se resolvió una carta-NP
  este turno, todo refund posterior (NP auto, EX manual) paga **+10 en vez de +20**. Dos ults/turno
  posibles (all-in legítimo) pero a coste neto ≥50.

## 5. HP, mazo inicial (7 básicas)
**HP = 80** (ancla Ironclad exacta — P1; NO 82). La identidad bruiser ya la paga la SdD inicial 2 +
`BlockMultiplier ×1.25` del BondRelic. Mazo inicial: QAABB fiel — 4-5 Golpe Buster + Defensa(s) +
firmas; **daño ≥49/ciclo** (umbral paridad Jeanne, P5/P7 Mash) → apuntar a **~52/ciclo**. El "Golpe"
va DENTRO del mazo (no fantasma — P6 Morgan, compat del tag Strike).

## 6. Rank-Up-as-upgrade (la innovación de fidelidad de A)
Los **3 strengthenings reales** son **upgrades de carta** (no cartas separadas):
- **Regla de Oro C− → Avaricia Dorada A**: base = NP-gain-up (regla-Artoria: carga plana 1:1 documentada);
  up = +carga +ATK-up.
- **Cazador A → A++**: base = +daño vs [Dragón] / supereffective; up = +Buster 1 turno (el turno del NP).
- **Balmung A+ → EX**: up = +daño NP + el refund +20 (ver §4). NO hornear el refund en la base (se gana con el up).

## 7. Cartas clave (números patcheados — A + injertos B)
| Carta | Rareza | ⚡ | Efecto (tentativo, tunable) |
|---|---|---|---|
| **Cicatriz del Tilo** (P7) | PC | 1 (Poder) | Cada vez que la Hoja de Tilo deja pasar un golpe: +1 SdD y +10 NP (up +15). La debilidad como MOTOR. |
| **Espalda Expuesta** (P7) | PC | 1 (At) | 14 daño (up 18); este turno tu SdD NO reduce ningún golpe (espejo riesgoso). |
| **El Peso de las Expectativas** (P7) | PC | 1 (Poder) | Al fin de tu turno, si NO jugaste Ataques: +20 NP y +1 SdD (auto-limitado: la ult ES Ataque → no proca en turnos de ult). máx 2/turno (P3). |
| **Por Convicción Propia** (P7) | Rara | 1, Exhaust | ELEGÍ una: +50 NP / +3 SdD / tu próximo Ataque Cazadragones ×2 (máx +12). up: elegí DOS. (Su sueño hecho carta de decisión.) |
| **Verdugo de Fafnir** | PC | 2 | daño con escalado acotado al techo ×1.5 (≈"+1 por cada 4 de X, máx +6" — NO 30 plano repetible; lección TyrantsSweep P3 Morgan). |
| **Última Voluntad** | Rara | 1, Exhaust | +50 NP; up: 0⚡ +100 (P5 — NO 0⚡/+100 de base). |
| **Acecho de Grani** | básica/PC | 0 | +10 Estrellas (up +20) — UN solo rider, sin +NP (P10). |

## 8. Reliquias (SIN multiplicador global)
1. **STARTER — La Hoja de Tilo**: SdD inicial 2; el 1er golpe que te alcanza/turno ignora la SdD; cuando
   un golpe reducido aún inflige ≥1 daño: +5 NP (cap 3/turno — P1/P2). Doccomment: orden de hooks (post-Bloqueo, pre-Guts).
2. **Das Rheingold** (Bond CE, **deduplicada P4**): *la PRIMERA carta-NP de cada turno: +20 Estrellas y robá 1*
   (cap estructural + el robo que el pool no tiene). NO clonar "Oro del Rin" (P4: evitar el dúo +40-50★/NP que rompía el loop).
3. **Reliquia de JEFE**: sube SOLO la conversión Vida→NP (+20×2), dejando la absorción en +5×3 (P3 — techo pasivo combinado ~70 NP/turno solo con jefe+rara+multi-hit, documentado como caso máximo).

## 9. Identidad NEGATIVA — sin Resistencia Mágica (P8, regla auditable)
Siegfried NO tiene Resistencia Mágica (rarísimo en Saber). Regla de pool **escrita y auditable**:
- **CERO `Artifact` en el pool propio**; ningún cleanse de debuffs salvo **Retirada Estratégica**
  (Exhaust = cooldown) y **Corona del Sin Par** (3⚡, el sobrecosto ES la trivia/identidad).
- Es vulnerable a Maldición/debuffs por diseño (sinergia/anti-sinergia con el ecosistema FGO).

## 10. FGOCore: reusa vs NUEVO
- **Reusa:** `NpCharge`/`ConsumeAllForNpCard`+`NpChargePower`, `NpLevels`+`INpLevelStore` (dupes),
  `OverchargeBlessingPower`, `CritStarsPower`/`CritReadyPower` (Estrellas/Das Rheingold), `GutsPower`,
  `BondRelic` (×1.25 daño/bloqueo, SIN ×global), `ILimitBreaker` (Santo Grial), keyword `Cazadragones`.
- **NUEVO (acotado, 1 mecánica + su excepción narrativa):** `DragonScalesPower` (SdD = reducción
  por-golpe, persistente) + el gancho de "primer golpe que te alcanza ignora SdD" para la Hoja de Tilo
  (interfaz-gancho estilo `ICurseAmplifier`, en FGOCore, sin que FGOCore conozca el mod). El proc de +NP
  exige daño residual ≥1 (P2). Keyword `Cazadragones` (trait dragón).

## 11. Assets (verificados HTTP 200)
- Modelo de batalla: `100800` → `static.atlasacademy.io/JP/Servants/100800/100800` (7.5 MB UnityFS) +
  textura `/textures/100800.png`. Charagraphs `100800a@1/a@2/b@1/b@2`. Costume `100830` (skin opcional).
- Descargar con `tools/fetch_fgo_bundle.ps1 -Ids 100800 -Texture` → GUI export (docs/ANIMATIONS.md §1) →
  `tools/render_all_<...>.ps1` análogo. Modelo único → render directo (sin multi-atlas ni superGiant esperados; verificar en `list`).
- Select-bg: charagraph oficial `100800b@2` (asc final). Card-NP art: `Commands/100800/card_servant_np.png`.

## 12. Checklist de implementación
**FGOCore primero:** `DragonScalesPower` + interfaz-gancho de la Hoja de Tilo + keyword `Cazadragones`.
**Mod SiegfriedSaber:** scaffold → HP 80, mazo QAABB ≥52/ciclo → starter Hoja de Tilo (+ Das Rheingold + jefe)
→ NP Balmung (`ConsumeAllForNpCard` + overcharge P6 + refund-flag P5) → 3 upgrades Rank-Up → cartas P7 →
regla negativa P8 (sin Artifact). Loc eng/esp/**zhs** (P9, fijar ANTES de IDs):
龙血护甲 (Sangre de Dragón) · 菩提叶之弱点 (Hoja de Tilo) · 屠龙 (Cazadragones, keyword) · 幻想大剑·天魔失坠 (Balmung).
Frases desde JP+中文 (VOICE-LINES.md). `audit_simpleloc.ps1` antes de publicar.

## 13. Perillas de playtest (en orden — NUNCA el daño base de la ult, P10)
1. Bautismo de Fafnir (conversión Vida→NP del starter/jefe). 2. Refund 20→10. 3. PerTen del overcharge 2→1.
Riesgos cerrados por código (no "vigilar"): batería AFK (P2), doble-refund (P5 flag), trigger agregado (P3), dedup Das Rheingold (P4).
