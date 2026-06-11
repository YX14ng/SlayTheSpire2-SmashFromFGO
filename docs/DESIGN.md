# Diseño de personaje — Mash Kyrielight (Shielder)

> Mod de personaje para **Slay the Spire 2 (rama MAIN, v0.103.x)** sobre **BaseLib ≥ 3.1.8** (actual v3.2.0).
> "Smash" del nombre del repo = **Mash Kyrielight**, la Demi-Servant Shielder de Fate/Grand Order.

---

## 0. Filosofía de balance

**Objetivo: top-tier, no tramposa.** Mash debe sentirse rota — números por encima de la tasa estándar (una Defender da 5; sus murallas dan 6–9 y persisten) — pero el poder está **embudado por sus propias mecánicas**, no regalado:

- Las cartas más fuertes requieren **Carga NP** (recurso que hay que construir defendiendo).
- Lo más explosivo cuesta **HP** o tiene **Exhaust** (un solo uso por combate).
- Su daño escala con el Bloqueo: si la fuerzan a no defender, baja a tasa normal.
- Referencia de nivel de poder: la Watcher de StS1 — claramente la más fuerte del elenco, sin trivializar jefes por sí sola.

## 1. Identidad y fantasía de juego

**La muralla que avanza.** Mash es el tanque definitivo de FGO: una Demi-Servant fusionada con el espíritu heroico **Galahad** que protege a su Senpai con el escudo **Lord Camelot** (la Mesa Redonda de Camelot). En StS2 se traduce en:

- **Acumular y conservar Bloqueo** entre turnos (keyword *Baluarte*).
- **Convertir defensa en ofensa**: la armadura **Ortinax** y el cañón **Black Barrel** gastan esa muralla como munición.
- **Cargar el Noble Phantasm**: medidor de *Carga NP* (0–100) que habilita cartas-NP devastadoras.
- **Progresión de formas en combate** que refleja su historia: *Shielder* → *Ortinax* → *Paladín*.

Fantasía: empezás el combate construyendo una fortaleza y terminás disparando la fortaleza.

## 2. Resumen de lore (fuente de todo el sabor)

- **Designer baby de Chaldea**: creada artificialmente, vida corta, criada en laboratorio. Su único amigo de infancia: **Fou**.
- **Demi-Servant**: fusionada con Galahad. Hereda el escudo **Lord Camelot** y su NP propio **Lord Chaldeas**.
- **Singularidad de Salomón**: se sacrifica contra Goetia; **Fou gasta su milagro** para revivirla.
- **Lostbelts**: Galahad le retira su poder → Chaldea le construye el exoesqueleto **Ortinax** y le entrega el **Black Barrel**, el cañón conceptual del Instituto Atlas que "mata lo inmortal". Skills: *Bunker Bolt*, *Paradox Cylinder*, *Amalgam Goad*, *Mold Camelot*.
- **Ordeal Call (Trinity Metatronius)**: alcanza su propia caballería y se convierte en **Paladín** — heroína por derecho propio (de 3★ a SSR en FGO).
- Personalidad para sabor: "Senpai", sus lentes, los libros, los sándwiches de Chaldea, proteger antes que atacar.

## 3. Stats base

| Atributo | Valor |
|---|---|
| HP máximo | **85** (el tanque del elenco) |
| Energía | 3 |
| Oro inicial | 99 |
| Color de cartas | Lila/violeta oscuro, acentos rosa |
| Carga NP máxima | 100 |

**Reliquia inicial — Fragmento de la Mesa Redonda**: *Al final de tu turno, conserva hasta 10 de Bloqueo.*

**Mazo inicial (10 cartas):** 4× Golpe (1⚡, 6 daño) · 4× Defender (1⚡, 5 Bloqueo) · más sus dos firmas:

