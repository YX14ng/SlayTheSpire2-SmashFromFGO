# DESIGN-MORDRED — diseño del personaje (Saber of Red, FGO) para StS2

Consolidado de `mordred_kit.json` / `mordred_design_b.json` / `mordred_assets.json` (solo existe `design_b`; este doc lo **finaliza** y le agrega el eje de FORMAS como arquetipo drafteable). Diseñado con la skill `sts2-mechanics-design`. Manifest id **`MordredSaber`** (inmutable). Lore investigado JP+中文 (regla WORKFLOW-FGO §2). Motor: **crit Buster + NP Clarent Blood Arthur (anti-padre)**. Servant id **100900**, collectionNo 76.

---

## 1. Identidad en una frase

**El Caballero de la Traición: tanquea su rabia tras el Yelmo de la Infidelidad (forma defensiva que sella sus parámetros y banca recursos), se lo arranca para cobrarla en CRÍTICOS de relámpago rojo con Clarent, y la corona con Clarent Blood Arthur — el NP que la desenmascara al jugarse y pega más fuerte cuanto más alta es la autoridad del rival (Élites/Jefes = el trait [Arthur]). Cada herida son Estrellas; cada Crítico es Carga NP: el odio se recicla en sí mismo.**

Arquetipo: **AoE Buster crit-DPS selfish** con eje de **FORMAS (casco puesto/quitado)** drafteable.

---

## 2. Inventario del kit FGO (lo que TIENE que existir)

| Pieza FGO | Efecto real | Dónde aterriza en el mod |
|---|---|---|
| **S1 Mana Burst A** → RankUp **Knight of Red Lightning A+** | Buster +50% 1t → 3t + crit-dmg +30% + ATK +20% | PC «Estallido de Maná A» (base, Exhaust=CD) → upgrade/Poder «Caballero del Relámpago Rojo A+» |
| **S2 Instinct B** → RankUp **Cigarette Lion B+** | +14★, crit-rate +20% → +★ + crit-dmg +50% + crit-rate Buster | PC «Instinto B» → Poder «León del Cigarrillo B+» (guiño a Kairi Sisigou) |
| **S3 Secret of Pedigree EX** (EL CASCO) | cleanse TODOS los debuffs + DEF +50% + carga NP +30% | PC «Secreto de Cuna EX» + **es el eje de FORMAS** (casco = Enmascarado) |
| **NP Clarent Blood Arthur** (Buster AoE 5 hits, anti-[Arthur] ×1.8, refund 10→20%) | el tajo de relámpago rojo-sangre | Cartas-NP (auto «Desatado», upgrade «Interludio», manual rara, mini-NP) |
| **Pasiva Magic Resistance B** | resist debuffs +17.5% | Reliquia PC «Amuleto de Resistencia Mágica B» (anula 1er debuff/combate) |
| **Pasiva Riding B** | Quick +8% (roba y conduce de todo) | Reliquia «Moto Roja de Trifas» + cartas «Cabalgar lo Robado» |
| **Trait Dragon (homúnculo dracónico)** | recibe daño anti-dragón | Poder PC «Sangre de Dragón» (vitalidad de homúnculo) |
| **Trait Saberface / reglas de trato** | meme-lore | Poder raro «Saberface», meme «¡No me llames niña!», PC «Trátame como caballero» |
| **Lore: Camlann, Kairi, Caliburn, el Grial** | beats narrativos | Raras «Camlann», «Sello de Comando de Kairi», «Reclamo de Caliburn», «Herencia Negada» |

**Regla NEGATIVA de pool (auditable):** Mordred SÍ tiene Resistencia Mágica B → **se permite UN cleanse** («Secreto de Cuna EX» = su casco) y una reliquia anti-debuff. No abusar: máx esos dos vectores de cleanse en todo el pool (a diferencia de Siegfried que tenía CERO, ella tiene exactamente lo que su kit justifica).

---

## 3. Recurso/motor del personaje

**Doble economía compartida de FGOCore + un trenzado propio vía starter. CERO contadores nuevos** (presupuesto §1.3: 1 mecánica propia = FORMAS; 2 keywords core = NP + Estrellas/Crítico — igual a Mash/Morgan).

1. **CARGA NP 0-300** (FGOCore `NpChargePower`): a **100** se auto-manifiesta GRATIS con Retain «Clarent Blood Arthur: Desatado» (`ConsumeAllForNpCard`, mín 100, consume TODA; el excedente hasta 300 es banco de Sobrecarga +PerTen).
2. **ESTRELLAS DE CRÍTICO** (`CritStarsPower` compartido, el GLOBAL estilo Jeanne/Morgan — **NO** el contador con candado de Artoria): a **100** se descuentan solas y dan **1 CRÍTICO LISTO** (próximo Ataque ×2, hacen cola).
3. **EL TRENZADO PROPIO (vía starter, lección 焰刑地狱):**
   - cada **pérdida de Vida → +10 Estrellas** (la rabia: sangrar carga el relámpago — calibra todos los riders ★).
   - cada **CRÍTICO LISTO consumido → +10 de Carga NP** (espejo del refund real del NP: Clarent canaliza el odio del golpe).
   - **Dirección única ★→×2→NP, sin loop**: un Crítico cuesta ~100★ (~2⚡ de generación) y devuelve 10 NP (~0,2⚡). Ninguna dirección es ⚡-positiva.
