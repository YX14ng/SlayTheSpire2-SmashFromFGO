# PROPUESTA C — U-Olga Marie: «El Presupuesto de la Directora»

Construida encima del acta (`docs/DESIGN-UOLGA.md` §1-§8, cerrada) sin tocar ninguna decisión;
reglas de pool de `WORKFLOW-FGO.md` §4.6, gotchas §5, invariantes de `DECISIONS.md` y rúbrica de
revisión aplicadas. Lente: **el medidor NP como plata — la Directora firma gastos.**

## 1. Una frase + los 3 verbos

**La Directora administra el combate como un ejercicio fiscal: cada golpe que recibe se factura como ingreso, cada medidor lleno es una partida que HAY que ejecutar — en el gasto inmediato (Planet Olga Marie) o en la compra grande del presupuesto (la conversión a Autoridad) — y ninguna moneda queda sin asiento contable.**

Los 3 verbos del jugador:
- **FACTURAR** — convertir eventos del combate (golpes recibidos, ⚡ sobrante, muertes de Amenazas) en Carga NP y Estrellas.
- **CAMBIAR** — mover valor entre las dos monedas (NP ↔ ★) a tasas fijas y redondear hacia el múltiplo legal.
- **DECRETAR** — firmar el gasto del medidor entero: Desatada (gasto corriente, AoE ya) o Refrendo de Autoridad (inversión, daño diferido en Decretos).

Distinción de lente: la propuesta no agrega NI UNA mecánica al presupuesto del acta (Autoridad + Guts/F3 + Amenaza). Todo lo demás es **tabla de cambio** sobre recursos FGOCore ya existentes (NpCharge, CritStars, Evasión FGO, SkillSeal, Bloqueo de Curación, Anti-Purga de Artoria).

---

## 2. Mazo inicial de 10 (QAABB sesgado)

QAABB = 1 Quick / 2 Arts / 2 Buster, exacto al mazo canónico de §0 del acta.

| Carta | ⚡ | Tipo | Efecto | Mejora |
|---|---|---|---|---|
| **Buster** ×2 | 1 | Ataque | 10 de daño. | 13 |
| **Arts** ×2 | 1 | Ataque | 6 de daño; +30 Carga NP. | 9 / +30 |
| **Quick** ×1 | 1 | Ataque | 6 de daño; +30 Estrellas. | 9 / +30 |
| **Golpe / Strike** ×1 | 1 | Ataque | 6 de daño. *(mantiene vivo el tag Strike, lección P6 Morgan)* | 9 |
| **Defender** ×3 | 1 | Habilidad | 5 de Bloqueo. | 8 |
| **FIRMA — «Asignación de Fondos» / "Fund Allocation"** ×1 | 1 | Habilidad | **Elegí una: +30 Carga NP o +30 Estrellas.** *(la lente entera en una carta: desde el turno 1 el jugador ASIGNA partidas, no recibe un número)* | elegí +50 |