| # | Carta | Tipo, Coste | Efecto |
|---|---|---|---|
| S1 | **Golpe de Escudo** | Ataque, 1⚡ | 9 de daño. Carga NP +10. *(+: 12 de daño, Carga +15)* |
| S2 | **¡Proteger a Senpai!** | Habilidad, 1⚡ | 8 de Bloqueo. Carga NP +10. *(+: 11 de Bloqueo, Carga +15)* |

## 4. Mecánicas y keywords (vía BaseLib: custom keywords + powers + energy counter)

### Carga NP (recurso, contador visible junto a la energía)
- Se gana con cartas (`Carga NP +X`) y con las pasivas de forma.
- Las cartas **NP** tienen un **costo mínimo** de Carga y al jugarse **consumen TODA la Carga disponible** *(rediseñado en v0.1, estilo FGO)*.
- **Sobrecarga escalonada**: cada 10 de Carga consumida por encima del mínimo mejora los stats de la carta (+Bloqueo o +daño según la carta). *BLACK BARREL: Full Burst* conserva además su bonus máximo al consumir 100 (borra todos los buffs). Mínimos: LORD CHALDEAS y Full Burst 50; LORD CAMELOT y Rhongomyniad 70; la ulti generada 100.
- **Manifestación del NP** *(agregado en v0.1)*: **cada vez** que el medidor llega a 100, se genera en tu mano **"LORD CAMELOT: Unleashed"** — la ulti de Mash: coste 0⚡, Exhaust (se agota al usarla), 30 Bloqueo Baluarte + 3 Fuerza + Intercepción 12. Generarla NO consume el medidor; la Carga solo se gasta al jugar cartas NP (incluida esta). Al gastar Carga y volver a llenar a 100, se manifiesta de nuevo — el ciclo completo de FGO.
- Se resetea a 0 entre combates (la reliquia *Núcleo del Ortinax* da 40 inicial).

### Baluarte (keyword)
*El Bloqueo ganado por esta carta no se elimina al inicio de tu turno.* La keyword de identidad: la muralla persiste y crece. El Bloqueo normal sigue las reglas estándar salvo por la reliquia inicial.

### Formas (stances exclusivas del personaje; solo una activa)

| Forma | Pasiva |
|---|---|
| **Shielder** (inicial) | Al final de tu turno, si tienes 8+ de Bloqueo: Carga NP +5. La primera carta de Bloqueo de cada turno otorga +3 de Bloqueo. |
| **Ortinax** | Tus ataques consumen hasta 5 de tu Bloqueo y suman ese daño (*Bunker Bolt*). Tus cartas de defensa dan 1 menos de Bloqueo. |
| **Paladín** (solo vía carta rara) | Ambas pasivas, sin la penalización de Ortinax. No se puede salir de ella. |

### Cobertura — mecánica de multijugador *(agregado en v0.1)*
En partidas co-op, **al inicio de cada turno** (mientras haya un aliado vivo) se genera **"Behind Me!"**: coste 0⚡, Ethereal (se desvanece si no se usa), Exhaust. Al jugarla: hasta tu próximo turno, **todo el daño que atraviese las defensas de tus aliados lo recibe Mash** (su propio Bloqueo aplica). El tanque del grupo, como en FGO. En solitario la carta no se genera.

### Intercepción (keyword de contraataque)
*Mientras tengas Intercepción X: cuando un enemigo te ataque y el golpe no atraviese tu Bloqueo, recibe X de daño.* El "taunt + counter" de Mash (*Shield of Rousing Resolution*, *Amalgam Goad*).

### Black Barrel (keyword de anti-escalado)
*El daño Black Barrel ignora el Bloqueo enemigo y elimina 1 buff del objetivo.* Su nicho anti-jefes: "las balas que matan lo inmortal".

## 5. Pool de cartas — COMPLETO (70 + 2 firmas)

> Distribución estándar StS: **20 comunes / 30 poco comunes / 20 raras**.
> Formato: **Nombre** — Tipo, Coste⚡ (efecto). *(+: mejora)*

### Comunes (20)