4. **RIDER TEMÁTICO «vs Élites/Jefes»** (la traducción del special anti-[Arthur] = la autoridad): no es keyword nuevo, es un **check de encuentro** con glow dorado. El multiplicador real ×1.8 solo existía contra 18 de 400+ servants → acá solo contra ~30-40% de los combates, apuntando el exceso EXACTAMENTE donde Hextech aprieta (Élites/Jefes con HP +20-40%).

**Qué lo interrumpe (plan A interrumpible, §1.bis-2):** presión que obliga a quedarse Enmascarada (los Ataques −2 apagan el plan ofensivo → bajás a tasa defensiva normal, nunca por debajo), multi-pelea sin Élite (el rider anti-autoridad duerme), y caer bajo 100 NP re-arma la ulti.
**Plan B:** el mazo pega y bloquea a tasa vanilla sin motor (Busters + Tajos + Defenders + Baluarte 10 de Enmascarado).

### 3.bis El eje de FORMAS como ARQUETIPO DRAFTEABLE (lo que este finalizado agrega)

**TRES estados = los tres looks verificados de las ascensiones** (FGOCore `FormPower`/`FormSwitch`/`FormVisuals` + `FormShiftedPower` + `IFormChangeListener`). Caso de pipeline NUEVO y más simple que Morgan: las 4 ascensiones comparten el puppet **100900** y el casco existe como pieza animable verificada en la textura → el swap visual es **attach/detach del yelmo sobre el mismo rig**, no swap de modelo.

| Forma | Lore / look | Pasivas | Decisión que cambia |
|---|---|---|---|
| **1 — CABALLERO ENMASCARADO** (inicial; Secreto de Cuna activo) ref `100900a@1` | identidad y parámetros sellados | tus Ataques hacen **−2**; al final del turno conservás hasta **10 de Bloqueo** (Baluarte/`IBlockRetentionSource`); al inicio del turno **+5 NP** | jugar Habilidades, tanquear y bancar; atacar es desperdicio |
| **2 — CABALLERO DE LA REBELIÓN** (sin casco) ref `100900b@1` | sin yelmo ni armadura (asc 3) | tus Ataques hacen **+2**; recibís **+2 de daño** por golpe enemigo | la ventana de cobro: entrar con Crítico armado, reventar, volver a ponerse el yelmo. Parkear acá es un all-in Berserker consciente (cada golpe recibido = +10★ vía starter), no un error |
| **3 — RELÁMPAGO CARMESÍ** (clímax, `IsPermanent`, vía rara «Poder Clímax») ref `100900b@2` | envuelta en relámpago carmesí (el fotograma del NP) | ambas pasivas SIN penalización (Ataques +2, retención 10, +5 NP/turno); la ulti pasa a «Interludio» | el premio de fin de run; ya no hay tensión de casco |

- **LAS DOS FIRMAS BÁSICAS SON LOS CAMBIOS DE FORMA** (precedente Watcher/Artoria). El daño de la firma ofensiva se resuelve **ANTES** del switch (no se auto-buffea). Cambios esperados/combate: **2-4**.
- **REGLA DE ORO LORE:** con el casco puesto NO puede gritar su rebelión → jugar la ulti estando Enmascarada **PRIMERO le arranca el yelmo** (entra en Rebelión). La escena de Apocrypha ante Sisigou, mecanizada. «¿enmascarada o no cruzo los 100?» es **LA decisión de timing** del personaje (cruzar enmascarada aplica el +2 de Rebelión a los 5 golpes del Desatado).
- §5 cumplido: cada forma cambia las DECISIONES (qué carta jugar), no solo números; el multiplicador real vive en cartas/poderes condicionales («en Rebelión: +X», «si Enmascarado: +Y»), no en la pasiva (números chicos 2-5).

---

## 4. Mapeo a FGOCore (reusa vs NUEVO)

**REUSA tal cual:**
- `NpChargePower` 0-300 + `ConsumeAllForNpCard` (mín+PerTen) + `OverchargeBlessingPower` + waivers anti-Event.
- `CritStarsPower`/`CritReadyPower` + helper `CritStars` (el contador GLOBAL estilo Jeanne — exactamente el que Mordred necesita, NO el de Artoria con candado).
- `FormPower`/`FormSwitch`/`FormVisuals` + `FormShiftedPower` + `IFormChangeListener` (la danza del yelmo; Morgan ya cambia forma desde cartas, incluida la carta-NP).
- `BondRelic` (×1.25 daño/bloqueo global + niveles — la palanca §1.bis, SIN ×daño nuevo).
- `INpLevelStore` + `NpLevels.Scale` + botón Invocar (dupes/NP level).
- `GutsPower` (Camlann), `BulwarkPower`/`IBlockRetentionSource` (retención de Enmascarado).
- `BusterCardBase`/`ArtsCardBase`/`QuickCardBase` (tríada de comando con loc/arte compartidos).

