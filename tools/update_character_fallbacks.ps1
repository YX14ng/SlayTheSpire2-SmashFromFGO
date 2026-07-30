param([string]$Root = 'F:\Programs\SlayTheSpire2-SmashFromFGO')

$ErrorActionPreference = 'Stop'
$items = @(
    @{ Project='MashShielder'; Resource='MashShielder'; Card='strike_mash'; Power='lord_camelot_charge_power'; Relic='a_team_diary' },
    @{ Project='MorganBerserker'; Resource='MorganBerserker'; Card='strike_morgan'; Power='fairy_queen_form_power'; Relic='queens_scepter' },
    @{ Project='ArtoriaCaster'; Resource='ArtoriaCaster'; Card='strike_artoria'; Power='avalon_form_power'; Relic='selection_staff' },
    @{ Project='MordredSaber'; Resource='MordredSaber'; Card='strike'; Power='masked_knight_form_power'; Relic='oath_of_the_knight_of_treachery' },
    @{ Project='GilgameshArcher'; Resource='GilgameshArcher'; Card='strike'; Power='divinity_power'; Relic='bab_ilu' },
    @{ Project='OkitaSaber'; Resource='OkitaSaber'; Card='strike'; Power='sword_genius_power'; Relic='bond_first_unit' },
    @{ Project='OberonPretender'; Resource='OberonPretender'; Card='strike'; Power='storybook_king_power'; Relic='dream_contract' },
    @{ Project='SiegfriedSaber'; Resource='SiegfriedSaber'; Card='strike'; Power='weight_of_expectations_power'; Relic='das_rheingold' },
    @{ Project='Tiamat'; Resource='TiamatBeast'; Card='abyssal_chrysalis'; Power='tiamat_femme_fatale_power'; Relic='sea_of_life_womb' }
)

foreach ($item in $items) {
    $images = Join-Path $Root "$($item.Project)\$($item.Resource)\images"
    Copy-Item "$images\card_portraits\$($item.Card).png" "$images\card_portraits\card.png" -Force
    Copy-Item "$images\card_portraits\big\$($item.Card).png" "$images\card_portraits\big\card.png" -Force
    Copy-Item "$images\powers\$($item.Power).png" "$images\powers\power.png" -Force
    Copy-Item "$images\powers\big\$($item.Power).png" "$images\powers\big\power.png" -Force
    Copy-Item "$images\relics\$($item.Relic).png" "$images\relics\relic.png" -Force
    Copy-Item "$images\relics\big\$($item.Relic).png" "$images\relics\big\relic.png" -Force
    Copy-Item "$images\relics\$($item.Relic)_outline.png" "$images\relics\relic_outline.png" -Force
}

Write-Output "Updated character-specific fallbacks for $($items.Count) mods."
