// Workflow generico de implementacion de un personaje FGO. args = { short, projId, designFile }
// Genera powers/cartas/reliquias/character/loc desde docs/DESIGN-<X>.md, usando SiegfriedSaber como
// patron que compila + FGOCore. Devuelve { files:[{path,content}] } para escribir + compilar.
export const meta = {
  name: 'implement-character',
  description: 'Genera la implementacion C# completa de un personaje FGO desde su DESIGN doc',
  phases: [
    { title: 'Spine', detail: 'powers + character + extensions + interfaces + manifest' },
    { title: 'Content', detail: 'cartas por rareza + reliquias' },
    { title: 'Loc', detail: 'loc trilingue' },
  ],
}

const ROOT = 'f:/Programs/SlayTheSpire2-SmashFromFGO'
const A = (typeof args === 'string') ? JSON.parse(args) : (args || {})
const SHORT = A.short                 // ej "Gilgamesh"
const PROJ_ID = A.projId              // ej "GilgameshArcher"
const DESIGN = `${ROOT}/${A.designFile}` // ej "docs/DESIGN-GILGAMESH.md"
const REF = `${ROOT}/SiegfriedSaber`
const FGOCORE = `${ROOT}/FGOCore/FGOCoreCode`
const PROJ = `${ROOT}/${PROJ_ID}`
const NS = `${PROJ_ID}.${PROJ_ID}Code`
const LOCDIR = `${PROJ}/${PROJ_ID}/localization`
const UPPER = PROJ_ID.toUpperCase()

const COMMON = `Sos ingeniero C# implementando el personaje **${SHORT}** como mod de Slay the Spire 2 (MegaDot/Godot 4.5.1, BaseLib 3.2.1, .NET 9).

CONTEXTO CRITICO — leelo con Read antes de generar:
- Diseno COMPLETO a implementar: ${DESIGN}
- Proyecto de REFERENCIA que COMPILA (copia sus patrones EXACTOS de API — usings, DynamicVar, DamageCmd/PowerCmd/CreatureCmd, [Pool], firma de OnPlay/OnUpgrade, SimpleLoc): ${REF}/SiegfriedSaberCode (mira Cards/, Powers/, Relics/, Character/) y su loc en ${REF}/SiegfriedSaber/localization/{eng,esp,zhs}/.
- Mecanicas FGOCore disponibles (NO reinventar): ${FGOCORE} — Np/ (NpChargePower 0-300, NpCharge.ConsumeAllForNpCard, GaugeFilled/Dropped, OverchargeBlessingPower), Stars/ (CritStarsPower, CritReadyPower), Forms/ (FormPower, FormSwitch, FormVisuals, FormShiftedPower, IFormChangeListener), Bond/ (BondRelic), Guts/, Curses/. Para formas tambien mira el uso real en ${ROOT}/Tiamat/TiamatCode/Powers/Forms/ y ${ROOT}/Tiamat/TiamatCode/Relics/SeaOfLifeWomb.cs.
- ESQUELETO YA EXISTENTE en ${PROJ}/${PROJ_ID}Code (NO lo regeneres, USALO): Cards/${SHORT}Card.cs (base, ya trae [Pool(${SHORT}CardPool)] + portrait paths), Powers/${SHORT}Power.cs (base, iconos), Relics/${SHORT}Relic.cs (base, [Pool(${SHORT}RelicPool)] + iconos), Character/${SHORT}CardPool.cs + ${SHORT}RelicPool.cs + ${SHORT}PotionPool.cs (pools OK), Cards/Basic/Strike.cs + Defend.cs (basicas genericas OK), Extensions/StringExtensions.cs (paths), MainFile.cs (ModId=${PROJ_ID}, ResPath). Namespace raiz: ${NS}.

REGLAS DURAS:
- Cada carta extiende ${SHORT}Card; cada power extiende ${SHORT}Power (o FGOCorePower si necesita mas control); cada reliquia extiende ${SHORT}Relic. El [Pool] se HEREDA de la base — NO re-decores.
- Las cartas NP usan NpCharge.ConsumeAllForNpCard (mira Siegfried/Balmung.cs). Formas = FormPower + FormSwitch.
- SimpleLoc: descripcion con prefijo #, DynamicVars como !D! !B!, *term para keyword tooltip, +texto+ solo-upgrade. Los CanonicalVars definen los DynamicVar (DamageVar/BlockVar/custom PowerVar con nombre EXPLICITO).
- NO inventes APIs: si no la ves en SiegfriedSaber o FGOCore, no la uses. Ante la duda, copia el patron de Siegfried EXACTO.
- Numeros EXACTOS del diseno.`