**Ataques (10)**
1. **Embate de Escudo** — Ataque, 1⚡: 8 de daño. Aplica 1 de Vulnerable. *(+: 11 de daño, 2 de Vulnerable)*
2. **Borde Afilado** — Ataque, 1⚡: 5 de daño + tu Bloqueo actual (máx. +8). *(+: máx. +12)*
3. **Carga Frontal** — Ataque, 2⚡: 13 de daño. Gana 5 de Bloqueo. *(+: 17 y 7)*
4. **Golpe Doble de Asta** — Ataque, 1⚡: 5 de daño ×2. Carga NP +5. *(+: 6×2, Carga +10)*
5. **Aplastamiento** — Ataque, 2⚡: 16 de daño. Pierde 4 de Bloqueo. *(+: 21 de daño)*
6. **Disparo de Cobertura** — Ataque, 0⚡: 5 de daño. Solo jugable con 8+ de Bloqueo. *(+: 8 de daño)*
7. **Puño del Caballero** — Ataque, 1⚡: 10 de daño. En forma Ortinax: Carga NP +10. *(+: 13 de daño)*
8. **Barrido Defensivo** — Ataque, 1⚡: 7 de daño a TODOS. Gana 3 de Bloqueo. *(+: 9 y 5)*
9. **Bala Conceptual** — Ataque, 1⚡: 6 de daño **Black Barrel**. *(+: 9)*
10. **Escaramuza** — Ataque, 1⚡: 6 de daño. 4 de Bloqueo. *(+: 8 y 6)*

**Habilidades (10)**
11. **Muro de Tiza** — Habilidad, 1⚡: 9 de Bloqueo. *(+: 13)*
12. **Postura Firme** — Habilidad, 1⚡: 6 de Bloqueo con **Baluarte**. *(+: 8)*
13. **Orden de Refuerzo** — Habilidad, 1⚡: 7 de Bloqueo. Roba 1. *(+: 10 de Bloqueo)*
14. **Respiración de Combate** — Habilidad, 0⚡: Carga NP +12. *(+: +18)*
15. **Manual de Chaldea** — Habilidad, 1⚡: roba 3, descarta 1. *(+: sin descartar)*
16. **Provocación** — Habilidad, 0⚡: Intercepción 4 este turno. *(+: 7)*
17. **Escudo en Alto** — Habilidad, 2⚡: 15 de Bloqueo. *(+: 20)*
18. **Reagrupar** — Habilidad, 1⚡: 5 de Bloqueo. Si no tenías Bloqueo: Carga NP +12. *(+: 7, +18)* *(ajustado en implementación: condición "sin Bloqueo previo" en vez de "primer Bloqueo del turno")*
19. **Mantenimiento del Ortinax** — Habilidad, 1⚡: pierde todo tu Bloqueo; Carga NP igual al Bloqueo perdido (máx. 30). *(+: máx. 40)*
20. **Paso de Guardia** — Habilidad, 0⚡: 4 de Bloqueo. *(+: 6 de Bloqueo, roba 1)*

### Poco comunes (30)

**Ataques (10)**
21. **Bunker Bolt** — Ataque, 2⚡: 12 de daño. Consume todo tu Bloqueo: +1 de daño por cada 2 consumidos. *(+: +1 por cada 1)*
22. **Embestida de Camelot** — Ataque, 2⚡: daño igual a tu Bloqueo actual. *(+: coste 1⚡)*
23. **Ráfaga del Black Barrel** — Ataque, 1⚡: 5 de daño **Black Barrel** ×2. *(+: ×3)*
24. **Juicio del Caballero** — Ataque, 1⚡: 9 de daño. Intercepción 5 (2 turnos). *(+: 12 y 7)*
25. **Golpe de Vanguardia** — Ataque, 1⚡: 11 de daño. +6 si cambiaste de forma este combate. *(+: 14, +8)*
26. **Lanza de Asedio** — Ataque, 3⚡: 24 de daño. Carga NP +20. *(+: 30, +25)*
27. **Represalia** — Ataque, 1⚡: 7 de daño, +4 por cada golpe enemigo que tu Bloqueo aguantó este turno. *(+: +6)*
28. **Danza Vívida de Hierro** — Ataque, 2⚡: 9 de daño ×2. Gana 4 de Bloqueo por golpe que impacte. *(+: 11×2, 5)*
29. **Descarga del Cilindro** — Ataque, X⚡: gasta además Carga NP 10 por ⚡: 10 de daño por ⚡. *(+: 13 por ⚡)*
30. **Tiro de Supresión** — Ataque, 1⚡: 9 de daño. Aplica 2 de Débil. *(+: 12 de daño, 3 de Débil)*

