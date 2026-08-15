# REBALANCE — Morgan: la danza de formas accesible + 4 arquetipos legibles — propuesta 2026-08-15

Origen: reportes de 七煌夜 (Steam, 08-14): (1) "pocas cartas de cambio de forma, todas hacia la
del agua; muy difícil volver a la Reina Hada para pegar — el cambio de forma termina siendo poco
útil"; (2) "¿no había una carta que evita que la Maldición pierda stacks? cuando la Reina del
Invierno ataca igual se limpia a cero — ¿bug?". Encargo del usuario: rediseñar el mazo teniendo
en cuenta que StS vanilla diseña cada personaje para VARIOS arquetipos jugables.
> **DECISIÓN DEL USUARIO (2026-08-15): RE-POOL COMPLETO.** Se ofreció el parche mínimo M1-M5 de
> abajo y el usuario eligió rehacer el pool desde cero con panel de diseño (WORKFLOW-FGO §4.6.7).
> Este documento queda como DIAGNÓSTICO (las secciones 1-2 y la matriz §4 son la evidencia base);
> el diseño nuevo vive en `REDESIGN-MORGAN-V2.md` cuando el panel lo produzca. Los M1-M5 no se
> implementan por separado: el panel debe resolver esos mismos problemas estructuralmente.

Complementa el diagnóstico sobre el motor de [REDESIGN-MORGAN.md](REDESIGN-MORGAN.md).

## 1. Reporte 2 primero: NO es bug — es la Sentencia (pero el texto invita al malentendido)

`WinterQueenFormPower` tiene a la vez (a) `ICursePreserver` («tu Maldición no decae») y
(d) Sentencia («tus Ataques infligen daño extra = la Maldición del objetivo **y la consumen**»).
El "no decae" es el decaimiento pasivo de fin de turno; la Sentencia consume por diseño — es el
motor entero de Morgan (sembrar → detonar). El jugador leyó lo primero y no conectó lo segundo.
**Acción (legibilidad, 5 idiomas):** las pasivas de Reina Hada / Reina del Invierno pasan a decir
explícitamente «tu Maldición no decae *al final del turno*; tus Ataques la *detonan* (la
consumen como daño extra)». Sin cambio de mecánica.

## 2. Diagnóstico del reporte 1: la asimetría de los interruptores (verificada)

| Destino | Cartas drafteables | Coste | Extra |
|---|---|---|---|
| → Bruja de la Lluvia (sembrar) | `RainChant` (COMÚN 1⚡ + Bloqueo), `RainWitchForm` (PC **0⚡** + NP) | barato y frecuente | + la Metamorfosis gratis del cetro (1/combate) suele gastarse acá |
| → Reina Hada (detonar) | `FairyQueenForm` (PC **1⚡** + Maldición) | LA ÚNICA, cara y poco común | — |
| Toggle | `MirrorClansTrick` (PC 1⚡, roba 2) | poco común | — |
| → Invierno (permanente) | `WinterCoronation` (RARA) | clímax | — |

Arrancás EN Reina Hada (cetro) → el primer cambio natural es hacia la Bruja (gratis) → para
COSECHAR dependés de 1 poco común. Con mala suerte de draft, la mitad del motor no aparece en
toda la run — exactamente el reporte ("可能是我运气太差没遇见"). La danza es la identidad
(REDESIGN §B) y su plomería es de rareza poco común: defecto estructural, no de números.

## 3. Cambios propuestos (ningún ID se renombra; saves intactos)

