using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using MegaCrit.Sts2.Core.Nodes.Combat;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;
using OkitaSaber.OkitaSaberCode.Cards.Basic;
using OkitaSaber.OkitaSaberCode.Extensions;
using OkitaSaber.OkitaSaberCode.Relics;

namespace OkitaSaber.OkitaSaberCode.Character;

public class Okita : PlaceholderCharacterModel
{
    public const string CharacterId = "Okita";

    // Rosa sakura del Bakumatsu (los pétalos-estrella, la última primavera).
    public static readonly Color Color = new("e58fae");

    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Feminine;

    // HP = 68 — la MÁS FRÁGIL del roster jugable (DESIGN-OKITA §7; Morgan 78, Artoria 70).
    // Su END E y death rate 35% son canon: compensa con Intangible puntual, Alzarse y velocidad,
    // NO inflando HP.
    public override int StartingHp => 68;

    // Mazo inicial 10 (DESIGN-OKITA §5.1): 4 Golpe + 4 Defensa + 2 FIRMAS. Las firmas enseñan
    // las dos identidades temporales (atacar = ★ vía la *Ráfaga Shukuchi; defender = respirar vía
    // Recuperar el Aliento) y el mazo gana el acto 1 sin el motor armado. Las cartas-comando QQABB
    // (Quick/Arts/Buster) y el resto del pool llegan en la fase de cartas.
    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<Strike>(), ModelDb.Card<Strike>(), ModelDb.Card<Strike>(), ModelDb.Card<Strike>(),
        ModelDb.Card<Defend>(), ModelDb.Card<Defend>(), ModelDb.Card<Defend>(), ModelDb.Card<Defend>(),
        ModelDb.Card<Shukuchi>(),
        ModelDb.Card<CatchYourBreath>()
    ];

    // Starters: motor Haori Asagi, vínculo de la Primera Unidad y almacén de nivel de NP/dupes.
    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<HaoriAsagi>(),
        ModelDb.Relic<BondFirstUnit>(),
        ModelDb.Relic<MenkyoKaiden>()
    ];

    public override CardPoolModel CardPool => ModelDb.CardPool<OkitaCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<OkitaRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<OkitaPotionPool>();

    // Modelo de batalla oficial 102710. La forma final comparte este set para mantener estable
    // el consumo de memoria cuando hay varios personajes FGO en combate.
    public override string CustomVisualPath => $"{MainFile.ResPath}/character/okita_visuals.tscn";

    // Robustez anti-conflicto: construye las visuals directo desde la factory de BaseLib, en vez
    // del Instantiate<NCreatureVisuals>() del juego que depende del patch global de conversion.
    // Inmune al clobber de otra BaseLib forkeada (p. ej. figure_Saya). Sin escena propia => null
    // => comportamiento original. Ver docs/FINDINGS.md.
    public override NCreatureVisuals? CreateCustomVisuals()
        => string.IsNullOrEmpty(CustomVisualPath)
            ? null
            : NodeFactory<NCreatureVisuals>.CreateFromScene(CustomVisualPath);

    // Multiplayer/perf: precargar los frames pesados en el set residente de la run; si no, Godot
    // los carga sincrónicamente al entrar a combate (freeze -> timeout/desconexión en MP).
    protected override IEnumerable<string> ExtraAssetPaths =>
    [
        $"{MainFile.ResPath}/character/okita_frames.tres",
    ];
    public override string CustomMerchantAnimPath => $"{MainFile.ResPath}/character/okita_merchant.tscn";
    public override string CustomRestSiteAnimPath => $"{MainFile.ResPath}/character/okita_rest.tscn";
    public override string CustomCharacterSelectBg => $"{MainFile.ResPath}/character/okita_select_bg.tscn";

    public override Control CustomIcon
    {
        get
        {
            var icon = NodeFactory<Control>.CreateFromResource(CustomIconTexturePath);
            icon.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
            return icon;
        }
    }

    // NOTA: usan los placeholders del scaffold (sufijo _char_name) hasta el pase de arte real (render
    // del modelo 102710 + charagraph). Renombrar cuando llegue el arte (WORKFLOW-FGO §6).
    public override string CustomIconTexturePath => "character_icon_char_name.png".CharacterUiPath();
    public override string CustomCharacterSelectIconPath => "char_select_char_name.png".CharacterUiPath();
    public override string CustomCharacterSelectLockedIconPath => "char_select_char_name_locked.png".CharacterUiPath();
    public override string CustomMapMarkerPath => "map_marker_char_name.png".CharacterUiPath();
}