**Habilidades (14)**
31. **Muro de Copos de Nieve** — Habilidad, 2⚡: 14 de Bloqueo con **Baluarte**. *(+: 18)*
32. **Cambio: ¡Ortinax!** — Habilidad, 1⚡: entra en forma **Ortinax**. 10 de Bloqueo. *(+: 14)*
33. **Cambio: ¡Shielder!** — Habilidad, 0⚡: entra en forma **Shielder**. Carga NP +15. *(+: +25)*
34. **Amalgam Goad** — Habilidad, 1⚡: Intercepción 6 (2 turnos). Carga NP +15. *(+: 9, +20)*
35. **Paradox Cylinder** — Habilidad, 1⚡: pierde 3 HP; 14 de Bloqueo e Intercepción 5 este turno. *(+: 18 y 7)*
36. **Escudo Trágico** — Habilidad, 0⚡: pierde 3 HP; 11 de Bloqueo. *(+: pierde 2, gana 13)*
37. **Plegaria a Galahad** — Habilidad, 1⚡: duplica tu Bloqueo (máx. +18). Exhaust. *(+: máx. +25)*
38. **Sándwich de Chaldea** — Habilidad, 1⚡: cura 5 HP. Solo jugable con 12+ de Bloqueo. Exhaust. *(+: cura 8)*
39. **Lentes de Mash** — Habilidad, 0⚡: mira las 4 cartas superiores del mazo; roba 1. *(+: roba 2)*
40. **Mold Camelot** — Habilidad, 2⚡: 11 de Bloqueo con **Baluarte**. Carga NP +12. *(+: 14, +18)*
41. **Formación Defensiva** — Habilidad, 1⚡: este turno, tus cartas de Bloqueo se juegan dos veces. Exhaust. *(+: sin Exhaust)*
42. **Protocolo Bunker** — Habilidad, 1⚡: en Shielder: Carga NP +25. En Ortinax/Paladín: 14 de Bloqueo. *(+: +35 / 18)*
43. **Simulacro de Entrenamiento** — Habilidad, 1⚡: roba 2. Gana 3 de Bloqueo. *(+: roba 3, gana 5)*
44. **Muro Móvil** — Habilidad, 1⚡: 8 de Bloqueo. Tu próximo ataque este turno hace +8 de daño. *(+: 11 y +11)*

**Poderes (6)**
45. **Voluntad de Acero** — Poder, 1⚡: al final de tu turno, gana 4 de Bloqueo. *(+: 6)*
46. **Doctrina del Muro** — Poder, 1⚡: la primera carta de Bloqueo de cada turno otorga Carga NP +10. *(+: +15)*
47. **Munición Conceptual** — Poder, 2⚡: tus ataques eliminan 1 buff del enemigo golpeado (1 vez por turno). *(+: coste 1⚡)*
48. **Guardiana Incansable** — Poder, 1⚡: Intercepción 4 permanente. *(+: 6)*
49. **Servomotores del Ortinax** — Poder, 1⚡: al final del turno, si jugaste 2+ ataques: gana 6 de Bloqueo. *(+: 8)*
50. **Biblioteca de Chaldea** — Poder, 2⚡: roba 1 carta adicional cada turno. *(+: coste 1⚡)*

