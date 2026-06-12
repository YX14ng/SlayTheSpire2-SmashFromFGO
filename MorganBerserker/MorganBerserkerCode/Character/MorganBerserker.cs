using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;
using MorganBerserker.MorganBerserkerCode.Cards.Basic;
using MorganBerserker.MorganBerserkerCode.Extensions;
using MorganBerserker.MorganBerserkerCode.Relics;

namespace MorganBerserker.MorganBerserkerCode.Character;

public class MorganBerserker : PlaceholderCharacterModel
{
    public const string CharacterId = "MorganBerserker";

    public static readonly Color Color = new("8fb4e8");

    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Feminine;
    public override int StartingHp => 78;

    /// <summary>
    /// Rediseño v2, parche P6 — mazo inicial fiel al command deck REAL de Morgan en
    /// FGO (QAABB = 1 Quick, 2 Arts, 2 Buster): codifica las 3 economías
    /// (daño / NP / estrellas) desde el turno 1. El StrikeMorgan restante conserva
    /// el tag Strike vivo en el mazo (compat con eventos vanilla).
    /// </summary>
    public override IEnumerable<CardModel> StartingDeck => [
        ModelDb.Card<BusterMorgan>(),
        ModelDb.Card<BusterMorgan>(),
        ModelDb.Card<ArtsMorgan>(),
        ModelDb.Card<ArtsMorgan>(),
        ModelDb.Card<QuickMorgan>(),
        ModelDb.Card<StrikeMorgan>(),
        ModelDb.Card<DefendMorgan>(),
        ModelDb.Card<DefendMorgan>(),
        ModelDb.Card<LanceOfTheWorldsEnd>(),
        ModelDb.Card<QueensMandate>()
    ];

    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<QueensScepter>(),
        ModelDb.Relic<MorganBond>(),
        ModelDb.Relic<MorganSummonSeal>()
    ];

    /// <summary>
    /// v1 combat visual: static charagraph per form (one-frame SpriteFrames swapped
    /// by FGOCore FormVisuals). The animated puppets (704020/505320/704030) come in v2
    /// after the manual AssetStudio export — same pipeline as Mash.
    /// </summary>
    public override string CustomVisualPath => $"{MainFile.ResPath}/character/morgan_visuals.tscn";

    public override string CustomCharacterSelectBg => $"{MainFile.ResPath}/character/morgan_select_bg.tscn";

    /// <summary>Merchant/rest-site visuals (static, replacing the Ironclad placeholders).</summary>
    public override string CustomMerchantAnimPath => $"{MainFile.ResPath}/character/morgan_merchant.tscn";
    public override string CustomRestSiteAnimPath => $"{MainFile.ResPath}/character/morgan_rest.tscn";

    public override CardPoolModel CardPool => ModelDb.CardPool<MorganCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<MorganRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<MorganPotionPool>();

    public override Control CustomIcon
    {
        get
        {
            var icon = NodeFactory<Control>.CreateFromResource(CustomIconTexturePath);
            icon.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
            return icon;
        }
    }
    public override string CustomIconTexturePath => "character_icon_char_name.png".CharacterUiPath();
    public override string CustomCharacterSelectIconPath => "char_select_char_name.png".CharacterUiPath();
    public override string CustomCharacterSelectLockedIconPath => "char_select_char_name_locked.png".CharacterUiPath();
    public override string CustomMapMarkerPath => "map_marker_char_name.png".CharacterUiPath();
}
