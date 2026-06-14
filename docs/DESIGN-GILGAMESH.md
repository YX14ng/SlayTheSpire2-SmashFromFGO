# DESIGN-GILGAMESH — diseño del personaje (Archer, FGO) para StS2

Fusión razonada de las dos propuestas (`gilgamesh_design_a.json` + `gilgamesh_design_b.json`) sobre el kit real (`gilgamesh_kit.json`). Diseñado con la skill `sts2-mechanics-design`. Kit/assets en `gilgamesh_kit.json` / `gilgamesh_assets.json`. Manifest id **`GilgameshArcher`** (inmutable), dependencies `["BaseLib","FGOCore"]`. Lore investigado JP (línea base) + 中文 simplificado (Mooncell), regla WORKFLOW-FGO §2.

**Veredicto de fusión (qué tomo de cada uno):**
- **De A:** la economía de Oro COMPLETA con denominaciones, los pares espejo deficitarios, el payoff por atesorar (Babilonia), la Sobrecarga de Enuma como multiplicador (`×1.5→×2.0` fiel), el techo Bond ×1.25, el riesgo técnico de mutar oro documentado, el mazo QAABB de 5 cartas de comando.
- **De B:** las denominaciones de oro más simples (5/10/25/50 en vez de 5/10/15/25), los 8 nombres concretos del arsenal con Dáinsleif/HP, los números limpios de Enuma (30 base / +15 anti-jefe / +4 por 20), el poder **Trono del Observador** (la arrogancia con castigo natural), Cobro de Deudas (oro al matar), y la separación clara de arquetipos.
- **Resuelvo conflictos:** HP **72** (de A — semidiós arrogante que no defiende, END C, kit explosivo → bajar como dice §1.bis regla 5). Oro inicial **99 estándar** (NO 149/150 — ver §7 justificación: el +50 distorsiona el meta del run, es la perilla #1 de ambos riesgos; la identidad "Rey del Oro" la dan las cartas, no el stat inicial). Enuma base de B (30/+15/+4 por 20, más limpio que el ×0.125 de A).

---

## 1. Identidad en una frase

**El Rey que ya posee todo y por eso jamás toca al enemigo: el oro abre la Puerta de Babilonia, la Puerta escupe los PROTOTIPOS de todas las armas (tribu de cartas-arma generadas), las armas pagan el juicio despectivo del Rey (Críticos), y cuando los mestizos aburren, Ea borra el mundo (Enuma Elish: ulti AoE cuya Sobrecarga muerde a lo divino — Élites/Jefes — pero no a lo meramente humano).** Es el ÚNICO personaje del roster que usa el **oro de la run** como recurso de combate.

---

## 2. Inventario del kit FGO (fidelidad §4.1: cada pieza = carta/reliquia reconocible)

**Servant:** Gilgamesh, Archer 5★ (SSR), collectionNo 12. 英雄王 "Rey de los Héroes". CV Tomokazu Seki, ilustrador Takeuchi. Deck QAABB (Quick-Arts-Arts-Buster-Buster). Atributo Cielo. NP EX, STR B, END C, LCK A.

| Pieza del kit FGO | Mapeo en el diseño |
|---|---|
| **Carisma A+** (S1 base: ATK party +21%, 3T) | Carta poder-skill **KIT Carisma A+** (PC) |
| **El Que Vio el Abismo EX** (S1 up: +NP Strength a aliados Cielo + 20★ instantáneas) | Carta rara **KIT El Que Vio el Abismo EX** (otorga Bendición de Sobrecarga + 20★) |
| **Regla de Oro A** (S2: NP Gain +50%, 3T — atraer riqueza) | **FIRMA básica Regla de Oro** + carta poder **KIT Regla de Oro A** (Oro→NP) |
| **Collector EX** (S3 base: Star Gather +600%) | Carta **KIT Coleccionista EX** (PC, +★ + robo) |
| **Tesorería de Babilonia EX** (S3 up: Star Gather + carga NP 30% instantánea, Interludio 3) | Carta rara **KIT Tesorería de Babilonia EX** |
| **Enuma Elish — 天地乖離す開闢の星** (NP A++→EX, AoE Buster, superefectivo ×1.5→×2.0 vs Cielo/Tierra escalado por OC) | **ENUMA ELISH: Desatado** (ulti auto a 100) + 2 cartas-NP drafteables |
| **Gate of Babylon — 王の財宝** (rank E~A++, su ataque normal: dispara prototipos de todos los NP) | **Tribu de Armas del Tesoro** (8 tokens) + cartas-NP "andanada" + reliquia starter |
| **Independent Action A+** (pasiva: Crit Damage +11%) | Carta poder **KIT Acción Independiente A+** + reliquia (Monóculo del Juez) |
| **Divinity B** (pasiva: Damage Plus +175 plano/carta — 2/3 dios) | Carta poder **KIT Divinidad B** + reliquia (Sangre de los Dioses) |
| **Magic Resistance E** (pasiva irrisoria: Debuff Resist +10%) | Reliquia meme **Amuleto de Pacotilla** (anula solo el 1er Débil) |
| **Append 2 — Load Magical Energy** (empieza con NP cargado) | Override de BondRelic Nv 7 + reliquia de tienda (Reserva de Maná) |
| **Cadenas del Cielo / Enkidu** (su tesoro: la cadena que ata dioses) | Carta **Cadenas del Cielo** (anti-divino, escala vs Élite/Jefe) |

---

## 3. Recurso / motor del personaje — EL LOOP ECONÓMICO

**Presupuesto de complejidad §1.3:** 2 mecánicas originales (**Armas del Tesoro**, **Oro en combate**) + 2 de FGOCore (**Carga NP**, **Estrellas de Crítico**). Al límite, **sin formas** (el kit no las pide — ver §3.5). Cuatro estaciones, **todas visibles en pantalla**:

1. **ORO → PUERTA.** Cartas con rider «Pagá X de Oro:» compran efectos sobre-tasa. El oro de la run ES el recurso acumulable más fungible (§3 de la skill): pagarlo en combate = renunciar a removals/reliquias/pociones de la tienda — **costo real, honesto y ÚNICO en el roster**. Glow dorado `ShouldGlowGoldInternal` cuando `CanPay`.
2. **PUERTA → ARMAS.** Generadores añaden **Armas del Tesoro** a la mano (tokens 0⚡ Exhaust de un arsenal de 8 prototipos nombrados — cada ataque es un arma distinta, fidelidad literal al lore "coleccioné los modelos originales de toda tecnología").
3. **ARMAS → ESTRELLAS / NP.** La reliquia starter **Bab-ilu** convierte el evento universal del kit (jugar un Arma) en **+10 Estrellas de Crítico, máx 3/turno** (regla §4.6: starter-motor con cap 3/turno). A 100 estrellas, FGOCore auto-paga 1 **Crítico Listo** (próximo Ataque ×2 = el juicio despectivo del Rey).
4. **TODO → NP → ENUMA ELISH.** Arts/Vajra/riders cargan NP; a **100** se auto-manifiesta **Enuma Elish: Desatado** (gratis, Retain). Y el ciclo cierra: la Regla de Oro recupera oro con generadores capeados.

**Tasa de cambio ancla:** `5 Oro ≈ ⅓⚡ ≈ 10 NP ≈ 10 Estrellas`. Denominación de Oro: **5/10/25/50** (10 Oro ≈ 1⚡ de efecto; 50 Oro ≈ una poción de tienda). **Round-trips DELIBERADAMENTE deficitarios** (Inversión Real: 25 oro→50 NP; Dividendos: 50 NP→15 oro = pierde 10 por vuelta — **sin arbitraje infinito**).

**DECISIÓN MACRO del personaje:** ¿el oro va a la tienda (poder permanente) o a la guerra (poder inmediato)? Con un payoff por **ATESORAR** (Babilonia, la Capital del Oro: daño = oro÷10) para que *no gastar* también sea build.

**Arquetipos drafteables:**
- **(a) Arsenal Total** — volumen de Armas + Divinidad B + Despliegue/Puertas de Par en Par.
- **(b) El Juicio del Rey** — Estrellas/Críticos + Coleccionista + Acción Independiente.
- **(c) Derroche Imperial** — Oro→tempo + Riqueza Heredada + El Más Rico del Mundo.
- **(d) Avaro** (contrapeso) — atesorar para Babilonia + ingreso pasivo.

**Plan A interrumpible (§1.2):** sin oro (gastado en tienda o run pobre) los pagos se apagan y Gil baja a **tasa normal, nunca debajo**; la tribu necesita generadores robados; la presión agresiva fuerza Bloqueo medio con HP 72 → el NP se frena (Arrogancia Dorada / Trono del Observador dan el plan B parcial); el crítico ×2 es de UN golpe (multi-hit/AoE lo diluyen); y **vs hordas de mobs el superefectivo de Enuma NO aplica** (×1 contra lo meramente humano — el anti-bonus es fiel Y es balance).

### 3.5 SIN FORMAS (justificado, regla §5)
Gilgamesh no tiene cambio de modo ni costumes en FGO; sus 3 modelos de batalla son ascensiones del MISMO kit. → **NO hay FormPower** (una "forma" sin cambio de modelo/decisión violaría §5). **Opcional cosmético (no bloqueante):** `FormVisuals` SIN pasivas — el combate abre con `200210` (armadura dorada, Gate activo) y al manifestarse Enuma Elish (NP≥100) swapea a `200220` (torso desnudo, líneas rojas, Ea en mano, pelo erizado), volviendo a `200210` bajo 100. Es solo el handler `GaugeFilled`/`GaugeDropped` de FGOCore disparando el swap visual, con precarga threaded (patrón Morgan). Si complica el pipeline, se lanza con `200210` fijo sin tocar balance.

---

## 4. Mapeo a FGOCore (reusa vs NUEVO)

**Se consume TAL CUAL (verificado en código):**
- `Np/NpCharge.cs` — 0-300, Gain capeado, `GaugeFilled`→manifestación con **Retain**, `GaugeDropped`→re-arme + swap visual, `ConsumeAllForNpCard` con mínimo + Sobrecarga, `INpCostWaiver` excluye Event.
- `Np/OverchargeBlessingPower.cs` — El Que Vio el Abismo / Decreto del Rey la otorgan.
- `Stars/CritStarsPower.cs` + `CritReadyPower.cs` — compartidas, auto-payoff a 100 (Morgan ya las estrenó; Gil solo agrega fuentes; iconos FGO 320/325).
- `Bond/BondRelic.cs` — `ServantDamageMultiplier`/`ServantBlockMultiplier` ×1.25 heredado (la palanca central §1.bis, NO ×global desde starter).
- `Np/INpLevelStore.cs` + `NpLevels.cs` — dupes +15%/nivel (NP1→NP5 real = 300%→500%).
- `ILimitBreaker.cs` — El Cáliz Original (Santo Grial).
- `MemeCard` + patrón cartas de comando Buster/Arts/Quick (copiar `BusterMorgan`/`ArtsMorgan`/`QuickMorgan` con `[Pool]` y prefijo propios, tag Strike heredado).
- `FormVisuals` (opcional cosmético, sin `FormPower`).

**NO se usan:** `Forms/FormPower` (salvo `FormVisuals`), `Curses/*`, `GutsPower`, `BulwarkPower`, `DragonScalesPower`.

**NUEVO en FGOCore (acotado, reutilizable — EMIYA/Unlimited Blade Works lo heredará):**
1. **Módulo `Arsenal/`:**
   - Clase base abstracta `ArsenalWeaponCard` (0⚡, Exhaust, `CardRarity.Special`, keyword custom de tribu «Arma del Tesoro» vía `CustomEnum CardKeyword`, con glow en lectores). Precedente directo: `KnightsArm` de Morgan.
   - Helper estático `Arsenal.AddRandom(creature, n)` contra un pool de tipos registrable por mod (patrón ModelDb + lista estática por personaje).
   - Evento `Arsenal.WeaponPlayed` (`Func<Creature, CardModel, Task>`, espejo de `NpCharge.GaugeFilled`) — lo consumen la starter, Ojos del Coleccionista, Colección Completa, los riders «si jugaste un Arma».
   - Contadores «Armas jugadas este turno/combate» como flags en power oculto `ArmsPlayedPower` (patrón `FormShiftedPower`). **En el mod de Gil, NO en FGOCore** hasta que otro personaje lo pida.
2. **Módulo `Gold/`** (en FGOCore — reutilizable para futuros servants mercantiles: Moriarty, da Vinci):
   - `CombatGold.Pay(player, amount)` / `.Gain(player, amount)` envolviendo la mutación del oro del `RunState` con `AssertMutable` + clamp a 0 + `InvokeDisplayAmountChanged` del HUD.
   - **VERIFICAR PRIMERO contra el decompilado** cómo mutan oro los Ladrones vanilla (localizar su cmd y calcar); y el sync multiplayer (¿bolsa por jugador?). **Fallback si el oro es inmutable en combate:** contador propio «Tesoro» sembrado al iniciar combate y liquidado al salir — misma matemática, peor elegancia.
   - `CanPay(player, amount)` → alimenta `ShouldGlowGoldInternal` de todos los riders de pago; debe re-evaluarse vía evento `AfterCombatGoldChanged(delta)` → refresh de mano (UX crítico).
   - Hook `IGoldChangedListener` para los acopladores (Regla de Oro A, Sello de la Regla de Oro) con sus caps (3/turno, 1/combate) **en el listener, no en el helper**.
3. **Variante de Sobrecarga multiplicativa** en `ConsumeAllForNpCard`: hoy escala daño base `PerTen`; Enuma necesita «bonus anti-RoomType por consumo». **Parametrizar el hook de OC** (`Func<int consumido, DamageContext, int/float>`) en vez de hardcodear `PerTen` — las cartas Morgan/Mash existentes no cambian.
4. **Loc trilingüe nueva en FGOCore** solo para el módulo Gold (keyword «Oro» hover) + keyword «Arma del Tesoro» + los 8 nombres del arsenal. El resto de keywords ya existe.
5. **Doc:** nota en `WORKFLOW-FGO.md` — oro-en-combate es economía FGOCore con denominaciones 5/10/25/50 y round-trips deficitarios obligatorios.

---

## 5. Pool COMPLETO de cartas

Nombres trilingües: **eng / esp latino / zhs (Mooncell)**. Denominaciones fijas (§4.6.3): NP y Estrellas en **10/20/30/50/100**; Oro en **5/10/25/50**; Armas en **1/2** (3-5 en burst raro). Glow dorado en TODA condicional.

### 5.1 Básicas (mazo inicial QAABB literal = Quick-Arts-Arts-Buster-Buster)

| Carta (eng / esp / zhs) | Coste | Tipo | Efecto exacto | Arquetipo |
|---|---|---|---|---|
| **Buster / Buster / 红卡** (×2) | 1⚡ | At | 10 de daño. (up +3) | base FGOCore, comando rojo |
| **Arts / Arts / 蓝卡** (×2) | 1⚡ | At | 6 de daño; +30 Carga NP. (up +3/+20) | NP — enseña el hilo |
| **Quick / Quick / 绿卡** (×1) | 1⚡ | At | 6 de daño; +30 Estrellas. (up +3/+20) | Estrellas — enseña el hilo |
| **Wall of Arms / Muralla de Armas / 兵装之壁** (×2) | 1⚡ | Hab (Defend) | 5 de Bloqueo. (up +3) | el Rey jamás bloquea con su cuerpo: portales interceptan |
| **Gate of Babylon / Puerta de Babilonia / 王之财宝** (FIRMA 1, ×2) | 1⚡ | Hab | Añade **2 Armas del Tesoro** aleatorias a tu mano. (up: 3) | Armas — el motor desde el turno 1 |
| **Golden Rule / Regla de Oro / 黄金律** (FIRMA 2, ×1) | 1⚡ | Hab | Ganá **5 de Oro**; +10 Carga NP. (up: 8 Oro / +20) | Oro+NP — la skill real S2 como básica |

**Mazo inicial (10):** 2× Buster + 2× Arts + 1× Quick + 2× Muralla de Armas + 2× Puerta de Babilonia + 1× Regla de Oro. Las 5 cartas de comando son la distribución QAABB exacta de FGO; las 2 Puertas garantizan tribu desde el turno 1; cero Strikes vanilla (Buster hereda el tag Strike para compat de eventos — patrón Morgan). **Gana el acto 1 sin motor:** 10+10+6+6+6 de comandos + ~10-14 de las Armas generadas (starter + Puertas) + 10 de Bloqueo. ✓

### 5.2 Comunes (20)

| Carta (eng / esp / zhs) | Coste | Tipo | Efecto exacto | Arquetipo |
|---|---|---|---|---|
| **Portal Volley / Andanada de Portales / 传送门连射** | 1⚡ | At | 9 de daño; si jugaste un Arma este turno: +3. Glow. (up 12/+4) | Armas (rider calibrado a starter) |
| **Scornful Shot / Disparo Desdeñoso / 蔑视之射** | 1⚡ | At | 5 de daño; +15 Estrellas. (up 8/+20) | Estrellas |
| **Rain of Blades / Lluvia de Hojas / 刃之雨** | 1⚡ | At | 3 de daño ×3 (aleatorio); +10 NP. (up 4×3) | NP (multi-hit, suma=single §2) |
| **Appraise the Spoils / Tasación del Botín / 战利品估价** | 1⚡ | At | 8 de daño; ganá 3 de Oro. (up 11/+5) | Oro (ingreso chico repetible) |
| **King's Verdict / Juicio del Rey / 王之裁决** | 2⚡ | At | 14 de daño; +20 Estrellas. (up 18/+30) | Estrellas (golpe grande = blanco del Crítico) |
| **Discarded Prototype / Prototipo Arrojado / 弃置原典** | 0⚡, Exhaust | At | 4 de daño; +5 NP. (up 6/+10) | NP (tasa Shiv) |
| **Open the Vault / Apertura del Tesoro / 开启宝库** | 0⚡ | Hab | Añade 1 Arma del Tesoro a tu mano. (up: además +5 NP) | Armas (grifo barato) |
| **Royal Investment / Inversión Real / 王之投资** (ESPEJO A) | 0⚡ | Hab | Pagá **25 de Oro**: +50 Carga NP. Glow. (up: pagá 15) | Oro→NP (comprás medio ulti con plata) |
| **Treasury Dividends / Dividendos del Tesoro / 宝库分红** (ESPEJO B) | 0⚡ | Hab | Perdé 50 NP: ganá **15 de Oro**. Glow. (up: perdé 30) | NP→Oro (deficitario, sin arbitraje) |
| **Collector's Greed / Avaricia del Coleccionista / 收藏家之贪** | 0⚡ | Hab | Perdé 50 Estrellas: +50 NP. Glow. (up: consume 30) | Estrellas→NP (等价交换) |
| **Wall of Spears / Muro de Lanzas / 长枪之壁** | 1⚡ | Hab | 8 de Bloqueo; si jugaste un Arma este turno: +3. Glow. (up 11/+4) | Armas+defensa |
| **Close the Gate / Cierre de la Puerta / 闭门** | 1⚡ | Hab | 6 de Bloqueo; +10 NP. (up 9/+15) | Bloqueo→NP |
| **Mandatory Tribute / Tributo Obligatorio / 强制纳贡** | 2⚡ | Hab | 16 de Bloqueo; ganá 5 de Oro. (up 20/+8) | Oro+defensa |
| **King's Coffers / Cofres del Rey / 王之金库** | 1⚡ | Hab | Robá 2; si tenés ≥50 de Oro: robá 1 más. Glow. (up: robá 3 base) | Oro (lector — casi siempre encendido) |
| **Appraiser's Gaze / Mirada del Tasador / 鉴定之眼** | 0⚡ | Hab | Aplica 1 de Débil; +10 Estrellas. (up: 2 Débil) | Estrellas+debuff |
| **"Mongrel." / «Mestizo.» / 「杂种。」** | 0⚡ | Hab | Aplica 1 de Vulnerable; +5 NP. (up: 2/+10) | el insulto marca registrada |
| **Wine of Victory / Vino de la Victoria / 凯旋之酒** | 1⚡, Exhaust | Hab | Curá 4; ganá 5 de Oro. (up 6/+8) | sustain+Oro |
| **Turned Key / Llave Girada / 转动之钥** | 1⚡ | Hab | +20 NP; robá 1. (up +30) | NP+ciclo (slot 高速咏唱) |
| **Treasure Count / Recuento del Tesoro / 宝物清点** | 1⚡ | Hab | +20 Estrellas; +10 NP. (up +30/+15) | doble paquete |
| **King's Festival / Festival del Rey / 王之飨宴** (meme) | 0⚡, Exhaust | Hab | +10 Estrellas; +10 NP; ganá 2 de Oro (la lotería siempre paga al Rey). (up: todo ×2) | meme — toca las 3 economías |

**Conectividad:** 20/20 comunes leen o escriben un recurso propio = **100%** (mínimo §4.6 = 90%). ✓ (Los dos fillers de tasa pura — Hoja Sin Nombre, etc. — viven en poco comunes donde el "deck inicial malo" lo exige).

### 5.3 Poco comunes (28)

| Carta (eng / esp / zhs) | Coste | Tipo | Efecto exacto | Arquetipo |
|---|---|---|---|---|
| **Total Barrage / Andanada Total / 全弹齐射** | 2⚡ | At | 4 de daño ×4 (aleatorio); +1 golpe por cada Arma jugada este turno (máx +2). Glow. (up 5×4) | payoff de tribu escalado |
| **Nameless Blade / Hoja Sin Nombre del Tesoro / 无铭之刃** | 1⚡ | At | 12 de daño. (up 16) | pan y manteca PURO (filler #1) |
| **Mongrel's Execution / Ejecución del Mestizo / 杂种处刑** | 2⚡ | At | 14 de daño; si tenés Crítico Listo: además 2 de Vulnerable. Glow. (up 18/3) | lector de crítico |
| **Twin Lances / Lanzas Gemelas / 双枪** | 1⚡ | At | 5 de daño ×2; +10 Estrellas. (up 7×2/+15) | Estrellas |
| **Treasury Bombardment / Bombardeo de la Tesorería / 宝库轰炸** | 2⚡ | At | 9 de daño a TODOS; pagá 10 de Oro: +5 a TODOS. Glow. (up 12/+6) | AoE comprable (oro→daño) |
| **Favored Weapon / Arma Favorita / 爱用之兵** | 1⚡ | At | 8 de daño; añade 1 Arma del Tesoro. (up 11) | ataque+generador |
| **Strike of Patrimony / Golpe del Patrimonio / 家业之击** | 1⚡ | At | 5 de daño +1 por cada 20 de Oro que tengas (máx +10). Glow. (up: por 15, máx +12) | Avaro (riqueza=poder) |
| **Caladbolg the Original / Caladbolg, el Original / 始源·光剑** | 2⚡ | At | 11 de daño a TODOS; +10 Estrellas. (up 14) | AoE −30% §2 |
| **Anticipated Shot / Disparo Anticipado / 先制射击** | 1⚡ | At | 9 de daño; si es la 1ª carta del turno: +20 Estrellas. Glow. (up 12/+30) | secuenciado |
| **Open Treasury Assault / Asalto del Tesoro Abierto / 宝库强袭** | 1⚡ | At | 6 de daño; +3 por cada Arma jugada este turno. Glow. (up 8/+4) | payoff de Armas |
| **Caster of Cu (Vajra) / Trueno de Vajra / 金刚杵之雷** | 2⚡ | At | 10 de daño a TODOS; +10 NP. (up 13) | AoE+NP |
| **KIT Charisma A+ / Carisma A+ / 卡里斯玛 A+** | 2⚡, Exhaust | Hab | 2 de Fuerza; +20 NP. (Co-op: aliados +1 Fuerza.) (up: 3/+30) | skill real S1, Exhaust=cooldown (regla Artoria) |
| **KIT Collector EX / Coleccionista EX / 收藏家 EX** | 1⚡, Exhaust | Hab | +50 Estrellas. (up: +50 y robá 1) | skill real S3 (acaparar YA) |
| **KIT Golden Rule A (Power) / Regla de Oro A (Poder) / 黄金律 A** | 1⚡ | Poder | Cada vez que ganás Oro en combate: +10 NP (máx 3/turno). (up: +15) | la skill real ES NP Gain up — cose Oro→NP |
| **Double Opening / Apertura Doble / 双重开门** | 1⚡ | Hab | Añade 2 Armas del Tesoro; +10 NP. (up: 3) | generador medio |
| **Arsenal Selection / Selección del Arsenal / 军械精选** | 1⚡ | Hab | Descubrí 1 de 3 Armas y añadí 2 copias. (up: cuesta 0⚡) | agencia sobre la tribu |
| **War Chest / Cofre de Guerra / 军资金柜** | 1⚡, Exhaust | Hab | Ganá 15 de Oro. (up: +25) | print de oro mediano (Exhaust = 1/combate) |
| **Thorough Appraisal / Tasación Exhaustiva / 详尽鉴定** | 1⚡ | Hab | Robá 2; +10 Estrellas. (up: robá 3) | ciclo |
| **Walls of Uruk / Murallas de Uruk / 乌鲁克城墙** | 2⚡ | Hab | 14 de Bloqueo; +10 NP. (up 18/+15) | pan y manteca defensivo |
| **Treasure Guards the King / El Tesoro Protege al Rey / 宝物护王** | 1⚡ | Hab | 5 de Bloqueo por cada Arma jugada este turno (máx 4). Glow. (up: 6/Arma) | defensa-payoff de tribu |
| **Impossible Bribe / Soborno Imposible / 不可能的贿赂** | 1⚡ | Hab | Pagá 15 de Oro: 18 de Bloqueo. Glow. (up: 24) | oro→defensa (sobre-tasa pagada §3) |
| **Decree of Confiscation / Decreto de Confiscación / 没收令** | 1⚡, Exhaust | Hab | El enemigo pierde 2 de Fuerza; ganá 10 de Oro. (up: 3/+15) | debuff+ingreso |
| **KIT Independent Action A+ / Acción Independiente A+ / 单独行动 A+** | 1⚡ | Poder | Tus ataques con Crítico Listo hacen +6 (antes del ×2). (up: +9) | pasiva real (Crit Damage) |
| **KIT Divinity B / Divinidad B / 神性 B** | 2⚡ | Poder | Tus Ataques hacen +2 de daño (incluye Armas del Tesoro). (up: +3) | pasiva real (Damage Plus) — sinergia tribu/multi-hit |
| **Permanent Gate / Puerta Permanente / 永驻之门** | 2⚡ | Poder | Al inicio de tu turno: añade 1 Arma del Tesoro. (up: 1⚡) | motor sostenido (patrón WinterCourt) |
| **War Economy / Economía de Guerra / 战争经济** | 1⚡ | Poder | Al final de tu turno: ganá 2 de Oro (máx 10/combate). (up: 3, máx 12) | ingreso pasivo CAPEADO |
| **Throne of the Onlooker / Trono del Observador / 旁观者之座** | 1⚡ | Poder | Al final de tu turno, si NO jugaste ninguna carta de Bloqueo: +10 Estrellas y +10 NP. (up: +15/+15) | la arrogancia como motor — la presión lo interrumpe (de B) |
| **Golden Arrogance / Arrogancia Dorada / 黄金傲慢** | 1⚡ | Poder | La 1ª vez que perdés Vida cada turno: +20 Estrellas. (up: +30) | plan B contra presión (lección 焰刑地狱) |

### 5.4 Raras (20)

| Carta (eng / esp / zhs) | Coste | Tipo | Efecto exacto | Arquetipo |
|---|---|---|---|---|
| **NP Enuma Elish / NP Enuma Elish / 天地乖离开辟之星** | 2⚡, Exhaust | At NP (mín. 70, consume TODA) | 24 a TODOS; +2 por cada 10 consumidos sobre 70; vs Élites/Jefes +12. Glow al ser pagable. (up: 30/+15) | disparar ANTES del auto-ulti |
| **NP Gate of Babylon: King's Barrage / Puerta de Babilonia: Andanada del Rey / 王之财宝·王之连射** | 1⚡ | At NP (mín. 40, consume TODA) | 4 de daño ×4 (aleatorio); añade 1 Arma; Sobrecarga +1 golpe por cada 10. (up: 5×4 / 2 Armas) | mini-NP de ciclo, gate 40 anti-spam |
| **KIT Treasury of Babylon EX / Tesorería de Babilonia EX / 巴比伦宝物库 EX** | 2⚡, Exhaust | Hab | +30 NP; +30 Estrellas; añade 1 Arma. (up: +50/+50/2 Armas) | el Interludio 3 real (Star Gather + batería) |
| **Chains of Heaven: Enkidu / Cadena del Cielo: Enkidu / 天之锁·恩奇都** | 2⚡, Exhaust | Hab | 2 de Vulnerable; vs Élites/Jefes: 4 de Vulnerable y el enemigo pierde 2 de Fuerza. Glow vs Élite/Jefe. (up: 3/5/3) | anti-divino del lore (más fuerte cuanto más divino) |
| **Babylon, Capital of Gold / Babilonia, la Capital del Oro / 黄金之都巴比伦** | 3⚡ | At | Daño = tu Oro ÷ 10 (máx 40). Glow. (up: ÷8, máx 50) | Avaro — no gastar también es build |
| **Full Gate Opening / Apertura Total de la Puerta / 全开之门** | 2⚡, Exhaust | Hab | Añade 5 Armas del Tesoro. (up: 6) | turno-burst de tribu (3 procs de starter) |
| **Complete Collection / Colección Completa / 全收藏** | 2⚡ | Poder | Cada vez que jugás un Arma: +5 NP y +5 Estrellas (máx 3/turno). (up: +8/+8) | duplica el dividendo de la starter — cap espejo 3/turno (riesgo #4) |
| **The King Collects / El Rey Recauda / 王之征收** | 2⚡ | Poder | Cuando un enemigo muere: ganá 10 de Oro (máx 30/combate). (up: +15, máx 45) | slot Hand of Greed, capeado |
| **Golden Throne / Trono Dorado / 黄金王座** | 2⚡ | Poder | Al inicio de tu turno: pagá 5 de Oro (si podés): robá 1 y +10 NP. Glow. (up: además +10 Estrellas) | convierte la billetera en motor por turno |
| **Collector's Absorption / Absorción del Coleccionista / 收藏家之汲取** | 0⚡, Exhaust | Hab | +100 Estrellas (dispara el umbral YA: Crítico Listo inmediato). (up: además +20 NP) | patrón 黑闪 — dispara el umbral en la misma jugada |
| **Conqueror's Loot / Botín del Conquistador / 征服者战利品** | 1⚡, Exhaust | Hab | Ganá 4 de Oro por cada Arma jugada este combate (máx 20). Glow. (up: 5, máx 25) | ingreso grande atado al motor |
| **Recital of Creation / Recital de la Creación / 开辟之咏唱** | 2⚡ | Poder | Cada vez que se manifiesta tu Enuma Elish (llegás a 100 NP): +1 Crítico Listo y robá 1. (up: 1⚡) | cose NP→crítico en el clímax |
| **Infinite Arsenal / Arsenal Infinito / 无限军械** | 3⚡ | Poder | Al inicio de tu turno: añade 2 Armas del Tesoro. (up: 2⚡) | stack con Puerta Permanente (redundancia de arquetipo, precedente Artoria) |
| **Richest in the World / El Más Rico del Mundo / 世界首富** | 2⚡ | Poder | Tus cartas que GANAN oro ganan +3 más; tus riders de PAGAR oro cuestan 5 menos. (up: +5/−10) | la rara que define el mazo económico |
| **Rain of Treasures / Lluvia de Tesoros / 宝物之雨** | 2⚡ | At | Pagá 25 de Oro: 10 de daño ×4 (aleatorio). Glow. (up: ×5) | el burst comprado (40-50 dmg por 2⚡+25 oro) |
| **Strike of Ea / Golpe de Ea / 乖离剑之击** | 2⚡ | At | 18 de daño; vs Élites/Jefes +8; +10 NP. Glow vs Élite/Jefe. (up: 24/+10) | Ea sin recitar — el preludio de Enuma |
| **Rain of a Thousand Blades / Lluvia de Mil Armas / 千刃之雨** | 0⚡ | At | Solo jugable con ≥50 Estrellas: consume 50 Estrellas; 25 de daño. Glow. (up: 32) | slot Comet (50★ ≈ 5★ vanilla); gastar retrasa el auto-Crítico — tensión real |
| **Tyrant's Armor / Armadura del Tirano / 暴君之铠** | 2⚡, Exhaust | Hab | 25 de Bloqueo; +10 NP. (up: 32) | slot Impervious (−5 bloqueo pagan el rider NP) |
| **Vimana Assault / Vimana de Asalto / 维摩那强袭** | 2⚡ | At | 16 de daño; robá 2; +10 NP. (up: 20/robá 3) | el trono volador; tempo rare |
| **Gram, Origin of All Swords / Gram, el Original de Todas las Espadas / 始源之剑·格拉墨** | 3⚡ | At | 30 de daño; +10 Estrellas. (up: 38/+20) | slot Bludgeon — el blanco soñado del Crítico ×2 (60-76) |

### 5.5 Especiales / Tokens

**ENUMA ELISH: Desatado** (ulti auto-manifestada — ver §6 abajo).

**ARSENAL — las 8 Armas del Tesoro** (tokens, todos 0⚡ Exhaust, `CardRarity.Special`, keyword «Arma del Tesoro», generados a mano, pool aleatorio ponderado; cada uno procesa la starter +10★ máx 3/turno; escalan con Divinidad B +2 y la reliquia El Prototipo de Todas las Cosas +3):

| # | Arma (eng / esp / zhs) | Tipo | Efecto |
|---|---|---|---|
| 1 | **Merodach, the Original Sword / Merodach, la Espada Original / 始源之剑·玛尔杜克** | At | 6 de daño [el prototipo de Caliburn; la vanilla del arsenal] |
| 2 | **Caladbolg / Caladbolg, la Espada Espiral / 螺旋剑·光剑** | At | 5 de daño; **ignora el Bloqueo** [perforante] |
| 3 | **Harpe / Hárpē, la Hoz Inmortal / 不死斩·哈尔贝** | At | 5 de daño; aplica 1 de Débil [niega la inmortalidad] |
| 4 | **Vajra / Vajra, el Rayo / 金刚杵** | At | 4 de daño a TODOS [el rayo de Indra — AoE] |
| 5 | **Durandal / Durandal, la Indestructible / 不毁之剑·杜兰达尔** | At | 6 de daño; +10 Estrellas |
| 6 | **Houtengeki / Houtengeki, la Alabarda Lunar / 方天戟** | At | 6 de daño; +10 NP |
| 7 | **Dáinsleif / Dáinsleif, la Espada Maldita / 咒剑·达因斯莱布** | At | 8 de daño; perdés 2 HP (`Unblockable\|Unpowered` — la espada exige sangre una vez desenvainada) |
| 8 | **Treasure Cuirass / Coraza del Tesoro / 宝库护甲** | Hab | 5 de Bloqueo [hasta defender es disparar el tesoro — plan B] |

**Estados FGOCore reutilizados:** `CritStarsPower` + `CritReadyPower` (auto-payoff a 100), `OverchargeBlessingPower` (Bendición de Sobrecarga), power oculto `ArmsPlayedPower` (contador «Armas jugadas turno/combate» para riders y Botín).

### 6. ENUMA ELISH: Desatado (ulti) — detalle

**ENUMA ELISH: Desatado / 天地乖离开辟之星·解放** — Ataque NP especial, auto-manifestada al cruzar **100** de Carga (patrón `GaugeFilled`); **0⚡, Retain, Exhaust**; mín. 100, **consume TODA** (`ConsumeAllForNpCard`); re-arme al caer bajo 100.

- **Daño: 30 a TODOS los enemigos.**
- **Contra Élites y Jefes: +15.**
- **SOBRECARGA (fidelidad EXACTA al NP real — «el daño base NO escala con OC; el multiplicador superefectivo SÍ»):** el bonus anti-Élite/Jefe aumenta **+4 por cada 20 consumidos sobre 100**. A consumo 300 (cap): 30 base / **75** contra Élite-Jefe (calca la curva OC100→500 = ×1.5→×2.0 usando el cap 300 de FGOCore como OC500). Contra «lo meramente humano» (enemigos comunes) SIEMPRE pega 30 plano.
- **Daño base escala SOLO con dupes:** `NpLevels.Scale` +15%/nivel (NP1→NP5 = 300%→500% del juego real).
- **Resultado:** contra salas normales la disparás a 100 (30 AoE cada ~2.5 turnos, el farmer AoE de FGO); contra Élite/Jefe **atesorás hasta 300** (30+75 dmg) — **la decisión de ahorro ES el Overcharge**. La **Bendición de Sobrecarga** (El Que Vio el Abismo, Decreto del Rey) suma al bonus anti-jefe: el plan de juego contra jefes es cargar de más ANTES de cruzar. Al manifestarse: swap visual a `200220` (Ea con relámpagos rojos).

---

## 6. Reliquias

| Reliquia (eng / esp / zhs) | Rareza | Efecto |
|---|---|---|
| **Bab-ilu, Key of the Treasury / Bab-ilu, la Llave del Tesoro / 巴比伦·宝库之钥** | **STARTER (motor)** | Al inicio de cada combate: añade 1 Arma del Tesoro aleatoria a tu mano. Cada vez que jugás un Arma del Tesoro: **+10 Estrellas (máx 3/turno, reset AfterSideTurnStart)**. [regla §4.6.4: evento del kit→recurso, cap 3/turno; garantiza la fuente pasiva que calibra todos los riders de tribu. También dispara la precarga de visuales en `BeforeCombatStartLate`. Icono: clase Archer DORADA 5★ (`Archergold.png&format=original`)] |
| **Oath of the King of Uruk / Juramento del Rey de Uruk / 乌鲁克王之誓约** | **STARTER (BondRelic)** | Vínculo estándar (+2/+3/+5 por victoria, +1 por sala) + herencia `ServantDamageMultiplier`/`ServantBlockMultiplier` **×1.25** (palanca §1.bis). Overrides: **Nv 4:** empezás cada combate con 1 Arma del Tesoro extra; **Nv 7:** +20 Carga NP inicial (Append 2 real, Load Magical Energy); **Nv 10 capstone «El Rey de los Héroes»:** empezás cada combate con 1 Crítico Listo. Nv 11-12 solo con el Cáliz. |
| **Catalog of the Royal Treasury / Catálogo de la Tesorería Real / 王室宝库目录** | **STARTER OCULTA (INpLevelStore)** | Dupes/NP level 1-5, pity 50%+25%, botón «Invocar (dupe)» (patrón MorganSummonSeal); +15%/nivel a las 3 cartas NP vía `NpLevels.Scale`. |
| **Ea, Sword of Rupture / Ea, la Espada de la Ruptura / 乖离剑·Ea** | **JEFE (Ancient, reemplaza a Bab-ilu)** | Todo lo de Bab-ilu, y las Armas jugadas también dan **+5 Carga NP** (mismo cap 3/turno); empezás con **2 Armas** en vez de 1. [el upgrade del motor: la tribu alimenta AMBOS medidores] |
| **Seal of the Golden Rule / Sello de la Regla de Oro / 黄金律之印** | **TIENDA** | La primera vez que pagás Oro en cada combate: +20 NP y +20 Estrellas. [rebaja efectiva del 1er pago; 1/combate = sin loop] |
| **King's Wine Cup / Copa de Vino del Rey / 王之酒杯** | **COMÚN** | Al final de cada combate: ganá 3 de Oro. [ingreso pasivo ~135/run ≈ Golden Idol; la inflación del Rey] |
| **Vimana, the Golden Throne / Vimana, el Trono Dorado / 维摩那·黄金王座** | **POCO COMÚN** | La primera vez que jugás 2+ Armas del Tesoro en un turno cada combate: robá 2. [tempo de tribu, 1/combate] |
| **Mantle of Arrogance / Manto de la Arrogancia / 傲慢之披风** | **POCO COMÚN** | Cada vez que un ataque tuyo con Crítico Listo mata a un enemigo: ganá 5 de Oro (máx 15/combate). [el juicio cobra botín; capeado] |
| **Blood of the Gods (Divinity B) / Sangre de los Dioses / 神之血 (神性 B)** | **POCO COMÚN** | Tus Armas del Tesoro hacen +2 de daño. [la pasiva como reliquia: el flat plus sobre el volumen] |
| **Magic Resistance E Amulet / Amuleto de Pacotilla / 对魔力 E 护符** | **TIENDA/PC (meme)** | El primer Débil que recibirías en cada combate se anula. [meme del rango E: solo la magia MENOR; «un rey no se digna a esquivar»] |
| **The Prototype of All Things / El Prototipo de Todas las Cosas / 万物之原典** | **RARA** | Tus Armas del Tesoro hacen +3 (daño o Bloqueo). [el buff de tribu: «coleccioné los modelos originales de toda tecnología» — literal] |
| **The Original Chalice / El Cáliz Original / 原典圣杯** | **RARA/EVENTO (ILimitBreaker)** | +15 HP máx; Vínculo hasta 12; NP level hasta 6. [«¿Un Santo Grial? Mestizo, tengo MILES» — el limit breaker estándar del roster con flavor de Gil] |

---

## 7. Stats del personaje

- **HP máximo: 72.** Ancla entre Silent 70 e Ironclad 80 (rango vanilla 66-80). Justificación §1.bis regla 5 (bajar el HP si el kit es explosivo): semidiós 2/3 pero **END C** y demasiado arrogante para defender — su defensa real es la Puerta, no su cuerpo; el pool tiene pocas Defensas. **NO 75** (B lo proponía pero el kit es más explosivo que defensivo: la economía permite picos de burst comprado).
- **Energía: 3⚡.** Mano inicial 5. Mazo inicial 10 cartas (QAABB literal, ver §5.1).
- **Oro inicial: 99 (ESTÁNDAR del roster).** **Esta es la corrección de fusión más importante.** Ambas propuestas pedían 149/150 ("nació destinado a poseer todo el oro"), pero ese +50 es exactamente la **perilla #1 de riesgo en ambos docs** (distorsiona el meta del run, compounding de reliquias en tienda). La identidad "Rey del Oro" la dan las CARTAS (Regla de Oro como básica, Copa de Vino del Rey común, el arquetipo Derroche entero), **no el stat inicial** — que es global del run y rompe el balance fuera del combate. Si el playtest lo pide, perilla **al alza** 99→125→149 (más seguro subir que bajar).
- **Carga NP 0-300, ulti auto a 100 con Retain** (FGOCore). **Estrellas de Crítico** compartidas FGOCore (auto-Crítico Listo a 100).
- **Starters:** Bab-ilu, la Llave del Tesoro + Juramento del Rey de Uruk (BondRelic ×1.25) + Catálogo de la Tesorería Real (dupes oculta).
- **Color/género:** dorado imperial sobre rojo vino (los relámpagos de Ea); Masculine.
- **Descripción (texto de selección):** «El Rey de los Héroes de Uruk, el transcendente completo que reunió todos los tesoros de la tierra. No combate con sus manos: la Puerta de Babilonia dispara los prototipos de toda arma jamás forjada, el oro de la run es su munición, y cuando los mestizos lo aburren, desenfunda a Ea y recita el viento de la creación que partió cielo y tierra. Difícil de jugar, imposible de igualar.»

**Justificación de balance (checklist §6):**
- **Tasa cruda vanilla** en todas las cartas (verificadas contra §2: 9-10/1⚡ común, 14/2⚡+rider, AoE −30%, Impervious 30→Armadura 25+rider, Bludgeon 32→Gram 30+10★, Shiv→Armas 4-8) + lift global ×1.25 por BondRelic heredado. Los MOTORES van al techo modded.
- **Sobre-tasa SIEMPRE pagada (§3):** riders «Pagá X de Oro» compran efecto con el recurso más fungible (25 oro ≈ ⅓ de un removal); generadores de oro con Exhaust o caps explícitos (Economía de Guerra máx 10, El Rey Recauda máx 30, Botín máx 20, Manto máx 15, Copa 3/combate). Ingreso neto realista de un mazo económico dedicado ≈ +10-25/combate ≈ Golden Idol+Hand of Greed de StS1, pagado en slots de mazo.
- **Round-trips deficitarios:** oro→NP→oro pierde 10 de oro; NP→oro→NP pierde 20 NP — sin loop de arbitraje. Sello de la Regla de Oro y Regla de Oro A capeados 1/combate y 3/turno.
- **Starter cap 3/turno:** máx +30★/turno por armas = 0.9 críticos — no se puede doble-crit-loopear ni con Apertura Total (5 armas = igual 3 procs). Colección Completa lleva cap espejo 3/turno (riesgo #4).
- **Enuma auditada:** a 100 = 30 AoE gratis (vs JeanneNP 80 ST gratis del mod de referencia — conservador); a 300 vs jefe = 75 (NP5: ~115) pagando ~9-10 cartas de generación y 2+ turnos sin ulti — dentro del ×1.5-2.0 de techo. OC fiel: solo el multiplicador escala.
- **Hextech-proof:** las Armas a 0⚡ son inmunes al hex de «3er ataque cuesta doble» (doble de 0 = 0).
- **Co-op:** Carisma/Abismo dan Fuerza a aliados (su rol real de buffer secundario; la ofensa propia NO escala con jugadores); el Oro es por jugador (VERIFICAR en implementación — si fuera compartido, fallback contador propio «Tesoro»); HP/defensa personal sin bonos de tanque que escalar.

---

## 8. Notas de arte / animación

- **Servant id 200200, collectionNo 12.** spriteModel por ascensión: asc 0 → `200200`; asc 1-2 → `200210`; asc 3-4 → `200220` (3 modelos únicos, todos verificados HTTP 200).
- **Modelos de batalla** (bundles `Modified Unity3D` = puppets 2D, NO spritesheets → re-rig en Godot, pipeline Mash DESIGN.md §7; texturas ~2048px):
  - `200210` (7.140.309 bytes) — **modelo en combate por defecto** (armadura dorada, asc 1-2, Gate activo: empuñaduras emergiendo de portales).
  - `200220` (7.068.380 bytes) — **swap cosmético con NP≥100 + animación de cast de la ulti** (torso desnudo, líneas/tatuajes rojos, Ea cilíndrica de 3 segmentos en mano, pelo erizado, ojos rojos, tormenta roja apocalíptica).
  - `200200` (6.464.524 bytes) — referencia/charui (rey ocioso, armadura completa).
  - Descargar con `tools/fetch_fgo_bundle.ps1 -Ids 200210 200220 -Texture` → GUI export (docs/ANIMATIONS.md §1) → `tools/render_all_<...>.ps1`. Precarga threaded en `BeforeCombatStartLate` desde la starter (gotcha del congelón de Morgan).
- **CharaGraphs:** `200200a@1` / `a@2` (asc 1-2) / `200200b@1` / `b@2` (asc 3-4). Select-bg = `200200b@2` (arte final, Enuma Elish). Sin charaGraphEx, sin costumes.
- **Faces:** `f_2002000`..`f_2002003`. **Iconos de skill** (deduplicados): `skill_00300` (Carisma A+ / El Que Vio el Abismo EX), `skill_00602` (Regla de Oro A), `skill_00311` (Collector EX / Tesorería de Babilonia EX). **Icono de carta NP:** `static.atlasacademy.io/JP/Servants/Commands/200200/card_servant_np.png` (HTTP 200).
- **Arte de cartas:** vía `match-ce-art.js`. CEs temáticos abundantes — **GilFest** (King's Festival), **Battle in New York** (Gil aparece), **Gate of Babylon**, CEs de oro/tesoro/Uruk. Dedup contra `mapping_morgan.csv` y `mapping_mash`.
- **VFX:** los portales dorados pueden reusar vfx existentes + el glow dorado de la tribu (`ShouldGlowGoldInternal`); Ea = relámpagos rojos. Validar paths contra el catálogo real del decompilado (gotcha de la carta congelada).

---

## 9. Naming trilingüe — CERRAR CON MOONCELL ANTES DE CREAR IDs (regla §4.7)

Terminología oficial 中文 simplificado (Mooncell `fgo.wiki`) — renombrar después rompe IDs:
- 英雄王 (Rey de los Héroes) · **王之财宝** / 王の財宝 (Gate of Babylon / Puerta de Babilonia) · **黄金律** / 黄金律 (Golden Rule / Regla de Oro) · **收藏家** (Collector / Coleccionista) · **巴比伦宝物库** (Treasury of Babylon / Tesorería de Babilonia) · **通晓深渊之人** (He Who Saw the Deep / El Que Vio el Abismo) · **天地乖离开辟之星** / 天地乖離す開闢の星 (Enuma Elish) · **乖离剑·Ea** (Ea, Sword of Rupture) · **天之锁** (Chains of Heaven / Cadena del Cielo) · **卡里斯玛** (Charisma) · **单独行动** (Independent Action) · **神性** (Divinity) · **杂种** / 雑種 («Mestizo» / Mongrel) · keyword **王之财宝·武具** (Treasure Arm / Arma del Tesoro).

---

## 10. Checklist de implementación

**FGOCore primero:** módulo `Arsenal/` (`ArsenalWeaponCard` + `Arsenal.AddRandom` + evento `WeaponPlayed` + keyword «Arma del Tesoro») → módulo `Gold/` (`CombatGold.Pay/Gain/CanPay` + `IGoldChangedListener` + `AfterCombatGoldChanged`, **verificar el cmd de oro de los Ladrones vanilla en el decompilado ANTES**) → variante de Sobrecarga multiplicativa (`Func<int, DamageContext, int>` parametrizado, sin tocar Morgan/Mash).
**Mod GilgameshArcher:** scaffold → HP 72, oro 99, mazo QAABB → starter Bab-ilu (+ Juramento BondRelic + Catálogo dupes) → ENUMA ELISH: Desatado (`ConsumeAllForNpCard` + Sobrecarga anti-RoomType + Retain + swap visual opcional) → 2 cartas-NP + arsenal de 8 tokens → pool (4 básicas + 20 comunes + 28 poco comunes + 20 raras) → poderes-kit reales (Carisma, Regla de Oro, Acción Independiente, Divinidad) → `ArmsPlayedPower` (contador, en el mod).
**Loc eng/esp/zhs** (§9, fijar ANTES de IDs). Correr `tools/audit_simpleloc.ps1` antes de CADA publish (escapar `/+`, `/-`, `/(`, `÷` — commit 9ce6bbe es la cicatriz).

**Perillas de playtest (en orden — NUNCA el daño base de Enuma):** 1. Oro inicial 99→125→149 (al alza). 2. Montos de ingreso (Copa 3→2, Cofre de Guerra 15→10, El Rey Recauda máx 30→20). 3. Costos de gasto (Derroche/Inversión). 4. Si Enuma se siente débil en salas comunes: subir base 30→34 ANTES de tocar el bonus anti-jefe (preservar la fidelidad del OC). Riesgos cerrados por código: cap 3/turno de starter Y Colección Completa (no degeneración de tribu), round-trips deficitarios (no arbitraje), API de oro con fallback «Tesoro» (riesgo técnico #1), pagos opcionales (todas las cartas «pagá X» tienen cuerpo base jugable — vigilar Soborno Imposible, la única excepción).