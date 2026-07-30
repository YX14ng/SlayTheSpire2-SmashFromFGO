using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using Godot;
using KagetoraLancer.KagetoraLancerCode.Cards.Basic;
using KagetoraLancer.KagetoraLancerCode.Extensions;
using KagetoraLancer.KagetoraLancerCode.Relics;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace KagetoraLancer.KagetoraLancerCode.Character;

public sealed class Kagetora : PlaceholderCharacterModel
{
    public const string CharacterId = "Kagetora";
    public static readonly Color Color = new("d9e9f5");

    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Feminine;
    public override int StartingHp => 72;
    public override int StartingGold => 99;

    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<Buster>(), ModelDb.Card<Buster>(),
        ModelDb.Card<Arts>(), ModelDb.Card<Arts>(),
        ModelDb.Card<Quick>(),
        ModelDb.Card<Defender>(), ModelDb.Card<Defender>(), ModelDb.Card<Defender>(),
        ModelDb.Card<FortuneIsInHeaven>(), ModelDb.Card<IncarnationOfBishamonten>()
    ];

    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<JeweledPagodaOfBishamonten>(),
        ModelDb.Relic<OathOfEchigo>(),
        ModelDb.Relic<RecordOfEightFormations>()
    ];

    public override CardPoolModel CardPool => ModelDb.CardPool<KagetoraCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<KagetoraRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<KagetoraPotionPool>();

    /// <summary>Animaciones oficiales de las formas Nagao Kagetora y Uesugi Kenshin.</summary>
    public override string CustomVisualPath => $"{MainFile.ResPath}/character/kagetora_visuals.tscn";

    // Evita depender del parche global de conversión de BaseLib, que otro mod puede sobrescribir.
    public override NCreatureVisuals? CreateCustomVisuals()
        => string.IsNullOrEmpty(CustomVisualPath)
            ? null
            : NodeFactory<NCreatureVisuals>.CreateFromScene(CustomVisualPath);

    // Mantiene ambas formas en el conjunto de recursos de la run. FormVisuals limita la carga
    // residente por personaje y, en cooperativo, aplica el cambio de forma de manera asincrona.
    protected override IEnumerable<string> ExtraAssetPaths =>
    [
        $"{MainFile.ResPath}/character/kagetora_frames.tres",
        $"{MainFile.ResPath}/character/kenshin_frames.tres",
    ];

    public override string CustomCharacterSelectBg => $"{MainFile.ResPath}/character/kagetora_select_bg.tscn";
    public override string CustomMerchantAnimPath => $"{MainFile.ResPath}/character/kagetora_merchant.tscn";
    public override string CustomRestSiteAnimPath => $"{MainFile.ResPath}/character/kagetora_rest.tscn";

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
