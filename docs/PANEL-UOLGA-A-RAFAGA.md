# PROPUESTA A — U-Olga Marie: «Ráfaga y Tempo»

Panel §4.6.7, lente: bancar y descargar. Construida ENCIMA del acta (`DESIGN-UOLGA.md` §1-§8, sin re-discutir nada), con formato de `DESIGN-GILGAMESH.md` y la disciplina de denominaciones/conectividad de §4.6. Presupuesto cerrado respetado: **Autoridad + Guts/F3 + marcador de Amenaza** — todo lo demás reusa FGOCore (`NpCharge`, `CritStars`, `SureHit`, `SkillSeal`, Bloqueo de Curación de Tiamat, `FgoAttributes`, `BondRelic`, Anti-Purga de Artoria). Los «riders de secuencia» (cadenas Q/A/B, «primera carta del turno») NO son mecánica nueva: leen el tipado de comando que FGOCore ya publica (`CommandType`/`ICommandTyped`) más flags por turno vía `FgoCombatState` con reset en `BeforeSideTurnStart` — cero contadores, cero economía nueva.

---

## 1. La frase y los tres verbos

**La Directora que administra su propia ira: cada carta jugada deposita en el presupuesto, cada 100 el presupuesto exige un decreto — gastarlo YA (Planet Olga Marie) o convertirlo en Autoridad — y el mazo entero premia EL ORDEN en que jugás el turno: encadenar comandos llena el medidor más rápido, y la ventana de 5 turnos de Autoridad se exprime hasta la última carga.**

Verbos del jugador: **CARGÁ** (encadenar Q/A/B para llenar rápido) → **DECRETÁ** (elegir el momento y la magnitud de la conversión: ¿2 cargas ahora o 5 en dos turnos?) → **EXPRIMÍ** (ordenar cada turno de la ventana para que el Decreto, el crítico y el Buster caigan donde duelen).

- **Distinción de las otras lentes**: acá la tensión central es TEMPORAL. Convertir a 100 rinde 30 de daño total (2 Decretos de 15); convertir a 300 rinde 175 (5 de 35) — casi el doble de eficiencia por punto de NP, pero pagando 2-3 turnos de banca en los que el medidor no hace nada y la Desatada tampoco sale. La eficiencia premia la paciencia; el tempo premia matar antes. Ninguna de las dos líneas es dominante y las dos se enseñan desde el mazo inicial.
- **Stats**: HP **70** (banda 70-72 del acta §8; kit explosivo → piso de la banda, precedente Tiamat). 3⚡, mano 5.

## 2. Mazo inicial (10) — QAABB sesgado

Básicas de comando estándar §4.6.1 exactas (Buster 10 / Arts 6+30 NP / Quick 6+30★, arte `card_servant_1.png` en 3 bandas).

| Carta ES | EN | ⚡ | Efecto | Nota |
|---|---|---|---|---|
| **Buster** ×2 | Buster | 1 | 10 de daño. (up 14) | comando rojo |
| **Arts** ×2 | Arts | 1 | 6 de daño; +30 Carga NP. (up 9/+30) | el hilo NP — y 2 copias = **cadena Arts posible turno 1** |
| **Quick** ×1 | Quick | 1 | 6 de daño; +30 Estrellas. (up 9/+30) | el hilo de estrellas |
| **Magnetosfera** ×2 | Magnetosphere | 1 | 5 de Bloqueo. (up 8) | el escudo del planeta — la Defend |
| **Orden Ejecutiva** ×2 (FIRMA 1) | Executive Order | 1 | +20 Carga NP; tu **próximo Ataque de este turno** hace +3 de daño. (up +30/+4) | enseña EL ORDEN: jugala ANTES de pegar. Glow si aún no atacaste |
| **Ultimátum** ×1 (FIRMA 2, `ITranscendenceCard`) | Ultimatum | 1 | 9 de daño; +10 NP; contra **Amenazas para la Humanidad**: +10 NP adicional. Glow. (up 12) | enseña el marcador de Amenaza desde el primer jefe |

