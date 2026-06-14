using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;
using TiamatBeast.TiamatCode.Cards.Basic;
using TiamatBeast.TiamatCode.Extensions;
using TiamatBeast.TiamatCode.Relics;

namespace TiamatBeast.TiamatCode.Character;

public class Tiamat : PlaceholderCharacterModel
{
    public const string CharacterId = "Tiamat";

    // Púrpura-abismo / lodo negro del Mar de Vida.
    public static readonly Color Color = new("6a3d8f");

    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Feminine;
    public override int StartingHp => 73;

    // Mazo inicial: 4 Marea + 4 Caparazón + 2 firmas (Engendrar / Lodo Negro). Gana el acto 1
    // sin el motor armado; las firmas enseñan el enjambre y la Marea.
    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<ChaosTide>(), ModelDb.Card<ChaosTide>(), ModelDb.Card<ChaosTide>(), ModelDb.Card<ChaosTide>(),
        ModelDb.Card<Carapace>(), ModelDb.Card<Carapace>(), ModelDb.Card<Carapace>(), ModelDb.Card<Carapace>(),
        ModelDb.Card<SpawnLahmu>(),
        ModelDb.Card<BlackMud>()
    ];

    public override IReadOnlyList<RelicModel> StartingRelics => [ModelDb.Relic<SeaOfLifeWomb>()];

    public override CardPoolModel CardPool => ModelDb.CardPool<TiamatCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<TiamatRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<TiamatPotionPool>();

    // Visuales: placeholders por ahora (faltan el render de las formas humano/Bestia y los
    // assets de selección — pase de arte; ver DESIGN-TIAMAT.md, riesgo de sprite de Laḫmu).
    public override string CustomVisualPath => $"{MainFile.ResPath}/character/tiamat_visuals.tscn";
    public override string CustomCharacterSelectBg => $"{MainFile.ResPath}/character/tiamat_select_bg.tscn";

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