**FALTA CREAR (poco, casi todo mod-local):**
1. **FGOCore:** evento **«Crítico consumido»** — si `CritReadyPower` no expone hook al decrementar en `AfterDamageGiven`, agregar evento estático `CritReady.OnConsumed` (o `ICritConsumedListener`) que consumen: la starter de Mordred (+10 NP), «La Espada Más Resplandeciente», «Aceleración de Homúnculo» y los riders «si consumiste un CRÍTICO este turno» (flag de turno mod-local, patrón `_isProcessing` ya documentado). Única pieza core nueva; reutilizable por futuros chars crit (Jalter-likes).
2. **FGOCore opcional:** helper `IsEliteOrBoss(room/encounter)` para el rider anti-autoridad si no existe — check trivial de tipo de encuentro, con glow.
3. **Mod-local `MordredSaber`:** 3 form powers; starter con `AfterDamageReceived`/pérdida de Vida (patrón calcado del Cetro de Morgan); el `FormSwitch`-desde-carta-NP del Desatado.
4. **Pipeline (documentar en WORKFLOW-FGO):** swap de forma = attach/detach del casco sobre el MISMO puppet 100900 (atlas trae yelmo cornado + cabeza rubia como partes separadas), no doble modelo. Render de 2 sets de frames del mismo rig + overlay de relámpago (partículas/shader) para Relámpago Carmesí. **Fallback barato:** renderizar dos rigs completos (con/sin yelmo) del mismo bundle, solo costo de tiempo de render.

---

## 5. Pool COMPLETO de cartas

Nomenclatura: **At** = Ataque, **Hab** = Habilidad, **Poder** = Power. Glow = `ShouldGlowGoldInternal` en la condicional. Denominaciones fijas (aritmética Jeanne): NP {5/10/20/30/50/70/100/300}, Estrellas {10/20/30/50/100}.

### 5.0 Cartas de Comando (tríada FGOCore) + 4 básicas propias

| Nombre (eng / esp / zhs) | Rar. | ⚡ | Tipo | Efecto | Engranaje |
|---|---|---|---|---|---|
| **Buster / Buster / Buster卡** (`BusterMordred`) | Básica | 1 | At | 10 de daño (up +3). Marco rojo | salida simple; blanco natural del ×2 |
| **Arts / Arts / Arts卡** (`ArtsMordred`) | Básica | 1 | At | 6 de daño + **30 NP** (up +3/+20). Marco azul | motor del ciclo de ultis desde turno 1 |
| **Quick / Quick / Quick卡** (`QuickMordred`) | Básica | 1 | At | 6 de daño + **30 Estrellas** (up +3/+20). Marco verde | entrada activa de ★ (con Moto Trifas: +40) |
| **Strike / Golpe / 打击** (`GolpeMordred`) | Básica | 1 | At | 6 de daño (up +3). FUERA del mazo inicial; existe por compat del tag 'Strike' (BusterMordred hereda el tag) | — |
| **Defend / Defender / 防御** (`DefenderMordred`) | Básica | 1 | Hab | 5 de Bloqueo (up +3). En Enmascarado se conserva hasta 10 | tanque base |
| **Rebellion / Rebelión / 叛逆** (FIRMA At) | Básica | 1 | At | 6 de daño; **LUEGO te quitás el yelmo** (entrás en Rebelión). El daño se resuelve ANTES del switch (up: 9) | FIRMA forma ofensiva; la escena de Apocrypha jugable desde combate 1 |
| **Lower the Visor / Bajar la Visera / 落下面甲** (FIRMA Hab) | Básica | 1 | Hab | 4 de Bloqueo; **te ponés el yelmo** (entrás en Enmascarado); +5 NP (up: 7 Bloqueo / +10 NP) | FIRMA forma defensiva; el retiro táctico que carga |

**MAZO INICIAL (10) — QAABB real sesgado a Buster:** 3× Buster + 2× Arts + 1× Quick + 2× Defender + 1× Rebelión + 1× Bajar la Visera. Cero Golpes vanilla. Empezás Enmascarada (Busters a 8); jugás Rebelión y los Busters pasan a 12 — la danza del casco ya está en el mazo inicial. 2 Arts + firmas ≈ 70-80 NP/ciclo → primera ulti en acto 1; Quick + starter → primer Crítico Listo dentro del acto 1.

### 5.1 Comunes (20) — ~40% pan-y-manteca, ~40% alimenta motor, ~20% payoff

