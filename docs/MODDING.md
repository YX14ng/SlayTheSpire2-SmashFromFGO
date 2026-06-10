# Guía de Modding — Slay the Spire 2 (mod de personaje)

Información recopilada el 2026-06-09 para arrancar el desarrollo de un mod de personaje para Slay the Spire 2.

## Contexto del juego

- **Slay the Spire 2** está en Early Access y corre sobre **Godot 4** (un fork propio de Mega Crit llamado **MegaDot**), con la lógica del juego en **C#/.NET**. Ya no es Java/libGDX como el primer juego.
- Instalación local detectada: `C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2`, versión **v0.103.3** (build del 2026-05-29).
- Soporta **Steam Workshop** (desde el Patch 54) y carga manual de mods desde la carpeta `mods/` del juego.
- El juego usa **Spine** para animaciones de criaturas (`libspine_godot...dll` en la carpeta del juego) y Godot `AnimationPlayer`/`AnimationTree` vía BaseLib.

## Formato de un mod

Cada mod instalado son 3 archivos en `Slay the Spire 2/mods/<NombreMod>/`:

| Archivo | Contenido |
|---|---|
| `<NombreMod>.json` | Manifiesto del mod |
| `<NombreMod>.dll` | Código C# compilado |
| `<NombreMod>.pck` | Assets empaquetados con MegaDot (imágenes, escenas, audio) |

Ejemplo real de manifiesto (del mod JeanneAlter, instalado localmente):

```json
{
  "id": "JeanneAlter",
  "name": "...",
  "author": "...",
  "description": "...",
  "version": "v1.0.0",
  "has_pck": true,
  "has_dll": true,
  "dependencies": ["BaseLib"],
  "affects_gameplay": false
}
```

> No modificar el `id` después de elegirlo: determina los nombres de archivo que el juego intenta cargar.

## Toolchain estándar (ecosistema BaseLib, el más usado)

1. **.NET 9.0 SDK** — descargar el SDK completo (no solo el runtime) desde Microsoft. ⚠️ **En esta máquina solo hay runtime, falta instalar el SDK** (`dotnet --list-sdks` devuelve vacío).
2. **MegaDot** — el build de Godot de Mega Crit, descarga oficial: <https://megadot.megacrit.com/> (elegir la versión que corresponda a la versión del juego).
3. **IDE** — Rider (recomendado por el template, porque Godot necesita `.sln`) o Visual Studio. VS Code con la extensión C# también funciona.
4. **BaseLib** — framework base del que dependen casi todos los mods de contenido. ✅ **Ya instalado** en la carpeta `mods/` del juego (igual que ModConfig). Como dependencia de desarrollo se referencia por NuGet: `Alchyr.Sts2.BaseLib` (última versión v3.2.0, 2026-06-04).
5. **Templates de proyecto**:
   ```
   dotnet new install Alchyr.Sts2.Templates
   ```
   Incluye 3 plantillas: **Slay the Spire 2 Character** (la nuestra), Content, y Empty Mod.

## Setup del proyecto de personaje (template de Alchyr)

1. Crear solución nueva con la plantilla "Slay the Spire 2 Character".
   - Nombre **sin espacios** (PascalCase).
   - Activar **"Put solution and project in same directory"** — obligatorio para que funcione con Godot.
2. Configurar `Directory.Build.props`: ruta al ejecutable de MegaDot/Godot y ruta del juego si no se autodetectan.
3. Compilar. Que el template de personaje muestre **errores de localización es señal de setup correcto**: se resuelven con Alt+Enter → "Generate localization" y moviendo los textos a los JSON de localización. El paquete `Alchyr.Sts2.ModAnalyzers` ayuda con esto.
4. **Build** (martillo) compila solo código. **Publish** (clic derecho al proyecto → Publish → Local folder) compila el `.dll`, genera el `.pck` con los assets y copia `.dll`/`.pck`/`.json` a la carpeta `mods/` del juego. **Cualquier cambio que no sea código (localización, imágenes, escenas) requiere Publish.**

## Qué provee BaseLib para un personaje

- `CustomCharacterModel` (personaje), `CustomCardModel` (cartas), `CustomRelicModel` (reliquias), `CustomEventModel`/`CustomEncounterModel`, actos, orbes, poderes temporales.
- Prefijado automático de IDs (modelos que implementan `ICustomModel`), enums y keywords custom, contador de energía propio.
- Visuales de criatura con Godot `AnimationPlayer`/`AnimationTree`/`AnimationPlayer2D`.
- Localización: "In-Code", "Simplified", tooltips con variables dinámicas; texto faltante loguea error en vez de crashear.
- Configuración declarativa de mod y comunicación entre mods.
- El pool de cartas del personaje custom aparece automáticamente en el compendio.

## Documentación y recursos

| Recurso | URL |
|---|---|
| Template de mods (Alchyr) | <https://github.com/Alchyr/ModTemplate-StS2> |
| Guía de setup del template | <https://github.com/Alchyr/ModTemplate-StS2/wiki/Setup> |
| BaseLib (repo) | <https://github.com/Alchyr/BaseLib-StS2> |
| **BaseLib Wiki** (doc principal de la API) | <https://alchyr.github.io/BaseLib-Wiki/> |
| MegaDot (Godot de Mega Crit) | <https://megadot.megacrit.com/> |
| Nexus Mods StS2 | <https://www.nexusmods.com/slaythespire2> |
| Mod de ejemplo simple | <https://github.com/jiegec/STS2FirstMod> |
| Creador de personajes web (sin código) | <https://slay.spencerstiles.com/> |
| ModSmith (framework alternativo, VS Code) | <https://github.com/cpimhoff/Sts2-ModSmith> |

### Herramientas de IA para modding (relevante con Claude Code)

- **sts2-modding-mcp** (<https://github.com/elliotttate/sts2-modding-mcp>): servidor MCP con ~153 herramientas — autodetecta el juego, descompila el código C# del juego con ilspycmd, genera código de cartas/reliquias/poderes/personajes, compila y despliega el mod, e inspecciona el árbol de escenas del juego en vivo. Setup: clonar repo, venv de Python, `pip install`.
- **STS2MCP** (<https://github.com/Gennadiyev/STS2MCP>): expone el estado del juego en partida vía MCP (más orientado a jugar/testear agénticamente).

### Referencias locales

En `C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2\mods\` ya hay decenas de mods de personaje instalados que sirven de referencia de estructura y alcance, incluyendo personajes de FGO como **JeanneAlter**, y otros como Faust, Kafka, Mordekaiser, Painter, etc.

## Próximos pasos

1. Instalar el **.NET 9.0 SDK** (único prerequisito que falta en esta máquina).
2. Descargar **MegaDot** desde <https://megadot.megacrit.com/> (versión compatible con v0.103.x).
3. Elegir IDE (Rider recomendado) e instalar los templates: `dotnet new install Alchyr.Sts2.Templates`.
4. Crear el proyecto del personaje con la plantilla "Slay the Spire 2 Character" dentro de este repo.
5. Definir el diseño del personaje (identidad, mecánica central, colores de cartas, pool inicial) antes de producir assets.