| # | Cambio | Antes | Después | Razón |
|---|---|---|---|---|
| M1 | `FairyQueenForm` | PC, **1⚡** | PC, **0⚡** | Paridad exacta con `RainWitchForm` (0⚡). Volver a cosechar no puede ser más caro que ir a sembrar. |
| M2 | `MirrorClansTrick` | PC, roba 2 | **COMÚN**, roba 2 | El toggle es la carta que arregla "quedé atrapado en una forma" — debe aparecer en cada run (conectividad de comunes ≥90%, regla 4.6). |
| M3 | `QueensScepter` (starter) | Metamorfosis gratis 1/combate (turno 1) | además, **se re-arma al llegar a 100 NP** | La ventana NP invita a re-entrar a la danza en el clímax; ata los dos recursos del kit sin economía nueva. |
| M4 | Pasivas Reina Hada / Invierno (loc) | «tu Maldición no decae» | «…no decae al final del turno; tus Ataques la detonan (consumen)» | Reporte 2. Solo texto, 5 idiomas. |
| M5 | `CurseOfCernunnos` (rara) | «tu Maldición no baja tras hacer daño» | «**tus detonaciones consumen solo la MITAD de la Maldición** (redondeo arriba)» | Reconversión pendiente desde REDESIGN §F (hoy es ~redundante con el no-decay de Bruja/Invierno). Es el puente rara entre danza y attrition: detonás sin vaciar el campo. |

Números que NO se tocan: todo el resto del pool. El motor (siembra→Sentencia, ventana NP,
adversidad) funciona; el reporte señala plomería y legibilidad, no curva de poder.

## 4. Multi-arquetipo (el encargo): matriz de cobertura del pool ACTUAL tras M1-M5

El pool ya contiene 4 arquetipos drafteables; M1-M3 completan el único con gap estructural.

| Arquetipo | Motor | Cartas clave (ya existentes) | Debilidad que lo castiga |
|---|---|---|---|
| **A. La Danza** (sembrar→detonar) | formas + Sentencia | interruptores (M1-M3), QueensScorn, TyrantsSweep, AlbionsBreath, MirrorStrike, FinalCollection, SovereignOfTwoFaces | requiere secuenciar; enemigos que castigan turnos sin defensa |
| **B. Invierno Perpetuo** (attrition, acampar Bruja/Invierno) | DoT de Maldición sin decay + spread | CursedRain, MistVeil, WildHuntCharge, FairyEyes, PerpetualWinter, ItemConstruction, Hailstorm, ExtraordinaryTax, WinterThorns, Barghest | −2 de daño en Bruja (lento vs élites rápidas); jefes que limpian debuffs |
| **C. Nuke de NP/Sobrecarga** | generación NP → ventana + cartas NP | ScepterBlow, TwinReplicas, TaxCollection, RoyalEdict, FaeBloodPact, FairyOfTheRainland, Roadless/Rhongomyniad/Londinium | pico diferido; frontload flojo |
| **D. Adversidad** (sangre/Guts) | HP propio → NP/daño | MadLunge, TyrantsBlood, QueensSacrifice, TyrantsLance, AdversitysFury, SaviorsVengeance, CharismaOfAdversity, MadnessEnhancement, Guts ×2, AHomeWithMorgan | jugar al filo de la vida; Maldición enemiga acelera tu reloj |

- Se DESCARTA un re-pool desde cero: rompería la memoria de los jugadores sobre ~80 cartas ya
  publicadas y descartaría un motor que el propio reporte no cuestiona (cuestiona el acceso).
  Si el playtest post-M1-M5 sigue mostrando un solo arquetipo dominante, el siguiente paso sería
  el panel de diseño completo (WORKFLOW-FGO §4.6.7).
- Hallazgo lateral (sin acción): `QuickMorgan` conserva un rider de 20★ globales — el REDESIGN
  §G decía que las Estrellas salieron de Morgan, pero Critical v2 las volvió GLOBALES después y
  el rider es coherente con eso (Morgan puede pagar el crítico global de 50★). El doc viejo
  quedó desactualizado, el código está bien.

## 5. Implementación (cuando se apruebe)

1. M1/M2: costo y rareza en los ctors; M5: efecto nuevo en `CurseOfCernunnos` + power.
2. M3: `QueensScepter` re-arma la Metamorfosis en `GaugeFilled`.
3. M4 + loc de M1/M2/M5 ×5 idiomas; `audit_simpleloc` + paridad + matriz MAIN/BETA.
4. Morgan v0.1.19; FGOCore no cambia. Publish/upload con orden explícita.

**Knobs de playtest:** si la danza queda demasiado fluida (toggle común + 0⚡), subir
`MirrorClansTrick` a robar 1; si M3 acelera de más los combates largos, re-armar cada 2 ventanas.
