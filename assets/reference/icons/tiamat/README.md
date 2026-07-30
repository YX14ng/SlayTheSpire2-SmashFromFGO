# Tiamat power icon provenance

This directory contains only source references for Beast II Tiamat. Larva/Tiamat 1001600 skill
icons are intentionally excluded because that playable unit is not the target of this mod.

## Direct sources

- `tiamat_femme_fatale_charagraph.png`: Atlas Academy JP CharaGraph 9935400,
  `https://static.atlasacademy.io/JP/CharaGraph/9935400/9935400a.png`.
- `skill_seal.png`: Atlas Academy JP FGO buff icon 512,
  `https://static.atlasacademy.io/JP/BuffIcons/bufficon_512.png`.

## Packaged power icons

- `tiamat_femme_fatale_power.png`: cropped from the official 9935400 idle frame already extracted
  under `Tiamat/TiamatBeast/character/frames_femme/`.
- `tiamat_beast_power.png`: cropped from the official 9935410 idle frame already extracted under
  `Tiamat/TiamatBeast/character/frames_beast/`.
- `skill_seal_power.png`: direct FGO Skill Seal status icon.
- Swarm/nurture powers: official Lahmu imagery from FGOCore.
- Curse, Guts, Bulwark, form-window and NP lockout powers: official FGO status/skill imagery already
  tracked by FGOCore.

The normal and `big/` power paths intentionally carry the same source image, matching the existing
BaseLib resource convention in this repository.