### Raras (20)

**Ataques (6)**
51. **BLACK BARREL — Disparo Pleno** — Ataque NP, 2⚡ (gasta Carga 50): 35 de daño **Black Barrel**. **Sobrecarga**: elimina TODOS los buffs del objetivo. *(+: 45 de daño)*
52. **Castigo de la Mesa Redonda** — Ataque, 3⚡: daño a TODOS los enemigos igual a tu Bloqueo. *(+: coste 2⚡)*
53. **Asalto del Paladín** — Ataque, 2⚡: 13 de daño ×2. Gana 10 de Bloqueo. Solo jugable en Ortinax/Paladín. *(+: 16×2 y 12)*
54. **Golpe del Alba** — Ataque, 1⚡: 12 de daño. Si mata al objetivo: Carga NP +50 y cura 3 HP. Exhaust. *(+: 16 de daño)*
55. **Réplica de Rhongomyniad** — Ataque NP, 3⚡ (gasta Carga 100): 60 de daño **Black Barrel**. Aplica 3 de Vulnerable. Exhaust. *(+: 75 de daño)*
56. **Voto del Caballero** — Ataque, 1⚡: 8 de daño. Si tienes 20+ de Bloqueo: gana 2 de Fuerza. *(+: 11 de daño, 3 de Fuerza)*

**Habilidades (9)**
57. **LORD CHALDEAS** — Habilidad NP, 2⚡ (gasta Carga 50): 24 de Bloqueo con **Baluarte**. **Sobrecarga**: +12 adicional. *(+: 30)*
58. **LORD CAMELOT** — Habilidad NP, 3⚡ (gasta Carga 100): 35 de Bloqueo con **Baluarte**. Gana 3 de Fuerza. **Sobrecarga**: Intercepción 12 (3 turnos). *(+: 45 de Bloqueo, 4 de Fuerza)*
59. **Vocación de Paladín** — Habilidad, 2⚡: entra en forma **Paladín** (permanente). Cura 6 HP. Exhaust. *(+: coste 1⚡, cura 10)*
60. **El Milagro de Fou** — Habilidad, 1⚡: este combate, la primera vez que fueras a morir: quedas a 1 HP y ganas 25 de Bloqueo. Exhaust. *(+: quedas a 12 HP)*
61. **Última Orden de Chaldea** — Habilidad, 0⚡: Carga NP +50. Exhaust. *(+: +75)*
62. **Fortaleza Utópica** — Habilidad, 2⚡: gana Bloqueo igual a tu Carga NP ÷ 2 (no la consume). Exhaust. *(+: ÷ 1.5)*
63. **Herencia de Galahad** — Habilidad, 1⚡: añade un **Mold Camelot+** a tu mano (cuesta 0 este turno). Exhaust. *(+: añade 2)*
64. **Pared Absoluta** — Habilidad, 2⚡: este turno, tu HP no puede bajar. Exhaust. *(+: coste 1⚡)*
65. **Campo de Nieve del Adiós** — Habilidad, 2⚡: 20 de Bloqueo con **Baluarte**. Carga NP +20. Exhaust. *(+: 26 y +25)*

**Poderes (5)**
66. **Demi-Servant** — Poder, 2⚡: al inicio de cada turno, gana 5 de Bloqueo con **Baluarte**. *(+: 7)*
67. **Castillo de la Utopía Lejana** — Poder, 3⚡: TODO tu Bloqueo persiste entre turnos. *(+: coste 2⚡)*
68. **Corazón del Homúnculo** — Poder, 1⚡: cada vez que cambias de forma: roba 2 y Carga NP +10. *(+: roba 3, +15)*
69. **Promesa a Senpai** — Poder, 2⚡: cuando un golpe enemigo no atraviese tu Bloqueo: Carga NP +6. *(+: +9)*
70. **Espíritu Pionero de las Estrellas** — Poder, 3⚡: la primera carta NP de cada combate no consume Carga. *(+: coste 2⚡)*

