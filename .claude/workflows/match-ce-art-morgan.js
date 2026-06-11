export const meta = {
  name: 'match-ce-art-morgan',
  description: 'Matchea las cartas de Morgan con arte oficial de Craft Essences (lotes embebidos)',
  phases: [
    { title: 'Match', detail: 'un agente por lote de cartas, busca CEs por tema en el catálogo' },
    { title: 'Verify', detail: 'dedup entre lotes con alternativas' },
  ],
}

const TSV = 'f:/Programs/SlayTheSpire2-SmashFromFGO/assets/reference/ce/ce_names.tsv'

const BATCHES = [
  { label: 'firmas-comunes-ataques', cards: [
    ['lance_of_the_worlds_end', "Morgan's lance Rhongomyniad replica thrust, the World's End spear, cursed strike"],
    ['queens_mandate', 'the Fairy Queen Morgan issuing a royal command, regal decree, protective order'],
    ['replica_lance', 'a conjured replica of the lance Rhongomyniad, magical spear projectile'],
    ['cursed_bolt', 'a cursed magical bolt or hex projectile, dark fae magic'],
    ['queens_scorn', "Morgan's cold contempt, a queen looking down on the weak"],
    ['mad_lunge', 'berserk reckless charge attack, madness-fueled assault'],
    ['tyrants_sweep', 'a tyrant queen sweeping away all enemies, wide regal attack'],
    ['scepter_blow', "striking with a royal scepter or staff, queen's authority as weapon"],
    ['twin_replicas', 'two conjured spear replicas thrown at once, double magical lances'],
    ['royal_punishment', "royal execution or punishment of a criminal, queen's judgment"],
    ['tyrants_blood', "paying with one's own blood, self-wounding fury, blood of a tyrant"],
    ['queens_fury', "the Fairy Queen's wrath unleashed, furious regal attack"],
  ]},
  { label: 'comunes-habilidades', cards: [
    ['mist_veil', 'a veil of mist concealing and protecting, fog defense'],
    ['cursed_rain', 'cursed rain falling on all enemies, dark storm hex'],
    ['witchs_mark', "a witch's curse mark or hex sigil placed on a victim"],
    ['tax_collection', "collecting taxes or tribute, the queen's tax collector, levy of magical energy"],
    ['royal_edict', 'a royal edict or proclamation scroll, official decree'],
    ['protective_frost', "protective frost or ice armor, winter's defensive chill"],
    ['queens_gaze', 'an intimidating regal gaze that weakens enemies, piercing stare'],
    ['rain_chant', 'a chant or song of rain, rainy incantation, gentle rainfall magic'],
    ['winter_steel', 'winter steel armor or icy fortress wall, strong cold defense'],
    ['vassalage', 'vassals kneeling and serving their queen, oath of fealty'],
  ]},
  { label: 'poco-comunes-1', cards: [
    ['replica_barrage', 'a barrage of many conjured spears raining down, volley of magical lances'],
    ['barghests_fang', 'Fairy Knight Gawain Barghest, the monstrous black dog fairy knight, devouring fangs'],
    ['melusines_talon', 'Fairy Knight Lancelot Melusine, the dragon fairy, dragon claw strike'],
    ['baobhan_siths_shriek', 'Fairy Knight Tristan Baobhan Sith, the vampiric fae girl, piercing shriek'],
    ['wild_hunt_charge', 'the Wild Hunt charging, spectral fae cavalry assault'],
    ['storms_wrath', "a devastating winter storm's wrath, blizzard tempest attack"],
    ['adversitys_fury', 'fighting harder when wounded, fury born of adversity and desperation'],
    ['royal_execution', "a public royal execution, the queen's death sentence"],
    ['albions_breath', 'the great dragon Albion, dragon breath attack from the abyss'],
    ['mirror_strike', 'a strike reflected through a magic mirror, mirrored double attack'],
    ['charisma_of_yearning', "Morgan's Charisma of Yearning, the queen rallying through fear and longing, dark charisma aura"],
    ['protection_of_the_lake', "the Lady of the Lake's protection, Vivian blessing from sacred waters"],
    ['fairy_queen_form', 'Morgan as the Berserker Fairy Queen of Britain on her throne, tyrant queen transformation'],
    ['rain_witch_form', 'Aesc the Rain Witch, young Morgan with staff under the rain, Tonelico the savior caster'],
  ]},
  { label: 'poco-comunes-2', cards: [
    ['winter_decree', 'a decree of eternal winter, frost falling over the land by royal order'],
    ['embrace_of_the_lake', 'embraced and protected by the waters of the lake, Lady of the Water'],
    ['winter_thorns', 'thorns of ice that wound attackers, defensive frozen brambles'],
    ['curse_harvest', 'harvesting and amplifying curses, doubling dark hexes'],
    ['memory_of_the_ash_tree', 'the great ash world-tree of Orkney, nostalgic memory of home under the tree'],
    ['mirror_clans_trick', 'the Mirror Clan fae illusion trick, swapping reflections, divination mirror'],
    ['saviors_tears', 'the savior fairy weeping, healing tears of compassion'],
    ['call_of_the_fairy_knights', 'summoning the three Fairy Knights together, Tam Lin assembly'],
    ['fairy_of_the_rainland', 'the fairy of the rainy land Orkney, rain blessing magical energy'],
    ['madness_enhancement', 'berserker madness enhancement, losing oneself to violent frenzy'],
    ['fairy_eyes', 'magical fae eyes that see through everything, glowing supernatural sight'],
    ['territory_creation', "creating a magical territory or workshop, caster's domain"],
    ['item_construction', "magical item construction or crafting, witch's tools and potions"],
    ['winter_court', 'the winter fae court in session, courtiers of the Winter Queen'],
  ]},
  { label: 'raras-especiales', cards: [
    ['roadless_camelot', 'Morgan Berserker unleashing her Noble Phantasm, the unreachable white castle Camelot, queen of fae Britain in full power'],
    ['memory_of_londinium', 'Aesc summoning the holy sword memories of Londinium, raining glorious knight weapons, dreams of a utopian city'],
    ['rhongomyniad_rain', 'a rain of Rhongomyniad lance replicas falling from the sky, light spears barrage'],
    ['tyrants_lance', "the tyrant's massive lance strike, overwhelming spear of the queen"],
    ['saviors_vengeance', "the betrayed savior's vengeance, wrath of one killed by those she saved"],
    ['final_collection', 'the final debt collection, the queen claims everything owed, ultimate levy'],
    ['from_the_worlds_end', "returning from the World's End, rising from death, the queen who died countless times"],
    ['last_resort', 'a last resort trump card, the final desperate measure, hotel resort vacation joke'],
    ['winter_coronation', 'the coronation of the Winter Queen, crowning ceremony of eternal winter'],
    ['extraordinary_tax', 'an extraordinary emergency tax levied on everyone, mass tribute collection'],
    ['under_the_world_tree', 'studying or dreaming under the great world tree, library of the ash tree'],
    ['vivians_gift', 'Vivian the Lady of the Lake gifting the holy sword, Excalibur rising from the lake'],
    ['hailstorm_wall', 'a wall of hailstorm and ice protecting the queen, frozen barrier'],
    ['a_home_with_morgan', 'a warm domestic home with Morgan, family life with the queen, cozy domestic scene'],
    ['charisma_of_adversity', 'hope and strength in adversity, inspiring the oppressed and wounded'],
    ['curse_of_cernunnos', "the curse of the god Cernunnos, the horned god's eternal blight"],
    ['sovereign_of_two_faces', 'a sovereign with two faces, queen and witch duality, two aspects of one ruler'],
    ['spriggans_vigil', 'Spriggan the giant guardian keeping watch over treasures and walls'],
    ['perpetual_winter', 'perpetual eternal winter covering the land in snow and curse'],
    ['fae_blood_pact', 'a blood pact with the fae, magical contract paid in blood'],
    ['silly_mama', 'Baobhan Sith and her mother Morgan, silly doting mother meme, fae family moment'],
    ['knights_arm', "a glorious knight's sword or weapon materialized from dreams"],
  ]},
]

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
  `You are matching Slay the Spire 2 card themes to official Fate/Grand Order Craft Essence (CE) artwork for a character mod (the character is Morgan, Lostbelt 6 Fairy Queen / Aesc the Rain Witch).

The full CE catalog is a TSV file at ${TSV} with columns: collectionNo <TAB> assetId <TAB> englishName. Use Grep with case-insensitive patterns (-i) to search it by keywords, and Read slices to browse.

For EACH of these cards, pick the best-fitting CE by name/theme (the art gets cropped for the card portrait, so the CE subject must match the card theme).

Cards (file -> theme):
${b.cards.map(c => `- ${c[0]}: ${c[1]}`).join('\n')}

Rules:
- Prefer CEs featuring Morgan, Aesc/Tonelico, the Fairy Knights (Barghest/Melusine/Baobhan Sith), Lostbelt 6 / fae Britain themes, or Arthurian lake/winter imagery when thematically right.
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