Gana el Acto 1 sin motor: 10+10+6+6+6+9 de comandos + 2×5 de Bloqueo; las Ordenes Ejecutivas + starter llevan el medidor a ~100 en el turno 2-3 → la primera decisión de decreto llega en el primer élite. ✓

## 3. Starter relic — el motor

| | |
|---|---|
| **Sello de la Dirección** / **Seal of the Directorate** | **STARTER (motor, índice 0, `GetUpgradeReplacement()`)**. Evento universal → recurso central: **cada vez que jugás un Ataque: +10 de Carga NP (máx 3/turno, reset en `BeforeSideTurnStart` sólo si `participants.Contains(Owner)`)**. Icono: clase **Beast dorada** (5★, regla WORKFLOW §6). Dispara la precarga de visuales en `BeforeCombatStartLate`. |

- **Por qué Ataque→NP y no →estrellas**: la identidad ENTERA es el medidor-presupuesto (acta §1); el starter tiene que garantizar el flujo que calibra todos los riders. Mash convierte bloqueo→★, Morgan sangre→★, Gil armas→★; U-Olga convierte **agresión→presupuesto**.
- **Candado gratis**: el Decreto «no dispara riders de "jugaste un Ataque"» (acta §2) → **el token no procesa el starter por construcción**. Cero riesgo de loop token→NP.
- Flujo garantizado: 2-3 Ataques/turno = +20-30 NP/turno pasivos; con Arts encima, 100 cada ~2 turnos, 300 en ~4-5 de banca dedicada. Ese es el reloj contra el que está calibrado todo el pool. Knob si acelera de más: cap 3→2 (nunca bajar el 10, denominación mínima).

## 4. Pool de 68 recompensas (20 C / 28 PC / 20 R)

Denominaciones NP/★ sólo 10/20/30/50/100. Glow dorado en TODA condicional; condición vacía = sin glow. «Cadena X» = la última carta jugada este turno era del tipo X (tipado FGOCore). **[Mal]** = poder etiquetado Mal: se pierde al levantarse como Protectora (acta §3) — el kit egoísta que compite con transformarse. **[F3]** = cambia tras la transformación (§7 abajo).

### 4.1 COMUNES (20) — engranajes; conectividad 18/20 = 90% ✓ (fillers declarados: #1 y #4, secuencia pura)

| Nombre ES | Nombre EN | Tipo | ⚡ | Efecto | Mejora | Recurso que lee/escribe |
|---|---|---|---|---|---|---|
| Impacto Sísmico | Seismic Impact | At (B) | 1 | 8 de daño; cadena Buster: +6. Glow | 11 / +8 | secuencia (filler declarado) |
| Circuito Cerrado | Closed Circuit | At (A) | 1 | 6 de daño; cadena Arts: +20 NP. Glow | 9 / +30 | NP |
| Ráfaga Estelar | Stellar Burst | At (Q) | 1 | 6 de daño; cadena Quick: +20 Estrellas. Glow | 9 / +30 | ★ |
| Golpe de Apertura | Opening Blow | At (B) | 1 | 10 de daño; si es la 1.ª carta del turno: +4. Glow | 14 / +5 | secuencia (filler declarado) |
| Tercera Orden | Third Directive | At | 1 | 6 de daño; si ya jugaste 2+ cartas este turno: +20 NP. Glow | 9 / +30 | NP |
| Peso de la Autoridad | Weight of Authority | At (B) | 2 | 14 de daño; +10 NP | 18 / +20 | NP |
| Andanada Orbital | Orbital Volley | At | 1 | 3 de daño ×3 (aleatorio); +10 NP | 4×3 | NP (multi-hit anti-Buffer) |
| Estela del Cometa | Comet Trail | At (Q) | 1 | 5 de daño; +20 Estrellas | 8 / +30 | ★ |
| Directiva de Caza **[F3]** | Hunting Directive | At | 1 | 8 de daño; contra Amenazas: +4 y +10 NP. Glow | 11 / +6 | Amenaza + NP |
| Exprimir la Ventana **[F3]** | Squeeze the Window | At | 1 | 7 de daño; con Autoridad activa: +4. Glow | 10 / +6 | Autoridad (lee) |
| Protocolo de Contención | Containment Protocol | Hab | 1 | 6 de Bloqueo; +10 NP | 9 / +20 | NP |
| Escudo de Ozono **[F3]** | Ozone Shield | Hab | 1 | 5 de Bloqueo; con Autoridad activa: +5. Glow | 8 / +7 | Autoridad (lee) |
| Repliegue Táctico | Tactical Withdrawal | Hab | 1 | 8 de Bloqueo; si es la 1.ª carta del turno: +10 NP. Glow | 11 / +20 | NP |
| Presión Atmosférica | Atmospheric Pressure | Hab | 1 | 6 de Bloqueo; +10 Estrellas | 9 / +20 | ★ |
| Requisa de Maná (ESPEJO A) | Mana Requisition | Hab | 0 | Consumí 50 Estrellas: +50 NP. Glow | consume 30 | ★→NP |
| Dividendo Estelar (ESPEJO B) | Stellar Dividend | Hab | 0 | Perdé 50 NP: +50 Estrellas. Glow | perdé 30 | NP→★ |
| Informe de Situación | Status Report | Hab | 1 | Robá 2; +10 Estrellas | robá 3 | ★ |
| Mirada Fulminante | Withering Glare | Hab | 0 | Aplica 1 Débil; +10 Estrellas | 2 Débil / +20 | ★ |
| Cierre de Presupuesto | Budget Closure | Hab | 1 | +20 NP; +10 Estrellas | +30 / +20 | NP + ★ |
| «¡Soy la Directora!» (meme) | "I'm the Director!" | Hab | 0, Agotar | +10 NP; +10 Estrellas; robá 1 | +20/+20, robá 2 | NP + ★ |