### Ajustes hechos durante la implementación (v0.1)

- **Reagrupar**: condición "si no tenías Bloqueo" (en vez de "primer Bloqueo del turno").
- **Juicio del Caballero / Amalgam Goad / LORD CAMELOT (Sobrecarga)**: la Intercepción dura "este turno" (no hay sistema de duración de 2-3 turnos en v1).
- **Lentes de Mash**: el juego base no tiene scry → "roba 1 (+2) y Carga +5 (+8)".
- **Fortaleza Utópica**: Bloqueo = 50% (+75%) de la Carga.
- **Sobrecarga**: se evalúa con la Carga exactamente en 100 ANTES de pagar; las NP de coste 100 siempre están sobrecargadas.
- **Black Barrel (keyword)**: cada impacto BB ignora Bloqueo Y remueve 1 buff (Ráfaga remueve hasta 2).
- **Formación Defensiva**: aplica a cartas que ganan Bloqueo (flag `GainsBlock` del juego).

### Cartas meme de la comunidad (agregado en v0.1)

Basadas en el canon de jerga de la comunidad china (Mooncell, "FGO黑话·梗"). **Incoloras** (aparecen en runs de cualquier personaje):

| Carta | Meme | Efecto |
|---|---|---|
| **Golden Apple** (PC) | 金苹果 — recargar AP para seguir grindeando | 1⚡: +2 Energía, roba 1. Exhaust |
| **Extra Spicy Mapo Tofu** (C) | 麻婆豆腐 — el tofu de Kotomine | 1⚡: cura 7, pierde 2 HP. Exhaust |
| **Insufficient QP!** (C) | QP不足 — nunca alcanza | 0⚡: +18 oro. Exhaust |
| **10-Pull Summon** (PC) | 十连/歪了 — la tirada del gacha | 1⚡: 1 carta aleatoria de tu pool a la mano, coste 0 este turno. Exhaust |
| **Black Keys** (C) | 黑键 — siempre hay más en la sotana | 0⚡: 3 daño; añade una copia al descarte |
| **EXP Ember** (PC) | 种火/狗粮 — todo es comida de experiencia | 1⚡: mejora 1 carta aleatoria de tu mano este combate. Exhaust |
| **Palingenesis** (R) | 圣杯转临 — el Grial sube el nivel | 2⚡: +4 HP máx y cura 4. Exhaust |

**En el pool de Mash** (memes de ella):

| Carta | Meme | Efecto |
|---|---|---|
| **Fou (Beast IV?)** (PC) | Fou = Primate Murder dormido | 0⚡: 3 daño; si mata: cura 5 y Carga +20. Exhaust |
| **Multipurpose Shield** (C) | 盾娘 + el manga de Riyo: escudo tabla/sartén/trineo | 1⚡: 6 Bloqueo, roba 1, Carga +5 |

Arte: CEs oficiales exactos ("Extremely Spicy Mapo Tofu", "Golden Apple", "Mona Lisa" para QP, "The Black Grail", "Learning with Manga! FGO", etc.).

## 6. Reliquias del personaje

| Reliquia | Tier | Efecto |
|---|---|---|
| **Fragmento de la Mesa Redonda** | Inicial | Al final del turno conserva hasta 10 de Bloqueo. |
| **Lord Camelot (restaurado)** | Jefe (reemplaza inicial) | Conserva hasta 25 de Bloqueo entre turnos. |
| **Amuleto de Fou** | Jefe/Raro | La primera vez que morirías en cada combate, quedas a 1 HP. |
| **Lentes de repuesto** | Común | Al inicio de cada combate, roba 1 carta adicional. |
| **Núcleo del Ortinax** | Tienda | Empiezas cada combate con 40 de Carga NP. |
| **Diario de la A-Team** | Evento | Al subir de piso, gana 1 HP máximo. |

