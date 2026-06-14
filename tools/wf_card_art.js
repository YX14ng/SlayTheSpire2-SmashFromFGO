// Arte de cartas para varios personajes FGO. args = [{projId, charName, hint}]
// Un agente por personaje: lee su eng/cards.json, deriva el filename (entry.lower()) y el tema
// (title+description), y matchea cada carta a un CE oficial del catalogo TSV. Devuelve {projId, matches}.
// Despues: escribir <projId>_cards.csv (file,assetId) y correr tools/make_card_art.ps1.
export const meta = {
  name: 'card-art-multi',
  description: 'Matchea las cartas de varios personajes FGO a arte CE oficial (lee cada cards.json)',
  phases: [{ title: 'Match', detail: 'un agente por personaje' }],
}

const ROOT = 'f:/Programs/SlayTheSpire2-SmashFromFGO'
const TSV = `${ROOT}/assets/reference/ce/ce_names.tsv`
const CHARS = (typeof args === 'string') ? JSON.parse(args) : (args || [])

const SCHEMA = {
  type: 'object', additionalProperties: false, required: ['projId', 'matches'],
  properties: {
    projId: { type: 'string' },
    matches: { type: 'array', items: { type: 'object', additionalProperties: false,
      required: ['file', 'assetId', 'ceName'],
      properties: { file: { type: 'string' }, assetId: { type: 'string' }, ceName: { type: 'string' } } } },
  },
}

phase('Match')
const results = await parallel(CHARS.map(c => () => agent(
  `Estás asignando arte oficial de Craft Essences (CE) de Fate/Grand Order a las cartas del personaje **${c.charName}** (mod de Slay the Spire 2).

1. Leé (Read) la loc de cartas: ${ROOT}/${c.projId}/${c.projId}/localization/eng/cards.json. Cada entrada es "${c.projId.toUpperCase()}-<ENTRY>.title" y ".description".
2. Para CADA carta (EXCLUÍ solo STRIKE y DEFEND — son básicas con placeholder):
   - file = <ENTRY> en minúsculas (ej. SLASH_OF_CLARENT -> "slash_of_clarent").
   - tema = el title + description (de qué va la carta).
3. El catálogo CE es un TSV en ${TSV} con columnas: collectionNo <TAB> assetId <TAB> englishName. Buscá con Grep -i por keywords del tema y elegí el CE que mejor calce visualmente (el arte se recorta para el retrato). Copiá el assetId (2da columna) EXACTO. NO inventes filas.
4. Preferí CEs del propio ${c.charName} o de su temática (${c.hint}) cuando calcen. Todos los assetId deben ser DISTINTOS dentro de tu personaje. Evitá la serie genérica "Heroic Portrait:" salvo cartas tipo retrato.

Devolvé via StructuredOutput { projId: "${c.projId}", matches: [{file, assetId, ceName}] } cubriendo TODAS las cartas no-básicas.`,
  { label: `art:${c.projId}`, phase: 'Match', schema: SCHEMA })))

return { results: results.filter(Boolean) }
