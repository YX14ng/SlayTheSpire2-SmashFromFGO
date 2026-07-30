using AstolfoRider.AstolfoRiderCode.Cards.Basic;
using AstolfoRider.AstolfoRiderCode.Extensions;
using AstolfoRider.AstolfoRiderCode.Relics;
using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace AstolfoRider.AstolfoRiderCode.Character;

public sealed class Astolfo : PlaceholderCharacterModel
{
    public const string CharacterId = "AstolfoRider";
    public static readonly Color Color = new("f07bb9");
    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Neutral;
    public override int StartingHp => 72;
    public override int StartingGold => 99;

    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<Quick>(), ModelDb.Card<Quick>(), ModelDb.Card<Quick>(),
        ModelDb.Card<Arts>(), ModelDb.Card<Buster>(),
        ModelDb.Card<Defender>(), ModelDb.Card<Defender>(),
        ModelDb.Card<Defender>(), ModelDb.Card<Defender>(),
        ModelDb.Card<PaladinsHunch>()
    ];

    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<ReasonEvaporatedRelic>(),
        ModelDb.Relic<OathOfTheJoyfulPaladin>(),
        ModelDb.Relic<BookOfTheForgottenName>()
    ];

    public override CardPoolModel CardPool => ModelDb.CardPool<AstolfoCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<AstolfoRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<AstolfoPotionPool>();
    public override string CustomVisualPath => $"{MainFile.ResPath}/character/astolfo_visuals.tscn";
    public override NCreatureVisuals? CreateCustomVisuals() =>
        NodeFactory<NCreatureVisuals>.CreateFromScene(CustomVisualPath);
    protected override IEnumerable<string> ExtraAssetPaths =>
        [$"{MainFile.ResPath}/character/astolfo_frames.tres"];
    public override string CustomCharacterSelectBg => $"{MainFile.ResPath}/character/astolfo_select_bg.tscn";
    public override string CustomMerchantAnimPath => $"{MainFile.ResPath}/character/astolfo_merchant.tscn";
    public override string CustomRestSiteAnimPath => $"{MainFile.ResPath}/character/astolfo_rest.tscn";

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
