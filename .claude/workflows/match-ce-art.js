export const meta = {
  name: 'match-ce-art',
  description: 'Matchea temas de cartas con arte oficial de Craft Essences (catálogo Atlas Academy)',
  whenToUse: 'Al asignar arte a cartas de un mod de personaje FGO. args = { tsvPath?, batches: [{label, cards: [[file, themeEn], ...]}] }. Requiere el TSV del catálogo CE (collectionNo\\tassetId\\tname); por defecto usa el de assets/reference/ce/ce_names.tsv. Devuelve {matches:[{file, collectionNo, ...}]} — mapear collectionNo→assetId con el TSV y bajar con tools/make_card_art.ps1.',
  phases: [
    { title: 'Match', detail: 'un agente por lote de cartas, busca CEs por tema en el catálogo' },
    { title: 'Verify', detail: 'dedup entre lotes con alternativas' },
  ],
}

const TSV = (args && args.tsvPath) || 'f:/Programs/SlayTheSpire2-SmashFromFGO/assets/reference/ce/ce_names.tsv'
const BATCHES = (args && args.batches) || []
if (BATCHES.length === 0) {
  return { error: 'args.batches vacío: pasar [{label, cards: [[fileName, themeDescriptionEnglish], ...]}]' }
}

const MATCH_SCHEMA = {
  type: 'object',
  properties: {
    matches: {
      type: 'array',
      items: {
        type: 'object',
        properties: {
          file: { type: 'string' },
          collectionNo: { type: 'number' },
          ceName: { type: 'string' },
          reason: { type: 'string' },
          alternates: { type: 'array', items: { type: 'number' } },
        },
        required: ['file', 'collectionNo', 'ceName', 'reason', 'alternates'],
      },
    },
  },
  required: ['matches'],
}

phase('Match')
const results = await parallel(BATCHES.map(b => () => agent(
  `You are matching Slay the Spire 2 card themes to official Fate/Grand Order Craft Essence (CE) artwork for a character mod.

The full CE catalog is a TSV file at ${TSV} with columns: collectionNo <TAB> assetId <TAB> englishName. Use Grep with case-insensitive patterns (-i) to search it by keywords, and Read slices to browse.

For EACH of these cards, pick the best-fitting CE by name/theme (the art gets cropped for the card portrait, so the CE subject must match the card theme).

Cards (file -> theme):
${b.cards.map(c => `- ${c[0]}: ${c[1]}`).join('\n')}

Rules:
- Prefer CEs featuring the mod character or their lore keywords when thematically right.
- collectionNo and ceName MUST be copied EXACTLY from the TSV (no invented entries).
- All picks within your batch must be DISTINCT CEs.
- Avoid the generic "Heroic Portrait:" series except for portrait-like cards.
- Give 2 alternate collectionNos per card (also real, from the TSV).
- reason: one short sentence.

Return via StructuredOutput: { matches: [{file, collectionNo, ceName, reason, alternates}] } with exactly ${b.cards.length} entries.`,
  { label: `match:${b.label}`, phase: 'Match', schema: MATCH_SCHEMA }
)))

phase('Verify')
const all = results.filter(Boolean).flatMap(r => r.matches)
const used = new Set()
const final = []
const conflicts = []
for (const m of all) {
  if (!used.has(m.collectionNo)) {
    used.add(m.collectionNo)
    final.push(m)
  } else {
    const alt = (m.alternates || []).find(a => !used.has(a))
    if (alt) {
      used.add(alt)
      final.push({ ...m, collectionNo: alt, ceName: '(alternate ' + alt + ')', reason: m.reason + ' [alt por duplicado]' })
      conflicts.push(m.file + ': duplicado -> alt ' + alt)
    } else {
      final.push({ ...m, duplicate: true })
      conflicts.push(m.file + ': duplicado SIN alternativa — resolver a mano')
    }
  }
}
log(final.length + ' cartas matcheadas, ' + conflicts.length + ' conflictos')
return { matches: final, conflicts }