## 7. Modelo 2D y animaciones — estado real y plan

### Lo que ya tenemos (descargado en `assets/reference/`)

| Archivo | Contenido |
|---|---|
| `charagraph/mash_base_1.png`, `mash_base_final.png` | Arte oficial (ascensiones base) — retrato / placeholder |
| `charagraph/mash_ortinax.png` | Arte de la forma Ortinax (Refurbished) |
| `charagraph/mash_paladin.png` | Arte de la forma Paladín |
| `battlesprites/mash_base_atlas.png` | **Atlas de partes del modelo de batalla** (base): cabeza, pelo, Lord Camelot, espada, torso, extremidades |
| `battlesprites/mash_ortinax_atlas.png` | Atlas de partes de Ortinax |
| `battlesprites/mash_paladin_atlas.png` | Atlas de partes de Paladín |

Fuente: API pública de [Atlas Academy](https://atlasacademy.io/) (`https://api.atlasacademy.io/nice/JP/servant/1`, IDs: base `800100`, Ortinax `800150`, Paladín `800200`).

### El problema de las animaciones

El modelo de batalla de FGO **no es un spritesheet de frames**: es un **puppet 2D de Unity** ("Modified Unity3D" según el manifest) — las partes se riggean y animan con datos de Unity. **No se pueden copiar las animaciones tal cual a Godot.** Opciones:

1. **Sprite cuasi-estático (v1, recomendado)**: pose desde la charagraph o compuesta con partes del atlas; animar con `AnimationPlayer2D` (respiración, lunge, flash de daño, pose de NP). Es lo que hacen mods existentes (referencia local: `JeanneAlter`).
2. **Re-rig del puppet en Godot**: partes del atlas + `Skeleton2D` + `AnimationPlayer`. Calidad alta, esfuerzo medio.
3. **Extracción real**: bajar el asset bundle (`https://static.atlasacademy.io/JP/Servants/{id}/`) y abrirlo con **AssetStudio** para exportar rig y clips; recrear en Spine (StS2 trae `libspine_godot`) o Godot. Máxima fidelidad, máximo esfuerzo.

**Plan**: v1 con opción 1 → migrar a opción 2 con el kit estable. Cada forma tiene su atlas: el cambio de forma cambia el visual en combate.

> ⚠️ Nota de IP: assets de FGO en un mod gratuito de fans es práctica común (hay varios mods de FGO publicados), pero no se puede monetizar y conviene acreditar a Atlas Academy y TYPE-MOON/Lasengle.

### Imágenes de cartas (próxima fase)
Con el pool cerrado (sección 5), el siguiente paso es asignar arte a las 72 cartas: recortes de charagraphs/atlas oficiales para las cartas de firma y NPs, y arte generado/compuesto coherente para el resto. Se trabajará carta por carta sobre la lista de la sección 5.

## 8. Orden de implementación

1. ~~Instalar .NET SDK~~ ✅ (10.0.301) · ~~Templates~~ ✅ (`Alchyr.Sts2.Templates` v2.4.3, template `alchyrsts2charmod`) · **Falta: MegaDot** (https://megadot.megacrit.com/) — necesario recién al hacer Publish.
2. **Esqueleto del mod**: proyecto con template de personaje, manifest `id: "MashShielder"` (no cambiar nunca), dependencia `BaseLib`.
3. **Personaje mínimo jugable**: stats, color, mazo inicial, reliquia inicial, retrato con charagraph. Probar en main v0.103.x.
4. **Carga NP** (energy counter de BaseLib) + keyword **Baluarte**.
5. **Comunes completas** → probar balance de un acto.
6. **Sistema de Formas** + poco comunes.
7. **NPs, raras y Black Barrel**.
8. **Reliquias propias** + localización ES/EN.
9. **Imágenes de cartas** (fase de arte, sección 7).
10. **Visual v2**: rig de partes con Skeleton2D, animación por forma.
11. **Publicar**: mods local → Steam Workshop.
