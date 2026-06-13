using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;
using SiegfriedSaber.SiegfriedSaberCode.Cards.Basic;
using SiegfriedSaber.SiegfriedSaberCode.Extensions;
using SiegfriedSaber.SiegfriedSaberCode.Relics;

namespace SiegfriedSaber.SiegfriedSaberCode.Character;

public class Siegfried : PlaceholderCharacterModel
{
    public const string CharacterId = "Siegfried";

    // Azul acero del Sanador de Dragones (armadura) con un dejo de sangre de dragón.
    public static readonly Color Color = new("37597a");

    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Masculine;

    // HP = 80 — ancla Ironclad exacta (DESIGN-SIEGFRIED §5, P1). La identidad bruiser la paga la
    // Sangre de Dragón inicial 2 (Hoja de Tilo), NO HP extra.
    public override int StartingHp => 80;

    // PROVISIONAL: mazo inicial mínimo jugable (5 Golpe Buster + 5 Defensa) mientras se construyen
    // las firmas y el NP Balmung. Objetivo de diseño = QAABB fiel, ~52 daño/ciclo (§5). El "Golpe"
    // vive DENTRO del mazo (P6 Morgan, compat del tag Strike).
    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<Strike>(), ModelDb.Card<Strike>(), ModelDb.Card<Strike>(), ModelDb.Card<Strike>(), ModelDb.Card<Strike>(),
        ModelDb.Card<Defend>(), ModelDb.Card<Defend>(), ModelDb.Card<Defend>(), ModelDb.Card<Defend>(), ModelDb.Card<Defend>()
    ];

    public override IReadOnlyList<RelicModel> StartingRelics => [ModelDb.Relic<LindenLeaf>()];

    public override CardPoolModel CardPool => ModelDb.CardPool<SiegfriedCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<SiegfriedRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<SiegfriedPotionPool>();

    // Visuales: placeholder por ahora (el modelo de batalla 100800 espera el export GUI del bundle;
    // DESIGN-SIEGFRIED §11). El render directo (modelo único) vendrá tras el export.
    public override string CustomVisualPath => $"{MainFile.ResPath}/character/siegfried_visuals.tscn";
    public override string CustomCharacterSelectBg => $"{MainFile.ResPath}/character/siegfried_select_bg.tscn";

    public override Control CustomIcon
    {
        get
        {
            var icon = NodeFactory<Control>.CreateFromResource(CustomIconTexturePath);
            icon.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
            return icon;
        }
    }

    public override string CustomIconTexturePath => "char_icon.png".CharacterUiPath();
    public override string CustomCharacterSelectIconPath => "char_select.png".CharacterUiPath();
    public override string CustomCharacterSelectLockedIconPath => "char_select_locked.png".CharacterUiPath();
    public override string CustomMapMarkerPath => "map_marker.png".CharacterUiPath();
}