| Nombre (eng / esp / zhs) | ⚡ | Tipo | Efecto | Engranaje |
|---|---|---|---|---|
| **Slash of Clarent / Tajo de Clarent / 克拉伦特斩** | 1 | At | 9 de daño; +10★ (up +3/+10) | [★] |
| **Rebel's Double Edge / Doble Filo Rebelde / 叛逆双刃** | 1 | At | 4 de daño ×2; +10 NP (up +2/+10) | [NP] |
| **Onslaught of Hatred / Embate de Odio / 憎恶猛击** | 2 | At | 14 de daño; vs Élites/Jefes: +4 (up +4/+2). Glow | [anti-autoridad; golpe-blanco del ×2] |
| **Sparks of the Helm / Chispas del Yelmo / 头盔火花** | 1 | Hab | 8 de Bloqueo; si Enmascarado: +20 NP (up +3 Bloqueo). Glow | [Formas→NP] |
| **Loudmouth's Taunt / Provocación de Bocazas / 大嘴挑衅** | 0 | Hab | 1 de Débil; +10★ (up: 2 de Débil) | [★; su trash-talk] |
| **Spoils of Camelot / Botín de Camelot / 卡美洛战利品** | 0 | Hab | si ≥50 NP: −50 NP → +50★ (up: consume 30). Glow | [ESPEJO A] |
| **Tribute to the Throne / Tributo al Trono / 王座供奉** | 0 | Hab | si ≥50★: −50★ → +50 NP (up: consume 30). Glow | [ESPEJO B] |
| **Mana Ignition / Ignición de Maná / 魔力点火** | 0 | Hab | +30 NP (up +50) | [NP; sabor Mana Burst] |
| **Battle Instinct / Instinto de Batalla / 战斗直觉** | 1 | Hab | si ≥30★: −30★ → robá 2 (up: consume 20). Glow | [★→robo; Instinct B en común] |
| **Residual Lightning / Relámpago Residual / 残雷** | 1 | At | 6 de daño a TODOS; +10★ (up +2) | [AoE + ★] |
| **Insolent Strike / Espadazo Insolente / 无礼一击** | 0 | At | 4 de daño; si consumiste un CRÍTICO este turno: +10 NP (up +2/+10). Glow | [rider calibrado a la starter] |
| **War Cry / Grito de Guerra / 战吼** | 1 | Hab | +20 NP; +10★ (up +10/+10) | [NP+★] |
| **Insolent Guard / Guardia Insolente / 无礼防御** | 1 | Hab | 5 de Bloqueo; +10 NP (up +3/+10) | [NP] |
| **Tournament Footwork / Paso del Torneo / 比武步法** | 0 | Hab | 3 de Bloqueo; +10 NP (up +2/+5) | [NP] |
| **Proud Greatsword / Mandoble Orgulloso / 傲慢大剑** | 1 | At | 12 de daño; perdés 2 de Vida (→ +10★ vía starter) (up +4) | [Vida→★] |
| **Defiance of Authority / Desafío a la Autoridad / 蔑视权威** | 1 | At | 7 de daño; vs Élites/Jefes: +10 NP (up +3/+20). Glow | [anti-autoridad→NP] |
| **Lightning Splinters / Astillas de Relámpago / 雷霆碎片** | 1 | At | 3 de daño ×3 (aleatorio); +10★ (up: 4×3) | [multi-hit + ★] |
| **Ride the Stolen / Cabalgar lo Robado / 骑乘掠夺** | 1 | Hab | robá 2; +10★ (up: robá 3) | [Riding B; ciclo+★] |
| **Knight's Steadfastness / Firmeza del Caballero / 骑士坚毅** | 2 | Hab | 13 de Bloqueo; si Enmascarado: +10 NP (up +4/+20). Glow | [Formas→NP defensivo] |
| **"Don't Call Me a Girl!" / «¡No me llames niña!» / 别叫我小姑娘！** (meme) | 0 | Hab, Exhaust | +10★; +10 NP (up: +20/+10) | [reglas de trato del perfil oficial] |

### 5.2 Poco comunes (28)