Los pares espejo 0⚡ (50 NP ↔ 50★) cumplen §4.6.2: ningún medidor se estanca, round-trip neto 0 sin robo ni energía → sin arbitraje.

### 4.2 POCO COMUNES (28)

| Nombre ES | Nombre EN | Tipo | ⚡ | Efecto | Mejora | Recurso que lee/escribe |
|---|---|---|---|---|---|---|
| KIT Sin Precedentes (空前絶後) | KIT Unparalleled EX | Hab | 1, Agotar | Descartá tu mano, robá esa cantidad; +20 Estrellas; **tu robo del próximo turno baja 1** (acta §5: demérito propio) | +30★ | ★ (consistencia con precio) |
| KIT Perfección Absoluta (天衣無縫) | KIT Flawlessness EX | Hab | 1, Agotar | +50 Estrellas; tus Ataques tienen **Certero** este turno (SureHit FGOCore) | +50★ y robá 1 | ★ |
| KIT Planta Atómica (アトミックプラント) | KIT Atomic Plant B | Poder | 1 | Al inicio de tu turno: +10 NP | al jugarla: +20 NP adicional | NP |
| Desprecio por la Humanidad **[Mal]** | Contempt for Humanity | Poder | 1 | Tus Ataques hacen +2 de daño | +3 | Mal |
| Egoísmo de Bestia **[Mal]** | Beast's Egoism | Poder | 1 | Fin de turno: si no recibiste daño este turno: +20 NP | +30 | Mal + NP — **paga por NO exponerse: la anti-transformación hecha carta** |
| Botín de la Bestia **[Mal]** | Beast's Spoils | Poder | 1 | Cuando matás a un enemigo: +30 NP y robá 1 | +50 | Mal + NP |
| Escolta del Decreto | Decree Escort | Poder | 1 | Cuando jugás un Decreto: 6 de Bloqueo | 9 | Autoridad (listener explícito de Decreto — Bloqueo, no NP: sin loop) |
| Eco de Autoridad | Echo of Authority | Poder | 2 | Tus Decretos hacen +6 de daño (**cap global de bonos al Decreto: +15**) | +9 | Autoridad |
| Ceremonia de Investidura | Investiture | Hab | 1 | Sólo con Autoridad activa (glow): robá 2; +10 NP | robá 3 | Autoridad + NP |
| Reserva Estratégica | Strategic Reserve | Poder | 1 | Fin de turno: si tu Carga ≥100: +10 Estrellas y 4 de Bloqueo | +20★ / 6 | NP (lee) + ★ — **paga la banca** |
| Coreografía de Combate | Combat Choreography | Poder | 1 | La 1.ª cadena de cada turno: +10 NP y +10 Estrellas | +20 / +20 | NP + ★ |
| Cadena Valiente | Brave Chain | At | 1 | 8 de daño; si ya jugaste 2+ Ataques este turno: +8. Glow | 11 / +10 | secuencia (Brave Chain FGO) |
| Agenda de la Directora | Director's Agenda | Hab | 0, Agotar | Tu próxima carta este turno cuesta 1⚡ menos; +10 NP | además robá 1 | NP + economía |
| Horas Extra | Overtime | Hab | 0, Agotar | Ganá 1⚡; tu robo del próximo turno baja 1 | además +10 NP | economía (precio estilo 空前絶後) |
| Tormenta Planetaria **[F3]** | Planetary Storm | At | 2 | 10 de daño a TODOS; +10 NP | 13 | NP (AoE) |
| Lluvia de Escombros | Debris Rain | At | 1 | 4 de daño ×3 (aleatorio); +10 Estrellas | 5×3 / +20 | ★ (multi-hit) |
| Cinturón de Asteroides | Asteroid Belt | Hab | 2 | 14 de Bloqueo; +10 NP | 18 / +20 | NP |
| Campo Gravitatorio | Gravity Field | Hab | 1 | 8 de Bloqueo; con ≥50 Estrellas: +6. Glow | 11 / +8 | ★ (lee) |
| Voluntad Inquebrantable **[F3]** | Unyielding Will | Poder | 1 | Cuando recibís daño: +10 NP (máx 2/turno) | +20 | NP — canon literal (recibir daño → 3% NP); alimenta la línea de exposición del Guts |
| Ojo de la Tormenta | Eye of the Storm | Poder | 1 | Cuando consumís un Crítico Listo: +20 NP | +30 | ★→NP (el crítico devuelve tempo) |
| Contraluz | Backlight | Hab | 1, Agotar | +50 Estrellas | +50 y robá 1 | ★ (batería de crítico) |
| Deber de la Protectora **[F3]** | Protector's Duty | At | 2 | 14 de daño; contra Amenazas: +6 y 5 de Bloqueo. Glow | 18 / +8 y 8 | Amenaza |
| Catalogar Amenaza | Threat Cataloguing | Hab | 1 | 2 Vulnerable; contra Amenazas: además **Bloqueo de Curación** 2T (rider de Tiamat, acta §5). Glow | 3 Vulnerable | Amenaza |
| Adelanto de Fondos | Advance Funds | Hab | 1, Agotar | +50 NP | 0⚡ | NP |
| Mantenimiento de Planta | Plant Maintenance | Hab | 1, Agotar | Curá 4; +10 NP | 6 / +20 | NP (sustain) |
| Órbita Estable | Stable Orbit | Hab | 1 | 10 de Bloqueo; si todavía no jugaste Ataques este turno: +10 NP. Glow | 13 / +20 | NP + secuencia (defensa-primero) |
| Segunda Firma | Second Signature | At (A) | 1 | 8 de daño; +20 NP | 11 / +30 | NP (Arts engordada) |
| Grito de Mando | Commanding Shout | Poder | 2 | +2 Fuerza; +10 Estrellas (co-op: aliados +1 Fuerza) | 3 Fuerza | ★ (slot Carisma del roster) |