const FILES_SCHEMA = {
  type: 'object', additionalProperties: false, required: ['files', 'notes'],
  properties: {
    files: { type: 'array', items: { type: 'object', additionalProperties: false, required: ['path', 'content'],
      properties: { path: { type: 'string' }, content: { type: 'string' } } } },
    notes: { type: 'string' },
  },
}

phase('Spine')
const spine = await agent(
  `${COMMON}

TU FASE = ESPINA. Genera SOLO las piezas COMPARTIDAS que las cartas referencian:
1. Todos los POWERS de ${SHORT} (incluidas formas via FormPower si el diseno las tiene, crit, motores propios). Si reusas FormVisuals/GaugeFilled, devolve un MainFile.cs actualizado que enganche lo necesario (FormVisuals.RegisterFrames de los frames de ${PROJ}/character/, NpCharge.GaugeFilled si hay ulti auto-manifestada).
2. Las INTERFACES del mod si el diseno las necesita.
3. Las EXTENSIONS propias (helpers).
4. Character/${SHORT}.cs REESCRITO: HP (del diseno §7), Color, StartingDeck (4 Golpe + 4 Defensa + 2 firmas que SI definas como basicas en Cards/Basic/), StartingRelics (la starter), pools (ya existen), CustomVisualPath=res://${PROJ_ID}/character/${SHORT.toLowerCase()}_visuals.tscn, deja los placeholders _char_name en los Custom*Path de icono/select como en Siegfried.
5. GlobalUsings.cs ajustado (quita FGOCore.FGOCoreCode.DragonScales; agrega Forms/etc si se usan).
6. Las 2 cartas FIRMA basicas del StartingDeck (Cards/Basic/).

Devolve TODOS esos archivos (path absoluto bajo ${PROJ} + content COMPLETO) y en notes el MANIFEST: cada power/interface/helper con nombre+uso, + la lista COMPLETA de cartas planeadas por rareza (id + efecto en una linea) del diseno.`,
  { label: `${SHORT}:spine`, phase: 'Spine', schema: FILES_SCHEMA })

const manifest = spine?.notes || ''

phase('Content')
const RARITIES = ['Common', 'Uncommon', 'Rare', 'Special']
const content = await parallel([
  ...RARITIES.map(r => () => agent(
    `${COMMON}

TU FASE = CONTENIDO, rareza **${r}**. Genera TODAS las cartas de esa rareza del pool de ${SHORT}.

MANIFEST de la ESPINA (referencia estos nombres EXACTOS):
${manifest}

Cada carta = archivo .cs en ${PROJ}/${PROJ_ID}Code/Cards/${r}/<Nombre>.cs, extiende ${SHORT}Card, patron EXACTO de las cartas equivalentes de Siegfried. Devolve los archivos. En notes, lista los ids de carta + sus DynamicVars (para Loc).`,
    { label: `${SHORT}:${r}`, phase: 'Content', schema: FILES_SCHEMA })),
  () => agent(
    `${COMMON}

TU FASE = CONTENIDO, **RELIQUIAS**. Genera TODAS las reliquias de ${SHORT} (starter + pool). Cada una = archivo .cs en ${PROJ}/${PROJ_ID}Code/Relics/<Nombre>.cs, extiende ${SHORT}Relic, patron EXACTO de ${REF}/SiegfriedSaberCode/Relics/.

MANIFEST de la ESPINA:
${manifest}

Devolve los archivos. En notes, lista los ids de reliquia.`,
    { label: `${SHORT}:relics`, phase: 'Content', schema: FILES_SCHEMA }),
])

const contentNotes = content.filter(Boolean).map(c => c.notes).join('\n\n')

phase('Loc')
const loc = await agent(
  `${COMMON}

TU FASE = LOCALIZACION TRILINGUE. Genera los 5 JSON x 3 idiomas para TODO ${SHORT}:
${LOCDIR}/{eng,esp,zhs}/{cards,powers,relics,characters,ancients}.json

Formato EXACTO = el de Siegfried (${REF}/SiegfriedSaber/localization/) — claves ${UPPER}-<ENTRY>.title/.description, SimpleLoc, prefijo ${UPPER}-. Cubri TODAS las cartas/powers/reliquias.

MANIFEST espina:
${manifest}

NOTAS de contenido (ids + DynamicVars):
${contentNotes}

eng=ingles, esp=espanol latino, zhs=chino simplificado (Mooncell). characters.json = nombre+descripcion de ${SHORT}; ancients.json = {} si no hay Ancient. Devolve los 15 JSON (path absoluto + content).`,
  { label: `${SHORT}:loc`, phase: 'Loc', schema: FILES_SCHEMA })

const allFiles = [
  ...(spine?.files || []),
  ...content.filter(Boolean).flatMap(c => c.files || []),
  ...(loc?.files || []),
]
log(`${SHORT}: ${allFiles.length} archivos generados`)
return { files: allFiles, spineNotes: manifest }