| Nombre (eng / esp / zhs) | ⚡ | Tipo | Efecto | Engranaje |
|---|---|---|---|---|
| **Mana Burst A / Estallido de Maná A / 魔力放出A** | 1 | Hab, Exhaust | este turno tus Ataques hacen +4 (up +6) | [S1 base 1:1; Exhaust = CD] |
| **Knight of Red Lightning A+ / Caballero del Relámpago Rojo A+ / 赤雷骑士A+** | 2 | Poder | tus Ataques +2; tus CRÍTICOS hacen +6 adicional (up +3/+8) | [S1 rank-up; el multiplicador real de la forma vive acá §5] |
| **Instinct B / Instinto B / 直感B** | 1 | Hab, Exhaust | +30★; robá 1 (up +50) | [S2 base 1:1] |
| **Cigarette Lion B+ / León del Cigarrillo B+ / 香烟雄狮B+** | 2 | Poder | al jugarla +20★; cada vez que obtenés un CRÍTICO LISTO: robá 1 (up: −1⚡) | [S2 rank-up; guiño a Kairi] |
| **Secret of Pedigree EX / Secreto de Cuna EX / 不贞隐藏之兜EX** | 1 | Hab, Exhaust | removés TODOS tus debuffs; 12 de Bloqueo; +20 NP; si Enmascarado: +10 NP más (up: 16/+30). Glow | [S3 1:1: cleanse+DEF+carga] |
| **Lightning of Clarent / Relámpago de Clarent / 克拉伦特之雷** | 2 | At | 18 de daño; +10★ (up +5) | [golpe-blanco del ×2] |
| **Storm of Steel / Tormenta de Acero / 钢铁风暴** | 2 | At | 5 de daño ×3; +10★ (up: 6×3) | [multi-hit] |
| **Defiant Cut / Corte Desafiante / 挑衅斩** | 1 | At | 12 de daño; vs Élites/Jefes: +10★ (up +4). Glow | [anti-autoridad→★] |
| **Reckless Charge / Carga Temeraria / 鲁莽冲锋** | 3 | At | 26 de daño (up +7) | [pan-y-manteca grande, slot Bludgeon] |
| **Lightning Speed / Velocidad del Relámpago / 雷速** | 1 | At | 9 de daño; en Rebelión: +10★ (up +3). Glow | [payoff forma ofensiva] |
| **Dented Helm / Yelmo Abollado / 凹陷头盔** | 1 | Hab | 11 de Bloqueo; en Enmascarado: +10★ (up +4). Glow | [payoff forma defensiva] |
| **Roar of Rebellion / Rugido de Rebelión / 叛逆咆哮** | 0 | Hab | cambiá a tu forma opuesta; robá 1 (up: robá 2) | [pan-y-manteca de FORMAS] |
| **Lightning Visit / Visita Relámpago / 闪电造访** | 0 | Hab | cambiá de forma; al final del turno volvés a la forma anterior (up: al volver, +10★) | [ventana de 1 turno: arrancarse el yelmo, rugir, volver] |
| **Rude Kick / Patada Descortés / 无礼飞踢** | 1 | At | 8 de daño; 1 Vulnerable; si tenés CRÍTICO LISTO: 2 Vulnerable (up +3/+1). Glow | [setup de crítico, patrón ShieldRam] |
| **Plunder of the Royal Hoard / Saqueo del Tesoro Real / 王室宝库劫掠** | 1 | Hab, Exhaust | +30 NP; +10★ (up +40/+20) | [burst NP] |
| **Chained Lightning / Relámpago Encadenado / 连锁闪电** | 1 | At | 6 de daño a TODOS; si consumiste un CRÍTICO este turno: +6 a todos (up +2/+2). Glow | [crítico→AoE] |
| **Dragon's Blood / Sangre de Dragón / 龙之血** | 1 | Poder | al inicio del turno: +5 NP y 3 de Bloqueo (up: +5/+5) | [trait Dragon; engorda hilos existentes] |
| **Accumulated Hatred / Odio Acumulado / 积怨** | 1 | Poder | cada vez que perdés Vida: +10 NP (máx 2/turno) (up: máx 3) | [Vida→NP; cada sangrado paga DOS economías con la starter] |
| **Banner of Rebellion / Estandarte de la Rebelión / 叛逆旗帜** | 2 | Poder | cada cambio de forma: +10★ y +5 NP (up: y robá 1) | [motor de FORMAS] |
| **Demanded Duel / Duelo Exigido / 强求决斗** | 1 | Hab | 1 Débil y 1 Vulnerable a UN enemigo; +10 NP (up: 2/2) | [control+NP] |
| **Father's Sword / Espada del Padre / 父亲之剑** | 2 | At | 14 de daño; si tenés ≥50 NP (sin gastarla): +6 (up +4/+8). Glow | [lee el banco sin gastar, patrón UtopianFortress; su deseo del Grial] |
| **Royal Tantrum / Berrinche Real / 王室任性** | 1 | At | 10 de daño; perdés 1 de Vida (→ +10★) (up +4) | [Vida→★] |
| **Tournament Guard / Guardia del Torneo / 比武防御** | 1 | Hab | 9 de Bloqueo; +10★ (up +3/+10) | [★ defensivas] |
| **Ill-Repaid Loyalty / Lealtad Mal Pagada / 错付的忠诚** | 1 | Hab, Exhaust | curás 6; +10 NP (up: 9/+20) | [sustain conectado] |
| **Homunculus Acceleration / Aceleración de Homúnculo / 人造人加速** | 1 | Poder | la 1ª vez que consumís un CRÍTICO cada turno: +10★ (up +20) | [crítico→★, capeado 1/turno] |
| **Double Rebellion / Doble Rebelión / 双重叛逆** | 1 | At | 7 de daño; si cambiaste de forma este combate (`FormShiftedPower`): +10★ y +10 NP (up +3). Glow | [payoff de la danza] |
| **Scorn for the Throne / Desdén al Trono / 蔑视王座** | 1 | Hab | +20 NP; vs Élite/Jefe: +10 más (up +10). Glow | [anti-autoridad→NP] |
| **"Treat Me as a Knight" / «Trátame como caballero» / 把我当骑士对待** | 1 | Hab | robá 2; si Enmascarado: robá 1 más (up: robá 3 / +1). Glow | [ciclo+forma; regla de trato] |

### 5.3 Raras (20)