### 4.3 RARAS (20)

| Nombre ES | Nombre EN | Tipo | ⚡ | Efecto | Mejora | Recurso que lee/escribe |
|---|---|---|---|---|---|---|
| NP El Fin Ya Pasado | NP The End Long Past | At NP (B) | 2, Agotar | **Mín. 50, consume TODA**: 4 de daño ×5 a TODOS; contra **Humanos**: +2 por golpe; Sobrecarga: +1 por golpe por cada 50 consumidos sobre 50. Glow al ser pagable | 5×5 | NP→daño — **el NP "temprano" del tempo** (dispara antes que la Desatada) |
| Trono de la Bestia VII **[Mal]** | Throne of Beast VII | Poder | 3 | Ganá 1⚡ al inicio de tu turno | 2⚡ | Mal + economía — el poder que MÁS duele perder al transformarse |
| Odio a la Humanidad **[Mal]** | "I Hate Humanity" | Poder | 1 | Fin de turno: si jugaste 3+ cartas: +20 NP y +10 Estrellas | +30 / +20 | Mal + NP + ★ |
| Última Carga | Final Charge | Hab | 2, Agotar | +100 NP | 1⚡ | NP |
| Enmienda al Decreto | Decree Amendment | Hab | 1, Agotar | Tu próxima conversión otorga +1 carga (**cap 5 intacto**). Glow con Carga ≥50 | 0⚡ | Autoridad |
| Autoridad Suprema | Supreme Authority | Poder | 2 | Tus Decretos hacen +8 y ganás 4 de Bloqueo al jugarlos (cap global +15) | +12 / 6 | Autoridad |
| Límite Excedido | Limit Exceeded | At | 2 | 12 de daño; +4 por cada turno restante de tu Autoridad (máx +20). Glow | 16 / +5 por turno | Autoridad (lee el reloj — la carta-lente) |
| Juicio Final de la Directora | The Director's Final Judgment | At (B) | 3 | 30 de daño; +10 Estrellas | 38 / +20 | ★ (slot Bludgeon, blanco soñado del ×2) |
| Ejecución Sumaria | Summary Execution | At | 0 | Sólo con ≥50 Estrellas (glow): consume 50; 25 de daño | 32 | ★→daño (slot Comet; gastar retrasa el auto-Crítico — tensión) |
| Planificación Total | Master Plan | Poder | 2 | Cuando convertís tu medidor: ganá 1⚡ y robá 2 | robá 3 | Autoridad + economía — **hace de la conversión un turno, no un sacrificio** |
| Memoria del Fin | Memory of the End | Hab | 0 | Gastá 30 NP: robá 2. Glow | robá 3 | NP→cartas (la válvula) |
| Escudo Planetario | Planetary Shield | Hab | 2, Agotar | 25 de Bloqueo; +10 NP | 32 | NP (slot Impervious) |
| Protectora de la Humanidad **[F3]** | Guardian of Humanity | Poder | 2 | Contra Amenazas tus Ataques hacen +4; cuando una Amenaza te ataca: +10 Estrellas | +6 / +20 | Amenaza + ★ |
| Atmósfera Densa | Dense Atmosphere | Poder | 2 | Fin de turno: 3 de Bloqueo; con Autoridad activa: 6 | 5 / 9 | Autoridad (lee) |
| Sala de Mando | Command Room | Hab | 1 | Robá 3; +20 NP | robá 4 | NP (consistencia rara) |
| Bombardeo Orbital **[F3]** | Orbital Bombardment | At | 2 | 12 de daño a TODOS; contra Humanos: +4. Glow | 16 / +6 | atributo Humano (el eco del special del NP) |
| Golpe de Autoridad | Authority Strike | At (B) | 2 | 22 de daño; si es la 1.ª carta del turno: +20 Estrellas. Glow | 28 / +30 | ★ + secuencia (frontload anti-élite) |
| Cadena Perfecta | Perfect Chain | At | 1 | 10 de daño; si completa una cadena de 3 del mismo tipo: +30 NP y +30 Estrellas. Glow | 14 / +50 NP y +30★ | NP + ★ (el payoff de cadenas) |
| Fondos de Reserva | Reserve Funds | Hab | 1 | Si convertiste este turno (glow): +50 NP y robá 1 | +50 / robá 2 | NP + Autoridad — amortigua el todo-o-nada |
| Veto de la Dirección | Directorate Veto | Hab | 1, Agotar | **Sello de Habilidad** a TODOS 1 turno (SkillSeal FGOCore, acta §5); +10 NP | además +20 Estrellas | NP (respuesta a jefes) |