- 44 de daño en comandos + 15 de Bloqueo: **gana el Acto 1 sin motor** ✓. Primer ulti esperado turno 3-4 (2× Arts + firma + starter).
- Las 3 básicas siguen el estándar §4.6.1 al número; la firma es deliberadamente la carta más simple del kit que enseña la decisión central (¿a qué caja va la plata?).
- HP base **70** (banda 70-72 del acta; elijo el piso: el kit factura golpes, no los tanquea — ver auto-crítica #2).

---

## 3. Starter relic — «Informe de Daños y Perjuicios» / "Damage Assessment Report"

**Evento universal → recurso:** *cada vez que un golpe ENEMIGO te hace perder Vida: **+10 Carga NP**.* **Cap 3 procs/turno**, reset en `BeforeSideTurnStart` y sólo si `participants.Contains(Owner)` (regla §4.6.4 + DECISIONS de estado efímero).

- **Justificación canónica directa** (§0 del acta): «recibir daño → 3% NP». Es la pasiva real de U-Olga traducida a la gramática del proyecto.
- **Doble función de sistema**: la starter también asienta el marcador de **Amenaza** en `BeforeCombatStartLate` (jefes siempre, Estrella siempre, élites fallback — §4 del acta) y materializa los appends como estado de run (§6 del acta: la starter «ya viaja sincronizada»).
- **Candado de auditoría (explícito, no negociable en esta propuesta):** dispara SÓLO con golpes de origen enemigo. La Vida pagada a la reliquia de jefe, a «Fondos de Emergencia» o a cualquier autodaño **no factura** — sin esto, Vida→NP→Vida sería circuito (ver §7).
- Coherencia con el Guts (§3 del acta): exponerse a la Amenaza que puede armarte la Forma 3 **también cobra** +10 NP por golpe. El motor y la transformación empujan la misma decisión de exposición, sin regla extra.
- Icono: clase **Beast oro 5★** (regla del workflow §6). Índice 0 de `StartingRelics` + `GetUpgradeReplacement()` → su Ancient es la reliquia de jefe (§5, cumple Touch of Orobas de DECISIONS).

---

## 4. Pool de 68 recompensas (20 / 28 / 20)

Gramática: denominaciones **10/20/30/50/100** en NP/Estrellas; glow dorado en TODA condicional; Amenaza/umbral-NP/CRÍTICO LISTO/Forma 3 son los gates visibles. Conversión FGOCore: ★ a 100 → CRÍTICO LISTO (auto ×2, modelo Morgan/Jeanne).

### 4.1 Comunes (20) — engranajes de conversión

| Nombre ES | Nombre EN | Tipo | ⚡ | Efecto | Mejora | Recurso que lee/escribe |
|---|---|---|---|---|---|---|
| Nota de Cargo | Debit Note | At | 1 | 9 de daño; +10 NP. | 12 / +10 | escribe NP |
| Timbrado | Stamp Duty | At | 1 | 6 de daño; +20 Estrellas *(starGen 98 canónico)*. | 9 / +30 | escribe ★ |
| Caja Chica | Petty Cash | Hab·Exhaust | 0 | +20 NP. | +30 | escribe NP |
| Cambio de Ventanilla | Teller Window | Hab | 0 | si ≥50 NP: −50 NP → +50 Estrellas. Glow. | consume 40 | NP→★ (espejo A) |
| Recaudación | Revenue Collection | Hab | 0 | si ≥50 Estrellas: −50 ★ → +50 NP. Glow. | consume 40 | ★→NP (espejo B) |
| Arancel | Tariff | At | 1 | 8 de daño; contra **Amenaza**: +4. Glow. | 11 / +6 | lee Amenaza |
| Partida de Defensa | Defense Appropriation | Hab | 1 | 6 de Bloqueo; +10 NP. | 9 / +10 | escribe NP |
| Retención | Withholding | Hab | 1 | 5 de Bloqueo; +10 Estrellas. | 8 / +20 | escribe ★ |
| Balance Diario | Daily Ledger | Hab | 1 | robá 2; +10 NP. | robá 2 / +20 | escribe NP |
| Viáticos | Per Diem | Hab | 0 | robá 1; +10 Estrellas. | +20 | escribe ★ |
| Auditoría Sorpresa | Surprise Audit | At | 1 | 5 de daño ×2; +5 NP por golpe que dañe Vida. | 7×2 | escribe NP |
| Orden de Clausura | Closure Order | At | 1 | 4 de daño a TODOS; +10 NP. | 6 | AoE + escribe NP |
| Sello de la Dirección | Directorate Seal | Hab | 1 | 8 de Bloqueo; si ≥50 NP: +4. Glow. | 11 / +5 | lee NP |
| Fondo de Reserva | Reserve Fund | Hab | 1 | +20 NP y +10 Estrellas. | +30 / +20 | escribe NP+★ |
| Amonestación | Written Warning | At | 1 | 8 de daño; si ≥50 NP: +4. Glow. *(ahorrar también pega)* | 11 / +5 | lee NP |
| Recorte de Gastos | Budget Cut | Hab | 1 | 1 Débil a un enemigo; +10 NP. | 2 Débil / +10 | escribe NP |
| Expediente 444 | Case File 444 | Hab | 0 | robá 1; +5 NP. | robá 1 / +10 | escribe NP |
| Horas Extra | Overtime | Hab | 1 | 4 de Bloqueo; robá 1; +5 NP. | 7 | escribe NP |
| Creación de Territorio A | Territory Creation A | Hab | 1 | 9 de Bloqueo. *(陣地作成 A canónico — el meme espejado del E− de Oberon: mismo skill, siete rangos más)* | 12 y +5 NP | — (meme permitido) |
| Trámite Urgente | Expedited Processing | At | 0 | 4 de daño; +5 NP. | 6 / +10 | escribe NP |

**Conectividad: 19/20 = 95% ✓** (sólo Territorio A desconectada, a propósito, y su upgrade la conecta). Pares espejo 0⚡ a 50↔50 presentes (regla §4.6.2): ningún medidor se estanca.

### 4.2 Poco comunes (28)

| Nombre ES | Nombre EN | Tipo | ⚡ | Efecto | Mejora | Recurso que lee/escribe |
|---|---|---|---|---|---|---|
| KIT Sin Precedentes EX (空前絶後) | Unparalleled EX | Hab | 1 | descartá tu mano y robá esa cantidad; +10 Estrellas por carta descartada (máx +30); tu próximo robo de turno: **−2 cartas**. *(la inversión a demérito de §5 del acta, riders tal cual)* | −1 carta | escribe ★ / lee mano |
| Intervención de la Dirección | Directorate Intervention | Hab | 1 | **Sello de Habilidad** a TODOS (1 turno); +10 NP. *(rider SkillSeal reutilizado, §5 del acta)* | y Bloqueo de Curación 2 | escribe NP |
| Fondos de Emergencia | Emergency Funds | Hab·Exhaust | 1 | perdés 5 Vida (imparable; **no alimenta nada**): +50 NP. *(tasa legal 1:10, espejo chico de la reliquia de jefe; misma cláusula anti-Morgan del acta)* | perdés 4 | Vida→NP |
| PODER Planta Atómica B (アトミックプラント) | Atomic Plant B | Poder | 2 | al inicio de tu turno: +10 NP. *(H2 de F3, canon)* | 1⚡ | escribe NP |
| PODER Superávit | Budget Surplus | Poder | 1 | al final de tu turno: +10 NP por cada ⚡ sin gastar (máx +30). *(el presupuesto no ejecutado se transfiere)* | y +5 ★ por ⚡ | ⚡→NP |
| Licitación | Public Tender | Hab·Exhaust | 1 | elegí: +50 NP **o** +50 Estrellas. | y robá 1 | escribe NP o ★ |
| Doble Imposición | Double Taxation | At | 2 | 14 de daño; +10 NP y +10 Estrellas. | 18 | escribe NP+★ |
| Embargo | Asset Seizure | At | 1 | 9 de daño; contra **Amenaza**: +20 Estrellas. Glow. | 12 / +30 | lee Amenaza, escribe ★ |
| Multa Ejemplar | Exemplary Fine | At | 1 | 12 de daño; si ≥50 NP: +10 Estrellas. Glow. | 16 | lee NP, escribe ★ |
| PODER Nómina de Chaldea | Chaldea Payroll | Poder | 1 | la primera Habilidad que jugás cada turno: +10 NP. | y +10 ★ | escribe NP |
| Rendición de Cuentas | Statement of Accounts | Hab | 1 | 8 de Bloqueo; robá 1; +5 NP. | 11 | escribe NP |
| Peso Crítico 99 | Critical Weight 99 | At | 1 | 8 de daño; si este Ataque critica: +20 NP. Glow con CRÍTICO LISTO. *(cose ★→crit→NP)* | 11 / +30 | lee CRÍTICO, escribe NP |
| Cobertura de Seguro | Insurance Coverage | Hab | 1 | 10 de Bloqueo; la próxima vez este turno que un golpe enemigo te quite Vida: +20 Estrellas. | 13 / +30 | amplifica el evento starter |
| PODER Grilla de Detección | Threat Detection Grid | Poder | 1 | tus Ataques hacen +2 contra **Amenazas**; cuando muere una Amenaza: +30 NP. | +3 / +50 | lee Amenaza, escribe NP |
| Anexo Antártico | Antarctic Annex | Hab | 2 | 16 de Bloqueo; +10 NP. | 20 / +20 | escribe NP |
| Orden de Desalojo | Eviction Order | At | 2 | 8 de daño a TODOS; +10 Estrellas. | 11 / +20 | AoE, escribe ★ |
| Subsidio | Subsidy | Hab | 1 | +30 NP; robá 1. (Co-op: aliados +10 NP — rider extra, nunca el cuerpo de la carta.) | +50 | escribe NP |
| PODER Doctrina de la Protectora | Sentinel Doctrine | Poder | 2 | cuando un golpe enemigo te hace perder Vida: +10 Estrellas (**máx 3/turno**, contador propio). | 1⚡ | golpe→★ (2.º motor de exposición) |
| Reasignación de Partidas | Budget Reallocation | Hab | 0 | convertí hasta 30 NP en Estrellas o hasta 30 Estrellas en NP (1:1). Glow. | hasta 50 | NP↔★ flexible |
| Amortización | Amortization | At | 1 | 6 de daño ×2; +5 NP por golpe que dañe Vida. | 8×2 | escribe NP |
| Redondeo a Favor | Round Up | Hab·Exhaust | 0 | tu Carga NP sube al **próximo múltiplo de 50**. Glow si no estás en múltiplo. *(la carta más de contadora del pool)* | y +10 ★ | escribe NP |
| Auditoría General | General Audit | Hab | 2 | robá 3; +10 NP. | robá 4 | escribe NP |
| KIT Ultra Manifest EX (F3) | Ultra Manifest EX | Hab·Exhaust | 1 | +30 NP; en **Forma 3**: además +30 Estrellas. Glow en F3. *(usable siempre; mejor tras levantarse — nunca carta muerta)* | +50 base | lee forma |
| Custodia de la Humanidad | Custody of Humanity | Hab | 1 | 6 de Bloqueo; si hay una **Amenaza** en el combate: +10 NP y +10 Estrellas. Glow. | 9 | lee Amenaza |
| Contrapartida | Matching Funds | Hab | 1 | +20 Estrellas; si ≥50 NP: +20 más. Glow. | +30 base | lee NP, escribe ★ |
| Gasto Corriente | Operating Expenses | At | 0 | 5 de daño; +5 Estrellas. | 8 / +10 | escribe ★ |
| PODER Divinidad EX (神性) | Divinity EX | Poder | 2 | tus Ataques hacen +2 **por golpe** (patrón DivinityPower; aplica a Decretos y a los 5 hits del NP). | +3 | amplifica sumideros |
| Protocolo Antártico | Antarctic Protocol | Hab·Exhaust | 1 | ganás 1 **Evasión** (FGO compartida, máx 3); +10 NP. | y +20 ★ | escribe NP |

### 4.3 Raras (20)

| Nombre ES | Nombre EN | Tipo | ⚡ | Efecto | Mejora | Recurso que lee/escribe |
|---|---|---|---|---|---|---|
| NP Planet Olga Marie (drafteable) | Planet Olga Marie | At NP·Exhaust | 2 (mín 70, consume TODA) | 6 de daño ×5 a TODOS; SOBRECARGA: +1 por golpe por cada 20 sobre el mínimo; contra **Humano**: +50%; contra **Estrella**: +20%. Glow al gate. | 7×5 | consume NP entero |
| PODER Autoridad Delegada | Delegated Authority | Poder | 2 | tus **Decretos** hacen +5 de daño. Glow. *(lee Autoridad; escribe SÓLO daño — candado de loop)* | +8 | lee Autoridad |
| PODER Escolta de la Directora | Director's Escort | Poder | 1 | cuando jugás un **Decreto**: 6 de Bloqueo. *(cubre los turnos de cobro del calendario de 5)* | 9 | lee Autoridad |
| Sesión Extraordinaria | Extraordinary Session | Hab·Exhaust | 1 | +50 NP y +50 Estrellas. | +60 / +60 | escribe NP+★ |
| Decreto de Necesidad y Urgencia | Emergency Executive Order | At·Exhaust | 2 | 20 de daño; contra **Amenaza**: +10. Glow. | 26 / +14 | lee Amenaza |
| PODER Presupuesto de Guerra | War Budget | Poder | 2 | cuando jugás una carta que da Carga NP: 4 de daño a un enemigo aleatorio. *(el antídoto estructural al solitario: generar YA pega)* | 6 | NP-play→daño |
| PODER Secretaría Técnica | Technical Secretariat | Poder | 2 | al inicio de tu turno: robá 1 adicional. | 1⚡ | consistencia |
| Contraluz (逆光) | Backlight | At | 1 | 10 de daño; en **Forma 3**: +10 y +20 Estrellas. Glow. | 14 | lee forma, escribe ★ |
| Amenaza Neutralizada | Threat Neutralized | At | 2 | 16 de daño; si mata a una **Amenaza**: +100 NP. Glow. | 22 | lee Amenaza, escribe NP |
| PODER El Ojo de la Directora | The Director's Eye | Poder | 1 | al inicio de tu turno: +20 Estrellas. *(peso de crítico 99 canónico)* | +30 | escribe ★ |
| Intervención Total | Total Intervention | At | 3 | 12 de daño ×3; +10 NP. *(multi-hit anti-Buffer, regla del techo)* | 14×3 | escribe NP |
| Cierre de Ejercicio | Closed Fiscal Year | Hab·Exhaust | 1 | tu Carga NP sube al **próximo múltiplo de 100** (máx +40). Glow si estás a ≤40. | y +20 ★ | escribe NP |
| PODER Partida Blindada | Armored Appropriation | Poder | 2 | al final de tu turno: 3 de Bloqueo por cada 50 de Carga NP que tengas (máx 9). *(ahorrar defiende: tensión directa con gastar)* | 4 por 50 (máx 12) | lee NP, escribe Bloqueo |
| Requisa | Requisition | At | 2 | 14 de daño; +20 NP y +20 Estrellas. | 18 | escribe NP+★ |
| La Firma de la Directora | The Director's Signature | Hab·Exhaust | 2 | +100 Estrellas (→ CRÍTICO LISTO ya). | 1⚡ | escribe ★ al umbral |
| Censo Final | Final Census | At | 2 | 9 de daño a TODOS; contra **Humano**: +50%. Glow en salas Monster. *(el guiño anti-pasto del NP, en carta)* | 12 | AoE, lee atributo |
| Moción de Censura | Motion of Censure | Hab·Exhaust | 2 | Sello de Habilidad 2 turnos a un enemigo; Bloqueo de Curación 3; +20 NP. | 3 turnos | escribe NP |
| Escudo de la Humanidad | Shield of Humanity | Hab·Exhaust | 2 | ganás 2 **Evasión**; +10 NP. | y +30 ★ | escribe NP |
| Dividendos | Dividends | At | 1 | 8 de daño; +2 por cada 20 Estrellas que tengas (máx +10). Glow. | máx +16 | lee ★ |
| El Informe Final | The Final Report | Hab·Exhaust | 3 | +100 Carga NP. Glow. *(el jackpot pagado en ⚡ y Exhaust — el pago de U-Olga es siempre inmediato, sin cuotas)* | 2⚡ | escribe NP |

**Ataques directos: 22/68 (~32%)** + los dos sumideros grandes (Desatada, Decretos). Es el share más bajo del roster y es deliberado — ver §8 y auto-crítica #3.

---

## 5. Las 12 reliquias

| # | Rareza | Nombre ES / EN | Efecto |
|---|---|---|---|
| 1 | **STARTER motor** | «Informe de Daños y Perjuicios» / Damage Assessment Report | §3 completo: golpe enemigo que te quita Vida → +10 NP (cap 3/turno, reset `BeforeSideTurnStart`); asienta Amenazas en `BeforeCombatStartLate`; materializa appends. Icono Beast oro. Índice 0 + `GetUpgradeReplacement()` → #4. |
| 2 | **STARTER Bond CE** | «El Deber de la Directora» / The Director's Duty *(nombre canónico del Bond CE de collectionNo 444 a verificar en Atlas antes de implementar — no lo invento como hecho)* | `BondRelic` estándar: ×1.25 daño/bloqueo heredado (la palanca central, sin ×global por carta). Nv4: +10 NP al iniciar combate. Nv7: +10 NP y +20 Estrellas. Nv10 capstone «人理の防人»: al iniciar combates contra **Amenazas**: 8 de Bloqueo y +10 NP. |
| 3 | **STARTER oculta** (`INpLevelStore`) | «Archivo de Trismegistus II» / Trismegistus II Archive | dupes/NP level 1-5, pity estándar; +15%/nivel a cartas NP (`NpLevels.Scale`). |
| 4 | **ANCIENT / JEFE (§5 del acta — 驚天動地 B)** | «Conmoción de Cielo y Tierra» / Earth-Shattering B | Reemplaza físicamente a #1 (Orobas) conservando su motor (golpe→+10 NP, cap 3) y agrega: **activable 1 vez por combate — llenás tu medidor pagando Vida imparable: 1 Vida por cada 10 faltante hasta 100; por encima de 100, 1 Vida por cada 5** (el excedente hacia 300 sale más caro, número de arranque del acta). La Vida pagada **no alimenta nada** (ni la starter, ni Doctrina, ni Cobertura). Comprás llegar antes a la decisión de §2 del acta, nunca más total. El append 技能再装填 la re-arma. |
| 5 | TIENDA | «Sello Fiscal» / Fiscal Stamp | la **primera** carta que da Carga NP cada turno da +10 más. *(el «primera por turno» es el candado: sin él, los espejos harían un goteo neto — ver auditoría C1)* |
| 6 | POCO COMÚN | «Insignia de la Dirección» / Directorate Badge | al morir una **Amenaza**: +30 NP y +30 Estrellas. |
| 7 | POCO COMÚN | «Manual de Procedimientos» / Procedures Manual | la primera Habilidad que jugás cada turno: robá 1. |
| 8 | POCO COMÚN | «Garantía de Cumplimiento» / Performance Bond | al **convertir a Autoridad**: ganás 10 de Bloqueo por carga obtenida. *(paga el hueco de tempo del turno de conversión con defensa, nunca con daño ni recursos)* |
| 9 | RARA | «Autorización Nivel EX» / Clearance Level EX | tus **Decretos** hacen +3 de daño. *(sólo daño: apilable con Delegada y el append sin tocar recursos)* |
| 10 | RARA | «Radar de Amenazas» / Threat Radar | al iniciar combates contra **Amenazas**: +30 NP y 6 de Bloqueo. |
| 11 | RARA | «Fondo Rotatorio» / Revolving Fund | al ganar un combate con ≥50 de Carga NP sin gastar: +15 de Oro. *(el superávit se rinde a caja — sabor puro, fuera del combate, inauditable por diseño)* |
| 12 | EVENTO (`ILimitBreaker`, Grial Acto 2) | «Santo Grial de la Dirección» / Holy Grail of the Directorate | +15 HP máx; Vínculo hasta Nv12; NP level hasta 6; **repara 単独顕現** (acta §6): empezás cada combate con +10 NP (la manifestación independiente recuperada). |

---

## 6. Carta-NP + Desatada + la carta Event de conversión

### «Planet Olga Marie: Desatado» (auto a 100, `GaugeFilled`)
- Ataque NP, **0⚡, Exhaust**: **7 de daño ×5 a TODOS**; consume TODA la carga (`ConsumeAllForNpCard`). **SOBRECARGA: +1 por golpe por cada 20 sobre 100.** Contra **Humano** (salas Monster, convención `FgoAttributes`): **+50%** — el peso real del special, inédito en el roster. Contra **Estrella**: +20% (huevo de pascua, nunca línea de balance). +15%/nivel `NpLevels`. El rider «−20% resistencia Q/A/B» queda pospuesto (acta §5).
- La rara drafteable (tabla 4.3) es la versión 2⚡/mín 70, ratio idéntico al precedente Oberon.

### «Refrendo de Autoridad» (アルテミット・U / 天衣無縫 fusionados — la carta `Event` del §2 del acta)
- `CardRarity.Event`, Habilidad, **0⚡, mín 100, Exhaust**, pasa por `ConsumeAllForNpCard`, **no hace daño**.
- **A 100 se manifiestan LAS DOS** (Desatada + Refrendo) vía `ManifestCards`: la decisión de §1 del acta es literal, dos cartas en la mano, una firma posible.
- Efecto: **1 carga de Autoridad por cada 50 de tier consumido** (100→2, 150→3, 200→4, 250→5, 300→6→**cap 5**). Los Decretos llegan YA a la mano, **máx 1/turno**, **expiran a los 5 turnos**. Números del acta sin tocar.
- En Forma 3 la carta muestra el arte/nombre de アルテミット・U; misma mecánica.

### «Decreto de la Directora» / "Director's Decree" (el token único)
- Ataque, **0⚡**, Exhaust, no drafteable, arte de command card oficial por ascensión.
- **Daño = tier convertido ÷ 10** (100→10, 150→15, 200→20, 250→25, 300→30). La única variable es la magnitud, como manda el acta.
- **En Forma 3: AoE con daño por objetivo a la mitad, completo contra enemigo único** (canon Extra Attack de §0).
- Candados del acta implementados tal cual: **no genera NP**, `IsFirstInSeries` (no dispara riders de "jugaste un Ataque", patrón EchoForm), no re-dispara `CommandBonusPower`. **SÍ** recibe Fuerza/Divinidad/lecturas explícitas de Decreto (Delegada, Escolta, Autorización EX, append +3/+6) — está contemplado en la cuenta del pico.

---

## 7. La tabla de cambio + auditoría de ciclos

### Tabla de cambio (todas las conversiones del pool)

| Entra | Sale | Tasa | Canal | Límite |
|---|---|---|---|---|
| 1⚡ (carta) | 30 NP + 6 daño | 30 NP/⚡ | Arts básica y familia | por carta |
| 1⚡ (carta) | 30 ★ + 6 daño | 30 ★/⚡ | Quick básica y familia | por carta |
| golpe enemigo recibido | 10 NP | fija | **starter** | **3/turno** |
| golpe enemigo recibido | 10 ★ | fija | Doctrina de la Protectora | **3/turno** (contador propio) |
| ⚡ sin gastar | 10 NP c/u | fija | Superávit | máx 30/turno |
| 50 NP | 50 ★ | 1:1 | espejo A (Ventanilla) | 1 carta+robo por uso |
| 50 ★ | 50 NP | 1:1 | espejo B (Recaudación) | 1 carta+robo por uso |
| ≤30 NP ↔ ≤30 ★ | 1:1 | Reasignación de Partidas | 1 carta por uso |
| 5 Vida (imparable) | 50 NP | 1:10 | Fondos de Emergencia | **Exhaust**; la Vida no alimenta nada |
| Vida (imparable) | llenar medidor | 1:10 hasta 100; 1:5 arriba | **reliquia de jefe** | 1/combate; la Vida no alimenta nada |
| tier NP entero | Autoridad | **50:1, cap 5** | Refrendo (Event) | acta §2, inmutable |
| 1 Autoridad | tier÷10 de daño | fija | Decreto | 1/turno, expira a 5 turnos |
| 100 ★ | CRÍTICO LISTO (×2) | FGOCore | automático | motor compartido |
| muerte de Amenaza | 30 NP / 30-50 NP+★ | fija | Grilla / Insignia / Amenaza Neutralizada | enemigos finitos |
| NP ahorrado | Bloqueo | 3 por 50, máx 9 | Partida Blindada | fin de turno |

**Reglas de sentido único (invariantes de la propuesta):** (a) **nada** convierte NP/★/daño → ⚡ ni → robo gratuito; (b) **nada** convierte recursos → Vida (cero curación por recursos en el pool; la única curación es fogata/Bond regen); (c) **ningún power dispara "al ganar NP/★"** genéricamente — los triggers son por evento discreto (jugar carta, golpe recibido, muerte); (d) el token no produce recursos de ningún tipo.

### Auditoría de ciclos cerrados (cada uno termina en pérdida neta)

| Ciclo | Recorrido | Neto por vuelta | Veredicto |
|---|---|---|---|
| **C1** espejos | 50 NP → 50 ★ → 50 NP | recursos **0**; costo: 2 cartas jugadas + 2 robos consumidos, 0⚡ | **pérdida** (economía de cartas). Con «Sello Fiscal»: +10 NP **una vez por turno** — igual a jugar cualquier generadora; no compone. |
| **C2** inversión | ⚡→NP→Refrendo→Decretos→daño | el daño no vuelve a ningún recurso (candados del token; «Presupuesto de Guerra» dispara por *dar NP*, y el Decreto no da NP) | **sentido único** ✓ |
| **C3** sangre | Vida→NP (relic/Fondos) → ¿NP→Vida? | no existe NP/★→Vida en el pool; y la Vida pagada no factura en starter/Doctrina/Cobertura (sólo golpes enemigos) | **circuito abierto** ✓ — cláusula del acta §5 respetada y extendida |
| **C4** crítico | ★→CRÍTICO→daño (+«Peso Crítico 99»: crit→+20 NP) | 100★ → ×2 en un ataque → +20 NP. 100★ costaron ≥3⚡/cartas; vuelven 20 NP: tasa de retorno 0,2 — muy por debajo de cualquier generadora directa | **pérdida** ✓ |
| **C5** energía | ⚡ sobrante→NP (Superávit) → ¿NP→⚡? | inexistente («El Informe Final+» baja su PROPIO coste, fijo, no es conversión) | **sentido único** ✓ |
| **C6** redondeos | Redondeo/Cierre de Ejercicio | +NP «gratis» pero **Exhaust** — acotado a 1 uso por copia por combate, sin ciclo posible | **acotado** ✓ |
| **C7** Autoridad→defensa | Decreto→Escolta/Garantía→Bloqueo | el Bloqueo no se convierte en nada en todo el pool | **sentido único** ✓ |

**Conclusión de auditoría: no existe ningún ciclo neto positivo en recursos ni en cartas.** Los dos únicos «+X de la nada» (Redondeo, Cierre) llevan Exhaust; los dos motores de golpe-recibido tienen cap 3/turno cada uno con contadores separados (perilla de playtest: si apilarlos resulta grosero, unificar el contador como en Oberon #5).

---

## 8. La cuenta del pico + ¿mata cosas o cuenta plata?

**Setup razonable de Acto 3** (sin appends, como manda el acta): Forma 3 activa (逆光 al levantarse: **+3 Fuerza**, 1 Anti-Purga —reuso Artoria—, +30 NP), poderes en mesa **Divinidad EX** (+2/golpe) y **Autoridad Delegada** (+5 Decreto), reliquia de jefe armada, Decretos en calendario de una conversión previa a **250** (Decreto base 25), CRÍTICO LISTO banqueado, sala con Amenaza + acompañantes Humanos.

Turno pico (3⚡):
1. **Decreto** (0⚡): 25 +5 (Delegada) +3 (Fuerza) +2 (Divinidad) = **35** (AoE en F3 vs grupo, completo vs único).
2. **Reliquia de jefe** (0⚡): medidor 0 → 150 pagando 20 Vida imparable (10 + 10 del excedente).
3. **Desatada @150** (0⚡): (7+2 sobrecarga+3+2)×5 = 70 → contra Humano ×1,5 = **105** AoE.
4. **Contraluz+** (1⚡): 14+10 (F3) +3+2 = 29 → CRÍTICO ×2 = **58** (+20 ★ de vuelta).
5. **Doble Imposición+** (2⚡): 18+3+2 = **23**.

**Total: ~221 contra Humano en el caso perfecto; ~185-200 en la versión sin crítico banqueado o sin el ×1,5.** Entra en el techo 180-220 tocando el borde superior sólo con la tormenta perfecta; **perillas declaradas si el juez lo pide**: Delegada +5→+4, sobrecarga +1/20→+1/30, o Fuerza de 逆光 3→2. Nunca el daño base del NP.

**¿Este personaje mata cosas o sólo cuenta plata?** Mata, y en tres registros: (1) **ritmo corriente** — turno medio de Acto 1-2 sin motor: Nota de Cargo + Timbrado + Arancel ≈ 23-27, curva vanilla; (2) **el pasto** — la Desatada cada 3-4 turnos limpia salas Monster con el +50% anti-Humano (nadie más del roster premia eso); (3) **el calendario** — 5 Decretos de una conversión a 250 son 125 de daño garantizado repartido en 5 turnos **que sobrevive a cualquier limpieza de buffs** (es un tanque de cartas, no un power — la respuesta estructural del acta §9 a jefes que strippean). El 32% de Ataques directos es bajo a propósito: casi todo lo demás convierte HACIA los dos sumideros de daño, no lejos de ellos, y «Presupuesto de Guerra» convierte el acto mismo de facturar en chip damage.

**Cobertura rápida (rúbrica):** frontload (Caja Chica, Trámite, reliquia de jefe) · defensa (7 cartas de Bloqueo + Evasión ×2 + Anti-Purga de 逆光 + Partida Blindada + Escolta/Garantía) · consistencia (Balance, Viáticos, Expediente, Auditoría General, Secretaría, Manual) · economía (0⚡ ×8, Superávit) · escalado (Autoridad, Divinidad, Delegada, Ojo) · multiobjetivo (Clausura, Desalojo, Censo, NP, Decreto F3) · jefes anti-buff (Decretos + medidor + Estrellas son estado no-buff).

---

## 9. Auto-crítica honesta (lo más frágil de ESTA propuesta)

1. **El turno del Refrendo puede ser siempre-incorrecto.** Convertir a 100 rinde 2 Decretos (10+10 en 2 turnos) contra una Desatada de ~35 AoE inmediata + el especial anti-Humano. A tiers bajos la conversión es matemáticamente pobre y a tiers altos exige aguantar hasta 200-250 sin gastar — el riesgo de que Autoridad sea o trampa para novatos o botón que el jugador óptimo nunca toca es real. «Garantía de Cumplimiento» y Partida Blindada pagan la espera en Bloqueo, pero **la tasa 50:1 y el daño tier÷10 son LOS números que el playtest tiene que golpear primero** (dentro del cap 5 y el 1/turno del acta, que no se tocan).
2. **La starter factura golpes en un cuerpo de 70 HP cuyo clímax exige morir.** El kit empuja dos direcciones de Vida a la vez: exponerse rinde NP (y arma el Guts contra Amenazas), pero la transformación cuesta −10 MaxHP permanente y el pool tiene **cero curación por candado anti-loop**. Una run que facturó golpes todo el Acto 2 puede llegar al jefe sin colchón para «dejarse matar bien». Si el playtest muestra espiral, la válvula es defensa (subir Escudo de la Humanidad / Anexo), nunca curación por recursos.
3. **Densidad contable en mano.** Muchas cartas escriben 2-3 números (daño/NP/★) y el jugador administra cuatro lecturas simultáneas (medidor, estrellas, calendario de Decretos, Amenazas). Los glows y el contador de Autoridad ayudan, pero ésta es la propuesta con más texto-por-carta de las tres casi seguro, y «solitario de conversiones» sigue siendo mi riesgo declarado: si en playtest aparecen turnos frecuentes de 0 daño, la corrección es engordar los riders de daño de la familia Tesorería (Nota, Doble Imposición, Requisa), no agregar cartas.

---

Cumplimiento formal: 20/28/20 ✓ · conectividad comunes 95% ✓ · denominaciones 10/20/30/50/100 (upgrades usan la banda extendida 40/60 con precedente Oberon) ✓ · cap 3/turno + `BeforeSideTurnStart` en starter ✓ · 12 reliquias con jefe-驚天動地 y Bond CE ✓ · sin interés, sin cuotas, todo pago inmediato ✓ · presupuesto mecánico cerrado: Autoridad + Guts/F3 + Amenaza, nada nuevo ✓.