| Nombre (eng / esp / zhs) | ⚡ | Tipo | Efecto | Engranaje |
|---|---|---|---|---|
| **Clarent Blood Arthur (manual) / Clarent Blood Arthur (manual) / 克拉伦特·血染亚瑟（手动）** | 2 | At NP | mín 70, consume TODA: 5 de daño ×5 a TODOS; vs Élites/Jefes +2/golpe; después +10 NP. SOBRECARGA +1/golpe por cada 20 sobre mín (up: 6×5). Glow al poder pagarse | [el NP como carta; nicho: disparar antes del auto-ulti] |
| **Mana Burst: Red Lightning (mini-NP) / Estallido de Maná: Relámpago Rojo / 魔力放出·赤雷** | 1 | At NP | mín 50, consume TODA: 16 a UN enemigo, +4 por cada 10 sobre mín (up: base 22). Glow | [piso spameable de la economía NP] |
| **Beheading of the Usurper / Decapitación del Usurpador / 篡位者斩首** | 3 | At | 24 de daño; vs Élites/Jefes +12 (up: 30/+15). Glow | [anti-autoridad clímax, slot Ejecución de la Guardiana] |
| **Hundred Shattered Swords / Cien Espadas Astilladas / 百剑碎裂** | 0 | At | solo jugable con ≥50★: consumí 50; 26 de daño (up 32). Glow | [slot Comet: ★ como munición] |
| **Avalanche of Hatred / Avalancha de Odio / 憎恶雪崩** | 2 | At | 4 de daño ×4 a UN enemigo; si consumiste un CRÍTICO este turno: +2/golpe (up: 5×4). Glow | [multi-hit post-crítico] |
| **Camlann / Camlann / 卡姆兰** | 1 | Hab, Exhaust | ganás 1 de ALZARSE (`GutsPower`); cuando se activa: +100 NP (up: y +50★) | [lore beat de Camlann; reuso GutsPower] |
| **Crimson Lightning (Climax Power) / Relámpago Carmesí (Poder Clímax) / 绯红闪电（巅峰之力）** | 2 | Poder, Exhaust | entrás en forma **RELÁMPAGO CARMESÍ** (permanente: Ataques +2, retención 10, +5 NP/turno, sin penalización; tu ulti pasa a «Interludio») (up: 1⚡) | [asc 4: el clímax del eje de FORMAS] |
| **Secret Revealed / Secreto Revelado / 秘密揭露** | 2 | Poder | cada vez que te quitás el yelmo (entrás en Rebelión): +20★ y robá 1 (up: −1⚡) | [motor de la danza] |
| **Crown of Lightning / Corona del Relámpago / 雷之冠** | 2 | Poder | al inicio del turno: +10★ (up +20) | [per-turn ★, slot Angel] |
| **Ambition for the Throne / Ambición del Trono / 王座野望** | 2 | Poder | al inicio del turno: +10 NP (up +15) | [per-turn NP, slot Bendición de Avalon] |
| **The Most Radiant Sword / La Espada Más Resplandeciente / 最耀眼之剑** | 2 | Poder | tus CRÍTICOS hacen +8 adicional y devuelven +10 NP extra al consumirse (up +12) | [Clarent; cose ★→NP, precedente Lupa] |
| **Kairi's Command Seal / Sello de Comando de Kairi / 凯利的令咒** | 0 | Hab, Exhaust | +50 NP (up +100) | [slot LastOrder; su Master ordena] |
| **Claim Caliburn / Reclamo de Caliburn / 夺取石中剑** | 2 | Hab, Exhaust | agregá a tu mano 1 carta RARA aleatoria de tu pool; cuesta 0 este turno (up: 1⚡) | [su deseo: sacar la espada de la selección; el pool se cita a sí mismo, slot 投影魔术] |
| **Feast After Battle / Festín tras la Batalla / 战后宴席** | 1 | Hab, Exhaust | curás 10; +10 NP (up: 14/+20) | [la cena con Sisigou] |
| **Storm of Camelot / Tormenta de Camelot / 卡美洛风暴** | 2 | At | 9 de daño a TODOS; +20★ (up: 12/+30) | [AoE raro + ★] |
| **Coup d'État / Golpe de Estado / 政变** | 3 | At, Exhaust | 36 de daño (up +10) | [clímax single-hit: EL blanco del ×2; Exhaust paga la sobre-tasa] |
| **Saberface / Saberface / 同脸** | 1 | Poder | la 1ª vez por turno que un enemigo te golpea: +10★ (up: y +10 NP) | [defensa→★; meme-lore] |
| **Denied Inheritance / Herencia Negada / 被否定的继承** | 0 | Hab, Exhaust | +30 NP y +30★ (up +40/+40) | [burst doble, slot CurseE] |
| **Double Edge of Hatred / Doble Filo del Odio / 憎恶双刃** | 2 | Poder | en Rebelión: tus Ataques hacen +3 adicional (up +4) | [stack del arquetipo de forma ofensiva, redundante adrede] |
| **Memory of Trifas / Memoria de Trifas / 特里法斯的记忆** | 2 | Poder | al inicio de cada turno: curás 2 y +5 NP (up: 3/+5) | [sustain conectado al hilo NP; el epílogo] |

### 5.4 Tokens / especiales (manifestadas, no drafteables)

| Nombre (eng / esp / zhs) | ⚡ | Efecto | Notas |
|---|---|---|---|
| **Clarent Blood Arthur: Unleashed / …: Desatado / …：解放** | 0 | auto-manifestada a 100 (Retain, mín 100, consume TODA, Exhaust): **si Enmascarado, PRIMERO te quitás el yelmo** (entrás en Rebelión). 6 de daño ×5 a TODOS; vs Élites/Jefes +1/golpe; después +10 NP. SOBRECARGA +1/golpe por cada 20 sobre 100 | escala +15%/nivel (`NpLevels.Scale`). A 300: 16×5=80 AoE; con forma +2: 18×5 |
| **Clarent Blood Arthur: Interlude / …: Interludio / …：幕间** | 0 | la manifestada en forma Relámpago Carmesí (upgrade 400→600%): 8 de daño ×5 a TODOS; vs Élites/Jefes +2/golpe; después +20 NP. SOBRECARGA +1/golpe por cada 15 sobre 100 | escala con dupes |
| **3 Form Powers (Single)** | — | Caballero Enmascarado / Caballero de la Rebelión / Relámpago Carmesí (`IsPermanent`) | sin powers-contadores nuevos; las Estrellas/Crítico Listo son los de FGOCore compartidos |

**Interacción Crítico ×2:** dobla solo el PRIMER golpe de los 5 del NP (precedente JeanneNP) — micro-decisión de secuenciado. **Sin Fuerza en el pool propio** (decisión deliberada: la ulti multi-hit ×5 escalaría doble; su única Fuerza es externa).

---

## 6. Reliquias (SIN multiplicador global nuevo; trilingüe)

