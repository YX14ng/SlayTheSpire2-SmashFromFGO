# TODO — personajes FGO

Lista canónica del roster implementado y de los próximos personajes considerados para el proyecto.
Agregar aquí una entrada antes de iniciar su documento `DESIGN-<PERSONAJE>.md` o asignarle un ID de
mod permanente.

## Pendientes

- Ningún personaje sin diseño asignado.

## En validación

- [x] **Astolfo — Rider**
  - Mod `AstolfoRider` implementado: Caprichos Q/A/B, Críticos v2, Evasión compartida, Hippogriff,
    68 recompensas (20/28/20), mazo inicial de 10, 1 NP, 35 powers y 12 reliquias.
  - Producción visual oficial `400400`, 180 cuadros animados, 80 retratos de modelo, iconos propios,
    selector, mapa, mercader y descanso. MAIN/BETA, SimpleLoc y PCK optimizado validados.
  - Pendiente: playtest dentro del juego, guardado/carga, cooperativo y decisión de Workshop.
  - Fuente de verdad: [`DESIGN-ASTOLFO.md`](DESIGN-ASTOLFO.md).

- [x] **Shuten Dōji — Assassin/Caster híbrida**
  - Mod `ShutenDouji` implementado: Sake, Cross, 68 recompensas, 5 iniciales propias, 2 NP, 12
    reliquias, español/inglés completos y paridad estructural en cinco idiomas.
  - Producción visual oficial `602100`: animaciones, 80 retratos de carta, iconos, selector, mapa,
    mercader y descanso. MAIN/BETA y el PCK optimizado pasan las auditorías automáticas.
  - Pendiente: playtest dentro del juego, guardado/carga, cooperativo y decisión de Workshop.

- [x] **Nagao Kagetora / Uesugi Kenshin**
  - Diseño, mod `KagetoraLancer`, Doctrina, ascensión, 68 cartas de recompensa, dos NP, 12
    reliquias y cinco idiomas implementados.
  - Las dos formas ya tienen animaciones oficiales. Pendiente: arte de cartas/reliquias,
    UI/feedback de Doctrina, VFX/audio y playtest dentro del juego (guardado/carga y cooperativo).

## Implementados

- [x] Mash Kyrielight
- [x] Morgan
- [x] Artoria Caster
- [x] Mordred
- [x] Gilgamesh
- [x] Okita Sōji
- [x] Oberon
- [x] Siegfried
- [x] Tiamat

## Flujo para cada personaje pendiente

- [ ] Investigar material canónico en japonés y contrastarlo con chino simplificado.
- [ ] Crear `docs/DESIGN-<PERSONAJE>.md` y revisar su distinción respecto del roster actual.
- [ ] Cerrar identidad mecánica, mazo inicial, pool, reliquias, NP y límites de balance.
- [ ] Registrar y producir assets oficiales con procedencia verificable.
- [ ] Implementar código y localización en español, inglés, chino simplificado, coreano y ruso.
- [ ] Validar MAIN/BETA, SimpleLoc, PCK, rendimiento, cooperativo y playtest antes de publicar.
