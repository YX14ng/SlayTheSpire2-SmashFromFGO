using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
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

    // Starter = el Haori Asagi (el motor: atacar→★, criticar→NP, y fija el Aliento inicial 6).
    public override IReadOnlyList<RelicModel> StartingRelics => [ModelDb.Relic<HaoriAsagi>()];

    public override CardPoolModel CardPool => ModelDb.CardPool<OkitaCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<OkitaRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<OkitaPotionPool>();

    // Modelo de batalla único 102710 (traje blanco); el clímax «Flor del Bakumatsu» hace el swap a
    // 102720 (haori asagi) vía FormVisuals (DESIGN-OKITA §3.4/§8). El render directo vendrá tras el export.
    public override string CustomVisualPath => $"{MainFile.ResPath}/character/okita_visuals.tscn";
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