| Slot | Nombre (eng / esp / zhs) | Efecto |
|---|---|---|
| **STARTER 1** | **Clarent, the Stolen Sword / Clarent, la Espada Robada / 克拉伦特·被窃之剑** | (1) al iniciar cada combate entrás en forma **Caballero Enmascarado** (dispara la precarga de FormVisuals); (2) cada pérdida de Vida → **+10 Estrellas**; (3) cada CRÍTICO LISTO consumido → **+10 NP**. El motor estilo 焰刑地狱: convierte eventos universales en recursos del kit. Calibra TODOS los riders de ★ |
| **STARTER 2** | **Oath of the Knight of Treachery / Juramento del Caballero de la Traición / 叛逆骑士的誓约** | `BondRelic` FGOCore: vínculo con niveles + `ServantDamageMultiplier`/`ServantBlockMultiplier` **×1.25 global** (palanca §1.bis, sin ×daño nuevo). Override Nv 4/7: empezás cada combate con +10/+20★. Capstone Nv 10 «Reconocido como Hijo»: la 1ª vez que te quitás el yelmo cada combate: **+1⚡** (capeado 1/combate, sin loop ⚡-positivo) |
| **STARTER OCULTA** | **Summoning Seal: Saber of Red / Sello de Invocación: Saber of Red / 召唤刻印·红Saber** | `INpLevelStore`: dupes/NP level 1-5, pity 50%+25%, botón «Invocar (dupe)»; +15%/nivel a todas las cartas NP vía `NpLevels.Scale` |
| **JEFE** (reemplaza Starter 1) | **Clarent Overloaded with Hatred / Clarent Sobrecargada de Odio / 充溢憎恶的克拉伦特** | mantiene la entrada en Enmascarado y **DUPLICA ambas conversiones**: +20★ por pérdida de Vida; +20 NP por Crítico consumido (arco Hellup/LordCamelotRestored) |
| **TIENDA** | **Red Bike of Trifas / Moto Roja de Trifas / 特里法斯的红摩托** | tus cartas Quick otorgan **+10★ adicionales** (Riding B: sabe montar de todo) |
| **POCO COMÚN** | **Kairi's Cigarettes / Cigarrillos de Kairi / 凯利的香烟** | la 1ª vez que obtenés un CRÍTICO LISTO cada combate: robá 2 |
| **POCO COMÚN** | **Magic Resistance B Charm / Amuleto de Resistencia Mágica B / 对魔力B护符** | el primer debuff enemigo de cada combate se anula (pasiva real; precedente Artoria) |
| **POCO COMÚN** | **Red Glasses of Saber / Gafas Rojas de Saber / 红Saber的眼镜** | al inicio de cada combate: +20★ (costume Glasses Spiritron) |
| **RARA** | **Banner of Camlann / Estandarte de Camlann / 卡姆兰旗帜** | en combates vs Élite/Jefe: empezás con +30 NP y +10★ (engorda el rider anti-autoridad donde importa) |
| **RARA** | **The Empty Seat of the Round Table / El Asiento Vacío de la Mesa Redonda / 圆桌的空席** | cada carta NP jugada: +20★ (la silla que nunca le dieron) |
| **RARA** | **Grey Cat of Trifas / Gato Gris de Trifas / 特里法斯的灰猫** | al final de tu turno, si no jugaste ningún Ataque: +10★ y curás 2 (el turno de tregua; precedente Búho) |
| **RARA/EVENTO** | **Holy Grail of Selection / Santo Grial de la Selección / 选定的圣杯** | `ILimitBreaker`: +15 HP máx; Vínculo hasta 12, NP level hasta 6 (precedente Cáliz de Artoria) |

---

## 7. Stats del personaje

- **HP máx = 75**, 3⚡, 99 oro. Entre Ironclad 80 / Morgan 78 y Silent 70 / Artoria 70: tiene END A y armadura completa, **pero su plan ofensivo la deja sin armadura** (Rebelión recibe +2/golpe) → más carne que las casters, menos que las berserkers. Rango ecosistema §1.bis-5 (centro ~75), explosiva → no inflar.
- **Carga NP 0-300** (ulti auto a 100 con Retain); **Estrellas de Crítico FGOCore compartidas** (0-100+, auto-payoff a 100, sin candado de forma). Arranca cada combate en forma **Caballero Enmascarado**.
- **Mazo inicial de 10**: QAABB sesgado (3B/2A/1Q/2D/2 firmas, cero Golpes vanilla).
- **Pool**: 4 básicas propias (+3 de comando FGOCore) + 20 comunes + 28 poco comunes + 20 raras + 2 especiales manifestadas (plantilla vanilla 4/20/36/26 adaptada al estándar del proyecto ~4/20/28/20).
- **Descripción in-game (tentativa, eng/esp/zhs):** *"The Knight of Treachery. Tank your rage behind the Helm, then tear it off to cash it in as crimson-lightning Crits and Clarent Blood Arthur — the blade that strikes hardest against those who sit higher than you."* / *"El Caballero de la Traición. Tanqueá tu rabia tras el Yelmo, después arráncatelo para cobrarla en Críticos de relámpago carmesí y en Clarent Blood Arthur — la espada que más fuerte hiere a quien se sienta más alto que vos."* / *"叛逆的骑士。在头盔之下积蓄怒火，然后将它扯下，化作绯红雷霆的暴击与克拉伦特·血染亚瑟——对地位高于你者斩击最重之剑。"*

---

## 8. Notas de arte/animación

