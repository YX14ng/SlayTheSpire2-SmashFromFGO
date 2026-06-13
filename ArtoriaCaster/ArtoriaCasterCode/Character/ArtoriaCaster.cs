using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;
using ArtoriaCaster.ArtoriaCasterCode.Cards.Basic;
using ArtoriaCaster.ArtoriaCasterCode.Extensions;
using ArtoriaCaster.ArtoriaCasterCode.Relics;

namespace ArtoriaCaster.ArtoriaCasterCode.Character;

public class ArtoriaCaster : PlaceholderCharacterModel
{
    public const string CharacterId = "ArtoriaCaster";

    // Blanco-azul cielo con dorado: la Niña de la Profecía.
    public static readonly Color Color = new("9fd4f0");

    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Feminine;
    // HP de ecosistema (revertido el +10 del re-stat de JeanneAlter): caster de soporte.
    // Valor puente; el rediseño desde 0 lo confirma.
    public override int StartingHp => 74;

    // Mazo QAABB del rediseño v2 (paridad con Mash/Morgan): la economía completa
    // desde el turno 1, sesgado a soporte (2 Defender) con las 2 firmas-cambio.
    public override IEnumerable<CardModel> StartingDeck => [
        ModelDb.Card<StrikeArtoria>(),
        ModelDb.Card<StrikeArtoria>(),
        ModelDb.Card<DefendArtoria>(),
        ModelDb.Card<DefendArtoria>(),
        ModelDb.Card<BusterArtoria>(),
        ModelDb.Card<ArtsArtoria>(),
        ModelDb.Card<ArtsArtoria>(),
        ModelDb.Card<QuickArtoria>(),
        ModelDb.Card<SummerOutburst>(),
        ModelDb.Card<SongOfProphecy>()
    ];

    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<SelectionStaff>(),
        ModelDb.Relic<ArtoriaBond>(),
        ModelDb.Relic<ProphecyChildTalisman>()
    ];

    /// <summary>
    /// v2 combat visual: real animated frames of the three FGO battle models
    /// (504520 Castoria / 704710 Summer Berserker / 704720 Avalon), rendered with
    /// tools/render_project and swapped by FGOCore FormVisuals.
    /// </summary>
    public override string CustomVisualPath => $"{MainFile.ResPath}/character/artoria_visuals.tscn";

    public override string CustomCharacterSelectBg => $"{MainFile.ResPath}/character/artoria_select_bg.tscn";

    public override string CustomMerchantAnimPath => $"{MainFile.ResPath}/character/artoria_merchant.tscn";
    public override string CustomRestSiteAnimPath => $"{MainFile.ResPath}/character/artoria_rest.tscn";

    public override CardPoolModel CardPool => ModelDb.CardPool<ArtoriaCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<ArtoriaRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<ArtoriaPotionPool>();

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
