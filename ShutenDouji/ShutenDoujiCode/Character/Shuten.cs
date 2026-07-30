using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using ShutenDouji.ShutenDoujiCode.Cards.Basic;
using ShutenDouji.ShutenDoujiCode.Extensions;
using ShutenDouji.ShutenDoujiCode.Relics;

namespace ShutenDouji.ShutenDoujiCode.Character;

public sealed class Shuten : PlaceholderCharacterModel
{
    public const string CharacterId = "ShutenDouji";
    public static readonly Color Color = new("8f3ca8");

    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Feminine;
    public override int StartingHp => 68;
    public override int StartingGold => 99;

    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<Buster>(), ModelDb.Card<Buster>(),
        ModelDb.Card<Arts>(), ModelDb.Card<Arts>(),
        ModelDb.Card<Quick>(),
        ModelDb.Card<Defender>(), ModelDb.Card<Defender>(),
        ModelDb.Card<Defender>(), ModelDb.Card<Defender>(),
        ModelDb.Card<FruityWineAroma>()
    ];

    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<ScarletGourd>(),
        ModelDb.Relic<BanquetOath>(),
        ModelDb.Relic<SeveredHeadMemory>()
    ];

    public override CardPoolModel CardPool => ModelDb.CardPool<ShutenCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<ShutenRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<ShutenPotionPool>();

    /// <summary>Animaciones oficiales del modelo Assassin de Shuten-Douji.</summary>
    public override string CustomVisualPath => $"{MainFile.ResPath}/character/shuten_visuals.tscn";

    public override NCreatureVisuals? CreateCustomVisuals()
        => NodeFactory<NCreatureVisuals>.CreateFromScene(CustomVisualPath);

    protected override IEnumerable<string> ExtraAssetPaths =>
    [
        $"{MainFile.ResPath}/character/shuten_frames.tres",
    ];

    public override string CustomCharacterSelectBg => $"{MainFile.ResPath}/character/shuten_select_bg.tscn";
    public override string CustomMerchantAnimPath => $"{MainFile.ResPath}/character/shuten_merchant.tscn";
    public override string CustomRestSiteAnimPath => $"{MainFile.ResPath}/character/shuten_rest.tscn";

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