**Matriz de cobertura**: frontload (Golpe de Apertura/Autoridad, Juicio Final) · defensa (7 cartas + Escolta/Atmósfera) · consistencia (Informe, Sala de Mando, 空前絶後, Memoria del Fin, Ceremonia) · economía (Horas Extra, Agenda, Trono [Mal], Planificación) · escalado (poderes de Decreto + [Mal] + Bond) · multiobjetivo (Tormenta, Bombardeo, Andanada/Lluvia multi-hit, ambos NP AoE, Decreto en F3) · **jefes que limpian buffs: la Autoridad es un tanque de CARTAS (inmune al strip por construcción), NP/★ son recursos, y Veto sella habilidades** — sólo los poderes caen, y el kit no depende de debuffs. ✓

## 5. Reliquias (12)

| Reliquia ES / EN | Rareza | Efecto |
|---|---|---|
| **Sello de la Dirección** / Seal of the Directorate | STARTER (motor) | §3 arriba. Jugar Ataque → +10 NP, máx 3/turno, reset `BeforeSideTurnStart`. Icono Beast dorada |
| **Contrato con la Humanidad** / Contract with Humanity (la Bond CE) | STARTER (BondRelic) | Vínculo estándar (+2/+3/+5, +1/sala) + `ServantDamageMultiplier`/`ServantBlockMultiplier` **×1.25** (palanca del roster, motor FGOCore que sobrevive al strip). Overrides: **Nv 4**: tus Decretos +2 de daño (cuenta al cap +15); **Nv 7**: la primera conversión de cada combate: robá 2; **Nv 10 «La Directora de la Humanidad»**: al convertir, +1 carga (cap 5 intacto). Nv 11-12 sólo con Grial |
| **Registro de la Dirección** / Directorate Registry | STARTER OCULTA (`INpLevelStore`) | Dupes/NP level 1-5, pity estándar; +15%/nivel a la Desatada y al NP drafteable vía `NpLevels.Scale` |
| **Autoridad Plena** / Full Authority | JEFE (Ancient, **reemplaza al Sello** — contrato Orobas de DECISIONS) | Todo lo del Sello, y los Ataques también dan +5 Estrellas (mismo cap 3/turno). Reinstala motor, forma y contadores; transfiere `INpLevelStore` |
| **驚天動地 — Conmoción de Cielo y Tierra** / Earth-Shattering B | **JEFE (Ancient drafteable — acta §5)** | 1 vez por combate, botón: llená tu medidor pagando **Vida imparable**: 1 Vida por cada 10 faltante hasta 100; cada 10 por encima de 100 cuesta 2 Vida. La Vida pagada **no alimenta nada** (candado del acta: comprás llegar antes, nunca más total). Append 技能再装填: re-arme 1/combate → −25% Vida |
| **Grial de la Manifestación** / Grail of Manifestation (`ILimitBreaker`) | EVENTO (Acto 2, 200 oro) | **Repara 単独顕現**, la habilidad de clase perdida (acta §6): +15 HP máx; Vínculo hasta 12; NP level hasta 6 |
| **Batería de Repuesto** / Spare Battery | TIENDA | Al inicio de cada combate: +20 de Carga NP (apila con el append 魔力装填 — los appends van por encima de la vara, acta §6) |
| **Manual de Protocolo** / Protocol Manual | COMÚN | La primera cadena de cada combate (dos cartas seguidas del mismo tipo): +20 NP |
| **Cronómetro de Actividad** / Activity Timer | POCO COMÚN | Cuando tu ventana de Autoridad expira: +50 NP (el reloj devuelve tempo — suaviza el ciclo bancar→convertir) |
| **Lente de Contraluz** / Backlight Lens | POCO COMÚN | Cuando ganás un Crítico Listo: +10 NP y robá 1 (máx 1/turno) |
| **Archivo de Amenazas** / Threat Archive | POCO COMÚN | Las Amenazas para la Humanidad entran al combate con 1 Vulnerable |
| **Pluma de la Directora** / The Director's Pen | RARA | Tus Decretos hacen +3 de daño (cuenta al cap +15) y al jugar un Decreto: +10 Estrellas |