- **Servant id 100900** (collectionNo 76, Saber 5★, ilustrador Ototsugu Konoe, CV Miyuki Sawashiro). **Modelo de batalla ÚNICO 100900** para todas las ascensiones (5,969,913 bytes UnityFS, textura 2048×2048 verificada). Costumes opcionales como skins de reliquia: **100930** (Memories of Trifas) y **100940** (Red Saber con lentes).
- **Las 3 formas vía attach/detach del casco** sobre el MISMO rig (atlas trae yelmo cornado + cabeza rubia como partes separadas — verificado inspeccionando `100900.png`). `FramesPath` conceptual: `resources/forms/masked/` (ref `100900a@1`), `resources/forms/unmasked/` (ref `100900b@1`), `resources/forms/crimson/` (unmasked + overlay de partículas/shader de relámpago carmesí, ref `100900b@2` = fotograma del NP). **Caso de pipeline NUEVO** (más simple que Morgan): documentar en WORKFLOW-FGO. Fallback: 2 rigs completos del mismo bundle.
- **Charagraphs** (HTTP 200): `100900a@1/a@2` (asc 1-2, casco puesto/recién quitado), `100900b@1/b@2` (asc 3-4, casual/lightning). **Faces** `f_1009000..3`. **Select-bg:** charagraph `100900b@2` (asc final, el lightning).
- **Iconos de skill:** `skill_00306` (Mana Burst — OJO: la API da el MISMO icono para el rank-up A+; elegir otro a mano, p.ej. del catálogo `make_morgan_icons`), `skill_00603` (Instinto), `skill_00308` (León del Cigarrillo), `skill_00400` (Secreto de Cuna). **Card-NP art:** `Commands/100900/card_servant_np.png`. **Icono starter de mecánica:** clase Saber DORADA 5★ (patrón Morgan/Mooncell 金卡Saber).
- **Arte de cartas** vía `match-ce-art.js` (CEs de Apocrypha/Mordred abundan) con dedup contra `mapping_morgan`/`mash`/`artoria`.
- **Loc trilingüe** (~270 strings: ~75 cartas + 12 reliquias + 3 form powers × 3 idiomas). Terminología Mooncell fijada: 克拉伦特·血染亚瑟 (Clarent Blood Arthur) · 不贞隐藏之兜 (Secret of Pedigree) · 赤雷骑士 (Knight of Red Lightning) · 香烟雄狮 (Cigarette Lion) · 暴击星 (Estrellas de Crítico). Frases ORIGINALES desde JP+中文 → `VOICE-LINES.md`. **Reglas de trato verbatim** (perfil oficial): no insultar ni alabar al Rey Arturo; no tratarla como mujer ni de hombre obviamente; no ser formal; escuchar lo que dice. Correr `tools/audit_simpleloc.ps1` antes de publicar.

---

## 9. Checklist final (§6 de la skill — corrido)

- [x] **Cada carta sobre-tasa paga su exceso (§3):** Golpe de Estado 36/3⚡ con Exhaust (+30-50% ✓); Cien Espadas 26/0⚡ con −50★ (≈1⚡+banco ✓); Herencia Negada 0⚡ con Exhaust; Camlann +100 NP solo al gatillar Alzarse (condición que puede no llegar); Mandoble/Berrinche pagan con Vida (1-2 HP ≈ rider chico). NPs AoE 5 golpes con descuento −25-35% AoE aplicado (6/golpe vs 8-9 ST). Skills reales = Exhaust 1:1 (regla panel Artoria).
- [x] **Mazo inicial gana acto 1 sin motor:** 3 Busters + firma = 30-40 daño/ciclo a tasa vanilla; 2 Defenders + retención 10. ✓
- [x] **Plan B vs presión:** quedarse Enmascarada y pegar a −2 (tasa defensiva normal, nunca por debajo); el rider anti-autoridad duerme en pasillos pero las bases solas alcanzan. ✓
- [x] **Nombres en 3 idiomas con terminología CN oficial.** ✓
- [x] **Assets verificados** (22 URLs HTTP 200/206; modelo único 100900; casco confirmado como pieza separable en la textura). ✓
- [x] **Reutiliza FGOCore en todo** (NP, Estrellas global, Formas, Bond, dupes, Guts, Baluarte, tríada de comando); única pieza core nueva = evento «Crítico consumido» (reutilizable). ✓
- [x] **Power-budget al nivel Watcher, no por encima:** cero ×daño global nuevo; el burst emerge de motor gateado (bancar 100★ → ×2; cruzar 100 NP → ulti); su única Fuerza es externa; presupuesto de complejidad = formas + 2 economías core = igual a Mash/Morgan. ✓

**Perillas de playtest (en orden, NUNCA el daño base de la ulti):** (1) −2 de Enmascarado en turno 1 → si feo, −2→−1 o firma Rebelión garantizada en mano inicial (NO quitar el malus). (2) Parking en Rebelión con sustain externo → subir +2 recibido a +3. (3) Loop ★↔NP (starter + Aceleración + Espada Más Resplandeciente) → caps 1/turno + bajar extra de rara a +5. (4) Rider anti-autoridad en pasillos → tocar bases ±1-2 antes que el rider. (5) Desatado desenmascarando como auto-buff → si domina, switch DESPUÉS del daño + cambiar incentivo a +10★ al revelarte. (6) Ping-pong de formas → caps 1/combate y 1/turno. (7) Multi-hit ×5 vs Fuerza externa → marcar golpes 2-5 como no-escalantes (precedente vanilla) o aceptar como techo modded. (8) Re-rig del casco no probado → fallback 2 rigs completos.