## 6. Carta-NP, Desatada, y la carta de conversión (los números del tier)

**Ambas se manifiestan JUNTAS a 100** (`GaugeFilled` manifiesta la Desatada por el embudo estándar + el mod agrega el Ultra Manifiesto en el mismo handler; ambas Retain; jugar una consume TODA la carga por `ConsumeAllForNpCard` y remueve a la otra hasta el próximo llenado). Esa mano con dos cartas doradas ES la decisión del acta §1 hecha UI.

- **Planet Olga Marie: Desatada** (すでに過ぎし人理の終／プラネット・オルガマリー) — At NP Buster, 0⚡, Retain, Agotar, mín. 100, consume TODA. **5 golpes de 5 de daño a TODOS (25)** — los 5 hits canónicos, multi-hit anti-Buffer. Contra **Humanos** (salas Monster, convención `FgoAttributes` — el special inédito del acta §5): **+2 por golpe** (35). Contra **Estrellas**: +1 por golpe (el guiño +20%, huevo de pascua, nunca línea de balance). **Sobrecarga: +1 por golpe por cada 50 consumidos sobre 100** → a 300: 9/golpe = **45** (55 vs Humano). Daño base escala sólo con dupes (`NpLevels`).
- **Ultra Manifiesto** (la conversión, acta §2) — `CardRarity.Event`, Hab, 0⚡, Retain, mín. 100, consume TODA, **no hace daño**: ganás **1 carga de Autoridad por cada 50 consumidos (cap 5)**; el primer **Decreto** llega YA a tu mano; los siguientes, 1 al inicio de cada turno; **reloj de 5 turnos** (contador visible «Autoridad» = cargas + turnos, acta §7); al expirar, los Decretos restantes se agotan solos. En F3 se re-titula **«アルテミット・U»** y otorga además 5 de Bloqueo por carga convertida.
- **Decreto de la Directora** (el token único) — Event, At, 0⚡, sin tipo de comando (es el Extra Attack): **daño = 5 + 5 por cada 50 del tier consumido** → 100→**15** · 150→20 · 200→25 · 250→30 · 300→**35**. Arte oficial de command card por ascensión. Candados del acta intactos: no genera NP, no dispara riders de «jugaste un Ataque» (`IsFirstInSeries`), no re-dispara `CommandBonusPower`. **Puede consumir Crítico Listo** (decisión de esta propuesta — el crítico del Extra es fantasía FGO pura) y por eso lleva un **candado propio: los bonos aditivos al Decreto (cartas+reliquias+Bond) capean en +15**, auditable por construcción. En F3: **a TODOS con el daño por objetivo a la mitad** (salvo enemigo único — canon del acta §0).

## 7. Riders de Forma 3 (~12% del pool, precedente Kagetora)

**逆光 EX al levantarse** (números propuestos para el tier del acta §3): Invulnerable 1 golpe (**Anti-Purga de Artoria reutilizado**, no reinventado) + **+30 de Carga NP** + **tus cartas Buster hacen ×1.3 por el resto del combate** («Buster arriba»). Precio ya cerrado por el acta: −10 MaxHP permanente + **se remueven todos los poderes [Mal] activos** (Desprecio, Egoísmo, Botín, Trono VII, Odio — siguen siendo jugables de nuevo pagando su ⚡: el precio es tempo, no ladrillo).

Cartas del pool que cambian (8/68 = 11.8%):

| Carta | En Forma 3 |
|---|---|
| Directiva de Caza (C) | el bonus anti-Amenaza se duplica (+8 y +20 NP) |
| Exprimir la Ventana (C) | +4 → +8 |
| Escudo de Ozono (C) | +5 → +10 |
| Tormenta Planetaria (PC) | gana «contra Humanos: +4» |
| Voluntad Inquebrantable (PC) | cap 2/turno → 3/turno |
| Deber de la Protectora (PC) | el Bloqueo del rider se duplica (5→10) |
| Protectora de la Humanidad (R) | ambos bonos se duplican |
| Bombardeo Orbital (R) | el bonus vs Humanos se duplica |

Más los dos Event: **Decreto** (AoE a mitad de daño) y **Ultra Manifiesto** (→ アルテミット・U, +Bloqueo por carga). La lectura del giro: el kit egoísta muere, y todo lo que huele a *proteger* (Amenaza, Bloqueo, aguantar golpes) se enciende.

## 8. La cuenta del pico (contra el techo 180-220)

**Setup razonable** (sin appends — el personaje se balancea sin ellos, acta §6): F3 activa (逆光 ×1.3 Buster), conversión a 300 este turno o hace 1-2, Autoridad Suprema+ (+12) y Eco de Autoridad (+6) en mesa → **cap +15 corta**, Bond Nv 4, Crítico Listo armado, mano ideal, 3⚡ (+1⚡ de Planificación Total si la conversión fue este turno).

| Jugada | Cálculo | Acumulado |
|---|---|---|
| Decreto (0⚡, tier 300) con Crítico Listo | (35 + 15 cap) × 2 | 100 |
| Juicio Final de la Directora+ (3⚡, Buster, 逆光) | 38 × 1.3 = 49 | 149 |
| Impacto Sísmico+ (1⚡ de Planificación; cadena Buster tras el Juicio, 逆光) | (11+8) × 1.3 = 25 | **174** |
| *Stretch* (si además hay 50★ bancadas SOBRE el crítico ya pagado): Ejecución Sumaria+ (0⚡) | +32 | **206** |

**Veredicto: 174 típico, 206 en el stretch — dentro del techo 180-220, tocando la banda alta.** Es la firma honesta de la lente ráfaga: el pico existe, pero exige conversión a 300 (4-5 turnos de banca), dos poderes raros/PC, F3 (que costó morir a manos de una Amenaza, −10 MaxHP y los [Mal]) y ~150★ generadas. El sostenido de la ventana es sano: Decreto 35-50 + ~40 de cartas ≈ 75-90/turno durante 5 turnos. La conversión temprana (100→2×15) es tempo puro y no toca el techo. **Contingencias en orden si el playtest lo pasa**: 1) cap de bonos al Decreto +15→+10; 2) el Decreto deja de poder consumir Crítico Listo (candado declarado, listo para activar); 3) 逆光 ×1.3→×1.25. Nunca tocar la tasa 1 carga/50 ni el cap 5 (son del acta).

## 9. Auto-crítica honesta — dónde me va a pegar el juez

1. **La cadencia del starter puede matar la tensión que vendo.** Ataque→NP (30/turno) + Arts +30 llena 100 cada ~2 turnos; con la ventana durando 5, la Autoridad puede tener uptime casi permanente convirtiendo siempre a 100, y el «¿banco hasta 300?» se vuelve retórico en salas normales (la banca queda sólo para élites/jefes). La matemática 30 vs 175 de daño total favorece bancar 2:1 — si el juez corre la cuenta con Planta Atómica + Última Carga encima, puede encontrar que la banca domina SIEMPRE y la decisión colapsa para el otro lado. El fix barato es el cap del starter 3→2, pero recalibra medio pool.
2. **Decreto + Crítico Listo es el punto más caliente del pico.** Un 0⚡ que pega 100 con setup depende de dos decisiones mías (aditivos antes del ×2, cap +15) que son parches sobre el candado del acta, no parte de él; y el auto-gasto del Crítico (patrón StarlitCharge/Mordred: se consume en el PRÓXIMO Ataque, sea cual sea) significa que el crítico puede caer en un Impacto Sísmico de 8 en vez del Decreto — el jugador que no secuencia bien va a sentir que el juego lo estafó. Mi lente lo llama «aprender a ordenar»; un juez lo puede llamar «feel-bad de primera hora».
3. **Los riders de cadena son superficie técnica nueva y los [Mal] pueden ser una trampa de draft.** Las cadenas exigen tipado de comando en la mayoría de los Ataques del pool (los otros personajes sólo tipan las básicas — hay que verificar cuánto de `ICommandTyped` está realmente cableado para cartas no-básicas) más estado por turno impecable; mis dos únicos fillers de conectividad viven ahí. Y un jugador que draftea 3-4 poderes [Mal] y después muere ante una Amenaza pierde el motor entero en el momento más dramático — el acta lo pide como precio, pero «competitivo sin transformarse» es una promesa que sólo el playtest puede firmar, y no tengo forma de demostrarla en papel.
